using API.BackgroundJobs;
using API.Hubs;
using API.Middleware;
using API.Services;
using Application.BackgroundJobs;
using Application.Behaviors;
using Application.Features.Ticket.Queries.GetTicketById;
using Application.interfaces;
using Application.Mapping;
using Application.Services.Imp;
using Application.Services.Interface;
using Application.Settings;
using Application.Validators;
using Domain.Constants;
using Domain.Entities;
using FluentValidation;
using Hangfire;
using Infrastructure.ApplicationDBContext;
using Infrastructure.Repository;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using Serilog;

namespace API
{
    public  class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                .CreateLogger();

            builder.Host.UseSerilog();

            // Add services to the container.
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<TicketProfile>();
            });
            builder.Services.AddValidatorsFromAssemblyContaining<CreateTicketValidator>();

            builder.Services.AddTransient(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationBehavior<,>));
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddMediatR(cfg =>
                    cfg.RegisterServicesFromAssembly(typeof(GetTicketByIdHandler).Assembly));
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IAuthService, AuthServices>();
            builder.Services.AddScoped<IAuthRepo, AuthRepo>();
            builder.Services.AddScoped<ITicketService, TicketServices>();
            builder.Services.AddScoped<ITicketRepository, TicketRepository>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<ICriticalTicketJob,CriticalTicketJob>();
            builder.Services.Configure<JwtSettings>(
                builder.Configuration.GetSection("Jwt"));
            builder.Services.AddSignalR();
            builder.Services.AddDbContext<DBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnectionString")
            ));
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<DBContext>()
                .AddDefaultTokenProviders();

            var jwtSettings = builder.Configuration
                            .GetSection("Jwt")
                            .Get<JwtSettings>()
                            ?? throw new InvalidOperationException("JWT settings not found.");
            builder.Services
                    .AddAuthentication(options =>
                    {
                        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                    })
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,

                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(jwtSettings.Key)
                            ),

                            ValidateIssuer = true,
                            ValidIssuer = jwtSettings.Issuer,

                            ValidateAudience = true,
                            ValidAudience = jwtSettings.Audience,

                            ValidateLifetime = true
                        };
                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var accessToken = context.Request.Query["access_token"];
                                var path = context.HttpContext.Request.Path;
                                if (!string.IsNullOrEmpty(accessToken) &&
                                    (path.StartsWithSegments("/hubs/notifications")))
                                {
                                    context.Token = accessToken;
                                }
                                return Task.CompletedTask;
                            }
                        };
                    });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token."
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            builder.Services.AddControllers();
            builder.Services.AddHangfire(config =>
            {
                config.UseSqlServerStorage(
                    builder.Configuration.GetConnectionString("defaultConnectionString"));
            });

            builder.Services.AddHangfireServer();
            var app = builder.Build();
            
            app.UseSerilogRequestLogging();

            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseHangfireDashboard("/hangfire");
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider
                    .GetRequiredService<RoleManager<IdentityRole>>();

                var roles = new[]
                {
                    Roles.Employee,
                    Roles.Agent,
                    Roles.Admin
                };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Ticket API v1");
                });
            }

            RecurringJob.AddOrUpdate<ICriticalTicketJob>(
                    "critical-ticket-notification",
                    job => job.NotifyAdminsAboutCriticalTicketsAsync(),
                    Cron.Daily(0));
            app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();

            app.MapControllers();
            app.MapHub<NotificationHub>("/hubs/notifications");

            app.Run();
        }
    }
}

/*
 * {
  "email": "Menna@example.com",
  "password": "Menna@123"
    }
{
  "fullName": "Agent",
  "email": "agent@example.com",
  "password": "Agent@123"
}
{
  "fullName": "Agent2",
  "email": "agent2@example.com",
  "password": "Agent2@123"
}
{
  "fullName": "Admin",
  "email": "Admin@example.com",
  "password": "Admin@123"
}
 */

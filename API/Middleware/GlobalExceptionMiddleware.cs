using Application.Exceptions;
using FluentValidation;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(
    HttpContext context,
    Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = exception switch
            {
                ValidationException => (int)HttpStatusCode.BadRequest,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                ForbiddenException => (int)HttpStatusCode.Forbidden,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                InvalidOperationException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = statusCode;
            
            if (statusCode == (int)HttpStatusCode.InternalServerError)
            {
                _logger.LogError(exception, "An unexpected error occurred.");
            }
            else if (statusCode == (int)HttpStatusCode.Unauthorized || statusCode == (int)HttpStatusCode.Forbidden)
            {
                _logger.LogWarning("Authorization/Authentication error: {Message}", exception.Message);
            }
            else if (exception is ValidationException)
            {
                _logger.LogWarning("Validation failed: {Message}", exception.Message);
            }
            else
            {
                _logger.LogWarning(exception, "An expected error occurred: {Message}", exception.Message);
            }

            object response;

            if (exception is ValidationException validationException)
            {
                var errors = validationException.Errors
                    .GroupBy(error => error.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(error => error.ErrorMessage).ToArray()
                    );

                response = new
                {
                    statusCode,
                    message = "Validation failed.",
                    errors
                };
            }
            else
            {
                response = new
                {
                    statusCode,
                    message = exception.Message
                };
            }

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}
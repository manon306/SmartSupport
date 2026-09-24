# SmartSupport

## Overview
SmartSupport is a modern Support Ticketing Web API built with ASP.NET Core 9. It provides a robust backend for submitting, tracking, and resolving support requests. It utilizes Clean Architecture principles to separate concerns, ensuring scalability and maintainability.

## Features
- **Ticket Management:** Create, read, update, delete, and assign support tickets.
- **Status Workflows:** Built-in validation for allowed ticket status transitions (Open → InProgress → Resolved → Closed).
- **Authentication & Authorization:** Secure JWT-based authentication with refresh tokens and role-based access control.
- **Background Processing:** Daily Hangfire background jobs to alert administrators of critical unresolved tickets.
- **Real-time Notifications:** Real-time updates delivered to clients via SignalR using WebSockets.
- **Structured Logging:** Comprehensive Serilog implementation logging to both console and rolling files.

## Architecture
This project follows **Clean Architecture** patterns, divided into the following layers:

1. **Domain:** Contains core enterprise logic and types, such as Entities (`Ticket`, `ApplicationUser`, `Comment`, `RefreshToken`), Enums, and Constants (`Roles`).
2. **Application:** Contains business logic, interfaces, CQRS Commands/Queries, MediatR handlers, and FluentValidation rules. *Depends only on Domain.*
3. **Infrastructure:** Contains database access implementation using Entity Framework Core and external service integrations. *Depends on Application and Domain.*
4. **API:** The presentation layer containing ASP.NET Core Controllers, Middleware, SignalR Hubs, and Dependency Injection configurations. *Depends on Application and Infrastructure.*

**Dependency Rule:** Inner layers never depend on outer layers.

## Technologies
| Technology | Purpose |
|------------|---------|
| **ASP.NET Core 9 Web API** | Core framework |
| **Entity Framework Core** | ORM for database access |
| **SQL Server** | Primary relational database |
| **ASP.NET Core Identity** | User and role management |
| **JWT** | Secure, stateless authentication |
| **MediatR (CQRS)** | Decoupling requests from their handlers |
| **FluentValidation** | Request validation pipeline |
| **Hangfire** | Background job scheduling and processing |
| **SignalR** | Real-time WebSocket notifications |
| **Serilog** | Structured application and request logging |
| **AutoMapper** | Object-to-object mapping (DTOs to Entities) |

## Authentication & Authorization
The API uses JSON Web Tokens (JWT) for authentication. 

- **Registration (`/api/Auth/register`):** Automatically assigns the `Employee` role to new users.
- **Login (`/api/Auth/login`):** Validates credentials and returns an Access Token and a Refresh Token (valid for 7 days).
- **Refresh Tokens (`/api/Auth/refresh-token`):** Securely exchange a valid refresh token for a newly issued access token and a new refresh token, without requiring the user to log in again.

**Roles:**
- **`Employee`:** Can create tickets and view/update only the tickets they created.
- **`Agent`:** Can view all tickets, update ticket priorities, and change the status of tickets assigned to them.
- **`Admin`:** Has full control over the system, can view all tickets, delete tickets, and assign tickets to Agents.

## CQRS & MediatR
The application state mutations (Commands) and data retrievals (Queries) are strictly separated using MediatR.
- **Commands:** `CreateTicket`, `UpdateTicket`, `DeleteTicket`, `ChangeStatus`, `AssignTicket`.
- **Queries:** `GetTicketById`, `GetTickets` (supports filtering and pagination).

## Validation & Error Handling
- **FluentValidation:** Command and Query requests are validated using FluentValidation rules, which are integrated into the MediatR pipeline via a custom `ValidationBehavior`.
- **Global Exception Middleware:** A custom middleware (`GlobalExceptionMiddleware`) catches all unhandled exceptions and formats a consistent JSON error response.
  - Returns `400 Bad Request` for `ValidationException` and `InvalidOperationException`.
  - Returns `401 Unauthorized` for `UnauthorizedAccessException`.
  - Returns `403 Forbidden` for `ForbiddenException`.
  - Returns `404 Not Found` for `KeyNotFoundException`.
  - Returns `500 Internal Server Error` for unexpected issues.

## Ticket Management
The core entity is the `Ticket`. Business rules strictly govern ticket operations:
- A ticket starts as `Open`.
- **Valid Status Transitions:** `Open` → `InProgress` → `Resolved` → `Closed`.
- Employees cannot change ticket statuses directly.
- Agents can only change the status of tickets explicitly assigned to them.
- A ticket's priority cannot be set to Critical unless the ticket is already assigned to an Agent.
- Closed tickets cannot be updated.

## Hangfire
Hangfire is configured using SQL Server storage. It handles background tasks independently of HTTP requests.
- **Recurring Job:** `critical-ticket-notification` runs daily.
- **Behavior:** It queries the database for unresolved tickets with a `Critical` priority. If found, it fetches all Admin users and triggers a `NotificationService` call to alert them.
- **Dashboard:** Available at `/hangfire`. (Note: Currently accessible without authorization).

## SignalR
Real-time notifications are handled via ASP.NET Core SignalR.
- **Endpoint:** `/hubs/notifications`
- **Authentication:** Clients must provide their JWT in the `access_token` query string parameter when establishing the WebSocket connection.
- **Usage:** Used primarily by the Hangfire background job to send targeted real-time critical ticket alerts directly to specific connected Admin users.

## Logging
Structured logging is implemented via **Serilog**.
- **Outputs:** Logs are written to the Console and to a daily rolling file located in the `Logs/` directory (e.g., `Logs/log-YYYYMMDD.txt`).
- **HTTP Request Logging:** Captures all incoming HTTP requests, paths, methods, status codes, and execution durations.
- **Business Logic Logging:** Tracks key actions such as user logins, ticket creations, status changes, and background job executions.
- **Security:** Sensitive data (passwords, tokens) is never logged. The `Logs/` directory is ignored in Git via `.gitignore`.

## API Endpoints

### Auth
| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/api/Auth/login` | None | Authenticates a user and returns a JWT & Refresh Token |
| POST | `/api/Auth/register` | None | Registers a new Employee user |
| POST | `/api/Auth/refresh-token` | None | Refreshes an expired JWT |
| POST | `/api/Auth/logout` | None | Revokes a refresh token |

### Tickets
| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/api/Ticket` | Required | Retrieves tickets (with filtering support) |
| GET | `/api/Ticket/{id}` | Required | Retrieves a specific ticket |
| POST | `/api/Ticket` | Required | Creates a new ticket |
| PUT | `/api/Ticket/{id}` | Required | Updates title, description, or priority |
| DELETE | `/api/Ticket/{id}` | Required (Admin) | Deletes a ticket |
| PUT | `/api/Ticket/{ticketId}/assign/{agentId}` | Required (Admin) | Assigns a ticket to a specific Agent |
| PUT | `/api/Ticket/{ticketId}/status/{status}` | Required | Advances the status of a ticket |

### Admin
| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/api/Admin/dashboard` | Required (Admin) | Retrieves dashboard statistics (tickets and users) |
| GET | `/api/Admin/users` | Required (Admin) | Retrieves all users and their roles |
| GET | `/api/Admin/users/{userId}` | Required (Admin) | Retrieves a specific user by ID |
| PUT | `/api/Admin/users/{userId}/role` | Required (Admin) | Changes a user's role |
| GET | `/api/Admin/agents` | Required (Admin) | Retrieves all users in the Agent role |

## Database
The project uses **Entity Framework Core** with **SQL Server**.
- **Core Entities:** `Ticket`, `Comment`, `RefreshToken`.
- **Identity Entities:** standard ASP.NET Identity tables for `ApplicationUser` and Roles.
- **Migrations:** Managed via EF Core Tools.

## Project Structure
```text
SmartSupport/
├── API/                   # Controllers, Hubs, Middleware, DI Setup
├── Application/           # CQRS Features, DTOs, Mapping, Services, Validators
├── Domain/                # Entities, Enums, Constants
├── Infrastructure/        # DbContext, Repositories, EF Core Migrations
├── frontend/              # React, Vite, Tailwind CSS SPA
└── SmartSupport.sln
```

## Getting Started

1. **Clone the repository:**
   ```bash
   git clone <repository-url>
   cd SmartSupport
   ```

2. **Configure Database Connection:**
   Update the `defaultConnectionString` in `API/appsettings.json` (or use User Secrets) to point to your local SQL Server.
   ```json
   "ConnectionStrings": {
     "defaultConnectionString": "Server=...;Database=SmartSupport;..."
   }
   ```

3. **Configure JWT Settings:**
   Update the `Jwt` section in `API/appsettings.json` with a strong, secure key.
   ```json
   "Jwt": {
     "Key": "<Your_Super_Secret_Key_Here>",
     "Issuer": "SmartSupport",
     "Audience": "SmartSupportUsers",
     "DurationInMinutes": 60
   }
   ```

4. **Apply Migrations:**
   Run the EF Core migrations to create the database schema.
   ```bash
   dotnet ef database update --project Infrastructure --startup-project API
   ```
   *(Note: The `Program.cs` file is configured to automatically seed the default `Employee`, `Agent`, and `Admin` roles on startup).*

5. **Run the Application:**
   ```bash
   dotnet run --project API
   ```

6. **Access Interfaces:**
   - **Swagger UI:** Navigate to `https://localhost:<port>/swagger` to explore and test the API.
   - **Hangfire Dashboard:** Navigate to `https://localhost:<port>/hangfire`.

## Frontend SPA

The repository includes a modern Single Page Application built to interface with the API.

### Technology Stack
- **React & TypeScript:** Strongly typed UI components.
- **Vite:** Lightning-fast frontend tooling and bundling.
- **Tailwind CSS:** Utility-first CSS framework for rapid styling.
- **React Router:** Declarative routing and role-based protection guards.
- **Axios:** API client configured with HTTP interceptors for automatic token refresh handling.
- **SignalR:** Integration for real-time WebSocket notifications.

### Features
- **Role-based Dashboards:** Dynamic layouts modifying navigation based on the user's role.
- **Employee View:** Manage personal tickets, create new requests.
- **Agent View:** View unassigned/assigned tickets, manage ticket statuses.
- **Admin Dashboard:** Access high-level statistics, manage all tickets globally, and control user roles.
- **State Management:** Context API handling global authentication state transparently.

### Running the Frontend
1. **Navigate to the frontend directory:**
   ```bash
   cd frontend
   ```
2. **Install dependencies:**
   ```bash
   npm install
   ```
3. **Environment Setup:**
   Ensure `frontend/.env` points to your running backend:
   ```env
   VITE_API_BASE_URL=https://localhost:7250/api
   VITE_SIGNALR_HUB_URL=https://localhost:7250/hubs/notifications
   ```
4. **Start the development server:**
   ```bash
   npm run dev
   ```
   The application will be accessible at `http://localhost:5173`.

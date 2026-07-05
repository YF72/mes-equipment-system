# MES Equipment Management System

A learning-focused full-stack portfolio project for MES equipment management.

The project started as a Version 1 MVP for machine CRUD, JWT login, status logs,
Angular UI, and NgRx list loading. Version 2 focuses on making the MVP closer to
a practical internal management system by improving authentication,
configuration, validation, error handling, list querying, and frontend auth flow.
Version 3 focuses on engineering quality, starting with backend service tests
for core MachineService behaviors and regression coverage for status transition logging.

## Features

- Machine CRUD management
- Machine status history logs
- Database-backed JWT login
- Password hashing for user credentials
- Local JWT configuration with .NET User Secrets
- DTO validation for API request boundaries
- Global error handling with ProblemDetails
- Pagination, search, and status filtering for machine list
- Angular route guards and functional auth interceptor
- NgRx Store / Effects / Selectors for machine list state
- Loading, success, and error feedback in the frontend
- MySQL database with Docker Compose
- Swagger API testing
- Backend service tests for core MachineService behaviors

## Tech Stack

### Backend

- ASP.NET Core Web API
- Entity Framework Core
- MySQL
- Docker Compose
- JWT Authentication
- Swagger
- DTO + Service layer
- .NET User Secrets for local development secrets

### Frontend

- Angular 21
- TypeScript
- NgRx 21
- RxJS
- Signals
- Reactive Forms
- Tailwind CSS
- Functional HTTP interceptor
- Route guards

## Project Structure

```text
project-root/
  MesEquipment.sln
  backend/
    MesEquipment.Api/
    MesEquipment.Api.Tests/
  docker-compose.yml
  MesEquipment.Web/
```

## Local Setup

### 1. Start MySQL

From the project root:

```bash
docker compose up -d
```

MySQL runs on local port `3307`.

### 2. Configure Backend Secrets

```bash
cd backend/MesEquipment.Api
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "your-development-secret-key-at-least-32-characters"
dotnet user-secrets set "Jwt:Issuer" "MesEquipment.Api"
dotnet user-secrets set "Jwt:Audience" "MesEquipment.Web"
```

`Jwt:Key` should not be committed to source control. In production, this would
usually come from environment variables, CI/CD secrets, or a secret manager.

### 3. Update Database

```bash
cd backend/MesEquipment.Api
dotnet ef database update
```

### 4. Run Backend API

```bash
cd backend/MesEquipment.Api
dotnet run
```

Swagger:

```text
http://localhost:5264/swagger
```

### 5. Run Frontend

```bash
cd MesEquipment.Web
npm install
npm start
```

Frontend:

```text
http://localhost:4200
```

## Demo Account

```text
Username: admin
Password: password
```

The demo admin account is seeded only for local development.

## Version 2 Improvements

Version 2 turns the original MVP into a more practical portfolio project:

- Moved JWT secret out of `appsettings.json`
- Added `User` entity and `Users` table
- Added password hashing with `PasswordHasher<User>`
- Replaced fixed `admin / password` login with database login
- Added DTO validation
- Added global error handling middleware
- Added paged machine query API
- Added search and status filtering
- Added Angular route guards
- Added 401 handling in the auth interceptor
- Added frontend operation loading, success, and error feedback
- Cleaned up template code and project documentation

## Testing

The backend includes service-level tests for `MachineService`.

Covered behaviors:

- pagination
- keyword search
- status filtering
- create machine
- get machine by id
- delete machine
- status transition logging

Run all .NET tests:

```bash
dotnet test MesEquipment.sln
```

The service tests use EF Core InMemory database, so they can run without MySQL or Docker.

During Version 3, a regression test was added for machine status transitions.
The test caught a bug where `MachineStatusLog` was created but not persisted to
the database. The service was updated to persist the status log when a machine
status changes.

## API Overview

### Auth

```text
POST /api/Auth/login
```

### Machines

```text
GET    /api/Machines?page=1&pageSize=10&keyword=cnc&status=Running
GET    /api/Machines/{id}
POST   /api/Machines
PUT    /api/Machines/{id}
DELETE /api/Machines/{id}
GET    /api/Machines/{id}/status-logs
```

## Demo Script

1. Sign in with the demo account.
2. Show that `/machines` is protected by the route guard.
3. Load the machine list through NgRx.
4. Search, filter by status, and change pages.
5. Create, update, and delete a machine.
6. Show frontend loading, success, and error feedback.
7. Explain that the backend uses DTO validation and global error handling.
8. Explain that JWT secrets are kept out of `appsettings.json` for local development.

## Notes

- Frontend route guards improve user experience, but backend `[Authorize]`
  remains the real security boundary.
- User Secrets are for local development only.
- The project intentionally uses a simple demo admin seed instead of a public
  registration flow because MES systems are usually internal business tools.

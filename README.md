# MES Equipment Management System

[![CI](https://github.com/YF72/mes-equipment-system/actions/workflows/ci.yml/badge.svg)](https://github.com/YF72/mes-equipment-system/actions/workflows/ci.yml)

A full-stack MES equipment management system built to demonstrate pragmatic architecture evolution from MVP features to production-oriented engineering practices.

The system manages equipment records, machine status history, JWT-based authentication,
and an Angular management UI with NgRx-driven list state. The project has evolved
from core CRUD workflows into a more production-oriented architecture with
database-backed authentication, password hashing, DTO validation, global error
handling, paginated querying, route guards, automated backend tests, API
integration tests, and GitHub Actions CI.

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
- OpenAPI contract-based Angular API client generation with NSwag
- Application-level service adapter around generated API clients
- DTO mapper layer to isolate generated API contracts from frontend app models

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
- NSwag for OpenAPI TypeScript client generation

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
- Generated Angular API client from backend OpenAPI contract

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

The project uses Docker Compose to run MySQL for local development. Docker Compose
provides local defaults, and values can be overridden with a local `.env` file.

Create a local `.env` file from the example:

```bash
cp .env.example .env
```

Start MySQL:

```bash
docker compose up -d
```

Check container health:

```bash
docker ps
```

The MySQL container should show `healthy`.

MySQL runs on local port `3307` by default.

Local defaults are defined in `.env.example`:

```text
MYSQL_DATABASE=mes_equipment_db
MYSQL_USER=mes_user
MYSQL_PASSWORD=mes_password
MYSQL_PORT=3307
```

These defaults are for local development only. Production-like deployments should
provide secrets explicitly through environment variables, CI/CD secrets, or a
secret manager.

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

### Generate Angular API Client

The Angular API client is generated from the backend Swagger/OpenAPI contract with NSwag.

Start the backend first:

```bash
cd backend/MesEquipment.Api
dotnet run
```

Then generate the frontend client from the project root:

```bash
dotnet tool restore
dotnet tool run nswag run nswag.json
```

Generated file:

```text
MesEquipment.Web/src/app/api/mes-equipment-api.ts
```

The generated client is wrapped by `MachineService`, and DTO conversion is kept in
`machine-api.mapper.ts` so the Angular app does not depend directly on generated DTOs.

The generated file should not be edited manually. Update the backend OpenAPI contract
or `nswag.json`, then regenerate the client.

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

The service tests use the EF Core InMemory database, so they can run without MySQL or Docker.
During Version 3, a regression test was added for machine status transitions.
The test caught a bug where MachineStatusLog was created but not persisted to
the database. The service was updated to persist the status log when a machine
status changes.
The backend also includes API integration tests using WebApplicationFactory.
Covered API behaviors:

- protected machine endpoints return 401 without a JWT
- login returns a JWT token for valid credentials
- authorized machine list requests return paged data
- invalid machine DTOs return 400 Bad Request through ASP.NET Core validation
  The integration tests replace the MySQL DbContext configuration with the EF Core
  InMemory database and use test-specific JWT settings.

## CI

This project uses GitHub Actions to run backend tests and frontend builds on
pushes and pull requests to `main`.

CI checks:

- backend restore, build, and test
- frontend dependency installation with `npm ci`
- frontend production build

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

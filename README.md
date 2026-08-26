# TaskFlow API
A task management REST API built with ASP.NET Core, Entity Framework Core, and SQL Server.
Core CRUD, layered architecture, and full authentication/authorization are implemented and tested end to end.

## Tech Stack
- ASP.NET Core Web API (.NET 9)
- Entity Framework Core (SQL Server)
- Swagger / Swashbuckle
- ASP.NET Identity + JWT Bearer Authentication

## Progress
### ✅ Stage 1 — Core CRUD API
- Model, DTOs (Create/Update/Read/PartialUpdate), and manual mapper (extension methods)
- Service layer (interface + implementation) separated from Controller
- Full async/await database access via EF Core
- RESTful Controller with proper status codes (200, 201, 204, 404)
- Route constraints (`{id:int}`) for input safety and performance
- Relies on `[ApiController]`'s built-in model validation and binding inference (no redundant `ModelState.IsValid` / `[FromBody]` / `[FromRoute]`)
- Pagination on GetAll (`page`, `pageSize` query params, wrapped in `PagedResponse<T>`)
- Global exception handling middleware (no leaked stack traces)
- Structured logging via built-in `ILogger<T>` throughout the service layer
- Swagger UI auto-launch in development

### ✅ Stage 2 — Identity & Authorization
- ASP.NET Identity for user accounts (`User : IdentityUser`) — automatic password hashing and unique email enforcement
- JWT Bearer authentication — `AuthController` with Register/Login issuing tokens via `TokenService`
- TaskItem ↔ User one-to-many relationship (`UserId` foreign key)
- `[Authorize]` protecting all TaskItem endpoints — every query/command scoped to the logged-in user, so users can only ever see or modify their own tasks
- Swagger UI configured with Bearer token support (Authorize button)
- JWT signing key stored via `dotnet user-secrets`, not committed to source control
# Weather Forecast API – Project Details (A to Z)

This document describes **what** was built, **why** it was built that way, and **how** it works, with examples. It complements the [README](README.md).

---

## Table of Contents

1. [Project purpose and scope](#1-project-purpose-and-scope)
2. [Why this architecture?](#2-why-this-architecture)
3. [Layer-by-layer breakdown](#3-layer-by-layer-breakdown)
4. [Domain layer – what and why](#4-domain-layer--what-and-why)
5. [Application layer – what and why](#5-application-layer--what-and-why)
6. [Infrastructure layer – what and why](#6-infrastructure-layer--what-and-why)
7. [API layer – what and why](#7-api-layer--what-and-why)
8. [Key patterns and how they work](#8-key-patterns-and-how-they-work)
9. [Features implemented (with examples)](#9-features-implemented-with-examples)
10. [Configuration and environment](#10-configuration-and-environment)
11. [Database, migrations, and seed](#11-database-migrations-and-seed)
12. [Testing](#12-testing)
13. [Conventions and structure](#13-conventions-and-structure)
14. [End-to-end request examples](#14-end-to-end-request-examples)

---

## 1. Project purpose and scope

**What it is:** A REST API that:

- Manages **weather forecasts** (create, list, filter by temperature range).
- Provides **JWT-based authentication** (register and login).
- Applies **rate limiting**, **validation**, and **global error handling**.
- Uses **PostgreSQL** with **Entity Framework Core** and follows **Clean Architecture** with **CQRS** and **Unit of Work**.

**Why:** To demonstrate a production-style .NET API with clear separation of concerns, testability, and consistent patterns (Result, validation, UoW, repositories).

**Out of scope:** No UI; no refresh tokens or OAuth; no email verification or password reset (can be added later using the same patterns).

---

## 2. Why this architecture?

| Decision | Why |
|----------|-----|
| **Clean Architecture** | Domain has no dependencies; business rules stay in one place. Swapping database or API style does not touch domain or use cases. |
| **CQRS (MediatR)** | Commands and queries are explicit; each handler does one thing. Easy to add validation, logging, or new use cases without touching controllers. |
| **Unit of Work** | All writes go through one place (`SaveAsync`). Repositories only register changes; no accidental `SaveChanges` from random code. Transactions are explicit (Begin → Save → Commit/Rollback). |
| **Result&lt;T&gt;** | Success/failure and errors are returned as data instead of throwing. Controllers map `Result` to HTTP status (e.g. 200 vs 400/401). |
| **FluentValidation** | Validation rules live in validators, not in handlers or controllers. Reusable and testable. |
| **Global exception handler** | One middleware turns exceptions (validation, domain, not found, etc.) into consistent JSON and status codes. |
| **Repository interfaces in Application** | Application defines *what* it needs (e.g. `GetByEmailAsync`); Infrastructure implements *how*. Domain and application stay free of EF and PostgreSQL. |

---

## 3. Layer-by-layer breakdown

```
┌─────────────────────────────────────────────────────────────────┐
│  API (Controllers, Middleware, JWT, Rate limiting)               │
│  References: Application, Infrastructure                         │
└───────────────────────────┬─────────────────────────────────────┘
                            │
        ┌───────────────────┴───────────────────┐
        ▼                                       ▼
┌───────────────────────┐             ┌───────────────────────────┐
│  Application          │             │  Infrastructure            │
│  Commands, Queries,    │             │  EF Core, Repositories,    │
│  DTOs, Contracts,      │◄────────────│  JWT, BCrypt, Seed          │
│  Services, Validators  │  implements │  References: App + Domain   │
│  References: Domain    │             │                             │
└───────────┬────────────┘             └──────────────┬─────────────┘
            │                                         │
            └──────────────────┬─────────────────────┘
                               ▼
                    ┌──────────────────────┐
                    │  Domain               │
                    │  Entities, Value      │
                    │  Objects, Exceptions  │
                    │  No references        │
                    └──────────────────────┘
```

- **Domain:** Core entities and rules; no dependencies.
- **Application:** Use cases, contracts (repositories, services), DTOs, validation. Depends only on Domain.
- **Infrastructure:** Implements Application contracts (repos, UoW, JWT, hashing, seed). Depends on Application and Domain.
- **API:** Entry point; wires configuration, auth, rate limiting, and controllers. Depends on Application and Infrastructure.

---

## 4. Domain layer – what and why

**Location:** `src/WeatherForecastApp.Domain`

**What’s in it:**

| Item | Purpose |
|------|--------|
| **BaseEntity** | Shared `Id`, `CreatedAt`, `UpdatedAt`. `SetId`/`SetCreatedAt` are internal so only Infrastructure (e.g. repository) can set them when adding entities. |
| **IAggregateRoot** | Marks the root of an aggregate; used by generic repository. |
| **User** | Auth entity: `Email`, `PasswordHash`. Created via `User.Create(email, passwordHash)`. |
| **WeatherForecast** | Forecast entity: `Date`, `Temperature`, `Summary`. Created via `WeatherForecast.Create(...)`. |
| **Temperature** | Value object: `Celsius`, computed `Fahrenheit`. Validates range (-100 to 100°C). `FromCelsius` / `FromFahrenheit` factory methods. |
| **DomainException** | Thrown for domain rule violations (e.g. invalid temperature). Caught by global handler and returned as 400. |

**Why:**

- **No dependencies:** Domain does not reference Application, Infrastructure, or API. It can be tested and reused without any framework.
- **Value objects:** `Temperature` encapsulates validation and conversion; entities stay clear.
- **Factory methods:** `User.Create` and `WeatherForecast.Create` keep creation rules in one place and avoid anemic entities.

**Example – Temperature validation:**

```csharp
// Allowed
var t = Temperature.FromCelsius(25);

// Throws DomainException
var t2 = Temperature.FromCelsius(150);  // "Temperature must be between -100 and 100 degrees Celsius"
```

---

## 5. Application layer – what and why

**Location:** `src/WeatherForecastApp.Application`

**What’s in it:**

- **Contracts:** Repository and service interfaces (what the app needs, not how it’s done).
- **DTOs:** Request/response shapes per feature (Auth, WeatherForecasts).
- **Features:** CQRS – Commands/Queries with Handlers and Validators.
- **Services:** `IAuthService`, `IWeatherForecastService` and their implementations (orchestrate repos, UoW, token/hash).
- **Common:** `Result<T>`, `Result`, `JwtSettings`, `IMapper`.
- **Behaviors:** MediatR pipeline (e.g. validation before handler).
- **Specifications:** Reusable query specs for repositories (e.g. temperature range).
- **Mappings:** Mapster config (entity ↔ DTO).

**Why:**

- **Single responsibility:** One command/query per use case; handlers stay small.
- **Testability:** Handlers and services depend on interfaces; we mock repos and UoW in unit tests.
- **Validation in one place:** FluentValidation validators run via pipeline; handlers assume valid input.

**Example – Result usage:**

```csharp
// Success
return Result<LoginResponse>.Success(new LoginResponse(token, "Bearer", expiresAt));

// Failure (e.g. wrong password)
return Result<LoginResponse>.Failure("Invalid email or password.");

// Controller maps to HTTP
if (!result.IsSuccess) return Unauthorized(result);
return Ok(result);
```

**Example – validation (Login):**

```csharp
// LoginCommandValidator
RuleFor(x => x.Request.Email).NotEmpty().EmailAddress();
RuleFor(x => x.Request.Password).NotEmpty();

// If invalid, ValidationBehavior throws ValidationException
// → GlobalExceptionHandlerMiddleware returns 400 with list of property errors
```

---

## 6. Infrastructure layer – what and why

**Location:** `src/WeatherForecastApp.Infrastructure`

**What’s in it:**

| Component | Responsibility |
|-----------|----------------|
| **DbContext** | `WeatherForecastDbContext` – DbSets for `WeatherForecast`, `User`. Configurations applied from assembly. |
| **Configurations** | EF mappings (table names, columns, indexes, e.g. unique `Email` for `User`). |
| **Migrations** | `InitialCreate` (WeatherForecasts), `AddUsersTable` (Users). |
| **Repositories** | `GenericRepository<T>`, `WeatherForecastRepository`, `UserRepository`. `Add`/`Update`/`Delete` only track changes; they do not call `SaveChanges`. |
| **UnitOfWork** | Single place that calls `SaveChangesAsync`. Supports `BeginTransactionAsync`, `CommitTransactionAsync`, `RollbackTransactionAsync`. |
| **JwtTokenService** | Builds JWT with user id, email, expiry; uses `JwtSettings` from options. |
| **BCryptPasswordHasher** | Hash and verify passwords; used by AuthService and SeedDataService. |
| **SeedDataService** | On startup: create default user (`admin@example.com` / `Admin@123`) and 30 sample forecasts if tables are empty. Uses repos + UoW only (no direct `SaveChanges`). |

**Why:**

- **Persistence rule:** No one can persist by “just” calling a repository. They must call `IUnitOfWork.SaveAsync()` (and optionally wrap in a transaction). So: repository = “register change”, UoW = “persist”.
- **Auth behind interfaces:** `IJwtTokenService` and `IPasswordHasher` are in Application; Infrastructure implements them. API and Application stay unaware of BCrypt or JWT library details.
- **Seed via UoW:** Seed logic uses the same repos and UoW as normal features, so the same rules apply and tests can mock the same abstractions.

**Example – repository + UoW (from AuthService):**

```csharp
await unitOfWork.BeginTransactionAsync();
try
{
    var user = User.Create(request.Email, passwordHasher.Hash(request.Password));
    await userRepository.AddAsync(user);   // Only registers in DbContext
    await unitOfWork.SaveAsync();          // Persists to DB
    await unitOfWork.CommitTransactionAsync();
    return Result<RegisterResponse>.Success(...);
}
catch
{
    await unitOfWork.RollbackTransactionAsync();
    throw;
}
```

---

## 7. API layer – what and why

**Location:** `src/WeatherForecastApp.API`

**What’s in it:**

- **Program.cs:** Build app, Serilog, register API/Infrastructure/Application services, middleware order, seed on startup, run.
- **Controllers:** `AuthController` (login, register), `WeatherForecastController` (get all, create, get by temperature range). Both use MediatR; WeatherForecast is protected with `[Authorize]`.
- **Middleware:** `GlobalExceptionHandlerMiddleware` first in pipeline – catches all unhandled exceptions and returns JSON (validation, domain, 404, 401, 500).
- **Extensions:** `AddApiServices` – JWT Bearer config, rate limiter (fixed/sliding/token bucket), controllers, OpenAPI.
- **Constants:** Rate limit policy names, used by `[EnableRateLimiting(...)]` and `RequireRateLimiting(...)`.
- **Models:** `ErrorResponse`, `ValidationError` – shape of error JSON returned by the global handler.

**Why:**

- **Thin controllers:** Controllers only send commands/queries and map `Result` to HTTP status. No business or persistence logic.
- **Auth once:** JWT is configured once in DI; `[Authorize]` on controller or action enforces it everywhere you need it.
- **Consistent errors:** One middleware ensures every error becomes a predictable JSON structure and status code.

**Middleware order (important):**

```text
GlobalExceptionHandlerMiddleware  →  UseHttpsRedirection  →  UseRateLimiter  →  UseAuthentication  →  UseAuthorization  →  MapControllers
```

---

## 8. Key patterns and how they work

### 8.1 Unit of Work

- **What:** One object that exposes `SaveAsync()`, `BeginTransactionAsync()`, `CommitTransactionAsync()`, `RollbackTransactionAsync()`.
- **Why:** So that “save” and “transaction” are explicit. Repositories never call `SaveChanges`; only UoW does.
- **How:** Handlers (or application services) call `repository.AddAsync` / `Update` / `Delete`, then `unitOfWork.SaveAsync()`. For multi-step writes, they use Begin → … → Save → Commit (or Rollback on exception).

### 8.2 Repository

- **What:** Interfaces in Application (e.g. `IUserRepository.GetByEmailAsync`), implementations in Infrastructure. Extend `IGenericRepository<T>` for common CRUD.
- **Why:** Application stays independent of EF and database; tests can mock repositories.
- **Contract note:** Add/Update/Delete only register changes; persistence happens only when `IUnitOfWork.SaveAsync()` is called.

### 8.3 Result&lt;T&gt; / Result

- **What:** Wrapper with `IsSuccess`, `Value`, `Errors`. No throwing for “business” failures (e.g. wrong password).
- **Why:** Callers can handle success and failure uniformly; controllers map to 200/400/401/404 without try/catch for every case.
- **Example:** Login returns `Result<LoginResponse>`. Success → 200 + body; failure → 401 + body with error message.

### 8.4 CQRS with MediatR

- **What:** Commands (e.g. `LoginCommand`, `CreateWeatherForecastCommand`) and Queries (e.g. `GetAllWeatherForecastsQuery`) with one handler each. Controllers `Send(command)` or `Send(query)`.
- **Why:** Clear use-case boundaries; easy to add validation, logging, or new use cases without touching controllers or domain.
- **Flow:** Controller → MediatR → (optional ValidationBehavior) → Handler → (optional Application Service) → Repository/UoW → return Result or DTO.

### 8.5 Validation pipeline

- **What:** `ValidationBehavior<TRequest, TResponse>` runs all `IValidator<TRequest>` before the handler. If any fail, it throws `ValidationException`.
- **Why:** Handlers assume valid input; validation rules live in validators (FluentValidation), not in handlers.
- **Result:** `ValidationException` is caught by global handler → 400 with list of property errors (property name, message, attempted value).

### 8.6 Global exception handling

- **What:** Middleware wraps the pipeline in try/catch; maps exception type to HTTP status and `ErrorResponse` JSON.
- **Why:** No need to repeat try/catch in every controller; clients always get a consistent error shape.
- **Mapping (examples):** `ValidationException` → 400; `DomainException` → 400; `KeyNotFoundException` → 404; `UnauthorizedAccessException` → 401; rest → 500.

---

## 9. Features implemented (with examples)

### 9.1 Authentication

- **Register:** Accepts email + password. Checks duplicate email; hashes password (BCrypt); creates `User`; saves via UoW. Returns user id, email, message.
- **Login:** Loads user by email; verifies password; generates JWT (userId, email, expiry); returns token, token type, expiry.
- **Why JWT:** Stateless; API only validates signature and expiry. No server-side session store.
- **Why BCrypt:** Industry standard for password hashing; slow by design to reduce brute-force risk.

**Example – register then login:**

```http
POST /api/auth/register
Content-Type: application/json
{"email": "john@example.com", "password": "Secret123"}

→ 200: {"isSuccess":true,"value":{"id":"...","email":"john@example.com","message":"User registered successfully."}}

POST /api/auth/login
Content-Type: application/json
{"email": "john@example.com", "password": "Secret123"}

→ 200: {"isSuccess":true,"value":{"token":"eyJ...","tokenType":"Bearer","expiresAt":"2026-02-07T10:00:00Z"}}
```

### 9.2 Weather forecasts

- **Get all:** Query returns all forecasts; mapped to DTOs; wrapped in `Result<GetAllWeatherForecastsResponse>` (forecasts + count).
- **Create:** Command with `WeatherForecastDto`; service maps to entity, adds via repository, saves via UoW in a transaction; returns `Result<CreateWeatherForecastResponse>`.
- **Get by temperature range:** Query with min/max temp; repository uses specification to filter; returns `Result<GetForecastsByTemperatureRangeResponse>`.

All weather endpoints require `Authorization: Bearer <token>`.

**Example – create forecast (authenticated):**

```http
POST /api/WeatherForecast
Authorization: Bearer eyJ...
Content-Type: application/json
{"date": "2026-02-10T00:00:00Z", "temperatureC": 22, "temperatureF": 72, "summary": "Mild"}

→ 200: {"isSuccess":true,"value":{"id":"...","date":"2026-02-10T00:00:00Z","message":"Weather forecast created successfully."}}
```

### 9.3 Rate limiting

- **Fixed window:** Used globally (e.g. 100 requests per 1 minute per user/IP). Simple cap per fixed time window.
- **Sliding window:** Used on `WeatherForecastController` (e.g. 50 per 1 minute, 2 segments). Smoother limit over a rolling window.
- **Token bucket:** Configured but not applied to any route; can be enabled per controller/action for burst + sustained rate.

When limit is exceeded: **429 Too Many Requests** with `retryAfter` in body and `Retry-After` header. Details are in [README – Rate limiting](README.md#rate-limiting).

### 9.4 Seed data

- **When:** On application startup, after the pipeline is built, a scope runs `ISeedDataService.SeedAsync()`.
- **What:** If no user with email `admin@example.com` exists, creates one (password `Admin@123`). If no weather forecasts exist, creates 30 sample forecasts. All via repositories + UoW in one transaction.
- **Why:** So developers can run the app and immediately use login and forecast endpoints without manual DB setup.

---

## 10. Configuration and environment

- **appsettings.json:** Base config (e.g. JWT section, allowed hosts). No connection string (so it’s safe to commit).
- **appsettings.Development.json:** Connection string, JWT overrides, Serilog, rate limit values. Loaded in Development.
- **JWT:** `Secret` (min 32 chars), `Issuer`, `Audience`, `ExpirationMinutes`. Used by both token generation (Infrastructure) and validation (API).
- **Rate limiting:** All rate limit numbers (permit limit, window, queue, token bucket, etc.) come from config so they can change per environment without code change.

---

## 11. Database, migrations, and seed

- **Provider:** PostgreSQL (Npgsql).
- **DbContext:** In Infrastructure; exposes `DbSet<WeatherForecast>` and `DbSet<User>`.
- **Migrations:** In `Infrastructure/Persistence/Migrations`. Applied with:
  `dotnet ef database update --project src/WeatherForecastApp.Infrastructure --startup-project src/WeatherForecastApp.API`
- **Seed:** Implemented in `SeedDataService`; uses `IUserRepository`, `IWeatherForecastRepository`, `IPasswordHasher`, `IUnitOfWork` only. No direct `DbContext.SaveChanges` in seed.

---

## 12. Testing

- **Location:** `tests/WeatherForecastApp.UnitTests`.
- **Framework:** xUnit, Moq.
- **What’s tested:**  
  - **AuthService:** Login (user not found, wrong password, success), Register (duplicate email, success with UoW calls).  
  - **WeatherForecastService:** Create (UoW and repo calls), Get all, Get by range (mapping and repo usage).  
  - **JwtTokenService:** Token non-empty, different tokens per call / per user.  
  - **BCryptPasswordHasher:** Hash/verify.  
  - **SeedDataService:** No duplicate user/forecasts when already present; adds user + 30 forecasts when empty.
- **Helpers:** `DomainTestHelper` (e.g. set `Id` on entity via reflection for mocks).
- **Why:** To ensure services behave correctly with mocked dependencies and that UoW/repository contracts are used as intended.

Run: `dotnet test tests/WeatherForecastApp.UnitTests/WeatherForecastApp.UnitTests.csproj`

---

## 13. Conventions and structure

- **Global usings:** Each project has `GlobalUsings.cs` so that common namespaces (e.g. Application contracts, DTOs) don’t need to be repeated in every file.
- **DTOs by feature:** `DTOs/Auth` (LoginRequest/Response, RegisterRequest/Response), `DTOs/WeatherForecasts` (WeatherForecastDto, Create/GetAll/GetByRange responses). Matches feature structure.
- **Features:** Under `Features/<FeatureName>/Commands` or `Queries/<UseCase>/` with Command/Query, Handler, Validator.
- **Contracts:** Repository and service interfaces in `Application/Contracts`; implementations only in Infrastructure.
- **Naming:** Commands end with `Command`, queries with `Query`, handlers with `Handler`, validators with `Validator`. DTOs describe request/response (e.g. `LoginRequest`, `GetAllWeatherForecastsResponse`).

---

## 14. End-to-end request examples

### A. Register → Login → Create forecast

```http
1) Register
POST /api/auth/register
Content-Type: application/json
{"email": "dev@test.com", "password": "Pass123!"}

2) Login
POST /api/auth/login
Content-Type: application/json
{"email": "dev@test.com", "password": "Pass123!"}
→ Copy "token" from response.

3) Create forecast
POST /api/WeatherForecast
Authorization: Bearer <token from step 2>
Content-Type: application/json
{"date": "2026-02-15T12:00:00Z", "temperatureC": 18, "temperatureF": 64, "summary": "Cool"}
```

### B. Validation error (400)

```http
POST /api/auth/login
Content-Type: application/json
{"email": "not-an-email", "password": ""}

→ 400 Bad Request
{
  "error": "Validation Error",
  "message": "One or more validation errors occurred",
  "errors": [
    {"property": "Request.Email", "message": "Invalid email format.", "attemptedValue": "not-an-email"},
    {"property": "Request.Password", "message": "Password is required.", "attemptedValue": ""}
  ]
}
```

### C. Auth failure (401)

```http
POST /api/auth/login
Content-Type: application/json
{"email": "dev@test.com", "password": "WrongPassword"}

→ 401 Unauthorized
{"isSuccess":false,"value":null,"errors":["Invalid email or password."]}
```

### D. Rate limit exceeded (429)

After exceeding the allowed requests in the window (e.g. 100 for fixed window):

```http
→ 429 Too Many Requests
{"error": "Too many requests. Please try again later.", "retryAfter": 60}
Retry-After: 60
```

### E. Protected endpoint without token (401)

```http
GET /api/WeatherForecast
(no Authorization header)

→ 401 Unauthorized (JWT middleware)
```

---

This document and the [README](README.md) together give the full picture: what the project does, how it’s structured, why choices were made, and how to run and use it with concrete examples.

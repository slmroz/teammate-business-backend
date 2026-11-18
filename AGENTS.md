# ERP Backend Architecture and Change Guide

Read this guide before changing code under `ERP/`. It describes the current backend structure and the conventions future changes should follow. The recommendations below are not claims that the current implementation already enforces them.

## Solution shape

The backend is a .NET 10 solution (`ERP.sln`) organized as a layered, feature-oriented application:

| Project | Current responsibility |
| --- | --- |
| `ERP.Api` | ASP.NET Core host, controllers, HTTP routes, middleware pipeline and API composition root. |
| `ERP.Services.Customer` | Customer/contact/lead commands, queries, DTOs and validators. |
| `ERP.Services.Products` | Product, product group, currency and price-list commands, queries and DTOs. |
| `ERP.Services.Sales` | Quote and quote-item commands, queries and shared quote DTOs. |
| `ERP.Services.User` | User, sign-in and password workflows, handlers and DTOs. |
| `ERP.Services.Shared` | Shared CQRS interfaces, search/page-result types, security and email abstractions, exceptions and validation behavior. |
| `ERP.Services.Common` | Common service project; review its interface against `ERP.Services.Shared` before adding more shared contracts. |
| `ERP.Infrastructure` | Authentication, password hashing, email/web adapters, clock, logging and exception middleware. It currently references feature projects. |
| `ERP.Model` | EF Core `ErpContext`, SQL Server mappings, entity classes, enums and clock abstraction. |
| `ERP.Tests` | xUnit handler/service tests, using EF Core InMemory for persistence. |

Typical request flow:

`ERP.Api` controller -> feature `ICommandHandler<T>` / `IQueryHandler<T, TResult>` -> `ErpContext` -> SQL Server.

Controllers currently construct the command/query object and invoke its handler directly. Feature registration uses Scrutor assembly scanning in each feature's `Extensions.cs`. `ERP.Infrastructure` registers cross-cutting adapters and authentication. The API host should own the explicit registration and middleware composition.

## Conventions for new work

1. **Keep HTTP concerns in controllers.** Follow the existing controller pattern: route/binding, call one handler, translate the result to an HTTP response. Do not put database queries or business workflows in controllers.
2. **Place work in the owning feature.** Add a command/query, handler and feature DTO beside related code in `ERP.Services.<Feature>`. Reuse `ERP.Services.Shared` only for genuinely cross-feature contracts. Keep request/response DTOs separate from EF entities.
3. **Use the existing CQRS handler contracts unless the project is deliberately migrated.** Commands perform writes and queries return read models. Handlers currently depend directly on `ErpContext`; follow that only for a narrowly scoped addition, and do not introduce a second mediator/dispatch model in parallel.
4. **Do not assume validation runs automatically.** Controllers call the custom handler interfaces directly, whereas `ValidationBehavior` is a MediatR pipeline behavior. Until dispatch is consistently routed through that pipeline, invoke/register validation explicitly at the actual handler boundary and cover it with tests.
5. **Keep reads bounded and database-side.** Use `AsNoTracking()` for read-only EF queries, project to DTOs in the query, and apply filtering, a deterministic sort and pagination before materializing. Validate page/page-size bounds and allow-list sort keys. Avoid applying `ToLower()` to database columns for search/sort; prefer the configured SQL collation or a deliberately indexed search strategy.
6. **Thread cancellation through I/O.** New async handlers and service interfaces should accept a `CancellationToken` and pass it to EF Core and HTTP/email operations. Propagate it from controller actions.
7. **Register every feature explicitly.** Add a clearly named `Add<Feature>Application` extension to the feature assembly and call it from `ERP.Api/Program.cs`. Avoid duplicate or misleading extension names. Verify controller dependencies resolve at startup, especially for new feature controllers.
8. **Keep persistence changes deliberate.** `ERP.Model/Model/ErpContext.cs` contains SQL Server schema/table/column mappings. Review the existing database schema and deployment process before changing entities or mappings. No EF migrations were found in the solution; do not assume `EnsureCreated` or runtime migrations are the deployment strategy.
9. **Use the shared clock for time-sensitive behavior.** `IClock` is already available through infrastructure and is used in user workflows. Prefer it over direct `DateTime.Now`/`UtcNow` when behavior needs deterministic tests.
10. **Protect secrets and identity-derived fields.** Use `ERP.Api/appsettings.example.json` as the non-secret configuration template; keep local `appsettings*.json` files out of commits and supply credentials through environment-specific secret storage. Previously committed credentials must be treated as exposed and rotated. Enforce authorization at endpoints/policies, derive audit actor IDs from the authenticated principal, and never trust a client-supplied role or audit identity.
11. **Centralize errors.** Return the API's agreed validation/problem-details shape through one global error handler. Do not return exception details or credentials to clients; log errors with structured fields and avoid sensitive payloads.
12. **Test behavior at the correct boundary.** Add focused handler tests for business rules, API integration tests for routing/authentication/serialization, and SQL Server-compatible integration coverage for persistence behavior. EF InMemory tests are useful for fast unit tests but do not validate SQL translation, constraints, transactions or collation.

## Architecture and quality findings

Prioritize the following before expanding the API:

### Critical security and operational risks

- `ERP.Api/appsettings.json` contains literal database, mail and JWT signing credentials. Treat each as compromised: rotate/revoke them, remove them from tracked configuration and repository history where applicable, and supply replacements through deployment secrets. Keep only non-sensitive defaults in JSON files.
- Authentication is configured in infrastructure, but `ERP.Api/Program.cs` calls `UseAuthorization()` without `UseAuthentication()` and does not call `UseInfrastructure()`. The latter currently contains the authentication and exception middleware setup. Also, most business controllers have no authorization attributes or fallback policy; `UserController`'s user-list and sign-up actions are not protected. Define an explicit default authorization policy and least-privilege endpoint policies, then verify them with HTTP integration tests.
- Public sign-up accepts a role value that `SignUpHandler` stores. Do not allow callers to choose privileged roles; assign the ordinary role server-side and expose role changes only through an authorized administrative workflow. Add a database uniqueness constraint for normalized email to make duplicate prevention race-safe.
- `EmailService` explicitly sets SMTP `EnableSsl = false` and ignores the configured SSL value. Enable and validate TLS before sending credentials, use disposable email resources, and HTML-encode values inserted into email templates.
- Password reset tokens are persisted in clear text. Persist a cryptographic hash, compare in constant time where applicable, keep expiry and one-time-use semantics, and rate-limit reset/sign-in endpoints. Return generic public responses for account recovery.

### Correctness and composition risks

- `ERP.Api/Program.cs` imports both `ERP.Services.Customer` and `ERP.Services.Products`, and both expose an `AddProductApplication` extension. This appears to make the registration call ambiguous at compile time. In addition, `ERP.Services.Sales/Extensions.cs` also declares `AddProductApplication`, and the API does not register the Sales handlers. Rename the extensions to their actual feature names and register each module exactly once.
- FluentValidation is registered for Customer, but handlers are called directly rather than through MediatR; therefore the MediatR `ValidationBehavior` is not on the request path. Either consistently dispatch through MediatR and register handlers/validators there, or remove the unused pipeline and validate at the custom handler boundary. Add equivalent validation coverage for every feature.
- `ERP.Infrastructure/Extensions.cs` defines `UseInfrastructure()` with authentication and exception handling, while the API builds a different pipeline. It also combines custom exception middleware with `UseExceptionHandler`. Choose one global exception strategy and wire it once in the host, with CORS before authentication/authorization and only the middleware the application uses.
- Swagger is enabled unconditionally. Restrict its exposure to development or protect it according to deployment requirements. Keep CORS origins environment-configured and narrowly allow-listed; CORS is not an authorization control.
- Sign-in and password-reset flows throw generic `Exception` for expected invalid input. Use typed application errors and consistent status codes so expected failures do not become 500 responses or leak account existence.

### Maintainability and performance

- Keep all projects on the supported .NET 10 LTS target and align Microsoft package servicing versions across the solution. Central package management can prevent version drift across projects.
- The project graph is not a strict Clean Architecture dependency direction: `ERP.Infrastructure` references application feature projects, and `ERP.Model` owns both EF persistence and entity types. For a modular monolith, first make dependency ownership and composition explicit; only split persistence/domain assemblies if the separation provides clear value.
- Shared abstractions and CQRS are custom, while MediatR is also referenced. Choose one execution/validation model rather than retaining unused or partly wired abstractions. Review `ERP.Services.Common` and `ERP.Services.Shared` before adding duplicate common-service contracts.
- Several paged queries use column `ToLower()` for searching and sorting, which can inhibit index use. Use database collation-aware comparisons and add indexes based on measured query plans. Cap page size, use deterministic tie-breaker ordering, and measure large-list query plans before optimizing.
- All handlers depend on the concrete EF context and most do not accept cancellation tokens. Cancellation and database abstractions may be introduced incrementally where they improve testability or reliability; avoid adding repository wrappers that only duplicate `DbSet` operations.
- The tests primarily exercise handlers with EF Core InMemory. Add a small integration suite against SQL Server (or a compatible disposable SQL Server instance) for query translation, constraints, migrations/schema compatibility and transactions, plus `WebApplicationFactory` tests for middleware and authorization.
- Add an explicit, reviewed database migration/deployment process, health checks for dependencies, and structured observability (request IDs, metrics and redacted logs) before relying on this API in production.

## Validation note

The solution targets .NET 10. The latest recorded full test run had 124 passing tests and 9 failures: quote tests lacked the required `Quote.Subject`, and a sign-up test expected the previous generic exception type. Re-run the suite after changes and distinguish these existing failures from new regressions.

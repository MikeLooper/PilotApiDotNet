# Add Security to PilotApiDotNet — Implementation Plan

## Context

`docs/Add-Security-Plan.md` requires securing every endpoint except `/healthcheck` and `/about` with Keycloak-issued JWTs (OAuth2), role-based authorization keyed to HTTP verb, and a config-driven "active" flag that softens enforcement to a warning instead of a hard block while the feature is being rolled out or during incident response. The API currently has **no** authentication/authorization at all — `SimpleControllerBase` (the shared base for every V1 business controller) is decorated `[AllowAnonymous]`, and `ApiExtensions.AddSecurity`/`UseSecurity` already exist as stubbed-out, fully-commented extension methods wired into the pipeline, clearly left as placeholders for this exact feature.

Two decisions were confirmed with the user before designing this plan:
1. **When `Keycloak.Active = false`, ANY failure — missing/invalid token OR insufficient role for the HTTP verb — must not block the request.** It proceeds to the controller action, but the response gets a `Warning` header describing what failed. When `Active = true`, normal 401 (unauthenticated)/403 (insufficient role) blocking applies.
2. **Realm = `local-realm`, Client ID = `local-client`** — these are real values to put directly in configuration, not placeholders.

Roles are **not** read from Keycloak token role claims. They come from a mocked `IUserRolesRepository` (simulating a `UserRoles` table) keyed by the JWT's `preferred_username` claim (matches Keycloak's standard username claim, and matches the format of the mock table's `UserId` values: `reader_user`, `working_user`, `working_admin`).

## Architecture / layering

Dependency direction today: `Shared` (leaf, no project refs) ← `Domain` ← `Repositories` ← `Services` ← `Web`. `Shared` must never take a compile-time dependency on `Repositories` or `Services`. Since `IUserRolesRepository` belongs in `Repositories` (per the spec's explicit "repository object" requirement), anything that calls it directly must live in `Services` or higher. This splits the new code as:

- **`PilotApi.Shared`** — Keycloak configuration classes, `RoleNames`/`SecurityConstants`, the verb→role authorization requirement + handler (reads only `ClaimsPrincipal`/`HttpContext`), the active-flag bypass authorization result handler, `AddSecurity`/`UseSecurity` wiring, token-redaction utility.
- **`PilotApi.Repositories`** — `IUserRolesRepository` + `UserRolesRepository` (the mock table), same folder convention as `ICategoriesRepository`/`CategoriesRepository`.
- **`PilotApi.Services`** — `PreferredUsernameRoleClaimsTransformation` (needs `IUserRolesRepository`), new DI registrations added to the existing `ServicesInjectionExtensions.ServicesRegistration`.
- **`PilotApi.Web`** — remove `[AllowAnonymous]` from `SimpleControllerBase` only.

No new project-reference edges are needed.

## 1. New package

Add to `Directory.Packages.props` (alongside the other `10.0.10`-pinned `Microsoft.AspNetCore.*` packages):
```xml
<PackageVersion Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="10.0.10" />
```
Reference it (no version, CPM) from `src/PilotApi.Shared/PilotApi.Shared.csproj` — use `dotnet add package` per the `package-management` skill, not hand-edited XML. `Shared` already compiles code using `WebApplication`/`HttpContext`/`IServiceCollection` today with no explicit `FrameworkReference`, because packages like `Microsoft.AspNetCore.OpenApi` transitively bring in the ASP.NET Core shared framework; the new JwtBearer package should work the same way. **Contingency:** if the build complains about missing ASP.NET Core types after adding the package, add `<FrameworkReference Include="Microsoft.AspNetCore.App" />` to `PilotApi.Shared.csproj`.

## 2. Configuration

Follow the existing hand-rolled config pattern exactly (`ConfigurationBase` → `Active` bool already exists and is reused as *the* active flag; no new property name).

**New `src/PilotApi.Shared/Contracts/Configuration/IKeycloakConfiguration.cs`** (mirrors `IOpenApiConfiguration.cs`):
```csharp
public interface IKeycloakConfiguration : IConfigurationBase
{
    string? BaseUrl { get; set; }     // e.g. "http://localhost:55001"
    string? Realm { get; set; }       // "local-realm"
    string? ClientId { get; set; }    // "local-client"
    bool RequireHttpsMetadata { get; set; }
    int ClockSkewSeconds { get; set; } // tolerance applied to token expiry validation; see §4a
    string Authority { get; }         // computed: $"{BaseUrl}/realms/{Realm}"
}
```

**New `src/PilotApi.Shared/Configuration/KeycloakConfiguration.cs`** — mirror `OpenApiConfiguration.cs` (`[JsonObject]`, parameterless ctor, copy-ctor `KeycloakConfiguration(KeycloakConfiguration sourceConfiguration)`, `ToString()`, `override void Validate(ref List<Exception> exceptions)` requiring `BaseUrl`/`Realm`/`ClientId` non-blank via `ConfigurationException`). No `suppressSensitiveValues` parameter is needed (no secret fields — token acquisition in the README example uses the Resource Owner Password grant directly against Keycloak, no client secret). `Authority` is a computed property, not `[JsonProperty]`-bound. Validation runs unconditionally, even when `Active = false` — JWT bearer registration always happens so that valid tokens are still actually validated; only the *bypass-on-failure* behavior depends on `Active`.

`ClockSkewSeconds` defaults to `60` in the parameterless ctor (tighter than the ASP.NET Core/`Microsoft.IdentityModel` default of 300 seconds) and is deliberately **not** a required field in `Validate()` — it's a tunable with a sane default, not a value that can be "missing." `Validate()` should reject a negative value, since a negative clock skew is meaningless.

**Modify `IApplicationConfiguration.cs`**: add `KeycloakConfiguration? Keycloak { get; set; }`, update the `<example>` XML doc JSON block to include a `Keycloak` section.

**Modify `ApplicationConfiguration.cs`** (`src/PilotApi.Shared/Configuration/ApplicationConfiguration.cs`):
- Parameterless ctor: `this.Keycloak = new KeycloakConfiguration();`
- `Initialize(...)`: `this.Keycloak = new KeycloakConfiguration(sourceConfiguration.Keycloak);`
- `ToString()`: append `Keycloak`.
- The real, parameterless `Validate()` (not the `[Obsolete]` `Validate(ref exceptions)` override) — add the same null-check-then-`Validate(ref exceptions)` block already used for `OpenApi`/`OpenTelemetry` (`ApplicationConfiguration.cs:137-157`).

**Modify `test/PilotApi.TestingShared/Utilities/TestingSharedDoublesUtilities.cs`** (`GetApplicationConfiguration`) — this is the shared builder every config/controller test uses; once `ApplicationConfiguration.Validate()` requires `Keycloak`, add:
```csharp
Keycloak = new KeycloakConfiguration
{
    Active = true,
    BaseUrl = "http://localhost:55001",
    Realm = "local-realm",
    ClientId = "local-client",
    RequireHttpsMetadata = false
}
```

## 3. appsettings

`src/PilotApi.Web/appsettings.json` — add under `"Application"`:
```json
"Keycloak": {
    "Active": true,
    "BaseUrl": "http://local-keycloak:8080",
    "Realm": "local-realm",
    "ClientId": "local-client",
    "RequireHttpsMetadata": false,
    "ClockSkewSeconds": 60
}
```

`src/PilotApi.Web/appsettings.Development.json` — add (only `BaseUrl` differs by environment, per standard config layering):
```json
"Keycloak": {
    "BaseUrl": "http://localhost:55001",
    "RequireHttpsMetadata": false
}
```
(`ClockSkewSeconds` inherits the base value of `60` in Development too — no override needed unless local clock drift becomes an issue during testing.)

Update the `Application` example JSON block in `IApplicationConfiguration.cs`'s XML doc and the "Example configurations" `appsettings.json` block in `README.md` to match.

## 4. JWT Bearer registration

Modify `src/PilotApi.Shared/Api/Extensions/ApiExtensions.cs`:

- `AddSecurity` gains a parameter: `public static void AddSecurity(this IServiceCollection services, IKeycloakConfiguration keycloakConfiguration)` with the standard `ArgumentException` null-guards used elsewhere in this file. Implementation replaces the commented block:
```csharp
services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = keycloakConfiguration.Authority;
    options.Audience = keycloakConfiguration.ClientId;
    options.RequireHttpsMetadata = keycloakConfiguration.RequireHttpsMetadata;
    options.MapInboundClaims = false; // keep raw "preferred_username" claim name
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = keycloakConfiguration.Authority,
        ValidateAudience = true,
        ValidAudience = keycloakConfiguration.ClientId,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(keycloakConfiguration.ClockSkewSeconds),
        ValidateIssuerSigningKey = true,
        NameClaimType = SecurityConstants.PreferredUsernameClaimType
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = ...,       // log success, see §8
        OnAuthenticationFailed = ...  // log failure, redacted, distinguish expired tokens — see §4a and §8
    };
});

services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddRequirements(new HttpVerbRoleRequirement())
        .Build();
});

services.AddSingleton<IAuthorizationHandler, HttpVerbRoleAuthorizationHandler>();
services.AddSingleton<IAuthorizationMiddlewareResultHandler, BypassOnInactiveAuthorizationMiddlewareResultHandler>();
```

- `UseSecurity` becomes simply `webApp.UseAuthentication(); webApp.UseAuthorization();` — no changes to call order in `ApiWebApplication` (already runs after `UseMiddleware<UnhandledExceptionMiddleware>()`, before `MapControllers()`).

- `ApiWebApplicationBuilder` currently calls `builder.Services.AddSecurity();` *before* it builds the temporary `serviceProvider` (used later for OpenTelemetry/OpenApi). Move the `var serviceProvider = builder.Services.BuildServiceProvider();` line earlier so `AddSecurity` can resolve `IApplicationConfiguration` (registered by `ServicesConfiguration()`, which already runs before `ApiWebApplicationBuilder()` in `Program.cs`) and pass `applicationConfiguration.Keycloak!` into `AddSecurity`. This reuses the exact "build a temporary provider mid-registration" pattern already present in this method — no new pattern.

### 4a. Token expiry and refresh handling

**Expiry** is enforced on every request via `ValidateLifetime = true` plus the configurable `ClockSkew` above — an expired access token fails JWT validation exactly like any other invalid token (bad signature, wrong audience, etc.), producing a 401 (or the bypass-with-`Warning` behavior when `Active = false`). To make expiry failures distinguishable from other validation failures (both for audit logging and for a clearer `Warning` header message), `OnAuthenticationFailed` should special-case the exception type:
```csharp
OnAuthenticationFailed = context =>
{
    var isExpired = context.Exception is SecurityTokenExpiredException;
    // log: isExpired ? "Token expired" : "Token validation failed", with context.Exception, redacted Authorization header (§8)
    context.HttpContext.Items["AuthFailureReason"] = isExpired ? "Token expired." : "Missing or invalid bearer token.";
    return Task.CompletedTask;
}
```
`BypassOnInactiveAuthorizationMiddlewareResultHandler.BuildWarningMessage` (§6) reads `context.HttpContext.Items["AuthFailureReason"]` when building the `Challenged` branch's message, so an inactive-mode `Warning` header can say "Token expired." instead of a generic "Missing or invalid bearer token." — useful for callers debugging why they were let through with a warning.

**Refresh** is intentionally **not** implemented server-side. This API is a Keycloak *resource server* — it validates bearer tokens, it does not issue or manage them. Refresh tokens are an OAuth2 concept that lives entirely between the client and Keycloak's token endpoint (`.../protocol/openid-connect/token` with `grant_type=refresh_token`); the resource server never sees or stores a refresh token, and adding refresh handling here would mean taking on session/token state the API doesn't otherwise have. The correct behavior is: client gets 401 for an expired access token → client calls Keycloak directly with its `refresh_token` to obtain a new `access_token` → client retries the original request. §10 documents this flow for API consumers in the README so it isn't a silent gap.

**Required test fixes** in `test/PilotApi.Shared.Tests/Api/Extensions/ApiExtensionsTests.cs` — this file currently calls `services.AddSecurity()` with no args (2 tests) and calls `app.UseSecurity()`/`app.ApiWebApplication()` against a bare `WebApplication.CreateBuilder().Build()` with no prior auth registration (3 tests). Once `UseAuthentication()` is real, it throws `InvalidOperationException` if `IAuthenticationSchemeProvider` isn't registered. Update these tests to call `builder.Services.AddSecurity(new KeycloakConfiguration { BaseUrl = "http://localhost", Realm = "test-realm", ClientId = "test-client" })` before building, and pass a valid `KeycloakConfiguration` to the no-arg `AddSecurity()` calls.

## 5. Role-based authorization

**New `src/PilotApi.Repositories/Contracts/Repository/IUserRolesRepository.cs`**:
```csharp
public interface IUserRolesRepository
{
    Task<string?> GetRoleByUserIdAsync(string userId, CancellationToken cancellationToken = default);
}
```
Deliberately not derived from `IRepositoryBase<TEntity>` (that base assumes a Dapper/`IDataSourceContext`-backed entity, which doesn't apply to a hard-coded mock) — a small, intentional deviation.

**New `src/PilotApi.Repositories/Repositories/UserRolesRepository.cs`** — hard-coded `Dictionary<string,string>` (`StringComparer.OrdinalIgnoreCase`) mapping `reader_user`→`ReadOnly`, `working_user`→`ReadWrite`, `working_admin`→`Admin` (values from `RoleNames`). Constructor takes `ILoggerFactory` (matches `CategoriesRepository`'s convention), logs each lookup.

**Known scaling gap, flagged deliberately:** this mock has no cache or expiry semantics — every authenticated request triggers `PreferredUsernameRoleClaimsTransformation` calling `GetRoleByUserIdAsync` fresh (a dictionary lookup today). That's free at this scale, but if `UserRoles` is ever backed by a real table, this becomes a per-request database round-trip with no caching layer. Treat `IUserRolesRepository`'s interface as stable, but budget for adding a caching decorator (e.g. `IMemoryCache` keyed by `userId`, with a short TTL to bound staleness after a role change) when a real implementation replaces the mock — do not build that caching now, since it has nothing to cache against yet. See also the Assessment section.

**New `src/PilotApi.Shared/Constants/RoleNames.cs`**: `ReadOnly`, `ReadWrite`, `Admin` string constants. Lives in `Shared` since `Repositories`, `Services`, and `Shared` itself all need it.

**New `src/PilotApi.Shared/Constants/SecurityConstants.cs`**: `PreferredUsernameClaimType = "preferred_username"`, `WarningHeaderName = "Warning"` (reuse the exact header name already used ad hoc in `CustomersController.GetAll` etc. — `this.Response.Headers["Warning"] = ...`).

**New `src/PilotApi.Services/Security/PreferredUsernameRoleClaimsTransformation.cs`** — implements `IClaimsTransformation` (ASP.NET Core invokes this automatically after every successful `AuthenticateAsync()`, before authorization runs):
```csharp
public sealed class PreferredUsernameRoleClaimsTransformation : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal?.Identity?.IsAuthenticated != true) return principal!;
        if (principal.HasClaim(c => c.Type == ClaimTypes.Role)) return principal; // re-entrancy guard: TransformAsync can run more than once per request

        var userId = principal.FindFirst(SecurityConstants.PreferredUsernameClaimType)?.Value;
        if (string.IsNullOrWhiteSpace(userId)) { /* log warning */ return principal; }

        var role = await this.userRolesRepository.GetRoleByUserIdAsync(userId);
        if (string.IsNullOrWhiteSpace(role)) { /* log warning */ return principal; }

        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim(ClaimTypes.Role, role));
        principal.AddIdentity(identity);
        // log success
        return principal;
    }
}
```
Registered in `ServicesInjectionExtensions.ServicesRegistration` alongside the other registrations:
```csharp
builder.Services.AddTransient<IUserRolesRepository, UserRolesRepository>();
builder.Services.AddScoped<IClaimsTransformation, PreferredUsernameRoleClaimsTransformation>();
```

**Policy design — single dynamic requirement + `FallbackPolicy`** (recommended over 3 named per-verb policies):

`src/PilotApi.Shared/Api/Security/HttpVerbRoleRequirement.cs` — empty marker `IAuthorizationRequirement`.

`src/PilotApi.Shared/Api/Security/HttpVerbRoleAuthorizationHandler.cs`:
```csharp
public sealed class HttpVerbRoleAuthorizationHandler : AuthorizationHandler<HttpVerbRoleRequirement>
{
    private static readonly IReadOnlyDictionary<string, string[]> RoleVerbMap = new Dictionary<string, string[]>
    {
        [RoleNames.ReadOnly]  = ["GET"],
        [RoleNames.ReadWrite] = ["GET", "POST", "PUT"],
        [RoleNames.Admin]     = ["GET", "POST", "PUT", "DELETE"],
    };

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HttpVerbRoleRequirement requirement)
    {
        var httpContext = context.Resource as HttpContext
            ?? (context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext)?.HttpContext;
        // resolve role claim from context.User, compare against httpContext.Request.Method,
        // context.Succeed(requirement) or context.Fail(new AuthorizationFailureReason(this, "<explanation>"))
    }
}
```

Why this over per-verb `[Authorize(Policy=...)]` attributes on every controller action:
1. Business controllers today carry no `[Authorize]` at all — only `[AllowAnonymous]` (being removed). `AddAuthorization(o => o.FallbackPolicy = ...)` auto-applies to every endpoint lacking an explicit policy **except** `[AllowAnonymous]` ones, which always short-circuit. `SystemController`'s `[AllowAnonymous]` on `/healthcheck` and `/about` therefore needs zero extra code to stay untouched by any of this — those endpoints never reach the new handler regardless of the `Active` flag.
2. §7 (removing `[AllowAnonymous]` from `SimpleControllerBase`) becomes the *only* controller-level change — no per-verb attribute clutter across 8 controllers, and any future controller is secure-by-default.
3. The verb→role mapping is cumulative and keyed off the live HTTP method — one requirement/handler pair expresses this naturally via `httpContext.Request.Method`.

`RequireAuthenticatedUser()` is included in the same `FallbackPolicy` so a missing/invalid token fails via `DenyAnonymousAuthorizationRequirement` too.

## 6. Active-flag bypass with `Warning` header

**Recommended approach: decorate `IAuthorizationMiddlewareResultHandler`.** This is the single choke point `AuthorizationMiddleware` calls after evaluating a policy, and `PolicyAuthorizationResult` already distinguishes `Challenged` (auth missing/failed) from `Forbidden` (authenticated but insufficient role) from `Succeeded`. (Rejected alternatives: a custom `AuthenticationHandler` can't see authorization/role failures; custom middleware wrapping `UseAuthentication`/`UseAuthorization` would have to re-implement challenge-vs-forbid logic the framework already encapsulates.)

**New `src/PilotApi.Shared/Api/Security/BypassOnInactiveAuthorizationMiddlewareResultHandler.cs`**:
```csharp
public sealed class BypassOnInactiveAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();

    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Succeeded || this.keycloakConfiguration.Active)
        {
            await this.defaultHandler.HandleAsync(next, context, policy, authorizeResult);
            return;
        }

        // Keycloak.Active == false AND (authentication or authorization) failed -> bypass, but warn
        var warningMessage = BuildWarningMessage(context, authorizeResult);
        context.Response.Headers[SecurityConstants.WarningHeaderName] = warningMessage;
        this.logger.LogWarning("Security bypass: allowing {Method} {Path} through despite failed check because Keycloak.Active=false. Reason: {Reason}", ...);
        await next(context);
    }

    private static string BuildWarningMessage(HttpContext context, PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Forbidden)
        {
            var reasons = authorizeResult.AuthorizationFailure?.FailureReasons.Select(r => r.Message) ?? [];
            return reasons.Any() ? string.Join("; ", reasons) : "Insufficient role for the requested operation.";
        }

        // Challenged: AuthorizationFailure is null here; the real reason lives on the authentication feature.
        // Prefer the specific reason OnAuthenticationFailed stashed in HttpContext.Items (§4a — distinguishes
        // an expired token from a missing/malformed one), falling back to the raw AuthenticateResult failure.
        if (context.Items.TryGetValue("AuthFailureReason", out var reason) && reason is string reasonText)
        {
            return reasonText;
        }

        var authenticateResult = context.Features.Get<IAuthenticateResultFeature>()?.AuthenticateResult;
        return string.IsNullOrWhiteSpace(authenticateResult?.Failure?.Message)
            ? "Missing or invalid bearer token."
            : authenticateResult.Failure.Message;
    }
}
```
Registered via `services.AddSingleton<IAuthorizationMiddlewareResultHandler, BypassOnInactiveAuthorizationMiddlewareResultHandler>()` inside `AddSecurity` — this replaces ASP.NET Core's built-in default registration (last registration wins), so no `Replace()` call needed as long as it's registered after `AddAuthorization()`. Use the plain-string `Warning` header value (not RFC 7234's structured format), matching the existing `CustomersController` convention.

**Verify during implementation** (framework-internals note, should hold for current ASP.NET Core but confirm against the installed version): `AuthorizationMiddleware` only produces `PolicyAuthorizationResult.Forbid(failure)` (carrying `AuthorizationFailure`) when authentication succeeded but a requirement failed; if authentication itself failed/was absent, it short-circuits to `PolicyAuthorizationResult.Challenge()` with no attached failure — hence the two branches in `BuildWarningMessage`.

## 7. Controller changes

`src/PilotApi.Web/Controllers/Base/SimpleControllerBase.cs` — delete `[AllowAnonymous]` (and the `using Microsoft.AspNetCore.Authorization;` if nothing else in the file needs it). No `[Authorize]` needs adding (the `FallbackPolicy` covers it).

`src/PilotApi.Web/Controllers/SystemController.cs` — **no changes**. Its own `[AllowAnonymous]` and `[ApiVersionNeutral]` absolute routing (`/healthcheck`, `/about`) stay untouched.

## 8. Logging

- **Successful auth**: `JwtBearerEvents.OnTokenValidated` — resolve `ILogger` via `context.HttpContext.RequestServices.GetRequiredService<ILogger<...>>()`, log the `preferred_username` claim + remote IP. Never log the raw token.
- **Failed auth**: `JwtBearerEvents.OnAuthenticationFailed` — `LogWarning(context.Exception, ...)` with request path; redact any `Authorization` header value before it could appear in a log message. Distinguish `SecurityTokenExpiredException` ("Token expired") from other validation failures ("Token validation failed") in the log message, per §4a — this is the audit trail's only way to tell an expired-but-otherwise-legitimate token apart from a forged/malformed one.
- **Failed authorization (role/verb mismatch)**: `LogWarning` inside `HttpVerbRoleAuthorizationHandler` on the failure branch.
- **Bypass events**: `LogWarning` inside `BypassOnInactiveAuthorizationMiddlewareResultHandler` — this is the actual audit-relevant line ("something that should have been blocked was let through").
- **Redaction utility** — add to `src/PilotApi.Shared/Utilities/SecurityUtilities.cs`, following the exact pattern of the existing `ConnectionStringClean`:
```csharp
public static string BearerTokenClean(string? authorizationHeaderValue)
{
    if (string.IsNullOrWhiteSpace(authorizationHeaderValue)) return authorizationHeaderValue ?? string.Empty;
    return authorizationHeaderValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
        ? $"Bearer {StringConstants.Redacted}"
        : StringConstants.Redacted;
}
```
Reuses the existing `StringConstants.Redacted` constant — no new one needed.

## 9. Unit tests (NUnit + Moq, `<ClassUnderTest>_<Scenario>_<Expected>_Test` naming, derive from `PilotApi.TestingShared.TestBase`)

- `test/PilotApi.Shared.Tests/Configuration/KeycloakConfigurationTests.cs` (new) — mirror `OpenApiConfigurationTests.cs`: ctor defaults (incl. `ClockSkewSeconds` defaulting to `60`), copy-ctor incl. null-source throw, `Authority` computed property (trailing-slash handling), `Validate` per missing required field plus a negative-`ClockSkewSeconds` failure case, `ToString()`, null-`exceptions` throw.
- `test/PilotApi.Shared.Tests/Configuration/ApplicationConfigurationTests.cs` (modify) — add `Keycloak` init/copy/suppress + `Validate()` null/invalid cases, mirroring existing `OpenApi`/`OpenTelemetry` cases.
- `test/PilotApi.Shared.Tests/Api/Extensions/ApiExtensionsTests.cs` (modify, required — see §4) — fix the 5 breaking tests; add assertions that `AddSecurity` registers `IAuthorizationHandler`/`IAuthorizationMiddlewareResultHandler`/an authentication scheme.
- `test/PilotApi.Shared.Tests/Api/Security/HttpVerbRoleAuthorizationHandlerTests.cs` (new) — construct `AuthorizationHandlerContext` directly with a `ClaimsPrincipal` (with/without a role claim) and a `DefaultHttpContext { Request = { Method = "..." } }` resource; assert succeed/fail for each Role × Verb combo in the cumulative table (`ReadOnly`+`GET`→succeed, `ReadOnly`+`POST`→fail, `ReadWrite`+`DELETE`→fail, `Admin`+`DELETE`→succeed), plus no-role-claim and non-HttpContext-resource fail cases.
- `test/PilotApi.Shared.Tests/Api/Security/BypassOnInactiveAuthorizationMiddlewareResultHandlerTests.cs` (new) — Moq `IKeycloakConfiguration` (toggle `Active`) and a `RequestDelegate`; assert: `Active=true`+failed → `next` not invoked, no `Warning` header; `Active=false`+`Forbid` → `next` invoked, `Warning` header with the failure reason; `Active=false`+`Challenge` with no `AuthFailureReason` item → `next` invoked, generic-token `Warning` header; `Active=false`+`Challenge` with `HttpContext.Items["AuthFailureReason"] = "Token expired."` set → `Warning` header carries that exact expired-token message (§4a); `Succeeded` (any `Active`) → `next` invoked, no header.
- `test/PilotApi.Repositories.Tests/Repositories/UserRolesRepositoryTests.cs` (new) — each of the 3 known UserIds returns the expected role; unknown UserId returns null; case-insensitivity; null/empty `userId` throws `ArgumentException`.
- `test/PilotApi.Services.Tests/Security/PreferredUsernameRoleClaimsTransformationTests.cs` (new) — Moq `IUserRolesRepository`: unauthenticated principal unchanged (repo never called); authenticated with `preferred_username`="working_admin" gets `Role`="Admin" added; missing claim → unchanged + warning logged; unmapped user → unchanged; already-has-role-claim → repo not called again (re-entrancy guard).
- `test/PilotApi.Services.Tests/Extensions/ServicesInjectionExtensionsTests.cs` (modify) — assert `ServicesRegistration` resolves `IUserRolesRepository`/`IClaimsTransformation` without throwing.
- `test/PilotApi.Web.Tests/Controllers/Base/SimpleControllerBaseTests.cs` (modify) — reflection assertion that `[AllowAnonymous]` is gone from `SimpleControllerBase`.

**Run and validate**: `dotnet test` across the whole solution must pass 100% after these changes, per the explicit requirement.

## 10. README update

In the existing `## Usage` section (`README.md`, currently ~lines 475-485, Swagger/OpenAPI content only), add a `### Calling a Secured Endpoint` subsection before the closing `back to top` footer, in the same style as the rest of the doc:
- All endpoints are secured except `/healthcheck` and `/about`.
- `curl` example: obtain a token from `http://localhost:55001/realms/local-realm/protocol/openid-connect/token` (password grant, `client_id=local-client`), then call a business endpoint with `Authorization: Bearer <access_token>`.
- Explain the role table (`reader_user`→ReadOnly/GET, `working_user`→ReadWrite/GET+POST+PUT, `working_admin`→Admin/all) and that role comes from `preferred_username`, not Keycloak role claims.
- Explain `401` (no/invalid token) vs `403` (insufficient role) vs the `Active=false` bypass-with-`Warning`-header behavior.
- **Token expiry & refresh**: state plainly that the API only validates tokens and never issues or refreshes them. When a request fails with `401` because the access token expired, the client must call Keycloak's token endpoint again with `grant_type=refresh_token` (using the `refresh_token` value returned alongside the original `access_token`) to obtain a new `access_token`, then retry:
    ```
    curl -X POST "http://localhost:55001/realms/local-realm/protocol/openid-connect/token" \
      -H "Content-Type: application/x-www-form-urlencoded" \
      -d "grant_type=refresh_token" \
      -d "client_id=local-client" \
      -d "refresh_token=<refresh_token from the original token response>"
    ```
  Note the small clock-skew tolerance (`ClockSkewSeconds`, default 60s) applied when validating token expiry, so a token isn't rejected purely due to minor clock drift between the API host and Keycloak.

Also mirror the new `Keycloak` config section into the README's "Example configurations" `appsettings.json` block and add a `##### Keycloak` config-reference subsection matching the existing per-property doc style (one `#####` heading per property, next to `#### OpenApi`/`#### OpenTelemetry`).

## Verification

1. `dotnet build` the solution — confirm no compile errors (watch for the `FrameworkReference` contingency noted in §1).
2. `dotnet test` the whole solution — all tests pass, including the updated `ApiExtensionsTests` and all new test files from §9.
3. Manually run the API against local Keycloak (`http://localhost:55001`, realm `local-realm`, client `local-client`):
   - Call `/healthcheck` and `/about` with no token → 200 (unaffected).
   - Call a business `GET` endpoint with no token, `Keycloak.Active=true` → 401.
   - Call a business `GET` endpoint with a `reader_user` token → 200; same token against a `POST`/`PUT`/`DELETE` → 403.
   - Call with a `working_admin` token against `DELETE` → 200 (or whatever the normal success response is).
   - Set `Keycloak.Active=false`, repeat the no-token and insufficient-role cases → requests succeed but responses carry a `Warning` header.
4. Confirm `/about?show-details=true` still renders correctly with the new `Keycloak` section present and no secret leakage.

## Assessment (spec review for human — not to be implemented in this work)

- **No integration/end-to-end test coverage.** The solution has no `Microsoft.AspNetCore.Mvc.Testing`/`WebApplicationFactory` package, so this plan is scoped to pure unit tests (direct construction of `AuthorizationHandlerContext`/`PolicyAuthorizationResult`), not a real HTTP call through the full pipeline with a forged JWT. Consider a follow-up to add `Microsoft.AspNetCore.Mvc.Testing` (or the existing Aspire integration-testing pattern, if adopted elsewhere) so the active-flag bypass and role matrix are verified end-to-end, not just at the unit-handler level.
- **`test/PilotApi.Architecture.Tests/AttributesTests.cs` already contains two fully-commented-out ArchUnitNET tests** — `Anonymous_Controllers_Should_Not_Include_An_Authorize_Attribute_Test` and `Authenticated_Controllers_Should_Not_Include_An_Anonymous_Attribute_Test` — written in clear anticipation of this feature but currently disabled (the whole test project is presently inert). Re-enabling them would cheaply enforce "no controller accidentally gets both `[AllowAnonymous]` and `[Authorize]`" and "SystemController stays the only anonymous controller" as a standing architectural rule, rather than relying on manual review. Recommended as a fast-follow, not blocking.
- **Password Resource Owner grant in the README example** sends user passwords directly to Keycloak from the command line, which is fine for local dev docs but should not be the pattern recommended for any real client — worth a one-line caveat in the README that production/real clients should use Authorization Code + PKCE, not the password grant.
- **Token expiry and refresh are now addressed in the design (§4a)**: expiry is enforced via `ValidateLifetime` + a tunable `ClockSkewSeconds`, and expired-vs-invalid tokens are distinguished in both logs and the `Warning` header. Refresh is explicitly a client-to-Keycloak concern (documented in §10), never implemented server-side, since this API is a resource server, not a token issuer — implementing refresh-token handling here would mean taking on session state the API doesn't otherwise have.
- **The mock `IUserRolesRepository` has no cache/expiry semantics** (see the callout in §5) — every authenticated request does a dictionary lookup, which is free at this scale, but if `UserRoles` is ever backed by a real database, the per-request `IClaimsTransformation` call becomes a per-request DB round-trip with no caching layer. This is a known, deliberate scaling gap: budget for an `IMemoryCache`-backed decorator around a real `IUserRolesRepository` (short TTL, to bound staleness after a role change) when the mock is replaced — do not build the caching now, since there's nothing real to cache against yet.
- **Keycloak admin credentials in the source spec** (`my_admin`/`........`) are correctly excluded from the application's own configuration — they're for a human managing the Keycloak instance, not for the API. Noted here explicitly so it's clear this was a deliberate omission, not an oversight.

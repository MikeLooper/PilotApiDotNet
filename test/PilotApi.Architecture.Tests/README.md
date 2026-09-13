# PilotApi.Architecture.Tests

Architecture tests for the PilotApi solution, written with [ArchUnitNET](https://github.com/TNG/ArchUnitNET) and
run as NUnit tests. They analyze the compiled Domain, Repositories, Services, Shared, and Web assemblies and fail
the build when the codebase drifts from the rules below.

```bash
dotnet test test/PilotApi.Architecture.Tests/PilotApi.Architecture.Tests.csproj
```

## Layering

- Shared has no dependency on any other layer.
- Domain depends only on Shared.
- Repositories depends only on Domain and Shared.
- Services depends only on Domain, Repositories, and Shared.
- Web must reach the data store through Services, never Repositories directly.
- Within an allowed dependency, callers depend on interfaces, not concrete classes: Controllers on Service
  interfaces, Services on Repository interfaces, Repositories on the data-access interfaces. Each project's own
  DI-registration class is the accepted exception.
- Controllers never reference entity types - entities stay behind the Service/DTO boundary.
- No namespace cycles within a project (e.g. an implementation namespace depending back on its own contracts
  namespace).

## Third-party dependencies

- ASP.NET Core namespaces are banned from Domain and Repositories.
- Data-access libraries (Dapper, Microsoft.Data.SqlClient, the deprecated System.Data.SqlClient, Npgsql) are
  banned from Web and Domain.
- Test-only libraries (Microsoft.NET.Test.Sdk, Moq, NUnit) are banned from all application code.

## Naming and placement

- Interfaces are named `I*`.
- Service implementations end with `Service` and live in the Services project's own namespace.
- Repository implementations end with `Repository` and live in the Repositories project's own namespace.
- Configuration implementations end with `Configuration`; their interfaces live together under one namespace.
- DTOs end with `Dto`; entities end with `Entity`.
- Exception types end with `Exception`, live in one shared namespace, and are public.
- Static helper classes (`*Extensions`, `*Constants`) are actually static.
- Controllers end with `Controller` and live under the Web project's Controllers namespace.

## Base classes and construction

- Repository classes derive from the shared repository base class.
- Service classes derive from the shared service base class.
- Repository and service implementations each have exactly one public constructor.

## Method conventions

- Public async methods in Domain, Services, and Repositories end with `Async`. Controller actions are exempt,
  following ASP.NET Core convention.

## Controllers

- A controller marked anonymous never also requires authorization, and vice versa.
- Every controller declares its response content type, API version (or is explicitly version-neutral), and the
  `[ApiController]` attribute.
- Every versioned controller exposes the shared `ApiVersion` header.
- Every controller action requires the caller to be authorized, except the two public system endpoints
  (health check and about/info).

## Known, accepted exceptions

- The hard-coded user-role lookup repository is not required to derive from the shared repository base class,
  since it has no backing database entity.
- The Shared project's configuration classes and their interfaces have a known namespace cycle (the interfaces
  expose properties typed as concrete classes instead of other interfaces). Fixing it touches JSON binding
  behavior across several classes, so it's tracked as debt rather than enforced.

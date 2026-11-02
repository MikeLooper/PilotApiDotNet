# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

Proof-of-concept ASP.NET Core (.NET 10) REST API over the NorthWind database, using Dapper against SQL Server or PostgreSQL. The API contract is `shared/PilotSharedSource/OpenAPI/PilotApi_v1.yaml`, in a git submodule (`git submodule update --init --recursive`).

## Commands

```bash
dotnet build PilotApi.sln
dotnet test PilotApi.sln
dotnet test test/PilotApi.Shared.Tests --filter "FullyQualifiedName~RequestSettingsMiddlewareTests"   # one class
dotnet test test/PilotApi.Shared.Tests --filter "Name=RequestSettings_CallerName_Default_ShouldBeNull_Test"  # one test
dotnet test PilotApi.sln --settings .runsettings --collect "Code Coverage"
dotnet run --project src/PilotApi.Web   # needs User Secrets; see README "Local Development"
```

All tests, integration tests included, run without a database.

## Layout

Project references flow downward: `Web` (controllers) → `Services` (`ServiceBase` + DTO/entity mapping, all DI registration in `ServicesInjectionExtensions`) → `Repositories` (`RepositoryBase`, Dapper) → `Domain` (DTOs, interfaces, `RetrieveResponse<T>`) → `Shared` (config, pipeline setup in `ApiExtensions`, middleware, security, OpenAPI, OTEL/Serilog, `SqlBuilder`). A new domain feature touches every layer; use the `domain-feature-scaffold` skill.

## Code style

Match existing code over the generic rules in `.github/instructions/coding.instructions.md`: block-scoped namespaces, tabs, `this.`-qualified members, dependencies as protected get-only properties (no `_camelCase` fields), XML docs on all public/protected members. Every database or external operation must be logged and carry the CorrelationId.

Topic-specific guidance lives in `.claude/rules/` and loads when you work on matching files.

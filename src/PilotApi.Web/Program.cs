using Microsoft.AspNetCore.Builder;
using PilotApi.Shared.Api.Extensions;
using PilotApi.Web.Extensions;
using Serilog;
using System;

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.CreateBootstrapLogger();

try
{
	Log.Information("Starting server.");

	// ** app: create
	var webAppBuilder = WebApplication.CreateBuilder(args);

	// ** shared: setup
	// configuration and services registrations
	webAppBuilder.ApplicationRegistration();
	// controllers, security, versioning, logging, OpenTelemetry, OpenAPI
	webAppBuilder.ApiWebApplicationBuilder();

	// ** app: build
	var webApp = webAppBuilder.Build();

	// ** shared: setup
	// logging, OpenAPI, Swagger, security, middleware
	webApp.ApiWebApplication();

	// ** app: run
	webApp.Run();
}
catch (Exception ex)
{
	Log.Fatal(ex, "Server terminated unexpectedly.");
}
finally
{
	Log.CloseAndFlush();
}

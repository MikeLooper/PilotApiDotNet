using Microsoft.AspNetCore.Builder;
using Serilog;
using System;

namespace PilotApi.Shared.Logging.Extensions
{
	/// <summary>
	/// Extension methods for logging.
	/// </summary>
	public static class LoggingExtensions
	{
		/// <summary>
		/// Logging usage for the application.
		/// </summary>
		/// <param name="webApp">
		/// A web application object.
		/// </param>
		/// <example>
		/// Example usage:
		/// <code>
		/// // app: create
		/// var webAppBuilder = WebApplication.CreateBuilder(args);
		/// 
		/// // app: build
		/// var webApp = webAppBuilder.Build();
		/// 
		/// // shared: setups
		/// webApp.LoggingWebApplication();
		/// </code>
		/// </example>
		public static void LoggingWebApplication(this WebApplication webApp)
		{
			if (webApp == null)
			{
				throw new ArgumentException($"Invalid argument : {nameof(webApp)}. "
					+ $"A valid object type of: '{typeof(WebApplication)}' is needed to continue. ({nameof(LoggingExtensions)})");
			}

			webApp.UseSerilogRequestLogging();
		}

		/// <summary>
		/// Logging setup for the application.
		/// </summary>
		/// <param name="builder">
		/// A <see cref="WebApplicationBuilder"/> object.
		/// </param>
		/// <example>
		/// Example usage:
		/// <code>
		/// // app: create
		/// var webAppBuilder = WebApplication.CreateBuilder(args);
		/// 
		/// // shared: setups
		/// webAppBuilder.LoggingWebApplicationBuilder();
		/// </code>
		/// </example>
		public static void LoggingWebApplicationBuilder(this WebApplicationBuilder builder)
		{
			if (builder == null)
			{
				throw new ArgumentException($"Invalid argument : {nameof(builder)}. "
					+ $"A valid object type of: '{typeof(WebApplicationBuilder)}' is needed to continue. ({nameof(LoggingExtensions)})");
			}

			// writeToProviders: true is required so log events also flow into the
			// OpenTelemetry logging provider registered in OpenTelemetryWebApplicationBuilder()
			// (.WithLogging(...).AddOtlpExporter(...)). Without it, Serilog owns the
			// Microsoft.Extensions.Logging pipeline exclusively and no logs ever reach the
			// OTEL collector / Loki, even though Console and File sinks keep working.
			builder.Services.AddSerilog((services, loggerConfig) => loggerConfig
						.ReadFrom.Configuration(builder.Configuration)
						.ReadFrom.Services(services)
						.Enrich.FromLogContext()
						//.WriteTo.Console()
						,
						writeToProviders: true);
		}
	}
}

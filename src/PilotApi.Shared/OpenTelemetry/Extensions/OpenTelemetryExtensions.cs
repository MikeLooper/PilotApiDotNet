using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PilotApi.Shared.Contracts.Configuration;
using System;

namespace PilotApi.Shared.OpenApi.Extensions
{
	/// <summary>
	/// Extension methods for Open Telemetry.
	/// </summary>
	public static class OpenTelemetryExtensions
	{
		/// <summary>
		/// Set up OpenTelemetry services for the application.
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
		/// // shared: setup
		/// webAppBuilder.OpenTelemetryWebApplicationBuilder();
		/// </code>
		/// </example>
		public static void OpenTelemetryWebApplicationBuilder(this WebApplicationBuilder builder)
		{
			if (builder == null)
			{
				throw new ArgumentException($"Invalid argument : {nameof(builder)}. "
					+ $"A valid object type of: '{typeof(WebApplicationBuilder)}' is needed to continue. ({nameof(OpenTelemetryExtensions)})");
			}

			/* Note: ConsoleExporter is used for demo purpose only. In production
				environment, ConsoleExporter should be replaced with other exporters
				(e.g. OTLP Exporter). */

			var serviceProvider = builder.Services.BuildServiceProvider();
			var applicationConfiguration = serviceProvider.GetRequiredService<IApplicationConfiguration>();

			using var tracerProvider = Sdk.CreateTracerProviderBuilder()
					.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(
						serviceName: applicationConfiguration.OpenApi.Title,
						serviceVersion: applicationConfiguration.OpenApi.Version))
					.SetSampler(new AlwaysOnSampler())
					// This optional activates tracing for your application, if you trace your own activities:
					.AddSource("PilotApi.Web")
					// This activates up Npgsql's tracing:
					.AddNpgsql()
					// This activates tracing for incoming HTTP requests:
					.AddHttpClientInstrumentation()
					// This prints tracing data to the console:
					.AddConsoleExporter()
					//.AddOtlpExporter()
					.Build();

			var meterProvider = Sdk.CreateMeterProviderBuilder()
					.AddMeter(applicationConfiguration.OpenApi.Title)
					// rest of config not shown here.
					//.AddOtlpExporter()
					.Build();

			builder.Services.AddOpenTelemetry()
					.ConfigureResource(r => r.AddService(
						serviceName: applicationConfiguration.OpenApi.Title,
						serviceVersion: applicationConfiguration.OpenApi.Version))
					.WithLogging(logging =>	logging
									.AddConsoleExporter()
									//.AddOtlpExporter()
					)
					.WithMetrics(metrics => metrics
									.AddAspNetCoreInstrumentation()
									//.AddHttpClientInstrumentation()
									.AddSqlClientInstrumentation()
									.AddConsoleExporter()
									//.AddOtlpExporter()
					)
					.WithTracing(tracing => tracing
									.AddAspNetCoreInstrumentation()
									//.AddHttpClientInstrumentation()
									.AddSqlClientInstrumentation()
									.AddConsoleExporter()
									//.AddOtlpExporter()
					);
		}
	}
}

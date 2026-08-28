using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PilotApi.Shared.Configuration;
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
		/// <param name="serviceProvider">
		/// A <see cref="IServiceProvider"/> object.
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
		public static void OpenTelemetryWebApplicationBuilder(this WebApplicationBuilder builder, IServiceProvider serviceProvider)
		{
			if (builder == null)
			{
				throw new ArgumentException($"Invalid argument : {nameof(builder)}. "
					+ $"A valid object type of: '{typeof(WebApplicationBuilder)}' is needed to continue. ({nameof(OpenTelemetryExtensions)})");
			}

			var applicationConfiguration = serviceProvider.GetRequiredService<IApplicationConfiguration>();

			// Confirmed via curl: the collector's OTLP/HTTP receiver on this port serves
			// plain HTTP (POST /v1/traces -> 200); HTTPS fails at the TLS handshake.
			var otelBaseUrl = $"http://{applicationConfiguration.OpenTelemetry.Server}:{applicationConfiguration.OpenTelemetry.Port}";

			// The OTLP/HTTP protobuf protocol requires each signal to be posted to its own
			// path (/v1/traces, /v1/metrics, /v1/logs). The SDK only fills these in automatically
			// when Endpoint is left untouched, so an explicit Endpoint must include the suffix
			// itself, or the collector receives every request on "/" and silently drops it.
			void ConfigureOtlpExporter(OtlpExporterOptions options, string signalPath)
			{
				options.Endpoint = new Uri($"{otelBaseUrl}/{signalPath}");
				options.Protocol = OtlpExportProtocol.HttpProtobuf;
			}

			builder.Services.AddOpenTelemetry()
					.ConfigureResource(r => r.AddService(
						serviceName: applicationConfiguration.OpenApi.Title,
						serviceVersion: applicationConfiguration.OpenApi.Version))
					.WithLogging(logging =>			// send records to Grafana Loki via OpenTelemetry Collector
									{
										logging.AddOtlpExporter(options => ConfigureOtlpExporter(options, "v1/logs"));

										if (builder.Environment.IsDevelopment())
										{
											logging.AddConsoleExporter();
										}
									}
					)
					.WithMetrics(metrics =>			// send records to Grafana Mimir (Prometheus) via OpenTelemetry Collector
									{
										metrics
												.AddMeter(applicationConfiguration.OpenApi.Title)
												.AddAspNetCoreInstrumentation()
												.AddHttpClientInstrumentation()
												.AddSqlClientInstrumentation()
												.AddOtlpExporter(options => ConfigureOtlpExporter(options, "v1/metrics"));

										if (builder.Environment.IsDevelopment())
										{
											metrics.AddConsoleExporter();
										}
									}
					)
					.WithTracing(tracing =>			// send records to Grafana Tempo via OpenTelemetry Collector
									{
										tracing
												.SetSampler(new AlwaysOnSampler())
												.AddSource("PilotApi.Web")		// This activates tracing for your application, if you trace your own activities
												.AddNpgsql()					// This activates Npgsql's tracing
												.AddAspNetCoreInstrumentation()
												.AddHttpClientInstrumentation()
												.AddSqlClientInstrumentation()
												.AddOtlpExporter(options => ConfigureOtlpExporter(options, "v1/traces"));

										if (builder.Environment.IsDevelopment())
										{
											tracing.AddConsoleExporter();
										}
									}
					);
		}
	}
}

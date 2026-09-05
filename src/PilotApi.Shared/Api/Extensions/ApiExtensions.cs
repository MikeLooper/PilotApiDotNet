using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PilotApi.Shared.Api.Middleware;
using PilotApi.Shared.Api.Transformers;
using PilotApi.Shared.Handlers;
using PilotApi.Shared.Helpers;
using PilotApi.Shared.Logging.Extensions;
using PilotApi.Shared.OpenApi.Extensions;
using PilotApi.Shared.Swagger.Extensions;
using System;

namespace PilotApi.Shared.Api.Extensions
{
	/// <summary>
	/// Extension methods for APIs.
	/// </summary>
	public static class ApiExtensions
	{
		/// <summary>
		/// Add versioning processing.
		/// </summary>
		/// <param name="services">
		/// A list of service objects.
		/// </param>
		public static void AddVersioning(this IServiceCollection services)
		{
			services.AddApiVersioning(options =>
			{
				options.ApiVersionReader = ApiVersionReader.Combine(
					new HeaderApiVersionReader("api-version"),
					new QueryStringApiVersionReader(),
					new UrlSegmentApiVersionReader());
				options.AssumeDefaultVersionWhenUnspecified = true;
				options.DefaultApiVersion = new ApiVersion(1, 0);
				options.ReportApiVersions = true;
			})
			.AddApiExplorer(options =>
			{
				options.GroupNameFormat = "'v'VVV";
				options.SubstituteApiVersionInUrl = true;
			});
		}

		/// <summary>
		/// WebApplication usage for the API.
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
		/// var webApp = webAppBuilder.ApiWebApplication();
		///
		/// // shared: setups
		/// webApp.UseLogging();
		/// </code>
		/// </example>
		public static void ApiWebApplication(this WebApplication webApp)
		{
			if (webApp == null)
			{
				throw new ArgumentException($"Invalid argument : {nameof(webApp)}. "
					+ $"A valid object type of: '{typeof(WebApplication)}' is needed to continue. ({nameof(ApiExtensions)})");
			}

			if (webApp.Environment.IsDevelopment())
			{
				webApp.UseHttpsRedirection();
			}

			// custom
			webApp.OpenApiWebApplication();
			webApp.LoggingWebApplication();
			webApp.SwaggerWebApplication();

			webApp.UseMiddleware<UnhandledExceptionMiddleware>();


			// standard
			webApp.UseSecurity();
			try
			{
				webApp.MapControllers();
			}
			catch (InvalidOperationException)
			{
				// Controllers not registered; skip mapping
			}
		}

		/// <summary>
		/// WebApplicationBuilder setup for the application.
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
		/// webAppBuilder.ApiWebApplicationBuilder();
		/// </code>
		/// </example>
		public static void ApiWebApplicationBuilder(this WebApplicationBuilder builder)
		{
			if (builder == null)
			{
				throw new ArgumentException($"Invalid argument : {nameof(builder)}. "
					+ $"A valid object type of: '{typeof(WebApplicationBuilder)}' is needed to continue. ({nameof(ApiExtensions)})");
			}

			// security
			var securityHelper = new SecurityHelper(builder.Services);
			securityHelper.AddSecurity();

			// standard
			builder.Services.AddVersioning();
			builder.Services.AddControllers(options =>
			{
				options.Conventions.Add(
					new RouteTokenTransformerConvention(new LowercaseParameterTransformer()));
			});

			// custom
			var serviceProvider = builder.Services.BuildServiceProvider();
			builder.OpenTelemetryWebApplicationBuilder(serviceProvider);
			builder.OpenApiWebApplicationBuilder(serviceProvider);
			builder.LoggingWebApplicationBuilder();

			// services
			builder.Services.AddTransient<ISqlBuilder, SqlBuilder>();
		}

		/// <summary>
		/// Add security processing via the WebApplication.
		/// </summary>
		/// <param name="webApp">
		/// A WebApplication object.
		/// </param>
		public static void UseSecurity(this WebApplication webApp)
		{
			if (webApp == null)
			{
				throw new ArgumentException($"Invalid argument : {nameof(webApp)}. "
					+ $"A valid object type of: '{typeof(WebApplication)}' is needed to continue. ({nameof(ApiExtensions)})");
			}

			webApp.UseAuthentication();
			webApp.UseAuthorization();
		}
	}
}

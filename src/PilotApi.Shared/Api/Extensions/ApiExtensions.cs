using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PilotApi.Shared.Api.Middleware;
using PilotApi.Shared.Handlers;
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
		/// Add security processing via the ServiceCollection.
		/// </summary>
		/// <param name="services">
		/// A list of service objects.
		/// </param>
		public static void AddSecurity(this IServiceCollection services)
		{
			//services.AddAuthentication(options =>
			//{
			//	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			//	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			//})
			//.AddJwtBearer(options =>
			//{
			//	options.TokenValidationParameters = new TokenValidationParameters
			//	{
			//		ValidateIssuer = true,
			//		ValidateAudience = true,
			//		ValidateLifetime = true,
			//		ValidateIssuerSigningKey = true,
			//		ValidIssuer = builder.Configuration["Jwt:Issuer"],
			//		ValidAudience = builder.Configuration["Jwt:Audience"],
			//		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
			//	};
			//});

			//services.AddAuthorization();
		}

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
				options.DefaultApiVersion = new ApiVersion(1, 0);
				options.AssumeDefaultVersionWhenUnspecified = true;
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
					+ $"A valid object type of: '{typeof(IServiceCollection)}' is needed to continue. ({nameof(ApiExtensions)})");
			}

			// standard
			builder.Services.AddControllers();
			builder.Services.AddSecurity();
			builder.Services.AddVersioning();

			// custom
			builder.LoggingWebApplicationBuilder();
			builder.OpenTelemetryWebApplicationBuilder();
			builder.OpenApiWebApplicationBuilder();

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

			//webApp.UseAuthentication();
			//webApp.UseAuthorization();
		}
	}
}

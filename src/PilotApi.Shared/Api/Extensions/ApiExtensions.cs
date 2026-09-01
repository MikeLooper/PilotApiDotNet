using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PilotApi.Shared.Api.Middleware;
using PilotApi.Shared.Api.Security;
using PilotApi.Shared.Api.Transformers;
using PilotApi.Shared.Constants;
using PilotApi.Shared.Contracts.Configuration;
using PilotApi.Shared.Handlers;
using PilotApi.Shared.Logging.Extensions;
using PilotApi.Shared.OpenApi.Extensions;
using PilotApi.Shared.Swagger.Extensions;
using PilotApi.Shared.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

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
		/// <param name="securityConfiguration">
		/// A Security configuration object.
		/// </param>
		public static void AddSecurity(this IServiceCollection services, ISecurityConfiguration securityConfiguration)
		{
			if (services == null)
			{
				throw new ArgumentException($"Invalid argument : {nameof(services)}. "
					+ $"A valid object type of: '{typeof(IServiceCollection)}' is needed to continue. ({nameof(ApiExtensions)})");
			}

			if (securityConfiguration == null)
			{
				throw new ArgumentException($"Invalid argument : {nameof(securityConfiguration)}. "
					+ $"A valid object type of: '{typeof(ISecurityConfiguration)}' is needed to continue. ({nameof(ApiExtensions)})");
			}

			// The order of configuration matters (authentication before authorization)
			ConfigureAuthentication(services, securityConfiguration);
			ConfigureAuthorization(services);

			RegisterSecurityServices(services, securityConfiguration);
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
					+ $"A valid object type of: '{typeof(IServiceCollection)}' is needed to continue. ({nameof(ApiExtensions)})");
			}

			// custom: build a temporary provider early so security setup can resolve the already-registered
			// application configuration (registered by ServicesConfiguration(), which runs before this method)
			IApplicationConfiguration applicationConfiguration;
			using (var configurationServiceProvider = builder.Services.BuildServiceProvider())
			{
				applicationConfiguration = configurationServiceProvider.GetRequiredService<IApplicationConfiguration>();
			}

			// standard
			builder.Services.AddSecurity(applicationConfiguration.Security!);
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

		/// <summary>
		/// Configures JWT Bearer authentication scheme.
		/// </summary>
		/// <param name="services">
		/// A list of service objects.
		/// </param>
		/// <param name="securityConfiguration">
		/// A Security configuration object.
		/// </param>
		private static void ConfigureAuthentication(IServiceCollection services, ISecurityConfiguration securityConfiguration)
		{
			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.Authority = securityConfiguration.Authority;
				options.Audience = securityConfiguration.ClientId;
				options.RequireHttpsMetadata = securityConfiguration.RequireHttpsMetadata;
				options.MapInboundClaims = false; // keep raw "preferred_username" claim name
				options.TokenValidationParameters = ConfigureTokenValidationParameters(securityConfiguration);
				options.Events = ConfigureJwtBearerEvents();
			});
		}

		/// <summary>
		/// Configures authorization policy and fallback requirements.
		/// </summary>
		/// <param name="services">
		/// A list of service objects.
		/// </param>
		private static void ConfigureAuthorization(IServiceCollection services)
		{
			services.AddAuthorization(options =>
			{
				options.FallbackPolicy = new AuthorizationPolicyBuilder()
					.RequireAuthenticatedUser()
					.AddRequirements(new HttpVerbRoleRequirement())
					.Build();
			});
		}

		/// <summary>
		/// Configures JWT Bearer authentication event handlers.
		/// </summary>
		/// <returns>
		/// Configured JwtBearerEvents.
		/// </returns>
		private static JwtBearerEvents ConfigureJwtBearerEvents()
		{
			return new JwtBearerEvents
			{
				OnMessageReceived = HandleOnMessageReceived,
				OnTokenValidated = HandleOnTokenValidated,
				OnAuthenticationFailed = HandleOnAuthenticationFailed
			};
		}

		/// <summary>
		/// Configures JWT token validation parameters.
		/// </summary>
		/// <param name="securityConfiguration">
		/// A Security configuration object.
		/// </param>
		/// <returns>
		/// Configured TokenValidationParameters.
		/// </returns>
		private static TokenValidationParameters ConfigureTokenValidationParameters(ISecurityConfiguration securityConfiguration)
		{
			return new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidIssuer = securityConfiguration.Authority,
				ValidateAudience = true,
				AudienceValidator = (audiences, token, validationParameters) =>
				{
					var audienceList = audiences?.ToArray() ?? [];
					var clientId = securityConfiguration.ClientId;

					return audienceList.Contains(clientId, StringComparer.OrdinalIgnoreCase)
						|| audienceList.Contains("account", StringComparer.OrdinalIgnoreCase);
				},
				ValidateLifetime = true,
				ClockSkew = TimeSpan.FromSeconds(securityConfiguration.ClockSkewSeconds),
				ValidateIssuerSigningKey = true,
				NameClaimType = SecurityConstants.PreferredUsernameClaimType
			};
		}

		/// <summary>
		/// Handles the OnAuthenticationFailed event - logs authentication failures.
		/// </summary>
		private static Task HandleOnAuthenticationFailed(AuthenticationFailedContext context)
		{
			var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(ApiExtensions));
			var isExpired = context.Exception is SecurityTokenExpiredException;
			var redactedAuthorization = SecurityUtilities.BearerTokenClean(context.Request.Headers.Authorization.ToString());

			logger.LogWarning(
				context.Exception,
				"{FailureReason} for {Method} {Path}. Authorization: {Authorization}",
				isExpired ? "Token expired" : "Token validation failed",
				context.Request.Method,
				context.Request.Path,
				redactedAuthorization);

			context.HttpContext.Items["AuthFailureReason"] = isExpired ? "Token expired." : "Missing or invalid bearer token.";

			return Task.CompletedTask;
		}

		/// <summary>
		/// Handles the OnMessageReceived event - logs token reception before validation.
		/// </summary>
		private static Task HandleOnMessageReceived(MessageReceivedContext context)
		{
			var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(ApiExtensions));
			var authorizationString = context.Request.Headers.Authorization.ToString();
			var redactedAuthorization = SecurityUtilities.BearerTokenClean(authorizationString);
			var redactedJwt = SecurityUtilities.CleanJwtToken(authorizationString);

			logger.LogInformation(
				"Authentication token received for {Method} {Path}. Authorization: {Authorization}. JWT: {Jwt}. RemoteIp: '{RemoteIp}'",
				context.Request.Method,
				context.Request.Path,
				redactedAuthorization,
				redactedJwt,
				context.HttpContext.Connection.RemoteIpAddress);

			return Task.CompletedTask;
		}

		/// <summary>
		/// Handles the OnTokenValidated event - logs successful authentication.
		/// </summary>
		private static Task HandleOnTokenValidated(TokenValidatedContext context)
		{
			var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(ApiExtensions));
			var authorizationString = context.Request.Headers.Authorization.ToString();
			context.Principal = SecurityUtilities.GetJwtPrincipal(context.Principal, authorizationString);
			context.HttpContext.User = context.Principal ?? context.HttpContext.User;

			var userId = context.Principal?.FindFirst(SecurityConstants.PreferredUsernameClaimType)?.Value;

			logger.LogInformation(
				"Authentication succeeded for UserId: '{UserId}' from RemoteIp: '{RemoteIp}'",
				userId ?? StringConstants.LogNull,
				context.HttpContext.Connection.RemoteIpAddress);

			return Task.CompletedTask;
		}

		/// <summary>
		/// Registers security-related singleton services.
		/// </summary>
		/// <param name="services">
		/// A list of service objects.
		/// </param>
		/// <param name="securityConfiguration">
		/// A Security configuration object.
		/// </param>
		private static void RegisterSecurityServices(IServiceCollection services, ISecurityConfiguration securityConfiguration)
		{
			services.AddSingleton(securityConfiguration);
			services.AddSingleton<IAuthorizationHandler, HttpVerbRoleAuthorizationHandler>();
			services.AddSingleton<IAuthorizationMiddlewareResultHandler, BypassOnInactiveAuthorizationMiddlewareResultHandler>();
		}
	}
}

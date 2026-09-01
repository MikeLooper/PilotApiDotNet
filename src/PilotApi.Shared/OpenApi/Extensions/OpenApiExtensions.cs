using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using PilotApi.Shared.Models;
using PilotApi.Shared.OpenApi.Transformers;
using Scalar.AspNetCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PilotApi.Shared.OpenApi.Extensions
{
	/// <summary>
	/// Extension methods for OpenApi.
	/// </summary>
	public static class OpenApiExtensions
	{
		/// <summary>
		/// Set up OpenApi services for the application.
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
		/// webAppBuilder.OpenApiWebApplicationBuilder();
		/// </code>
		/// </example>
		public static void OpenApiWebApplicationBuilder(this WebApplicationBuilder builder, IServiceProvider serviceProvider)
		{
			if (builder == null)
			{
				throw new ArgumentException($"Invalid argument : {nameof(builder)}. "
					+ $"A valid object type of: '{typeof(WebApplicationBuilder)}' is needed to continue. ({nameof(OpenApiExtensions)})");
			}

			builder.Services.AddOutputCache(options =>
			{
				options.AddBasePolicy(policy => policy.Expire(TimeSpan.FromMinutes(10)));
			});

			var apiDescriptions = serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
			var actionDescriptorCollectionProvider = serviceProvider.GetRequiredService<IActionDescriptorCollectionProvider>();
			var controllerDescriptors = actionDescriptorCollectionProvider.ActionDescriptors.Items
												.OfType<ControllerActionDescriptor>()
												.ToList();
			var controllerClasses = controllerDescriptors
				.Select(descriptor => descriptor.ControllerTypeInfo.AsType())
				.Distinct()
				.Select(type => new ControllerAttributes
				{
					Name = type.Name,
					FullName = type.FullName,
					AssemblyName = type.Assembly.GetName().Name,
					Type = type
				})
				.ToList();

			// register each discovered API version
			if (apiDescriptions.ApiVersionDescriptions.Count > 0)
			{
				foreach (var description in apiDescriptions.ApiVersionDescriptions)
				{
					// get list of controllers for this version
					var versionControllers = new List<ControllerAttributes>();
					foreach (var controller in controllerClasses)
					{
						var apiVersionAttribute = controller.Type?.GetCustomAttribute<ApiVersionAttribute>();
						if (apiVersionAttribute != null && 
							apiVersionAttribute.Versions.Any(v => v.MajorVersion == description.ApiVersion.MajorVersion))
						{
							versionControllers.Add(controller);
						}
					}

					builder.Services.AddOpenApi($"v{description.ApiVersion.MajorVersion}", options =>
					{
						// Specify the OpenAPI version to use
						options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_1;

						// transformers
						options.AddOperationTransformer<GlobalOperationTransformer>();
						options.AddDocumentTransformer(new DocumentInfoTransformer(serviceProvider, description.ApiVersion));

						var xmlFilename = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
						var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
						options.AddOperationTransformer(new ManualXmlCommentsOperationTransformer(xmlPath));
					});
				}
			}
		}

		/// <summary>
		/// Set up OpenApi services for the application.
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
		/// webApp.OpenApiWebApplication();
		/// </code>
		/// </example>
		public static void OpenApiWebApplication(this WebApplication webApp)
		{
			if (webApp == null)
			{
				throw new ArgumentException($"Invalid argument : {nameof(webApp)}. "
					+ $"A valid object type of: '{typeof(WebApplication)}' is needed to continue. ({nameof(OpenApiExtensions)})"); 
			}

			webApp.UseOutputCache();

			// production code would include the following check to only enable OpenApi in development environments
			//if (webApp.Environment.IsDevelopment())

			webApp.MapOpenApi()
						.AllowAnonymous()
						.CacheOutput();

			// set display to look similar to Swagger
			webApp.MapScalarApiReference(options =>
			{
				//var apiDescriptions = webApp.Services.GetRequiredService<IApiVersionDescriptionProvider>();
				//// version switching enabling
				//foreach (var description in apiDescriptions.ApiVersionDescriptions)
				//{
				//	options.AddDocument($"/v{description.ApiVersion.MajorVersion}",
				//		$"API Version {description.ApiVersion.MajorVersion}, {description.ApiVersion.MinorVersion}",
				//		$"/openapi/v{description.ApiVersion.MajorVersion}.json");  //, isDefault: true);
				//}

				// Disables the AI "Agent" feature entirely
				options.DisableAgent();

				// Apply the classic three-column layout
				options.Layout = ScalarLayout.Classic;

				//// Inject CSS variables to match the classic Swagger UI aesthetic
				//var uiCss = ResourceUtilities.ReturnFullyQualifiedResourceFileAsTextFromCallingAssembly(
				// OpenApiDisplayConstants .CustomCssFolderFilePath);
				//options.WithCustomCss{uiCss);
				options.WithTheme(ScalarTheme.BluePlanet);
			})
			.AllowAnonymous();
		}
	}
}

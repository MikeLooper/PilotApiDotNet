using Asp.Versioning;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using PilotApi.Shared.Contracts.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PilotApi.Shared.OpenApi.Transformers
{
	/// <summary>
	/// A tranformer for assugning Info details.
	/// </summary>
	public class DocumentInfoTransformer : IOpenApiDocumentTransformer
	{
		/// <summary>
		/// Instantiate a <see cref="DocumentInfoTransformer"/> object.
		/// </summary>
		/// <param name="serviceProvider">
		/// A service provider object.
		/// </param>
		/// <param name="apiVersion">
		/// An API version object.
		/// </param>
		public DocumentInfoTransformer(
			IServiceProvider serviceProvider,
			ApiVersion apiVersion)
		{
			this.ServiceProvider = serviceProvider;
			this.ApiVersion = apiVersion;
		}

		/// <summary>
		/// Gets an API version object.
		/// </summary>
		public ApiVersion ApiVersion { get; }

		/// <summary>
		/// Gets a service provider object.
		/// </summary>
		public IServiceProvider ServiceProvider { get; }

		/// <inheritdoc/>
		public Task TransformAsync(
			OpenApiDocument document,
			OpenApiDocumentTransformerContext context,
			CancellationToken cancellationToken)
		{
			var applicationConfiguration = this.ServiceProvider.GetRequiredService<IApplicationConfiguration>();

			// Set the primary info section fields
			document.Info.Title = applicationConfiguration.OpenApi.Title;
			document.Info.Version = this.ApiVersion.MajorVersion.ToString();

			//var summaryText = $"{applicationConfiguration.OpenApi.Summary} (v: {applicationConfiguration.OpenApi.Version})";

			//// Scalar's UI does not render Info.Summary (only Info.Description is displayed), while Swagger UI
			//// renders both Info.Summary and Info.Description. To avoid the summary appearing twice in Swagger
			//// UI while still surfacing it in Scalar, merge the summary into the description and leave
			//// Info.Summary unset.
			//document.Info.Description = string.IsNullOrWhiteSpace(applicationConfiguration.OpenApi.Description)
			//	? summaryText
			//	: $"{summaryText}\n\n{applicationConfiguration.OpenApi.Description}";
			
			document.Info.Description = applicationConfiguration.OpenApi.Description;
			document.Info.Summary = applicationConfiguration.OpenApi.Summary;
			//document.Info.TermsOfService = new Uri("https://example.com");

			// Set developer contact info
			document.Info.Contact = new OpenApiContact
			{
				Name = applicationConfiguration.OpenApi.Contact.Name,
				Email = applicationConfiguration.OpenApi.Contact.Email,
				Url = new Uri(applicationConfiguration.OpenApi.Contact.URL)
			};

			// Set legal licensing details
			document.Info.License = new OpenApiLicense
			{
				Name = applicationConfiguration.OpenApi.License,
				Url = new Uri("https://opensource.org")
			};

			return Task.CompletedTask;
		}
	}
}

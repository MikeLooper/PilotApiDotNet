using Microsoft.AspNetCore.OpenApi;
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
		public DocumentInfoTransformer(IApplicationConfiguration applicationConfiguration)
		{
			this.ApplicationConfiguration = applicationConfiguration;
		}

		/// <summary>
		/// Gets an application configuration object.
		/// </summary>
		public IApplicationConfiguration? ApplicationConfiguration { get; }

		/// <inheritdoc/>
		public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
		{
			// Set the primary info section fields
			document.Info.Title = this.ApplicationConfiguration.OpenApi.Title;
			document.Info.Version = this.ApplicationConfiguration.OpenApi.Version;
			document.Info.Description = this.ApplicationConfiguration.OpenApi.Description;
			document.Info.Summary = this.ApplicationConfiguration.OpenApi.Summary;
			//document.Info.TermsOfService = new Uri("https://example.com");

			// Set developer contact info
			document.Info.Contact = new OpenApiContact
			{
				Name = this.ApplicationConfiguration.OpenApi.Contact.Name,
				Email = this.ApplicationConfiguration.OpenApi.Contact.Email,
				Url = new Uri(this.ApplicationConfiguration.OpenApi.Contact.URL)
			};

			// Set legal licensing details
			document.Info.License = new OpenApiLicense
			{
				Name = this.ApplicationConfiguration.OpenApi.License,
				Url = new Uri("https://opensource.org")
			};

			return Task.CompletedTask;
		}
	}
}

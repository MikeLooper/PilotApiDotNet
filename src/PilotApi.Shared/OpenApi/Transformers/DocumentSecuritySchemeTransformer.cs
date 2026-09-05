using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PilotApi.Shared.OpenApi.Transformers
{
	/// <summary>
	/// Adds the shared security scheme to the OpenAPI document.
	/// </summary>
	public class DocumentSecuritySchemeTransformer : IOpenApiDocumentTransformer
	{
		/// <inheritdoc/>
		public Task TransformAsync(
			OpenApiDocument document,
			OpenApiDocumentTransformerContext context,
			CancellationToken cancellationToken)
		{
			document.Components ??= new OpenApiComponents();
			document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
			document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
			{
				Type = SecuritySchemeType.Http,
				Scheme = "bearer",
				BearerFormat = "JWT",
				Description = "JWT bearer authentication"
			};

			return Task.CompletedTask;
		}
	}
}

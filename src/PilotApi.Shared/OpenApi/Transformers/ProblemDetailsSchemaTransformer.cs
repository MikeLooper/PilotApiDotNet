using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PilotApi.Shared.OpenApi.Transformers
{
	/// <summary>
	/// Ensures ProblemDetails-based schemas declare 'type' and 'title' as required properties,
	/// matching the shared OpenAPI specification.
	/// </summary>
	public class ProblemDetailsSchemaTransformer : IOpenApiSchemaTransformer
	{
		/// <inheritdoc/>
		public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
		{
			if (typeof(ProblemDetails).IsAssignableFrom(context.JsonTypeInfo.Type))
			{
				schema.Required ??= new HashSet<string>();
				schema.Required.Add("type");
				schema.Required.Add("title");
			}

			return Task.CompletedTask;
		}
	}
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace PilotApi.Shared.OpenApi.Transformers
{
	/// <summary>
	/// Global OpenApi operations.
	/// </summary>
	public class GlobalOperationTransformer : IOpenApiOperationTransformer
	{
		/// <inheritdoc/>
		public async Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
		{
			// remove query parameter
			if (operation.Parameters != null)
			{
				var versionParameter = operation.Parameters
							.FirstOrDefault(p =>
										!string.IsNullOrWhiteSpace(p.Name) &&
										p.Name.Equals("api-version", StringComparison.OrdinalIgnoreCase));

				if (versionParameter != null)
				{
					operation.Parameters.Remove(versionParameter);
				}
			}

			if (context.Description.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
			{
				operation.Parameters ??= [];

				var headerProperties = controllerActionDescriptor.ControllerTypeInfo
					.AsType()
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
					.Select(property => new
					{
						Property = property,
						FromHeader = property.GetCustomAttribute<FromHeaderAttribute>()
					})
					.Where(x => x.FromHeader != null);

				foreach (var headerProperty in headerProperties)
				{
					var headerName = string.IsNullOrWhiteSpace(headerProperty.FromHeader!.Name)
						? headerProperty.Property.Name
						: headerProperty.FromHeader.Name;

					if (operation.Parameters.Any(p => p.Name.Equals(headerName, StringComparison.OrdinalIgnoreCase)))
					{
						continue;
					}

					operation.Parameters.Add(new OpenApiParameter
					{
						Name = headerName,
						In = ParameterLocation.Header,
						Required = headerProperty.Property.GetCustomAttribute<RequiredAttribute>() != null,
						Schema = new OpenApiSchema
						{
							Type = JsonSchemaType.String
						}
					});
				}
			}

			return;
		}
	}
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PilotApi.Shared.OpenApi.Transformers
{
	/// <summary>
	/// Custom Operation Transformer that manually parses the XML file 
	/// to bind endpoint summaries and descriptions.
	/// </summary>
	public class ManualXmlCommentsOperationTransformer : IOpenApiOperationTransformer
	{
		/// <summary>
		/// An XML document.
		/// </summary>
		protected readonly XDocument _xmlDoc;

		/// <summary>
		/// Instantiate a <see cref="ManualXmlCommentsOperationTransformer"/> object.
		/// </summary>
		/// <param name="xmlPath">
		/// The path of the assembly XML file.
		/// </param>
		public ManualXmlCommentsOperationTransformer(string xmlPath)
		{
			if (xmlPath == null)
			{
				throw new ArgumentNullException(nameof(xmlPath));
			}

			if (string.IsNullOrWhiteSpace(xmlPath))
			{
				throw new InvalidOperationException($"The {nameof(xmlPath)} argument cannot be empty ({nameof(ManualXmlCommentsOperationTransformer)})");
			}

			_xmlDoc = XDocument.Load(xmlPath);
		}

		/// <inheritdoc/>
		public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
		{
			// Safely pull the ActionDescriptor to access the underlying controller/minimal API method info
			MethodInfo? methodInfo = null;
			if (context.Description.ActionDescriptor is ControllerActionDescriptor cad)
			{
				methodInfo = cad.MethodInfo;
			}

			if (methodInfo == null)
			{
				return Task.CompletedTask;
			}

			// Generate the standard XML documentation member ID format for the method
			// Format: M:Namespace.ClassName.MethodName(ParamType1,ParamType2)
			string memberName = GetXmlMemberKey(methodInfo);

			// Query the loaded XML for the corresponding member tag
			var memberNode = _xmlDoc.Descendants("member")
				.FirstOrDefault(m => m.Attribute("name")?.Value == memberName);

			if (memberNode != null)
			{
				var summaryText = memberNode.Element("summary")?.Value?.Trim();
				var remarksText = memberNode.Element("remarks")?.Value?.Trim();

				if (!string.IsNullOrEmpty(summaryText))
				{
					operation.Summary = summaryText;
				}

				if (!string.IsNullOrEmpty(remarksText))
				{
					operation.Description = remarksText;
				}

				if (operation.Parameters != null)
				{
					var methodParameters = methodInfo.GetParameters();

					foreach (var parameter in operation.Parameters)
					{
						// Find the corresponding method parameter, accounting for binding attributes that
						// rename the parameter (e.g. [FromQuery(Name = "show-details")]) so the OpenAPI
						// parameter name may not match the C# parameter name used in the XML doc comments.
						var methodParameter = methodParameters.FirstOrDefault(p =>
							GetBoundParameterName(p).Equals(parameter.Name, StringComparison.OrdinalIgnoreCase));

						var xmlParamName = methodParameter?.Name ?? parameter.Name;

						var paramNode = memberNode.Elements("param")
							.FirstOrDefault(p => p.Attribute("name")?.Value == xmlParamName);

						var paramDescription = paramNode?.Value?.Trim();
						if (!string.IsNullOrEmpty(paramDescription))
						{
							parameter.Description = paramDescription;
						}
					}
				}
			}

			return Task.CompletedTask;
		}

		/// <summary>
		/// Using the method info object, build the name of the current class method.
		/// </summary>
		/// <param name="methodInfo"></param>
		/// <returns></returns>
		protected static string GetXmlMemberKey(MethodInfo methodInfo)
		{
			var typeName = methodInfo.DeclaringType?.FullName?.Replace("+", ".");
			var methodName = methodInfo.Name;

			var parameters = methodInfo.GetParameters();
			if (parameters.Length == 0)
			{
				return $"M:{typeName}.{methodName}";
			}

			// Format parameter type names to perfectly match standard XML compiler outputs
			var paramStrings = parameters.Select(p => p.ParameterType.FullName ?? p.ParameterType.Name);
			return $"M:{typeName}.{methodName}({string.Join(",", paramStrings)})";
		}

		/// <summary>
		/// Gets the effective bound name for a method parameter, taking into account any
		/// binding source attribute (<see cref="FromQueryAttribute"/>, <see cref="FromRouteAttribute"/>,
		/// <see cref="FromHeaderAttribute"/>) that may rename the parameter as it appears in the OpenAPI document.
		/// </summary>
		/// <param name="parameter">
		/// The method parameter to inspect.
		/// </param>
		/// <returns>
		/// The bound name of the parameter.
		/// </returns>
		protected static string GetBoundParameterName(ParameterInfo parameter)
		{
			var fromQuery = parameter.GetCustomAttribute<FromQueryAttribute>();
			if (!string.IsNullOrWhiteSpace(fromQuery?.Name))
			{
				return fromQuery.Name;
			}

			var fromRoute = parameter.GetCustomAttribute<FromRouteAttribute>();
			if (!string.IsNullOrWhiteSpace(fromRoute?.Name))
			{
				return fromRoute.Name;
			}

			var fromHeader = parameter.GetCustomAttribute<FromHeaderAttribute>();
			if (!string.IsNullOrWhiteSpace(fromHeader?.Name))
			{
				return fromHeader.Name;
			}

			return parameter.Name ?? string.Empty;
		}
	}
}

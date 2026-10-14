using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PilotApi.Shared.OpenApi.Transformers
{
	/// <summary>
	/// Custom Schema Transformer that manually parses the XML file(s)
	/// to bind property summaries onto generated schemas.
	/// </summary>
	public class ManualXmlCommentsSchemaTransformer : IOpenApiSchemaTransformer
	{
		/// <summary>
		/// A cache of loaded XML documents, keyed by assembly location.
		/// </summary>
		protected readonly ConcurrentDictionary<string, XDocument?> xmlDocCache = new();

		/// <summary>
		/// Instantiate a <see cref="ManualXmlCommentsSchemaTransformer"/> object.
		/// </summary>
		/// <param name="xmlPath">
		/// The path of the entry assembly XML file.
		/// </param>
		public ManualXmlCommentsSchemaTransformer(string xmlPath)
		{
			if (xmlPath == null)
			{
				throw new ArgumentNullException(nameof(xmlPath));
			}

			if (string.IsNullOrWhiteSpace(xmlPath))
			{
				throw new InvalidOperationException($"The {nameof(xmlPath)} argument cannot be empty ({nameof(ManualXmlCommentsSchemaTransformer)})");
			}

			this.xmlDocCache[xmlPath] = XDocument.Load(xmlPath);
		}

		/// <inheritdoc/>
		public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
		{
			var type = context.JsonTypeInfo.Type;

			var xmlDoc = GetXmlDocForAssembly(type);
			if (xmlDoc == null || schema.Properties == null)
			{
				return Task.CompletedTask;
			}

			foreach (var schemaProperty in schema.Properties)
			{
				var propertyInfo = type.GetProperty(schemaProperty.Key,
					System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);

				if (propertyInfo == null)
				{
					continue;
				}

				var memberName = $"P:{propertyInfo.DeclaringType?.FullName}.{propertyInfo.Name}";

				var memberNode = xmlDoc.Descendants("member")
					.FirstOrDefault(m => m.Attribute("name")?.Value == memberName);

				var summaryText = memberNode?.Element("summary")?.Value?.Trim();
				if (!string.IsNullOrEmpty(summaryText))
				{
					schemaProperty.Value.Description = summaryText;
				}
			}

			return Task.CompletedTask;
		}

		/// <summary>
		/// Gets the XML documentation document associated with the given type's assembly, loading and caching it as needed.
		/// </summary>
		/// <param name="type">
		/// A type whose assembly's XML documentation file should be located.
		/// </param>
		/// <returns>
		/// The loaded <see cref="XDocument"/>, or null if no corresponding XML file could be found.
		/// </returns>
		protected XDocument? GetXmlDocForAssembly(Type type)
		{
			var assemblyLocation = type.Assembly.Location;
			if (string.IsNullOrWhiteSpace(assemblyLocation))
			{
				return null;
			}

			var xmlPath = Path.ChangeExtension(assemblyLocation, ".xml");

			return this.xmlDocCache.GetOrAdd(xmlPath, path => File.Exists(path) ? XDocument.Load(path) : null);
		}
	}
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using NUnit.Framework;
using PilotApi.Shared.OpenApi.Transformers;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace PilotApi.Shared.Tests.OpenApi.Transformers
{
	[TestFixture]
	public class ProblemDetailsSchemaTransformerTests : TestBase
	{
		[Test]
		public async Task ProblemDetailsSchemaTransformer_TransformAsync_RequiresTypeAndTitleForProblemDetails_Test()
		{
			// Arrange
			var transformer = new ProblemDetailsSchemaTransformer();
			var schema = new OpenApiSchema();
			var context = CreateContext(typeof(ProblemDetails));

			// Act
			await transformer.TransformAsync(schema, context, default);

			// Assert
			Assert.That(schema.Required, Does.Contain("type"));
			Assert.That(schema.Required, Does.Contain("title"));
		}

		[Test]
		public async Task ProblemDetailsSchemaTransformer_TransformAsync_LeavesUnrelatedSchemasUnchanged_Test()
		{
			// Arrange
			var transformer = new ProblemDetailsSchemaTransformer();
			var schema = new OpenApiSchema();
			var context = CreateContext(typeof(string));

			// Act
			await transformer.TransformAsync(schema, context, default);

			// Assert
			Assert.That(schema.Required, Is.Null);
		}

		private static OpenApiSchemaTransformerContext CreateContext(Type type)
		{
			var context = (OpenApiSchemaTransformerContext)Activator.CreateInstance(typeof(OpenApiSchemaTransformerContext))!;
			var jsonTypeInfo = JsonSerializerOptions.Default.GetTypeInfo(type);
			typeof(OpenApiSchemaTransformerContext).GetProperty(nameof(OpenApiSchemaTransformerContext.JsonTypeInfo))!
				.SetValue(context, jsonTypeInfo);

			return context;
		}
	}
}

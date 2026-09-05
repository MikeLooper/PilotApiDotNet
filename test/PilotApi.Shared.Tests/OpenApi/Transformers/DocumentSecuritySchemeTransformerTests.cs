using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using NUnit.Framework;
using PilotApi.Shared.OpenApi.Transformers;
using System;
using System.Threading.Tasks;

namespace PilotApi.Shared.Tests.OpenApi.Transformers
{
	[TestFixture]
	public class DocumentSecuritySchemeTransformerTests : TestBase
	{
		[Test]
		public async Task DocumentSecuritySchemeTransformer_Transformer_AddsBearerSecurityScheme_Test()
		{
			// Arrange
			var transformer = new DocumentSecuritySchemeTransformer();
			var document = new OpenApiDocument();
			var context = (OpenApiDocumentTransformerContext)Activator.CreateInstance(typeof(OpenApiDocumentTransformerContext))!;
			typeof(OpenApiDocumentTransformerContext).GetProperty(nameof(OpenApiDocumentTransformerContext.DocumentName))!.SetValue(context, "v1");
			typeof(OpenApiDocumentTransformerContext).GetProperty(nameof(OpenApiDocumentTransformerContext.ApplicationServices))!.SetValue(context, null);
			typeof(OpenApiDocumentTransformerContext).GetProperty(nameof(OpenApiDocumentTransformerContext.DescriptionGroups))!.SetValue(context, null);

			// Act
			await transformer.TransformAsync(document, context, default);

			// Assert
			Assert.That(document.Components, Is.Not.Null);
			Assert.That(document.Components.SecuritySchemes, Is.Not.Null);
			Assert.That(document.Components.SecuritySchemes.ContainsKey("Bearer"), Is.True);
			var securityScheme = document.Components.SecuritySchemes["Bearer"];
			Assert.That(securityScheme.Type, Is.EqualTo(SecuritySchemeType.Http));
			Assert.That(securityScheme.Scheme, Is.EqualTo("bearer"));
			Assert.That(securityScheme.BearerFormat, Is.EqualTo("JWT"));
		}
	}
}


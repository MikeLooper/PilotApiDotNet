using NUnit.Framework;
using PilotApi.Shared.Constants;
using PilotApi.Shared.Contracts.Configuration;
using PilotApi.Shared.OpenApi.Transformers;
using PilotApi.TestingShared.Utilities;
using System;

namespace PilotApi.Shared.Tests.OpenApi.Transformers
{
	[TestFixture]
	public class DocumentInfoTransformerTests
	{
		private IApplicationConfiguration applicationConfiguration;
		private DocumentInfoTransformer transformer;

		[SetUp]
		public void Setup()
		{
			applicationConfiguration = TestingSharedDoublesUtilities.GetApplicationConfiguration(DataSourceTypes.SqlServer);
			applicationConfiguration.OpenApi.Title = "Test API";
			applicationConfiguration.OpenApi.Version = "1.0.0";
			applicationConfiguration.OpenApi.Description = "Test Description";
			applicationConfiguration.OpenApi.Summary = "Test Summary";
			applicationConfiguration.OpenApi.License = "MIT";
			applicationConfiguration.OpenApi.Contact.Name = "Support Team";
			applicationConfiguration.OpenApi.Contact.Email = "support@example.com";
			applicationConfiguration.OpenApi.Contact.URL = "https://example.com/support";

			transformer = new DocumentInfoTransformer(applicationConfiguration);
		}

		[Test]
		public void DocumentInfoTransformer_Constructor_WithValidApplicationConfiguration_ShouldInitialize_Test()
		{
			// Arrange & Act - constructor called in Setup

			// Assert
			Assert.NotNull(transformer);
			Assert.NotNull(transformer.ApplicationConfiguration);
		}

		[Test]
		public void DocumentInfoTransformer_Constructor_WithNullApplicationConfiguration_ShouldInitializeWithNull_Test()
		{
			// Arrange & Act
			var transformerWithNull = new DocumentInfoTransformer(null);

			// Assert
			Assert.NotNull(transformerWithNull);
			Assert.Null(transformerWithNull.ApplicationConfiguration);
		}

		[Test]
		public void DocumentInfoTransformer_ApplicationConfiguration_ShouldBeAccessible_Test()
		{
			// Arrange & Act
			var config = transformer.ApplicationConfiguration;

			// Assert
			Assert.NotNull(config);
			Assert.That(config.OpenApi.Title, Is.EqualTo("Test API"));
			Assert.That(config.OpenApi.Version, Is.EqualTo("1.0.0"));
			Assert.That(config.OpenApi.Description, Is.EqualTo("Test Description"));
		}

		[Test]
		public void DocumentInfoTransformer_Transformer_IsIOpenApiDocumentTransformer_Test()
		{
			// Arrange & Act
			var isTransformer = transformer is Microsoft.AspNetCore.OpenApi.IOpenApiDocumentTransformer;

			// Assert
			Assert.That(isTransformer, Is.True);
		}
	}
}


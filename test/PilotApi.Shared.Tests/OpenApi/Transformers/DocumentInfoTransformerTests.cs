using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PilotApi.Shared.Constants;
using PilotApi.Shared.Contracts.Configuration;
using PilotApi.Shared.OpenApi.Transformers;
using PilotApi.TestingShared.Utilities;

namespace PilotApi.Shared.Tests.OpenApi.Transformers
{
	[TestFixture]
	public class DocumentInfoTransformerTests : TestBase
	{
		[Test]
		public void DocumentInfoTransformer_ApplicationConfiguration_ShouldBeAccessible_Test()
		{
			// Arrange
			var applicationConfiguration = TestingSharedDoublesUtilities.GetApplicationConfiguration(DataSourceTypes.SqlServer);
			var services = new ServiceCollection();
			services.AddSingleton<IApplicationConfiguration>(applicationConfiguration);
			var serviceProvider = services.BuildServiceProvider();
			var apiVersion = new ApiVersion(2, 0);
			var transformer = new DocumentInfoTransformer(serviceProvider, apiVersion);

			// Act
			var resolvedConfiguration = transformer.ServiceProvider.GetService<IApplicationConfiguration>();

			// Assert
			Assert.NotNull(resolvedConfiguration);
			Assert.That(transformer.ApiVersion.MajorVersion, Is.EqualTo(2));
		}

		[Test]
		public void DocumentInfoTransformer_Constructor_WithNullApplicationConfiguration_ShouldInitializeWithNull_Test()
		{
			// Arrange
			var apiVersion = new ApiVersion(1, 0);

			// Act
			var transformerWithNull = new DocumentInfoTransformer(null, apiVersion);

			// Assert
			Assert.NotNull(transformerWithNull);
			Assert.That(transformerWithNull.ServiceProvider, Is.Null);
			Assert.That(transformerWithNull.ApiVersion, Is.EqualTo(apiVersion));
		}

		[Test]
		public void DocumentInfoTransformer_Constructor_WithValidApplicationConfiguration_ShouldInitialize_Test()
		{
			// Arrange
			var applicationConfiguration = TestingSharedDoublesUtilities.GetApplicationConfiguration(DataSourceTypes.SqlServer);
			var services = new ServiceCollection();
			services.AddSingleton<IApplicationConfiguration>(applicationConfiguration);
			var serviceProvider = services.BuildServiceProvider();
			var apiVersion = new ApiVersion(1, 0);

			// Act
			var transformer = new DocumentInfoTransformer(serviceProvider, apiVersion);

			// Assert
			Assert.NotNull(transformer);
			Assert.That(transformer.ServiceProvider, Is.EqualTo(serviceProvider));
			Assert.That(transformer.ApiVersion, Is.EqualTo(apiVersion));
		}

		[Test]
		public void DocumentInfoTransformer_Transformer_IsIOpenApiDocumentTransformer_Test()
		{
			// Arrange
			var applicationConfiguration = TestingSharedDoublesUtilities.GetApplicationConfiguration(DataSourceTypes.SqlServer);
			var services = new ServiceCollection();
			services.AddSingleton<IApplicationConfiguration>(applicationConfiguration);
			var serviceProvider = services.BuildServiceProvider();
			var apiVersion = new ApiVersion(1, 0);
			var transformer = new DocumentInfoTransformer(serviceProvider, apiVersion);

			// Act
			var isTransformer = transformer is Microsoft.AspNetCore.OpenApi.IOpenApiDocumentTransformer;

			// Assert
			Assert.That(isTransformer, Is.True);
		}
	}
}


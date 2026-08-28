using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using PilotApi.Shared.Configuration;
using PilotApi.Shared.Constants;
using PilotApi.TestingShared.Utilities;
using PilotApi.Web.Controllers;
using System.Reflection;

namespace PilotApi.Web.Tests.Controllers
{
	[TestFixture]
	public class SystemControllerTests
	{
		[Test]
		public void SystemController_About_ShowDetailsFalse_ReturnsMetadataWithoutConfigurationSettings_Test()
		{
			// Arrange
			var applicationConfiguration = TestingSharedDoublesUtilities.GetApplicationConfiguration(DataSourceTypes.SqlServer);
			applicationConfiguration.OpenApi.Title = "PilotApi";
			applicationConfiguration.OpenApi.Version = "1.2.3";
			var controller = new SystemController(applicationConfiguration);

			// Act
			var result = controller.About(false) as OkObjectResult;
			var valueType = result?.Value?.GetType();
			var nameProperty = valueType?.GetProperty("Name");
			var appVersionProperty = valueType?.GetProperty("ApiVersion");
			var buildVersionProperty = valueType?.GetProperty("BuildVersion");
			var deployDateProperty = valueType?.GetProperty("DeployDate");
			var configurationSettingsProperty = valueType?.GetProperty("ConfigurationSettings");

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result?.StatusCode ?? 200, Is.EqualTo(200));
			Assert.That(nameProperty?.GetValue(result?.Value), Is.EqualTo("PilotApi"));
			Assert.That(appVersionProperty?.GetValue(result?.Value), Is.EqualTo("1.2.3"));
			Assert.That(buildVersionProperty, Is.Not.Null);
			Assert.That(deployDateProperty, Is.Not.Null);
			Assert.That(configurationSettingsProperty, Is.Null);
		}

		[Test]
		public void SystemController_About_ShowDetailsTrue_ReturnsMetadataWithConfigurationSettings_Test()
		{
			// Arrange
			var applicationConfiguration = TestingSharedDoublesUtilities.GetApplicationConfiguration(DataSourceTypes.SqlServer);
			applicationConfiguration.OpenApi.Title = "PilotApi";
			applicationConfiguration.OpenApi.Version = "1.2.3";
			applicationConfiguration.DataConnections[0].Password = "SecretValue";
			var controller = new SystemController(applicationConfiguration);

			// Act
			var result = controller.About(true) as OkObjectResult;
			var valueType = result?.Value?.GetType();
			var configurationSettingsProperty = valueType?.GetProperty("ApplicationConfiguration");
			var configurationSettingsValue = configurationSettingsProperty?.GetValue(result?.Value);
			var cleanedConfiguration = configurationSettingsValue as ApplicationConfiguration;

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result?.StatusCode ?? 200, Is.EqualTo(200));
			Assert.That(configurationSettingsProperty, Is.Not.Null);
			Assert.That(cleanedConfiguration, Is.Not.Null);
			Assert.That(cleanedConfiguration?.DataConnections, Is.Not.Null);
			Assert.That(cleanedConfiguration?.DataConnections?.Count, Is.EqualTo(1));
			Assert.That(cleanedConfiguration?.DataConnections?[0].Password, Is.EqualTo("[Redacted]"));
		}

		[Test]
		public void SystemController_Constructor_WithApplicationConfiguration_SetsProtectedApplicationConfigurationProperty_Test()
		{
			// Arrange
			var applicationConfiguration = TestingSharedDoublesUtilities.GetApplicationConfiguration(DataSourceTypes.SqlServer);

			// Act
			var controller = new SystemController(applicationConfiguration);
			var applicationConfigurationProperty = typeof(SystemController)
				.GetProperty("ApplicationConfiguration", BindingFlags.Instance | BindingFlags.NonPublic);
			var propertyValue = applicationConfigurationProperty?.GetValue(controller);

			// Assert
			Assert.That(applicationConfigurationProperty, Is.Not.Null);
			Assert.That(propertyValue, Is.SameAs(applicationConfiguration));
		}
	}
}

using NUnit.Framework;
using PilotApi.Shared.Configuration;
using PilotApi.Shared.Exceptions;
using System;

namespace PilotApi.Shared.Tests.Configuration
{
	[TestFixture]
	public class ApplicationConfigurationTests
	{
		[Test]
		public void ApplicationConfiguration_Active_ShouldHaveDefaultValueOfTrue_Test()
		{
			// Arrange & Act
			var config = new ApplicationConfiguration();

			// Assert
			Assert.That(config.Active, Is.True);
		}

		[Test]
		public void ApplicationConfiguration_Constructor_ShouldInitializeProperties_Test()
		{
			// Arrange & Act
			var config = new ApplicationConfiguration();

			// Assert
			Assert.NotNull(config.DataConnections);
			Assert.That(config.DataConnections.Count, Is.EqualTo(0));
			Assert.NotNull(config.DataSources);
			Assert.That(config.DataSources.Count, Is.EqualTo(0));
			Assert.NotNull(config.OpenApi);
			Assert.That(config.Active, Is.True);
		}
		[Test]
		public void ApplicationConfiguration_ConstructorWithSourceConfigurationAndSuppressSensitiveValuesTrue_RedactsNestedPassword_Test()
		{
			// Arrange
			var sourceConfiguration = new ApplicationConfiguration
			{
				Active = false,
				OpenApi = new OpenApiConfiguration
				{
					Active = true,
					Title = "PilotApi",
					Version = "1.0.0",
					Description = "description",
					Summary = "summary",
					License = "MIT",
					Contact = new OpenApiContactConfiguration
					{
						Active = true,
						Name = "Support",
						Email = "support@example.com",
						URL = "https://example.com"
					}
				}
			};
			sourceConfiguration.DataConnections.Add(new DataConnectionConfiguration
			{
				Active = true,
				DataSourceName = "Primary",
				Host = "localhost",
				Password = "VerySecret",
				UserName = "sa",
				Port = 1433,
				ConnectTimeout = 15
			});
			sourceConfiguration.DataSources.Add(new DataSourceConfiguration
			{
				Active = true,
				DataSource = "MainDB",
				DataSourceName = "Primary",
				DataSourceType = "SqlServer",
				Schema = "dbo"
			});

			// Act
			var result = new ApplicationConfiguration(sourceConfiguration, true);

			// Assert
			Assert.That(result.Active, Is.False);
			Assert.That(result.DataConnections, Is.Not.Null);
			Assert.That(result.DataSources, Is.Not.Null);
			Assert.That(result.OpenApi, Is.Not.Null);
			Assert.That(result.DataConnections.Count, Is.EqualTo(1));
			Assert.That(result.DataConnections[0].Password, Is.EqualTo("[Redacted]"));
			Assert.That(result.DataSources.Count, Is.EqualTo(1));
			Assert.That(result.OpenApi.Title, Is.EqualTo("PilotApi"));
			Assert.That(result.DataConnections[0], Is.Not.SameAs(sourceConfiguration.DataConnections[0]));
			Assert.That(result.DataSources[0], Is.Not.SameAs(sourceConfiguration.DataSources[0]));
		}

		[Test]
		public void ApplicationConfiguration_ConstructorWithSourceConfigurationNull_ThrowsArgumentException_Test()
		{
			// Arrange
			ApplicationConfiguration sourceConfiguration = null;

			// Act
			TestDelegate action = () =>
			{
				new ApplicationConfiguration(sourceConfiguration);
			};

			// Assert
			var exception = Assert.Throws<ArgumentException>(action);
			Assert.That(exception?.Message, Does.Contain("sourceConfiguration"));
		}

		[Test]
		public void ApplicationConfiguration_GetDataSource_WithDifferentCase_ShouldReturnNull_Test()
		{
			// Arrange
			var config = new ApplicationConfiguration();
			var dataSource = new DataSourceConfiguration
			{
				Active = true,
				DataSourceName = "TestSource",
				DataSource = "TestDB",
				DataSourceType = "SqlServer",
				Schema = "dbo"
			};
			config.DataSources.Add(dataSource);

			// Act
			var result = config.GetDataSource("testsource");

			// Assert
			Assert.Null(result);
		}

		[Test]
		public void ApplicationConfiguration_GetDataSource_WithNonExistentDataSourceName_ShouldReturnNull_Test()
		{
			// Arrange
			var config = new ApplicationConfiguration();

			// Act
			var result = config.GetDataSource("NonExistent");

			// Assert
			Assert.Null(result);
		}

		[Test]
		public void ApplicationConfiguration_GetDataSource_WithValidDataSourceName_ShouldReturnDataSource_Test()
		{
			// Arrange
			var config = new ApplicationConfiguration();
			var dataSource = new DataSourceConfiguration
			{
				Active = true,
				DataSourceName = "TestSource",
				DataSource = "TestDB",
				DataSourceType = "SqlServer",
				Schema = "dbo"
			};
			config.DataSources.Add(dataSource);

			// Act
			var result = config.GetDataSource("TestSource");

			// Assert
			Assert.NotNull(result);
			Assert.That(result.DataSourceName, Is.EqualTo("TestSource"));
		}
		[Test]
		public void ApplicationConfiguration_ToString_ShouldReturnFormattedString_Test()
		{
			// Arrange
			var config = new ApplicationConfiguration();
			config.Active = true;

			// Act
			var result = config.ToString();

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("Active=True"));
			Assert.That(result, Does.Contain("DataConnections="));
			Assert.That(result, Does.Contain("DataSources="));
			Assert.That(result, Does.Contain("OpenApi="));
		}

		[Test]
		public void ApplicationConfiguration_Validate_CalledMultipleTimes_ShouldSucceed_Test()
		{
			// Arrange
			var config = new ApplicationConfiguration();

			var dataConnection = new DataConnectionConfiguration
			{
				Active = true,
				DataSourceName = "Primary",
				Host = "localhost",
				Password = "password",
				UserName = "user"
			};
			config.DataConnections.Add(dataConnection);

			var dataSource = new DataSourceConfiguration
			{
				Active = true,
				DataSourceName = "Primary",
				DataSource = "MainDB",
				DataSourceType = "SqlServer",
				Schema = "dbo"
			};
			config.DataSources.Add(dataSource);

			config.OpenApi = new OpenApiConfiguration
			{
				Active = true,
				Title = "API",
				Version = "1.0",
				Description = "Test",
				Summary = "Test API",
				License = "MIT",
				Contact = new OpenApiContactConfiguration
				{
					Active = true,
					Name = "Support",
					Email = "support@example.com",
					URL = "https://example.com"
				}
			};

			// Act & Assert - calling validate twice should not throw
			Assert.DoesNotThrow(() => config.Validate());
			Assert.DoesNotThrow(() => config.Validate());
		}

		[Test]
		public void ApplicationConfiguration_Validate_WithMismatchedDataSourceName_ShouldThrow_Test()
		{
			// Arrange
			var config = new ApplicationConfiguration();

			var dataConnection = new DataConnectionConfiguration
			{
				Active = true,
				DataSourceName = "Primary",
				Host = "localhost",
				Password = "password",
				UserName = "user"
			};
			config.DataConnections.Add(dataConnection);

			var dataSource = new DataSourceConfiguration
			{
				Active = true,
				DataSourceName = "Secondary",
				DataSource = "MainDB",
				DataSourceType = "SqlServer",
				Schema = "dbo"
			};
			config.DataSources.Add(dataSource);

			config.OpenApi = new OpenApiConfiguration
			{
				Active = true,
				Title = "API",
				Version = "1.0",
				Description = "Test",
				Summary = "Test API",
				License = "MIT",
				Contact = new OpenApiContactConfiguration
				{
					Active = true,
					Name = "Support",
					Email = "support@example.com",
					URL = "https://example.com"
				}
			};

			// Act & Assert
			var exception = Assert.Throws<ConfigurationException>(() => config.Validate());
			Assert.That(exception.Message, Does.Contain("does not match"));
		}

		[Test]
		public void ApplicationConfiguration_Validate_WithMultipleActiveDataConnections_ShouldThrow_Test()
		{
			// Arrange
			var config = new ApplicationConfiguration();

			var connection1 = new DataConnectionConfiguration
			{
				Active = true,
				DataSourceName = "Primary",
				Host = "localhost",
				Password = "password",
				UserName = "user"
			};
			var connection2 = new DataConnectionConfiguration
			{
				Active = true,
				DataSourceName = "Secondary",
				Host = "localhost",
				Password = "password",
				UserName = "user"
			};
			config.DataConnections.Add(connection1);
			config.DataConnections.Add(connection2);

			// Act & Assert
			var exception = Assert.Throws<ConfigurationException>(() => config.Validate());
			Assert.That(exception.Message, Does.Contain("one active item"));
		}

		[Test]
		public void ApplicationConfiguration_Validate_WithoutDataConnections_ShouldThrow_Test()
		{
			// Arrange
			var config = new ApplicationConfiguration();
			config.DataConnections = null;

			// Act & Assert
			var exception = Assert.Throws<ConfigurationException>(() => config.Validate());
			Assert.That(exception.Message, Does.Contain("DataConnections"));
		}

		[Test]
		public void ApplicationConfiguration_Validate_WithoutDataSources_ShouldThrow_Test()
		{
			// Arrange
			var config = new ApplicationConfiguration();
			var dataConnection = new DataConnectionConfiguration
			{
				Active = true,
				DataSourceName = "Primary",
				Host = "localhost",
				Password = "password",
				UserName = "user"
			};
			config.DataConnections.Add(dataConnection);
			config.DataSources = null;

			// Act & Assert
			var exception = Assert.Throws<ConfigurationException>(() => config.Validate());
			Assert.That(exception.Message, Does.Contain("DataSources"));
		}

		[Test]
		public void ApplicationConfiguration_Validate_WithoutOpenApi_ShouldThrow_Test()
		{
			// Arrange
			var config = new ApplicationConfiguration();

			var dataConnection = new DataConnectionConfiguration
			{
				Active = true,
				DataSourceName = "Primary",
				Host = "localhost",
				Password = "password",
				UserName = "user"
			};
			config.DataConnections.Add(dataConnection);

			var dataSource = new DataSourceConfiguration
			{
				Active = true,
				DataSourceName = "Primary",
				DataSource = "MainDB",
				DataSourceType = "SqlServer",
				Schema = "dbo"
			};
			config.DataSources.Add(dataSource);

			config.OpenApi = null;

			// Act & Assert
			var exception = Assert.Throws<ConfigurationException>(() => config.Validate());
			Assert.That(exception.Message, Does.Contain("OpenApi"));
		}

		[Test]
		public void ApplicationConfiguration_Validate_WithValidConfiguration_ShouldNotThrow_Test()
		{
			// Arrange
			var config = new ApplicationConfiguration();

			var dataConnection = new DataConnectionConfiguration
			{
				Active = true,
				DataSourceName = "Primary",
				Host = "localhost",
				Password = "password",
				UserName = "user",
				Port = 1433,
				ConnectTimeout = 30
			};
			config.DataConnections.Add(dataConnection);

			var dataSource = new DataSourceConfiguration
			{
				Active = true,
				DataSourceName = "Primary",
				DataSource = "MainDB",
				DataSourceType = "SqlServer",
				Schema = "dbo"
			};
			config.DataSources.Add(dataSource);

			config.OpenApi = new OpenApiConfiguration
			{
				Active = true,
				Title = "API",
				Version = "1.0",
				Description = "Test",
				Summary = "Test API",
				License = "MIT",
				Contact = new OpenApiContactConfiguration
				{
					Active = true,
					Name = "Support",
					Email = "support@example.com",
					URL = "https://example.com"
				}
			};

			// Act & Assert
			Assert.DoesNotThrow(() => config.Validate());
		}
	}
}


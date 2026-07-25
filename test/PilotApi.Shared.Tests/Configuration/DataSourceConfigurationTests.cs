using NUnit.Framework;
using PilotApi.Shared.Configuration;
using PilotApi.Shared.Constants;
using PilotApi.Shared.Exceptions;
using System;
using System.Collections.Generic;

namespace PilotApi.Shared.Tests.Configuration
{
	[TestFixture]
	public class DataSourceConfigurationTests
	{
		[Test]
		public void DataSourceConfiguration_Constructor_ShouldInitializeWithDefaults_Test()
		{
			// Arrange & Act
			var config = new DataSourceConfiguration();

			// Assert
			Assert.Null(config.DataSource);
			Assert.That(config.DataSourceEnum, Is.EqualTo(DataSourceTypes.Unrecognized));
			Assert.Null(config.DataSourceName);
			Assert.Null(config.DataSourceType);
			Assert.Null(config.Schema);
			Assert.That(config.Active, Is.True);
		}

		[Test]
		public void DataSourceConfiguration_ConstructorWithSourceConfiguration_CopiesValues_Test()
		{
			// Arrange
			var sourceConfiguration = new DataSourceConfiguration
			{
				Active = false,
				DataSource = "MainDB",
				DataSourceEnum = DataSourceTypes.SqlServer,
				DataSourceName = "Primary",
				DataSourceType = "SqlServer",
				Schema = "dbo"
			};

			// Act
			var result = new DataSourceConfiguration(sourceConfiguration);

			// Assert
			Assert.That(result.Active, Is.False);
			Assert.That(result.DataSource, Is.EqualTo("MainDB"));
			Assert.That(result.DataSourceEnum, Is.EqualTo(DataSourceTypes.SqlServer));
			Assert.That(result.DataSourceName, Is.EqualTo("Primary"));
			Assert.That(result.DataSourceType, Is.EqualTo("SqlServer"));
			Assert.That(result.Schema, Is.EqualTo("dbo"));
		}

		[Test]
		public void DataSourceConfiguration_ConstructorWithSourceConfigurationNull_ThrowsArgumentException_Test()
		{
			// Arrange
			DataSourceConfiguration sourceConfiguration = null;

			// Act
			TestDelegate action = () =>
			{
				new DataSourceConfiguration(sourceConfiguration);
			};

			// Assert
			var exception = Assert.Throws<ArgumentException>(action);
			Assert.That(exception?.Message, Does.Contain("sourceConfiguration"));
		}

		[Test]
		public void DataSourceConfiguration_DataSourceEnum_StartsAsUnrecognized_Test()
		{
			// Arrange & Act
			var config = new DataSourceConfiguration();

			// Assert
			Assert.That(config.DataSourceEnum, Is.EqualTo(DataSourceTypes.Unrecognized));
		}

		[Test]
		public void DataSourceConfiguration_Properties_CanBeSet_Test()
		{
			// Arrange
			var config = new DataSourceConfiguration();

			// Act
			config.Active = false;
			config.DataSource = "MainDB";
			config.DataSourceEnum = DataSourceTypes.SqlServer;
			config.DataSourceName = "Primary";
			config.DataSourceType = "SqlServer";
			config.Schema = "dbo";

			// Assert
			Assert.That(config.Active, Is.False);
			Assert.That(config.DataSource, Is.EqualTo("MainDB"));
			Assert.That(config.DataSourceEnum, Is.EqualTo(DataSourceTypes.SqlServer));
			Assert.That(config.DataSourceName, Is.EqualTo("Primary"));
			Assert.That(config.DataSourceType, Is.EqualTo("SqlServer"));
			Assert.That(config.Schema, Is.EqualTo("dbo"));
		}

		[Test]
		public void DataSourceConfiguration_ToString_ShouldReturnFormattedString_Test()
		{
			// Arrange
			var config = new DataSourceConfiguration
			{
				Active = true,
				DataSource = "MainDB",
				DataSourceEnum = DataSourceTypes.SqlServer,
				DataSourceName = "Primary",
				DataSourceType = "SqlServer",
				Schema = "dbo"
			};

			// Act
			var result = config.ToString();

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("Active=True"));
			Assert.That(result, Does.Contain("DataSourceName=Primary"));
			Assert.That(result, Does.Contain("DataSource=MainDB"));
			Assert.That(result, Does.Contain("DataSourceEnum=SqlServer"));
			Assert.That(result, Does.Contain("DataSourceType=SqlServer"));
			Assert.That(result, Does.Contain("Schema=dbo"));
		}

		[Test]
		public void DataSourceConfiguration_Validate_WithCaseInsensitiveDataSourceType_ShouldResolve_Test()
		{
			// Arrange
			var config = new DataSourceConfiguration
			{
				DataSource = "MainDB",
				DataSourceName = "Primary",
				DataSourceType = "sqlserver",
				Schema = "dbo"
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(config.DataSourceEnum, Is.EqualTo(DataSourceTypes.SqlServer));
		}

		[Test]
		public void DataSourceConfiguration_Validate_WithEmptyDataSource_ShouldThrow_Test()
		{
			// Arrange
			var config = new DataSourceConfiguration
			{
				DataSource = "   ",
				DataSourceName = "Primary",
				DataSourceType = "SqlServer",
				Schema = "dbo"
			};
			var exceptions = new List<Exception>();

			// Act & Assert
			var exception = Assert.Throws<ConfigurationException>(() => config.Validate(ref exceptions));
			Assert.That(exception.Message, Does.Contain("DataSource"));
		}

		[Test]
		public void DataSourceConfiguration_Validate_WithInvalidDataSourceType_ShouldThrow_Test()
		{
			// Arrange
			var config = new DataSourceConfiguration
			{
				DataSource = "MainDB",
				DataSourceName = "Primary",
				DataSourceType = "InvalidType",
				Schema = "dbo"
			};
			var exceptions = new List<Exception>();

			// Act & Assert
			var exception = Assert.Throws<ConfigurationException>(() => config.Validate(ref exceptions));
			Assert.That(exception.Message, Does.Contain("not valid"));
		}

		[Test]
		public void DataSourceConfiguration_Validate_WithNullExceptions_ShouldThrowArgumentException_Test()
		{
			// Arrange
			var config = new DataSourceConfiguration();
			List<Exception> exceptions = null;

			// Act & Assert
			var exception = Assert.Throws<ArgumentException>(() => config.Validate(ref exceptions));
			Assert.That(exception.Message, Does.Contain("exceptions"));
		}

		[Test]
		public void DataSourceConfiguration_Validate_WithoutDataSource_ShouldThrow_Test()
		{
			// Arrange
			var config = new DataSourceConfiguration
			{
				DataSourceName = "Primary",
				DataSourceType = "SqlServer",
				Schema = "dbo"
			};
			var exceptions = new List<Exception>();

			// Act & Assert
			var exception = Assert.Throws<ConfigurationException>(() => config.Validate(ref exceptions));
			Assert.That(exception.Message, Does.Contain("DataSource"));
		}
		[Test]
		public void DataSourceConfiguration_Validate_WithoutDataSourceName_ShouldThrow_Test()
		{
			// Arrange
			var config = new DataSourceConfiguration
			{
				DataSource = "MainDB",
				DataSourceType = "SqlServer",
				Schema = "dbo"
			};
			var exceptions = new List<Exception>();

			// Act & Assert
			var exception = Assert.Throws<ConfigurationException>(() => config.Validate(ref exceptions));
			Assert.That(exception.Message, Does.Contain("DataSourceName"));
		}

		[Test]
		public void DataSourceConfiguration_Validate_WithoutDataSourceType_ShouldThrow_Test()
		{
			// Arrange
			var config = new DataSourceConfiguration
			{
				DataSource = "MainDB",
				DataSourceName = "Primary",
				Schema = "dbo"
			};
			var exceptions = new List<Exception>();

			// Act & Assert
			var exception = Assert.Throws<ConfigurationException>(() => config.Validate(ref exceptions));
			Assert.That(exception.Message, Does.Contain("DataSourceType"));
		}
		[Test]
		public void DataSourceConfiguration_Validate_WithoutSchema_ShouldThrow_Test()
		{
			// Arrange
			var config = new DataSourceConfiguration
			{
				DataSource = "MainDB",
				DataSourceName = "Primary",
				DataSourceType = "SqlServer"
			};
			var exceptions = new List<Exception>();

			// Act & Assert
			var exception = Assert.Throws<ConfigurationException>(() => config.Validate(ref exceptions));
			Assert.That(exception.Message, Does.Contain("Schema"));
		}

		[Test]
		public void DataSourceConfiguration_Validate_WithPostgreSQLCaseVariant_ShouldResolve_Test()
		{
			// Arrange
			var config = new DataSourceConfiguration
			{
				DataSource = "maindb",
				DataSourceName = "Primary",
				DataSourceType = "postgresql",
				Schema = "public"
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(config.DataSourceEnum, Is.EqualTo(DataSourceTypes.PostgreSQL));
		}

		[Test]
		public void DataSourceConfiguration_Validate_WithValidPostgreSqlConfiguration_ShouldNotThrow_Test()
		{
			// Arrange
			var config = new DataSourceConfiguration
			{
				Active = true,
				DataSource = "maindb",
				DataSourceName = "Primary",
				DataSourceType = "PostgreSQL",
				Schema = "public"
			};
			var exceptions = new List<Exception>();

			// Act & Assert
			Assert.DoesNotThrow(() => config.Validate(ref exceptions));
			Assert.That(config.DataSourceEnum, Is.EqualTo(DataSourceTypes.PostgreSQL));
		}

		[Test]
		public void DataSourceConfiguration_Validate_WithValidSqlServerConfiguration_ShouldNotThrow_Test()
		{
			// Arrange
			var config = new DataSourceConfiguration
			{
				Active = true,
				DataSource = "MainDB",
				DataSourceName = "Primary",
				DataSourceType = "SqlServer",
				Schema = "dbo"
			};
			var exceptions = new List<Exception>();

			// Act & Assert
			Assert.DoesNotThrow(() => config.Validate(ref exceptions));
			Assert.That(config.DataSourceEnum, Is.EqualTo(DataSourceTypes.SqlServer));
		}
	}
}


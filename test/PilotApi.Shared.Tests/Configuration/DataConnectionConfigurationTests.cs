using NUnit.Framework;
using PilotApi.Shared.Configuration;
using System;
using System.Collections.Generic;

namespace PilotApi.Shared.Tests.Configuration
{
	[TestFixture]
	public class DataConnectionConfigurationTests
	{
		[Test]
		public void DataConnectionConfiguration_ConnectTimeout_DefaultsToZero_Test()
		{
			// Arrange & Act
			var config = new DataConnectionConfiguration();

			// Assert
			Assert.That(config.ConnectTimeout, Is.EqualTo(0));
		}

		[Test]
		public void DataConnectionConfiguration_Constructor_ShouldInitializeWithDefaults_Test()
		{
			// Arrange & Act
			var config = new DataConnectionConfiguration();

			// Assert
			Assert.That(config.ConnectTimeout, Is.EqualTo(0));
			Assert.Null(config.DataSourceName);
			Assert.Null(config.Host);
			Assert.Null(config.Password);
			Assert.Null(config.Port);
			Assert.Null(config.UserName);
			Assert.That(config.Active, Is.True);
		}

		[Test]
		public void DataConnectionConfiguration_ConstructorWithSourceConfigurationAndSuppressSensitiveValuesTrue_RedactsPassword_Test()
		{
			// Arrange
			var sourceConfiguration = new DataConnectionConfiguration
			{
				Active = false,
				ConnectTimeout = 45,
				DataSourceName = "Primary",
				Host = "localhost",
				Password = "MySecret",
				Port = 5432,
				UserName = "postgres"
			};

			// Act
			var result = new DataConnectionConfiguration(sourceConfiguration, true);

			// Assert
			Assert.That(result.Active, Is.False);
			Assert.That(result.ConnectTimeout, Is.EqualTo(45));
			Assert.That(result.DataSourceName, Is.EqualTo("Primary"));
			Assert.That(result.Host, Is.EqualTo("localhost"));
			Assert.That(result.Password, Is.EqualTo("[Redacted]"));
			Assert.That(result.Port, Is.EqualTo(5432));
			Assert.That(result.UserName, Is.EqualTo("postgres"));
		}

		[Test]
		public void DataConnectionConfiguration_ConstructorWithSourceConfigurationNull_ThrowsArgumentException_Test()
		{
			// Arrange
			DataConnectionConfiguration sourceConfiguration = null;

			// Act
			TestDelegate action = () =>
			{
				new DataConnectionConfiguration(sourceConfiguration);
			};

			// Assert
			var exception = Assert.Throws<ArgumentException>(action);
			Assert.That(exception?.Message, Does.Contain("sourceConfiguration"));
		}

		[Test]
		public void DataConnectionConfiguration_Port_CanBeNullOrSet_Test()
		{
			// Arrange & Act
			var config1 = new DataConnectionConfiguration();
			var config2 = new DataConnectionConfiguration { Port = 1433 };

			// Assert
			Assert.Null(config1.Port);
			Assert.That(config2.Port, Is.EqualTo(1433));
		}

		[Test]
		public void DataConnectionConfiguration_Properties_CanBeSet_Test()
		{
			// Arrange
			var config = new DataConnectionConfiguration();

			// Act
			config.ConnectTimeout = 30;
			config.DataSourceName = "TestDB";
			config.Host = "localhost";
			config.Password = "secret";
			config.Port = 1433;
			config.UserName = "admin";
			config.Active = false;

			// Assert
			Assert.That(config.ConnectTimeout, Is.EqualTo(30));
			Assert.That(config.DataSourceName, Is.EqualTo("TestDB"));
			Assert.That(config.Host, Is.EqualTo("localhost"));
			Assert.That(config.Password, Is.EqualTo("secret"));
			Assert.That(config.Port, Is.EqualTo(1433));
			Assert.That(config.UserName, Is.EqualTo("admin"));
			Assert.That(config.Active, Is.False);
		}

		[Test]
		public void DataConnectionConfiguration_ToString_ShouldReturnFormattedString_Test()
		{
			// Arrange
			var config = new DataConnectionConfiguration
			{
				Active = true,
				ConnectTimeout = 30,
				DataSourceName = "Primary",
				Host = "localhost",
				Password = "secret",
				Port = 1433,
				UserName = "admin"
			};

			// Act
			var result = config.ToString();

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("Active=True"));
			Assert.That(result, Does.Contain("ConnectTimeout=30"));
			Assert.That(result, Does.Contain("DataSourceName=Primary"));
			Assert.That(result, Does.Contain("Host=localhost"));
			Assert.That(result, Does.Contain("[Redacted]"));
			Assert.That(result, Does.Contain("Port=1433"));
			Assert.That(result, Does.Contain("UserName=admin"));
		}

		[Test]
		public void DataConnectionConfiguration_ToString_WithEmptyPassword_ShouldShowEmpty_Test()
		{
			// Arrange
			var config = new DataConnectionConfiguration
			{
				Active = true,
				DataSourceName = "Primary",
				Password = ""
			};

			// Act
			var result = config.ToString();

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("-Empty-"));
		}

		[Test]
		public void DataConnectionConfiguration_ToString_WithNullPassword_ShouldShowEmpty_Test()
		{
			// Arrange
			var config = new DataConnectionConfiguration
			{
				Active = true,
				ConnectTimeout = 30,
				DataSourceName = "Primary",
				Host = "localhost",
				Password = null,
				Port = 1433,
				UserName = "admin"
			};

			// Act
			var result = config.ToString();

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("-Empty-"));
		}
		[Test]
		public void DataConnectionConfiguration_Validate_WithEmptyDataSourceName_ShouldThrow_Test()
		{
			// Arrange
			var config = new DataConnectionConfiguration
			{
				DataSourceName = "   ",
				Host = "localhost",
				Password = "password",
				UserName = "user"
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions, Is.Not.Null);
			Assert.That(exceptions, Has.Count.GreaterThan(0));
			var firstException = exceptions[0];
			Assert.That(firstException, Is.Not.Null);
			Assert.That(firstException.Message, Does.Contain("DataSourceName"));
		}

		[Test]
		public void DataConnectionConfiguration_Validate_WithMinimalValidConfiguration_ShouldNotThrow_Test()
		{
			// Arrange
			var config = new DataConnectionConfiguration
			{
				DataSourceName = "Primary",
				Host = "localhost",
				Password = "password",
				UserName = "user"
			};
			var exceptions = new List<Exception>();

			// Act & Assert
			Assert.DoesNotThrow(() => config.Validate(ref exceptions));
		}

		[Test]
		public void DataConnectionConfiguration_Validate_WithNullExceptions_ShouldThrowArgumentException_Test()
		{
			// Arrange
			var config = new DataConnectionConfiguration();
			List<Exception> exceptions = null;

			// Act & Assert
			var exception = Assert.Throws<ArgumentException>(() => config.Validate(ref exceptions));
			Assert.That(exception.Message, Does.Contain("exceptions"));
		}

		[Test]
		public void DataConnectionConfiguration_Validate_WithoutDataSourceName_ShouldThrow_Test()
		{
			// Arrange
			var config = new DataConnectionConfiguration
			{
				Host = "localhost",
				Password = "password",
				UserName = "user"
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions, Is.Not.Null);
			Assert.That(exceptions, Has.Count.GreaterThan(0));
			var firstException = exceptions[0];
			Assert.That(firstException, Is.Not.Null);
			Assert.That(firstException.Message, Does.Contain("DataSourceName"));
		}
		[Test]
		public void DataConnectionConfiguration_Validate_WithoutHost_ShouldThrow_Test()
		{
			// Arrange
			var config = new DataConnectionConfiguration
			{
				DataSourceName = "Primary",
				Password = "password",
				UserName = "user"
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions, Is.Not.Null);
			Assert.That(exceptions, Has.Count.GreaterThan(0));
			var firstException = exceptions[0];
			Assert.That(firstException, Is.Not.Null);
			Assert.That(firstException.Message, Does.Contain("Host"));
		}

		[Test]
		public void DataConnectionConfiguration_Validate_WithoutPassword_ShouldThrow_Test()
		{
			// Arrange
			var config = new DataConnectionConfiguration
			{
				DataSourceName = "Primary",
				Host = "localhost",
				UserName = "user"
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions, Is.Not.Null);
			Assert.That(exceptions, Has.Count.GreaterThan(0));
			var firstException = exceptions[0];
			Assert.That(firstException, Is.Not.Null);
			Assert.That(firstException.Message, Does.Contain("Password"));

		}

		[Test]
		public void DataConnectionConfiguration_Validate_WithoutUserName_ShouldThrow_Test()
		{
			// Arrange
			var config = new DataConnectionConfiguration
			{
				DataSourceName = "Primary",
				Host = "localhost",
				Password = "password"
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions, Is.Not.Null);
			Assert.That(exceptions, Has.Count.GreaterThan(0));
			var firstException = exceptions[0];
			Assert.That(firstException, Is.Not.Null);
			Assert.That(firstException.Message, Does.Contain("UserName"));
		}

		[Test]
		public void DataConnectionConfiguration_Validate_WithValidConfiguration_ShouldNotThrow_Test()
		{
			// Arrange
			var config = new DataConnectionConfiguration
			{
				Active = true,
				ConnectTimeout = 30,
				DataSourceName = "Primary",
				Host = "localhost",
				Password = "password",
				Port = 1433,
				UserName = "admin"
			};
			var exceptions = new List<Exception>();

			// Act & Assert
			Assert.DoesNotThrow(() => config.Validate(ref exceptions));
		}
	}
}


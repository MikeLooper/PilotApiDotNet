using NUnit.Framework;
using PilotApi.Shared.Configuration;
using PilotApi.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PilotApi.Shared.Tests.Configuration
{
	[TestFixture]
	public class OpenApiContactConfigurationTests
	{
		[Test]
		public void OpenApiContactConfiguration_Constructor_ShouldInitializeWithDefaults_Test()
		{
			// Arrange & Act
			var config = new OpenApiContactConfiguration();

			// Assert
			Assert.Null(config.Email);
			Assert.Null(config.Name);
			Assert.Null(config.URL);
			Assert.That(config.Active, Is.True);
		}

		[Test]
		public void OpenApiContactConfiguration_ConstructorWithSourceConfiguration_CopiesValues_Test()
		{
			// Arrange
			var sourceConfiguration = new OpenApiContactConfiguration
			{
				Active = false,
				Email = "support@example.com",
				Name = "Support",
				URL = "https://example.com"
			};

			// Act
			var result = new OpenApiContactConfiguration(sourceConfiguration);

			// Assert
			Assert.That(result.Active, Is.False);
			Assert.That(result.Email, Is.EqualTo("support@example.com"));
			Assert.That(result.Name, Is.EqualTo("Support"));
			Assert.That(result.URL, Is.EqualTo("https://example.com"));
		}

		[Test]
		public void OpenApiContactConfiguration_ConstructorWithSourceConfigurationNull_ThrowsArgumentException_Test()
		{
			// Arrange
			OpenApiContactConfiguration sourceConfiguration = null;

			// Act
			TestDelegate action = () =>
			{
				new OpenApiContactConfiguration(sourceConfiguration);
			};

			// Assert
			var exception = Assert.Throws<ArgumentException>(action);
			Assert.That(exception?.Message, Does.Contain("sourceConfiguration"));
		}

		[Test]
		public void OpenApiContactConfiguration_Properties_CanBeSet_Test()
		{
			// Arrange
			var config = new OpenApiContactConfiguration();

			// Act
			config.Active = false;
			config.Email = "support@example.com";
			config.Name = "Support Team";
			config.URL = "https://example.com";

			// Assert
			Assert.That(config.Active, Is.False);
			Assert.That(config.Email, Is.EqualTo("support@example.com"));
			Assert.That(config.Name, Is.EqualTo("Support Team"));
			Assert.That(config.URL, Is.EqualTo("https://example.com"));
		}

		[Test]
		public void OpenApiContactConfiguration_ToString_ShouldReturnFormattedString_Test()
		{
			// Arrange
			var config = new OpenApiContactConfiguration
			{
				Active = true,
				Email = "support@example.com",
				Name = "Support Team",
				URL = "https://example.com"
			};

			// Act
			var result = config.ToString();

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("Active=True"));
			Assert.That(result, Does.Contain("Email=support@example.com"));
			Assert.That(result, Does.Contain("Name=Support Team"));
			Assert.That(result, Does.Contain("URL=https://example.com"));
		}

		[Test]
		public void OpenApiContactConfiguration_Validate_ExceptionMessages_ShouldContainFieldNames_Test()
		{
			// Arrange
			var config = new OpenApiContactConfiguration
			{
				Email = null,
				Name = null,
				URL = null
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			var exceptionMessages = string.Join(" ", exceptions.Select(e => e.Message));
			Assert.That(exceptionMessages, Does.Contain("Email"));
			Assert.That(exceptionMessages, Does.Contain("Name"));
			Assert.That(exceptionMessages, Does.Contain("URL"));
		}

		[Test]
		public void OpenApiContactConfiguration_Validate_WithAllFieldsMissing_ShouldAddThreeExceptions_Test()
		{
			// Arrange
			var config = new OpenApiContactConfiguration
			{
				Email = null,
				Name = null,
				URL = null
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions.Count, Is.EqualTo(3));
			Assert.That(exceptions[0], Is.TypeOf<ConfigurationException>());
			Assert.That(exceptions[1], Is.TypeOf<ConfigurationException>());
			Assert.That(exceptions[2], Is.TypeOf<ConfigurationException>());
		}

		[Test]
		public void OpenApiContactConfiguration_Validate_WithEmptyEmail_ShouldAddException_Test()
		{
			// Arrange
			var config = new OpenApiContactConfiguration
			{
				Email = "   ",
				Name = "Support",
				URL = "https://example.com"
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions.Count, Is.GreaterThan(0));
			Assert.That(exceptions.Any(e => e.Message.Contains("Email")), Is.True);
		}

		[Test]
		public void OpenApiContactConfiguration_Validate_WithEmptyName_ShouldAddException_Test()
		{
			// Arrange
			var config = new OpenApiContactConfiguration
			{
				Email = "support@example.com",
				Name = "   ",
				URL = "https://example.com"
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions.Count, Is.GreaterThan(0));
			Assert.That(exceptions.Any(e => e.Message.Contains("Name")), Is.True);
		}

		[Test]
		public void OpenApiContactConfiguration_Validate_WithEmptyURL_ShouldAddException_Test()
		{
			// Arrange
			var config = new OpenApiContactConfiguration
			{
				Email = "support@example.com",
				Name = "Support",
				URL = "   "
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions.Count, Is.GreaterThan(0));
			Assert.That(exceptions.Any(e => e.Message.Contains("URL")), Is.True);
		}

		[Test]
		public void OpenApiContactConfiguration_Validate_WithMultipleMissingFields_ShouldAddMultipleExceptions_Test()
		{
			// Arrange
			var config = new OpenApiContactConfiguration();
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions.Count, Is.EqualTo(3)); // Email, Name, URL all missing
		}

		[Test]
		public void OpenApiContactConfiguration_Validate_WithNullExceptions_ShouldThrowArgumentException_Test()
		{
			// Arrange
			var config = new OpenApiContactConfiguration();
			List<Exception> exceptions = null;

			// Act & Assert
			var exception = Assert.Throws<ArgumentException>(() => config.Validate(ref exceptions));
			Assert.That(exception.Message, Does.Contain("exceptions"));
		}

		[Test]
		public void OpenApiContactConfiguration_Validate_WithoutEmail_ShouldAddException_Test()
		{
			// Arrange
			var config = new OpenApiContactConfiguration
			{
				Name = "Support",
				URL = "https://example.com"
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions.Count, Is.GreaterThan(0));
			Assert.That(exceptions.Any(e => e.Message.Contains("Email")), Is.True);
		}
		[Test]
		public void OpenApiContactConfiguration_Validate_WithoutName_ShouldAddException_Test()
		{
			// Arrange
			var config = new OpenApiContactConfiguration
			{
				Email = "support@example.com",
				URL = "https://example.com"
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions.Count, Is.GreaterThan(0));
			Assert.That(exceptions.Any(e => e.Message.Contains("Name")), Is.True);
		}
		[Test]
		public void OpenApiContactConfiguration_Validate_WithoutURL_ShouldAddException_Test()
		{
			// Arrange
			var config = new OpenApiContactConfiguration
			{
				Email = "support@example.com",
				Name = "Support"
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions.Count, Is.GreaterThan(0));
			Assert.That(exceptions.Any(e => e.Message.Contains("URL")), Is.True);
		}
		[Test]
		public void OpenApiContactConfiguration_Validate_WithValidConfiguration_ShouldNotAddExceptions_Test()
		{
			// Arrange
			var config = new OpenApiContactConfiguration
			{
				Active = true,
				Email = "support@example.com",
				Name = "Support Team",
				URL = "https://example.com"
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions.Count, Is.EqualTo(0));
		}
	}
}


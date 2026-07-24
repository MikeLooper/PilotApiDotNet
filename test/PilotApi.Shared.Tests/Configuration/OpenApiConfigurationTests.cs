using NUnit.Framework;
using PilotApi.Shared.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PilotApi.Shared.Tests.Configuration
{
	[TestFixture]
	public class OpenApiConfigurationTests
	{
		[Test]
		public void OpenApiConfiguration_Constructor_ShouldInitializeWithDefaults_Test()
		{
			// Arrange & Act
			var config = new OpenApiConfiguration();

			// Assert
			Assert.Null(config.Contact);
			Assert.Null(config.Description);
			Assert.Null(config.License);
			Assert.Null(config.Summary);
			Assert.Null(config.Title);
			Assert.Null(config.Version);
			Assert.That(config.Active, Is.True);
		}

		[Test]
		public void OpenApiConfiguration_ConstructorWithSourceConfiguration_CopiesValues_Test()
		{
			// Arrange
			var sourceConfiguration = new OpenApiConfiguration
			{
				Active = false,
				Contact = new OpenApiContactConfiguration
				{
					Name = "Support",
					Email = "support@example.com",
					URL = "https://example.com"
				},
				Description = "description",
				License = "MIT",
				Summary = "summary",
				Title = "PilotApi",
				Version = "1.2.3"
			};

			// Act
			var result = new OpenApiConfiguration(sourceConfiguration);

			// Assert
			Assert.That(result.Active, Is.False);
			Assert.That(result.Contact, Is.SameAs(sourceConfiguration.Contact));
			Assert.That(result.Description, Is.EqualTo("description"));
			Assert.That(result.License, Is.EqualTo("MIT"));
			Assert.That(result.Summary, Is.EqualTo("summary"));
			Assert.That(result.Title, Is.EqualTo("PilotApi"));
			Assert.That(result.Version, Is.EqualTo("1.2.3"));
		}

		[Test]
		public void OpenApiConfiguration_ConstructorWithSourceConfigurationNull_ThrowsArgumentException_Test()
		{
			// Arrange
			OpenApiConfiguration sourceConfiguration = null;

			// Act
			TestDelegate action = () =>
			{
				new OpenApiConfiguration(sourceConfiguration);
			};

			// Assert
			var exception = Assert.Throws<ArgumentException>(action);
			Assert.That(exception?.Message, Does.Contain("sourceConfiguration"));
		}

		[Test]
		public void OpenApiConfiguration_Properties_CanBeSet_Test()
		{
			// Arrange
			var config = new OpenApiConfiguration();
			var contact = new OpenApiContactConfiguration { Name = "Support" };

			// Act
			config.Active = false;
			config.Contact = contact;
			config.Description = "API Description";
			config.License = "MIT";
			config.Summary = "API Summary";
			config.Title = "My API";
			config.Version = "1.0.0";

			// Assert
			Assert.That(config.Active, Is.False);
			Assert.NotNull(config.Contact);
			Assert.That(config.Contact.Name, Is.EqualTo("Support"));
			Assert.That(config.Description, Is.EqualTo("API Description"));
			Assert.That(config.License, Is.EqualTo("MIT"));
			Assert.That(config.Summary, Is.EqualTo("API Summary"));
			Assert.That(config.Title, Is.EqualTo("My API"));
			Assert.That(config.Version, Is.EqualTo("1.0.0"));
		}

		[Test]
		public void OpenApiConfiguration_ToString_ShouldReturnFormattedString_Test()
		{
			// Arrange
			var contact = new OpenApiContactConfiguration
			{
				Name = "Support",
				Email = "support@example.com",
				URL = "https://example.com"
			};
			var config = new OpenApiConfiguration
			{
				Active = true,
				Contact = contact,
				Description = "Test API",
				License = "MIT",
				Summary = "Summary",
				Title = "Test API",
				Version = "1.0"
			};

			// Act
			var result = config.ToString();

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("Active=True"));
			Assert.That(result, Does.Contain("Contact="));
			Assert.That(result, Does.Contain("Description=Test API"));
			Assert.That(result, Does.Contain("License=MIT"));
			Assert.That(result, Does.Contain("Summary=Summary"));
			Assert.That(result, Does.Contain("Title=Test API"));
			Assert.That(result, Does.Contain("Version=1.0"));
		}

		[Test]
		public void OpenApiConfiguration_Validate_WithEmptyFields_ShouldAddExceptions_Test()
		{
			// Arrange
			var config = new OpenApiConfiguration
			{
				Contact = new OpenApiContactConfiguration
				{
					Name = "Support",
					Email = "support@example.com",
					URL = "https://example.com"
				},
				Description = "   ",
				License = "   ",
				Summary = "   ",
				Title = "   ",
				Version = "   "
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions.Count, Is.GreaterThan(0));
		}

		[Test]
		public void OpenApiConfiguration_Validate_WithInvalidContact_ShouldPropagateContactExceptions_Test()
		{
			// Arrange
			var config = new OpenApiConfiguration
			{
				Contact = new OpenApiContactConfiguration
				{
					// Missing required fields
					Name = "Support"
				},
				Description = "Test",
				License = "MIT",
				Summary = "Summary",
				Title = "API",
				Version = "1.0"
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions.Count, Is.GreaterThan(0));
		}

		[Test]
		public void OpenApiConfiguration_Validate_WithNullExceptions_ShouldThrowArgumentException_Test()
		{
			// Arrange
			var config = new OpenApiConfiguration();
			List<Exception> exceptions = null;

			// Act & Assert
			var exception = Assert.Throws<ArgumentException>(() => config.Validate(ref exceptions));
			Assert.That(exception.Message, Does.Contain("exceptions"));
		}

		[Test]
		public void OpenApiConfiguration_Validate_WithoutContact_ShouldAddException_Test()
		{
			// Arrange
			var config = new OpenApiConfiguration
			{
				Description = "Test",
				License = "MIT",
				Summary = "Summary",
				Title = "API",
				Version = "1.0"
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions.Count, Is.GreaterThan(0));
			Assert.That(exceptions[0].Message, Does.Contain("Contact"));
		}

		[Test]
		public void OpenApiConfiguration_Validate_WithoutDescription_ShouldAddException_Test()
		{
			// Arrange
			var config = new OpenApiConfiguration
			{
				Contact = new OpenApiContactConfiguration
				{
					Name = "Support",
					Email = "support@example.com",
					URL = "https://example.com"
				},
				License = "MIT",
				Summary = "Summary",
				Title = "API",
				Version = "1.0"
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions.Count, Is.GreaterThan(0));
			Assert.That(exceptions.Any(e => e.Message.Contains("Description")), Is.True);
		}

		[Test]
		public void OpenApiConfiguration_Validate_WithoutLicense_ShouldAddException_Test()
		{
			// Arrange
			var config = new OpenApiConfiguration
			{
				Contact = new OpenApiContactConfiguration
				{
					Name = "Support",
					Email = "support@example.com",
					URL = "https://example.com"
				},
				Description = "Test",
				Summary = "Summary",
				Title = "API",
				Version = "1.0"
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions.Count, Is.GreaterThan(0));
			Assert.That(exceptions.Any(e => e.Message.Contains("License")), Is.True);
		}

		[Test]
		public void OpenApiConfiguration_Validate_WithoutSummary_ShouldAddException_Test()
		{
			// Arrange
			var config = new OpenApiConfiguration
			{
				Contact = new OpenApiContactConfiguration
				{
					Name = "Support",
					Email = "support@example.com",
					URL = "https://example.com"
				},
				Description = "Test",
				License = "MIT",
				Title = "API",
				Version = "1.0"
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions.Count, Is.GreaterThan(0));
			Assert.That(exceptions.Any(e => e.Message.Contains("Summary")), Is.True);
		}

		[Test]
		public void OpenApiConfiguration_Validate_WithoutTitle_ShouldAddException_Test()
		{
			// Arrange
			var config = new OpenApiConfiguration
			{
				Contact = new OpenApiContactConfiguration
				{
					Name = "Support",
					Email = "support@example.com",
					URL = "https://example.com"
				},
				Description = "Test",
				License = "MIT",
				Summary = "Summary",
				Version = "1.0"
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions.Count, Is.GreaterThan(0));
			Assert.That(exceptions.Any(e => e.Message.Contains("Title")), Is.True);
		}

		[Test]
		public void OpenApiConfiguration_Validate_WithoutVersion_ShouldAddException_Test()
		{
			// Arrange
			var config = new OpenApiConfiguration
			{
				Contact = new OpenApiContactConfiguration
				{
					Name = "Support",
					Email = "support@example.com",
					URL = "https://example.com"
				},
				Description = "Test",
				License = "MIT",
				Summary = "Summary",
				Title = "API"
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions.Count, Is.GreaterThan(0));
			Assert.That(exceptions.Any(e => e.Message.Contains("Version")), Is.True);
		}

		[Test]
		public void OpenApiConfiguration_Validate_WithValidConfiguration_ShouldNotAddExceptions_Test()
		{
			// Arrange
			var config = new OpenApiConfiguration
			{
				Active = true,
				Contact = new OpenApiContactConfiguration
				{
					Active = true,
					Name = "Support",
					Email = "support@example.com",
					URL = "https://example.com"
				},
				Description = "Test API",
				License = "MIT",
				Summary = "Summary",
				Title = "Test API",
				Version = "1.0"
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions.Count, Is.EqualTo(0));
		}
	}
}


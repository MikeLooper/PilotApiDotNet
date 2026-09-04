using NUnit.Framework;
using PilotApi.Shared.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PilotApi.Shared.Tests.Configuration
{
	[TestFixture]
	public class SecurityConfigurationTests
	{
		[Test]
		public void SecurityConfiguration_Constructor_ShouldInitializeWithDefaults_Test()
		{
			// Arrange & Act
			var config = new SecurityConfiguration();

			// Assert
			Assert.Null(config.BaseUrl);
			Assert.Null(config.Realm);
			Assert.Null(config.ClientId);
			Assert.That(config.RequireHttpsMetadata, Is.False);
			Assert.That(config.ClockSkewSeconds, Is.EqualTo(60));
			Assert.That(config.Active, Is.True);
		}

		[Test]
		public void SecurityConfiguration_ConstructorWithSourceConfiguration_CopiesValues_Test()
		{
			// Arrange
			var sourceConfiguration = new SecurityConfiguration
			{
				Active = false,
				BaseUrl = "http://local-keycloak:8080",
				PublicBaseUrl = "http://localhost:55001",
				Realm = "local-realm",
				ClientId = "local-client",
				RequireHttpsMetadata = true,
				ClockSkewSeconds = 30
			};

			// Act
			var result = new SecurityConfiguration(sourceConfiguration);

			// Assert
			Assert.That(result.Active, Is.False);
			Assert.That(result.BaseUrl, Is.EqualTo("http://local-keycloak:8080"));
			Assert.That(result.PublicBaseUrl, Is.EqualTo("http://localhost:55001"));
			Assert.That(result.Realm, Is.EqualTo("local-realm"));
			Assert.That(result.ClientId, Is.EqualTo("local-client"));
			Assert.That(result.RequireHttpsMetadata, Is.True);
			Assert.That(result.ClockSkewSeconds, Is.EqualTo(30));
		}

		[Test]
		public void SecurityConfiguration_ConstructorWithSourceConfigurationNull_ThrowsArgumentException_Test()
		{
			// Arrange
			SecurityConfiguration sourceConfiguration = null;

			// Act
			TestDelegate action = () =>
			{
				new SecurityConfiguration(sourceConfiguration);
			};

			// Assert
			var exception = Assert.Throws<ArgumentException>(action);
			Assert.That(exception?.Message, Does.Contain("sourceConfiguration"));
		}

		[Test]
		public void SecurityConfiguration_Authority_ShouldComputeFromBaseUrlAndRealm_Test()
		{
			// Arrange
			var config = new SecurityConfiguration
			{
				BaseUrl = "http://localhost:55001",
				Realm = "local-realm"
			};

			// Act
			var result = config.Authority;

			// Assert
			Assert.That(result, Is.EqualTo("http://localhost:55001/realms/local-realm"));
		}

		[Test]
		public void SecurityConfiguration_Authority_WithTrailingSlashOnBaseUrl_ShouldNotDoubleSlash_Test()
		{
			// Arrange
			var config = new SecurityConfiguration
			{
				BaseUrl = "http://localhost:55001/",
				Realm = "local-realm"
			};

			// Act
			var result = config.Authority;

			// Assert
			Assert.That(result, Is.EqualTo("http://localhost:55001/realms/local-realm"));
		}

		[Test]
		public void SecurityConfiguration_PublicAuthority_WithoutPublicBaseUrl_ShouldFallBackToBaseUrl_Test()
		{
			// Arrange
			var config = new SecurityConfiguration
			{
				BaseUrl = "http://localhost:55001",
				Realm = "local-realm"
			};

			// Act
			var result = config.PublicAuthority;

			// Assert
			Assert.That(result, Is.EqualTo("http://localhost:55001/realms/local-realm"));
		}

		[Test]
		public void SecurityConfiguration_PublicAuthority_WithPublicBaseUrl_ShouldComputeFromPublicBaseUrlAndRealm_Test()
		{
			// Arrange
			var config = new SecurityConfiguration
			{
				BaseUrl = "http://local-keycloak:8080",
				PublicBaseUrl = "http://localhost:55001",
				Realm = "local-realm"
			};

			// Act
			var result = config.PublicAuthority;

			// Assert
			Assert.That(result, Is.EqualTo("http://localhost:55001/realms/local-realm"));
		}

		[Test]
		public void SecurityConfiguration_PublicAuthority_WithTrailingSlashOnPublicBaseUrl_ShouldNotDoubleSlash_Test()
		{
			// Arrange
			var config = new SecurityConfiguration
			{
				BaseUrl = "http://local-keycloak:8080",
				PublicBaseUrl = "http://localhost:55001/",
				Realm = "local-realm"
			};

			// Act
			var result = config.PublicAuthority;

			// Assert
			Assert.That(result, Is.EqualTo("http://localhost:55001/realms/local-realm"));
		}

		[Test]
		public void SecurityConfiguration_Properties_CanBeSet_Test()
		{
			// Arrange
			var config = new SecurityConfiguration();

			// Act
			config.Active = false;
			config.BaseUrl = "http://local-keycloak:8080";
			config.PublicBaseUrl = "http://localhost:55001";
			config.Realm = "local-realm";
			config.ClientId = "local-client";
			config.RequireHttpsMetadata = true;
			config.ClockSkewSeconds = 120;

			// Assert
			Assert.That(config.Active, Is.False);
			Assert.That(config.BaseUrl, Is.EqualTo("http://local-keycloak:8080"));
			Assert.That(config.PublicBaseUrl, Is.EqualTo("http://localhost:55001"));
			Assert.That(config.Realm, Is.EqualTo("local-realm"));
			Assert.That(config.ClientId, Is.EqualTo("local-client"));
			Assert.That(config.RequireHttpsMetadata, Is.True);
			Assert.That(config.ClockSkewSeconds, Is.EqualTo(120));
		}

		[Test]
		public void SecurityConfiguration_ToString_ShouldReturnFormattedString_Test()
		{
			// Arrange
			var config = new SecurityConfiguration
			{
				Active = true,
				BaseUrl = "http://local-keycloak:8080",
				PublicBaseUrl = "http://localhost:55001",
				Realm = "local-realm",
				ClientId = "local-client",
				RequireHttpsMetadata = false,
				ClockSkewSeconds = 60
			};

			// Act
			var result = config.ToString();

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("Active=True"));
			Assert.That(result, Does.Contain("BaseUrl=http://local-keycloak:8080"));
			Assert.That(result, Does.Contain("PublicBaseUrl=http://localhost:55001"));
			Assert.That(result, Does.Contain("Realm=local-realm"));
			Assert.That(result, Does.Contain("ClientId=local-client"));
			Assert.That(result, Does.Contain("RequireHttpsMetadata=False"));
			Assert.That(result, Does.Contain("ClockSkewSeconds=60"));
		}

		[Test]
		public void SecurityConfiguration_Validate_WithNullExceptions_ShouldThrowArgumentException_Test()
		{
			// Arrange
			var config = new SecurityConfiguration();
			List<Exception> exceptions = null;

			// Act & Assert
			var exception = Assert.Throws<ArgumentException>(() => config.Validate(ref exceptions));
			Assert.That(exception.Message, Does.Contain("exceptions"));
		}

		[Test]
		public void SecurityConfiguration_Validate_WithoutBaseUrl_ShouldAddException_Test()
		{
			// Arrange
			var config = new SecurityConfiguration
			{
				Realm = "local-realm",
				ClientId = "local-client"
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions.Any(e => e.Message.Contains("BaseUrl")), Is.True);
		}

		[Test]
		public void SecurityConfiguration_Validate_WithoutRealm_ShouldAddException_Test()
		{
			// Arrange
			var config = new SecurityConfiguration
			{
				BaseUrl = "http://localhost:55001",
				ClientId = "local-client"
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions.Any(e => e.Message.Contains("Realm")), Is.True);
		}

		[Test]
		public void SecurityConfiguration_Validate_WithoutClientId_ShouldAddException_Test()
		{
			// Arrange
			var config = new SecurityConfiguration
			{
				BaseUrl = "http://localhost:55001",
				Realm = "local-realm"
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions.Any(e => e.Message.Contains("ClientId")), Is.True);
		}

		[Test]
		public void SecurityConfiguration_Validate_WithNegativeClockSkewSeconds_ShouldAddException_Test()
		{
			// Arrange
			var config = new SecurityConfiguration
			{
				BaseUrl = "http://localhost:55001",
				Realm = "local-realm",
				ClientId = "local-client",
				ClockSkewSeconds = -1
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions.Any(e => e.Message.Contains("ClockSkewSeconds")), Is.True);
		}

		[Test]
		public void SecurityConfiguration_Validate_WithValidConfiguration_ShouldNotAddExceptions_Test()
		{
			// Arrange
			var config = new SecurityConfiguration
			{
				Active = true,
				BaseUrl = "http://localhost:55001",
				Realm = "local-realm",
				ClientId = "local-client",
				RequireHttpsMetadata = false
			};
			var exceptions = new List<Exception>();

			// Act
			config.Validate(ref exceptions);

			// Assert
			Assert.That(exceptions.Count, Is.EqualTo(0));
		}
	}
}

using NUnit.Framework;
using PilotApi.Shared.Utilities;

namespace PilotApi.Shared.Tests.Utilities
{
	[TestFixture]
	public class SecurityUtilitiesTests
	{
		[Test]
		public void SecurityUtilities_ConnectionStringClean_WithSqlServerConnectionString_ShouldRedactPassword_Test()
		{
			// Arrange
			var connectionString = "Data Source=localhost;Initial Catalog=MainDB;User Id=sa;Password=MyPassword;";

			// Act
			var result = SecurityUtilities.ConnectionStringClean(connectionString);

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("Data Source=localhost"));
			Assert.That(result, Does.Contain("Initial Catalog=MainDB"));
			Assert.That(result, Does.Contain("User Id=sa"));
			Assert.That(result, Does.Contain("[Redacted]"));
			Assert.That(result, Does.Not.Contain("MyPassword"));
		}

		[Test]
		public void SecurityUtilities_ConnectionStringClean_WithPostgreSqlConnectionString_ShouldRedactPassword_Test()
		{
			// Arrange
			var connectionString = "Host=localhost;Database=maindb;Username=postgres;Password=pgPassword;";

			// Act
			var result = SecurityUtilities.ConnectionStringClean(connectionString);

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("Host=localhost"));
			Assert.That(result, Does.Contain("Database=maindb"));
			Assert.That(result, Does.Contain("Username=postgres"));
			Assert.That(result, Does.Contain("[Redacted]"));
			Assert.That(result, Does.Not.Contain("pgPassword"));
		}

		[Test]
		public void SecurityUtilities_ConnectionStringClean_WithCaseInsensitivePassword_ShouldRedactPassword_Test()
		{
			// Arrange
			var connectionString = "Server=localhost;Database=TestDB;User Id=admin;password=SecretPassword;";

			// Act
			var result = SecurityUtilities.ConnectionStringClean(connectionString);

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("[Redacted]"));
			Assert.That(result, Does.Not.Contain("SecretPassword"));
		}

		[Test]
		public void SecurityUtilities_ConnectionStringClean_WithPASSWORDInAllCaps_ShouldRedactPassword_Test()
		{
			// Arrange
			var connectionString = "Server=localhost;Database=TestDB;PASSWORD=TopSecret;";

			// Act
			var result = SecurityUtilities.ConnectionStringClean(connectionString);

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("[Redacted]"));
			Assert.That(result, Does.Not.Contain("TopSecret"));
		}

		[Test]
		public void SecurityUtilities_ConnectionStringClean_WithNullConnectionString_ShouldReturnNull_Test()
		{
			// Arrange
			string connectionString = null;

			// Act
			var result = SecurityUtilities.ConnectionStringClean(connectionString);

			// Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public void SecurityUtilities_ConnectionStringClean_WithEmptyConnectionString_ShouldReturnEmpty_Test()
		{
			// Arrange
			var connectionString = "";

			// Act
			var result = SecurityUtilities.ConnectionStringClean(connectionString);

			// Assert
			Assert.That(result, Is.EqualTo(""));
		}

		[Test]
		public void SecurityUtilities_ConnectionStringClean_WithWhitespaceOnlyConnectionString_ShouldReturnWhitespace_Test()
		{
			// Arrange
			var connectionString = "   ";

			// Act
			var result = SecurityUtilities.ConnectionStringClean(connectionString);

			// Assert
			Assert.That(result, Is.EqualTo("   "));
		}

		[Test]
		public void SecurityUtilities_ConnectionStringClean_WithoutPasswordField_ShouldReturnClean_Test()
		{
			// Arrange
			var connectionString = "Server=localhost;Database=TestDB;User Id=admin;";

			// Act
			var result = SecurityUtilities.ConnectionStringClean(connectionString);

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("Server=localhost"));
			Assert.That(result, Does.Contain("Database=TestDB"));
			Assert.That(result, Does.Not.Contain("[Redacted]"));
		}

		[Test]
		public void SecurityUtilities_ConnectionStringClean_WithMultiplePasswords_ShouldRedactAll_Test()
		{
			// Arrange
			var connectionString = "Server=localhost;Password=Password1;Database=TestDB;Password=Password2;";

			// Act
			var result = SecurityUtilities.ConnectionStringClean(connectionString);

			// Assert
			Assert.NotNull(result);
			var redactedCount = result.Split("[Redacted]").Length - 1;
			Assert.That(redactedCount, Is.GreaterThanOrEqualTo(1));
			Assert.That(result, Does.Not.Contain("Password1"));
			Assert.That(result, Does.Not.Contain("Password2"));
		}

		[Test]
		public void SecurityUtilities_ConnectionStringClean_WithPasswordContainingEquals_ShouldRedactCorrectly_Test()
		{
			// Arrange
			var connectionString = "Server=localhost;Password=Pass=Word;User=admin;";

			// Act
			var result = SecurityUtilities.ConnectionStringClean(connectionString);

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("[Redacted]"));
		}

		[Test]
		public void SecurityUtilities_ConnectionStringClean_WithInvalidSeparators_ShouldHandleGracefully_Test()
		{
			// Arrange
			var connectionString = "Server=localhost;;Database=TestDB;";

			// Act
			var result = SecurityUtilities.ConnectionStringClean(connectionString);

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("Server=localhost"));
			Assert.That(result, Does.Contain("Database=TestDB"));
		}

		[Test]
		public void SecurityUtilities_ConnectionStringClean_WithLeadingAndTrailingSemicolons_ShouldHandleGracefully_Test()
		{
			// Arrange
			var connectionString = ";Server=localhost;Database=TestDB;";

			// Act
			var result = SecurityUtilities.ConnectionStringClean(connectionString);

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("Server=localhost"));
		}

		[Test]
		public void SecurityUtilities_ConnectionStringClean_WithSpacesAroundSemicolons_ShouldHandleGracefully_Test()
		{
			// Arrange
			var connectionString = "Server=localhost ; Database=TestDB ; Password=Secret ;";

			// Act
			var result = SecurityUtilities.ConnectionStringClean(connectionString);

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("localhost"));
		}

		[Test]
		public void SecurityUtilities_ConnectionStringClean_PreservesAllNonPasswordFields_Test()
		{
			// Arrange
			var connectionString = "Data Source=MyServer;Initial Catalog=MyDB;User Id=MyUser;Password=MyPassword;Connect Timeout=30;";

			// Act
			var result = SecurityUtilities.ConnectionStringClean(connectionString);

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("Data Source=MyServer"));
			Assert.That(result, Does.Contain("Initial Catalog=MyDB"));
			Assert.That(result, Does.Contain("User Id=MyUser"));
			Assert.That(result, Does.Contain("Connect Timeout=30"));
			Assert.That(result, Does.Contain("[Redacted]"));
		}

		[Test]
		public void SecurityUtilities_ConnectionStringClean_ReturnsSameFormatAsInput_Test()
		{
			// Arrange
			var connectionString = "Server=localhost;Database=TestDB;";

			// Act
			var result = SecurityUtilities.ConnectionStringClean(connectionString);

			// Assert
			Assert.NotNull(result);
			Assert.That(result.Contains(";"), Is.True);
		}
	}
}


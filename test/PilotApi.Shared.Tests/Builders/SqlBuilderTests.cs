using Microsoft.Extensions.Logging;
using NUnit.Framework;
using PilotApi.Shared.Handlers;
using PilotApi.Shared.Constants;
using PilotApi.Shared.Contracts.Configuration;
using PilotApi.Shared.Exceptions;
using PilotApi.TestingShared.Utilities;
using System;
using System.Collections.Generic;

namespace PilotApi.Shared.Tests.Builders
{
	[TestFixture]
	public class SqlBuilderTests
	{
		private ILoggerFactory loggerFactory;
		private IApplicationConfiguration applicationConfiguration;
		private SqlBuilder sqlBuilder;

		[SetUp]
		public void Setup()
		{
			loggerFactory = new LoggerFactory();

			applicationConfiguration = TestingSharedDoublesUtilities.GetApplicationConfiguration(DataSourceTypes.SqlServer);
			sqlBuilder = new SqlBuilder(loggerFactory, applicationConfiguration);
		}

		[Test]
		public void SqlBuilder_Constructor_WithValidParameters_ShouldInitialize_Test()
		{
			// Arrange & Act - Constructor called in Setup

			// Assert
			Assert.NotNull(sqlBuilder);
			Assert.IsInstanceOf<ISqlBuilder>(sqlBuilder);
		}

		[Test]
		public void SqlBuilder_Constructor_WithNullLoggerFactory_ShouldThrow_Test()
		{
			// Arrange & Act & Assert
			Assert.Throws<ArgumentNullException>(() =>
				new SqlBuilder(null, applicationConfiguration));
		}

		[Test]
		public void SqlBuilder_Constructor_WithNullApplicationConfiguration_ShouldThrow_Test()
		{
			// Arrange & Act & Assert
			Assert.Throws<ConfigurationException>(() =>
				new SqlBuilder(loggerFactory, null));
		}

		[Test]
		public void SqlBuilder_BuildCount_WithValidTableName_ShouldReturnValidSql_Test()
		{
			// Arrange
			var tableName = "Users";

			// Act
			var result = sqlBuilder.BuildCount(tableName);

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("COUNT(*)"));
			Assert.That(result, Does.Contain("[dbo].[Users]"));
		}

		[Test]
		public void SqlBuilder_BuildCount_WithNullTableName_ShouldThrow_Test()
		{
			// Arrange & Act & Assert
			var exception = Assert.Throws<ArgumentException>(() =>
				sqlBuilder.BuildCount(null));
			Assert.That(exception.Message, Does.Contain("tableName"));
		}

		[Test]
		public void SqlBuilder_BuildCount_WithEmptyTableName_ShouldThrow_Test()
		{
			// Arrange & Act & Assert
			var exception = Assert.Throws<ArgumentException>(() =>
				sqlBuilder.BuildCount(""));
			Assert.That(exception.Message, Does.Contain("tableName"));
		}

		[Test]
		public void SqlBuilder_BuildDelete_WithValidParameters_ShouldReturnValidSql_Test()
		{
			// Arrange
			var tableName = "Users";
			var keyColumnNames = new List<string> { "UserId" };

			// Act
			var result = sqlBuilder.BuildDelete(tableName, keyColumnNames);

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("DELETE FROM"));
			Assert.That(result, Does.Contain("[dbo].[Users]"));
			Assert.That(result, Does.Contain("WHERE"));
		}

		[Test]
		public void SqlBuilder_BuildDelete_WithNullTableName_ShouldThrow_Test()
		{
			// Arrange
			var keyColumnNames = new List<string> { "UserId" };

			// Act & Assert
			var exception = Assert.Throws<ArgumentException>(() =>
				sqlBuilder.BuildDelete(null, keyColumnNames));
			Assert.That(exception.Message, Does.Contain("tableName"));
		}

		[Test]
		public void SqlBuilder_BuildDelete_WithNullKeyColumnNames_ShouldThrow_Test()
		{
			// Arrange
			var tableName = "Users";

			// Act & Assert
			var exception = Assert.Throws<ArgumentException>(() =>
				sqlBuilder.BuildDelete(tableName, null));
			Assert.That(exception.Message, Does.Contain("keyColumnNames"));
		}

		[Test]
		public void SqlBuilder_BuildDelete_WithEmptyKeyColumnNames_ShouldThrow_Test()
		{
			// Arrange
			var tableName = "Users";
			var keyColumnNames = new List<string>();

			// Act & Assert
			var exception = Assert.Throws<ArgumentException>(() =>
				sqlBuilder.BuildDelete(tableName, keyColumnNames));
			Assert.That(exception.Message, Does.Contain("keyColumnNames"));
		}

		[Test]
		public void SqlBuilder_BuildGetNextId_WithIntType_ShouldReturnValidSql_Test()
		{
			// Arrange
			var tableName = "Users";
			var keyColumnName = "UserId";
			var keyDataType = KeyColumnDataTypeConstants.Int;

			// Act
			var result = sqlBuilder.BuildGetNextId(tableName, keyColumnName, keyDataType);

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("MAX"));
			Assert.That(result, Does.Contain("[UserId]"));
		}

		[Test]
		public void SqlBuilder_BuildGetNextId_WithStringType_SqlServer_ShouldReturnValidSql_Test()
		{
			// Arrange
			var tableName = "Users";
			var keyColumnName = "UserId";
			var keyDataType = KeyColumnDataTypeConstants.String;

			// Act
			var result = sqlBuilder.BuildGetNextId(tableName, keyColumnName, keyDataType);

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("GenerateUniqueKeyDynamic"));
		}

		[Test]
		public void SqlBuilder_BuildGetNextId_WithNullTableName_ShouldThrow_Test()
		{
			// Arrange & Act & Assert
			var exception = Assert.Throws<ArgumentException>(() =>
				sqlBuilder.BuildGetNextId(null, "Id", KeyColumnDataTypeConstants.Int));
			Assert.That(exception.Message, Does.Contain("tableName"));
		}

		[Test]
		public void SqlBuilder_BuildGetNextId_WithNullKeyColumnName_ShouldThrow_Test()
		{
			// Arrange & Act & Assert
			var exception = Assert.Throws<ArgumentException>(() =>
				sqlBuilder.BuildGetNextId("Users", null, KeyColumnDataTypeConstants.Int));
			Assert.That(exception.Message, Does.Contain("keyColumnName"));
		}

		[Test]
		public void SqlBuilder_BuildGetNextId_WithNullKeyDataType_ShouldThrow_Test()
		{
			// Arrange & Act & Assert
			var exception = Assert.Throws<ArgumentException>(() =>
				sqlBuilder.BuildGetNextId("Users", "Id", null));
			Assert.That(exception.Message, Does.Contain("keyDataType"));
		}

		[Test]
		public void SqlBuilder_BuildInsert_WithValidParameters_ShouldReturnValidSql_Test()
		{
			// Arrange
			var tableName = "Users";
			var columnNames = new List<string> { "UserId", "UserName", "Email" };
			var keyColumnNames = new List<string> { "UserId" };
			var entityColumns = new List<string> { "userId", "userName", "email" };

			// Act
			var result = sqlBuilder.BuildInsert(tableName, columnNames, keyColumnNames, entityColumns);

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("INSERT INTO"));
			Assert.That(result, Does.Contain("[dbo].[Users]"));
			Assert.That(result, Does.Contain("VALUES"));
			Assert.That(result, Does.Contain("SCOPE_IDENTITY"));
		}

		[Test]
		public void SqlBuilder_BuildInsert_WithNullTableName_ShouldThrow_Test()
		{
			// Arrange & Act & Assert
			var exception = Assert.Throws<ArgumentException>(() =>
				sqlBuilder.BuildInsert(null, new List<string>(), new List<string>(), new List<string>()));
			Assert.That(exception.Message, Does.Contain("tableName"));
		}

		[Test]
		public void SqlBuilder_BuildInsert_WithNullColumnNames_ShouldThrow_Test()
		{
			// Arrange & Act & Assert
			var exception = Assert.Throws<ArgumentException>(() =>
				sqlBuilder.BuildInsert("Users", null, new List<string>(), new List<string>()));
			Assert.That(exception.Message, Does.Contain("columnNames"));
		}

		[Test]
		public void SqlBuilder_BuildConnectionString_WithValidConfiguration_ShouldReturnValidSql_Test()
		{
			// Arrange & Act
			var result = sqlBuilder.BuildConnectionString(applicationConfiguration.OpenApi);

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("Data Source="));
			Assert.That(result, Does.Contain("Initial Catalog="));
			Assert.That(result, Does.Contain("User Id="));
			Assert.That(result, Does.Contain("Password="));
		}

		[Test]
		public void SqlBuilder_BuildConnectionString_WithNullOpenApi_ShouldThrow_Test()
		{
			// Arrange & Act & Assert
			var exception = Assert.Throws<ArgumentException>(() =>
				sqlBuilder.BuildConnectionString(null));
			Assert.That(exception.Message, Does.Contain("openApi"));
		}

		[Test]
		public void SqlBuilder_BuildSelect_WithValidParameters_ShouldReturnValidSql_Test()
		{
			// Arrange
			var tableName = "Users";
			var columnNames = new List<string> { "UserId", "UserName", "Email" };

			// Act
			var result = sqlBuilder.BuildSelect(tableName, columnNames);

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("SELECT"));
			Assert.That(result, Does.Contain("[UserId]"));
			Assert.That(result, Does.Contain("[UserName]"));
			Assert.That(result, Does.Contain("[Email]"));
			Assert.That(result, Does.Contain("FROM [dbo].[Users]"));
		}

		[Test]
		public void SqlBuilder_BuildUpdate_WithValidParameters_ShouldReturnValidSql_Test()
		{
			// Arrange
			var tableName = "Users";
			var columnNames = new List<string> { "UserName", "Email" };
			var keyColumnNames = new List<string> { "UserId" };
			var entityColumns = new List<string> { "userName", "email" };

			// Act
			var result = sqlBuilder.BuildUpdate(tableName, columnNames, keyColumnNames, entityColumns);

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("UPDATE"));
			Assert.That(result, Does.Contain("[dbo].[Users]"));
			Assert.That(result, Does.Contain("SET"));
			Assert.That(result, Does.Contain("WHERE"));
		}

		[Test]
		public void SqlBuilder_IsSqlServer_WithSqlServerConfiguration_ShouldBeTrue_Test()
		{
			// Arrange, Act & Assert - verified by BuildConnectionString containing SQL Server syntax
			var result = sqlBuilder.BuildConnectionString(applicationConfiguration.OpenApi);
			Assert.That(result, Does.Contain("Data Source="));
		}

		[TearDown]
		public void TearDown()
		{
			loggerFactory?.Dispose();
		}
	}
}


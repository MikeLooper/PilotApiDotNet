using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Npgsql;
using NUnit.Framework;
using PilotApi.Repositories.DataSource;
using PilotApi.Shared.Configuration;
using PilotApi.Shared.Constants;
using PilotApi.Shared.Contracts.Configuration;
using PilotApi.Shared.Handlers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PilotApi.Repositories.Tests.DataSource
{
	[TestFixture]
	public class DataSourceContextTests : TestBase
	{
		private class MockLogger : ILogger
		{
			public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
			public bool IsEnabled(LogLevel logLevel) => true;
			public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
		}

		private class MockLoggerFactory : ILoggerFactory
		{
			public ILogger CreateLogger(string categoryName) => new MockLogger();
			public void AddProvider(ILoggerProvider provider) { }
			public void Dispose() { }
		}

		private class MockSqlBuilder : ISqlBuilder
		{
			public string? BuildConnectionString(OpenApiConfiguration? openApi)
			{
				return "Server=localhost;Database=TestDb;Password=secret;";
			}

			public string BuildCount(string tableName) => $"SELECT COUNT(*) FROM {tableName}";
			public string BuildDelete(string tableName, List<string> keyColumnNames) => $"DELETE FROM {tableName}";
			public string BuildGetNextId(string tableName, string keyColumnName, string keyDataType) => $"SELECT MAX({keyColumnName}) FROM {tableName}";
			public string BuildInsert(string tableName, List<string> columnNames, List<string> keyColumnNames, List<string> entityColumns, bool keyIsAutoIncrement = true) => $"INSERT INTO {tableName}";
			public string BuildSelect(string tableName, List<string> columnNames, List<string>? keyColumnNames = null) => $"SELECT * FROM {tableName}";
			public string BuildUpdate(string tableName, List<string> columnNames, List<string> keyColumnNames, List<string> entityColumns, bool keyIsAutoIncrement = true) => $"UPDATE {tableName}";
			public string BuildWhereClause(List<string> keyColumnNames) => "WHERE 1=1";
		}

		private class TestableDataSourceContext : DataSourceContext
		{
			public TestableDataSourceContext(
				ILoggerFactory loggerFactory,
				IApplicationConfiguration applicationConfiguration,
				ISqlBuilder sqlBuilder)
				: base(loggerFactory, applicationConfiguration, sqlBuilder)
			{
			}

			public void SetDataSourceConnection(IDbConnection connection)
			{
				this.DataSourceConnectionInternal = connection;
			}

			public void InvokeDispose(bool disposing)
			{
				base.Dispose(disposing);
			}
		}

		private class FakeDbConnection : IDbConnection
		{
			public FakeDbConnection(ConnectionState initialState = ConnectionState.Closed)
			{
				this.State = initialState;
			}

			public string ConnectionString { get; set; } = string.Empty;
			public int ConnectionTimeout => 30;
			public string Database => "TestDb";
			public ConnectionState State { get; private set; }
			public bool BeginTransactionCalled { get; private set; }
			public bool CloseCalled { get; private set; }
			public bool DisposeCalled { get; private set; }
			public bool OpenCalled { get; private set; }
			public FakeDbTransaction? CreatedTransaction { get; private set; }

			public IDbTransaction BeginTransaction()
			{
				this.BeginTransactionCalled = true;
				this.CreatedTransaction = new FakeDbTransaction(this);
				return this.CreatedTransaction;
			}

			public IDbTransaction BeginTransaction(IsolationLevel il)
			{
				return this.BeginTransaction();
			}

			public void ChangeDatabase(string databaseName)
			{
			}

			public void Close()
			{
				this.CloseCalled = true;
				this.State = ConnectionState.Closed;
			}

			public IDbCommand CreateCommand()
			{
				throw new NotSupportedException();
			}

			public void Dispose()
			{
				this.DisposeCalled = true;
			}

			public void Open()
			{
				this.OpenCalled = true;
				this.State = ConnectionState.Open;
			}
		}

		private class FakeDbTransaction : IDbTransaction
		{
			public FakeDbTransaction(IDbConnection connection)
			{
				this.Connection = connection;
			}

			public IDbConnection Connection { get; }
			public IsolationLevel IsolationLevel => IsolationLevel.ReadCommitted;
			public bool CommitCalled { get; private set; }
			public bool DisposeCalled { get; private set; }
			public bool RollbackCalled { get; private set; }

			public void Commit()
			{
				this.CommitCalled = true;
			}

			public void Dispose()
			{
				this.DisposeCalled = true;
			}

			public void Rollback()
			{
				this.RollbackCalled = true;
			}
		}

		[Test]
		public void DataSourceContextConstructorSetsActiveConfiguration_Test()
		{
			// Arrange
			var applicationConfiguration = CreateApplicationConfiguration(DataSourceTypes.SqlServer);
			var loggerFactory = new MockLoggerFactory();
			var sqlBuilder = new MockSqlBuilder();

			// Act
			var dataSourceContext = new DataSourceContext(loggerFactory, applicationConfiguration, sqlBuilder);

			// Assert
			Assert.That(dataSourceContext.DataConnectionConfiguration, Is.SameAs(applicationConfiguration.DataConnections![0]));
			Assert.That(dataSourceContext.DataSourceConfiguration, Is.SameAs(applicationConfiguration.DataSources![0]));
		}

		[Test]
		public void DataSourceContextGetConnectionBuildsSqlConnectionAndSanitizesConnectionString_Test()
		{
			// Arrange
			var dataSourceContext = new DataSourceContext(new MockLoggerFactory(), CreateApplicationConfiguration(DataSourceTypes.SqlServer), new MockSqlBuilder());

			// Act
			var connection = dataSourceContext.GetConnection(false);

			// Assert
			Assert.That(connection, Is.TypeOf<SqlConnection>());
			Assert.That(connection.State, Is.EqualTo(ConnectionState.Closed));
			Assert.That(dataSourceContext.ConnectionStringClean, Is.EqualTo($"Server=localhost;Database=TestDb;Password={StringConstants.Redacted}"));
			Assert.That(dataSourceContext.DataSourceTransaction, Is.Null);
		}

		[Test]
		public void DataSourceContextGetConnectionBuildsPostgreSqlConnection_Test()
		{
			// Arrange
			var dataSourceContext = new DataSourceContext(new MockLoggerFactory(), CreateApplicationConfiguration(DataSourceTypes.PostgreSQL), new MockSqlBuilder());

			// Act
			var connection = dataSourceContext.GetConnection(false);

			// Assert
			Assert.That(connection, Is.TypeOf<NpgsqlConnection>());
			Assert.That(connection.State, Is.EqualTo(ConnectionState.Closed));
		}

		[Test]
		public void DataSourceContextGetConnectionReturnsSameConnectionInstance_Test()
		{
			// Arrange
			var dataSourceContext = new DataSourceContext(new MockLoggerFactory(), CreateApplicationConfiguration(DataSourceTypes.SqlServer), new MockSqlBuilder());

			// Act
			var firstConnection = dataSourceContext.GetConnection(false);
			var secondConnection = dataSourceContext.GetConnection(false);

			// Assert
			Assert.That(secondConnection, Is.SameAs(firstConnection));
		}

		[Test]
		public void DataSourceContextGetConnectionOpensInjectedConnectionAndStartsTransaction_Test()
		{
			// Arrange
			var dataSourceContext = new TestableDataSourceContext(new MockLoggerFactory(), CreateApplicationConfiguration(DataSourceTypes.SqlServer), new MockSqlBuilder());
			var fakeConnection = new FakeDbConnection();
			dataSourceContext.SetDataSourceConnection(fakeConnection);

			// Act
			var connection = dataSourceContext.GetConnection();

			// Assert
			Assert.That(connection, Is.SameAs(fakeConnection));
			Assert.That(fakeConnection.OpenCalled, Is.True);
			Assert.That(fakeConnection.BeginTransactionCalled, Is.True);
			Assert.That(dataSourceContext.DataSourceTransaction, Is.SameAs(fakeConnection.CreatedTransaction));
		}

		[Test]
		public async Task DataSourceContextCommitCommitsTransaction_Test()
		{
			// Arrange
			var dataSourceContext = new TestableDataSourceContext(new MockLoggerFactory(), CreateApplicationConfiguration(DataSourceTypes.SqlServer), new MockSqlBuilder());
			var fakeTransaction = new FakeDbTransaction(new FakeDbConnection(ConnectionState.Open));
			dataSourceContext.DataSourceTransaction = fakeTransaction;

			// Act
			await dataSourceContext.Commit();

			// Assert
			Assert.That(fakeTransaction.CommitCalled, Is.True);
		}

		[Test]
		public async Task DataSourceContextRollbackRollsBackTransaction_Test()
		{
			// Arrange
			var dataSourceContext = new TestableDataSourceContext(new MockLoggerFactory(), CreateApplicationConfiguration(DataSourceTypes.SqlServer), new MockSqlBuilder());
			var fakeConnection = new FakeDbConnection(ConnectionState.Open);
			var fakeTransaction = new FakeDbTransaction(fakeConnection);
			dataSourceContext.DataSourceTransaction = fakeTransaction;

			// Act
			await dataSourceContext.Rollback();

			// Assert
			Assert.That(fakeTransaction.RollbackCalled, Is.True);
		}

		[Test]
		public void DataSourceContextToStringReturnsSanitizedValues_Test()
		{
			// Arrange
			var dataSourceContext = new DataSourceContext(new MockLoggerFactory(), CreateApplicationConfiguration(DataSourceTypes.SqlServer), new MockSqlBuilder());
			dataSourceContext.GetConnection(false);

			// Act
			var result = dataSourceContext.ToString();

			// Assert
			Assert.That(result, Does.Contain($"ConnectionString=Server=localhost;Database=TestDb;Password={StringConstants.Redacted}"));
			Assert.That(result, Does.Contain($"{nameof(DataSourceContext.DataSourceTransaction)}={StringConstants.LogNull}"));
		}

		[Test]
		public void DataSourceContextDisposeClosesConnectionAndDisposesTransaction_Test()
		{
			// Arrange
			var dataSourceContext = new TestableDataSourceContext(new MockLoggerFactory(), CreateApplicationConfiguration(DataSourceTypes.SqlServer), new MockSqlBuilder());
			var fakeConnection = new FakeDbConnection(ConnectionState.Open);
			var fakeTransaction = new FakeDbTransaction(fakeConnection);
			dataSourceContext.SetDataSourceConnection(fakeConnection);
			dataSourceContext.DataSourceTransaction = fakeTransaction;

			// Act
			dataSourceContext.Dispose();

			// Assert
			Assert.That(fakeConnection.CloseCalled, Is.True);
			Assert.That(fakeConnection.DisposeCalled, Is.True);
			Assert.That(fakeTransaction.DisposeCalled, Is.True);
			Assert.That(dataSourceContext.DataSourceTransaction, Is.Null);
		}

		[Test]
		public void DataSourceContextGetConnectionWithUnhandledDataSourceThrowsInvalidOperationException_Test()
		{
			// Arrange
			var dataSourceContext = new DataSourceContext(new MockLoggerFactory(), CreateApplicationConfiguration(DataSourceTypes.Unrecognized), new MockSqlBuilder());

			// Act
			var action = () => dataSourceContext.GetConnection(false);

			// Assert
			Assert.That(action, Throws.TypeOf<InvalidOperationException>());
		}

		private static ApplicationConfiguration CreateApplicationConfiguration(DataSourceTypes dataSourceType)
		{
			return new ApplicationConfiguration
			{
				DataConnections = new List<DataConnectionConfiguration>
				{
					new DataConnectionConfiguration
					{
						Active = true,
						DataSourceName = "NorthwindConnection",
						Host = "localhost",
						Password = "secret",
						UserName = "sa"
					}
				},
				DataSources = new List<DataSourceConfiguration>
				{
					new DataSourceConfiguration
					{
						Active = true,
						DataSource = "Northwind",
						DataSourceEnum = dataSourceType,
						DataSourceName = "NorthwindConnection",
						DataSourceType = dataSourceType.ToString(),
						Schema = "dbo"
					}
				},
				OpenApi = new OpenApiConfiguration()
			};
		}
	}
}


using NUnit.Framework;
using PilotApi.Integration.Tests.Testing.Doubles;
using PilotApi.Integration.Tests.Testing.Utilities;
using PilotApi.Repositories.Handlers;
using PilotApi.Repositories.Models.Entities;
using PilotApi.Repositories.Repositories;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PilotApi.Integration.Tests.Repositories
{
	/// <summary>
	/// Integration tests that exercise the real <see cref="CustomersRepository"/>, real <see cref="SqlBuilder"/>,
	/// real <see cref="PilotApi.Repositories.DataSource.DataSourceContext"/> (via <see cref="TestableDataSourceContext"/>),
	/// and real <see cref="EntityUpdateHandler"/> together. Only the raw ADO.NET connection/transaction/reader
	/// (<see cref="FakeDbConnection"/>) is faked, since no live database server is available in this environment.
	/// </summary>
	/// <remarks>
	/// Each test builds its own local, disposable set of dependencies (via <see cref="IntegrationTestDoublesUtilities"/>)
	/// rather than sharing state through class-level fields, to avoid any risk of cross-test data corruption.
	/// </remarks>
	[TestFixture]
	public class CustomersRepositoryIntegrationTests
	{
		[Test]
		public async Task DeleteAsync_WhenNoRowsAffected_RollsBackRealTransaction_AndReturnsError_Test()
		{
			// Arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetCustomersRepository(dataSourceContext, sqlBuilder);

			fakeConnection.NextNonQueryResult = 0;

			// Act
			var response = await repository.DeleteAsync(new[] { "ALFKI" });

			// Assert
			Assert.That(response.Result, Is.False);
			Assert.That(response.IsError, Is.True);

			var transaction = (FakeDbTransaction)dataSourceContext.DataSourceTransaction!;
			Assert.That(transaction.RollbackCalled, Is.True);
			Assert.That(transaction.CommitCalled, Is.False);
		}

		[Test]
		public async Task DeleteAsync_WhenRowsAffected_CommitsRealTransaction_Test()
		{
			// Arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetCustomersRepository(dataSourceContext, sqlBuilder);

			fakeConnection.NextNonQueryResult = 1;

			// Act
			var response = await repository.DeleteAsync(new[] { "ALFKI" });

			// Assert
			Assert.That(response.Result, Is.True);

			var executedCommand = fakeConnection.ExecutedCommands.Single();
			Assert.That(executedCommand.CommandText, Does.Contain("DELETE"));

			var transaction = (FakeDbTransaction)dataSourceContext.DataSourceTransaction!;
			Assert.That(transaction.CommitCalled, Is.True);
			Assert.That(transaction.RollbackCalled, Is.False);
		}

		[Test]
		public async Task GetAllAsync_ReturnsMappedEntities_UsingRealSqlBuilderGeneratedQuery_Test()
		{
			// Arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetCustomersRepository(dataSourceContext, sqlBuilder);

			var table = new DataTable();
			table.Columns.Add(nameof(CustomersEntity.CustomerID), typeof(string));
			table.Columns.Add(nameof(CustomersEntity.CompanyName), typeof(string));
			table.Rows.Add("ALFKI", "Alfreds Futterkiste");
			table.Rows.Add("ANATR", "Ana Trujillo Emparedados");
			fakeConnection.NextQueryResult = table;

			// Act
			var response = await repository.GetAllAsync();

			// Assert
			Assert.That(response, Is.Not.Null);
			Assert.That(response!.IsError, Is.False);
			Assert.That(response.Result, Has.Count.EqualTo(2));
			Assert.That(response.Result![0].CompanyName, Is.EqualTo("Alfreds Futterkiste"));

			var executedCommand = fakeConnection.ExecutedCommands.Single();
			Assert.That(executedCommand.CommandText, Does.Contain("SELECT"));
			Assert.That(executedCommand.CommandText, Does.Contain("Customers"));
		}

		[Test]
		public async Task GetAsync_BuildsRealParameterizedQuery_AndCommitsTransaction_Test()
		{
			// Arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetCustomersRepository(dataSourceContext, sqlBuilder);

			var table = new DataTable();
			table.Columns.Add(nameof(CustomersEntity.CustomerID), typeof(string));
			table.Columns.Add(nameof(CustomersEntity.CompanyName), typeof(string));
			table.Rows.Add("ALFKI", "Alfreds Futterkiste");
			fakeConnection.NextQueryResult = table;

			// Act
			var response = await repository.GetAsync(new[] { "ALFKI" });

			// Assert
			Assert.That(response, Is.Not.Null);
			Assert.That(response!.Result, Is.Not.Null);
			Assert.That(response.Result!.CompanyName, Is.EqualTo("Alfreds Futterkiste"));

			var executedCommand = fakeConnection.ExecutedCommands.Single();
			Assert.That(executedCommand.CommandText, Does.Contain("WHERE"));
			Assert.That(executedCommand.Parameters, Does.ContainKey("CustomerID"));
			Assert.That(executedCommand.Parameters["CustomerID"], Is.EqualTo("ALFKI"));
		}

		[Test]
		public void GetAsync_WithNoIds_ThrowsArgumentException_Test()
		{
			// Arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetCustomersRepository(dataSourceContext, sqlBuilder);

			// Act, Assert
			Assert.ThrowsAsync<ArgumentException>(async () => await repository.GetAsync(Array.Empty<string>()));
		}
	}
}

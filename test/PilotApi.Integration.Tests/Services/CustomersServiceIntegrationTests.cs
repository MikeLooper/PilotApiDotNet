using NUnit.Framework;
using PilotApi.Domain.Models.Dto;
using PilotApi.Integration.Tests.Testing.Doubles;
using PilotApi.Integration.Tests.Testing.Utilities;
using PilotApi.Repositories.Models.Entities;
using PilotApi.Services.Handlers;
using PilotApi.Services.Services;
using System;
using System.Data;
using System.Threading.Tasks;

namespace PilotApi.Integration.Tests.Services
{
	/// <summary>
	/// Integration tests that exercise the real <see cref="CustomersService"/>, real <see cref="DataMapperHandler"/>,
	/// and the real <see cref="PilotApi.Repositories.Repositories.CustomersRepository"/> (built via
	/// <see cref="IntegrationTestDoublesUtilities.GetCustomersRepository"/>) all wired together. Only the raw ADO.NET
	/// connection (<see cref="FakeDbConnection"/>) is faked, since no live database server is available in this environment.
	/// </summary>
	/// <remarks>
	/// Each test builds its own local, disposable set of dependencies (via <see cref="IntegrationTestDoublesUtilities"/>)
	/// rather than sharing state through class-level fields, to avoid any risk of cross-test data corruption.
	/// </remarks>
	[TestFixture]
	public class CustomersServiceIntegrationTests
	{
		[Test]
		public async Task CustomersService_GetAllAsync_ReturnsMappedDtos_Test()
		{
			// arrange
			var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetCustomersRepository(dataSourceContext, sqlBuilder);

			var table = new DataTable();
			table.Columns.Add(nameof(CustomersEntity.CustomerID), typeof(string));
			table.Columns.Add(nameof(CustomersEntity.CompanyName), typeof(string));
			table.Columns.Add(nameof(CustomersEntity.City), typeof(string));
			table.Rows.Add("ALFKI", "Alfreds Futterkiste", "Berlin");
			fakeConnection.NextQueryResult = table;

			var testObject = new CustomersService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act
			var result = await testObject.GetAllAsync();

			// assert
			Assert.IsNotNull(result);
			Assert.That(result!.Result!.Count, Is.EqualTo(1));
			Assert.That(result.Result![0].CustomerID, Is.EqualTo("ALFKI"));
			Assert.That(result.Result![0].City, Is.EqualTo("Berlin"));
		}

		[Test]
		public async Task CustomersService_GetByIdAsync_ReturnsMappedDto_Test()
		{
			// arrange
			var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetCustomersRepository(dataSourceContext, sqlBuilder);

			var table = new DataTable();
			table.Columns.Add(nameof(CustomersEntity.CustomerID), typeof(string));
			table.Columns.Add(nameof(CustomersEntity.CompanyName), typeof(string));
			table.Rows.Add("ANATR", "Ana Trujillo");
			fakeConnection.NextQueryResult = table;

			var testObject = new CustomersService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act
			var result = await testObject.GetByIdAsync(new[] { "ANATR" });

			// assert
			Assert.IsNotNull(result);
			Assert.That(result!.Result!.CustomerID, Is.EqualTo("ANATR"));
			Assert.That(result.Result!.CompanyName, Is.EqualTo("Ana Trujillo"));
		}

		[Test]
		public void CustomersService_GetByIdAsync_WithEmptyIds_ThrowsArgumentException_Test()
		{
			// arrange
			var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetCustomersRepository(dataSourceContext, sqlBuilder);

			var testObject = new CustomersService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act & assert
			Assert.ThrowsAsync<ArgumentException>(async () => await testObject.GetByIdAsync(Array.Empty<string>()));
		}

		[Test]
		public async Task CustomersService_InsertAsync_MapsDtoToEntity_AndReturnsResult_Test()
		{
			// arrange
			var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetCustomersRepository(dataSourceContext, sqlBuilder);

			fakeConnection.NextScalarResult = "NEWCO";
			var table = new DataTable();
			table.Columns.Add("Id", typeof(string));
			table.Rows.Add("NEWCO");
			fakeConnection.NextQueryResult = table;

			var testObject = new CustomersService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			var dto = new CustomersDto { CustomerID = "NEWCO", CompanyName = "New Company" };

			// act
			var result = await testObject.InsertAsync<string>(dto);

			// assert
			Assert.IsNotNull(result);
			Assert.That(result!.Result, Is.EqualTo("NEWCO"));

			var executedCommand = fakeConnection.ExecutedCommands[0];
			Assert.That(executedCommand.CommandText, Does.Contain("INSERT"));
			Assert.That(executedCommand.Parameters["CompanyName"], Is.EqualTo("New Company"));
		}

		[Test]
		public void CustomersService_InsertAsync_WithNullModel_ThrowsArgumentException_Test()
		{
			// arrange
			var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetCustomersRepository(dataSourceContext, sqlBuilder);

			var testObject = new CustomersService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act & assert
			Assert.ThrowsAsync<ArgumentException>(async () => await testObject.InsertAsync<string>(null!));
		}

		[Test]
		public async Task CustomersService_UpdateAsync_MapsDtoToEntity_AndReturnsSuccess_Test()
		{
			// arrange
			var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetCustomersRepository(dataSourceContext, sqlBuilder);

			fakeConnection.NextNonQueryResult = 1;

			var testObject = new CustomersService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			var dto = new CustomersDto { CustomerID = "ALFKI", CompanyName = "Updated Name" };

			// act
			var result = await testObject.UpdateAsync(dto);

			// assert
			Assert.IsTrue(result.Result);

			var executedCommand = fakeConnection.ExecutedCommands[0];
			Assert.That(executedCommand.CommandText, Does.Contain("UPDATE"));
			Assert.That(executedCommand.Parameters["CompanyName"], Is.EqualTo("Updated Name"));
		}

		[Test]
		public async Task CustomersService_DeleteAsync_ReturnsSuccess_Test()
		{
			// arrange
			var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetCustomersRepository(dataSourceContext, sqlBuilder);

			fakeConnection.NextNonQueryResult = 1;

			var testObject = new CustomersService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act
			var result = await testObject.DeleteAsync(new[] { "ALFKI" });

			// assert
			Assert.IsTrue(result.Result);
		}

		[Test]
		public async Task CustomersService_DeleteAsync_WhenRepositoryFails_ReturnsErrorMessage_Test()
		{
			// arrange
			var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetCustomersRepository(dataSourceContext, sqlBuilder);

			fakeConnection.NextNonQueryResult = 0;

			var testObject = new CustomersService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act
			var result = await testObject.DeleteAsync(new[] { "NOPE" });

			// assert
			Assert.IsFalse(result.Result);
			Assert.IsTrue(result.IsError);
			Assert.That(result.ErrorMessage, Does.Contain("Zero rows were deleted"));
		}
	}
}

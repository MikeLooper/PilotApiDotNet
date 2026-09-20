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
	/// Integration tests that exercise the real <see cref="SuppliersService"/>, real <see cref="DataMapperHandler"/>,
	/// and the real <see cref="PilotApi.Repositories.Repositories.SuppliersRepository"/> (built via
	/// <see cref="IntegrationTestDoublesUtilities.GetSuppliersRepository"/>) all wired together. Only the raw ADO.NET
	/// connection (<see cref="FakeDbConnection"/>) is faked, since no live database server is available in this environment.
	/// </summary>
	/// <remarks>
	/// Each test builds its own local, disposable set of dependencies (via <see cref="IntegrationTestDoublesUtilities"/>)
	/// rather than sharing state through class-level fields, to avoid any risk of cross-test data corruption.
	/// </remarks>
	[TestFixture]
	public class SuppliersServiceIntegrationTests
	{
		[Test]
		public async Task SuppliersService_GetAllAsync_ReturnsMappedDtos_Test()
		{
			// arrange
			var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetSuppliersRepository(dataSourceContext, sqlBuilder);

			var table = new DataTable();
			table.Columns.Add(nameof(SuppliersEntity.SupplierID), typeof(int));
			table.Columns.Add(nameof(SuppliersEntity.CompanyName), typeof(string));
			table.Rows.Add(1, "Exotic Liquids");
			fakeConnection.NextQueryResult = table;

			var testObject = new SuppliersService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act
			var result = await testObject.GetAllAsync();

			// assert
			Assert.That(result!.Result!.Count, Is.EqualTo(1));
			Assert.That(result.Result![0].CompanyName, Is.EqualTo("Exotic Liquids"));
		}

		[Test]
		public async Task SuppliersService_GetByIdAsync_ReturnsMappedDto_Test()
		{
			// arrange
			var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetSuppliersRepository(dataSourceContext, sqlBuilder);

			var table = new DataTable();
			table.Columns.Add(nameof(SuppliersEntity.SupplierID), typeof(int));
			table.Columns.Add(nameof(SuppliersEntity.CompanyName), typeof(string));
			table.Rows.Add(2, "New Orleans Cajun Delights");
			fakeConnection.NextQueryResult = table;

			var testObject = new SuppliersService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act
			var result = await testObject.GetByIdAsync(new[] { 2 });

			// assert
			Assert.That(result!.Result!.SupplierID, Is.EqualTo(2));
			Assert.That(result.Result!.CompanyName, Is.EqualTo("New Orleans Cajun Delights"));
		}

		[Test]
		public void SuppliersService_GetByIdAsync_WithEmptyIds_ThrowsArgumentException_Test()
		{
			// arrange
			var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetSuppliersRepository(dataSourceContext, sqlBuilder);

			var testObject = new SuppliersService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act & assert
			Assert.ThrowsAsync<ArgumentException>(async () => await testObject.GetByIdAsync(Array.Empty<int>()));
		}

		[Test]
		public async Task SuppliersService_InsertAsync_MapsDtoToEntity_AndReturnsResult_Test()
		{
			// arrange
			var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetSuppliersRepository(dataSourceContext, sqlBuilder);

			var table = new DataTable();
			table.Columns.Add("Id", typeof(int));
			table.Rows.Add(30);
			fakeConnection.NextQueryResult = table;

			var testObject = new SuppliersService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			var dto = new SuppliersDto { CompanyName = "Grandma Kelly's Homestead" };

			// act
			var result = await testObject.InsertAsync<int>(dto);

			// assert
			Assert.That(result!.Result, Is.EqualTo(30));

			var executedCommand = fakeConnection.ExecutedCommands[0];
			Assert.That(executedCommand.CommandText, Does.Contain("INSERT"));
			Assert.That(executedCommand.Parameters["CompanyName"], Is.EqualTo("Grandma Kelly's Homestead"));
		}

		[Test]
		public void SuppliersService_InsertAsync_WithNullModel_ThrowsArgumentException_Test()
		{
			// arrange
			var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetSuppliersRepository(dataSourceContext, sqlBuilder);

			var testObject = new SuppliersService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act & assert
			Assert.ThrowsAsync<ArgumentException>(async () => await testObject.InsertAsync<int>(null!));
		}

		[Test]
		public async Task SuppliersService_UpdateAsync_MapsDtoToEntity_AndReturnsSuccess_Test()
		{
			// arrange
			var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetSuppliersRepository(dataSourceContext, sqlBuilder);

			fakeConnection.NextNonQueryResult = 1;

			var testObject = new SuppliersService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			var dto = new SuppliersDto { SupplierID = 3, CompanyName = "Tokyo Traders" };

			// act
			var result = await testObject.UpdateAsync(dto);

			// assert
			Assert.IsTrue(result.Result);

			var executedCommand = fakeConnection.ExecutedCommands[0];
			Assert.That(executedCommand.Parameters["CompanyName"], Is.EqualTo("Tokyo Traders"));
		}

		[Test]
		public async Task SuppliersService_DeleteAsync_ReturnsSuccess_Test()
		{
			// arrange
			var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetSuppliersRepository(dataSourceContext, sqlBuilder);

			fakeConnection.NextNonQueryResult = 1;

			var testObject = new SuppliersService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act
			var result = await testObject.DeleteAsync(new[] { 3 });

			// assert
			Assert.IsTrue(result.Result);
		}

		[Test]
		public void SuppliersService_DeleteAsync_WithEmptyIds_ThrowsArgumentException_Test()
		{
			// arrange
			var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetSuppliersRepository(dataSourceContext, sqlBuilder);

			var testObject = new SuppliersService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act & assert
			Assert.ThrowsAsync<ArgumentException>(async () => await testObject.DeleteAsync(Array.Empty<int>()));
		}
	}
}

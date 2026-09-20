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
	/// Integration tests that exercise the real <see cref="ProductsService"/>, real <see cref="DataMapperHandler"/>,
	/// and the real <see cref="PilotApi.Repositories.Repositories.ProductsRepository"/> (built via
	/// <see cref="IntegrationTestDoublesUtilities.GetProductsRepository"/>) all wired together. Only the raw ADO.NET
	/// connection (<see cref="FakeDbConnection"/>) is faked, since no live database server is available in this environment.
	/// </summary>
	/// <remarks>
	/// Each test builds its own local, disposable set of dependencies (via <see cref="IntegrationTestDoublesUtilities"/>)
	/// rather than sharing state through class-level fields, to avoid any risk of cross-test data corruption.
	/// </remarks>
	[TestFixture]
	public class ProductsServiceIntegrationTests
	{
		[Test]
		public async Task ProductsService_GetAllAsync_ReturnsMappedDtos_Test()
		{
			// arrange
			var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetProductsRepository(dataSourceContext, sqlBuilder);

			var table = new DataTable();
			table.Columns.Add(nameof(ProductsEntity.ProductID), typeof(int));
			table.Columns.Add(nameof(ProductsEntity.ProductName), typeof(string));
			table.Columns.Add(nameof(ProductsEntity.UnitPrice), typeof(decimal));
			table.Rows.Add(1, "Chai", 18m);
			fakeConnection.NextQueryResult = table;

			var testObject = new ProductsService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act
			var result = await testObject.GetAllAsync();

			// assert
			Assert.That(result!.Result!.Count, Is.EqualTo(1));
			Assert.That(result.Result![0].ProductName, Is.EqualTo("Chai"));
		}

		[Test]
		public async Task ProductsService_GetByIdAsync_ReturnsMappedDto_Test()
		{
			// arrange
			var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetProductsRepository(dataSourceContext, sqlBuilder);

			var table = new DataTable();
			table.Columns.Add(nameof(ProductsEntity.ProductID), typeof(int));
			table.Columns.Add(nameof(ProductsEntity.ProductName), typeof(string));
			table.Rows.Add(2, "Chang");
			fakeConnection.NextQueryResult = table;

			var testObject = new ProductsService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act
			var result = await testObject.GetByIdAsync(new[] { 2 });

			// assert
			Assert.That(result!.Result!.ProductID, Is.EqualTo(2));
			Assert.That(result.Result!.ProductName, Is.EqualTo("Chang"));
		}

		[Test]
		public void ProductsService_GetByIdAsync_WithEmptyIds_ThrowsArgumentException_Test()
		{
			// arrange
			var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetProductsRepository(dataSourceContext, sqlBuilder);

			var testObject = new ProductsService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act & assert
			Assert.ThrowsAsync<ArgumentException>(async () => await testObject.GetByIdAsync(Array.Empty<int>()));
		}

		[Test]
		public async Task ProductsService_InsertAsync_MapsDtoToEntity_AndReturnsResult_Test()
		{
			// arrange
			var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetProductsRepository(dataSourceContext, sqlBuilder);

			var table = new DataTable();
			table.Columns.Add("Id", typeof(int));
			table.Rows.Add(80);
			fakeConnection.NextQueryResult = table;

			var testObject = new ProductsService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			var dto = new ProductsDto { ProductName = "Ikura", UnitPrice = 31 };

			// act
			var result = await testObject.InsertAsync<int>(dto);

			// assert
			Assert.That(result!.Result, Is.EqualTo(80));

			var executedCommand = fakeConnection.ExecutedCommands[0];
			Assert.That(executedCommand.CommandText, Does.Contain("INSERT"));
			Assert.That(executedCommand.Parameters["ProductName"], Is.EqualTo("Ikura"));
		}

		[Test]
		public void ProductsService_InsertAsync_WithNullModel_ThrowsArgumentException_Test()
		{
			// arrange
			var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetProductsRepository(dataSourceContext, sqlBuilder);

			var testObject = new ProductsService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act & assert
			Assert.ThrowsAsync<ArgumentException>(async () => await testObject.InsertAsync<int>(null!));
		}

		[Test]
		public async Task ProductsService_UpdateAsync_MapsDtoToEntity_AndReturnsSuccess_Test()
		{
			// arrange
			var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetProductsRepository(dataSourceContext, sqlBuilder);

			fakeConnection.NextNonQueryResult = 1;

			var testObject = new ProductsService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			var dto = new ProductsDto { ProductID = 3, ProductName = "Aniseed Syrup", Discontinued = true };

			// act
			var result = await testObject.UpdateAsync(dto);

			// assert
			Assert.IsTrue(result.Result);

			var executedCommand = fakeConnection.ExecutedCommands[0];
			Assert.That(executedCommand.Parameters["Discontinued"], Is.EqualTo(true));
		}

		[Test]
		public async Task ProductsService_DeleteAsync_ReturnsSuccess_Test()
		{
			// arrange
			var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetProductsRepository(dataSourceContext, sqlBuilder);

			fakeConnection.NextNonQueryResult = 1;

			var testObject = new ProductsService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act
			var result = await testObject.DeleteAsync(new[] { 3 });

			// assert
			Assert.IsTrue(result.Result);
		}

		[Test]
		public void ProductsService_DeleteAsync_WithEmptyIds_ThrowsArgumentException_Test()
		{
			// arrange
			var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetProductsRepository(dataSourceContext, sqlBuilder);

			var testObject = new ProductsService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act & assert
			Assert.ThrowsAsync<ArgumentException>(async () => await testObject.DeleteAsync(Array.Empty<int>()));
		}
	}
}

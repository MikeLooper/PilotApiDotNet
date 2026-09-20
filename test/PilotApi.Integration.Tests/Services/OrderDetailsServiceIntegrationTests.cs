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
	/// Integration tests that exercise the real <see cref="OrderDetailsService"/>, real <see cref="DataMapperHandler"/>,
	/// and the real <see cref="PilotApi.Repositories.Repositories.OrderDetailsRepository"/> (built via
	/// <see cref="IntegrationTestDoublesUtilities.GetOrderDetailsRepository"/>) all wired together. Only the raw ADO.NET
	/// connection (<see cref="FakeDbConnection"/>) is faked, since no live database server is available in this environment.
	/// </summary>
	/// <remarks>
	/// Each test builds its own local, disposable set of dependencies (via <see cref="IntegrationTestDoublesUtilities"/>)
	/// rather than sharing state through class-level fields, to avoid any risk of cross-test data corruption.
	/// The composite key is ordered (ProductID, OrderID), matching <see cref="PilotApi.Repositories.Repositories.OrderDetailsRepository"/>'s
	/// <c>KeyColumnNames</c> definition.
	/// </remarks>
	[TestFixture]
	public class OrderDetailsServiceIntegrationTests
	{
		[Test]
		public async Task OrderDetailsService_GetAllAsync_ReturnsMappedDtos_Test()
		{
			// arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetOrderDetailsRepository(dataSourceContext, sqlBuilder);

			var table = new DataTable();
			table.Columns.Add(nameof(OrderDetailsEntity.ProductID), typeof(int));
			table.Columns.Add(nameof(OrderDetailsEntity.OrderID), typeof(int));
			table.Columns.Add(nameof(OrderDetailsEntity.UnitPrice), typeof(decimal));
			table.Columns.Add(nameof(OrderDetailsEntity.Quantity), typeof(short));
			table.Rows.Add(11, 10248, 14m, (short)12);
			fakeConnection.NextQueryResult = table;

			var testObject = new OrderDetailsService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act
			var result = await testObject.GetAllAsync();

			// assert
			Assert.That(result!.Result!.Count, Is.EqualTo(1));
			Assert.That(result.Result![0].OrderID, Is.EqualTo(10248));
			Assert.That(result.Result![0].ProductID, Is.EqualTo(11));
		}

		[Test]
		public async Task OrderDetailsService_GetByIdAsync_WithCompositeKey_ReturnsMappedDto_Test()
		{
			// arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetOrderDetailsRepository(dataSourceContext, sqlBuilder);

			var table = new DataTable();
			table.Columns.Add(nameof(OrderDetailsEntity.ProductID), typeof(int));
			table.Columns.Add(nameof(OrderDetailsEntity.OrderID), typeof(int));
			table.Columns.Add(nameof(OrderDetailsEntity.Quantity), typeof(short));
			table.Rows.Add(42, 10249, (short)9);
			fakeConnection.NextQueryResult = table;

			var testObject = new OrderDetailsService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act
			var result = await testObject.GetByIdAsync(new[] { 42, 10249 });

			// assert
			Assert.That(result!.Result!.OrderID, Is.EqualTo(10249));
			Assert.That(result.Result!.ProductID, Is.EqualTo(42));
		}

		[Test]
		public void OrderDetailsService_GetByIdAsync_WithEmptyIds_ThrowsArgumentException_Test()
		{
			// arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetOrderDetailsRepository(dataSourceContext, sqlBuilder);

			var testObject = new OrderDetailsService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act & assert
			Assert.ThrowsAsync<ArgumentException>(async () => await testObject.GetByIdAsync(Array.Empty<int>()));
		}

		[Test]
		public async Task OrderDetailsService_InsertAsync_MapsDtoToEntity_AndReturnsResult_Test()
		{
			// arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetOrderDetailsRepository(dataSourceContext, sqlBuilder);

			var table = new DataTable();
			table.Columns.Add("Id", typeof(int));
			table.Rows.Add(1);
			fakeConnection.NextQueryResult = table;

			var testObject = new OrderDetailsService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			var dto = new OrderDetailsDto { OrderID = 10250, ProductID = 5, Quantity = 3, UnitPrice = 21.5m, Discount = 0.1f };

			// act
			var result = await testObject.InsertAsync<int>(dto);

			// assert
			Assert.That(result!.Result, Is.EqualTo(1));

			var executedCommand = fakeConnection.ExecutedCommands[0];
			Assert.That(executedCommand.CommandText, Does.Contain("INSERT"));
			Assert.That(executedCommand.Parameters["OrderID"], Is.EqualTo(10250));
			Assert.That(executedCommand.Parameters["ProductID"], Is.EqualTo(5));
		}

		[Test]
		public void OrderDetailsService_InsertAsync_WithNullModel_ThrowsArgumentException_Test()
		{
			// arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetOrderDetailsRepository(dataSourceContext, sqlBuilder);

			var testObject = new OrderDetailsService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act & assert
			Assert.ThrowsAsync<ArgumentException>(async () => await testObject.InsertAsync<int>(null!));
		}

		[Test]
		public async Task OrderDetailsService_UpdateAsync_MapsDtoToEntity_AndReturnsSuccess_Test()
		{
			// arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetOrderDetailsRepository(dataSourceContext, sqlBuilder);

			fakeConnection.NextNonQueryResult = 1;

			var testObject = new OrderDetailsService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			var dto = new OrderDetailsDto { OrderID = 10251, ProductID = 22, Quantity = 6 };

			// act
			var result = await testObject.UpdateAsync(dto);

			// assert
			Assert.IsTrue(result.Result);

			var executedCommand = fakeConnection.ExecutedCommands[0];
			Assert.That(executedCommand.Parameters["Quantity"], Is.EqualTo((short)6));
		}

		[Test]
		public async Task OrderDetailsService_DeleteAsync_WithCompositeKey_ReturnsSuccess_Test()
		{
			// arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetOrderDetailsRepository(dataSourceContext, sqlBuilder);

			fakeConnection.NextNonQueryResult = 1;

			var testObject = new OrderDetailsService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act
			var result = await testObject.DeleteAsync(new[] { 22, 10251 });

			// assert
			Assert.IsTrue(result.Result);
		}
	}
}

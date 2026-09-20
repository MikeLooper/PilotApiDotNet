using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using PilotApi.Domain.Models.Dto;
using PilotApi.Domain.Models.Responses;
using PilotApi.Integration.Tests.Testing.Doubles;
using PilotApi.Integration.Tests.Testing.Utilities;
using PilotApi.Repositories.Models.Entities;
using PilotApi.Services.Handlers;
using PilotApi.Services.Services;
using PilotApi.Web.Controllers.V1;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace PilotApi.Integration.Tests.Controllers
{
	/// <summary>
	/// Integration tests that exercise the real <see cref="OrderDetailsController"/>, real <see cref="OrderDetailsService"/>,
	/// real <see cref="DataMapperHandler"/>, and the real <see cref="PilotApi.Repositories.Repositories.OrderDetailsRepository"/>
	/// (built via <see cref="IntegrationTestDoublesUtilities.GetOrderDetailsService"/>) all wired together. Only the raw
	/// ADO.NET connection (<see cref="FakeDbConnection"/>) is faked, since no live database server is available in
	/// this environment.
	/// </summary>
	/// <remarks>
	/// Each test builds its own local, disposable set of dependencies (via <see cref="IntegrationTestDoublesUtilities"/>)
	/// rather than sharing state through class-level fields, to avoid any risk of cross-test data corruption.
	/// </remarks>
	[TestFixture]
	public class OrderDetailsControllerIntegrationTests
	{
		[Test]
		public async Task Add_WhenRepositoryInsertSucceeds_ReturnsCreatedAtActionWithNewId_Test()
		{
			// Arrange
			using var fakeConnection = new FakeDbConnection();
			var service = IntegrationTestDoublesUtilities.GetOrderDetailsService(fakeConnection);
			using var controller = new OrderDetailsController(service);

			var table = new DataTable();
			table.Columns.Add("Id", typeof(int));
			table.Rows.Add(1);
			fakeConnection.NextQueryResult = table;

			var model = new OrderDetailsDto { ProductID = 1, OrderID = 10248, UnitPrice = 14.0m, Quantity = 12 };

			// Act
			var actionResult = await controller.Add(model, CancellationToken.None);

			// Assert
			var createdResult = actionResult as CreatedAtActionResult;
			Assert.That(createdResult, Is.Not.Null);
			Assert.That(createdResult!.StatusCode, Is.EqualTo(201));
			var addResponse = createdResult.Value as AddResponseInt;
			Assert.That(addResponse, Is.Not.Null);
			Assert.That(addResponse!.Id, Is.EqualTo(1));
		}

		[Test]
		public async Task Delete_WhenRepositoryReturnsError_ReturnsBadRequestWithWarningHeader_Test()
		{
			// Arrange
			using var fakeConnection = new FakeDbConnection();
			var service = IntegrationTestDoublesUtilities.GetOrderDetailsService(fakeConnection);
			using var controller = new OrderDetailsController(service);

			fakeConnection.NextNonQueryResult = 0;

			var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
			controller.ControllerContext = new ControllerContext
			{
				HttpContext = httpContext
			};

			// Act
			var actionResult = await controller.Delete(5, 10249, CancellationToken.None);

			// Assert
			Assert.That(actionResult, Is.TypeOf<BadRequestResult>());
			Assert.That(httpContext.Response.Headers["Warning"].ToString(), Does.Contain("Zero rows were deleted"));
		}

		[Test]
		public async Task GetAll_WhenRepositoryReturnsEntities_ReturnsOkWithMappedDtos_Test()
		{
			// Arrange
			using var fakeConnection = new FakeDbConnection();
			var service = IntegrationTestDoublesUtilities.GetOrderDetailsService(fakeConnection);
			using var controller = new OrderDetailsController(service);

			var table = new DataTable();
			table.Columns.Add(nameof(OrderDetailsEntity.ProductID), typeof(int));
			table.Columns.Add(nameof(OrderDetailsEntity.OrderID), typeof(int));
			table.Columns.Add(nameof(OrderDetailsEntity.UnitPrice), typeof(decimal));
			table.Rows.Add(1, 10248, 14.0m);
			table.Rows.Add(2, 10248, 9.8m);
			fakeConnection.NextQueryResult = table;

			// Act
			var actionResult = await controller.GetAll(cancellationToken: CancellationToken.None);

			// Assert
			var okResult = actionResult as OkObjectResult;
			Assert.That(okResult, Is.Not.Null);
			var dtos = okResult!.Value as IReadOnlyList<OrderDetailsDto>;
			Assert.That(dtos, Is.Not.Null);
			Assert.That(dtos!, Has.Count.EqualTo(2));
			Assert.That(dtos![0].UnitPrice, Is.EqualTo(14.0m));
		}

		[Test]
		public async Task GetById_WhenRepositoryReturnsEntity_ReturnsOkWithMappedDto_Test()
		{
			// Arrange
			using var fakeConnection = new FakeDbConnection();
			var service = IntegrationTestDoublesUtilities.GetOrderDetailsService(fakeConnection);
			using var controller = new OrderDetailsController(service);

			var table = new DataTable();
			table.Columns.Add(nameof(OrderDetailsEntity.ProductID), typeof(int));
			table.Columns.Add(nameof(OrderDetailsEntity.OrderID), typeof(int));
			table.Columns.Add(nameof(OrderDetailsEntity.UnitPrice), typeof(decimal));
			table.Rows.Add(7, 10250, 21.0m);
			fakeConnection.NextQueryResult = table;

			// Act
			var actionResult = await controller.GetById(7, 10250, CancellationToken.None);

			// Assert
			var okResult = actionResult as OkObjectResult;
			Assert.That(okResult, Is.Not.Null);
			var dto = okResult!.Value as OrderDetailsDto;
			Assert.That(dto, Is.Not.Null);
			Assert.That(dto!.UnitPrice, Is.EqualTo(21.0m));
		}

		[Test]
		public async Task GetById_WhenRepositoryReturnsNull_ReturnsNotFound_Test()
		{
			// Arrange
			using var fakeConnection = new FakeDbConnection();
			var service = IntegrationTestDoublesUtilities.GetOrderDetailsService(fakeConnection);
			using var controller = new OrderDetailsController(service);

			fakeConnection.NextQueryResult = new DataTable();

			// Act
			var actionResult = await controller.GetById(99, 99, CancellationToken.None);

			// Assert
			Assert.That(actionResult, Is.TypeOf<NotFoundResult>());
		}
	}
}

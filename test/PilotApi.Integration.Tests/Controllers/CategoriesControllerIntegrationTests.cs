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
	/// Integration tests that exercise the real <see cref="CategoriesController"/>, real <see cref="CategoriesService"/>,
	/// real <see cref="DataMapperHandler"/>, and the real <see cref="PilotApi.Repositories.Repositories.CategoriesRepository"/>
	/// (built via <see cref="IntegrationTestDoublesUtilities.GetCategoriesService"/>) all wired together. Only the raw
	/// ADO.NET connection (<see cref="FakeDbConnection"/>) is faked, since no live database server is available in
	/// this environment. This verifies that HTTP requests flow correctly all the way from the controller, through
	/// the service and mapping layers, down to the real SQL generation and Dapper execution pipeline in the repository.
	/// </summary>
	/// <remarks>
	/// Each test builds its own local, disposable set of dependencies (via <see cref="IntegrationTestDoublesUtilities"/>)
	/// rather than sharing state through class-level fields, to avoid any risk of cross-test data corruption.
	/// </remarks>
	[TestFixture]
	public class CategoriesControllerIntegrationTests
	{
		[Test]
		public async Task Add_WhenRepositoryInsertSucceeds_ReturnsCreatedAtActionWithNewId_Test()
		{
			// Arrange
			using var fakeConnection = new FakeDbConnection();
			var service = IntegrationTestDoublesUtilities.GetCategoriesService(fakeConnection);
			using var controller = new CategoriesController(service);

			var table = new DataTable();
			table.Columns.Add("Id", typeof(int));
			table.Rows.Add(42);
			fakeConnection.NextQueryResult = table;

			var model = new CategoriesDto { CategoryName = "Produce" };

			// Act
			var actionResult = await controller.Add(model, CancellationToken.None);

			// Assert
			var createdResult = actionResult as CreatedAtActionResult;
			Assert.That(createdResult, Is.Not.Null);
			Assert.That(createdResult!.StatusCode, Is.EqualTo(201));
			var addResponse = createdResult.Value as AddResponseInt;
			Assert.That(addResponse, Is.Not.Null);
			Assert.That(addResponse!.Id, Is.EqualTo(42));
		}

		[Test]
		public async Task Delete_WhenRepositoryReturnsError_ReturnsBadRequestWithWarningHeader_Test()
		{
			// Arrange
			using var fakeConnection = new FakeDbConnection();
			var service = IntegrationTestDoublesUtilities.GetCategoriesService(fakeConnection);
			using var controller = new CategoriesController(service);

			fakeConnection.NextNonQueryResult = 0;

			var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
			controller.ControllerContext = new ControllerContext
			{
				HttpContext = httpContext
			};

			// Act
			var actionResult = await controller.Delete(5, CancellationToken.None);

			// Assert
			Assert.That(actionResult, Is.TypeOf<BadRequestResult>());
			Assert.That(httpContext.Response.Headers["Warning"].ToString(), Does.Contain("Zero rows were deleted"));
		}

		[Test]
		public async Task GetAll_WhenRepositoryReturnsEntities_ReturnsOkWithMappedDtos_Test()
		{
			// Arrange
			using var fakeConnection = new FakeDbConnection();
			var service = IntegrationTestDoublesUtilities.GetCategoriesService(fakeConnection);
			using var controller = new CategoriesController(service);

			var table = new DataTable();
			table.Columns.Add(nameof(CategoriesEntity.CategoryID), typeof(int));
			table.Columns.Add(nameof(CategoriesEntity.CategoryName), typeof(string));
			table.Rows.Add(1, "Beverages");
			table.Rows.Add(2, "Condiments");
			fakeConnection.NextQueryResult = table;

			// Act
			var actionResult = await controller.GetAll(cancellationToken: CancellationToken.None);

			// Assert
			var okResult = actionResult as OkObjectResult;
			Assert.That(okResult, Is.Not.Null);
			var dtos = okResult!.Value as IReadOnlyList<CategoriesDto>;
			Assert.That(dtos, Is.Not.Null);
			Assert.That(dtos!, Has.Count.EqualTo(2));
			Assert.That(dtos![0].CategoryName, Is.EqualTo("Beverages"));
		}

		[Test]
		public async Task GetById_WhenRepositoryReturnsEntity_ReturnsOkWithMappedDto_Test()
		{
			// Arrange
			using var fakeConnection = new FakeDbConnection();
			var service = IntegrationTestDoublesUtilities.GetCategoriesService(fakeConnection);
			using var controller = new CategoriesController(service);

			var table = new DataTable();
			table.Columns.Add(nameof(CategoriesEntity.CategoryID), typeof(int));
			table.Columns.Add(nameof(CategoriesEntity.CategoryName), typeof(string));
			table.Rows.Add(7, "Seafood");
			fakeConnection.NextQueryResult = table;

			// Act
			var actionResult = await controller.GetById(7, CancellationToken.None);

			// Assert
			var okResult = actionResult as OkObjectResult;
			Assert.That(okResult, Is.Not.Null);
			var dto = okResult!.Value as CategoriesDto;
			Assert.That(dto, Is.Not.Null);
			Assert.That(dto!.CategoryName, Is.EqualTo("Seafood"));
		}

		[Test]
		public async Task GetById_WhenRepositoryReturnsNull_ReturnsNotFound_Test()
		{
			// Arrange
			using var fakeConnection = new FakeDbConnection();
			var service = IntegrationTestDoublesUtilities.GetCategoriesService(fakeConnection);
			using var controller = new CategoriesController(service);

			fakeConnection.NextQueryResult = new DataTable();

			// Act
			var actionResult = await controller.GetById(99, CancellationToken.None);

			// Assert
			Assert.That(actionResult, Is.TypeOf<NotFoundResult>());
		}
	}
}

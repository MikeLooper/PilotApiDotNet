using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using PilotApi.Domain.Contracts.Services;
using PilotApi.Web.Controllers.V1;
using PilotApi.Web.Tests.TestDoubles;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace PilotApi.Web.Tests.Controllers
{
	[TestFixture]
	public class ProductsControllerTests
	{
		[Test]
		public void ProductsController_Constructor_WithService_SetsProtectedServiceProperty_Test()
		{
			var service = ProxyFactory.Create<IProductsService>();
			var controller = new ProductsController(service);

			var serviceProperty = typeof(ProductsController)
				.GetProperty("Service", BindingFlags.Instance | BindingFlags.NonPublic);
			var serviceValue = serviceProperty?.GetValue(controller);

			Assert.That(serviceProperty, Is.Not.Null);
			Assert.That(serviceValue, Is.SameAs(service));
		}

		[Test]
		public void ProductsController_ApiVersion_SetValue_GetReturnsSameValue_Test()
		{
			var service = ProxyFactory.Create<IProductsService>();
			var controller = new ProductsController(service)
			{
				ApiVersion = "1.0"
			};

			Assert.That(controller.ApiVersion, Is.EqualTo("1.0"));
		}

		[Test]
		public async Task ProductsController_GetAll_WhenResultIsNull_ReturnsNoContent_Test()
		{
			var service = ProxyFactory.CreateNullResult<IProductsService>();
			var controller = new ProductsController(service);

			var actionResult = await controller.GetAll();

			Assert.That(actionResult, Is.TypeOf<NoContentResult>());
		}

		[Test]
		public async Task ProductsController_GetById_WhenResultIsNull_ReturnsNoContent_Test()
		{
			var service = ProxyFactory.CreateNullResult<IProductsService>();
			var controller = new ProductsController(service);

			var actionResult = await controller.GetById(1, CancellationToken.None);

			Assert.That(actionResult, Is.TypeOf<NoContentResult>());
		}
	}
}


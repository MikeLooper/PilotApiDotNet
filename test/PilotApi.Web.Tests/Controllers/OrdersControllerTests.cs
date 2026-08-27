using NUnit.Framework;
using PilotApi.Domain.Contracts.Services;
using PilotApi.Web.Controllers.V1;
using PilotApi.Web.Tests.TestDoubles;
using System.Reflection;

namespace PilotApi.Web.Tests.Controllers
{
	[TestFixture]
	public class OrdersControllerTests
	{
		[Test]
		public void OrdersController_Constructor_WithService_SetsProtectedServiceProperty_Test()
		{
			var service = ProxyFactory.Create<IOrdersService>();
			var controller = new OrdersController(service);

			var serviceProperty = typeof(OrdersController)
				.GetProperty("Service", BindingFlags.Instance | BindingFlags.NonPublic);
			var serviceValue = serviceProperty?.GetValue(controller);

			Assert.That(serviceProperty, Is.Not.Null);
			Assert.That(serviceValue, Is.SameAs(service));
		}

		[Test]
		public void OrdersController_ApiVersion_SetValue_GetReturnsSameValue_Test()
		{
			var service = ProxyFactory.Create<IOrdersService>();
			var controller = new OrdersController(service)
			{
				ApiVersion = "1.0"
			};

			Assert.That(controller.ApiVersion, Is.EqualTo("1.0"));
		}
	}
}


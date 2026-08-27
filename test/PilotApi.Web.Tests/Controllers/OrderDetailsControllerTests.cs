using NUnit.Framework;
using PilotApi.Domain.Contracts.Services;
using PilotApi.Web.Controllers.V1;
using PilotApi.Web.Tests.TestDoubles;
using System.Reflection;

namespace PilotApi.Web.Tests.Controllers
{
	[TestFixture]
	public class OrderDetailsControllerTests
	{
		[Test]
		public void OrderDetailsController_Constructor_WithService_SetsProtectedServiceProperty_Test()
		{
			var service = ProxyFactory.Create<IOrderDetailsService>();
			var controller = new OrderDetailsController(service);

			var serviceProperty = typeof(OrderDetailsController)
				.GetProperty("Service", BindingFlags.Instance | BindingFlags.NonPublic);
			var serviceValue = serviceProperty?.GetValue(controller);

			Assert.That(serviceProperty, Is.Not.Null);
			Assert.That(serviceValue, Is.SameAs(service));
		}

		[Test]
		public void OrderDetailsController_ApiVersion_SetValue_GetReturnsSameValue_Test()
		{
			var service = ProxyFactory.Create<IOrderDetailsService>();
			var controller = new OrderDetailsController(service)
			{
				ApiVersion = "1.0"
			};

			Assert.That(controller.ApiVersion, Is.EqualTo("1.0"));
		}
	}
}


using Microsoft.AspNetCore.Authorization;
using NUnit.Framework;
using PilotApi.Web.Controllers;

namespace PilotApi.Web.Tests.Controllers.Base
{
	[TestFixture]
	public class SimpleControllerBaseTests
	{
		[Test]
		public void SimpleControllerBase_ApiVersion_SetValue_GetReturnsSameValue_Test()
		{
			var controller = new SimpleControllerBase
			{
				ApiVersion = "1.0"
			};

			Assert.That(controller.ApiVersion, Is.EqualTo("1.0"));
		}

		[Test]
		public void SimpleControllerBase_ApiVersion_DefaultValue_IsNull_Test()
		{
			var controller = new SimpleControllerBase();

			Assert.That(controller.ApiVersion, Is.Null);
		}

		[Test]
		public void SimpleControllerBase_DoesNotHaveAllowAnonymousAttribute_Test()
		{
			// Arrange
			var type = typeof(SimpleControllerBase);

			// Act
			var hasAllowAnonymous = type.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).Length > 0;

			// Assert
			Assert.That(hasAllowAnonymous, Is.False);
		}
	}
}


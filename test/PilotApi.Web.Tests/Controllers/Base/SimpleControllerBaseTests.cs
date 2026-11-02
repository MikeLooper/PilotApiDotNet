using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using PilotApi.Web.Controllers;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace PilotApi.Web.Tests.Controllers.Base
{
	[TestFixture]
	public class SimpleControllerBaseTests
	{
		private class TestSimpleController : SimpleControllerBase
		{
		}

		[Test]
		public void SimpleControllerBase_ApiVersion_SetValue_GetReturnsSameValue_Test()
		{
			var controller = new TestSimpleController
			{
				ApiVersion = "1.0"
			};

			Assert.That(controller.ApiVersion, Is.EqualTo("1.0"));
		}

		[Test]
		public void SimpleControllerBase_CallerName_SetValue_GetReturnsSameValue_Test()
		{
			var controller = new TestSimpleController
			{
				CallerName = "PilotAngularUi"
			};

			Assert.That(controller.CallerName, Is.EqualTo("PilotAngularUi"));
		}

		[Test]
		public void SimpleControllerBase_CorrelationId_SetValue_GetReturnsSameValue_Test()
		{
			var controller = new TestSimpleController
			{
				CorrelationId = "correlation-id"
			};

			Assert.That(controller.CorrelationId, Is.EqualTo("correlation-id"));
		}

		[Test]
		public void SimpleControllerBase_ApiVersion_DefaultValue_IsNull_Test()
		{
			var controller = new TestSimpleController();

			Assert.That(controller.ApiVersion, Is.Null);
		}

		[TestCase(nameof(SimpleControllerBase.ApiVersion), "X-Api-Version", 15)]
		[TestCase(nameof(SimpleControllerBase.CallerName), "X-Caller-Name", 100)]
		[TestCase(nameof(SimpleControllerBase.CorrelationId), "X-Correlation-Id", 40)]
		public void SimpleControllerBase_HeaderProperty_HasExpectedHeaderNameAndMaxLength_Test(string propertyName, string headerName, int maxLength)
		{
			// Arrange
			var property = typeof(SimpleControllerBase).GetProperty(propertyName);

			// Act
			var fromHeader = property?.GetCustomAttribute<FromHeaderAttribute>();
			var maxLengthAttribute = property?.GetCustomAttribute<MaxLengthAttribute>();

			// Assert
			Assert.That(fromHeader?.Name, Is.EqualTo(headerName));
			Assert.That(maxLengthAttribute?.Length, Is.EqualTo(maxLength));
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


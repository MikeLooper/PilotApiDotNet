using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using PilotApi.Shared.Api.Security;
using PilotApi.Shared.Constants;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PilotApi.Shared.Tests.Api.Security
{
	[TestFixture]
	public class HttpVerbRoleAuthorizationHandlerTests
	{
		private static AuthorizationHandlerContext BuildContext(string role, string method)
		{
			var identity = new ClaimsIdentity("TestAuthType");
			if (role != null)
			{
				identity.AddClaim(new Claim(ClaimTypes.Role, role));
			}

			var principal = new ClaimsPrincipal(identity);
			var httpContext = new DefaultHttpContext();
			httpContext.Request.Method = method;

			return new AuthorizationHandlerContext(
				new[] { new HttpVerbRoleRequirement() },
				principal,
				httpContext);
		}

		private static HttpVerbRoleAuthorizationHandler GetHandler()
		{
			return new HttpVerbRoleAuthorizationHandler(NullLogger<HttpVerbRoleAuthorizationHandler>.Instance);
		}

		[TestCase(RoleNames.ReadOnly, "GET", true)]
		[TestCase(RoleNames.ReadOnly, "POST", false)]
		[TestCase(RoleNames.ReadOnly, "PUT", false)]
		[TestCase(RoleNames.ReadOnly, "DELETE", false)]
		[TestCase(RoleNames.ReadWrite, "GET", true)]
		[TestCase(RoleNames.ReadWrite, "POST", true)]
		[TestCase(RoleNames.ReadWrite, "PUT", true)]
		[TestCase(RoleNames.ReadWrite, "DELETE", false)]
		[TestCase(RoleNames.Admin, "GET", true)]
		[TestCase(RoleNames.Admin, "POST", true)]
		[TestCase(RoleNames.Admin, "PUT", true)]
		[TestCase(RoleNames.Admin, "DELETE", true)]
		public async Task HttpVerbRoleAuthorizationHandler_HandleRequirementAsync_RoleVerbMatrix_Test(string role, string method, bool expectedSucceeded)
		{
			// Arrange
			var handler = GetHandler();
			var context = BuildContext(role, method);

			// Act
			await handler.HandleAsync(context);

			// Assert
			Assert.That(context.HasSucceeded, Is.EqualTo(expectedSucceeded));
		}

		[Test]
		public async Task HttpVerbRoleAuthorizationHandler_HandleRequirementAsync_WithNoRoleClaim_ShouldFail_Test()
		{
			// Arrange
			var handler = GetHandler();
			var context = BuildContext(null, "GET");

			// Act
			await handler.HandleAsync(context);

			// Assert
			Assert.That(context.HasSucceeded, Is.False);
			Assert.That(context.HasFailed, Is.True);
		}

		[Test]
		public async Task HttpVerbRoleAuthorizationHandler_HandleRequirementAsync_WithNonHttpContextResource_ShouldFail_Test()
		{
			// Arrange
			var handler = GetHandler();
			var identity = new ClaimsIdentity("TestAuthType");
			identity.AddClaim(new Claim(ClaimTypes.Role, RoleNames.Admin));
			var principal = new ClaimsPrincipal(identity);

			var context = new AuthorizationHandlerContext(
				new[] { new HttpVerbRoleRequirement() },
				principal,
				new object());

			// Act
			await handler.HandleAsync(context);

			// Assert
			Assert.That(context.HasSucceeded, Is.False);
			Assert.That(context.HasFailed, Is.True);
		}

		[Test]
		public async Task HttpVerbRoleAuthorizationHandler_HandleRequirementAsync_WithUnknownRole_ShouldFail_Test()
		{
			// Arrange
			var handler = GetHandler();
			var context = BuildContext("SomeUnmappedRole", "GET");

			// Act
			await handler.HandleAsync(context);

			// Assert
			Assert.That(context.HasSucceeded, Is.False);
		}
	}
}

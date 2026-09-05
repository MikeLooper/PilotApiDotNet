using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using PilotApi.Shared.Api.Security;
using PilotApi.Shared.Constants;
using PilotApi.Shared.Contracts.Configuration;
using System.Threading.Tasks;

namespace PilotApi.Shared.Tests.Api.Security
{
	[TestFixture]
	public class BypassOnInactiveAuthorizationMiddlewareResultHandlerTests
	{
		private static HttpContext BuildHttpContext()
		{
			var services = new ServiceCollection();
			services.AddLogging();
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.Authority = "http://localhost:55001/realms/test-realm";
					options.Audience = "test-client";
					options.RequireHttpsMetadata = false;
				});
			services.AddAuthorization();

			var serviceProvider = services.BuildServiceProvider();

			return new DefaultHttpContext
			{
				RequestServices = serviceProvider
			};
		}

		private static BypassOnInactiveAuthorizationMiddlewareResultHandler GetHandler(bool active)
		{
			var securityConfiguration = new Mock<ISecurityConfiguration>();
			securityConfiguration.SetupGet(s => s.Active).Returns(active);

			return new BypassOnInactiveAuthorizationMiddlewareResultHandler(
				securityConfiguration.Object,
				NullLogger<BypassOnInactiveAuthorizationMiddlewareResultHandler>.Instance);
		}

		[Test]
		public async Task BypassOnInactiveAuthorizationMiddlewareResultHandler_HandleAsync_WithActiveAndForbidden_ShouldNotInvokeNext_Test()
		{
			// Arrange
			var handler = GetHandler(active: true);
			var context = BuildHttpContext();
			var nextInvoked = false;
			RequestDelegate next = _ => { nextInvoked = true; return Task.CompletedTask; };
			var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
			var authorizeResult = PolicyAuthorizationResult.Forbid();

			// Act
			await handler.HandleAsync(next, context, policy, authorizeResult);

			// Assert
			Assert.That(nextInvoked, Is.False);
			Assert.That(context.Response.Headers.ContainsKey(SecurityConstants.WarningHeaderName), Is.False);
		}

		[Test]
		public async Task BypassOnInactiveAuthorizationMiddlewareResultHandler_HandleAsync_WithInactiveAndForbidden_ShouldInvokeNextAndSetWarningHeader_Test()
		{
			// Arrange
			var handler = GetHandler(active: false);
			var context = BuildHttpContext();
			var nextInvoked = false;
			RequestDelegate next = _ => { nextInvoked = true; return Task.CompletedTask; };
			var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

			var failureHandler = new Mock<IAuthorizationHandler>();
			var failure = AuthorizationFailure.Failed(new[]
			{
				new AuthorizationFailureReason(failureHandler.Object, "No role has been assigned to the current user.")
			});
			var authorizeResult = PolicyAuthorizationResult.Forbid(failure);

			// Act
			await handler.HandleAsync(next, context, policy, authorizeResult);

			// Assert
			Assert.That(nextInvoked, Is.True);
			Assert.That(context.Response.Headers[SecurityConstants.WarningHeaderName].ToString(), Does.Contain("No role has been assigned"));
		}

		[Test]
		public async Task BypassOnInactiveAuthorizationMiddlewareResultHandler_HandleAsync_WithInactiveAndChallengedWithNoStashedReason_ShouldUseGenericMessage_Test()
		{
			// Arrange
			var handler = GetHandler(active: false);
			var context = BuildHttpContext();
			var nextInvoked = false;
			RequestDelegate next = _ => { nextInvoked = true; return Task.CompletedTask; };
			var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
			var authorizeResult = PolicyAuthorizationResult.Challenge();

			// Act
			await handler.HandleAsync(next, context, policy, authorizeResult);

			// Assert
			Assert.That(nextInvoked, Is.True);
			Assert.That(context.Response.Headers[SecurityConstants.WarningHeaderName].ToString(), Is.EqualTo("Missing or invalid bearer token."));
		}

		[Test]
		public async Task BypassOnInactiveAuthorizationMiddlewareResultHandler_HandleAsync_WithInactiveAndChallengedWithStashedExpiredReason_ShouldUseStashedMessage_Test()
		{
			// Arrange
			var handler = GetHandler(active: false);
			var context = BuildHttpContext();
			context.Items["AuthFailureReason"] = "Token expired.";
			var nextInvoked = false;
			RequestDelegate next = _ => { nextInvoked = true; return Task.CompletedTask; };
			var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
			var authorizeResult = PolicyAuthorizationResult.Challenge();

			// Act
			await handler.HandleAsync(next, context, policy, authorizeResult);

			// Assert
			Assert.That(nextInvoked, Is.True);
			Assert.That(context.Response.Headers[SecurityConstants.WarningHeaderName].ToString(), Is.EqualTo("Token expired."));
		}

		[Test]
		public async Task BypassOnInactiveAuthorizationMiddlewareResultHandler_HandleAsync_WithSucceededAndActive_ShouldInvokeNextAndNotSetWarningHeader_Test()
		{
			// Arrange
			var handler = GetHandler(active: true);
			var context = BuildHttpContext();
			var nextInvoked = false;
			RequestDelegate next = _ => { nextInvoked = true; return Task.CompletedTask; };
			var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
			var authorizeResult = PolicyAuthorizationResult.Success();

			// Act
			await handler.HandleAsync(next, context, policy, authorizeResult);

			// Assert
			Assert.That(nextInvoked, Is.True);
			Assert.That(context.Response.Headers.ContainsKey(SecurityConstants.WarningHeaderName), Is.False);
		}

		[Test]
		public async Task BypassOnInactiveAuthorizationMiddlewareResultHandler_HandleAsync_WithSucceededAndInactive_ShouldInvokeNextAndNotSetWarningHeader_Test()
		{
			// Arrange
			var handler = GetHandler(active: false);
			var context = BuildHttpContext();
			var nextInvoked = false;
			RequestDelegate next = _ => { nextInvoked = true; return Task.CompletedTask; };
			var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
			var authorizeResult = PolicyAuthorizationResult.Success();

			// Act
			await handler.HandleAsync(next, context, policy, authorizeResult);

			// Assert
			Assert.That(nextInvoked, Is.True);
			Assert.That(context.Response.Headers.ContainsKey(SecurityConstants.WarningHeaderName), Is.False);
		}
	}
}

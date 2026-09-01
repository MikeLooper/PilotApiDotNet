using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using PilotApi.Repositories.Contracts.Repository;
using PilotApi.Services.Security;
using PilotApi.Shared.Constants;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace PilotApi.Services.Tests.Security
{
	[TestFixture]
	public class PreferredUsernameRoleClaimsTransformationTests
	{
		private static PreferredUsernameRoleClaimsTransformation GetTransformation(Mock<IUserRolesRepository> userRolesRepository)
		{
			return new PreferredUsernameRoleClaimsTransformation(
				NullLoggerFactory.Instance,
				userRolesRepository.Object);
		}

		[Test]
		public async Task PreferredUsernameRoleClaimsTransformation_TransformAsync_WithUnauthenticatedPrincipal_LeavesPrincipalUnchanged_Test()
		{
			// Arrange
			var userRolesRepository = new Mock<IUserRolesRepository>();
			var transformation = GetTransformation(userRolesRepository);
			var principal = new ClaimsPrincipal(new ClaimsIdentity());

			// Act
			var result = await transformation.TransformAsync(principal);

			// Assert
			Assert.That(result, Is.SameAs(principal));
			Assert.That(result.HasClaim(c => c.Type == ClaimTypes.Role), Is.False);
			userRolesRepository.Verify(v => v.GetRoleByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
		}

		[Test]
		public async Task PreferredUsernameRoleClaimsTransformation_TransformAsync_WithKnownUser_AddsRoleClaim_Test()
		{
			// Arrange
			var userRolesRepository = new Mock<IUserRolesRepository>();
			userRolesRepository
				.Setup(s => s.GetRoleByUserIdAsync("working_admin", It.IsAny<CancellationToken>()))
				.ReturnsAsync(RoleNames.Admin);
			var transformation = GetTransformation(userRolesRepository);

			var identity = new ClaimsIdentity("TestAuthType");
			identity.AddClaim(new Claim(SecurityConstants.PreferredUsernameClaimType, "working_admin"));
			var principal = new ClaimsPrincipal(identity);

			// Act
			var result = await transformation.TransformAsync(principal);

			// Assert
			Assert.That(result.FindFirst(ClaimTypes.Role)?.Value, Is.EqualTo(RoleNames.Admin));
		}

		[Test]
		public async Task PreferredUsernameRoleClaimsTransformation_TransformAsync_WithMissingPreferredUsernameClaim_LeavesPrincipalUnchanged_Test()
		{
			// Arrange
			var userRolesRepository = new Mock<IUserRolesRepository>();
			var transformation = GetTransformation(userRolesRepository);
			var identity = new ClaimsIdentity("TestAuthType");
			var principal = new ClaimsPrincipal(identity);

			// Act
			var result = await transformation.TransformAsync(principal);

			// Assert
			Assert.That(result.HasClaim(c => c.Type == ClaimTypes.Role), Is.False);
			userRolesRepository.Verify(v => v.GetRoleByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
		}

		[Test]
		public async Task PreferredUsernameRoleClaimsTransformation_TransformAsync_WithUnmappedUser_LeavesPrincipalUnchanged_Test()
		{
			// Arrange
			var userRolesRepository = new Mock<IUserRolesRepository>();
			userRolesRepository
				.Setup(s => s.GetRoleByUserIdAsync("unknown_user", It.IsAny<CancellationToken>()))
				.ReturnsAsync((string?)null);
			var transformation = GetTransformation(userRolesRepository);

			var identity = new ClaimsIdentity("TestAuthType");
			identity.AddClaim(new Claim(SecurityConstants.PreferredUsernameClaimType, "unknown_user"));
			var principal = new ClaimsPrincipal(identity);

			// Act
			var result = await transformation.TransformAsync(principal);

			// Assert
			Assert.That(result.HasClaim(c => c.Type == ClaimTypes.Role), Is.False);
		}

		[Test]
		public async Task PreferredUsernameRoleClaimsTransformation_TransformAsync_WithExistingRoleClaim_DoesNotCallRepositoryAgain_Test()
		{
			// Arrange
			var userRolesRepository = new Mock<IUserRolesRepository>();
			var transformation = GetTransformation(userRolesRepository);

			var identity = new ClaimsIdentity("TestAuthType");
			identity.AddClaim(new Claim(SecurityConstants.PreferredUsernameClaimType, "working_admin"));
			identity.AddClaim(new Claim(ClaimTypes.Role, RoleNames.Admin));
			var principal = new ClaimsPrincipal(identity);

			// Act
			var result = await transformation.TransformAsync(principal);

			// Assert
			Assert.That(result, Is.SameAs(principal));
			userRolesRepository.Verify(v => v.GetRoleByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
		}
	}
}

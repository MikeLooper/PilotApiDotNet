using NUnit.Framework;
using PilotApi.Repositories.Repositories;
using PilotApi.Shared.Constants;
using PilotApi.TestingShared.Utilities;
using System;
using System.Threading.Tasks;

namespace PilotApi.Repositories.Tests.Repositories
{
	[TestFixture]
	public class UserRolesRepositoryTests : TestBase
	{
		private static UserRolesRepository GetRepository()
		{
			return new UserRolesRepository(TestingSharedDoublesUtilities.GetMockLoggerFactory());
		}

		[TestCase("reader_user", RoleNames.ReadOnly)]
		[TestCase("working_user", RoleNames.ReadWrite)]
		[TestCase("working_admin", RoleNames.Admin)]
		public async Task UserRolesRepository_GetRoleByUserIdAsync_WithKnownUserId_ReturnsExpectedRole_Test(string userId, string expectedRole)
		{
			// Arrange
			var repository = GetRepository();

			// Act
			var result = await repository.GetRoleByUserIdAsync(userId);

			// Assert
			Assert.That(result, Is.EqualTo(expectedRole));
		}

		[TestCase("READER_USER", RoleNames.ReadOnly)]
		[TestCase("Working_Admin", RoleNames.Admin)]
		public async Task UserRolesRepository_GetRoleByUserIdAsync_IsCaseInsensitive_Test(string userId, string expectedRole)
		{
			// Arrange
			var repository = GetRepository();

			// Act
			var result = await repository.GetRoleByUserIdAsync(userId);

			// Assert
			Assert.That(result, Is.EqualTo(expectedRole));
		}

		[Test]
		public async Task UserRolesRepository_GetRoleByUserIdAsync_WithUnknownUserId_ReturnsNull_Test()
		{
			// Arrange
			var repository = GetRepository();

			// Act
			var result = await repository.GetRoleByUserIdAsync("unknown_user");

			// Assert
			Assert.That(result, Is.Null);
		}

		[TestCase(null)]
		[TestCase("")]
		[TestCase("   ")]
		public void UserRolesRepository_GetRoleByUserIdAsync_WithInvalidUserId_ThrowsArgumentException_Test(string? userId)
		{
			// Arrange
			var repository = GetRepository();

			// Act & Assert
			Assert.ThrowsAsync<ArgumentException>(() => repository.GetRoleByUserIdAsync(userId));
		}
	}
}

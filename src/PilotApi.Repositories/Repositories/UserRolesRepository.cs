using Microsoft.Extensions.Logging;
using PilotApi.Repositories.Contracts.Repository;
using PilotApi.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PilotApi.Repositories.Repositories
{
	/// <summary>
	/// A repository that mocks reading role assignments from a "UserRoles" database table, hard-coded for now.
	/// </summary>
	public class UserRolesRepository : IUserRolesRepository
	{
		private static readonly IReadOnlyDictionary<string, string> UserRoles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["reader_user"] = RoleNames.ReadOnly,
			["working_user"] = RoleNames.ReadWrite,
			["working_admin"] = RoleNames.Admin
		};

		/// <summary>
		/// Instantiates a new instance of the <see cref="UserRolesRepository"/> class.
		/// </summary>
		/// <param name="loggerFactory">
		/// A logger factory used to create loggers for logging information, warnings, and errors.
		/// </param>
		public UserRolesRepository(ILoggerFactory loggerFactory)
		{
			this.Logger = loggerFactory.CreateLogger(this.GetType());
		}

		/// <summary>
		/// Gets a logger for the current repository.
		/// </summary>
		protected ILogger Logger { get; }

		/// <inheritdoc/>
		public Task<string?> GetRoleByUserIdAsync(string userId, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(userId))
			{
				throw new ArgumentException($"Invalid argument: {nameof(userId)}");
			}

			UserRoles.TryGetValue(userId, out var role);

			this.Logger.LogInformation(
				"UserRoles lookup for UserId: '{UserId}' returned Role: '{Role}' ({RepositoryType})",
				userId,
				role ?? StringConstants.LogNull,
				this.GetType().Name);

			return Task.FromResult(role);
		}
	}
}

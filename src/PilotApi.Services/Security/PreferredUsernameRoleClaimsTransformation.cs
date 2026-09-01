using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using PilotApi.Repositories.Contracts.Repository;
using PilotApi.Shared.Constants;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PilotApi.Services.Security
{
	/// <summary>
	/// A claims transformation that adds a role claim to an authenticated principal, based on a lookup of the
	/// "preferred_username" claim (Security's standard username claim) against the (mocked) UserRoles table.
	/// </summary>
	/// <remarks>
	/// Roles are deliberately not read from Security token role claims; they come from <see cref="IUserRolesRepository"/>.
	/// </remarks>
	public sealed class PreferredUsernameRoleClaimsTransformation : IClaimsTransformation
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="PreferredUsernameRoleClaimsTransformation"/> class.
		/// </summary>
		/// <param name="loggerFactory">
		/// A logger factory used to create loggers for logging information, warnings, and errors.
		/// </param>
		/// <param name="userRolesRepository">
		/// A repository object used to look up a user's role.
		/// </param>
		public PreferredUsernameRoleClaimsTransformation(
			ILoggerFactory loggerFactory,
			IUserRolesRepository userRolesRepository)
		{
			this.Logger = loggerFactory.CreateLogger(this.GetType());
			this.UserRolesRepository = userRolesRepository;
		}

		private ILogger Logger { get; }

		private IUserRolesRepository UserRolesRepository { get; }

		/// <inheritdoc/>
		public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
		{
			if (principal?.Identity?.IsAuthenticated is not true)
			{
				return principal!;
			}

			// re-entrancy guard: TransformAsync can run more than once per request
			if (principal.HasClaim(c => c.Type == ClaimTypes.Role))
			{
				return principal;
			}

			var userId = principal.FindFirst(SecurityConstants.PreferredUsernameClaimType)?.Value;
			if (string.IsNullOrWhiteSpace(userId))
			{
				this.Logger.LogWarning(
					"Unable to resolve a role: the principal has no '{ClaimType}' claim ({TransformationType})",
					SecurityConstants.PreferredUsernameClaimType,
					this.GetType().Name);

				return principal;
			}

			var role = await this.UserRolesRepository.GetRoleByUserIdAsync(userId);
			if (string.IsNullOrWhiteSpace(role))
			{
				this.Logger.LogWarning(
					"Unable to resolve a role: no UserRoles entry was found for UserId: '{UserId}' ({TransformationType})",
					userId,
					this.GetType().Name);

				return principal;
			}

			var identity = new ClaimsIdentity();
			identity.AddClaim(new Claim(ClaimTypes.Role, role));
			principal.AddIdentity(identity);

			this.Logger.LogInformation(
				"Resolved Role: '{Role}' for UserId: '{UserId}' ({TransformationType})",
				role,
				userId,
				this.GetType().Name);

			return principal;
		}
	}
}

using System.Threading;
using System.Threading.Tasks;

namespace PilotApi.Repositories.Contracts.Repository
{
	/// <summary>
	/// An interface for a repository that reads role assignments for a user, from the (mocked) UserRoles table.
	/// </summary>
	public interface IUserRolesRepository
	{
		/// <summary>
		/// Return the role assigned to the supplied UserId.
		/// </summary>
		/// <param name="userId">
		/// The UserId to look up (matches the Security "preferred_username" claim).
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// The role name assigned to the UserId, or null if no role is assigned.
		/// </returns>
		Task<string?> GetRoleByUserIdAsync(string userId, CancellationToken cancellationToken = default);
	}
}

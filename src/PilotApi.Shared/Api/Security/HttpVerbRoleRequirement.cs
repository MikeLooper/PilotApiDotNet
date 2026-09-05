using Microsoft.AspNetCore.Authorization;

namespace PilotApi.Shared.Api.Security
{
	/// <summary>
	/// An authorization requirement that is satisfied when the current user's role claim allows the current HTTP verb.
	/// </summary>
	public sealed class HttpVerbRoleRequirement : IAuthorizationRequirement
	{
	}
}

namespace PilotApi.Shared.Constants
{
	/// <summary>
	/// Constants used with security processes.
	/// </summary>
	public static class SecurityConstants
	{
		/// <summary>
		/// The claim type for the Security "preferred_username" claim.
		/// </summary>
		public const string PreferredUsernameClaimType = "preferred_username";

		/// <summary>
		/// The name of the response header used to warn a caller about a bypassed security failure.
		/// </summary>
		public const string WarningHeaderName = "Warning";
	}
}

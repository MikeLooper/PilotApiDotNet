using System;
using System.Collections.Generic;

namespace PilotApi.Shared.Constants
{
	/// <summary>
	/// Constants for the role names used with role-based authorization.
	/// </summary>
	public static class RoleNames
	{
		/// <summary>
		/// A role that can perform all data maintenance (HTTP GET, POST, PUT, DELETE).
		/// </summary>
		public const string Admin = "admin_role";

		/// <summary>
		/// A role that can only read data (HTTP GET).
		/// </summary>
		public const string ReadOnly = "read_only_role";

		/// <summary>
		/// A role that can read and update data (HTTP GET, POST, PUT).
		/// </summary>
		public const string ReadWrite = "read_write_role";

		/// <summary>
		/// A read-only dictionary that maps role names to the HTTP verbs they are allowed to use.
		/// </summary>
		public static readonly IReadOnlyDictionary<string, string[]> MapToVerbs = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
		{
			[ReadOnly] = ["GET", "HEAD", "OPTIONS", "QUERY", "TRACE"],
			[ReadWrite] = ["GET", "HEAD", "OPTIONS", "QUERY", "PATCH", "POST", "PUT", "TRACE"],
			[Admin] = ["DELETE", "GET", "HEAD", "OPTIONS", "QUERY", "PATCH", "POST", "PUT", "TRACE"]
		};
	}
}

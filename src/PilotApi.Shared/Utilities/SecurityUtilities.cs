using PilotApi.Shared.Constants;
using System.Text;

namespace PilotApi.Shared.Utilities
{
	/// <summary>
	/// Utility methods used with security processes.
	/// </summary>
	public static class SecurityUtilities
	{
		/// <summary>
		/// Clean and return the supplied ConnectionString.
		/// </summary>
		/// <param name="connectionString">
		/// The ConnectionString to clean.
		/// </param>
		/// <returns>
		///  A cleaned ConnectionString.
		/// </returns>
		public static string ConnectionStringClean(string connectionString)
		{
			if (string.IsNullOrWhiteSpace(connectionString))
			{
				return connectionString;
			}

			var connectionStringCleaned = new StringBuilder();
			var connectionParts = connectionString.Split(";", System.StringSplitOptions.RemoveEmptyEntries);
			foreach ( var part in connectionParts )
			{
				var partParts = part.Split("=", System.StringSplitOptions.RemoveEmptyEntries);
				if (partParts.Length < 2)
				{
					continue;
				}

				if (partParts[0].Equals("Password", System.StringComparison.OrdinalIgnoreCase))
				{
					partParts[1] = StringConstants.Redacted;
				}

				if (connectionStringCleaned.Length > 0)
				{
					connectionStringCleaned.Append(";");
				}

				connectionStringCleaned.Append($"{partParts[0]}={partParts[1]}");
			}

			return connectionStringCleaned.ToString();
		}
	}
}

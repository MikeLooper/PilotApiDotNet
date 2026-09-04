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

		/// <summary>
		/// Return a value after redacting.
		/// </summary>
		/// <param name="sourceValue">
		/// The source value to clean.
		/// </param>
		/// <param name="edgeInclusions">
		/// The number of characters to include at the edges of the source value.
		/// Default = 4. If the source value is shorter than (edgeInclusions * 2), the entire value will be redacted.
		/// </param>
		/// <returns>
		/// A cleaned value with the edges included and the middle redacted.
		/// If a null or white spaces are passed in, the return value will be "-Empty-".
		/// </returns>
		public static string Redact(string? sourceValue, int edgeInclusions = 4)
		{
			if (string.IsNullOrWhiteSpace(sourceValue))
			{
				return StringConstants.LogEmpty;
			}

			if (edgeInclusions <= 0 || sourceValue.Length <= (edgeInclusions * 2))
			{
				return StringConstants.Redacted;
			}

			var prefix = sourceValue[..edgeInclusions];
			var suffix = sourceValue[^edgeInclusions..];
			return $"{prefix}{StringConstants.Redacted}{suffix}";
		}
	}
}

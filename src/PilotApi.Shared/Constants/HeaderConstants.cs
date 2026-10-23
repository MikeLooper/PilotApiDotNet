namespace PilotApi.Shared.Constants
{
	/// <summary>
	/// Constants used for working with request and response headers.
	/// </summary>
	public static class HeaderConstants
	{
		/// <summary>
		/// The header name used for the Api Version.
		/// </summary>
		public const string ApiVersionHeaderName = "X-Api-Version";

		/// <summary>
		/// The header name used for the correlation ID.
		/// </summary>
		public const string CorrelationIdHeaderName = "X-Correlation-Id";

		/// <summary>
		/// The header name used for the request ID.
		/// </summary>
		public const string RequestIdHeaderName = "X-Request-Id";
	}
}

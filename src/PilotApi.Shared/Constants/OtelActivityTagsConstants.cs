namespace PilotApi.Shared.Constants
{
	/// <summary>
	/// Constants used for working with Open Telemetry Activity Tags.
	/// </summary>
	public static class OtelActivityTagsConstants
	{
		/// <summary>
		/// The key name used for the correlation ID.
		/// </summary>
		public const string CorrelationId = "correlationId";

		/// <summary>
		/// The key name used for the request ID.
		/// </summary>
		public const string RequestId = "request.id";
	}
}

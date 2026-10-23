namespace PilotApi.Shared.Constants
{
	/// <summary>
	/// Constants used for working with Open Telemetry LogRecord Attributes.
	/// </summary>
	public static class OtelLogRecordAttributesConstants
	{
		/// <summary>
		/// The key name used for the correlation ID.
		/// </summary>
		public const string CorrelationId = "CorrelationId";

		/// <summary>
		/// The key name used for an exceptions inner exception.
		/// </summary>
		public const string ExceptionInnerException = "Exception.InnerException";

		/// <summary>
		/// The key name used for an exceptions message.
		/// </summary>
		public const string ExceptionMessage = "Exception.Message";

		/// <summary>
		/// The key name used for an exceptions stack trace.
		/// </summary>
		public const string ExceptionStacktrace = "Exception.Stacktrace";

		/// <summary>
		/// The key name used for the request ID.
		/// </summary>
		public const string RequestId = "Request.Id";
	}
}

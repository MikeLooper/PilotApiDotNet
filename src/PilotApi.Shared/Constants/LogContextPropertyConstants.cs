namespace PilotApi.Shared.Constants
{
	/// <summary>
	/// Constants used for the property names pushed onto the Serilog log context for the
	/// life of a request. Every log event written while a request is in progress will carry
	/// these properties, and they can be referenced by name in a sink output template.
	/// </summary>
	public static class LogContextPropertyConstants
	{
		/// <summary>
		/// The property name used for the name of the caller.
		/// </summary>
		public const string CallerName = "CallerName";

		/// <summary>
		/// The property name used for the correlation ID.
		/// </summary>
		public const string CorrelationId = "CorrelationId";

		/// <summary>
		/// The property name used for the request ID.
		/// </summary>
		public const string RequestId = "RequestId";
	}
}

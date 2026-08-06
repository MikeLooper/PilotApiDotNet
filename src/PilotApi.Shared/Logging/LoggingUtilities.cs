using PilotApi.Shared.Logging.Models;

namespace PilotApi.Shared.Logging
{
	/// <summary>
	/// Utilities used for logging processes.
	/// </summary>
	public static class LoggingUtilities
	{
		/// <summary>
		/// Gets a new instance of the <see cref="LoggingCorrelation"/> class.
		/// This will include a new correlation ID and a user message.
		/// </summary>
		/// <returns>
		/// A new instance of the <see cref="LoggingCorrelation"/> class.
		/// </returns>
		public static LoggingCorrelation GetLoggingCorrelation()
		{
			return new LoggingCorrelation();
		}
	}
}

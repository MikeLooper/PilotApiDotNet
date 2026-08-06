using System;

namespace PilotApi.Shared.Logging.Models
{
	/// <summary>
	/// A model used for returning logging correlation details.
	/// </summary>
	public class LoggingCorrelation
	{
		/// <summary>
		/// Instantiate a <see cref="LoggingCorrelation"/> object.
		/// </summary>
		public LoggingCorrelation()
		{
			this.CorrelationId = Guid.NewGuid().ToString();
		}

		/// <summary>
		/// Gets or sets the correlation ID for the logging process.
		/// </summary>
		public string CorrelationId { get; protected set; }

		/// <summary>
		/// Gets or sets the correlation ID for the logging process.
		/// </summary>
		public string UserMessage
		{
			get
			{
				return "An error occurred. " +
					$"The error details can be found in the log with the following correlation ID: {this.CorrelationId}";
			}
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			return $"{nameof(this.CorrelationId)}={this.CorrelationId}" +
				$"{nameof(this.UserMessage)}=: {this.UserMessage}";
		}
	}
}

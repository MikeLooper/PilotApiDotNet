using OpenTelemetry;
using OpenTelemetry.Logs;
using PilotApi.Shared.Constants;
using System.Collections.Generic;
using System.Linq;

namespace PilotApi.Shared.OpenTelemetry.Processors
{
	/// <summary>
	/// Include exception information in the log record attributes, such as stack trace and inner exception details.
	/// </summary>
	public class LogExceptionStackTraceProcessor : BaseProcessor<LogRecord>
	{
		private readonly string processorName;

		/// <summary>
		/// Initializes a new instance of the <c>LogExceptionStackTraceProcessor</c> class.
		/// </summary>
		/// <param name="name">
		/// Name used to identify the processor. Defaults to <c>"LogExceptionStackTraceProcessor"</c> when not provided.
		/// </param>
		public LogExceptionStackTraceProcessor(string name = "LogExceptionStackTraceProcessor")
		{
			this.processorName = name;
		}

		/// <summary>
		/// Appends exception details to the log record attributes when an exception is present.
		/// </summary>
		/// <remarks>
		/// Adds the <c>exception.stacktrace</c> attribute and, when available, the
		/// <c>exception.innerexception</c> attribute.
		/// </remarks>
		/// <param name="record">
		/// The log record to enrich with exception metadata.
		/// </param>
		public override void OnEnd(LogRecord record)
		{
			if (record.Exception != null)
			{
				var attributes = record.Attributes?.ToList() ?? new List<KeyValuePair<string, object?>>();

				attributes.Add(new KeyValuePair<string, object?>(
					OtelLogRecordAttributesConstants.ExceptionMessage, record.Exception.Message));
				attributes.Add(new KeyValuePair<string, object?>(
					OtelLogRecordAttributesConstants.ExceptionStacktrace, record.Exception.StackTrace));

				if (record.Exception.InnerException != null)
				{
					attributes.Add(new KeyValuePair<string, object?>(
						OtelLogRecordAttributesConstants.ExceptionInnerException, record.Exception.InnerException.ToString()));
				}

				record.Attributes = attributes;
			}
		}
	}
}

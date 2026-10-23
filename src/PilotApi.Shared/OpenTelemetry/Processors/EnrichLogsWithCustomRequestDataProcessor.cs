using Microsoft.AspNetCore.Http;
using OpenTelemetry;
using OpenTelemetry.Logs;
using PilotApi.Shared.Constants;
using PilotApi.Shared.Models;
using System.Collections.Generic;
using System.Linq;

namespace PilotApi.Shared.OpenTelemetry.Processors
{
	/// <summary>
	/// Add custom request data (e.g., correlation ID, request ID) to log records.
	/// </summary>
	public class EnrichLogsWithCustomRequestDataProcessor : BaseProcessor<LogRecord>
	{
		private readonly IHttpContextAccessor httpContextAccessor;

		/// <summary>
		/// Initializes a new instance of the <see cref="EnrichLogsWithCustomRequestDataProcessor"/> class.
		/// </summary>
		/// <param name="httpContextAccessor">
		/// Provides access to the current <see cref="HttpContext"/>.
		/// </param>
		public EnrichLogsWithCustomRequestDataProcessor(IHttpContextAccessor httpContextAccessor)
		{
			this.httpContextAccessor = httpContextAccessor;
		}

		/// Enriches the log record with user and request correlation attributes at the end of processing.
		/// </summary>
		/// <remarks>
		/// Adds correlation and request identifiers when request settings are available from the current HTTP
		/// request services.
		/// </remarks>
		/// <param name="record">
		/// The log record to update with additional attributes.
		/// </param>
		public override void OnEnd(LogRecord record)
		{
			var httpContext = this.httpContextAccessor?.HttpContext;

			var attributes = record.Attributes?.ToList() ?? new List<KeyValuePair<string, object?>>();

			var requestSettings = httpContext?.RequestServices.GetService(typeof(RequestSettings)) as RequestSettings;

			if (requestSettings != null)
			{
				attributes.Add(new KeyValuePair<string, object?>(
					OtelLogRecordAttributesConstants.CorrelationId, requestSettings.CorrelationId));
				attributes.Add(new KeyValuePair<string, object?>(
					OtelLogRecordAttributesConstants.RequestId, requestSettings.RequestId));
			}

			record.Attributes = attributes;
		}
	}
}

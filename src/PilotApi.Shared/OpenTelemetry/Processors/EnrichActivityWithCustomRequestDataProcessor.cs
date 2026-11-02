using Microsoft.AspNetCore.Http;
using OpenTelemetry;
using PilotApi.Shared.Constants;
using PilotApi.Shared.Models;
using System.Diagnostics;

namespace PilotApi.Shared.OpenTelemetry.Processors
{
	/// <summary>
	/// Add custom request data (e.g., correlation ID, request ID) to the spans started during a request.
	/// </summary>
	public class EnrichActivityWithCustomRequestDataProcessor : BaseProcessor<Activity>
	{
		private readonly IHttpContextAccessor httpContextAccessor;

		/// <summary>
		/// Initializes a new instance of the <see cref="EnrichActivityWithCustomRequestDataProcessor"/> class.
		/// </summary>
		/// <param name="httpContextAccessor">
		/// Provides access to the current <see cref="HttpContext"/>.
		/// </param>
		public EnrichActivityWithCustomRequestDataProcessor(IHttpContextAccessor httpContextAccessor)
		{
			this.httpContextAccessor = httpContextAccessor;
		}

		/// <summary>
		/// Tags the span when it starts, while the request is still current.
		/// </summary>
		/// <remarks>
		/// The request's own server span is skipped. It starts before the middleware has read the correlation ID,
		/// so the ASP.NET Core instrumentation tags it instead.
		/// </remarks>
		/// <param name="data">
		/// The span to update.
		/// </param>
		public override void OnStart(Activity data)
		{
			if (data.Kind == ActivityKind.Server)
			{
				return;
			}

			var httpContext = this.httpContextAccessor?.HttpContext;

			if (httpContext?.RequestServices.GetService(typeof(RequestSettings)) is RequestSettings requestSettings)
			{
				data.SetTag(OtelActivityTagsConstants.CorrelationId, requestSettings.CorrelationId);
				data.SetTag(OtelActivityTagsConstants.RequestId, requestSettings.RequestId);

				if (!string.IsNullOrEmpty(requestSettings.CallerName))
				{
					data.SetTag(OtelActivityTagsConstants.CallerName, requestSettings.CallerName);
				}
			}
		}
	}
}

using System;

namespace PilotApi.Shared.Models
{
	/// <summary>
	/// A request-scoped object containing data associated with the current request
	/// (e.g. RequestId, CorrelationId). An instance of this class is created at the
	/// start of a request and remains available throughout the life of the request.
	/// </summary>
	public class RequestSettings
	{
		/// <summary>
		/// Gets or sets the unique identifier for the current request.
		/// </summary>
		public string RequestId { get; set; } = Guid.NewGuid().ToString();

		/// <summary>
		/// Gets or sets the correlation ID for the current request. This value is
		/// typically read from the <c>X-Correlation-Id</c> request header, or generated
		/// as a new GUID when the header is not present.
		/// </summary>
		public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
	}
}

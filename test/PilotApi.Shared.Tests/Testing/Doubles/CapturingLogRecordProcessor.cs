using OpenTelemetry;
using OpenTelemetry.Logs;
using System.Collections.Generic;
using System.Linq;

namespace PilotApi.Shared.Tests.Testing.Doubles
{
	internal sealed class CapturingLogRecordProcessor : BaseProcessor<LogRecord>
	{
		public IReadOnlyList<KeyValuePair<string, object?>>? Attributes { get; private set; }

		public override void OnEnd(LogRecord record)
		{
			this.Attributes = record.Attributes?.ToList();
		}
	}
}

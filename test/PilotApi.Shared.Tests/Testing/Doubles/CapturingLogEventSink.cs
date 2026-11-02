using Serilog.Core;
using Serilog.Events;
using System.Collections.Generic;

namespace PilotApi.Shared.Tests.Testing.Doubles
{
	internal sealed class CapturingLogEventSink : ILogEventSink
	{
		public List<LogEvent> Events { get; } = new List<LogEvent>();

		public void Emit(LogEvent logEvent) => this.Events.Add(logEvent);
	}
}

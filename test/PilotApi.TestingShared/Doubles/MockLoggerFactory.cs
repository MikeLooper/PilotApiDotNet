using Microsoft.Extensions.Logging;
using PilotApi.TestingShared.Utilities;

namespace PilotApi.TestingShared.Doubles
{
	/// <summary>
	/// A mock logger factory for testing purposes.
	/// </summary>
	public class MockLoggerFactory : ILoggerFactory
	{
			public ILogger CreateLogger(string categoryName) => TestingSharedDoublesUtilities.GetMockLogger();
			public void AddProvider(ILoggerProvider provider) { }
			public void Dispose() { }
	}
}

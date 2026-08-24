using NUnit.Framework;
using PilotApi.Shared.Logging.Models;

namespace PilotApi.Shared.Tests.Logging.Models
{
	[TestFixture]
	public class LoggingCorrelationTests
	{
		[Test]
		public void LoggingCorrelation_CorrelationId_ShouldBeGenerated_Test()
		{
			// Arrange
			// Act
			var loggingCorrelation = new LoggingCorrelation();

			// Assert
			Assert.That(loggingCorrelation.CorrelationId, Is.Not.Null.And.Not.Empty);
		}

		[Test]
		public void LoggingCorrelation_ToString_ShouldIncludeCorrelationIdAndUserMessage_Test()
		{
			// Arrange
			var loggingCorrelation = new LoggingCorrelation();
			var propertyInfo = typeof(LoggingCorrelation).GetProperty(nameof(LoggingCorrelation.CorrelationId));
			var correlationId = "12345678-1234-1234-1234-123456789abc";
			propertyInfo?.SetValue(loggingCorrelation, correlationId);

			// Act
			var result = loggingCorrelation.ToString();

			// Assert
			Assert.That(result, Does.Contain($"CorrelationId={correlationId}"));
			Assert.That(result, Does.Contain("UserMessage=An error occurred."));
			Assert.That(result, Does.Contain(correlationId));
		}

		[Test]
		public void LoggingCorrelation_UserMessage_ShouldContainCorrelationId_Test()
		{
			// Arrange
			var loggingCorrelation = new LoggingCorrelation();

			// Act
			var result = loggingCorrelation.UserMessage;

			// Assert
			Assert.That(result, Does.StartWith("An error occurred."));
			Assert.That(result, Does.Contain(loggingCorrelation.CorrelationId));
		}
	}
}

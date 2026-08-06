using NUnit.Framework;
using PilotApi.Shared.Logging;

namespace PilotApi.Shared.Tests.Logging
{
	[TestFixture]
	public class LoggingUtilitiesTests
	{
		[Test]
		public void LoggingUtilities_GetLoggingCorrelation_ShouldReturnCorrelationWithExpectedMessage_Test()
		{
			// Arrange
			// Act
			var loggingCorrelation = LoggingUtilities.GetLoggingCorrelation();

			// Assert
			Assert.That(loggingCorrelation.UserMessage, Does.StartWith("An error occurred."));
			Assert.That(loggingCorrelation.ToString(), Does.Contain(nameof(loggingCorrelation.CorrelationId)));
			Assert.That(loggingCorrelation.ToString(), Does.Contain(nameof(loggingCorrelation.UserMessage)));
		}

		[Test]
		public void LoggingUtilities_GetLoggingCorrelation_ShouldReturnDifferentCorrelationIds_Test()
		{
			// Arrange
			// Act
			var firstLoggingCorrelation = LoggingUtilities.GetLoggingCorrelation();
			var secondLoggingCorrelation = LoggingUtilities.GetLoggingCorrelation();

			// Assert
			Assert.That(firstLoggingCorrelation.CorrelationId, Is.Not.EqualTo(secondLoggingCorrelation.CorrelationId));
		}

		[Test]
		public void LoggingUtilities_GetLoggingCorrelation_ShouldReturnNewLoggingCorrelation_Test()
		{
			// Arrange
			// Act
			var loggingCorrelation = LoggingUtilities.GetLoggingCorrelation();

			// Assert
			Assert.NotNull(loggingCorrelation);
			Assert.That(loggingCorrelation.CorrelationId, Is.Not.Null.And.Not.Empty);
			Assert.That(loggingCorrelation.UserMessage, Does.Contain(loggingCorrelation.CorrelationId));
		}
	}
}

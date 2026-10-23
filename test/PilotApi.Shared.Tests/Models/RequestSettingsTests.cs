using System;
using NUnit.Framework;
using PilotApi.Shared.Models;

namespace PilotApi.Shared.Tests.Models
{
	[TestFixture]
	public class RequestSettingsTests : TestBase
	{
		[Test]
		public void RequestSettings_Constructor_ShouldGenerateIndependentIdentifiers_Test()
		{
			// Arrange & Act
			var settings = new RequestSettings();
			var otherSettings = new RequestSettings();

			// Assert
			Assert.That(Guid.TryParse(settings.RequestId, out _), Is.True);
			Assert.That(Guid.TryParse(settings.CorrelationId, out _), Is.True);
			Assert.That(settings.RequestId, Is.Not.EqualTo(settings.CorrelationId));
			Assert.That(settings.RequestId, Is.Not.EqualTo(otherSettings.RequestId));
			Assert.That(settings.CorrelationId, Is.Not.EqualTo(otherSettings.CorrelationId));
		}

		[Test]
		public void RequestSettings_RequestId_WhenOverridden_ShouldReturnAssignedValue_Test()
		{
			// Arrange
			var settings = new RequestSettings();
			var requestId = "request-id";

			// Act
			settings.RequestId = requestId;

			// Assert
			Assert.That(settings.RequestId, Is.EqualTo(requestId));
		}

		[Test]
		public void RequestSettings_CorrelationId_WhenOverridden_ShouldReturnAssignedValue_Test()
		{
			// Arrange
			var settings = new RequestSettings();
			var correlationId = "correlation-id";

			// Act
			settings.CorrelationId = correlationId;

			// Assert
			Assert.That(settings.CorrelationId, Is.EqualTo(correlationId));
		}
	}
}

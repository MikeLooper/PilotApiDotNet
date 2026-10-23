using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OpenTelemetry;
using OpenTelemetry.Logs;
using PilotApi.Shared.Constants;
using PilotApi.Shared.Models;
using PilotApi.Shared.OpenTelemetry.Processors;
using System.Collections.Generic;
using System.Linq;

namespace PilotApi.Shared.Tests.OpenTelemetry.Processors
{
	[TestFixture]
	public class EnrichLogsWithCustomRequestDataProcessorTests : TestBase
	{
		[Test]
		public void EnrichLogsWithCustomRequestDataProcessor_OnEnd_WithNullHttpContext_DoesNotAddRequestAttributes_Test()
		{
			// Arrange
			var httpContextAccessor = new HttpContextAccessor();

			// Act
			var attributes = ProcessLog(httpContextAccessor);

			// Assert
			Assert.That(attributes, Has.None.Matches<KeyValuePair<string, object?>>(
				attribute => attribute.Key == OtelLogRecordAttributesConstants.CorrelationId ||
					attribute.Key == OtelLogRecordAttributesConstants.RequestId));
		}

		[Test]
		public void EnrichLogsWithCustomRequestDataProcessor_OnEnd_WithoutRequestSettings_DoesNotAddRequestAttributes_Test()
		{
			// Arrange
			var httpContextAccessor = new HttpContextAccessor
			{
				HttpContext = new DefaultHttpContext
				{
					RequestServices = new ServiceCollection().BuildServiceProvider()
				}
			};

			// Act
			var attributes = ProcessLog(httpContextAccessor);

			// Assert
			Assert.That(attributes, Has.None.Matches<KeyValuePair<string, object?>>(
				attribute => attribute.Key == OtelLogRecordAttributesConstants.CorrelationId ||
					attribute.Key == OtelLogRecordAttributesConstants.RequestId));
		}

		[Test]
		public void EnrichLogsWithCustomRequestDataProcessor_OnEnd_WithRequestSettings_AddsRequestAttributes_Test()
		{
			// Arrange
			var requestSettings = new RequestSettings
			{
				CorrelationId = "correlation-id",
				RequestId = "request-id"
			};
			var httpContextAccessor = new HttpContextAccessor
			{
				HttpContext = new DefaultHttpContext
				{
					RequestServices = new ServiceCollection()
						.AddSingleton(requestSettings)
						.BuildServiceProvider()
				}
			};
			// Act
			var attributes = ProcessLog(httpContextAccessor);

			// Assert
			Assert.That(attributes.Single(attribute => attribute.Key == OtelLogRecordAttributesConstants.CorrelationId).Value,
				Is.EqualTo("correlation-id"));
			Assert.That(attributes.Single(attribute => attribute.Key == OtelLogRecordAttributesConstants.RequestId).Value,
				Is.EqualTo("request-id"));
		}

		private static IReadOnlyList<KeyValuePair<string, object?>> ProcessLog(IHttpContextAccessor httpContextAccessor)
		{
			var attributes = new List<KeyValuePair<string, object?>>();
			var attributeCaptureProcessor = new Mock<BaseProcessor<LogRecord>>();
			attributeCaptureProcessor
				.Setup(processor => processor.OnEnd(It.IsAny<LogRecord>()))
				.Callback<LogRecord>(record => attributes.AddRange(record.Attributes ?? []));

			using var loggerFactory = LoggerFactory.Create(builder => builder.AddOpenTelemetry(options =>
			{
				options.AddProcessor(new EnrichLogsWithCustomRequestDataProcessor(httpContextAccessor));
				options.AddProcessor(attributeCaptureProcessor.Object);
			}));

			loggerFactory.CreateLogger<EnrichLogsWithCustomRequestDataProcessorTests>()
				.LogInformation("Test log message");

			return attributes;
		}
	}
}

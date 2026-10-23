using Microsoft.Extensions.Logging;
using NUnit.Framework;
using PilotApi.Shared.Constants;
using PilotApi.Shared.OpenTelemetry.Processors;
using System;
using System.Linq;
using PilotApi.Shared.Tests.Testing.Doubles;

namespace PilotApi.Shared.Tests.OpenTelemetry.Processors
{
	[TestFixture]
	public class LogExceptionStackTraceProcessorTests : TestBase
	{
		[Test]
		public void LogExceptionStackTraceProcessor_OnEnd_WhenRecordHasNoException_ShouldNotAddAttributes_Test()
		{
			// Arrange
			var processor = new CapturingLogRecordProcessor();

			// Act
			var attributes = EmitLog(null, processor);

			// Assert
			var attributeKeys = attributes!.Select(attribute => attribute.Key).ToArray();
			Assert.That(attributeKeys, Does.Not.Contain(OtelLogRecordAttributesConstants.ExceptionMessage));
			Assert.That(attributeKeys, Does.Not.Contain(OtelLogRecordAttributesConstants.ExceptionStacktrace));
			Assert.That(attributeKeys, Does.Not.Contain(OtelLogRecordAttributesConstants.ExceptionInnerException));
		}

		[Test]
		public void LogExceptionStackTraceProcessor_OnEnd_WhenExceptionHasNoInnerException_ShouldAddMessageAndNullStackTrace_Test()
		{
			// Arrange
			var exception = new InvalidOperationException("A failure occurred.");
			var processor = new CapturingLogRecordProcessor();

			// Act
			var loggedAttributes = EmitLog(exception, processor);

			// Assert
			var attributes = loggedAttributes!.ToDictionary(attribute => attribute.Key, attribute => attribute.Value);
			Assert.That(attributes[OtelLogRecordAttributesConstants.ExceptionMessage], Is.EqualTo(exception.Message));
			Assert.That(attributes, Does.ContainKey(OtelLogRecordAttributesConstants.ExceptionStacktrace));
			Assert.That(attributes[OtelLogRecordAttributesConstants.ExceptionStacktrace], Is.Null);
			Assert.That(attributes, Does.Not.ContainKey(OtelLogRecordAttributesConstants.ExceptionInnerException));
		}

		[Test]
		public void LogExceptionStackTraceProcessor_OnEnd_WhenExceptionHasInnerException_ShouldAddNestedExceptionAndNullStackTrace_Test()
		{
			// Arrange
			var innerException = new ArgumentException("The inner failure occurred.");
			var exception = new InvalidOperationException("A failure occurred.", innerException);
			var processor = new CapturingLogRecordProcessor();

			// Act
			var loggedAttributes = EmitLog(exception, processor);

			// Assert
			var attributes = loggedAttributes!.ToDictionary(attribute => attribute.Key, attribute => attribute.Value);
			Assert.That(attributes[OtelLogRecordAttributesConstants.ExceptionMessage], Is.EqualTo(exception.Message));
			Assert.That(attributes, Does.ContainKey(OtelLogRecordAttributesConstants.ExceptionStacktrace));
			Assert.That(attributes[OtelLogRecordAttributesConstants.ExceptionStacktrace], Is.Null);
			Assert.That(attributes[OtelLogRecordAttributesConstants.ExceptionInnerException], Is.EqualTo(innerException.ToString()));
		}

		private static System.Collections.Generic.IReadOnlyList<System.Collections.Generic.KeyValuePair<string, object?>>? EmitLog(
			Exception? exception, CapturingLogRecordProcessor processor)
		{
			using var loggerFactory = LoggerFactory.Create(builder =>
				builder.AddOpenTelemetry(options =>
				{
					options.AddProcessor(new LogExceptionStackTraceProcessor());
					options.AddProcessor(processor);
				}));
			var logger = loggerFactory.CreateLogger<LogExceptionStackTraceProcessorTests>();

			if (exception == null)
			{
				logger.LogInformation("A log record without an exception.");
			}
			else
			{
				logger.LogError(exception, "A log record with an exception.");
			}

			return processor.Attributes;
		}
	}
}

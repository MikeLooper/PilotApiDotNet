using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PilotApi.Shared.Constants;
using PilotApi.Shared.Models;
using PilotApi.Shared.OpenTelemetry.Processors;
using System.Diagnostics;

namespace PilotApi.Shared.Tests.OpenTelemetry.Processors
{
	[TestFixture]
	public class EnrichActivityWithCustomRequestDataProcessorTests : TestBase
	{
		[Test]
		public void EnrichActivityWithCustomRequestDataProcessor_OnStart_WithNullHttpContext_AddsNoTags_Test()
		{
			// Arrange
			using var activity = new Activity("test");

			// Act
			new EnrichActivityWithCustomRequestDataProcessor(new HttpContextAccessor()).OnStart(activity);

			// Assert
			Assert.That(activity.GetTagItem(OtelActivityTagsConstants.CorrelationId), Is.Null);
		}

		[Test]
		public void EnrichActivityWithCustomRequestDataProcessor_OnStart_WithRequestSettings_AddsTags_Test()
		{
			// Arrange
			var requestSettings = new RequestSettings();
			using var activity = new Activity("test");

			// Act
			new EnrichActivityWithCustomRequestDataProcessor(CreateAccessor(requestSettings)).OnStart(activity);

			// Assert
			Assert.That(activity.GetTagItem(OtelActivityTagsConstants.CorrelationId), Is.EqualTo(requestSettings.CorrelationId));
			Assert.That(activity.GetTagItem(OtelActivityTagsConstants.RequestId), Is.EqualTo(requestSettings.RequestId));
		}

		[Test]
		public void EnrichActivityWithCustomRequestDataProcessor_OnStart_ServerSpan_AddsNoTags_Test()
		{
			// Arrange
			using var source = new ActivitySource("test");
			using var listener = new ActivityListener
			{
				ShouldListenTo = _ => true,
				Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData
			};
			ActivitySource.AddActivityListener(listener);
			using var activity = source.StartActivity("test", ActivityKind.Server)!;

			// Act
			new EnrichActivityWithCustomRequestDataProcessor(CreateAccessor(new RequestSettings())).OnStart(activity);

			// Assert
			Assert.That(activity.GetTagItem(OtelActivityTagsConstants.CorrelationId), Is.Null);
		}

		private static HttpContextAccessor CreateAccessor(RequestSettings requestSettings)
		{
			return new HttpContextAccessor
			{
				HttpContext = new DefaultHttpContext
				{
					RequestServices = new ServiceCollection().AddSingleton(requestSettings).BuildServiceProvider()
				}
			};
		}
	}
}

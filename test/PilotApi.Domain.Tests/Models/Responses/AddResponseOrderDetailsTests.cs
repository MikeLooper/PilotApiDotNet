using NUnit.Framework;
using PilotApi.Domain.Models.Responses;

namespace PilotApi.Domain.Tests.Models.Responses
{
	[TestFixture]
	public class AddResponseOrderDetailsTests : TestBase
	{
		[Test]
		public void AddResponseOrderDetails_DefaultConstructor_InitializesOrderIdAndProductIdToZero_Test()
		{
			AddResponseOrderDetails response = new();

			Assert.That(response.OrderId, Is.EqualTo(0));
			Assert.That(response.ProductId, Is.EqualTo(0));
		}

		[Test]
		public void AddResponseOrderDetails_Constructor_SetsOrderIdAndProductId_Test()
		{
			AddResponseOrderDetails response = new(42, 7);

			Assert.That(response.OrderId, Is.EqualTo(42));
			Assert.That(response.ProductId, Is.EqualTo(7));
		}

		[Test]
		public void AddResponseOrderDetails_OrderIdProperty_CanBeUpdated_Test()
		{
			AddResponseOrderDetails response = new();

			response.OrderId = 99;

			Assert.That(response.OrderId, Is.EqualTo(99));
		}

		[Test]
		public void AddResponseOrderDetails_ProductIdProperty_CanBeUpdated_Test()
		{
			AddResponseOrderDetails response = new();

			response.ProductId = 55;

			Assert.That(response.ProductId, Is.EqualTo(55));
		}

		[Test]
		public void AddResponseOrderDetails_ToString_ReturnsFormattedString_Test()
		{
			AddResponseOrderDetails response = new(42, 7);

			string result = response.ToString();

			Assert.That(result, Is.EqualTo("OrderId=42, ProductId=7"));
		}
	}
}

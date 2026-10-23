using NUnit.Framework;
using PilotApi.Domain.Models.Dto;

namespace PilotApi.Domain.Tests.Models.Dto
{
	[TestFixture]
	public class OrderDetailsDtoTests
	{
		[Test]
		public void OrderDetailsDto_Properties_RoundTripAndToStringReturnsExpectedValue_Test()
		{
			OrderDetailsDto dto = new()
			{
				Discount = 0.15f,
				OrderId = 12,
				ProductId = 34,
				Quantity = 5,
				UnitPrice = 19.95m,
			};

			Assert.That(dto.Discount, Is.EqualTo(0.15f));
			Assert.That(dto.OrderId, Is.EqualTo(12));
			Assert.That(dto.ProductId, Is.EqualTo(34));
			Assert.That(dto.Quantity, Is.EqualTo(5));
			Assert.That(dto.UnitPrice, Is.EqualTo(19.95m));
			Assert.That(dto.ToString(), Is.EqualTo("Discount=0.15, OrderId=12, ProductId=34, Quantity=5, UnitPrice=19.95"));
		}
	}
}


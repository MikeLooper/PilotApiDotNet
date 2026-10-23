using NUnit.Framework;
using PilotApi.Domain.Models.Dto;

namespace PilotApi.Domain.Tests.Models.Dto
{
	[TestFixture]
	public class ShippersDtoTests
	{
		[Test]
		public void ShippersDto_Properties_RoundTripAndToStringReturnsExpectedValue_Test()
		{
			ShippersDto dto = new()
			{
				CompanyName = "Speedy Express",
				Phone = "555-7777",
				ShipperId = 3,
			};

			Assert.That(dto.CompanyName, Is.EqualTo("Speedy Express"));
			Assert.That(dto.Phone, Is.EqualTo("555-7777"));
			Assert.That(dto.ShipperId, Is.EqualTo(3));
			Assert.That(dto.ToString(), Is.EqualTo("CompanyName=Speedy Express, Phone=555-7777, ShipperId=3"));
		}
	}
}


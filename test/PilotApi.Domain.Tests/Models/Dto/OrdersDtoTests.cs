using NUnit.Framework;
using PilotApi.Domain.Models.Dto;
using System;

namespace PilotApi.Domain.Tests.Models.Dto
{
	[TestFixture]
	public class OrdersDtoTests
	{
		[Test]
		public void OrdersDto_Properties_RoundTripAndToStringReturnsExpectedValue_Test()
		{
			DateTime orderDate = new(2024, 1, 2, 3, 4, 5);
			DateTime requiredDate = new(2024, 1, 10, 3, 4, 5);
			DateTime shippedDate = new(2024, 1, 5, 3, 4, 5);
			OrdersDto dto = new()
			{
				CustomerId = "ALFKI",
				EmployeeId = 9,
				Freight = 27.5m,
				OrderDate = orderDate,
				OrderId = 77,
				RequiredDate = requiredDate,
				ShipAddress = "1 Dock Way",
				ShipCity = "Portland",
				ShipCountry = "USA",
				ShipName = "Speedy Express",
				ShippedDate = shippedDate,
				ShipPostalCode = "97201",
				ShipRegion = "OR",
				ShipVia = 3,
			};

			Assert.That(dto.CustomerId, Is.EqualTo("ALFKI"));
			Assert.That(dto.EmployeeId, Is.EqualTo(9));
			Assert.That(dto.Freight, Is.EqualTo(27.5m));
			Assert.That(dto.OrderDate, Is.EqualTo(orderDate));
			Assert.That(dto.OrderId, Is.EqualTo(77));
			Assert.That(dto.RequiredDate, Is.EqualTo(requiredDate));
			Assert.That(dto.ShipAddress, Is.EqualTo("1 Dock Way"));
			Assert.That(dto.ShipCity, Is.EqualTo("Portland"));
			Assert.That(dto.ShipCountry, Is.EqualTo("USA"));
			Assert.That(dto.ShipName, Is.EqualTo("Speedy Express"));
			Assert.That(dto.ShippedDate, Is.EqualTo(shippedDate));
			Assert.That(dto.ShipPostalCode, Is.EqualTo("97201"));
			Assert.That(dto.ShipRegion, Is.EqualTo("OR"));
			Assert.That(dto.ShipVia, Is.EqualTo(3));
			Assert.That(dto.ToString(), Is.EqualTo($"CustomerId=ALFKI, EmployeeId=9, Freight=27.5, OrderDate={orderDate}, OrderId=77, RequiredDate={requiredDate}, ShipAddress=1 Dock Way, ShipCity=Portland, ShipCountry=USA, ShipName=Speedy Express, ShippedDate={shippedDate}, ShipPostalCode=97201, ShipRegion=OR, ShipVia=3"));
		}
	}
}


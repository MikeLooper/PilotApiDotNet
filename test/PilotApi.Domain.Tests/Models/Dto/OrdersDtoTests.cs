using NUnit.Framework;
using PilotApi.Domain.Models.Dto;
using PilotApi.Domain.Tests.Testing.Utilities;
using System;
using System.Collections.Generic;

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

		[Test]
		public void OrdersDto_Validate_ValidValues_ReturnsNoErrors_Test()
		{
			// Arrange
			var dto = CreateValidDto();

			// Act
			var invalidMembers = ValidationTestUtilities.ValidateMemberNames(dto);

			// Assert
			Assert.That(invalidMembers, Is.Empty);
		}

		[TestCaseSource(nameof(BoundaryValueCases))]
		public void OrdersDto_Validate_BoundaryValue_ReturnsNoErrors_Test(string propertyName, Action<OrdersDto> applyValue)
		{
			// Arrange
			var dto = CreateValidDto();
			applyValue(dto);

			// Act
			var invalidMembers = ValidationTestUtilities.ValidateMemberNames(dto);

			// Assert
			Assert.That(invalidMembers, Is.Empty, propertyName);
		}

		[TestCaseSource(nameof(InvalidValueCases))]
		public void OrdersDto_Validate_InvalidValue_ReturnsPropertyError_Test(string propertyName, Action<OrdersDto> applyValue)
		{
			// Arrange
			var dto = CreateValidDto();
			applyValue(dto);

			// Act
			var invalidMembers = ValidationTestUtilities.ValidateMemberNames(dto);

			// Assert
			Assert.That(invalidMembers, Is.EqualTo(new[] { propertyName }));
		}

		private static IEnumerable<TestCaseData> BoundaryValueCases()
		{
			yield return new TestCaseData(nameof(OrdersDto.CustomerId), (Action<OrdersDto>)(dto => dto.CustomerId = null))
				.SetArgDisplayNames("CustomerId = null");
			yield return new TestCaseData(nameof(OrdersDto.Freight), (Action<OrdersDto>)(dto => dto.Freight = -922337203685477.5808m))
				.SetArgDisplayNames("Freight = -922337203685477.5808m");
			yield return new TestCaseData(nameof(OrdersDto.Freight), (Action<OrdersDto>)(dto => dto.Freight = null))
				.SetArgDisplayNames("Freight = null");
			yield return new TestCaseData(nameof(OrdersDto.ShipName), (Action<OrdersDto>)(dto => dto.ShipName = new string('x', 40)))
				.SetArgDisplayNames("ShipName = new string('x', 40)");
			yield return new TestCaseData(nameof(OrdersDto.ShipVia), (Action<OrdersDto>)(dto => dto.ShipVia = null))
				.SetArgDisplayNames("ShipVia = null");
		}

		private static IEnumerable<TestCaseData> InvalidValueCases()
		{
			yield return new TestCaseData(nameof(OrdersDto.CustomerId), (Action<OrdersDto>)(dto => dto.CustomerId = "ALFK"))
				.SetArgDisplayNames("CustomerId = ALFK");
			yield return new TestCaseData(nameof(OrdersDto.EmployeeId), (Action<OrdersDto>)(dto => dto.EmployeeId = 0))
				.SetArgDisplayNames("EmployeeId = 0");
			yield return new TestCaseData(nameof(OrdersDto.Freight), (Action<OrdersDto>)(dto => dto.Freight = 922337203685477.5808m))
				.SetArgDisplayNames("Freight = 922337203685477.5808m");
			yield return new TestCaseData(nameof(OrdersDto.OrderId), (Action<OrdersDto>)(dto => dto.OrderId = 0))
				.SetArgDisplayNames("OrderId = 0");
			yield return new TestCaseData(nameof(OrdersDto.ShipAddress), (Action<OrdersDto>)(dto => dto.ShipAddress = new string('x', 61)))
				.SetArgDisplayNames("ShipAddress = new string('x', 61)");
			yield return new TestCaseData(nameof(OrdersDto.ShipCity), (Action<OrdersDto>)(dto => dto.ShipCity = new string('x', 16)))
				.SetArgDisplayNames("ShipCity = new string('x', 16)");
			yield return new TestCaseData(nameof(OrdersDto.ShipCountry), (Action<OrdersDto>)(dto => dto.ShipCountry = new string('x', 16)))
				.SetArgDisplayNames("ShipCountry = new string('x', 16)");
			yield return new TestCaseData(nameof(OrdersDto.ShipName), (Action<OrdersDto>)(dto => dto.ShipName = new string('x', 41)))
				.SetArgDisplayNames("ShipName = new string('x', 41)");
			yield return new TestCaseData(nameof(OrdersDto.ShipPostalCode), (Action<OrdersDto>)(dto => dto.ShipPostalCode = new string('x', 11)))
				.SetArgDisplayNames("ShipPostalCode = new string('x', 11)");
			yield return new TestCaseData(nameof(OrdersDto.ShipRegion), (Action<OrdersDto>)(dto => dto.ShipRegion = new string('x', 16)))
				.SetArgDisplayNames("ShipRegion = new string('x', 16)");
			yield return new TestCaseData(nameof(OrdersDto.ShipVia), (Action<OrdersDto>)(dto => dto.ShipVia = 0))
				.SetArgDisplayNames("ShipVia = 0");
		}

		private static OrdersDto CreateValidDto()
		{
			return new OrdersDto
			{
				CustomerId = "ALFKI",
				EmployeeId = 9,
				Freight = 27.5m,
				OrderId = 77,
				ShipAddress = "1 Dock Way",
				ShipCity = "Portland",
				ShipCountry = "USA",
				ShipName = "Speedy Express",
				ShipPostalCode = "97201",
				ShipRegion = "OR",
				ShipVia = 3,
			};
		}
	}
}

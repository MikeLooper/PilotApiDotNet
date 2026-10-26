using NUnit.Framework;
using PilotApi.Domain.Models.Dto;
using PilotApi.Domain.Tests.Testing.Utilities;
using System;
using System.Collections.Generic;

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

		[Test]
		public void OrderDetailsDto_Validate_ValidValues_ReturnsNoErrors_Test()
		{
			// Arrange
			var dto = CreateValidDto();

			// Act
			var invalidMembers = ValidationTestUtilities.ValidateMemberNames(dto);

			// Assert
			Assert.That(invalidMembers, Is.Empty);
		}

		[TestCaseSource(nameof(BoundaryValueCases))]
		public void OrderDetailsDto_Validate_BoundaryValue_ReturnsNoErrors_Test(string propertyName, Action<OrderDetailsDto> applyValue)
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
		public void OrderDetailsDto_Validate_InvalidValue_ReturnsPropertyError_Test(string propertyName, Action<OrderDetailsDto> applyValue)
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
			yield return new TestCaseData(nameof(OrderDetailsDto.Discount), (Action<OrderDetailsDto>)(dto => dto.Discount = 0f))
				.SetArgDisplayNames("Discount = 0f");
			yield return new TestCaseData(nameof(OrderDetailsDto.Discount), (Action<OrderDetailsDto>)(dto => dto.Discount = 1f))
				.SetArgDisplayNames("Discount = 1f");
			yield return new TestCaseData(nameof(OrderDetailsDto.Quantity), (Action<OrderDetailsDto>)(dto => dto.Quantity = short.MaxValue))
				.SetArgDisplayNames("Quantity = short.MaxValue");
			yield return new TestCaseData(nameof(OrderDetailsDto.UnitPrice), (Action<OrderDetailsDto>)(dto => dto.UnitPrice = 0m))
				.SetArgDisplayNames("UnitPrice = 0m");
			yield return new TestCaseData(nameof(OrderDetailsDto.UnitPrice), (Action<OrderDetailsDto>)(dto => dto.UnitPrice = 922337203685477.5807m))
				.SetArgDisplayNames("UnitPrice = 922337203685477.5807m");
		}

		private static IEnumerable<TestCaseData> InvalidValueCases()
		{
			yield return new TestCaseData(nameof(OrderDetailsDto.Discount), (Action<OrderDetailsDto>)(dto => dto.Discount = -0.01f))
				.SetArgDisplayNames("Discount = -0.01f");
			yield return new TestCaseData(nameof(OrderDetailsDto.Discount), (Action<OrderDetailsDto>)(dto => dto.Discount = 1.01f))
				.SetArgDisplayNames("Discount = 1.01f");
			yield return new TestCaseData(nameof(OrderDetailsDto.OrderId), (Action<OrderDetailsDto>)(dto => dto.OrderId = 0))
				.SetArgDisplayNames("OrderId = 0");
			yield return new TestCaseData(nameof(OrderDetailsDto.ProductId), (Action<OrderDetailsDto>)(dto => dto.ProductId = 0))
				.SetArgDisplayNames("ProductId = 0");
			yield return new TestCaseData(nameof(OrderDetailsDto.Quantity), (Action<OrderDetailsDto>)(dto => dto.Quantity = 0))
				.SetArgDisplayNames("Quantity = 0");
			yield return new TestCaseData(nameof(OrderDetailsDto.UnitPrice), (Action<OrderDetailsDto>)(dto => dto.UnitPrice = -0.01m))
				.SetArgDisplayNames("UnitPrice = -0.01m");
		}

		private static OrderDetailsDto CreateValidDto()
		{
			return new OrderDetailsDto
			{
				Discount = 0.15f,
				OrderId = 12,
				ProductId = 34,
				Quantity = 5,
				UnitPrice = 19.95m,
			};
		}
	}
}

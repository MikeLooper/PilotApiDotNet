using NUnit.Framework;
using PilotApi.Domain.Models.Dto;
using PilotApi.Domain.Tests.Testing.Utilities;
using System;
using System.Collections.Generic;

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

		[Test]
		public void ShippersDto_Validate_ValidValues_ReturnsNoErrors_Test()
		{
			// Arrange
			var dto = CreateValidDto();

			// Act
			var invalidMembers = ValidationTestUtilities.ValidateMemberNames(dto);

			// Assert
			Assert.That(invalidMembers, Is.Empty);
		}

		[TestCaseSource(nameof(BoundaryValueCases))]
		public void ShippersDto_Validate_BoundaryValue_ReturnsNoErrors_Test(string propertyName, Action<ShippersDto> applyValue)
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
		public void ShippersDto_Validate_InvalidValue_ReturnsPropertyError_Test(string propertyName, Action<ShippersDto> applyValue)
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
			yield return new TestCaseData(nameof(ShippersDto.CompanyName), (Action<ShippersDto>)(dto => dto.CompanyName = new string('x', 40)))
				.SetArgDisplayNames("CompanyName = new string('x', 40)");
			yield return new TestCaseData(nameof(ShippersDto.Phone), (Action<ShippersDto>)(dto => dto.Phone = null))
				.SetArgDisplayNames("Phone = null");
			yield return new TestCaseData(nameof(ShippersDto.ShipperId), (Action<ShippersDto>)(dto => dto.ShipperId = null))
				.SetArgDisplayNames("ShipperId = null");
		}

		private static IEnumerable<TestCaseData> InvalidValueCases()
		{
			yield return new TestCaseData(nameof(ShippersDto.CompanyName), (Action<ShippersDto>)(dto => dto.CompanyName = string.Empty))
				.SetArgDisplayNames("CompanyName = string.Empty");
			yield return new TestCaseData(nameof(ShippersDto.CompanyName), (Action<ShippersDto>)(dto => dto.CompanyName = new string('x', 41)))
				.SetArgDisplayNames("CompanyName = new string('x', 41)");
			yield return new TestCaseData(nameof(ShippersDto.Phone), (Action<ShippersDto>)(dto => dto.Phone = new string('x', 25)))
				.SetArgDisplayNames("Phone = new string('x', 25)");
			yield return new TestCaseData(nameof(ShippersDto.ShipperId), (Action<ShippersDto>)(dto => dto.ShipperId = 0))
				.SetArgDisplayNames("ShipperId = 0");
		}

		private static ShippersDto CreateValidDto()
		{
			return new ShippersDto
			{
				CompanyName = "Speedy Express",
				Phone = "555-7777",
				ShipperId = 3,
			};
		}
	}
}

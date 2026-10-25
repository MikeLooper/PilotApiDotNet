using NUnit.Framework;
using PilotApi.Domain.Models.Dto;
using PilotApi.Domain.Tests.Testing.Utilities;
using System;
using System.Collections.Generic;

namespace PilotApi.Domain.Tests.Models.Dto
{
	[TestFixture]
	public class SuppliersDtoTests
	{
		[Test]
		public void SuppliersDto_Properties_RoundTripAndToStringReturnsExpectedValue_Test()
		{
			SuppliersDto dto = new()
			{
				Address = "500 Supply Ave",
				City = "Boise",
				CompanyName = "Exotic Liquids",
				ContactName = "Charlotte Cooper",
				ContactTitle = "Purchasing Manager",
				Country = "USA",
				Fax = "555-8888",
				HomePage = "https://example.test/supplier",
				Phone = "555-9999",
				PostalCode = "83702",
				Region = "ID",
				SupplierId = 11,
			};

			Assert.That(dto.Address, Is.EqualTo("500 Supply Ave"));
			Assert.That(dto.City, Is.EqualTo("Boise"));
			Assert.That(dto.CompanyName, Is.EqualTo("Exotic Liquids"));
			Assert.That(dto.ContactName, Is.EqualTo("Charlotte Cooper"));
			Assert.That(dto.ContactTitle, Is.EqualTo("Purchasing Manager"));
			Assert.That(dto.Country, Is.EqualTo("USA"));
			Assert.That(dto.Fax, Is.EqualTo("555-8888"));
			Assert.That(dto.HomePage, Is.EqualTo("https://example.test/supplier"));
			Assert.That(dto.Phone, Is.EqualTo("555-9999"));
			Assert.That(dto.PostalCode, Is.EqualTo("83702"));
			Assert.That(dto.Region, Is.EqualTo("ID"));
			Assert.That(dto.SupplierId, Is.EqualTo(11));
			Assert.That(dto.ToString(), Is.EqualTo("Address=500 Supply Ave, City=Boise, CompanyName=Exotic Liquids, ContactName=Charlotte Cooper, ContactTitle=Purchasing Manager, Country=USA, Fax=555-8888, HomePage=https://example.test/supplier, Phone=555-9999, PostalCode=83702, Region=ID, SupplierId=11"));
		}

		[Test]
		public void SuppliersDto_Validate_ValidValues_ReturnsNoErrors_Test()
		{
			// Arrange
			var dto = CreateValidDto();

			// Act
			var invalidMembers = ValidationTestUtilities.ValidateMemberNames(dto);

			// Assert
			Assert.That(invalidMembers, Is.Empty);
		}

		[TestCaseSource(nameof(BoundaryValueCases))]
		public void SuppliersDto_Validate_BoundaryValue_ReturnsNoErrors_Test(string propertyName, Action<SuppliersDto> applyValue)
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
		public void SuppliersDto_Validate_InvalidValue_ReturnsPropertyError_Test(string propertyName, Action<SuppliersDto> applyValue)
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
			yield return new TestCaseData(nameof(SuppliersDto.CompanyName), (Action<SuppliersDto>)(dto => dto.CompanyName = new string('x', 40)))
				.SetArgDisplayNames("CompanyName = new string('x', 40)");
			yield return new TestCaseData(nameof(SuppliersDto.HomePage), (Action<SuppliersDto>)(dto => dto.HomePage = new string('x', 500)))
				.SetArgDisplayNames("HomePage = new string('x', 500)");
			yield return new TestCaseData(nameof(SuppliersDto.SupplierId), (Action<SuppliersDto>)(dto => dto.SupplierId = null))
				.SetArgDisplayNames("SupplierId = null");
		}

		private static IEnumerable<TestCaseData> InvalidValueCases()
		{
			yield return new TestCaseData(nameof(SuppliersDto.Address), (Action<SuppliersDto>)(dto => dto.Address = new string('x', 61)))
				.SetArgDisplayNames("Address = new string('x', 61)");
			yield return new TestCaseData(nameof(SuppliersDto.City), (Action<SuppliersDto>)(dto => dto.City = new string('x', 16)))
				.SetArgDisplayNames("City = new string('x', 16)");
			yield return new TestCaseData(nameof(SuppliersDto.CompanyName), (Action<SuppliersDto>)(dto => dto.CompanyName = string.Empty))
				.SetArgDisplayNames("CompanyName = string.Empty");
			yield return new TestCaseData(nameof(SuppliersDto.CompanyName), (Action<SuppliersDto>)(dto => dto.CompanyName = new string('x', 41)))
				.SetArgDisplayNames("CompanyName = new string('x', 41)");
			yield return new TestCaseData(nameof(SuppliersDto.ContactName), (Action<SuppliersDto>)(dto => dto.ContactName = new string('x', 31)))
				.SetArgDisplayNames("ContactName = new string('x', 31)");
			yield return new TestCaseData(nameof(SuppliersDto.ContactTitle), (Action<SuppliersDto>)(dto => dto.ContactTitle = new string('x', 31)))
				.SetArgDisplayNames("ContactTitle = new string('x', 31)");
			yield return new TestCaseData(nameof(SuppliersDto.Country), (Action<SuppliersDto>)(dto => dto.Country = new string('x', 16)))
				.SetArgDisplayNames("Country = new string('x', 16)");
			yield return new TestCaseData(nameof(SuppliersDto.Fax), (Action<SuppliersDto>)(dto => dto.Fax = new string('x', 25)))
				.SetArgDisplayNames("Fax = new string('x', 25)");
			yield return new TestCaseData(nameof(SuppliersDto.Phone), (Action<SuppliersDto>)(dto => dto.Phone = new string('x', 25)))
				.SetArgDisplayNames("Phone = new string('x', 25)");
			yield return new TestCaseData(nameof(SuppliersDto.PostalCode), (Action<SuppliersDto>)(dto => dto.PostalCode = new string('x', 11)))
				.SetArgDisplayNames("PostalCode = new string('x', 11)");
			yield return new TestCaseData(nameof(SuppliersDto.Region), (Action<SuppliersDto>)(dto => dto.Region = new string('x', 16)))
				.SetArgDisplayNames("Region = new string('x', 16)");
			yield return new TestCaseData(nameof(SuppliersDto.SupplierId), (Action<SuppliersDto>)(dto => dto.SupplierId = 0))
				.SetArgDisplayNames("SupplierId = 0");
		}

		private static SuppliersDto CreateValidDto()
		{
			return new SuppliersDto
			{
				Address = "500 Supply Ave",
				City = "Boise",
				CompanyName = "Exotic Liquids",
				ContactName = "Charlotte Cooper",
				ContactTitle = "Purchasing Manager",
				Country = "USA",
				Fax = "555-8888",
				HomePage = "https://example.test/supplier",
				Phone = "555-9999",
				PostalCode = "83702",
				Region = "ID",
				SupplierId = 11,
			};
		}
	}
}

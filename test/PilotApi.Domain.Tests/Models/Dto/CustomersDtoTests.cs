using NUnit.Framework;
using PilotApi.Domain.Models.Dto;
using PilotApi.Domain.Tests.Testing.Utilities;
using System;
using System.Collections.Generic;

namespace PilotApi.Domain.Tests.Models.Dto
{
	[TestFixture]
	public class CustomersDtoTests
	{
		[Test]
		public void CustomersDto_Properties_RoundTripAndToStringReturnsExpectedValue_Test()
		{
			CustomersDto dto = new()
			{
				Address = "123 Main St",
				City = "Seattle",
				CompanyName = "Northwind",
				ContactName = "Jane Doe",
				ContactTitle = "Manager",
				Country = "USA",
				CustomerId = "ALFKI",
				Fax = "555-2222",
				Phone = "555-1111",
				PostalCode = "98101",
				Region = "WA",
			};

			Assert.That(dto.Address, Is.EqualTo("123 Main St"));
			Assert.That(dto.City, Is.EqualTo("Seattle"));
			Assert.That(dto.CompanyName, Is.EqualTo("Northwind"));
			Assert.That(dto.ContactName, Is.EqualTo("Jane Doe"));
			Assert.That(dto.ContactTitle, Is.EqualTo("Manager"));
			Assert.That(dto.Country, Is.EqualTo("USA"));
			Assert.That(dto.CustomerId, Is.EqualTo("ALFKI"));
			Assert.That(dto.Fax, Is.EqualTo("555-2222"));
			Assert.That(dto.Phone, Is.EqualTo("555-1111"));
			Assert.That(dto.PostalCode, Is.EqualTo("98101"));
			Assert.That(dto.Region, Is.EqualTo("WA"));
			Assert.That(dto.ToString(), Is.EqualTo("Address=123 Main St, City=Seattle, CompanyName=Northwind, ContactName=Jane Doe, ContactTitle=Manager, Country=USA, CustomerId=ALFKI, Fax=555-2222, Phone=555-1111, PostalCode=98101, Region=WA"));
		}

		[Test]
		public void CustomersDto_Validate_ValidValues_ReturnsNoErrors_Test()
		{
			// Arrange
			var dto = CreateValidDto();

			// Act
			var invalidMembers = ValidationTestUtilities.ValidateMemberNames(dto);

			// Assert
			Assert.That(invalidMembers, Is.Empty);
		}

		[TestCaseSource(nameof(BoundaryValueCases))]
		public void CustomersDto_Validate_BoundaryValue_ReturnsNoErrors_Test(string propertyName, Action<CustomersDto> applyValue)
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
		public void CustomersDto_Validate_InvalidValue_ReturnsPropertyError_Test(string propertyName, Action<CustomersDto> applyValue)
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
			yield return new TestCaseData(nameof(CustomersDto.Address), (Action<CustomersDto>)(dto => dto.Address = new string('x', 60)))
				.SetArgDisplayNames("Address = new string('x', 60)");
			yield return new TestCaseData(nameof(CustomersDto.CompanyName), (Action<CustomersDto>)(dto => dto.CompanyName = new string('x', 40)))
				.SetArgDisplayNames("CompanyName = new string('x', 40)");
			yield return new TestCaseData(nameof(CustomersDto.Region), (Action<CustomersDto>)(dto => dto.Region = null))
				.SetArgDisplayNames("Region = null");
		}

		private static IEnumerable<TestCaseData> InvalidValueCases()
		{
			yield return new TestCaseData(nameof(CustomersDto.Address), (Action<CustomersDto>)(dto => dto.Address = new string('x', 61)))
				.SetArgDisplayNames("Address = new string('x', 61)");
			yield return new TestCaseData(nameof(CustomersDto.City), (Action<CustomersDto>)(dto => dto.City = new string('x', 16)))
				.SetArgDisplayNames("City = new string('x', 16)");
			yield return new TestCaseData(nameof(CustomersDto.CompanyName), (Action<CustomersDto>)(dto => dto.CompanyName = string.Empty))
				.SetArgDisplayNames("CompanyName = string.Empty");
			yield return new TestCaseData(nameof(CustomersDto.CompanyName), (Action<CustomersDto>)(dto => dto.CompanyName = new string('x', 41)))
				.SetArgDisplayNames("CompanyName = new string('x', 41)");
			yield return new TestCaseData(nameof(CustomersDto.ContactName), (Action<CustomersDto>)(dto => dto.ContactName = new string('x', 31)))
				.SetArgDisplayNames("ContactName = new string('x', 31)");
			yield return new TestCaseData(nameof(CustomersDto.ContactTitle), (Action<CustomersDto>)(dto => dto.ContactTitle = new string('x', 31)))
				.SetArgDisplayNames("ContactTitle = new string('x', 31)");
			yield return new TestCaseData(nameof(CustomersDto.Country), (Action<CustomersDto>)(dto => dto.Country = new string('x', 16)))
				.SetArgDisplayNames("Country = new string('x', 16)");
			yield return new TestCaseData(nameof(CustomersDto.CustomerId), (Action<CustomersDto>)(dto => dto.CustomerId = "ALFK"))
				.SetArgDisplayNames("CustomerId = ALFK");
			yield return new TestCaseData(nameof(CustomersDto.CustomerId), (Action<CustomersDto>)(dto => dto.CustomerId = "ALFKIS"))
				.SetArgDisplayNames("CustomerId = ALFKIS");
			yield return new TestCaseData(nameof(CustomersDto.Fax), (Action<CustomersDto>)(dto => dto.Fax = new string('x', 25)))
				.SetArgDisplayNames("Fax = new string('x', 25)");
			yield return new TestCaseData(nameof(CustomersDto.Phone), (Action<CustomersDto>)(dto => dto.Phone = new string('x', 25)))
				.SetArgDisplayNames("Phone = new string('x', 25)");
			yield return new TestCaseData(nameof(CustomersDto.PostalCode), (Action<CustomersDto>)(dto => dto.PostalCode = new string('x', 11)))
				.SetArgDisplayNames("PostalCode = new string('x', 11)");
			yield return new TestCaseData(nameof(CustomersDto.Region), (Action<CustomersDto>)(dto => dto.Region = new string('x', 16)))
				.SetArgDisplayNames("Region = new string('x', 16)");
		}

		private static CustomersDto CreateValidDto()
		{
			return new CustomersDto
			{
				Address = "123 Main St",
				City = "Seattle",
				CompanyName = "Northwind",
				ContactName = "Jane Doe",
				ContactTitle = "Manager",
				Country = "USA",
				CustomerId = "ALFKI",
				Fax = "555-2222",
				Phone = "555-1111",
				PostalCode = "98101",
				Region = "WA",
			};
		}
	}
}

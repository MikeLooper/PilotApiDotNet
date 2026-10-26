using NUnit.Framework;
using PilotApi.Domain.Models.Dto;
using PilotApi.Domain.Tests.Testing.Utilities;
using PilotApi.Shared.Constants;
using System;
using System.Collections.Generic;

namespace PilotApi.Domain.Tests.Models.Dto
{
	[TestFixture]
	public class EmployeesDtoTests
	{
		[Test]
		public void EmployeesDto_ToString_ReturnsExpectedValue_WhenPhotoIsNull_Test()
		{
			DateTime birthDate = new(1980, 1, 2, 3, 4, 5);
			DateTime hireDate = new(2010, 6, 7, 8, 9, 10);
			EmployeesDto dto = new()
			{
				Address = "456 Market St",
				BirthDate = birthDate,
				City = "Tacoma",
				Country = "USA",
				EmployeeId = 5,
				Extension = "123",
				FirstName = "Nancy",
				HireDate = hireDate,
				HomePhone = "555-3333",
				LastName = "Davolio",
				Notes = "Top performer",
				Photo = null,
				PhotoPath = "/photos/nancy.jpg",
				PostalCode = "98402",
				Region = "WA",
				ReportsTo = 1,
				Title = "Sales Representative",
				TitleOfCourtesy = "Ms.",
			};

			Assert.That(dto.ToString(), Is.EqualTo($"Address=456 Market St, BirthDate={birthDate}, City=Tacoma, Country=USA, EmployeeId=5, Extension=123, FirstName=Nancy, HireDate={hireDate}, HomePhone=555-3333, LastName=Davolio, Notes=Top performer, Photo={StringConstants.LogNull}, PhotoPath=/photos/nancy.jpg, PostalCode=98402, Region=WA, ReportsTo=1, Title=Sales Representative, TitleOfCourtesy=Ms."));
		}

		[Test]
		public void EmployeesDto_Properties_RoundTripAndToStringUsesHasContents_WhenPhotoHasData_Test()
		{
			DateTime birthDate = new(1975, 4, 5);
			DateTime hireDate = new(2001, 7, 8);
			byte[] photo = [8, 9, 10];
			EmployeesDto dto = new()
			{
				Address = "789 Pine St",
				BirthDate = birthDate,
				City = "Redmond",
				Country = "USA",
				EmployeeId = 7,
				Extension = "456",
				FirstName = "Andrew",
				HireDate = hireDate,
				HomePhone = "555-4444",
				LastName = "Fuller",
				Notes = "Regional lead",
				Photo = photo,
				PhotoPath = "/photos/andrew.jpg",
				PostalCode = "98052",
				Region = "WA",
				ReportsTo = 2,
				Title = "Vice President",
				TitleOfCourtesy = "Dr.",
			};

			Assert.That(dto.Address, Is.EqualTo("789 Pine St"));
			Assert.That(dto.BirthDate, Is.EqualTo(birthDate));
			Assert.That(dto.City, Is.EqualTo("Redmond"));
			Assert.That(dto.Country, Is.EqualTo("USA"));
			Assert.That(dto.EmployeeId, Is.EqualTo(7));
			Assert.That(dto.Extension, Is.EqualTo("456"));
			Assert.That(dto.FirstName, Is.EqualTo("Andrew"));
			Assert.That(dto.HireDate, Is.EqualTo(hireDate));
			Assert.That(dto.HomePhone, Is.EqualTo("555-4444"));
			Assert.That(dto.LastName, Is.EqualTo("Fuller"));
			Assert.That(dto.Notes, Is.EqualTo("Regional lead"));
			Assert.That(dto.Photo, Is.SameAs(photo));
			Assert.That(dto.PhotoPath, Is.EqualTo("/photos/andrew.jpg"));
			Assert.That(dto.PostalCode, Is.EqualTo("98052"));
			Assert.That(dto.Region, Is.EqualTo("WA"));
			Assert.That(dto.ReportsTo, Is.EqualTo(2));
			Assert.That(dto.Title, Is.EqualTo("Vice President"));
			Assert.That(dto.TitleOfCourtesy, Is.EqualTo("Dr."));
			Assert.That(dto.ToString(), Does.Contain($"Photo={StringConstants.HasContents}"));
		}

		[Test]
		public void EmployeesDto_Validate_ValidValues_ReturnsNoErrors_Test()
		{
			// Arrange
			var dto = CreateValidDto();

			// Act
			var invalidMembers = ValidationTestUtilities.ValidateMemberNames(dto);

			// Assert
			Assert.That(invalidMembers, Is.Empty);
		}

		[TestCaseSource(nameof(BoundaryValueCases))]
		public void EmployeesDto_Validate_BoundaryValue_ReturnsNoErrors_Test(string propertyName, Action<EmployeesDto> applyValue)
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
		public void EmployeesDto_Validate_InvalidValue_ReturnsPropertyError_Test(string propertyName, Action<EmployeesDto> applyValue)
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
			yield return new TestCaseData(nameof(EmployeesDto.FirstName), (Action<EmployeesDto>)(dto => dto.FirstName = new string('x', 10)))
				.SetArgDisplayNames("FirstName = new string('x', 10)");
			yield return new TestCaseData(nameof(EmployeesDto.LastName), (Action<EmployeesDto>)(dto => dto.LastName = new string('x', 20)))
				.SetArgDisplayNames("LastName = new string('x', 20)");
			yield return new TestCaseData(nameof(EmployeesDto.PhotoPath), (Action<EmployeesDto>)(dto => dto.PhotoPath = new string('x', 255)))
				.SetArgDisplayNames("PhotoPath = new string('x', 255)");
			yield return new TestCaseData(nameof(EmployeesDto.ReportsTo), (Action<EmployeesDto>)(dto => dto.ReportsTo = null))
				.SetArgDisplayNames("ReportsTo = null");
		}

		private static IEnumerable<TestCaseData> InvalidValueCases()
		{
			yield return new TestCaseData(nameof(EmployeesDto.Address), (Action<EmployeesDto>)(dto => dto.Address = new string('x', 61)))
				.SetArgDisplayNames("Address = new string('x', 61)");
			yield return new TestCaseData(nameof(EmployeesDto.City), (Action<EmployeesDto>)(dto => dto.City = new string('x', 16)))
				.SetArgDisplayNames("City = new string('x', 16)");
			yield return new TestCaseData(nameof(EmployeesDto.Country), (Action<EmployeesDto>)(dto => dto.Country = new string('x', 16)))
				.SetArgDisplayNames("Country = new string('x', 16)");
			yield return new TestCaseData(nameof(EmployeesDto.EmployeeId), (Action<EmployeesDto>)(dto => dto.EmployeeId = 0))
				.SetArgDisplayNames("EmployeeId = 0");
			yield return new TestCaseData(nameof(EmployeesDto.Extension), (Action<EmployeesDto>)(dto => dto.Extension = new string('x', 5)))
				.SetArgDisplayNames("Extension = new string('x', 5)");
			yield return new TestCaseData(nameof(EmployeesDto.FirstName), (Action<EmployeesDto>)(dto => dto.FirstName = string.Empty))
				.SetArgDisplayNames("FirstName = string.Empty");
			yield return new TestCaseData(nameof(EmployeesDto.FirstName), (Action<EmployeesDto>)(dto => dto.FirstName = new string('x', 11)))
				.SetArgDisplayNames("FirstName = new string('x', 11)");
			yield return new TestCaseData(nameof(EmployeesDto.HomePhone), (Action<EmployeesDto>)(dto => dto.HomePhone = new string('x', 25)))
				.SetArgDisplayNames("HomePhone = new string('x', 25)");
			yield return new TestCaseData(nameof(EmployeesDto.LastName), (Action<EmployeesDto>)(dto => dto.LastName = string.Empty))
				.SetArgDisplayNames("LastName = string.Empty");
			yield return new TestCaseData(nameof(EmployeesDto.LastName), (Action<EmployeesDto>)(dto => dto.LastName = new string('x', 21)))
				.SetArgDisplayNames("LastName = new string('x', 21)");
			yield return new TestCaseData(nameof(EmployeesDto.PhotoPath), (Action<EmployeesDto>)(dto => dto.PhotoPath = new string('x', 256)))
				.SetArgDisplayNames("PhotoPath = new string('x', 256)");
			yield return new TestCaseData(nameof(EmployeesDto.PostalCode), (Action<EmployeesDto>)(dto => dto.PostalCode = new string('x', 11)))
				.SetArgDisplayNames("PostalCode = new string('x', 11)");
			yield return new TestCaseData(nameof(EmployeesDto.Region), (Action<EmployeesDto>)(dto => dto.Region = new string('x', 16)))
				.SetArgDisplayNames("Region = new string('x', 16)");
			yield return new TestCaseData(nameof(EmployeesDto.ReportsTo), (Action<EmployeesDto>)(dto => dto.ReportsTo = 0))
				.SetArgDisplayNames("ReportsTo = 0");
			yield return new TestCaseData(nameof(EmployeesDto.Title), (Action<EmployeesDto>)(dto => dto.Title = new string('x', 31)))
				.SetArgDisplayNames("Title = new string('x', 31)");
			yield return new TestCaseData(nameof(EmployeesDto.TitleOfCourtesy), (Action<EmployeesDto>)(dto => dto.TitleOfCourtesy = new string('x', 26)))
				.SetArgDisplayNames("TitleOfCourtesy = new string('x', 26)");
		}

		private static EmployeesDto CreateValidDto()
		{
			return new EmployeesDto
			{
				Address = "456 Market St",
				City = "Tacoma",
				Country = "USA",
				EmployeeId = 5,
				Extension = "123",
				FirstName = "Nancy",
				HomePhone = "555-3333",
				LastName = "Davolio",
				PhotoPath = "/photos/nancy.jpg",
				PostalCode = "98402",
				Region = "WA",
				ReportsTo = 1,
				Title = "Sales Representative",
				TitleOfCourtesy = "Ms.",
			};
		}
	}
}

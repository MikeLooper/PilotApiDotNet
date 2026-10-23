using NUnit.Framework;
using PilotApi.Domain.Models.Dto;
using PilotApi.Shared.Constants;
using System;

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
	}
}


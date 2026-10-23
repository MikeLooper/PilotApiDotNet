using NUnit.Framework;
using PilotApi.Domain.Models.Dto;

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
	}
}


using NUnit.Framework;
using PilotApi.Domain.Models.Dto;

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
	}
}


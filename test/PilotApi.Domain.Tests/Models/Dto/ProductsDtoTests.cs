using NUnit.Framework;
using PilotApi.Domain.Models.Dto;

namespace PilotApi.Domain.Tests.Models.Dto
{
	[TestFixture]
	public class ProductsDtoTests
	{
		[Test]
		public void ProductsDto_Properties_RoundTripAndToStringReturnsExpectedValue_Test()
		{
			ProductsDto dto = new()
			{
				CategoryId = 4,
				Discontinued = true,
				ProductId = 18,
				ProductName = "Chef Anton's Cajun Seasoning",
				QuantityPerUnit = "48 - 6 oz jars",
				ReorderLevel = 10,
				SupplierId = 2,
				UnitPrice = 22.5m,
				UnitsInStock = 53,
				UnitsOnOrder = 0,
			};

			Assert.That(dto.CategoryId, Is.EqualTo(4));
			Assert.That(dto.Discontinued, Is.True);
			Assert.That(dto.ProductId, Is.EqualTo(18));
			Assert.That(dto.ProductName, Is.EqualTo("Chef Anton's Cajun Seasoning"));
			Assert.That(dto.QuantityPerUnit, Is.EqualTo("48 - 6 oz jars"));
			Assert.That(dto.ReorderLevel, Is.EqualTo(10));
			Assert.That(dto.SupplierId, Is.EqualTo(2));
			Assert.That(dto.UnitPrice, Is.EqualTo(22.5m));
			Assert.That(dto.UnitsInStock, Is.EqualTo(53));
			Assert.That(dto.UnitsOnOrder, Is.EqualTo(0));
			Assert.That(dto.ToString(), Is.EqualTo("CategoryId=4, Discontinued=True, ProductId=18, ProductName=Chef Anton's Cajun Seasoning, QuantityPerUnit=48 - 6 oz jars, ReorderLevel=10, SupplierId=2, UnitPrice=22.5, UnitsInStock=53, UnitsOnOrder=0"));
		}
	}
}


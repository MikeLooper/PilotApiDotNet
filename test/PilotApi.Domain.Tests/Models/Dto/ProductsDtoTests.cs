using NUnit.Framework;
using PilotApi.Domain.Models.Dto;
using PilotApi.Domain.Tests.Testing.Utilities;
using System;
using System.Collections.Generic;

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

		[Test]
		public void ProductsDto_Validate_ValidValues_ReturnsNoErrors_Test()
		{
			// Arrange
			var dto = CreateValidDto();

			// Act
			var invalidMembers = ValidationTestUtilities.ValidateMemberNames(dto);

			// Assert
			Assert.That(invalidMembers, Is.Empty);
		}

		[TestCaseSource(nameof(BoundaryValueCases))]
		public void ProductsDto_Validate_BoundaryValue_ReturnsNoErrors_Test(string propertyName, Action<ProductsDto> applyValue)
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
		public void ProductsDto_Validate_InvalidValue_ReturnsPropertyError_Test(string propertyName, Action<ProductsDto> applyValue)
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
			yield return new TestCaseData(nameof(ProductsDto.ProductName), (Action<ProductsDto>)(dto => dto.ProductName = new string('x', 40)))
				.SetArgDisplayNames("ProductName = new string('x', 40)");
			yield return new TestCaseData(nameof(ProductsDto.ReorderLevel), (Action<ProductsDto>)(dto => dto.ReorderLevel = null))
				.SetArgDisplayNames("ReorderLevel = null");
			yield return new TestCaseData(nameof(ProductsDto.UnitsInStock), (Action<ProductsDto>)(dto => dto.UnitsInStock = null))
				.SetArgDisplayNames("UnitsInStock = null");
			yield return new TestCaseData(nameof(ProductsDto.UnitsOnOrder), (Action<ProductsDto>)(dto => dto.UnitsOnOrder = null))
				.SetArgDisplayNames("UnitsOnOrder = null");
			yield return new TestCaseData(nameof(ProductsDto.UnitsInStock), (Action<ProductsDto>)(dto => dto.UnitsInStock = short.MaxValue))
				.SetArgDisplayNames("UnitsInStock = short.MaxValue");
		}

		private static IEnumerable<TestCaseData> InvalidValueCases()
		{
			yield return new TestCaseData(nameof(ProductsDto.CategoryId), (Action<ProductsDto>)(dto => dto.CategoryId = 0))
				.SetArgDisplayNames("CategoryId = 0");
			yield return new TestCaseData(nameof(ProductsDto.ProductId), (Action<ProductsDto>)(dto => dto.ProductId = 0))
				.SetArgDisplayNames("ProductId = 0");
			yield return new TestCaseData(nameof(ProductsDto.ProductName), (Action<ProductsDto>)(dto => dto.ProductName = string.Empty))
				.SetArgDisplayNames("ProductName = string.Empty");
			yield return new TestCaseData(nameof(ProductsDto.ProductName), (Action<ProductsDto>)(dto => dto.ProductName = new string('x', 41)))
				.SetArgDisplayNames("ProductName = new string('x', 41)");
			yield return new TestCaseData(nameof(ProductsDto.QuantityPerUnit), (Action<ProductsDto>)(dto => dto.QuantityPerUnit = new string('x', 21)))
				.SetArgDisplayNames("QuantityPerUnit = new string('x', 21)");
			yield return new TestCaseData(nameof(ProductsDto.ReorderLevel), (Action<ProductsDto>)(dto => dto.ReorderLevel = (short)-1))
				.SetArgDisplayNames("ReorderLevel = (short)-1");
			yield return new TestCaseData(nameof(ProductsDto.SupplierId), (Action<ProductsDto>)(dto => dto.SupplierId = 0))
				.SetArgDisplayNames("SupplierId = 0");
			yield return new TestCaseData(nameof(ProductsDto.UnitPrice), (Action<ProductsDto>)(dto => dto.UnitPrice = -0.01m))
				.SetArgDisplayNames("UnitPrice = -0.01m");
			yield return new TestCaseData(nameof(ProductsDto.UnitsInStock), (Action<ProductsDto>)(dto => dto.UnitsInStock = (short)-1))
				.SetArgDisplayNames("UnitsInStock = (short)-1");
			yield return new TestCaseData(nameof(ProductsDto.UnitsOnOrder), (Action<ProductsDto>)(dto => dto.UnitsOnOrder = (short)-1))
				.SetArgDisplayNames("UnitsOnOrder = (short)-1");
		}

		private static ProductsDto CreateValidDto()
		{
			return new ProductsDto
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
		}
	}
}

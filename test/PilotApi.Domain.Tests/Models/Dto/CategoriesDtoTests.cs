using NUnit.Framework;
using PilotApi.Domain.Models.Dto;
using PilotApi.Domain.Tests.Testing.Utilities;
using PilotApi.Shared.Constants;
using System;
using System.Collections.Generic;

namespace PilotApi.Domain.Tests.Models.Dto
{
	[TestFixture]
	public class CategoriesDtoTests
	{
		[Test]
		public void CategoriesDto_ToString_ReturnsExpectedValue_WhenPictureIsNull_Test()
		{
			CategoriesDto dto = new()
			{
				CategoryId = 1,
				CategoryName = "Beverages",
				Description = "Soft drinks",
				Picture = null,
			};

			Assert.That(dto.ToString(), Is.EqualTo($"CategoryId=1, CategoryName=Beverages, Description=Soft drinks, Picture={StringConstants.LogNull}"));
		}

		[Test]
		public void CategoriesDto_Properties_RoundTripAndToStringUsesHasContents_WhenPictureHasData_Test()
		{
			byte[] picture = [1, 2, 3];
			CategoriesDto dto = new()
			{
				CategoryId = 2,
				CategoryName = "Condiments",
				Description = "Sauces",
				Picture = picture,
			};

			Assert.That(dto.CategoryId, Is.EqualTo(2));
			Assert.That(dto.CategoryName, Is.EqualTo("Condiments"));
			Assert.That(dto.Description, Is.EqualTo("Sauces"));
			Assert.That(dto.Picture, Is.SameAs(picture));
			Assert.That(dto.ToString(), Is.EqualTo($"CategoryId=2, CategoryName=Condiments, Description=Sauces, Picture={StringConstants.HasContents}"));
		}

		[Test]
		public void CategoriesDto_Validate_ValidValues_ReturnsNoErrors_Test()
		{
			// Arrange
			var dto = CreateValidDto();

			// Act
			var invalidMembers = ValidationTestUtilities.ValidateMemberNames(dto);

			// Assert
			Assert.That(invalidMembers, Is.Empty);
		}

		[TestCaseSource(nameof(BoundaryValueCases))]
		public void CategoriesDto_Validate_BoundaryValue_ReturnsNoErrors_Test(string propertyName, Action<CategoriesDto> applyValue)
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
		public void CategoriesDto_Validate_InvalidValue_ReturnsPropertyError_Test(string propertyName, Action<CategoriesDto> applyValue)
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
			yield return new TestCaseData(nameof(CategoriesDto.CategoryId), (Action<CategoriesDto>)(dto => dto.CategoryId = int.MaxValue))
				.SetArgDisplayNames("CategoryId = int.MaxValue");
			yield return new TestCaseData(nameof(CategoriesDto.CategoryId), (Action<CategoriesDto>)(dto => dto.CategoryId = null))
				.SetArgDisplayNames("CategoryId = null");
			yield return new TestCaseData(nameof(CategoriesDto.CategoryName), (Action<CategoriesDto>)(dto => dto.CategoryName = new string('x', 15)))
				.SetArgDisplayNames("CategoryName = new string('x', 15)");
		}

		private static IEnumerable<TestCaseData> InvalidValueCases()
		{
			yield return new TestCaseData(nameof(CategoriesDto.CategoryId), (Action<CategoriesDto>)(dto => dto.CategoryId = 0))
				.SetArgDisplayNames("CategoryId = 0");
			yield return new TestCaseData(nameof(CategoriesDto.CategoryName), (Action<CategoriesDto>)(dto => dto.CategoryName = string.Empty))
				.SetArgDisplayNames("CategoryName = string.Empty");
			yield return new TestCaseData(nameof(CategoriesDto.CategoryName), (Action<CategoriesDto>)(dto => dto.CategoryName = new string('x', 16)))
				.SetArgDisplayNames("CategoryName = new string('x', 16)");
		}

		private static CategoriesDto CreateValidDto()
		{
			return new CategoriesDto
			{
				CategoryId = 1,
				CategoryName = "Beverages",
				Description = "Soft drinks",
				Picture = null,
			};
		}
	}
}

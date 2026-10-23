using NUnit.Framework;
using PilotApi.Domain.Models.Dto;
using PilotApi.Shared.Constants;

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
	}
}


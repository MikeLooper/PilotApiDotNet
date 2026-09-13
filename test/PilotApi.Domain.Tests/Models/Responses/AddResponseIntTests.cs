using NUnit.Framework;
using PilotApi.Domain.Models.Responses;

namespace PilotApi.Domain.Tests.Models.Responses
{
	[TestFixture]
	public class AddResponseIntTests
	{
		[Test]
		public void AddResponseInt_DefaultConstructor_InitializesIdToZero_Test()
		{
			AddResponseInt response = new();

			Assert.That(response.Id, Is.EqualTo(0));
		}

		[Test]
		public void AddResponseInt_Constructor_SetsId_Test()
		{
			AddResponseInt response = new(42);

			Assert.That(response.Id, Is.EqualTo(42));
		}

		[Test]
		public void AddResponseInt_IdProperty_CanBeUpdated_Test()
		{
			AddResponseInt response = new();

			response.Id = 99;

			Assert.That(response.Id, Is.EqualTo(99));
		}
	}
}


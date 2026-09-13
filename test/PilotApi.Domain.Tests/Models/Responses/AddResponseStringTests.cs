using NUnit.Framework;
using PilotApi.Domain.Models.Responses;

namespace PilotApi.Domain.Tests.Models.Responses
{
	[TestFixture]
	public class AddResponseStringTests
	{
		[Test]
		public void AddResponseString_DefaultConstructor_LeavesIdNull_Test()
		{
			AddResponseString response = new();

			Assert.That(response.Id, Is.Null);
		}

		[Test]
		public void AddResponseString_Constructor_SetsId_Test()
		{
			AddResponseString response = new("ALFKI");

			Assert.That(response.Id, Is.EqualTo("ALFKI"));
		}

		[Test]
		public void AddResponseString_IdProperty_CanBeUpdated_Test()
		{
			AddResponseString response = new();

			response.Id = "BONAP";

			Assert.That(response.Id, Is.EqualTo("BONAP"));
		}
	}
}


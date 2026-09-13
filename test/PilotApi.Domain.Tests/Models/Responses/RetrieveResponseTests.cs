using NUnit.Framework;
using PilotApi.Domain.Models.Responses;

namespace PilotApi.Domain.Tests.Models.Responses
{
	[TestFixture]
	public class RetrieveResponseTests
	{
		[Test]
		public void RetrieveResponse_DefaultConstructor_InitializesWithoutError_Test()
		{
			RetrieveResponse<string> response = new();

			Assert.That(response.Result, Is.Null);
			Assert.That(response.ErrorMessage, Is.Null);
			Assert.That(response.IsError, Is.False);
		}

		[Test]
		public void RetrieveResponse_Constructor_SetsResultAndLeavesIsErrorFalse_WhenErrorMessageIsNull_Test()
		{
			RetrieveResponse<int> response = new(15);

			Assert.That(response.Result, Is.EqualTo(15));
			Assert.That(response.ErrorMessage, Is.Null);
			Assert.That(response.IsError, Is.False);
		}

		[Test]
		public void RetrieveResponse_Constructor_SetsErrorState_WhenErrorMessageHasContent_Test()
		{
			RetrieveResponse<string> response = new("value", "failure");

			Assert.That(response.Result, Is.EqualTo("value"));
			Assert.That(response.ErrorMessage, Is.EqualTo("failure"));
			Assert.That(response.IsError, Is.True);
		}

		[Test]
		public void RetrieveResponse_IsError_ReturnsFalse_WhenErrorMessageIsWhitespace_Test()
		{
			RetrieveResponse<string> response = new()
			{
				Result = "value",
				ErrorMessage = "   ",
			};

			Assert.That(response.IsError, Is.False);
		}
	}
}


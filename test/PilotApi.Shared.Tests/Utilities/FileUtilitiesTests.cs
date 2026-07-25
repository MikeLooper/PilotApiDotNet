using NUnit.Framework;
using PilotApi.Shared.Utilities;

namespace PilotApi.Shared.Tests.Utilities
{
	[TestFixture]
	public class FileUtilitiesTests
	{
		[Test]
		public void FileUtilities_GetApplicationVersion_ReturnsValue_Test()
		{
			// Arrange

			// Act
			var result = FileUtilities.GetApplicationVersion();

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.Not.Empty);
		}
	}
}

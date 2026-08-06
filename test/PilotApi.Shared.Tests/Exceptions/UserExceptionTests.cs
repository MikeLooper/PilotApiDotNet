using NUnit.Framework;
using PilotApi.Shared.Exceptions;
using System;
using System.IO;

namespace PilotApi.Shared.Tests.Exceptions
{
	[TestFixture]
	public class UserExceptionTests
	{
		[Test]
		public void UserException_Constructor_Default_ShouldCreateValidException_Test()
		{
			// Arrange & Act
			var exception = new UserException();

			// Assert
			Assert.NotNull(exception);
			Assert.IsInstanceOf<Exception>(exception);
			Assert.That(exception.Message, Is.EqualTo("Exception of type 'PilotApi.Shared.Exceptions.UserException' was thrown."));
		}

		[Test]
		public void UserException_Constructor_WithMessage_ShouldIncludeMessage_Test()
		{
			// Arrange
			var message = "Test user error";

			// Act
			var exception = new UserException(message);

			// Assert
			Assert.NotNull(exception);
			Assert.That(exception.Message, Is.EqualTo(message));
		}

		[Test]
		public void UserException_Constructor_WithMessageAndInnerException_ShouldIncludeBoth_Test()
		{
			// Arrange
			var message = "Outer exception";
			var innerException = new InvalidOperationException("Inner exception");

			// Act
			var exception = new UserException(message, innerException);

			// Assert
			Assert.NotNull(exception);
			Assert.That(exception.Message, Is.EqualTo(message));
			Assert.That(exception.InnerException, Is.SameAs(innerException));
			Assert.That(exception.InnerException?.Message, Is.EqualTo("Inner exception"));
		}

		[Test]
		public void UserException_Exception_ShouldBeSerializable_Test()
		{
			// Arrange
			var exception = new UserException("Test message");

			// Act
			var isSerializable = exception.GetType().IsSerializable;

			// Assert
			Assert.That(isSerializable, Is.True);
		}

		[Test]
		public void UserException_Exception_ShouldHaveSerializableAttribute_Test()
		{
			// Arrange & Act
			var attribute = typeof(UserException).GetCustomAttributes(typeof(SerializableAttribute), false);

			// Assert
			Assert.That(attribute.Length, Is.EqualTo(1));
		}

		[Test]
		public void UserException_ToString_ShouldReturnFormattedString_Test()
		{
			// Arrange
			var message = "User friendly message";
			var exception = new UserException(message, new IOException("File not found"));

			// Act
			var result = exception.ToString();

			// Assert
			Assert.NotNull(result);
			Assert.That(result, Does.Contain("UserException"));
			Assert.That(result, Does.Contain(message));
			Assert.That(result, Does.Contain("IOException"));
		}
	}
}

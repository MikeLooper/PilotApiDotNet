using System;

namespace PilotApi.Shared.Exceptions
{
	/// <summary>
	/// A custom exception for reporting to the user that an exception was caught and logged.
	/// The message should be user-friendly and not contain any sensitive information - including stacktraces, 
	/// class names, or other internal information.
	/// The message should be suitable for display to the user.
	/// </summary>
	[Serializable]
	public class UserException : Exception
	{
		/// <summary>
		/// Instantiate a <see cref="UserException"/> object.
		/// </summary>
		public UserException()
		{
		}

		/// <summary>
		/// Instantiate a <see cref="UserException"/> object.
		/// </summary>
		/// <param name="message">
		/// A message to include in the exception.
		/// </param>
		public UserException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Instantiate a <see cref="UserException"/> object.
		/// </summary>
		/// <param name="message">
		/// A message to include in the exception.
		/// </param>
		/// <param name="innerException">
		/// An inner exception to include in the exception.
		/// </param>
		public UserException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}

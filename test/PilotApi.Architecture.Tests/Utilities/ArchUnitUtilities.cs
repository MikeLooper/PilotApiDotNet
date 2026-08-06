using ArchUnitNET.Domain;
using ArchUnitNET.Fluent.Syntax.Elements;
using System;
using System.Text;

namespace PilotApi.Architecture.Tests.Utilities
{
	/// <summary>
	/// Utility methods used with ArchUnit code.
	/// </summary>
	public static class ArchUnitUtilities
	{
		/// <summary>
		/// Prints the classes in the given collection to the console.
		/// </summary>
		/// <typeparam name="TThat">
		/// The type of the "that" object provider.
		/// </typeparam>
		/// <typeparam name="TShould">
		/// The type of the "should" object provider.
		/// </typeparam>
		/// <typeparam name="TDescription">
		/// The type of the "description" object provider.
		/// </typeparam>
		/// <typeparam name="TType">
		/// The type of the objects being analyzed.
		/// </typeparam>
		/// <param name="collection">
		/// The collection of objects to print.
		/// </param>
		/// <param name="architecture">
		/// The architecture context.
		/// </param>
		/// <returns>
		/// A string description.
		/// If the supplied collection is null, null is returned.
		/// </returns>
		/// <example>
		/// <code>
		/// var tableClasses = Classes()
		///						.That()
		///						.AreAssignableTo(typeof(ControllerBase))
		///						.Or()
		///						.AreAssignableTo(typeof(Controller))
		///						.And()
		///						.AreNotAbstract()
		///						
		/// var description = ArchUnitUtilities.DescribeCollection(tableClasses, myArchitecture);
		/// </code>
		/// </example>
		public static string? DescribeCollection<TThat, TShould, TDescription, TType>(
			GivenObjectsConjunction<TThat, TShould, TDescription, TType> collection,
			ArchUnitNET.Domain.Architecture architecture)
			//where TThat : IObjectProvider<TType>
			//where TShould : IObjectProvider<TType>
			//where TDescription : IObjectProvider<TType>
			where TType : ICanBeAnalyzed
		{
			if (architecture == null)
			{
				throw new ArgumentException($"Invalid argument: {nameof(architecture)}");
			};

			if (collection == null)
			{
				return null;
			};

			var description = new StringBuilder();
			description.Append($"Printing classes for: {collection.Description}");

			var filteredClasses = collection.GetObjects(architecture);
			foreach ( var clazz in filteredClasses )
			{
				description.AppendLine($"- {clazz.FullName}");
			}

			return description.ToString();
		}

		/// <summary>
		/// Prints the classes in the given collection to the console.
		/// </summary>
		/// <typeparam name="TThat">
		/// The type of the "that" object provider.
		/// </typeparam>
		/// <typeparam name="TShould">
		/// The type of the "should" object provider.
		/// </typeparam>
		/// <typeparam name="TDescription">
		/// The type of the "description" object provider.
		/// </typeparam>
		/// <typeparam name="TType">
		/// The type of the objects being analyzed.
		/// </typeparam>
		/// <param name="collection">
		/// The collection of objects to print.
		/// </param>
		/// <param name="architecture">
		/// The architecture context.
		/// </param>
		/// <example>
		/// <code>
		/// var tableClasses = Classes()
		///						.That()
		///						.AreAssignableTo(typeof(ControllerBase))
		///						.Or()
		///						.AreAssignableTo(typeof(Controller))
		///						.And()
		///						.AreNotAbstract()
		///						
		/// ArchUnitUtilities.PrintCollection(tableClasses, myArchitecture);
		/// </code>
		/// </example>
		public static void PrintCollection<TThat, TShould, TDescription, TType>(
			GivenObjectsConjunction<TThat, TShould, TDescription, TType> collection,
			ArchUnitNET.Domain.Architecture architecture)
			//where TThat : IObjectProvider<TType>
			//where TShould : IObjectProvider<TType>
			//where TDescription : IObjectProvider<TType>
			where TType : ICanBeAnalyzed
		{
			if (architecture == null)
			{
				throw new ArgumentException($"Invalid argument: {nameof(architecture)}");
			};

			if (collection == null)
			{
				throw new ArgumentException($"Invalid argument: {nameof(collection)}");
			};

			var description = DescribeCollection(collection, architecture);
			Console.WriteLine(description);
		}
	}
}

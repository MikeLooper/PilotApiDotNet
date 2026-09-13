using ArchUnitNET.NUnit;
using NUnit.Framework;
using PilotApi.Architecture.Tests.Base;
using PilotApi.Architecture.Tests.Constants;
using PilotApi.Architecture.Tests.Utilities;
using System;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace PilotApi.Architecture.Tests
{
	/// <summary>
	/// Enforces the "Async" suffix convention for asynchronous methods in the Domain, Services, and Repositories
	/// layers, mirroring what these layers already do (GetAllAsync, InsertAsync, DeleteAsync, etc.). Controller
	/// actions are deliberately out of scope: ASP.NET Core convention omits the suffix on action methods.
	/// </summary>
	[TestFixture]
	public class AsyncNamingTests : ArchitectureTestBase
	{
		[Test]
		public void Public_Async_Methods_In_Domain_Services_And_Repositories_Should_End_With_Async_Test()
		{
			var domainServicesOrRepositoriesTypes = Types()
				.That().ResideInAssemblyMatching(ArchTestConstants.DomainAssemblyRegex)
				.Or().ResideInAssemblyMatching(ArchTestConstants.ServicesAssemblyRegex)
				.Or().ResideInAssemblyMatching(ArchTestConstants.RepositoriesAssemblyRegex);

			// MethodMember.Name includes the parameter signature (e.g. "GetAllAsync(System.Int32,...)"), so
			// "return a Task" and "ends with Async" are both matched against that shape rather than a bare name.
			var asyncMethods = MethodMembers()
				.That().AreDeclaredIn(domainServicesOrRepositoriesTypes)
				.And().ArePublic()
				.And().AreNoConstructors()
				.And().FollowCustomPredicate(
					m => m.ReturnType.FullName.StartsWith("System.Threading.Tasks.Task", StringComparison.Ordinal),
					"have a return type of Task or Task<T>");

			ArchUnitUtilities.PrintCollection(asyncMethods, Architecture);

			asyncMethods
				.Should()
				.HaveNameMatching(@"Async\(.*\)$")
				.Check(Architecture);
		}
	}
}

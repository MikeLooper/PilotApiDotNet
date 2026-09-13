using ArchUnitNET.Domain;
using ArchUnitNET.NUnit;
using NUnit.Framework;
using PilotApi.Architecture.Tests.Base;
using PilotApi.Architecture.Tests.Constants;
using PilotApi.Architecture.Tests.Utilities;
using PilotApi.Domain.Contracts.Services.Base;
using PilotApi.Repositories.Contracts.Repository.Base;
using PilotApi.Repositories.Repositories.Base;
using PilotApi.Services.Services.Base;
using System.Linq;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace PilotApi.Architecture.Tests
{
	/// <summary>
	/// Enforces that every repository and service implementation is actually built on its shared base class,
	/// rather than reimplementing common plumbing (logging, CRUD wiring, etc.) from scratch.
	/// </summary>
	[TestFixture]
	public class BaseClassTests : ArchitectureTestBase
	{
		[Test]
		public void Repository_Classes_Should_Derive_From_RepositoryBase_Test()
		{
			var repositoryClasses = Classes()
				.That().ResideInNamespaceMatching(ArchTestConstants.PilotApiNamespaceRegex)
				.And().HaveNameEndingWith("Repository")
				// UserRolesRepository is a deliberate exception: it is a hard-coded/mocked role lookup with no
				// backing EntityBase entity and no SQL access, so it cannot derive from the generic RepositoryBase<TEntity>
				// the way every real CRUD repository does.
				.And().DoNotHaveName("UserRolesRepository");

			ArchUnitUtilities.PrintCollection(repositoryClasses, Architecture);

			repositoryClasses
				.Should()
				.BeAssignableTo(typeof(RepositoryBase<>))
				.Check(Architecture);
		}

		[Test]
		public void Service_Classes_Should_Derive_From_ServiceBase_Test()
		{
			var serviceClasses = Classes()
				.That().ResideInNamespaceMatching(ArchTestConstants.PilotApiNamespaceRegex)
				.And().HaveNameEndingWith("Service");

			ArchUnitUtilities.PrintCollection(serviceClasses, Architecture);

			serviceClasses
				.Should()
				.BeAssignableTo(typeof(ServiceBase<,>))
				.Check(Architecture);
		}

		[Test]
		public void Repository_Implementations_Should_Have_Exactly_One_Public_Constructor_Test()
		{
			var repositoryImplementations = Classes()
				.That().ResideInNamespaceMatching(ArchTestConstants.PilotApiNamespaceRegex)
				.And().AreAssignableTo(typeof(IRepositoryBase<>))
				.And().AreNotAbstract();

			ArchUnitUtilities.PrintCollection(repositoryImplementations, Architecture);

			repositoryImplementations
				.Should()
				.FollowCustomCondition(
					cls => HasExactlyOnePublicConstructor(cls),
					"have exactly one public constructor",
					"does not have exactly one public constructor")
				.Check(Architecture);
		}

		[Test]
		public void Service_Implementations_Should_Have_Exactly_One_Public_Constructor_Test()
		{
			var serviceImplementations = Classes()
				.That().ResideInNamespaceMatching(ArchTestConstants.PilotApiNamespaceRegex)
				.And().AreAssignableTo(typeof(IServiceBase<>))
				.And().AreNotAbstract();

			ArchUnitUtilities.PrintCollection(serviceImplementations, Architecture);

			serviceImplementations
				.Should()
				.FollowCustomCondition(
					cls => HasExactlyOnePublicConstructor(cls),
					"have exactly one public constructor",
					"does not have exactly one public constructor")
				.Check(Architecture);
		}

		private static bool HasExactlyOnePublicConstructor(Class cls)
		{
			return cls.Members
				.OfType<MethodMember>()
				.Count(m => m.MethodForm == MethodForm.Constructor && m.Visibility == Visibility.Public) == 1;
		}
	}
}

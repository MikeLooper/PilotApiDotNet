using ArchUnitNET.NUnit;
using NUnit.Framework;
using PilotApi.Architecture.Tests.Base;
using PilotApi.Architecture.Tests.Constants;
using PilotApi.Architecture.Tests.Utilities;
using PilotApi.Repositories.DataSource;
using PilotApi.Shared.Handlers;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace PilotApi.Architecture.Tests
{
	/// <summary>
	/// Enforces dependency inversion at each layer boundary: a caller should depend on the interface the callee
	/// publishes, never on the concrete class behind it. The one accepted exception is a project's own "*Extensions"
	/// class - the composition root has to reference concrete types somewhere in order to register them with DI.
	/// </summary>
	[TestFixture]
	public class DependencyInversionTests : ArchitectureTestBase
	{
		[Test]
		public void Controllers_Should_Not_Depend_On_Concrete_Service_Classes_Test()
		{
			var controllerTypes = Types()
				.That().ResideInNamespaceMatching(ArchTestConstants.ControllersNamespaceRegex);

			ArchUnitUtilities.PrintCollection(controllerTypes, Architecture);

			var concreteServiceTypes = Types()
				.That().ResideInNamespaceMatching(@"^PilotApi\.Services\.Services(\..+)?$");

			ArchUnitUtilities.PrintCollection(concreteServiceTypes, Architecture);

			controllerTypes
				.Should()
				.NotDependOnAny(concreteServiceTypes)
				.Check(Architecture);
		}

		[Test]
		public void Services_Should_Not_Depend_On_Concrete_Repository_Classes_Test()
		{
			// ServicesInjectionExtensions is this project's composition root - it necessarily references the
			// concrete repository classes once, to register them against their interfaces with the DI container.
			var serviceTypes = Types()
				.That().ResideInAssemblyMatching(ArchTestConstants.ServicesAssemblyRegex)
				.And().DoNotHaveNameEndingWith("Extensions");

			ArchUnitUtilities.PrintCollection(serviceTypes, Architecture);

			var concreteRepositoryTypes = Types()
				.That().ResideInNamespaceMatching(@"^PilotApi\.Repositories\.Repositories(\..+)?$");

			ArchUnitUtilities.PrintCollection(concreteRepositoryTypes, Architecture);

			serviceTypes
				.Should()
				.NotDependOnAny(concreteRepositoryTypes)
				.Check(Architecture);
		}

		[Test]
		public void Repositories_Should_Not_Depend_On_Concrete_DataSourceContext_Or_SqlBuilder_Test()
		{
			var repositoryImplementationTypes = Types()
				.That().ResideInNamespaceMatching(@"^PilotApi\.Repositories\.Repositories(\..+)?$");

			ArchUnitUtilities.PrintCollection(repositoryImplementationTypes, Architecture);

			var concreteDataAccessTypes = Types()
				.That().Are(typeof(DataSourceContext), typeof(SqlBuilder));

			ArchUnitUtilities.PrintCollection(concreteDataAccessTypes, Architecture);

			repositoryImplementationTypes
				.Should()
				.NotDependOnAny(concreteDataAccessTypes)
				.Check(Architecture);
		}

		[Test]
		public void Controllers_Should_Not_Depend_On_Entity_Types_Test()
		{
			// Entities are a Repositories-layer implementation detail. Controllers should only ever see DTOs.
			var controllerTypes = Types()
				.That().ResideInNamespaceMatching(ArchTestConstants.ControllersNamespaceRegex);

			ArchUnitUtilities.PrintCollection(controllerTypes, Architecture);

			var entityTypes = Types()
				.That().ResideInNamespaceMatching(@"^PilotApi\.Repositories\.Models\.Entities(\..+)?$");

			ArchUnitUtilities.PrintCollection(entityTypes, Architecture);

			controllerTypes
				.Should()
				.NotDependOnAny(entityTypes)
				.Check(Architecture);
		}
	}
}

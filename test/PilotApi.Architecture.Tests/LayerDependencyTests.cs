using ArchUnitNET.NUnit;
using NUnit.Framework;
using PilotApi.Architecture.Tests.Base;
using PilotApi.Architecture.Tests.Constants;
using PilotApi.Architecture.Tests.Utilities;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace PilotApi.Architecture.Tests
{
	/// <summary>
	/// Enforces the layering of the solution's projects: Shared sits below every other layer, Domain sits above
	/// Shared, Repositories sits above Domain, Services sits above Repositories, and Web sits above everything.
	/// A lower layer must never depend on a higher layer.
	/// </summary>
	[TestFixture]
	public class LayerDependencyTests : ArchitectureTestBase
	{
		[Test]
		public void Shared_Should_Not_Depend_On_Domain_Repositories_Services_Or_Web_Test()
		{
			var sharedTypes = Types()
				.That().ResideInAssemblyMatching(ArchTestConstants.SharedAssemblyRegex);

			ArchUnitUtilities.PrintCollection(sharedTypes, Architecture);

			var higherLayers = Types()
				.That().ResideInAssemblyMatching(ArchTestConstants.DomainAssemblyRegex)
				.Or().ResideInAssemblyMatching(ArchTestConstants.RepositoriesAssemblyRegex)
				.Or().ResideInAssemblyMatching(ArchTestConstants.ServicesAssemblyRegex)
				.Or().ResideInAssemblyMatching(ArchTestConstants.WebAssemblyRegex);

			sharedTypes
				.Should()
				.NotDependOnAny(higherLayers)
				.Check(Architecture);
		}

		[Test]
		public void Domain_Should_Not_Depend_On_Repositories_Services_Or_Web_Test()
		{
			var domainTypes = Types()
				.That().ResideInAssemblyMatching(ArchTestConstants.DomainAssemblyRegex);

			ArchUnitUtilities.PrintCollection(domainTypes, Architecture);

			var higherLayers = Types()
				.That().ResideInAssemblyMatching(ArchTestConstants.RepositoriesAssemblyRegex)
				.Or().ResideInAssemblyMatching(ArchTestConstants.ServicesAssemblyRegex)
				.Or().ResideInAssemblyMatching(ArchTestConstants.WebAssemblyRegex);

			domainTypes
				.Should()
				.NotDependOnAny(higherLayers)
				.Check(Architecture);
		}

		[Test]
		public void Repositories_Should_Not_Depend_On_Services_Or_Web_Test()
		{
			var repositoriesTypes = Types()
				.That().ResideInAssemblyMatching(ArchTestConstants.RepositoriesAssemblyRegex);

			ArchUnitUtilities.PrintCollection(repositoriesTypes, Architecture);

			var higherLayers = Types()
				.That().ResideInAssemblyMatching(ArchTestConstants.ServicesAssemblyRegex)
				.Or().ResideInAssemblyMatching(ArchTestConstants.WebAssemblyRegex);

			repositoriesTypes
				.Should()
				.NotDependOnAny(higherLayers)
				.Check(Architecture);
		}

		[Test]
		public void Services_Should_Not_Depend_On_Web_Test()
		{
			var servicesTypes = Types()
				.That().ResideInAssemblyMatching(ArchTestConstants.ServicesAssemblyRegex);

			ArchUnitUtilities.PrintCollection(servicesTypes, Architecture);

			var webTypes = Types()
				.That().ResideInAssemblyMatching(ArchTestConstants.WebAssemblyRegex);

			servicesTypes
				.Should()
				.NotDependOnAny(webTypes)
				.Check(Architecture);
		}

		[Test]
		public void Web_Should_Not_Depend_On_Repositories_Test()
		{
			// The web layer should only reach the data store through the services layer; a direct
			// dependency on Repositories would bypass that abstraction.
			var webTypes = Types()
				.That().ResideInAssemblyMatching(ArchTestConstants.WebAssemblyRegex);

			ArchUnitUtilities.PrintCollection(webTypes, Architecture);

			var repositoriesTypes = Types()
				.That().ResideInAssemblyMatching(ArchTestConstants.RepositoriesAssemblyRegex);

			webTypes
				.Should()
				.NotDependOnAny(repositoriesTypes)
				.Check(Architecture);
		}
	}
}

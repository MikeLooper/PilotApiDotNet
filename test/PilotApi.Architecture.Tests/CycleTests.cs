using ArchUnitNET.Fluent.Slices;
using ArchUnitNET.NUnit;
using NUnit.Framework;
using PilotApi.Architecture.Tests.Base;

namespace PilotApi.Architecture.Tests
{
	/// <summary>
	/// Guards against circular namespace dependencies developing as the codebase grows - e.g. a "Repositories"
	/// implementation namespace depending back on the "Contracts" namespace that is supposed to depend on it, or
	/// two sibling namespaces within the same project reaching into each other.
	/// </summary>
	/// <remarks>
	/// Cross-project cycles are already ruled out by the strict, one-directional layering asserted in
	/// LayerDependencyTests (a real cycle would require two projects to each depend on the other, which those
	/// tests forbid). What is not yet covered is a cycle developing between two sub-namespaces of the *same*
	/// project, which is what the checks below catch.
	///
	/// Shared is deliberately excluded here: its Configuration and Contracts.Configuration namespaces have a
	/// known, accepted cycle today (IApplicationConfiguration and its siblings expose properties typed as
	/// concrete Configuration classes instead of their own interfaces). Untangling that touches JSON/Newtonsoft
	/// binding behavior across several classes, so it is tracked as known debt rather than fixed here.
	/// </remarks>
	[TestFixture]
	public class CycleTests : ArchitectureTestBase
	{
		[Test]
		public void Domain_Namespaces_Should_Be_Free_Of_Cyclic_Dependencies_Test()
		{
			AssertNoCyclesWithin("Domain");
		}

		[Test]
		public void Repositories_Namespaces_Should_Be_Free_Of_Cyclic_Dependencies_Test()
		{
			AssertNoCyclesWithin("Repositories");
		}

		[Test]
		public void Services_Namespaces_Should_Be_Free_Of_Cyclic_Dependencies_Test()
		{
			AssertNoCyclesWithin("Services");
		}

		[Test]
		public void Web_Namespaces_Should_Be_Free_Of_Cyclic_Dependencies_Test()
		{
			AssertNoCyclesWithin("Web");
		}

		private void AssertNoCyclesWithin(string project)
		{
			// Each slice is a sub-namespace of the given project - e.g. for "Repositories", the slices are
			// "Repositories.Repositories", "Repositories.Contracts", "Repositories.Models", etc.
			SliceRuleDefinition.Slices()
				.Matching($"PilotApi.{project}.(*)..")
				.Should()
				.BeFreeOfCycles()
				.Check(Architecture);
		}
	}
}

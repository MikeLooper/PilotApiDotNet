using System.Reflection;
using ArchUnitNET.Loader;
using PilotApi.Architecture.Tests.Constants;

namespace PilotApi.Architecture.Tests.Base
{
	/// <summary>
	/// A base class for architecture tests that provides a common architecture definition for all derived test classes.
	/// </summary>
	public abstract class ArchitectureTestBase
	{
		/// <summary>
		/// define the whole architecture that will be tested.
		/// </summary>
		protected static readonly ArchUnitNET.Domain.Architecture Architecture = 
			new ArchLoader()
				.LoadAssemblies(
					Assembly.Load(ArchTestConstants.DomainAssembly),
					Assembly.Load(ArchTestConstants.RepositoriesAssembly),
					Assembly.Load(ArchTestConstants.ServicesAssembly),
					Assembly.Load(ArchTestConstants.SharedAssembly),
					Assembly.Load(ArchTestConstants.WebAssembly))
				.Build();
	}
}

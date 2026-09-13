namespace PilotApi.Architecture.Tests.Constants
{
	/// <summary>
	/// Constants used in architecture tests.
	/// </summary>
	public static class ArchTestConstants
	{
		/// <summary>
		/// The Domain Assembly main namespace.
		/// </summary>
		public const string DomainAssembly = "PilotApi.Domain";

		/// <summary>
		/// The Repositories Assembly main namespace.
		/// </summary>
		public const string RepositoriesAssembly = "PilotApi.Repositories";

		/// <summary>
		/// The Services Assembly main namespace.
		/// </summary>
		public const string ServicesAssembly = "PilotApi.Services";

		/// <summary>
		/// The Shared Assembly main namespace.
		/// </summary>
		public const string SharedAssembly = "PilotApi.Shared";

		/// <summary>
		/// The Web Assembly main namespace.
		/// </summary>
		public const string WebAssembly = "PilotApi.Web";

		/// <summary>
		/// Matches the "PilotApi.Web.Controllers" namespace and any of its sub-namespaces (e.g. "PilotApi.Web.Controllers.V1").
		/// </summary>
		/// <remarks>
		/// ResideInNamespace only matches the exact namespace given, so it would silently miss versioned
		/// controllers living in child namespaces. Use ResideInNamespaceMatching with this pattern instead.
		/// </remarks>
		public const string ControllersNamespaceRegex = @"^PilotApi\.Web\.Controllers(\..+)?$";

		/// <summary>
		/// Matches the full assembly identity (name, version, culture, public key token) of an assembly by its simple name.
		/// </summary>
		/// <remarks>
		/// ResideInAssembly compares against the assembly's full identity string (e.g. "PilotApi.Domain, Version=1.0.0.0, ..."),
		/// not its simple name, so an exact match on the simple name alone would never succeed. Use ResideInAssemblyMatching
		/// with these patterns instead.
		/// </remarks>
		public const string DomainAssemblyRegex = @"^PilotApi\.Domain,";

		/// <inheritdoc cref="DomainAssemblyRegex"/>
		public const string RepositoriesAssemblyRegex = @"^PilotApi\.Repositories,";

		/// <inheritdoc cref="DomainAssemblyRegex"/>
		public const string ServicesAssemblyRegex = @"^PilotApi\.Services,";

		/// <inheritdoc cref="DomainAssemblyRegex"/>
		public const string SharedAssemblyRegex = @"^PilotApi\.Shared,";

		/// <inheritdoc cref="DomainAssemblyRegex"/>
		public const string WebAssemblyRegex = @"^PilotApi\.Web,";

		/// <summary>
		/// Matches any namespace that is part of this solution's own code, as opposed to a third-party
		/// namespace (e.g. from the ASP.NET Core assembly that ArchitectureTestBase also loads into the architecture).
		/// </summary>
		public const string PilotApiNamespaceRegex = @"^PilotApi\..+$";
	}
}

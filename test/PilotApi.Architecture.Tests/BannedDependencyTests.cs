using ArchUnitNET.NUnit;
using NUnit.Framework;
using PilotApi.Architecture.Tests.Base;
using PilotApi.Architecture.Tests.Constants;
using PilotApi.Architecture.Tests.Utilities;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace PilotApi.Architecture.Tests
{
	/// <summary>
	/// Enforces which third-party namespaces each layer is allowed to depend on: ASP.NET Core belongs only to the
	/// web layer, data-access libraries (Dapper, Microsoft.Data.SqlClient, Npgsql) belong only to the repositories
	/// layer, and test-only libraries (Microsoft.NET.Test.Sdk, Moq, NUnit) never belong in application code at all.
	/// </summary>
	/// <remarks>
	/// The "banned" side of each rule is built with Types(includeReferenced: true), because the offending
	/// assemblies (Dapper, Npgsql, Moq, etc.) are never explicitly loaded into the architecture - by default
	/// Types() only sees types from loaded assemblies. Passing includeReferenced: true pulls in the stub types
	/// ArchUnitNET records for every externally-referenced type, which is what a real dependency edge points at.
	/// </remarks>
	[TestFixture]
	public class BannedDependencyTests : ArchitectureTestBase
	{
		[Test]
		public void Domain_And_Repositories_Should_Not_Depend_On_AspNetCore_Test()
		{
			var domainOrRepositoryTypes = Types()
				.That().ResideInAssemblyMatching(ArchTestConstants.DomainAssemblyRegex)
				.Or().ResideInAssemblyMatching(ArchTestConstants.RepositoriesAssemblyRegex);

			ArchUnitUtilities.PrintCollection(domainOrRepositoryTypes, Architecture);

			var aspNetCoreTypes = Types(includeReferenced: true)
				.That().ResideInNamespaceMatching(@"^Microsoft\.AspNetCore(\..+)?$");

			ArchUnitUtilities.PrintCollection(aspNetCoreTypes, Architecture);

			domainOrRepositoryTypes
				.Should()
				.NotDependOnAny(aspNetCoreTypes)
				.Check(Architecture);
		}

		[Test]
		public void Web_And_Domain_Should_Not_Depend_On_Dapper_Test()
		{
			var webOrDomainTypes = Types()
				.That().ResideInAssemblyMatching(ArchTestConstants.WebAssemblyRegex)
				.Or().ResideInAssemblyMatching(ArchTestConstants.DomainAssemblyRegex);

			ArchUnitUtilities.PrintCollection(webOrDomainTypes, Architecture);

			var dapperTypes = Types(includeReferenced: true)
				.That().ResideInNamespaceMatching(@"^Dapper(\..+)?$");

			ArchUnitUtilities.PrintCollection(dapperTypes, Architecture);

			webOrDomainTypes
				.Should()
				.NotDependOnAny(dapperTypes)
				.Check(Architecture);
		}

		[Test]
		public void Web_And_Domain_Should_Not_Depend_On_SqlClient_Test()
		{
			var webOrDomainTypes = Types()
				.That().ResideInAssemblyMatching(ArchTestConstants.WebAssemblyRegex)
				.Or().ResideInAssemblyMatching(ArchTestConstants.DomainAssemblyRegex);

			ArchUnitUtilities.PrintCollection(webOrDomainTypes, Architecture);

			var sqlClientTypes = Types(includeReferenced: true)
				.That().ResideInNamespaceMatching(@"^Microsoft\.Data\.SqlClient(\..+)?$");

			ArchUnitUtilities.PrintCollection(sqlClientTypes, Architecture);

			webOrDomainTypes
				.Should()
				.NotDependOnAny(sqlClientTypes)
				.Check(Architecture);
		}

		[Test]
		public void Web_And_Domain_Should_Not_Depend_On_Legacy_SqlClient_Test()
		{
			// System.Data.SqlClient is the deprecated predecessor to Microsoft.Data.SqlClient.
			var webOrDomainTypes = Types()
				.That().ResideInAssemblyMatching(ArchTestConstants.WebAssemblyRegex)
				.Or().ResideInAssemblyMatching(ArchTestConstants.DomainAssemblyRegex);

			ArchUnitUtilities.PrintCollection(webOrDomainTypes, Architecture);

			var legacySqlClientTypes = Types(includeReferenced: true)
				.That().ResideInNamespaceMatching(@"^System\.Data\.SqlClient(\..+)?$");

			ArchUnitUtilities.PrintCollection(legacySqlClientTypes, Architecture);

			webOrDomainTypes
				.Should()
				.NotDependOnAny(legacySqlClientTypes)
				.Check(Architecture);
		}

		[Test]
		public void Web_And_Domain_Should_Not_Depend_On_Npgsql_Test()
		{
			var webOrDomainTypes = Types()
				.That().ResideInAssemblyMatching(ArchTestConstants.WebAssemblyRegex)
				.Or().ResideInAssemblyMatching(ArchTestConstants.DomainAssemblyRegex);

			ArchUnitUtilities.PrintCollection(webOrDomainTypes, Architecture);

			var npgsqlTypes = Types(includeReferenced: true)
				.That().ResideInNamespaceMatching(@"^Npgsql(\..+)?$");

			ArchUnitUtilities.PrintCollection(npgsqlTypes, Architecture);

			webOrDomainTypes
				.Should()
				.NotDependOnAny(npgsqlTypes)
				.Check(Architecture);
		}

		[Test]
		public void Application_Code_Should_Not_Depend_On_Test_Sdk_Test()
		{
			var applicationTypes = Types()
				.That().ResideInNamespaceMatching(ArchTestConstants.PilotApiNamespaceRegex);

			ArchUnitUtilities.PrintCollection(applicationTypes, Architecture);

			var testSdkTypes = Types(includeReferenced: true)
				.That().ResideInNamespaceMatching(@"^Microsoft\.NET\.Test\.Sdk(\..+)?$");

			ArchUnitUtilities.PrintCollection(testSdkTypes, Architecture);

			applicationTypes
				.Should()
				.NotDependOnAny(testSdkTypes)
				.Check(Architecture);
		}

		[Test]
		public void Application_Code_Should_Not_Depend_On_Moq_Test()
		{
			var applicationTypes = Types()
				.That().ResideInNamespaceMatching(ArchTestConstants.PilotApiNamespaceRegex);

			ArchUnitUtilities.PrintCollection(applicationTypes, Architecture);

			var moqTypes = Types(includeReferenced: true)
				.That().ResideInNamespaceMatching(@"^Moq(\..+)?$");

			ArchUnitUtilities.PrintCollection(moqTypes, Architecture);

			applicationTypes
				.Should()
				.NotDependOnAny(moqTypes)
				.Check(Architecture);
		}

		[Test]
		public void Application_Code_Should_Not_Depend_On_NUnit_Test()
		{
			var applicationTypes = Types()
				.That().ResideInNamespaceMatching(ArchTestConstants.PilotApiNamespaceRegex);

			ArchUnitUtilities.PrintCollection(applicationTypes, Architecture);

			var nUnitTypes = Types(includeReferenced: true)
				.That().ResideInNamespaceMatching(@"^NUnit(\..+)?$");

			ArchUnitUtilities.PrintCollection(nUnitTypes, Architecture);

			applicationTypes
				.Should()
				.NotDependOnAny(nUnitTypes)
				.Check(Architecture);
		}
	}
}

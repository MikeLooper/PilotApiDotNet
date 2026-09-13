using ArchUnitNET.NUnit;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using PilotApi.Architecture.Tests.Base;
using PilotApi.Architecture.Tests.Constants;
using PilotApi.Architecture.Tests.Utilities;
using System;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace PilotApi.Architecture.Tests
{
	/// <summary>
	/// Enforces the naming and placement conventions this solution relies on to keep each layer predictable:
	/// interfaces are prefixed with "I", implementations carry the suffix that matches their role (Service,
	/// Repository, Dto, Entity, Configuration, Controller, Exception), and live in the namespace that owns that role.
	/// </summary>
	[TestFixture]
	public class NamingConventionTests : ArchitectureTestBase
	{
		[Test]
		public void Interfaces_Should_Have_Name_Starting_With_I_Test()
		{
			var interfaces = Interfaces()
				.That().ResideInNamespaceMatching(ArchTestConstants.PilotApiNamespaceRegex);

			ArchUnitUtilities.PrintCollection(interfaces, Architecture);

			interfaces
				.Should()
				.HaveNameStartingWith("I")
				.Check(Architecture);
		}

		[Test]
		public void Service_Implementations_Should_Have_Name_Ending_With_Service_And_Reside_In_Services_Namespace_Test()
		{
			var serviceInterfaces = Interfaces()
				.That().ResideInNamespaceMatching(@"^PilotApi\.Domain\.Contracts\.Services(\..+)?$")
				.And().HaveNameEndingWith("Service");

			var serviceImplementations = Classes()
				.That().ImplementAnyInterfaces(serviceInterfaces)
				.And().AreNotAbstract();

			ArchUnitUtilities.PrintCollection(serviceImplementations, Architecture);

			serviceImplementations
				.Should()
				.HaveNameEndingWith("Service")
				.AndShould()
				.ResideInNamespaceMatching(@"^PilotApi\.Services\.Services$")
				.Check(Architecture);
		}

		[Test]
		public void Repository_Implementations_Should_Have_Name_Ending_With_Repository_And_Reside_In_Repositories_Namespace_Test()
		{
			var repositoryInterfaces = Interfaces()
				.That().ResideInNamespaceMatching(@"^PilotApi\.Repositories\.Contracts\.Repository(\..+)?$")
				.And().HaveNameEndingWith("Repository");

			var repositoryImplementations = Classes()
				.That().ImplementAnyInterfaces(repositoryInterfaces)
				.And().AreNotAbstract();

			ArchUnitUtilities.PrintCollection(repositoryImplementations, Architecture);

			repositoryImplementations
				.Should()
				.HaveNameEndingWith("Repository")
				.AndShould()
				.ResideInNamespaceMatching(@"^PilotApi\.Repositories\.Repositories$")
				.Check(Architecture);
		}

		[Test]
		public void Configuration_Implementations_Should_Have_Name_Ending_With_Configuration_Test()
		{
			var configurationInterfaces = Interfaces()
				.That().ResideInNamespaceMatching(@"^PilotApi\.Shared\.Contracts\.Configuration(\..+)?$")
				.And().HaveNameEndingWith("Configuration");

			var configurationImplementations = Classes()
				.That().ImplementAnyInterfaces(configurationInterfaces)
				.And().AreNotAbstract();

			ArchUnitUtilities.PrintCollection(configurationImplementations, Architecture);

			configurationImplementations
				.Should()
				.HaveNameEndingWith("Configuration")
				.Check(Architecture);
		}

		[Test]
		public void Configuration_Interfaces_Should_Reside_In_Contracts_Configuration_Namespace_Test()
		{
			// Every "I*Configuration" interface should live alongside its siblings under Shared.Contracts.Configuration,
			// rather than next to its implementation under Shared.Configuration.
			var configurationInterfaces = Interfaces()
				.That().ResideInAssemblyMatching(ArchTestConstants.SharedAssemblyRegex)
				.And().HaveNameEndingWith("Configuration");

			ArchUnitUtilities.PrintCollection(configurationInterfaces, Architecture);

			configurationInterfaces
				.Should()
				.ResideInNamespaceMatching(@"^PilotApi\.Shared\.Contracts\.Configuration(\..+)?$")
				.Check(Architecture);
		}

		[Test]
		public void Dto_Classes_Should_Have_Name_Ending_With_Dto_Test()
		{
			var dtoClasses = Classes()
				.That().ResideInNamespaceMatching(@"^PilotApi\.Domain\.Models\.Dto$");

			ArchUnitUtilities.PrintCollection(dtoClasses, Architecture);

			dtoClasses
				.Should()
				.HaveNameEndingWith("Dto")
				.Check(Architecture);
		}

		[Test]
		public void Entity_Classes_Should_Have_Name_Ending_With_Entity_Test()
		{
			var entityClasses = Classes()
				.That().ResideInNamespaceMatching(@"^PilotApi\.Repositories\.Models\.Entities$")
				.And().AreNotAbstract();

			ArchUnitUtilities.PrintCollection(entityClasses, Architecture);

			entityClasses
				.Should()
				.HaveNameEndingWith("Entity")
				.Check(Architecture);
		}

		[Test]
		public void Exception_Classes_Should_Have_Name_Ending_With_Exception_And_Reside_In_Shared_Exceptions_Namespace_Test()
		{
			var exceptionClasses = Classes()
				.That().ResideInNamespaceMatching(ArchTestConstants.PilotApiNamespaceRegex)
				.And().AreAssignableTo(typeof(Exception));

			ArchUnitUtilities.PrintCollection(exceptionClasses, Architecture);

			exceptionClasses
				.Should()
				.HaveNameEndingWith("Exception")
				.AndShould()
				.ResideInNamespace("PilotApi.Shared.Exceptions")
				.AndShould()
				.BePublic()
				.Check(Architecture);
		}

		[Test]
		public void Extension_Classes_Should_Be_Static_Test()
		{
			var extensionClasses = Classes()
				.That().ResideInNamespaceMatching(ArchTestConstants.PilotApiNamespaceRegex)
				.And().HaveNameEndingWith("Extensions");

			ArchUnitUtilities.PrintCollection(extensionClasses, Architecture);

			// A static class compiles down to a sealed, abstract class.
			extensionClasses
				.Should()
				.BeSealed()
				.AndShould()
				.BeAbstract()
				.Check(Architecture);
		}

		[Test]
		public void Constants_Classes_Should_Be_Static_Test()
		{
			var constantsClasses = Classes()
				.That().ResideInNamespaceMatching(ArchTestConstants.PilotApiNamespaceRegex)
				.And().HaveNameEndingWith("Constants");

			ArchUnitUtilities.PrintCollection(constantsClasses, Architecture);

			// A static class compiles down to a sealed, abstract class.
			constantsClasses
				.Should()
				.BeSealed()
				.AndShould()
				.BeAbstract()
				.Check(Architecture);
		}

		[Test]
		public void Controllers_Should_Have_Name_Ending_With_Controller_And_Reside_In_Controllers_Namespace_Test()
		{
			var controllerClasses = Classes()
				.That().AreAssignableTo(typeof(Controller))
				.And().AreNotAbstract()
				.And().ResideInNamespaceMatching(ArchTestConstants.PilotApiNamespaceRegex);

			ArchUnitUtilities.PrintCollection(controllerClasses, Architecture);

			controllerClasses
				.Should()
				.HaveNameEndingWith("Controller")
				.AndShould()
				.ResideInNamespaceMatching(ArchTestConstants.ControllersNamespaceRegex)
				.Check(Architecture);
		}
	}
}

using ArchUnitNET.NUnit;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using PilotApi.Architecture.Tests.Base;
using PilotApi.Architecture.Tests.Constants;
using PilotApi.Architecture.Tests.Utilities;
using PilotApi.Web.Controllers;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace PilotApi.Architecture.Tests
{
	[TestFixture]
	public class AttributesTests : ArchitectureTestBase
	{
		[Test]
		public void Anonymous_Controllers_Should_Not_Include_An_Authorize_Attribute_Test()
		{
			var controllerClasses = Classes()
				.That().ResideInNamespaceMatching(ArchTestConstants.ControllersNamespaceRegex)
				.And().AreAssignableTo(typeof(Controller))
				.And()
				.AreNotAbstract();

			ArchUnitUtilities.PrintCollection(controllerClasses, Architecture);

			var anonControllers = controllerClasses
				.And()
				.HaveAnyAttributes(typeof(AllowAnonymousAttribute));

			ArchUnitUtilities.PrintCollection(anonControllers, Architecture);

			var rule = anonControllers
				.Should()
				.NotHaveAnyAttributes(typeof(AuthorizeAttribute));

			Assert.True(rule.HasNoViolations(Architecture));
		}

		[Test]
		public void Authenticated_Controllers_Should_Not_Include_An_Anonymous_Attribute_Test()
		{
			var controllerClasses = Classes()
				.That().ResideInNamespaceMatching(ArchTestConstants.ControllersNamespaceRegex)
				.And().AreAssignableTo(typeof(Controller))
				.And()
				.AreNotAbstract();

			ArchUnitUtilities.PrintCollection(controllerClasses, Architecture);

			var authedControllers = controllerClasses
				.And()
				.HaveAnyAttributes(typeof(AuthorizeAttribute));

			ArchUnitUtilities.PrintCollection(authedControllers, Architecture);

			authedControllers
				.Should()
				.NotHaveAnyAttributes(typeof(AllowAnonymousAttribute))
				.WithoutRequiringPositiveResults()
				.Check(Architecture);
		}

		[Test]
		public void Controllers_Should_Include_A_Produces_Attribute_Test()
		{
			var controllerClasses = Classes()
				.That().ResideInNamespaceMatching(ArchTestConstants.ControllersNamespaceRegex)
				.And().AreAssignableTo(typeof(Controller))
				.And()
				.AreNotAbstract();

			ArchUnitUtilities.PrintCollection(controllerClasses, Architecture);

			// A controller satisfies this rule either by declaring the attribute itself, or by deriving from
			// SimpleControllerBase, which declares it once for every controller that inherits from it.
			controllerClasses
				.Should()
				.HaveAnyAttributes(typeof(ProducesAttribute))
				.OrShould()
				.BeAssignableTo(typeof(SimpleControllerBase))
				.Check(Architecture);
		}

		[Test]
		public void Controllers_Should_Include_A_Version_Or_Version_Neutral_Indicator_Test()
		{
			var controllerClasses = Classes()
				.That().ResideInNamespaceMatching(ArchTestConstants.ControllersNamespaceRegex)
				.And().AreAssignableTo(typeof(Controller))
				.And()
				.AreNotAbstract();

			ArchUnitUtilities.PrintCollection(controllerClasses, Architecture);

			controllerClasses
				.Should()
				.HaveAnyAttributes(typeof(ApiVersionAttribute), typeof(ApiVersionNeutralAttribute))
				.Check(Architecture);
		}

		[Test]
		public void Controllers_Should_Include_An_ApiController_Attribute_Test()
		{
			var controllerClasses = Classes()
				.That().ResideInNamespaceMatching(ArchTestConstants.ControllersNamespaceRegex)
				.And().AreAssignableTo(typeof(Controller))
				.And()
				.AreNotAbstract();

			ArchUnitUtilities.PrintCollection(controllerClasses, Architecture);

			// A controller satisfies this rule either by declaring the attribute itself, or by deriving from
			// SimpleControllerBase, which declares it once for every controller that inherits from it.
			controllerClasses
				.Should()
				.HaveAnyAttributes(typeof(ApiControllerAttribute))
				.OrShould()
				.BeAssignableTo(typeof(SimpleControllerBase))
				.Check(Architecture);
		}
	}
}

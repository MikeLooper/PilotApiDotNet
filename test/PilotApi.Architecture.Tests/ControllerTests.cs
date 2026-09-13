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
	public class ControllerTests : ArchitectureTestBase
	{
		[Test]
		public void All_Controller_Endpoints_Require_Specific_Headers_Test()
		{
			var controllerClasses = Classes()
				.That().ResideInNamespaceMatching(ArchTestConstants.ControllersNamespaceRegex)
				.And().AreAssignableTo(typeof(Controller))
				.And()
				.AreNotAbstract()
				.And()
				.DoNotHaveAnyAttributes(typeof(ApiVersionNeutralAttribute));

			ArchUnitUtilities.PrintCollection(controllerClasses, Architecture);

			// A controller satisfies this rule either by declaring the "ApiVersion" header property itself, or
			// by deriving from SimpleControllerBase, which declares it once for every controller that inherits from it.
			controllerClasses
				.Should()
				.HavePropertyMemberWithName("ApiVersion")
				.OrShould()
				.BeAssignableTo(typeof(SimpleControllerBase))
				.Check(Architecture);
		}

		/// <summary>
		/// Enforces that domain endpoints require the caller to supply an "Authorization" header.
		/// </summary>
		/// <remarks>
		/// This application has no per-controller [Authorize] attribute; every request is subject to a global
		/// fallback policy (see SecurityHelper.ConfigureAuthorization) that requires an authenticated user and
		/// validates the "Authorization" header, unless the action opts out with [AllowAnonymous]. "About" and
		/// "Healthcheck" (both on SystemController) are the only endpoints meant to opt out. So this rule is
		/// enforced by asserting that no other action - and no other controller - carries [AllowAnonymous].
		/// </remarks>
		[Test]
		public void Domain_Endpoints_Should_Require_An_Authorization_Header_Test()
		{
			var controllerClasses = Classes()
				.That().ResideInNamespaceMatching(ArchTestConstants.ControllersNamespaceRegex)
				.And().AreAssignableTo(typeof(Controller))
				.And().AreNotAbstract();

			// MethodMember.Name includes the parameter signature (e.g. "About(System.Boolean)", "Healthcheck()"),
			// so excluded action names are matched with a "starts with" check rather than an exact one.
			var domainEndpoints = MethodMembers()
				.That().AreDeclaredIn(controllerClasses)
				.And().ArePublic()
				.And().AreNoConstructors()
				.And().DoNotHaveNameStartingWith("About(")
				.And().DoNotHaveNameStartingWith("Healthcheck(");

			ArchUnitUtilities.PrintCollection(domainEndpoints, Architecture);

			// No individual action may opt out of the global authorization policy.
			domainEndpoints
				.Should()
				.NotHaveAnyAttributes(typeof(AllowAnonymousAttribute))
				.Check(Architecture);

			// Nor may the controller that declares them opt out at the class level - which would exempt every
			// action on it, including ones ArchUnitNET cannot see are anonymous via the check above (since
			// attribute presence is not inherited from the declaring class down to its methods).
			var anonymousControllers = controllerClasses
				.And().HaveAnyAttributes(typeof(AllowAnonymousAttribute));

			var domainEndpointsOnAnonymousControllers = MethodMembers()
				.That().AreDeclaredIn(anonymousControllers)
				.And().ArePublic()
				.And().AreNoConstructors()
				.And().DoNotHaveNameStartingWith("About(")
				.And().DoNotHaveNameStartingWith("Healthcheck(");

			ArchUnitUtilities.PrintCollection(domainEndpointsOnAnonymousControllers, Architecture);

			domainEndpointsOnAnonymousControllers
				.Should()
				.NotExist()
				.Check(Architecture);
		}
	}
}

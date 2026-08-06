//using ArchUnitNET.NUnit;
//using Microsoft.AspNetCore.Mvc;
//using NUnit.Framework;
//using PilotApi.Architecture.Tests.Base;
//using PilotApi.Architecture.Tests.Utilities;
//using static ArchUnitNET.Fluent.ArchRuleDefinition;

//namespace PilotApi.Architecture.Tests
//{
//	[TestFixture]
//	public class ControllerTests : ArchitectureTestBase
//	{
//		[Test]
//		public void All_Controller_Endpoints_Require_Specific_Headers_Test()
//		{
//			var controllerClasses = Classes()
//				.That().ResideInNamespace("PilotApi.Web.Controllers")
//				.And().AreAssignableTo(typeof(Controller))
//				.And()
//				.AreNotAbstract();

//			ArchUnitUtilities.PrintCollection(controllerClasses, Architecture);

//			controllerClasses
//				.Should()
//				.HavePropertyMemberWithName("ApiVersion")
//				.Check(Architecture);
//		}
//	}
//}

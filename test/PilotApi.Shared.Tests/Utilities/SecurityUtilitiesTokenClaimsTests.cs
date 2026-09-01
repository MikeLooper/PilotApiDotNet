using NUnit.Framework;
using PilotApi.Shared.Utilities;
using System;
using System.Linq;
using System.Security.Claims;

namespace PilotApi.Shared.Tests.Utilities
{
	[TestFixture]
	public class SecurityUtilitiesTokenClaimsTests
	{
		[Test]
		public void SecurityUtilities_GetJwtClaims_WithKeycloakStyleToken_ShouldProjectRolesAndScopes_Test()
		{
			// Arrange
			var authorizationString = "Bearer eyJhbGciOiJub25lIn0.eyJyZWFsbV9hY2Nlc3MiOnsicm9sZXMiOlsicmVhZF9vbmx5IiwicmVhZF93cml0ZSJdfSwicmVzb3VyY2VfYWNjZXNzIjp7ImNsaWVudCI6eyJyb2xlcyI6WyJhZG1pbiJdfX0sInNjb3BlIjoiZW1haWwgcHJvZmlsZSIsInNjcCI6WyJvZmZsaW5lX2FjY2VzcyIsIm9ubGluZV9hY2Nlc3MiXSwic3ViIjoidXNlci0xMjMiLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJqb2huLmRvZSIsImVtYWlsIjoiam9obi5kb2VAZXhhbXBsZS5jb20ifQ.";

			// Act
			var claims = SecurityUtilities.GetJwtClaims(authorizationString).ToList();

			// Assert
			Console.WriteLine($"Results: {string.Join(", ", claims.Select(c => $"{c.Type}={c.Value}"))}");

			Assert.That(claims.Any(claim => claim.Type == ClaimTypes.Role && claim.Value == "read_only"), Is.True);
			Assert.That(claims.Any(claim => claim.Type == ClaimTypes.Role && claim.Value == "read_write"), Is.True);
			Assert.That(claims.Any(claim => claim.Type == ClaimTypes.Role && claim.Value == "admin"), Is.True);
			Assert.That(claims.Any(claim => claim.Type == "scope" && claim.Value == "email"), Is.True);
			Assert.That(claims.Any(claim => claim.Type == "scope" && claim.Value == "profile"), Is.True);
			Assert.That(claims.Any(claim => claim.Type == "scope" && claim.Value == "offline_access"), Is.True);
			Assert.That(claims.Any(claim => claim.Type == "scope" && claim.Value == "online_access"), Is.True);
			//Assert.That(claims.Any(claim => claim.Type == "preferred_username" && claim.Value == "john.doe"), Is.True);
			//Assert.That(claims.Any(claim => claim.Type == "email" && claim.Value == "john.doe@example.com"), Is.True);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SpotifyAcronymPlaylist.Controllers;
using SpotifyAcronymPlaylist.Helpers;

namespace SpotifyAcronymPlaylist.Tests.Controllers
{
	[TestClass]
	public class AuthControllerTest
	{
		[TestMethod]
		public void LoginShouldRedirectWithCorrectQueryParameters()
		{
			//Arrange
			var controller = this.ArrangeAuthController();

			var expectedQueryParameters = new List<string> {"client_id", "response_type", "redirect_uri", "scope"};

			//Act
			RedirectResult result = controller.Login();

			var resultUriQueryPart = UriHelper.GetQueryParameters(result.Url);

			//Assert
			resultUriQueryPart.Should().Contain(expectedQueryParameters);
		}

		[TestMethod]
		public void LoginRedirectUriQueryParameterShouldBeToCorrectRoute()
		{
			//Arrange
			var controller = this.ArrangeAuthController();

			//Act
			RedirectResult result = controller.Login();

			var resultUriQueryPart = UriHelper.GetQueryParameters(result.Url);

			var redirectUriPath = UriHelper.GetPath(resultUriQueryPart["redirect_uri"]);

			//Assert
			redirectUriPath.Should().Be("/Auth/GetAuthenticationToken");
		}

		[TestMethod]
		public void LoginRedirectResponseTypeQueryParameterShouldBeCode()
		{
			//Arrange
			var controller = this.ArrangeAuthController();

			//Act
			RedirectResult result = controller.Login();

			var resultUriQueryPart = UriHelper.GetQueryParameters(result.Url);

			//Assert
			resultUriQueryPart["response_type"].Should().Be("code");
		}

		[TestMethod]
		public void LoginRedirectScopeQueryParameterShouldContain()
		{
			//Arrange
			var controller = this.ArrangeAuthController();

			var expectedScopes = new List<string>
			{
				"playlist-read-private",
				"playlist-modify-public",
				"playlist-modify-private",
				"user-read-private"
			};

			//Act
			RedirectResult result = controller.Login();

			var resultUriQueryPart = UriHelper.GetQueryParameters(result.Url);

			//Assert
			resultUriQueryPart["scope"].Split(' ').Should().Contain(expectedScopes);
		}

		[TestMethod]
		public async Task GetAuthenticationTokenShouldRedirectWithErrorMessageSetOnSessionOnEmptyCodeParameter()
		{
			//Arrange
			var session = new Mock<HttpSessionStateBase>();
			var controller = this.ArrangeAuthController(session);

			//Act
			RedirectResult result = await controller.GetAuthenticationToken("", "");

			//Assert
			session.VerifySet(x => x["ErrorMessage"] = It.IsAny<string>());
			result.Url.Should().Be("/");
		}

		[TestMethod]
		public async Task GetAuthenticationTokenShouldRedirectWithErrorMessageSetOnSessionOnErrorCodeParameterAccessDenied()
		{
			//Arrange
			var session = new Mock<HttpSessionStateBase>();
			var controller = this.ArrangeAuthController(session);

			//Act
			RedirectResult result = await controller.GetAuthenticationToken("", "access_denied");

			//Assert
			session.VerifySet(x => x["ErrorMessage"] = It.Is<string>(z => z == "Spotify Acronym Playlist needs your permission to be able to create playlists. Please try again"));
			result.Url.Should().Be("/");
		}

		private AuthController ArrangeAuthController(Mock<HttpSessionStateBase> session = null)
		{
			if (session == null)
			{
				session = new Mock<HttpSessionStateBase>();
			}

			var uriBuilder = new UriBuilder("http://localhost/Auth/Login");

			var urlHelperStub = new Mock<UrlHelper>();
			urlHelperStub.Setup(uh => uh.Content("~")).Returns("/");

			var controller = new AuthController() { Url = urlHelperStub.Object };

			var httpContextBaseStub = new Mock<HttpContextBase>();
			httpContextBaseStub.SetupGet(hcb => hcb.Request.Url).Returns(uriBuilder.Uri);
			httpContextBaseStub.Setup(x => x.Session).Returns(session.Object);
			
			var requestContext = new RequestContext(httpContextBaseStub.Object, new RouteData());
			controller.ControllerContext = new ControllerContext(requestContext, controller) { HttpContext = httpContextBaseStub.Object };

			return controller;
		}
	}
}

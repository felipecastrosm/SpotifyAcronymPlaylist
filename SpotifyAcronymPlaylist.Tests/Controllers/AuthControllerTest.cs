using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
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

		private AuthController ArrangeAuthController()
		{
			var uriBuilder = new UriBuilder("http://localhost/Auth/Login");

			var httpContextBaseStub = new Mock<HttpContextBase>();
			httpContextBaseStub.SetupGet(hcb => hcb.Request.Url).Returns(uriBuilder.Uri);

			var controllerContext = new ControllerContext() { HttpContext = httpContextBaseStub.Object };

			var urlHelperStub = new Mock<UrlHelper>();
			urlHelperStub.Setup(uh => uh.Content("~")).Returns("/");

			var controller = new AuthController() { ControllerContext = controllerContext, Url = urlHelperStub.Object };

			return controller;
		}
	}
}

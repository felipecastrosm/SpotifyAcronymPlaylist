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
using SpotifyAcronymPlaylist.Models;
using SpotifyWebAPI;

namespace SpotifyAcronymPlaylist.Tests.Controllers
{
	[TestClass]
	public class PlaylistControllerTest
	{
		[TestMethod]
		public async Task IndexShouldRedirectToAuthLoginIfAuthenticationTokenNotSetInSession()
		{
			//Arrange
			var session = new Mock<HttpSessionStateBase>();
			session.Setup(s => s["Spotify.AuthenticationToken"]).Returns(null);
			var controller = this.ArrangePlaylistController(session);

			//Act
			ActionResult result = await controller.Index();

			//Assert
			result.Should().BeOfType<RedirectResult>();
			((RedirectResult) result).Url.Should().Be("/Auth/Login");
		}

		[TestMethod]
		public async Task IndexShouldCallSpotifyIntegrationModelMethodsUsingAuthenticationTokenSetInSession()
		{
			//Arrange
			var session = new Mock<HttpSessionStateBase>();
			var authenticationTokenStub = new Mock<AuthenticationToken>();
			session.Setup(s => s["Spotify.AuthenticationToken"]).Returns(authenticationTokenStub.Object);

			var spotifyIntegrationModelStub = new Mock<ISpotifyIntegrationModel>();
			var controller = new PlaylistController(spotifyIntegrationModelStub.Object);

			this.MockHttpContextAndRequestContextAndControllerContext(ref controller, session.Object);

			//Act
			ActionResult result = await controller.Index();

			//Assert
			spotifyIntegrationModelStub.Verify(sim => sim.GetUserId(authenticationTokenStub.Object));
			spotifyIntegrationModelStub.Verify(sim => sim.GenerateAcronymPlaylist(authenticationTokenStub.Object));
		}

		[TestMethod]
		public async Task IndexShouldSetUserIdOnViewBag()
		{
			//Arrange
			var controller = this.ArrangePlaylistController();

			//Act
			ActionResult result = await controller.Index();

			var userId = controller.ViewBag.UserId as string;

			//Assert
			userId.Should().NotBeNull();
			userId.Should().Be("testUserId");
		}

		[TestMethod]
		public async Task IndexShouldSetAcronymPlaylistOnViewBag()
		{
			//Arrange
			var sampleTrack = new Track { Id = "sampleTrackId", Name = "SampleTrackName" };
			var sampleTrackList = new List<Track> { sampleTrack };

			var controller = this.ArrangePlaylistController(sampleTrackList: sampleTrackList);

			//Act
			ActionResult result = await controller.Index();

			var returnedTrackList = controller.ViewBag.AcronymPlaylist as List<Track>;

			//Assert
			returnedTrackList.Should().NotBeNullOrEmpty();
			returnedTrackList.Should().BeEquivalentTo(sampleTrackList);
			returnedTrackList.First().Id.Should().Be(sampleTrack.Id);
		}

		private PlaylistController ArrangePlaylistController(Mock<HttpSessionStateBase> session = null, List<Track> sampleTrackList = null)
		{
			if (session == null)
			{
				session = new Mock<HttpSessionStateBase>();
				var authenticationTokenStub = new Mock<AuthenticationToken>();
				session.Setup(s => s["Spotify.AuthenticationToken"]).Returns(authenticationTokenStub.Object);
			}

			var spotifyIntegrationModelStub = new Mock<ISpotifyIntegrationModel>();
			spotifyIntegrationModelStub.Setup(sim => sim.GetUserId(It.IsAny<AuthenticationToken>())).ReturnsAsync("testUserId");
			spotifyIntegrationModelStub.Setup(sim => sim.GenerateAcronymPlaylist(It.IsAny<AuthenticationToken>())).ReturnsAsync(sampleTrackList);

			var controller = new PlaylistController(spotifyIntegrationModelStub.Object);

			this.MockHttpContextAndRequestContextAndControllerContext(ref controller, session.Object);

			return controller;
		}

		private void MockHttpContextAndRequestContextAndControllerContext(ref PlaylistController controller, HttpSessionStateBase session)
		{
			var httpContextBaseStub = new Mock<HttpContextBase>();
			httpContextBaseStub.Setup(x => x.Session).Returns(session);

			var requestContext = new RequestContext(httpContextBaseStub.Object, new RouteData());
			controller.ControllerContext = new ControllerContext(requestContext, controller) { HttpContext = httpContextBaseStub.Object };
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SpotifyAcronymPlaylist.Models;
using Spotify = SpotifyWebAPI;

namespace SpotifyAcronymPlaylist.Controllers
{
	public class PlaylistController : Controller
	{
		public ISpotifyIntegrationModel SpotifyIntegrationModel { get; set; }

		public PlaylistController(ISpotifyIntegrationModel spotifyIntegrationModel)
		{
			this.SpotifyIntegrationModel = spotifyIntegrationModel;
		}

		public async Task<ActionResult> Index()
		{
			var spotifyAuthenticationToken = (Spotify.AuthenticationToken) ControllerContext.HttpContext.Session["Spotify.AuthenticationToken"];

			if (spotifyAuthenticationToken == null)
			{
				return new RedirectResult("/Auth/Login");
			}

			ViewBag.UserId = await this.SpotifyIntegrationModel.GetUserId(spotifyAuthenticationToken);

			List<Spotify.Track> acronymTracks = await this.SpotifyIntegrationModel.GenerateAcronymPlaylist(spotifyAuthenticationToken);

			ViewBag.AcronymPlaylist = acronymTracks;

			return View();
		}
	}
}
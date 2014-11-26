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
			var username = await this.SpotifyIntegrationModel.GetUserDisplayName(spotifyAuthenticationToken);

			ViewBag.Username = username ?? ViewBag.UserId;

			List<Spotify.Track> acronymTracks = await this.SpotifyIntegrationModel.GenerateAcronymPlaylist(spotifyAuthenticationToken);

			ViewBag.AcronymPlaylist = acronymTracks;

			return View();
		}

		public async Task<JsonResult> Save(string playlistName, bool isPublic, List<string> trackIds, bool overwrite = false)
		{
			var spotifyAuthenticationToken = (Spotify.AuthenticationToken)ControllerContext.HttpContext.Session["Spotify.AuthenticationToken"];

			if (spotifyAuthenticationToken == null)
			{
				return null;
			}

			var playlistCreated = await this.SpotifyIntegrationModel.CreatePlaylist(spotifyAuthenticationToken, playlistName, isPublic, trackIds);

			Dictionary<string, string> messageDictionary;

			if (playlistCreated)
			{
				messageDictionary = new Dictionary<string, string>
				{
					{"status", "s"},
					{"message", "Playlist successfuly saved! Enjoy!"}
				};
			}
			else
			{
				messageDictionary = new Dictionary<string, string>
				{
					{"status", "e"},
					{"message", "Whoops! An error ocurred trying to save your Playlist. Please try again"}
				};
			}

			return Json(messageDictionary);
		}
	}
}
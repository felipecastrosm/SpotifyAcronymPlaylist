using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Spotify = SpotifyWebAPI;

namespace SpotifyAcronymPlaylist.Controllers
{
	public class PlaylistController : Controller
	{
		public async Task<ActionResult> Index()
		{
			ViewBag.User = await Spotify.User.GetCurrentUserProfile((Spotify.AuthenticationToken)Session["Spotify.AuthenticationToken"]);

			return View();
		}
	}
}
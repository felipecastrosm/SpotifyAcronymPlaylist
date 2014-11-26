using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpotifyAcronymPlaylist.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
			//TODO: If Session["ErrorMessage"] is not null, show error message on View

            return View();
        }
    }
}
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
	        if (Session["ErrorMessage"] != null)
	        {
		        ViewBag.ErrorMessage = Session["ErrorMessage"];

		        Session["ErrorMessage"] = null;
	        }

            return View();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SpotifyAcronymPlaylist.Helpers;
using Spotify = SpotifyWebAPI;

namespace SpotifyAcronymPlaylist.Controllers
{
    public class AuthController : Controller
    {
		private const string SpotifyAuthorizeUri = "https://accounts.spotify.com/authorize";

		private const string SpotifyClientId = "1800c8f79b134e36ad9152792620ad84";

		private const string SpotifyClientSecret = "456f0f0eaf554075b03435f3e6ec68e3";

	    private string GetRedirectUri()
	    {
			string baseUri = string.Format("{0}://{1}{2}", ControllerContext.HttpContext.Request.Url.Scheme, ControllerContext.HttpContext.Request.Url.Authority, Url.Content("~"));

			string redirectUri = baseUri + "Auth/GetAuthenticationToken";

		    return redirectUri;
	    }

        public RedirectResult Login()
        {
            string[] scope = { "playlist-read-private", "playlist-modify-public", "playlist-modify-private", "user-read-private" };

			var apiUriBuilder = new UriBuilder(SpotifyAuthorizeUri) { Port = -1 };

			var uriQueryPart = HttpUtility.ParseQueryString(apiUriBuilder.Query);
			
			uriQueryPart["client_id"] = SpotifyClientId;
			uriQueryPart["response_type"] = "code";
			uriQueryPart["redirect_uri"] = this.GetRedirectUri();
			uriQueryPart["scope"] = String.Join(" ", scope);

			apiUriBuilder.Query = uriQueryPart.ToString();
			string finalAuthenticationUri = apiUriBuilder.ToString();

			return new RedirectResult(finalAuthenticationUri);
        }

	    public async Task<RedirectResult> GetAuthenticationToken(string code, string error)
	    {
		    if (String.IsNullOrEmpty(code))
		    {
			    if (!String.IsNullOrEmpty(error))
			    {
				    //TODO: Handle all possible error types on callback
				    switch (error)
				    {
					    case "access_denied":
							ErrorMessageHelper.SetMessage(ControllerContext.HttpContext.Session, "Spotify Acronym Playlist needs your permission to be able to create playlists. Please try again");
						    break;
						default:
							ErrorMessageHelper.SetMessage(ControllerContext.HttpContext.Session, "An unrecornized error code was returned. Please try again");
						    break;
				    }
			    }
			    else
			    {
					ErrorMessageHelper.SetMessage(ControllerContext.HttpContext.Session, "An unexpected error ocurred during Spotify authentication. Please try again");
			    }

				return new RedirectResult("/");
		    }

		    Spotify.Authentication.ClientId = SpotifyClientId;
		    Spotify.Authentication.ClientSecret = SpotifyClientSecret;
		    Spotify.Authentication.RedirectUri = this.GetRedirectUri();

		    Spotify.AuthenticationToken authToken = await Spotify.Authentication.GetAccessToken(code);

			ControllerContext.HttpContext.Session.Add("Spotify.AuthenticationToken", authToken);

			return new RedirectResult("/Playlist");
	    }
    }
}
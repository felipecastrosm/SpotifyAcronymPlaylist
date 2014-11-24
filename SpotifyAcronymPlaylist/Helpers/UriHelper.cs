using System;
using System.Collections.Specialized;
using System.Web;

namespace SpotifyAcronymPlaylist.Helpers
{
	public static class UriHelper
	{
		public static NameValueCollection GetQueryParameters(string uri)
		{
			var uriBuilder = new UriBuilder(uri) { Port = -1 };

			var uriQueryPart = HttpUtility.ParseQueryString(uriBuilder.Query);

			return uriQueryPart;
		}

		public static string GetPath(string uri)
		{
			var uriBuilder = new UriBuilder(uri) { Port = -1 };

			return uriBuilder.Path;
		}
	}
}
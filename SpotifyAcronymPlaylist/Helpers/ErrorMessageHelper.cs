using System.Web;

namespace SpotifyAcronymPlaylist.Helpers
{
	public static class ErrorMessageHelper
	{
		public static void SetMessage(HttpSessionStateBase session, string message)
		{
			if (session["ErrorMessage"] == null)
			{
				session["ErrorMessage"] = message;
			}
			else
			{
				session["ErrorMessage"] += "\n" + message;
			}
		}
	}
}
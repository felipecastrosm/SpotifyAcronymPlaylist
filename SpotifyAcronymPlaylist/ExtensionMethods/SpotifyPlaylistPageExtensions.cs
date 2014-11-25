using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using SpotifyWebAPI;

namespace SpotifyAcronymPlaylist.ExtensionMethods
{
	public static class SpotifyPlaylistPageExtensions
	{
		public static async Task<List<Playlist>> AllPagesToList(this Page<Playlist> playlistPage)
		{
			var playlists = new List<Playlist>();

			playlists.AddRange(await playlistPage.ToList());

			var currentPage = playlistPage;

			while (currentPage.HasNextPage)
			{
				currentPage = await currentPage.GetNextPage();

				playlists.AddRange(await currentPage.ToList());
			}

			return playlists;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using SpotifyAcronymPlaylist.ExtensionMethods;
using Spotify = SpotifyWebAPI;

namespace SpotifyAcronymPlaylist.Models
{
	public class SpotifyIntegrationModel : ISpotifyIntegrationModel
	{
		public async Task<List<Spotify.Track>> GenerateAcronymPlaylist(Spotify.AuthenticationToken authenticationToken)
		{
			var userPlaylists = await this.GetAllCurrentUserPlaylists(authenticationToken);

			if (userPlaylists == null)
			{
				return null;
			}

			var playlistTracks = (await Task.WhenAll(userPlaylists.Select(async p => await (await p.GetPlaylistTracks(authenticationToken)).ToList()))).SelectMany(p => p).ToList();

			if (playlistTracks.Count == 0)
			{
				return null;
			}

			var displayName = await this.GetUserDisplayName(authenticationToken);

			if (String.IsNullOrEmpty(displayName))
			{
				displayName = await this.GetUserId(authenticationToken);
			}

			var acronymPlaylist = new List<Spotify.Track>();

			foreach (var letter in displayName)
			{
				var track = playlistTracks.FirstOrDefault(t => t.Track.Name.ToLower().StartsWith(letter.ToString()));

				if (track == null)
				{
					//TODO: Add default track if none was found with corresponding initial letter
					//track = defaultTrack
				}

				acronymPlaylist.Add(track.Track);
			}

			return acronymPlaylist;
		}

		public async Task<string> GetUserId(Spotify.AuthenticationToken authenticationToken)
		{
			var user = await Spotify.User.GetCurrentUserProfile(authenticationToken);

			return user.Id;
		}

		public async Task<string> GetUserDisplayName(Spotify.AuthenticationToken authenticationToken)
		{
			var user = await Spotify.User.GetCurrentUserProfile(authenticationToken);

			return user.DisplayName;
		}

		public async Task<string> GetUserFollowers(Spotify.AuthenticationToken authenticationToken, string userId)
		{
			var httpClient = new HttpClient();

			string spotifyUserUri = String.Format("https://api.spotify.com/v1/users/{0}", userId);

			string userJsonData = await httpClient.GetStringAsync(spotifyUserUri);

			var userObject = JObject.Parse(userJsonData);

			int followersAmount = Int32.Parse(userObject.SelectToken("followers/total").ToString());

			if (followersAmount == 0)
			{
				return null;
			}

			string followersUri = userObject.SelectToken("followers/href").ToString();

			string followersJsonData = await httpClient.GetStringAsync(followersUri);

			var followersObject = JObject.Parse(followersJsonData);

			//TODO: Não há forma conhecida de buscar informação de followers, e a URL retornada é sempre null - avaliar questão

			return null;
		}

		public async Task<List<Spotify.Playlist>> GetAllCurrentUserPlaylists(Spotify.AuthenticationToken authenticationToken)
		{
			var spotifyUser = await Spotify.User.GetCurrentUserProfile(authenticationToken);

			var userPlaylistsPage = await spotifyUser.GetPlaylists(authenticationToken);

			var playlists = await userPlaylistsPage.AllPagesToList();

			return playlists;
		}
	}
}
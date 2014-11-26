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
			Spotify.Track defaultTrack = null;

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
				//Ordering made to avoid repeating the same song if there are others available for the same letter
				var playlistTrack = playlistTracks.OrderBy(pt => acronymPlaylist.Contains(pt.Track)).FirstOrDefault(pt => pt.Track.Name.ToLower().StartsWith(letter.ToString()));

				Spotify.Track track;

				if (playlistTrack == null)
				{
					if (defaultTrack == null)
					{
						defaultTrack = await Spotify.Track.GetTrack("0vGGptKFy2B0ETY1cn7n19");
					}

					track = defaultTrack;
				}
				else
				{
					track = playlistTrack.Track;
				}

				acronymPlaylist.Add(track);
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
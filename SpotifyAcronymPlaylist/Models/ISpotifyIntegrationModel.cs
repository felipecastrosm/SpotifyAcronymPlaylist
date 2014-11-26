using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotifyAcronymPlaylist.Models
{
	public interface ISpotifyIntegrationModel
	{
		Task<List<SpotifyWebAPI.Track>> GenerateAcronymPlaylist(SpotifyWebAPI.AuthenticationToken authenticationToken);
		Task<string> GetUserId(SpotifyWebAPI.AuthenticationToken authenticationToken);
		Task<string> GetUserDisplayName(SpotifyWebAPI.AuthenticationToken authenticationToken);
		Task<string> GetUserFollowers(SpotifyWebAPI.AuthenticationToken authenticationToken, string userId);
		Task<List<SpotifyWebAPI.Playlist>> GetAllCurrentUserPlaylists(SpotifyWebAPI.AuthenticationToken authenticationToken);
		Task<bool> CreatePlaylist(SpotifyWebAPI.AuthenticationToken authenticationToken, string playlistName, bool isPublic, List<string> trackIds);
	}
}
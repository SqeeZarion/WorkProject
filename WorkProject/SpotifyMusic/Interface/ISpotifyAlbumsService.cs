using SpotifyWebApi.Models;

namespace SpotifyWebApi.Interface;

public interface ISpotifyAlbumsService
{

    Task<IEnumerable<Album>> GetReleaseAsync(string countryCode, int limit, string accessToken);
    Task<AlbumDetails> GetAlbumAsync(string id, string accessToken);
    Task<Tracks> GetListTracksAsync(string albumId, string accessToken, int limit, int offset);
}
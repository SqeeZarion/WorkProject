using SpotifyWebApi.Models;

namespace WorkProject.Models.External;

public class SpotifyTracksResponse
{
    public List<SpotifyTrack> items { get; set; }
}

public class SpotifySavedTracksResponse
{
    public List<SpotifySavedTrack> items { get; set; }
}

public class SpotifyRecentlyPlayedResponse
{
    public List<SpotifyRecentlyPlayedItem> items { get; set; }
}

public class SpotifyTrack
{
    public SpotifyGetAlbumforTrack Album { get; set; }
    public string Id { get; set; }
    public int Popularity { get; set; }
    public string Name { get; set; }
    public int TrackNumber { get; set; }
    public External_urls ExternalUrls { get; set; }
    public string Uri { get; set; }
    public List<SpotifyArtist> Artists { get; set; }
}

public class Tracks
{
    public string href { get; set; }
    public int offset { get; set; }
    public List<SpotifyTrack> items { get; set; }
}

public class SpotifySavedTrack
{
    public SpotifyTrack track { get; set; }
}

public class SpotifyRecentlyPlayedItem
{
    public SpotifyTrack track { get; set; }
    public string played_at { get; set; }
}

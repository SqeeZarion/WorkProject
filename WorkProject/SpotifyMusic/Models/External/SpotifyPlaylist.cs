namespace WorkProject.Models.External;

public class SpotifyPlaylistsResponse
{
    public List<SpotifyPlaylistItem> items { get; set; }
}

public class SpotifyPlaylistItem
{
    public string id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public SpotifyPlaylistOwner owner { get; set; }
    public bool @public { get; set; }
    public List<SpotifyImage> images { get; set; }
    public SpotifyExternalUrls external_urls { get; set; }
    public SpotifyTracksInfo tracks { get; set; }
}

public class SpotifyPlaylistOwner
{
    public string id { get; set; }
    public string display_name { get; set; }
}

public class SpotifyExternalUrls
{
    public string spotify { get; set; }
}

public class SpotifyTracksInfo
{
    public int total { get; set; }
}

public class SpotifyImage
{
    public int? height { get; set; }
    public int? width { get; set; }
    public string url { get; set; }
}
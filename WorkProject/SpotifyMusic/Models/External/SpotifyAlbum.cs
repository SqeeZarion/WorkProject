using SpotifyApi.NetCore;
using SpotifyWebApi.Models;


namespace WorkProject.Models.External;

public class SpotifyAlbum
{
    public string album_type { get; set; }
    public List<Artist> Artists { get; set; }
    public List<string> available_markets { get; set; }
    public int Popularity { get; set; }
    public ExternalUrls ExternalUrls { get; set; }
    public string href { get; set; }
    public string Id { get; set; }
    public List<Image> Images { get; set; }
    public string Name { get; set; }
    public string release_date { get; set; }
    public string release_date_precision { get; set; }
    public int TotalTracks { get; set; }
    public string Type { get; set; }
    public string Uri { get; set; }
}

public class Image
{
    public int Height { get; set; }
    public string Url { get; set; }
    public int Width { get; set; }
}

//Для запитів альбомів напряму для альбомів

public class SpotifyAlbumsResponse
{
    public Albums Albums { get; set; }
}

public class Albums
{
    public List<SpotifyAlbum> Items { get; set; }
}

public class SpotifyGetAlbumforTrack
{
    public string id { get; set; } // Зроби string, бо так у Spotify
    public string name { get; set; }
    public string release_date { get; set; }
    public List<Images> images { get; set; }
    public Tracks tracks { get; set; }
}
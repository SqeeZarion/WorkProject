using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SpotifyApi.NetCore;


namespace SpotifyWebApi.Models;

public class Album
{
    public string album_type { get; set; }
    public List<Artist> artists { get; set; }
    public List<string> available_markets { get; set; }
    public ExternalUrls external_urls { get; set; }
    public string href { get; set; }
    public string id { get; set; }
    public List<Image> images { get; set; }
    public string name { get; set; }
    public string release_date { get; set; }
    public string release_date_precision { get; set; }
    public int total_tracks { get; set; }
    public string type { get; set; }
    public string uri { get; set; }
}

public class Image
{
    public int height { get; set; }
    public string url { get; set; }
    public int width { get; set; }
}

public class AlbumDetails 
{
    public string Name { get; set; }
    public string AlbumType { get; set; }
    public int TotalTracks { get; set; }
    public string Artist { get; set; }
    public string Date { get; set; }
    public string ImageUrl { get; set; }
    public string LinqUrl { get; set; }
}

public class Albums
{
    public string href { get; set; }
    public List<Album> Items { get; set; }
}

public class SpotifyAlbumResponse
{
    public string Name { get; set; }
    public string AlbumType { get; set; }
    public int TotalTracks { get; set; }
    public string ReleaseDate { get; set; }
    public List<Images> Images { get; set; }
    public External_urls ExternalUrls { get; set; }
    public List<Artists> Artists { get; set; }
}


public class ActAlbum
{
    [Key]
    public int ActAlbumId  { get; set; }
    
    [ForeignKey("Act")]
    public int ActId { get; set; }
    
    [ForeignKey("Album")]
    public int AlbumId { get; set; }
}


public class AlbumRecording
{
    [Key]
    public int AlbumRecordingId { get; set;}
    
    [ForeignKey("Album")]
    public int AlbumId { get; set; }
    
    [ForeignKey("Recording")]
    public double RecordingId { get; set; }
    
    public int TrackNumber { get; set; }
}

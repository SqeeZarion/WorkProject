using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpotifyWebApi.Models;

public class Album
{
    public string AlbumName { get; set; }
    public string AlbumType { get; set; }
    public int TotalTracks { get; set; }
    public string Artist { get; set; }
    public string Date { get; set; }
    public string ImageUrl { get; set; }
    public string LinqUrl { get; set; }
    [Key]
    public int AlbumId { get; set; }
    public string ReleaseDate { get; set; }
    public virtual ICollection<Images> Images { get; set; }
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
    public string Href { get; set; }
    public int Limit { get; set; }
    public string Next { get; set; }
    public int Offset { get; set; }
    public string Previous { get; set; }
    public int Total { get; set; }
    public List<Items> Items { get; set; }
    public Track[] Tracks { get; set; }
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

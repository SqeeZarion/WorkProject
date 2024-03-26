using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpotifyWebApi.Models;

public class GetAlbumforTrack
{
    public int id { get; set; }
    public string name { get; set; }
    public string release_date { get; set; }
    public List<Images> images { get; set; }
    public Tracks tracks { get; set; }
}

public class Tracks
{
 
    public string ID { get; set; }
    public string href { get; set; }
    public int offset { get; set; }
    public List<Track> items { get; set; }
}

public class Track
{
    public  GetAlbumforTrack album { get; set; }
    public string id { get; set; }
    public string name { get; set; }
    public int track_number { get; set; }
    public External_urls external_urls { get; set; }
    public string uri { get; set; }
    public List<Artist> artists { get; set; }
}

// public class Artist
// {
//     public string id { get; set; }
//     public string name { get; set; }
//     public External_urls external_urls { get; set; }
//     public string uri { get; set; }
// }

public class Song //change on Track
{
    [Key]
    public int SongId { get; set; }
    public string SongName { get; set; }
    public DateTime SongDate { get; set; }
}
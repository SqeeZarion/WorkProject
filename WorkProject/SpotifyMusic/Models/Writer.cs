using System.ComponentModel.DataAnnotations.Schema;

namespace SpotifyWebApi.Models;

public class Writer
{
    public int WriterId { get; set; }
    
    [ForeignKey("Artist")]
    public int ArtistId { get; set;}
    
    [ForeignKey("Song")]
    public int SongId { get; set; }
}
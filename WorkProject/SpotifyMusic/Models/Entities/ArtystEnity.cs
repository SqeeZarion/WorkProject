using System.ComponentModel.DataAnnotations;

namespace SpotifyWebApi.Models.Entities;

public class ArtistEnity
{
    [Key]
    public int ArtistId { get; set; }
    public string SpotifyId { get; set; }
    public string Name { get; set; }
}
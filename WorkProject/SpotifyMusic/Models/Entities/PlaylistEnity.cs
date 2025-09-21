using System.Text.Json.Serialization;

namespace WorkProject.Models.Entities;

using System.ComponentModel.DataAnnotations; 
using System.ComponentModel.DataAnnotations.Schema;

public class PlaylistEnity
{
    [Key]
    public int PlaylistId { get; set; }
    public string SpotifyId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string OwnerName { get; set; }
    public bool IsPublic { get; set; }
    public string ImageUrl { get; set; }
    public string SpotifyUrl { get; set; }           // Посилання на Spotify
    public int TotalTracks { get; set; }             // Кількість треків
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public virtual ICollection<PlaylistTrackEnity> PlaylistTracks { get; set; }
}

public class PlaylistTrackEnity
{
    [Key]
    public int PlaylistTrackId { get; set; }

    public int PlaylistId { get; set; }
    [ForeignKey("PlaylistId")]
    public PlaylistEnity PlaylistEnity { get; set; }

    public int TrackId { get; set; }
    [ForeignKey("TrackId")]
    public TrackEntity Track { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
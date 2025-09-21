using System.Text.Json.Serialization;
using SpotifyWebApi.Models.Entities;

namespace WorkProject.Models.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class UserEnity
{
    [Key]
    public int UserId { get; set; }
    public string SpotifyUserId { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public string Country { get; set; }
    
    public string SpotifyProfileUrl { get; set; }
    public string ImageUrl { get; set; }
    public string Product { get; set; } // Тип підписки: free, premium, open

    public string Uri { get; set; }
    public string Href { get; set; }
    
    // ✅ Only store refresh token
    public string RefreshToken { get; set; }

    // ⏳ Optional — helps decide when to refresh
    public DateTime? TokenExpiresAt { get; set; }

    [JsonIgnore]
    public virtual ICollection<UserTrackLogEnity> TrackLogs { get; set; }
    [JsonIgnore]
    public virtual ICollection<SavedSpotifyAlbumEnity> SavedAlbums { get; set; }
    [JsonIgnore]
    public virtual ICollection<UserAlbumEnity> UserAlbum { get; set; }
}

public class UserTrackLogEnity
{
    [Key]
    public int LogId { get; set; }

    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public UserEnity UserEnity { get; set; }

    public string TrackId { get; set; } // Spotify track ID
    public string TrackName { get; set; }
    public string ArtistName { get; set; }
    public string Genre { get; set; }
    public DateTime PlayedAt { get; set; }
    public int Popularity { get; set; }
    public int DurationMs { get; set; }
}
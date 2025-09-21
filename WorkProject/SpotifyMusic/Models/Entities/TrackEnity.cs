using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using SpotifyWebApi.Models.Entities;

namespace WorkProject.Models.Entities;

public class TrackEntity
{
    [Key]
    public int TrackId { get; set; }
    public string TrackName { get; set; }
    public DateTime TrackDate { get; set; }

    public int? AlbumId { get; set; }
    [ForeignKey("AlbumId")]
    public UserAlbumEnity UserAlbum { get; set; }

    [JsonIgnore]
    public virtual ICollection<TrackArtistEnity> TrackArtists { get; set; }
    [JsonIgnore]
    public virtual ICollection<PlaylistTrackEnity> PlaylistTracks { get; set; }
}


//допоміжна таблицю для зв’язку N:M між піснями й артистами:
public class TrackArtistEnity
{
    [Key]
    public int TrackArtistId { get; set; }

    public int TrackId { get; set; }
    [ForeignKey("TrackId")]
    public TrackEntity Track { get; set; }

    public int ArtistId { get; set; }
    [ForeignKey("ArtistId")]
    public ArtistEnity ArtistEnity { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using WorkProject.Models.Entities;

namespace SpotifyWebApi.Models.Entities;

public class UserAlbumEnity
{
    [Key]
    public int Id { get; set; }
    public string SpotifyId { get; set; }
    public string Name { get; set; }
    public string AlbumType { get; set; }
    public int Popularity { get; set; }
    public int TotalTracks { get; set; }
    public string ArtistName { get; set; }
    public string ReleaseDate { get; set; }
    public string ImageUrl { get; set; }
    public string SpotifyUrl { get; set; }
    
    [JsonIgnore]
    // 🔗 Зв’язок із треками
    public virtual ICollection<TrackEntity> Tracks { get; set; }
    [JsonIgnore]
    // 🔗 Зв’язок із артистами (мультиартист альбоми)
    public virtual ICollection<AlbumArtistEnity> AlbumArtists { get; set; }
}

//допоміжну таблицю для зв’язку N:M між альбомами й артистами
public class AlbumArtistEnity
{
    [Key]
    public int AlbumArtistId { get; set; }

    public int AlbumId { get; set; }
    [ForeignKey("AlbumId")]
    public UserAlbumEnity UserAlbumEnity { get; set; }

    public int ArtistId { get; set; }
    [ForeignKey("ArtistId")]
    public ArtistEnity ArtistEnity { get; set; }
}

public class SavedSpotifyAlbumEnity
{
    [Key]
    public int Id { get; set; }

    public string SpotifyId { get; set; }
    public string Name { get; set; }
    public string AlbumType { get; set; }
    public int Popularity { get; set; }
    public int TotalTracks { get; set; }
    public string ArtistNames { get; set; } // через кому
    public string ReleaseDate { get; set; }
    public string SpotifyUrl { get; set; }
    public string ImageUrl { get; set; }

    // 📌 Дата, коли користувач зберіг альбом
    public DateTime SavedAt { get; set; } = DateTime.UtcNow;

    // 🔗 Зв’язок із користувачем (якщо треба)
    public int? UserId { get; set; }
    // public virtual User User { get; set; } // якщо є User

    [JsonIgnore]
    // 🔗 Можливі майбутні зв'язки
    public virtual ICollection<SavedAlbumArtistEnity> AlbumArtists { get; set; }
}

//Допоміжна N:M таблиця для артистів
public class SavedAlbumArtistEnity
{
    [Key]
    public int Id { get; set; }

    public int AlbumId { get; set; }
    [ForeignKey("AlbumId")]
    public SavedSpotifyAlbumEnity AlbumEnity { get; set; }

    public int ArtistId { get; set; }
    [ForeignKey("ArtistId")]
    public ArtistEnity ArtistEnity { get; set; }
}



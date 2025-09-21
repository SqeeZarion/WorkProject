using Microsoft.EntityFrameworkCore;
using SpotifyWebApi.Models;
using SpotifyWebApi.Models.Entities;
using WorkProject.Models.Entities;
using WorkProject.Models.External;

namespace SpotifyWebApi.Database;

public class DbConnection : DbContext
{
    public DbConnection(DbContextOptions<DbConnection> options) : base(options)
    {
    }
    
    public DbSet<UserEnity> Users { get; set; }
    public DbSet<UserTrackLogEnity> UserTrackLogs { get; set; }
    public DbSet<UserAlbumEnity> UserAlbums { get; set; }
    public DbSet<AlbumArtistEnity> AlbumArtists { get; set; }
    public DbSet<SavedSpotifyAlbumEnity> SavedSpotifyAlbums { get; set; }
    public DbSet<SavedAlbumArtistEnity> SavedAlbumArtists { get; set; }
    public DbSet<ArtistEnity> Artists { get; set; }
    public DbSet<TrackEntity> Tracks { get; set; }
    public DbSet<TrackArtistEnity> TrackArtists { get; set; }
    public DbSet<PlaylistEnity> Playlists { get; set; }
    public DbSet<PlaylistTrackEnity> PlaylistTracks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Уникнення повторних зв'язків
        // Зв’язок AlbumArtistEnity ↔ UserAlbumEnity ↔ ArtistEnity
        modelBuilder.Entity<AlbumArtistEnity>()
            .HasOne(aa => aa.UserAlbumEnity)
            .WithMany(a => a.AlbumArtists)
            .HasForeignKey(aa => aa.AlbumId);

        modelBuilder.Entity<AlbumArtistEnity>()
            .HasOne(aa => aa.ArtistEnity)
            .WithMany()
            .HasForeignKey(aa => aa.ArtistId);

        modelBuilder.Entity<SavedAlbumArtistEnity>()
            .HasOne(saa => saa.AlbumEnity)
            .WithMany(a => a.AlbumArtists)
            .HasForeignKey(saa => saa.AlbumId);

        modelBuilder.Entity<SavedAlbumArtistEnity>()
            .HasOne(saa => saa.ArtistEnity)
            .WithMany()
            .HasForeignKey(saa => saa.ArtistId);

        modelBuilder.Entity<TrackArtistEnity>()
            .HasOne(ta => ta.Track)
            .WithMany(t => t.TrackArtists)
            .HasForeignKey(ta => ta.TrackId);

        modelBuilder.Entity<TrackArtistEnity>()
            .HasOne(ta => ta.ArtistEnity)
            .WithMany()
            .HasForeignKey(ta => ta.ArtistId);

        modelBuilder.Entity<PlaylistTrackEnity>()
            .HasOne(pt => pt.PlaylistEnity)
            .WithMany(p => p.PlaylistTracks)
            .HasForeignKey(pt => pt.PlaylistId);

        modelBuilder.Entity<PlaylistTrackEnity>()
            .HasOne(pt => pt.Track)
            .WithMany(t => t.PlaylistTracks)
            .HasForeignKey(pt => pt.TrackId);
        
        modelBuilder.Entity<UserEnity>()
            .HasIndex(u => new { u.SpotifyUserId })
            .IsUnique();

    }
}
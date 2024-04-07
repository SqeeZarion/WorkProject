using Microsoft.EntityFrameworkCore;
using SpotifyWebApi.Models;

namespace SpotifyWebApi.Database;

public class DbConnection : DbContext
{
    public DbConnection(DbContextOptions<DbConnection> options) : base(options)
    {
    
    }
    
    public DbSet<Tracks> Tracks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tracks>().ToTable("UserLoveTracks");
        modelBuilder.Entity<Album>().ToTable("UsersLoveAlbums");
    }
}
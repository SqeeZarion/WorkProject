using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;

namespace WebApplication1.Database
{
    public class DBConnection : DbContext
    {
        protected  void disconnect(){}
        public DBConnection(DbContextOptions<DBConnection> options) : base(options)
        {

        }
        
        public DbSet<UserAccount> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAccount>().ToTable("Users");
        }
    }
}
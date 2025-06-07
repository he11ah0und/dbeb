using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using DiplomBeb.Database.Model;

namespace DiplomBeb.Database
{
    public class MainDBContext : DbContext
    {
        public MainDBContext(DbContextOptions<MainDBContext> options)
            : base(options)
        {
            
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRank>(entity =>
            {
                entity.HasNoKey(); // Mark as keyless
                entity.ToView(null); // Not mapped to a database view/table
            });
        }
        
        public DbSet<User> Users { get; set; }

        public DbSet<Document> Documents { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<ApiRole> ApiRoles { get; set; }
        public DbSet<WebRole> WebRoles { get; set; }
        public DbSet<Library> Library { get; set; }
    }
}

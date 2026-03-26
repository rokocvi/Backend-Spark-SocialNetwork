using Microsoft.EntityFrameworkCore;
using Spark.API.Models;

namespace Spark.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Tablice
        public DbSet<User> Users { get; set; }
        public DbSet<UserSpark> Sparks { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Username).IsUnique();
            });

           
            modelBuilder.Entity<UserSpark>(entity =>
            {
                // Jedan korisnik, jedan Spark po danu
                entity.HasIndex(s => new { s.UserId, s.SparkDate }).IsUnique();

                // Konfiguracija Tags arraya za PostgreSQL
                entity.Property(s => s.Tags)
                      .HasColumnType("text[]");

                // Relacija Spark -> User
                entity.HasOne(s => s.User)
                      .WithMany(u => u.Sparks)
                      .HasForeignKey(s => s.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Match>(entity =>
            {
                // Jedan par korisnika, jedan match po danu
                entity.HasIndex(m => new { m.User1Id, m.User2Id, m.MatchDate }).IsUnique();

                // Relacija Match -> User1
                entity.HasOne(m => m.User1)
                      .WithMany()
                      .HasForeignKey(m => m.User1Id)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relacija Match -> User2
                entity.HasOne(m => m.User2)
                      .WithMany()
                      .HasForeignKey(m => m.User2Id)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relacija Match -> Spark1
                entity.HasOne(m => m.Spark1)
                      .WithMany()
                      .HasForeignKey(m => m.Spark1Id)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relacija Match -> Spark2
                entity.HasOne(m => m.Spark2)
                      .WithMany()
                      .HasForeignKey(m => m.Spark2Id)
                      .OnDelete(DeleteBehavior.Restrict);
            });

          
            modelBuilder.Entity<Message>(entity =>
            {
                // Relacija Message -> Match
                entity.HasOne(msg => msg.Match)
                      .WithMany(m => m.Messages)
                      .HasForeignKey(msg => msg.MatchId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relacija Message -> User (Sender)
                entity.HasOne(msg => msg.Sender)
                      .WithMany(u => u.Messages)
                      .HasForeignKey(msg => msg.SenderId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
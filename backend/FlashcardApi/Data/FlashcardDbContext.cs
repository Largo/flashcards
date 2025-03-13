using Microsoft.EntityFrameworkCore;
using FlashcardApi.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FlashcardApi.Data
{
    public class FlashcardDbContext : DbContext
    {
        public FlashcardDbContext(DbContextOptions<FlashcardDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Flashcard> Flashcards { get; set; }
        public DbSet<StudySession> StudySessions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => 
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
            
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
                
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
                
            modelBuilder.Entity<Flashcard>()
                .HasOne(f => f.User)
                .WithMany(u => u.Flashcards)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<StudySession>()
                .HasOne(s => s.Flashcard)
                .WithMany()
                .HasForeignKey(s => s.FlashcardId);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "default",
                    Email = "default@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("DefaultPassword123!"),
                    CreatedAt = new DateTime(2025, 3, 13)
                }
            );
        }
    }
}

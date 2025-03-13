using FlashcardApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FlashcardApi.Data
{
    public class FlashcardDbContext : DbContext
    {
        public FlashcardDbContext(DbContextOptions<FlashcardDbContext> options) : base(options)
        {
        }

        public DbSet<Flashcard> Flashcards { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<StudySession> StudySessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships
            modelBuilder.Entity<Flashcard>()
                .HasOne(f => f.Category)
                .WithMany(c => c.Flashcards)
                .HasForeignKey(f => f.CategoryId);

            modelBuilder.Entity<StudySession>()
                .HasOne(s => s.Flashcard)
                .WithMany()
                .HasForeignKey(s => s.FlashcardId);

            // Seed some initial categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "General Knowledge", Description = "General trivia and facts" },
                new Category { Id = 2, Name = "Programming", Description = "Programming concepts and code snippets" },
                new Category { Id = 3, Name = "Languages", Description = "Foreign language vocabulary" }
            );
        }
    }
}

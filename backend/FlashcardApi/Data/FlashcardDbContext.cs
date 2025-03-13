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
        public DbSet<StudySession> StudySessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudySession>()
                .HasOne(s => s.Flashcard)
                .WithMany()
                .HasForeignKey(s => s.FlashcardId);
        }
    }
}

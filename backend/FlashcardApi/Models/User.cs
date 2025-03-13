using System.ComponentModel.DataAnnotations;

namespace FlashcardApi.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property for user's flashcards
        public virtual ICollection<Flashcard> Flashcards { get; set; } = new List<Flashcard>();
    }
}

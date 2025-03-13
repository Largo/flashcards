using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FlashcardApi.Models
{
    public class Flashcard
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Question { get; set; }
        
        [Required]
        public string Answer { get; set; }
        
        public string? Hint { get; set; }
        
        // User relationship
        public int? UserId { get; set; }
        
        [ForeignKey("UserId")]
        [JsonIgnore]
        public virtual User? User { get; set; }
        
        // Spaced Repetition Algorithm properties
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastReviewedAt { get; set; } = DateTime.UtcNow;
        public DateTime NextReviewAt { get; set; } = DateTime.UtcNow;
        
        // SM-2 algorithm parameters
        public int RepetitionNumber { get; set; } = 0;
        public double EasinessFactor { get; set; } = 2.5; // Default value
        public int IntervalDays { get; set; } = 0;
    }
}

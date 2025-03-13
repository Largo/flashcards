using System;
using System.ComponentModel.DataAnnotations;

namespace FlashcardApi.Models
{
    public class StudySession
    {
        [Key]
        public int Id { get; set; }
        
        public int FlashcardId { get; set; }
        public Flashcard Flashcard { get; set; }
        
        public DateTime StudiedAt { get; set; } = DateTime.UtcNow;
        
        // Quality of response (0-5) as per SM-2 algorithm
        // 0 - complete blackout
        // 1 - incorrect response; the correct one remembered
        // 2 - incorrect response; where the correct one seemed easy to recall
        // 3 - correct response, but with difficulty
        // 4 - correct response, after some hesitation
        // 5 - perfect response
        public int QualityOfResponse { get; set; }
    }
}

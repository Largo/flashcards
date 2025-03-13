using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlashcardApi.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public string? Description { get; set; }
        
        public ICollection<Flashcard> Flashcards { get; set; } = new List<Flashcard>();
    }
}

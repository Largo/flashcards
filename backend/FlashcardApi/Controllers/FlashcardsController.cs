using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlashcardApi.Data;
using FlashcardApi.Models;
using FlashcardApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlashcardApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlashcardsController : ControllerBase
    {
        private readonly FlashcardDbContext _context;
        private readonly SpacedRepetitionService _spacedRepetitionService;

        public FlashcardsController(FlashcardDbContext context, SpacedRepetitionService spacedRepetitionService)
        {
            _context = context;
            _spacedRepetitionService = spacedRepetitionService;
        }

        // GET: api/Flashcards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flashcard>>> GetFlashcards()
        {
            return await _context.Flashcards
                .Include(f => f.Category)
                .ToListAsync();
        }

        // GET: api/Flashcards/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Flashcard>> GetFlashcard(int id)
        {
            var flashcard = await _context.Flashcards
                .Include(f => f.Category)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (flashcard == null)
            {
                return NotFound();
            }

            return flashcard;
        }

        // GET: api/Flashcards/due
        [HttpGet("due")]
        public async Task<ActionResult<IEnumerable<Flashcard>>> GetDueFlashcards()
        {
            var now = DateTime.UtcNow;
            var dueFlashcards = await _context.Flashcards
                .Include(f => f.Category)
                .Where(f => f.NextReviewAt <= now)
                .ToListAsync();

            return dueFlashcards;
        }

        // POST: api/Flashcards
        [HttpPost]
        public async Task<ActionResult<Flashcard>> CreateFlashcard(Flashcard flashcard)
        {
            // Set initial spaced repetition values
            flashcard.CreatedAt = DateTime.UtcNow;
            flashcard.LastReviewedAt = DateTime.UtcNow;
            flashcard.NextReviewAt = DateTime.UtcNow;
            flashcard.RepetitionNumber = 0;
            flashcard.EasinessFactor = 2.5;
            flashcard.IntervalDays = 0;

            _context.Flashcards.Add(flashcard);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFlashcard), new { id = flashcard.Id }, flashcard);
        }

        // PUT: api/Flashcards/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFlashcard(int id, Flashcard flashcard)
        {
            if (id != flashcard.Id)
            {
                return BadRequest();
            }

            _context.Entry(flashcard).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlashcardExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Flashcards/5/review
        [HttpPost("{id}/review")]
        public async Task<ActionResult<Flashcard>> ReviewFlashcard(int id, [FromBody] ReviewRequest request)
        {
            var flashcard = await _context.Flashcards.FindAsync(id);
            if (flashcard == null)
            {
                return NotFound();
            }

            // Process the review using the spaced repetition algorithm
            flashcard = _spacedRepetitionService.ProcessReview(flashcard, request.QualityOfResponse);

            // Save the updated flashcard
            _context.Entry(flashcard).State = EntityState.Modified;

            // Create a study session record
            var studySession = new StudySession
            {
                FlashcardId = id,
                QualityOfResponse = request.QualityOfResponse,
                StudiedAt = DateTime.UtcNow
            };
            _context.StudySessions.Add(studySession);

            await _context.SaveChangesAsync();

            return Ok(flashcard);
        }

        // DELETE: api/Flashcards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlashcard(int id)
        {
            var flashcard = await _context.Flashcards.FindAsync(id);
            if (flashcard == null)
            {
                return NotFound();
            }

            _context.Flashcards.Remove(flashcard);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FlashcardExists(int id)
        {
            return _context.Flashcards.Any(e => e.Id == id);
        }
    }

    public class ReviewRequest
    {
        public int QualityOfResponse { get; set; }
    }
}

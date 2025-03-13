using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FlashcardApi.Data;
using FlashcardApi.Models;
using FlashcardApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlashcardApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FlashcardsController : ControllerBase
    {
        private readonly FlashcardDbContext _context;
        private readonly SpacedRepetitionService _spacedRepetitionService;

        public FlashcardsController(FlashcardDbContext context, SpacedRepetitionService spacedRepetitionService)
        {
            _context = context;
            _spacedRepetitionService = spacedRepetitionService;
        }
        
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }
            return int.Parse(userIdClaim.Value);
        }

        // GET: api/Flashcards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flashcard>>> GetFlashcards()
        {
            var userId = GetUserId();
            return await _context.Flashcards
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }

        // GET: api/Flashcards/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Flashcard>> GetFlashcard(int id)
        {
            var userId = GetUserId();
            var flashcard = await _context.Flashcards
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

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
            var userId = GetUserId();
            var now = DateTime.UtcNow;
            
            return await _context.Flashcards
                .Where(f => f.UserId == userId && f.NextReviewAt <= now)
                .ToListAsync();
        }

        // POST: api/Flashcards
        [HttpPost]
        public async Task<ActionResult<Flashcard>> PostFlashcard(Flashcard flashcard)
        {
            var userId = GetUserId();
            
            // Set the user ID for the new flashcard
            flashcard.UserId = userId;
            
            // Initialize spaced repetition properties
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
        public async Task<IActionResult> PutFlashcard(int id, Flashcard flashcard)
        {
            var userId = GetUserId();
            
            if (id != flashcard.Id)
            {
                return BadRequest();
            }

            // Ensure the flashcard belongs to the current user
            var existingFlashcard = await _context.Flashcards
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
                
            if (existingFlashcard == null)
            {
                return NotFound();
            }

            // Update only the content fields, not the spaced repetition properties
            existingFlashcard.Question = flashcard.Question;
            existingFlashcard.Answer = flashcard.Answer;
            existingFlashcard.Hint = flashcard.Hint;

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

        // DELETE: api/Flashcards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlashcard(int id)
        {
            var userId = GetUserId();
            
            var flashcard = await _context.Flashcards
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
                
            if (flashcard == null)
            {
                return NotFound();
            }

            _context.Flashcards.Remove(flashcard);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Flashcards/5/review
        [HttpPost("{id}/review")]
        public async Task<ActionResult<Flashcard>> ReviewFlashcard(int id, [FromBody] ReviewRequest request)
        {
            var userId = GetUserId();
            
            var flashcard = await _context.Flashcards
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

            if (flashcard == null)
            {
                return NotFound();
            }

            // Apply the spaced repetition algorithm
            flashcard = _spacedRepetitionService.ProcessReview(flashcard, request.QualityOfResponse);
            
            // Create a study session record
            var studySession = new StudySession
            {
                FlashcardId = id,
                QualityOfResponse = request.QualityOfResponse,
                StudiedAt = DateTime.UtcNow
            };
            
            _context.StudySessions.Add(studySession);
            _context.Entry(flashcard).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(flashcard);
        }

        private bool FlashcardExists(int id)
        {
            var userId = GetUserId();
            return _context.Flashcards.Any(e => e.Id == id && e.UserId == userId);
        }
    }

    public class ReviewRequest
    {
        public int QualityOfResponse { get; set; }
    }
}

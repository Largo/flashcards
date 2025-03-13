using System;
using FlashcardApi.Models;

namespace FlashcardApi.Services
{
    public class SpacedRepetitionService
    {
        /// <summary>
        /// Implements the SM-2 spaced repetition algorithm
        /// Based on the SuperMemo-2 algorithm: https://www.supermemo.com/en/archives1990-2015/english/ol/sm2
        /// </summary>
        /// <param name="flashcard">The flashcard to update</param>
        /// <param name="qualityOfResponse">Quality of response (0-5)</param>
        /// <returns>Updated flashcard with new scheduling information</returns>
        public Flashcard ProcessReview(Flashcard flashcard, int qualityOfResponse)
        {
            // Ensure quality is within valid range
            qualityOfResponse = Math.Clamp(qualityOfResponse, 0, 5);
            
            // Record the current time as the review time
            flashcard.LastReviewedAt = DateTime.UtcNow;
            
            // If the response was a failure (quality < 3), reset repetition count
            if (qualityOfResponse < 3)
            {
                flashcard.RepetitionNumber = 0;
                flashcard.IntervalDays = 1;
            }
            else
            {
                // Calculate the new interval based on repetition number
                if (flashcard.RepetitionNumber == 0)
                {
                    flashcard.IntervalDays = 1;
                }
                else if (flashcard.RepetitionNumber == 1)
                {
                    flashcard.IntervalDays = 6;
                }
                else
                {
                    // For repetitions > 1, use the formula: interval = interval * easiness
                    flashcard.IntervalDays = (int)Math.Round(flashcard.IntervalDays * flashcard.EasinessFactor);
                }
                
                // Increment repetition number
                flashcard.RepetitionNumber++;
            }
            
            // Update easiness factor based on quality of response
            // EF' = EF + (0.1 - (5 - q) * (0.08 + (5 - q) * 0.02))
            flashcard.EasinessFactor += (0.1 - (5 - qualityOfResponse) * (0.08 + (5 - qualityOfResponse) * 0.02));
            
            // Ensure easiness factor doesn't go below 1.3
            if (flashcard.EasinessFactor < 1.3)
            {
                flashcard.EasinessFactor = 1.3;
            }
            
            // Calculate the next review date
            flashcard.NextReviewAt = flashcard.LastReviewedAt.AddDays(flashcard.IntervalDays);
            
            return flashcard;
        }
        
        /// <summary>
        /// Gets flashcards that are due for review
        /// </summary>
        /// <param name="now">Current time (usually DateTime.UtcNow)</param>
        /// <returns>True if the flashcard is due for review</returns>
        public bool IsDueForReview(Flashcard flashcard, DateTime now)
        {
            return flashcard.NextReviewAt <= now;
        }
    }
}

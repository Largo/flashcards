<template>
  <div class="home-view">
    <div class="jumbotron bg-light p-5 rounded mb-4">
      <h1 class="display-4">Welcome to Flashcard App</h1>
      <p class="lead">
        A spaced repetition system to help you memorize and retain information more effectively.
      </p>
      <hr class="my-4">
      <p>
        Get started by creating flashcards and studying them using the scientifically-proven spaced repetition algorithm.
      </p>
      <div class="d-flex gap-2">
        <router-link to="/flashcards" class="btn btn-primary">
          View All Flashcards
        </router-link>
        <router-link to="/study" class="btn btn-success">
          Start Studying
        </router-link>
      </div>
    </div>

    <div class="row">
      <div class="col-md-6 mb-4">
        <div class="card h-100">
          <div class="card-body">
            <h5 class="card-title">
              <i class="bi bi-card-list me-2"></i>Flashcards
            </h5>
            <p class="card-text">
              Create and manage your flashcards. Add questions, answers, and hints to help you study.
            </p>
            <div class="d-flex justify-content-between align-items-center">
              <router-link to="/flashcards" class="btn btn-outline-primary">
                Manage Flashcards
              </router-link>
              <span class="badge bg-primary rounded-pill">{{ stats.totalFlashcards }}</span>
            </div>
          </div>
        </div>
      </div>

      <div class="col-md-6 mb-4">
        <div class="card h-100">
          <div class="card-body">
            <h5 class="card-title">
              <i class="bi bi-lightning me-2"></i>Study Session
            </h5>
            <p class="card-text">
              Review flashcards that are due for review based on the spaced repetition algorithm.
            </p>
            <div class="d-flex justify-content-between align-items-center">
              <router-link to="/study" class="btn btn-outline-success">
                Start Studying
              </router-link>
              <span class="badge bg-success rounded-pill">{{ stats.dueFlashcards }} due</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="card mt-4">
      <div class="card-header">
        <h5 class="mb-0">How Spaced Repetition Works</h5>
      </div>
      <div class="card-body">
        <p>
          This app uses the SM-2 spaced repetition algorithm to optimize your learning:
        </p>
        <ol>
          <li>After reviewing a flashcard, rate your recall quality from 0-5</li>
          <li>Cards you find difficult will appear more frequently</li>
          <li>Cards you know well will appear less often</li>
          <li>The system automatically schedules the optimal time for your next review</li>
        </ol>
        <p class="mb-0">
          This scientifically-proven method helps you focus on what you need to learn most while reinforcing your knowledge at the optimal intervals.
        </p>
      </div>
    </div>
  </div>
</template>

<script>
import api from '../services/api';

export default {
  name: 'HomeView',
  data() {
    return {
      stats: {
        totalFlashcards: 0,
        dueFlashcards: 0
      }
    }
  },
  async created() {
    await this.loadStats();
  },
  methods: {
    async loadStats() {
      try {
        // Get all flashcards to count total
        const flashcardsResponse = await api.getFlashcards();
        this.stats.totalFlashcards = flashcardsResponse.data.length;
        
        // Get due flashcards
        const dueFlashcardsResponse = await api.getDueFlashcards();
        this.stats.dueFlashcards = dueFlashcardsResponse.data.length;
      } catch (error) {
        console.error('Error loading stats:', error);
      }
    }
  }
}
</script>

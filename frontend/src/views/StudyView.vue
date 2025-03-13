<template>
  <div class="study-view">
    <h1 class="mb-4">Study Session</h1>
    
    <div v-if="loading" class="text-center my-5">
      <div class="spinner-border" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    
    <div v-else-if="dueFlashcards.length === 0" class="alert alert-success">
      <h4 class="alert-heading">All caught up!</h4>
      <p>You don't have any flashcards due for review right now.</p>
      <hr>
      <p class="mb-0">
        <router-link to="/flashcards/new" class="alert-link">Create new flashcards</router-link> or 
        <router-link to="/flashcards" class="alert-link">review all your flashcards</router-link>.
      </p>
    </div>
    
    <div v-else>
      <div class="progress mb-4">
        <div 
          class="progress-bar" 
          role="progressbar" 
          :style="`width: ${(currentIndex / dueFlashcards.length) * 100}%`" 
          :aria-valuenow="currentIndex" 
          aria-valuemin="0" 
          :aria-valuemax="dueFlashcards.length"
        >
          {{ currentIndex }} / {{ dueFlashcards.length }}
        </div>
      </div>
      
      <div v-if="currentFlashcard" class="card mb-4">
        <div class="card-header d-flex justify-content-between align-items-center">
          <span class="badge bg-primary">Next Review: {{ formatDate(currentFlashcard.nextReviewAt) }}</span>
          <span>Card {{ currentIndex + 1 }} of {{ dueFlashcards.length }}</span>
        </div>
        <div class="card-body">
          <div class="flashcard" :class="{ flipped: showAnswer }">
            <div class="flashcard-inner">
              <div class="flashcard-front">
                <h3>{{ currentFlashcard.question }}</h3>
                <p v-if="currentFlashcard.hint && !showAnswer" class="text-muted mt-3">
                  <button class="btn btn-sm btn-outline-secondary" @click="showHint = !showHint">
                    {{ showHint ? 'Hide Hint' : 'Show Hint' }}
                  </button>
                  <span v-if="showHint" class="d-block mt-2">
                    <strong>Hint:</strong> {{ currentFlashcard.hint }}
                  </span>
                </p>
              </div>
              <div class="flashcard-back">
                <h3>{{ currentFlashcard.answer }}</h3>
              </div>
            </div>
          </div>
        </div>
        <div class="card-footer">
          <button v-if="!showAnswer" class="btn btn-primary w-100" @click="flipCard">
            Show Answer
          </button>
          <div v-else>
            <p class="text-center mb-3">How well did you know this?</p>
            <div class="d-flex justify-content-between">
              <button 
                v-for="quality in [0, 1, 2, 3, 4, 5]" 
                :key="quality"
                class="btn" 
                :class="getQualityButtonClass(quality)"
                @click="rateFlashcard(quality)"
              >
                {{ getQualityLabel(quality) }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Study Complete Modal -->
    <div class="modal fade" id="completeModal" tabindex="-1" aria-hidden="true" ref="completeModal">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Study Session Complete!</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
          </div>
          <div class="modal-body">
            <p>You've completed your study session for today. Great job!</p>
            <div class="text-center">
              <div class="mb-3">
                <h4>Session Summary</h4>
                <p>Cards reviewed: {{ dueFlashcards.length }}</p>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <router-link to="/" class="btn btn-primary">Return Home</router-link>
            <button type="button" class="btn btn-success" @click="restartSession">
              Study Again
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import api from '../services/api';
import { Modal } from 'bootstrap';

export default {
  name: 'StudyView',
  data() {
    return {
      dueFlashcards: [],
      currentIndex: 0,
      showAnswer: false,
      showHint: false,
      loading: true,
      completeModal: null
    }
  },
  computed: {
    currentFlashcard() {
      return this.dueFlashcards[this.currentIndex] || null;
    }
  },
  async created() {
    await this.loadDueFlashcards();
  },
  mounted() {
    this.completeModal = new Modal(this.$refs.completeModal);
  },
  methods: {
    async loadDueFlashcards() {
      try {
        this.loading = true;
        const response = await api.getDueFlashcards();
        this.dueFlashcards = response.data;
        this.currentIndex = 0;
        this.showAnswer = false;
        this.showHint = false;
        this.loading = false;
      } catch (error) {
        console.error('Error loading due flashcards:', error);
        this.loading = false;
      }
    },
    flipCard() {
      this.showAnswer = true;
    },
    async rateFlashcard(quality) {
      try {
        if (!this.currentFlashcard) return;
        
        // Send the review to the API
        await api.reviewFlashcard(this.currentFlashcard.id, quality);
        
        // Move to the next card or finish
        if (this.currentIndex < this.dueFlashcards.length - 1) {
          this.currentIndex++;
          this.showAnswer = false;
          this.showHint = false;
        } else {
          // Show completion modal
          this.completeModal.show();
        }
      } catch (error) {
        console.error('Error rating flashcard:', error);
      }
    },
    getQualityButtonClass(quality) {
      const baseClass = 'btn-sm';
      if (quality <= 1) return `${baseClass} btn-danger`;
      if (quality <= 3) return `${baseClass} btn-warning`;
      return `${baseClass} btn-success`;
    },
    getQualityLabel(quality) {
      switch (quality) {
        case 0: return 'Blackout';
        case 1: return 'Wrong';
        case 2: return 'Difficult';
        case 3: return 'Hard';
        case 4: return 'Good';
        case 5: return 'Perfect';
        default: return '';
      }
    },
    restartSession() {
      this.completeModal.hide();
      this.loadDueFlashcards();
    },
    formatDate(dateString) {
      const date = new Date(dateString);
      return date.toLocaleDateString();
    }
  }
}
</script>

<style scoped>
.flashcard {
  perspective: 1000px;
  height: 200px;
  margin: 20px 0;
}

.flashcard-inner {
  position: relative;
  width: 100%;
  height: 100%;
  text-align: center;
  transition: transform 0.6s;
  transform-style: preserve-3d;
}

.flashcard.flipped .flashcard-inner {
  transform: rotateY(180deg);
}

.flashcard-front, .flashcard-back {
  position: absolute;
  width: 100%;
  height: 100%;
  backface-visibility: hidden;
  display: flex;
  flex-direction: column;
  justify-content: center;
  padding: 20px;
}

.flashcard-back {
  transform: rotateY(180deg);
}
</style>

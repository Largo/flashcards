<template>
  <div class="flashcards-view">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>All Flashcards</h1>
      <router-link to="/flashcards/new" class="btn btn-success">
        <i class="bi bi-plus-circle"></i> Create New Flashcard
      </router-link>
    </div>
    
    <div class="mb-4">
      <div class="input-group">
        <input 
          type="text" 
          class="form-control" 
          placeholder="Search flashcards..." 
          v-model="searchQuery"
          @input="filterFlashcards"
        >
        <button class="btn btn-outline-secondary" type="button" @click="filterFlashcards">
          <i class="bi bi-search"></i>
        </button>
      </div>
    </div>
    
    <div v-if="loading" class="text-center my-5">
      <div class="spinner-border" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    
    <div v-else-if="filteredFlashcards.length === 0" class="alert alert-info">
      No flashcards found. Create your first flashcard to get started.
    </div>
    
    <div v-else class="row">
      <div v-for="flashcard in filteredFlashcards" :key="flashcard.id" class="col-md-4 mb-4">
        <div class="card h-100">
          <div class="card-header d-flex justify-content-between align-items-center">
            <span class="badge bg-primary">
              {{ formatDate(flashcard.nextReviewAt) }}
            </span>
            <div class="dropdown">
              <button class="btn btn-sm btn-outline-secondary dropdown-toggle" type="button" data-bs-toggle="dropdown">
                <i class="bi bi-three-dots-vertical"></i>
              </button>
              <ul class="dropdown-menu dropdown-menu-end">
                <li>
                  <router-link :to="`/flashcards/${flashcard.id}`" class="dropdown-item">
                    Edit
                  </router-link>
                </li>
                <li>
                  <button class="dropdown-item text-danger" @click="confirmDelete(flashcard)">
                    Delete
                  </button>
                </li>
              </ul>
            </div>
          </div>
          <div class="card-body">
            <h5 class="card-title">{{ flashcard.question }}</h5>
            <div class="card-text mt-3">
              <button class="btn btn-sm btn-outline-primary" @click="toggleAnswer(flashcard)">
                {{ flashcard.showAnswer ? 'Hide Answer' : 'Show Answer' }}
              </button>
              <div v-if="flashcard.showAnswer" class="mt-2">
                <strong>Answer:</strong> {{ flashcard.answer }}
              </div>
              <div v-if="flashcard.hint" class="mt-2">
                <strong>Hint:</strong> {{ flashcard.hint }}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Delete Confirmation Modal -->
    <div class="modal fade" id="deleteModal" tabindex="-1" aria-hidden="true" ref="deleteModal">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Confirm Delete</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
          </div>
          <div class="modal-body" v-if="selectedFlashcard">
            <p>Are you sure you want to delete this flashcard?</p>
            <div class="card">
              <div class="card-body">
                <p><strong>Question:</strong> {{ selectedFlashcard.question }}</p>
                <p><strong>Answer:</strong> {{ selectedFlashcard.answer }}</p>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
            <button type="button" class="btn btn-danger" @click="deleteFlashcard" :disabled="deleting">
              <span v-if="deleting" class="spinner-border spinner-border-sm me-2" role="status"></span>
              Delete
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
  name: 'FlashcardsView',
  data() {
    return {
      flashcards: [],
      filteredFlashcards: [],
      selectedFlashcard: null,
      searchQuery: '',
      loading: true,
      deleting: false,
      deleteModal: null
    }
  },
  async created() {
    await this.loadFlashcards();
  },
  mounted() {
    this.deleteModal = new Modal(this.$refs.deleteModal);
  },
  methods: {
    async loadFlashcards() {
      try {
        this.loading = true;
        const response = await api.getFlashcards();
        this.flashcards = response.data.map(flashcard => ({
          ...flashcard,
          showAnswer: false
        }));
        this.filteredFlashcards = [...this.flashcards];
        this.loading = false;
      } catch (error) {
        console.error('Error loading flashcards:', error);
        this.loading = false;
      }
    },
    filterFlashcards() {
      if (!this.searchQuery.trim()) {
        this.filteredFlashcards = [...this.flashcards];
        return;
      }
      
      const query = this.searchQuery.toLowerCase();
      this.filteredFlashcards = this.flashcards.filter(flashcard => 
        flashcard.question.toLowerCase().includes(query) || 
        flashcard.answer.toLowerCase().includes(query)
      );
    },
    toggleAnswer(flashcard) {
      flashcard.showAnswer = !flashcard.showAnswer;
    },
    confirmDelete(flashcard) {
      this.selectedFlashcard = flashcard;
      this.deleteModal.show();
    },
    async deleteFlashcard() {
      if (!this.selectedFlashcard) return;
      
      try {
        this.deleting = true;
        await api.deleteFlashcard(this.selectedFlashcard.id);
        
        // Remove the flashcard from the lists
        this.flashcards = this.flashcards.filter(f => f.id !== this.selectedFlashcard.id);
        this.filteredFlashcards = this.filteredFlashcards.filter(f => f.id !== this.selectedFlashcard.id);
        
        this.deleteModal.hide();
        this.deleting = false;
        this.selectedFlashcard = null;
      } catch (error) {
        console.error('Error deleting flashcard:', error);
        this.deleting = false;
      }
    },
    formatDate(dateString) {
      const date = new Date(dateString);
      return date.toLocaleDateString();
    }
  }
}
</script>

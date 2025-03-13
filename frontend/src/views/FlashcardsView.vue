<template>
  <div class="flashcards-view">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>All Flashcards</h1>
      <router-link to="/flashcards/new" class="btn btn-success">
        <i class="bi bi-plus-circle"></i> Create New Flashcard
      </router-link>
    </div>

    <div class="row mb-4">
      <div class="col-md-6">
        <div class="input-group">
          <input 
            type="text" 
            class="form-control" 
            placeholder="Search flashcards..." 
            v-model="searchQuery"
          >
          <button class="btn btn-outline-secondary" type="button" @click="searchQuery = ''">
            Clear
          </button>
        </div>
      </div>
      <div class="col-md-6">
        <div class="input-group">
          <label class="input-group-text" for="categoryFilter">Category</label>
          <select class="form-select" id="categoryFilter" v-model="selectedCategory">
            <option value="">All Categories</option>
            <option v-for="category in categories" :key="category.id" :value="category.id">
              {{ category.name }}
            </option>
          </select>
        </div>
      </div>
    </div>

    <div v-if="loading" class="text-center my-5">
      <div class="spinner-border" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>

    <div v-else-if="filteredFlashcards.length === 0" class="alert alert-info">
      No flashcards found. 
      <router-link to="/flashcards/new" class="alert-link">Create a new flashcard</router-link>.
    </div>

    <div v-else class="row">
      <div v-for="flashcard in filteredFlashcards" :key="flashcard.id" class="col-md-4 mb-4">
        <div class="card h-100">
          <div class="card-header d-flex justify-content-between align-items-center">
            <span class="badge bg-primary">{{ flashcard.category.name }}</span>
            <div class="dropdown">
              <button class="btn btn-sm btn-outline-secondary dropdown-toggle" type="button" data-bs-toggle="dropdown">
                Actions
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
            <p class="card-text text-muted">
              <small>
                Next review: {{ formatDate(flashcard.nextReviewAt) }}
              </small>
            </p>
          </div>
          <div class="card-footer bg-transparent">
            <button class="btn btn-sm btn-primary" @click="showAnswer(flashcard)">
              Show Answer
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Answer Modal -->
    <div class="modal fade" id="answerModal" tabindex="-1" aria-hidden="true" ref="answerModal">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Flashcard Answer</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
          </div>
          <div class="modal-body" v-if="selectedFlashcard">
            <h5>Question:</h5>
            <p>{{ selectedFlashcard.question }}</p>
            <h5>Answer:</h5>
            <p>{{ selectedFlashcard.answer }}</p>
            <div v-if="selectedFlashcard.hint">
              <h5>Hint:</h5>
              <p>{{ selectedFlashcard.hint }}</p>
            </div>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
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
            <p><strong>Question:</strong> {{ selectedFlashcard.question }}</p>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
            <button type="button" class="btn btn-danger" @click="deleteFlashcard">Delete</button>
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
      categories: [],
      loading: true,
      searchQuery: '',
      selectedCategory: '',
      selectedFlashcard: null,
      answerModal: null,
      deleteModal: null
    }
  },
  computed: {
    filteredFlashcards() {
      let filtered = this.flashcards;
      
      // Filter by search query
      if (this.searchQuery) {
        const query = this.searchQuery.toLowerCase();
        filtered = filtered.filter(f => 
          f.question.toLowerCase().includes(query) || 
          f.answer.toLowerCase().includes(query)
        );
      }
      
      // Filter by category
      if (this.selectedCategory) {
        filtered = filtered.filter(f => f.categoryId === parseInt(this.selectedCategory));
      }
      
      return filtered;
    }
  },
  async created() {
    try {
      // Load flashcards and categories in parallel
      const [flashcardsResponse, categoriesResponse] = await Promise.all([
        api.getFlashcards(),
        api.getCategories()
      ]);
      
      this.flashcards = flashcardsResponse.data;
      this.categories = categoriesResponse.data;
      this.loading = false;
    } catch (error) {
      console.error('Error loading flashcards:', error);
      this.loading = false;
    }
  },
  mounted() {
    // Initialize Bootstrap modals
    this.answerModal = new Modal(this.$refs.answerModal);
    this.deleteModal = new Modal(this.$refs.deleteModal);
  },
  methods: {
    formatDate(dateString) {
      const date = new Date(dateString);
      return date.toLocaleDateString();
    },
    showAnswer(flashcard) {
      this.selectedFlashcard = flashcard;
      this.answerModal.show();
    },
    confirmDelete(flashcard) {
      this.selectedFlashcard = flashcard;
      this.deleteModal.show();
    },
    async deleteFlashcard() {
      try {
        await api.deleteFlashcard(this.selectedFlashcard.id);
        this.flashcards = this.flashcards.filter(f => f.id !== this.selectedFlashcard.id);
        this.deleteModal.hide();
        this.selectedFlashcard = null;
      } catch (error) {
        console.error('Error deleting flashcard:', error);
      }
    }
  }
}
</script>

<template>
  <div class="flashcard-form">
    <h1 class="mb-4">{{ isEditing ? 'Edit Flashcard' : 'Create New Flashcard' }}</h1>
    
    <div v-if="loading" class="text-center my-5">
      <div class="spinner-border" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    
    <form v-else @submit.prevent="saveFlashcard">
      <div class="card mb-4">
        <div class="card-body">
          <div class="mb-3">
            <label for="category" class="form-label">Category</label>
            <select id="category" class="form-select" v-model="flashcard.categoryId" required>
              <option value="" disabled>Select a category</option>
              <option v-for="category in categories" :key="category.id" :value="category.id">
                {{ category.name }}
              </option>
            </select>
          </div>
          
          <div class="mb-3">
            <label for="question" class="form-label">Question</label>
            <textarea 
              id="question" 
              class="form-control" 
              v-model="flashcard.question" 
              rows="3" 
              required
            ></textarea>
          </div>
          
          <div class="mb-3">
            <label for="answer" class="form-label">Answer</label>
            <textarea 
              id="answer" 
              class="form-control" 
              v-model="flashcard.answer" 
              rows="3" 
              required
            ></textarea>
          </div>
          
          <div class="mb-3">
            <label for="hint" class="form-label">Hint (Optional)</label>
            <textarea 
              id="hint" 
              class="form-control" 
              v-model="flashcard.hint" 
              rows="2"
            ></textarea>
            <div class="form-text">Provide a hint that can help you remember the answer.</div>
          </div>
        </div>
      </div>
      
      <div class="d-flex justify-content-between">
        <router-link to="/flashcards" class="btn btn-secondary">Cancel</router-link>
        <button type="submit" class="btn btn-primary" :disabled="saving">
          <span v-if="saving" class="spinner-border spinner-border-sm me-2" role="status"></span>
          {{ isEditing ? 'Update Flashcard' : 'Create Flashcard' }}
        </button>
      </div>
    </form>
  </div>
</template>

<script>
import api from '../services/api';

export default {
  name: 'FlashcardFormView',
  props: {
    id: {
      type: String,
      required: false
    }
  },
  data() {
    return {
      flashcard: {
        question: '',
        answer: '',
        hint: '',
        categoryId: ''
      },
      categories: [],
      loading: true,
      saving: false
    }
  },
  computed: {
    isEditing() {
      return !!this.id;
    }
  },
  async created() {
    try {
      // Load categories
      const categoriesResponse = await api.getCategories();
      this.categories = categoriesResponse.data;
      
      // If editing, load the flashcard data
      if (this.isEditing) {
        const flashcardResponse = await api.getFlashcard(this.id);
        const flashcard = flashcardResponse.data;
        
        this.flashcard = {
          question: flashcard.question,
          answer: flashcard.answer,
          hint: flashcard.hint || '',
          categoryId: flashcard.categoryId
        };
      }
      
      this.loading = false;
    } catch (error) {
      console.error('Error loading form data:', error);
      this.loading = false;
    }
  },
  methods: {
    async saveFlashcard() {
      try {
        this.saving = true;
        
        if (this.isEditing) {
          await api.updateFlashcard(this.id, this.flashcard);
        } else {
          await api.createFlashcard(this.flashcard);
        }
        
        // Navigate back to flashcards list
        this.$router.push('/flashcards');
      } catch (error) {
        console.error('Error saving flashcard:', error);
        this.saving = false;
      }
    }
  }
}
</script>

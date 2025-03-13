<template>
  <div class="flashcard-form">
    <h1 class="mb-4">{{ isEditing ? 'Edit Flashcard' : 'Create New Flashcard' }}</h1>
    
    <div v-if="loading" class="text-center my-5">
      <div class="spinner-border" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    
    <form v-else @submit.prevent="saveFlashcard">
      <div class="mb-3">
        <label for="question" class="form-label">Question</label>
        <textarea 
          class="form-control" 
          id="question" 
          v-model="flashcard.question" 
          rows="3" 
          required
        ></textarea>
      </div>
      
      <div class="mb-3">
        <label for="answer" class="form-label">Answer</label>
        <textarea 
          class="form-control" 
          id="answer" 
          v-model="flashcard.answer" 
          rows="3" 
          required
        ></textarea>
      </div>
      
      <div class="mb-3">
        <label for="hint" class="form-label">Hint (Optional)</label>
        <textarea 
          class="form-control" 
          id="hint" 
          v-model="flashcard.hint" 
          rows="2"
        ></textarea>
        <div class="form-text">Provide a hint that can help you remember the answer.</div>
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
        hint: ''
      },
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
    if (this.isEditing) {
      await this.loadFlashcard();
    }
    this.loading = false;
  },
  methods: {
    async loadFlashcard() {
      try {
        const response = await api.getFlashcard(this.id);
        this.flashcard = {
          question: response.data.question,
          answer: response.data.answer,
          hint: response.data.hint || ''
        };
      } catch (error) {
        console.error('Error loading flashcard:', error);
      }
    },
    async saveFlashcard() {
      try {
        this.saving = true;
        
        if (this.isEditing) {
          await api.updateFlashcard(this.id, this.flashcard);
        } else {
          await api.createFlashcard(this.flashcard);
        }
        
        this.$router.push('/flashcards');
      } catch (error) {
        console.error('Error saving flashcard:', error);
        this.saving = false;
      }
    }
  }
}
</script>

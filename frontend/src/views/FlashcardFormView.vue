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
        <div>
          <router-link to="/flashcards" class="btn btn-secondary me-2">
            <i class="bi bi-arrow-left me-1"></i> Cancel
          </router-link>
          <button 
            v-if="isEditing" 
            type="button" 
            class="btn btn-danger" 
            @click="confirmDelete"
            :disabled="saving"
          >
            <i class="bi bi-trash me-1"></i> Delete
          </button>
        </div>
        <button type="submit" class="btn btn-primary" :disabled="saving">
          <span v-if="saving" class="spinner-border spinner-border-sm me-2" role="status"></span>
          <i v-else class="bi" :class="isEditing ? 'bi-save' : 'bi-plus-circle'"></i>
          {{ isEditing ? 'Update Flashcard' : 'Create Flashcard' }}
        </button>
      </div>
    </form>
    
    <!-- Delete Confirmation Modal -->
    <div class="modal fade" id="deleteModal" tabindex="-1" aria-hidden="true" ref="deleteModal">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Confirm Delete</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
          </div>
          <div class="modal-body">
            <p>Are you sure you want to delete this flashcard?</p>
            <div class="card">
              <div class="card-body">
                <p><strong>Question:</strong> {{ flashcard.question }}</p>
                <p><strong>Answer:</strong> {{ flashcard.answer }}</p>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
            <button type="button" class="btn btn-danger" @click="deleteFlashcard" :disabled="deleting">
              <span v-if="deleting" class="spinner-border spinner-border-sm me-2" role="status"></span>
              <i v-else class="bi bi-trash me-1"></i> Delete
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
      saving: false,
      deleting: false,
      deleteModal: null
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
  mounted() {
    this.deleteModal = new Modal(this.$refs.deleteModal);
  },
  methods: {
    async loadFlashcard() {
      try {
        const response = await api.getFlashcard(this.id);
        this.flashcard = response.data;
      } catch (error) {
        console.error('Error loading flashcard:', error);
      }
    },
    async saveFlashcard() {
      try {
        this.saving = true;
        
        if (this.isEditing) {
          // Only update the fields we want to change
          const updatedFields = {
            question: this.flashcard.question,
            answer: this.flashcard.answer,
            hint: this.flashcard.hint || ''
          };
          await api.updateFlashcard(this.id, updatedFields);
        } else {
          await api.createFlashcard(this.flashcard);
        }
        
        this.$router.push('/flashcards');
      } catch (error) {
        console.error('Error saving flashcard:', error);
        this.saving = false;
      }
    },
    confirmDelete() {
      this.deleteModal.show();
    },
    async deleteFlashcard() {
      try {
        this.deleting = true;
        await api.deleteFlashcard(this.id);
        this.deleteModal.hide();
        this.$router.push('/flashcards');
      } catch (error) {
        console.error('Error deleting flashcard:', error);
        this.deleting = false;
      }
    }
  }
}
</script>

<style scoped>
.card {
  border: 1px solid rgba(0, 0, 0, 0.125);
  border-radius: 0.25rem;
}

textarea {
  resize: vertical;
}
</style>

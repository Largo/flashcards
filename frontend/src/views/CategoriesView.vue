<template>
  <div class="categories-view">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>Categories</h1>
      <button class="btn btn-success" @click="showCreateModal">
        <i class="bi bi-plus-circle"></i> Create New Category
      </button>
    </div>
    
    <div v-if="loading" class="text-center my-5">
      <div class="spinner-border" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    
    <div v-else-if="categories.length === 0" class="alert alert-info">
      No categories found. Create your first category to organize your flashcards.
    </div>
    
    <div v-else class="row">
      <div v-for="category in categories" :key="category.id" class="col-md-4 mb-4">
        <div class="card h-100">
          <div class="card-body">
            <h5 class="card-title">{{ category.name }}</h5>
            <p class="card-text">{{ category.description || 'No description' }}</p>
          </div>
          <div class="card-footer bg-transparent d-flex justify-content-between">
            <button class="btn btn-sm btn-primary" @click="viewCategoryFlashcards(category)">
              View Flashcards
            </button>
            <div>
              <button class="btn btn-sm btn-outline-secondary me-2" @click="showEditModal(category)">
                Edit
              </button>
              <button class="btn btn-sm btn-outline-danger" @click="showDeleteModal(category)">
                Delete
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Category Form Modal -->
    <div class="modal fade" id="categoryModal" tabindex="-1" aria-hidden="true" ref="categoryModal">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ isEditing ? 'Edit Category' : 'Create New Category' }}</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="saveCategory">
              <div class="mb-3">
                <label for="categoryName" class="form-label">Name</label>
                <input 
                  type="text" 
                  class="form-control" 
                  id="categoryName" 
                  v-model="categoryForm.name" 
                  required
                >
              </div>
              <div class="mb-3">
                <label for="categoryDescription" class="form-label">Description (Optional)</label>
                <textarea 
                  class="form-control" 
                  id="categoryDescription" 
                  v-model="categoryForm.description" 
                  rows="3"
                ></textarea>
              </div>
              <div class="d-flex justify-content-end">
                <button type="button" class="btn btn-secondary me-2" data-bs-dismiss="modal">
                  Cancel
                </button>
                <button type="submit" class="btn btn-primary" :disabled="saving">
                  <span v-if="saving" class="spinner-border spinner-border-sm me-2" role="status"></span>
                  {{ isEditing ? 'Update' : 'Create' }}
                </button>
              </div>
            </form>
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
          <div class="modal-body" v-if="selectedCategory">
            <p>Are you sure you want to delete the category <strong>{{ selectedCategory.name }}</strong>?</p>
            <div class="alert alert-warning">
              <strong>Warning:</strong> You can only delete categories that don't have any flashcards.
            </div>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
            <button type="button" class="btn btn-danger" @click="deleteCategory" :disabled="deleting">
              <span v-if="deleting" class="spinner-border spinner-border-sm me-2" role="status"></span>
              Delete
            </button>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Category Flashcards Modal -->
    <div class="modal fade" id="flashcardsModal" tabindex="-1" aria-hidden="true" ref="flashcardsModal">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" v-if="selectedCategory">{{ selectedCategory.name }} Flashcards</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
          </div>
          <div class="modal-body">
            <div v-if="loadingFlashcards" class="text-center my-3">
              <div class="spinner-border" role="status">
                <span class="visually-hidden">Loading...</span>
              </div>
            </div>
            <div v-else-if="categoryFlashcards.length === 0" class="alert alert-info">
              No flashcards in this category.
            </div>
            <div v-else>
              <div class="list-group">
                <div v-for="flashcard in categoryFlashcards" :key="flashcard.id" class="list-group-item">
                  <div class="d-flex w-100 justify-content-between">
                    <h5 class="mb-1">{{ flashcard.question }}</h5>
                    <small>Next review: {{ formatDate(flashcard.nextReviewAt) }}</small>
                  </div>
                  <p class="mb-1">{{ flashcard.answer }}</p>
                </div>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
            <router-link 
              v-if="selectedCategory" 
              :to="{ path: '/flashcards/new', query: { categoryId: selectedCategory.id } }" 
              class="btn btn-primary"
              @click="closeFlashcardsModal"
            >
              Add Flashcard to Category
            </router-link>
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
  name: 'CategoriesView',
  data() {
    return {
      categories: [],
      categoryFlashcards: [],
      selectedCategory: null,
      categoryForm: {
        name: '',
        description: ''
      },
      loading: true,
      loadingFlashcards: false,
      saving: false,
      deleting: false,
      isEditing: false,
      categoryModal: null,
      deleteModal: null,
      flashcardsModal: null
    }
  },
  async created() {
    await this.loadCategories();
  },
  mounted() {
    this.categoryModal = new Modal(this.$refs.categoryModal);
    this.deleteModal = new Modal(this.$refs.deleteModal);
    this.flashcardsModal = new Modal(this.$refs.flashcardsModal);
  },
  methods: {
    async loadCategories() {
      try {
        this.loading = true;
        const response = await api.getCategories();
        this.categories = response.data;
        this.loading = false;
      } catch (error) {
        console.error('Error loading categories:', error);
        this.loading = false;
      }
    },
    showCreateModal() {
      this.isEditing = false;
      this.categoryForm = {
        name: '',
        description: ''
      };
      this.categoryModal.show();
    },
    showEditModal(category) {
      this.isEditing = true;
      this.selectedCategory = category;
      this.categoryForm = {
        name: category.name,
        description: category.description || ''
      };
      this.categoryModal.show();
    },
    showDeleteModal(category) {
      this.selectedCategory = category;
      this.deleteModal.show();
    },
    async viewCategoryFlashcards(category) {
      try {
        this.selectedCategory = category;
        this.loadingFlashcards = true;
        this.flashcardsModal.show();
        
        const response = await api.getCategoryFlashcards(category.id);
        this.categoryFlashcards = response.data;
        this.loadingFlashcards = false;
      } catch (error) {
        console.error('Error loading category flashcards:', error);
        this.loadingFlashcards = false;
      }
    },
    closeFlashcardsModal() {
      this.flashcardsModal.hide();
    },
    async saveCategory() {
      try {
        this.saving = true;
        
        if (this.isEditing && this.selectedCategory) {
          await api.updateCategory(this.selectedCategory.id, this.categoryForm);
          
          // Update the category in the local list
          const index = this.categories.findIndex(c => c.id === this.selectedCategory.id);
          if (index !== -1) {
            this.categories[index] = {
              ...this.categories[index],
              ...this.categoryForm
            };
          }
        } else {
          const response = await api.createCategory(this.categoryForm);
          this.categories.push(response.data);
        }
        
        this.categoryModal.hide();
        this.saving = false;
      } catch (error) {
        console.error('Error saving category:', error);
        this.saving = false;
      }
    },
    async deleteCategory() {
      if (!this.selectedCategory) return;
      
      try {
        this.deleting = true;
        await api.deleteCategory(this.selectedCategory.id);
        
        // Remove the category from the local list
        this.categories = this.categories.filter(c => c.id !== this.selectedCategory.id);
        
        this.deleteModal.hide();
        this.deleting = false;
        this.selectedCategory = null;
      } catch (error) {
        console.error('Error deleting category:', error);
        this.deleting = false;
        
        // Show error message if the category has flashcards
        if (error.response && error.response.status === 400) {
          alert('Cannot delete a category that has flashcards. Remove all flashcards from this category first.');
        }
      }
    },
    formatDate(dateString) {
      const date = new Date(dateString);
      return date.toLocaleDateString();
    }
  }
}
</script>

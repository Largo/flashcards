<template>
  <div class="home">
    <div class="row mb-4">
      <div class="col-12">
        <div class="jumbotron bg-light p-5 rounded">
          <h1 class="display-4">Welcome to Flashcard App</h1>
          <p class="lead">Improve your learning with spaced repetition</p>
          <hr class="my-4">
          <p>This application uses the SM-2 spaced repetition algorithm to help you remember information more effectively.</p>
          <div class="d-flex gap-2">
            <router-link to="/study" class="btn btn-primary btn-lg">Start Studying</router-link>
            <router-link to="/flashcards/new" class="btn btn-success btn-lg">Create Flashcard</router-link>
          </div>
        </div>
      </div>
    </div>

    <div class="row mb-4">
      <div class="col-md-4 mb-3">
        <div class="card h-100">
          <div class="card-body">
            <h5 class="card-title">Due for Review</h5>
            <p class="card-text">You have {{ dueCount }} flashcards due for review.</p>
            <router-link to="/study" class="btn btn-primary">Study Now</router-link>
          </div>
        </div>
      </div>
      <div class="col-md-4 mb-3">
        <div class="card h-100">
          <div class="card-body">
            <h5 class="card-title">Total Flashcards</h5>
            <p class="card-text">You have created {{ totalCount }} flashcards in total.</p>
            <router-link to="/flashcards" class="btn btn-primary">View All</router-link>
          </div>
        </div>
      </div>
      <div class="col-md-4 mb-3">
        <div class="card h-100">
          <div class="card-body">
            <h5 class="card-title">Categories</h5>
            <p class="card-text">Organize your flashcards into {{ categoryCount }} categories.</p>
            <router-link to="/categories" class="btn btn-primary">Manage Categories</router-link>
          </div>
        </div>
      </div>
    </div>

    <div class="row">
      <div class="col-12">
        <h2>Recently Added Flashcards</h2>
        <div v-if="loading" class="text-center">
          <div class="spinner-border" role="status">
            <span class="visually-hidden">Loading...</span>
          </div>
        </div>
        <div v-else-if="recentFlashcards.length === 0" class="alert alert-info">
          You haven't created any flashcards yet. <router-link to="/flashcards/new">Create your first flashcard</router-link>.
        </div>
        <div v-else class="row">
          <div v-for="flashcard in recentFlashcards" :key="flashcard.id" class="col-md-4 mb-3">
            <div class="card h-100">
              <div class="card-body">
                <h5 class="card-title">{{ truncateText(flashcard.question, 50) }}</h5>
                <p class="card-text text-muted">{{ flashcard.category.name }}</p>
              </div>
              <div class="card-footer bg-transparent">
                <router-link :to="`/flashcards/${flashcard.id}`" class="btn btn-sm btn-primary">View</router-link>
              </div>
            </div>
          </div>
        </div>
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
      loading: true,
      dueCount: 0,
      totalCount: 0,
      categoryCount: 0,
      recentFlashcards: []
    }
  },
  async created() {
    try {
      // Get all flashcards
      const flashcardsResponse = await api.getFlashcards();
      this.totalCount = flashcardsResponse.data.length;
      
      // Get due flashcards
      const dueFlashcardsResponse = await api.getDueFlashcards();
      this.dueCount = dueFlashcardsResponse.data.length;
      
      // Get categories
      const categoriesResponse = await api.getCategories();
      this.categoryCount = categoriesResponse.data.length;
      
      // Get recent flashcards (last 3)
      this.recentFlashcards = flashcardsResponse.data
        .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
        .slice(0, 3);
      
      this.loading = false;
    } catch (error) {
      console.error('Error loading dashboard data:', error);
      this.loading = false;
    }
  },
  methods: {
    truncateText(text, maxLength) {
      if (text.length <= maxLength) return text;
      return text.slice(0, maxLength) + '...';
    }
  }
}
</script>

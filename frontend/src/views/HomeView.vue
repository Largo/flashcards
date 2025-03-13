<template>
  <div class="home">
    <div class="jumbotron bg-light p-5 rounded-3 mb-4">
      <div class="container">
        <h1 class="display-4">Welcome to Flashcards App</h1>
        <p class="lead">An efficient way to learn and remember anything with spaced repetition.</p>
        
        <div v-if="!isAuthenticated" class="mt-4">
          <router-link to="/register" class="btn btn-primary btn-lg me-2">Get Started</router-link>
          <router-link to="/login" class="btn btn-outline-primary btn-lg">Login</router-link>
        </div>
        <div v-else class="mt-4">
          <router-link to="/flashcards" class="btn btn-primary btn-lg me-2">My Flashcards</router-link>
          <router-link to="/study" class="btn btn-success btn-lg">Start Studying</router-link>
        </div>
      </div>
    </div>

    <div class="row mb-4">
      <div class="col-md-4">
        <div class="card h-100">
          <div class="card-body">
            <h5 class="card-title">Create Flashcards</h5>
            <p class="card-text">Create your own flashcards with questions and answers to help you learn and remember important information.</p>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card h-100">
          <div class="card-body">
            <h5 class="card-title">Spaced Repetition</h5>
            <p class="card-text">Our app uses a scientifically proven spaced repetition algorithm to help you remember information more effectively.</p>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card h-100">
          <div class="card-body">
            <h5 class="card-title">Track Progress</h5>
            <p class="card-text">Keep track of your learning progress and focus on the cards that need more attention.</p>
          </div>
        </div>
      </div>
    </div>

    <div v-if="isAuthenticated && dueFlashcards > 0" class="alert alert-info">
      <strong>You have {{ dueFlashcards }} flashcards due for review!</strong>
      <router-link to="/study" class="btn btn-sm btn-primary ms-3">Review Now</router-link>
    </div>
  </div>
</template>

<script>
import auth from '../services/auth';
import api from '../services/api';

export default {
  name: 'HomeView',
  data() {
    return {
      isAuthenticated: false,
      dueFlashcards: 0
    }
  },
  created() {
    this.isAuthenticated = auth.isAuthenticated();
    
    if (this.isAuthenticated) {
      this.fetchDueFlashcards();
    }
  },
  methods: {
    async fetchDueFlashcards() {
      try {
        const response = await api.getDueFlashcards();
        this.dueFlashcards = response.data.length;
      } catch (error) {
        console.error('Error fetching due flashcards:', error);
      }
    }
  }
}
</script>

<style scoped>
.home {
  max-width: 1200px;
  margin: 0 auto;
}

.jumbotron {
  background-color: #f8f9fa;
  border-radius: 0.5rem;
}

.card {
  transition: transform 0.3s;
  border-radius: 0.5rem;
}

.card:hover {
  transform: translateY(-5px);
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.1);
}
</style>

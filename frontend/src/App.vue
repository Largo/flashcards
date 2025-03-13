<template>
  <div class="app-container">
    <header>
      <nav class="navbar navbar-expand-lg navbar-light bg-light">
        <div class="container">
          <RouterLink class="navbar-brand" to="/">Flashcards App</RouterLink>
          <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
            <span class="navbar-toggler-icon"></span>
          </button>
          <div class="collapse navbar-collapse" id="navbarNav">
            <ul class="navbar-nav me-auto">
              <li class="nav-item">
                <RouterLink class="nav-link" to="/">Home</RouterLink>
              </li>
              <li class="nav-item" v-if="isAuthenticated">
                <RouterLink class="nav-link" to="/flashcards">Flashcards</RouterLink>
              </li>
              <li class="nav-item" v-if="isAuthenticated">
                <RouterLink class="nav-link" to="/study">Study</RouterLink>
              </li>
            </ul>
            <ul class="navbar-nav">
              <template v-if="isAuthenticated">
                <li class="nav-item">
                  <span class="nav-link me-2">Welcome, {{ user?.username || 'User' }}!</span>
                </li>
                <li class="nav-item">
                  <button class="btn btn-outline-danger" @click="logout">Logout</button>
                </li>
              </template>
              <template v-else>
                <li class="nav-item">
                  <RouterLink class="nav-link" to="/login">Login</RouterLink>
                </li>
                <li class="nav-item">
                  <RouterLink class="nav-link" to="/register">Register</RouterLink>
                </li>
              </template>
            </ul>
          </div>
        </div>
      </nav>
    </header>

    <main class="container py-4">
      <RouterView />
    </main>

    <footer class="footer mt-5 py-3 bg-light">
      <div class="container text-center">
        <span class="text-muted">Flashcard App with Spaced Repetition &copy; {{ currentYear }}</span>
      </div>
    </footer>
  </div>
</template>

<script setup>
import { RouterLink, RouterView } from 'vue-router'
import { ref, onMounted, computed } from 'vue'
import auth from './services/auth'

const user = ref(null)
const isAuthenticated = computed(() => auth.isAuthenticated())

onMounted(() => {
  user.value = auth.getCurrentUser()
})

const logout = () => {
  auth.logout()
  // Refresh the page to ensure all components update their auth state
  window.location.href = '/'
}

const currentYear = computed(() => new Date().getFullYear())
</script>

<style>
.app-container {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.container {
  flex: 1;
}

.footer {
  margin-top: auto;
}

header {
  margin-bottom: 2rem;
}
</style>

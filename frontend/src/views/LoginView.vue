<template>
  <div class="login-view">
    <div class="row justify-content-center">
      <div class="col-md-6 col-lg-4">
        <div class="card shadow">
          <div class="card-body">
            <h2 class="text-center mb-4">Login</h2>
            
            <div v-if="error" class="alert alert-danger">
              {{ error }}
            </div>
            
            <form @submit.prevent="login">
              <div class="mb-3">
                <label for="username" class="form-label">Username</label>
                <input 
                  type="text" 
                  class="form-control" 
                  id="username" 
                  v-model="credentials.username" 
                  required
                  autocomplete="username"
                >
              </div>
              
              <div class="mb-3">
                <label for="password" class="form-label">Password</label>
                <input 
                  type="password" 
                  class="form-control" 
                  id="password" 
                  v-model="credentials.password" 
                  required
                  autocomplete="current-password"
                >
              </div>
              
              <div class="d-grid">
                <button type="submit" class="btn btn-primary" :disabled="loading">
                  <span v-if="loading" class="spinner-border spinner-border-sm me-2" role="status"></span>
                  Login
                </button>
              </div>
            </form>
            
            <div class="text-center mt-3">
              <p>Don't have an account? <router-link to="/register">Register</router-link></p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import auth from '../services/auth';

export default {
  name: 'LoginView',
  data() {
    return {
      credentials: {
        username: '',
        password: ''
      },
      loading: false,
      error: null
    }
  },
  created() {
    // Redirect if already logged in
    if (auth.isAuthenticated()) {
      this.$router.push('/');
    }
  },
  methods: {
    async login() {
      this.loading = true;
      this.error = null;
      
      try {
        await auth.login(this.credentials);
        this.$router.push('/');
      } catch (error) {
        this.error = error.response?.data?.message || 'Login failed. Please check your credentials.';
        console.error('Login error:', error);
      } finally {
        this.loading = false;
      }
    }
  }
}
</script>

<style scoped>
.login-view {
  padding-top: 5rem;
}

.card {
  border-radius: 10px;
}
</style>

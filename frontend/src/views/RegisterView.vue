<template>
  <div class="register-view">
    <div class="row justify-content-center">
      <div class="col-md-6 col-lg-4">
        <div class="card shadow">
          <div class="card-body">
            <h2 class="text-center mb-4">Register</h2>
            
            <div v-if="error" class="alert alert-danger">
              {{ error }}
            </div>
            
            <form @submit.prevent="register">
              <div class="mb-3">
                <label for="username" class="form-label">Username</label>
                <input 
                  type="text" 
                  class="form-control" 
                  id="username" 
                  v-model="userData.username" 
                  required
                  autocomplete="username"
                >
                <div class="form-text">Username must be at least 3 characters long.</div>
              </div>
              
              <div class="mb-3">
                <label for="email" class="form-label">Email</label>
                <input 
                  type="email" 
                  class="form-control" 
                  id="email" 
                  v-model="userData.email" 
                  required
                  autocomplete="email"
                >
              </div>
              
              <div class="mb-3">
                <label for="password" class="form-label">Password</label>
                <input 
                  type="password" 
                  class="form-control" 
                  id="password" 
                  v-model="userData.password" 
                  required
                  autocomplete="new-password"
                >
                <div class="form-text">Password must be at least 6 characters long.</div>
              </div>
              
              <div class="mb-3">
                <label for="confirmPassword" class="form-label">Confirm Password</label>
                <input 
                  type="password" 
                  class="form-control" 
                  id="confirmPassword" 
                  v-model="userData.confirmPassword" 
                  required
                  autocomplete="new-password"
                >
              </div>
              
              <div class="d-grid">
                <button type="submit" class="btn btn-primary" :disabled="loading">
                  <span v-if="loading" class="spinner-border spinner-border-sm me-2" role="status"></span>
                  Register
                </button>
              </div>
            </form>
            
            <div class="text-center mt-3">
              <p>Already have an account? <router-link to="/login">Login</router-link></p>
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
  name: 'RegisterView',
  data() {
    return {
      userData: {
        username: '',
        email: '',
        password: '',
        confirmPassword: ''
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
    async register() {
      // Validate form
      if (this.userData.password !== this.userData.confirmPassword) {
        this.error = 'Passwords do not match';
        return;
      }
      
      if (this.userData.password.length < 6) {
        this.error = 'Password must be at least 6 characters long';
        return;
      }
      
      this.loading = true;
      this.error = null;
      
      try {
        await auth.register(this.userData);
        this.$router.push('/');
      } catch (error) {
        this.error = error.response?.data?.message || 'Registration failed. Please try again.';
        console.error('Registration error:', error);
      } finally {
        this.loading = false;
      }
    }
  }
}
</script>

<style scoped>
.register-view {
  padding-top: 5rem;
}

.card {
  border-radius: 10px;
}
</style>

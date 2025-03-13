import api from './api';

// Initial state
const state = {
  user: null,
  isAuthenticated: false,
  isLoading: false,
  error: null
};

// Load user from localStorage on page refresh
const initializeAuth = () => {
  const userData = localStorage.getItem('user');
  const token = localStorage.getItem('token');
  
  if (userData && token) {
    state.user = JSON.parse(userData);
    state.isAuthenticated = true;
  }
};

// Register a new user
const register = async (userData) => {
  state.isLoading = true;
  state.error = null;
  
  try {
    const response = await api.register(userData);
    const { token, ...user } = response.data;
    
    // Save to localStorage
    localStorage.setItem('token', token);
    localStorage.setItem('user', JSON.stringify(user));
    
    // Update state
    state.user = user;
    state.isAuthenticated = true;
    state.isLoading = false;
    
    return user;
  } catch (error) {
    state.error = error.response?.data?.message || 'Registration failed';
    state.isLoading = false;
    throw error;
  }
};

// Login user
const login = async (credentials) => {
  state.isLoading = true;
  state.error = null;
  
  try {
    const response = await api.login(credentials);
    const { token, ...user } = response.data;
    
    // Save to localStorage
    localStorage.setItem('token', token);
    localStorage.setItem('user', JSON.stringify(user));
    
    // Update state
    state.user = user;
    state.isAuthenticated = true;
    state.isLoading = false;
    
    return user;
  } catch (error) {
    state.error = error.response?.data?.message || 'Login failed';
    state.isLoading = false;
    throw error;
  }
};

// Logout user
const logout = () => {
  // Clear localStorage
  localStorage.removeItem('token');
  localStorage.removeItem('user');
  
  // Reset state
  state.user = null;
  state.isAuthenticated = false;
};

// Check if user is authenticated
const isAuthenticated = () => {
  return state.isAuthenticated;
};

// Get current user
const getCurrentUser = () => {
  return state.user;
};

// Initialize auth state
initializeAuth();

export default {
  state,
  register,
  login,
  logout,
  isAuthenticated,
  getCurrentUser
};

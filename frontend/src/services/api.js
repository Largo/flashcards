import axios from 'axios';

const apiClient = axios.create({
  baseURL: 'http://localhost:5046/api',
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: false
});

// Add a request interceptor to include the auth token in requests
apiClient.interceptors.request.use(
  config => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  error => {
    return Promise.reject(error);
  }
);

export default {
  // Auth
  register(userData) {
    return apiClient.post('/auth/register', userData);
  },
  
  login(credentials) {
    return apiClient.post('/auth/login', credentials);
  },
  
  getCurrentUser() {
    return apiClient.get('/auth/user');
  },
  
  // Flashcards
  getFlashcards() {
    return apiClient.get('/flashcards');
  },
  
  getFlashcard(id) {
    return apiClient.get(`/flashcards/${id}`);
  },
  
  createFlashcard(flashcard) {
    return apiClient.post('/flashcards', flashcard);
  },
  
  updateFlashcard(id, flashcard) {
    return apiClient.put(`/flashcards/${id}`, flashcard);
  },
  
  deleteFlashcard(id) {
    return apiClient.delete(`/flashcards/${id}`);
  },
  
  getDueFlashcards() {
    return apiClient.get('/flashcards/due');
  },
  
  reviewFlashcard(id, qualityOfResponse) {
    return apiClient.post(`/flashcards/${id}/review`, { qualityOfResponse });
  }
};

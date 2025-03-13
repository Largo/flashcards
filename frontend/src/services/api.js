import axios from 'axios';

const apiClient = axios.create({
  baseURL: 'http://localhost:5046/api',
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: false
});

export default {
  // Flashcards
  getFlashcards() {
    return apiClient.get('/flashcards');
  },
  getFlashcard(id) {
    return apiClient.get(`/flashcards/${id}`);
  },
  getDueFlashcards() {
    return apiClient.get('/flashcards/due');
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
  reviewFlashcard(id, qualityOfResponse) {
    return apiClient.post(`/flashcards/${id}/review`, { qualityOfResponse });
  },

  // Categories
  getCategories() {
    return apiClient.get('/categories');
  },
  getCategory(id) {
    return apiClient.get(`/categories/${id}`);
  },
  getCategoryFlashcards(id) {
    return apiClient.get(`/categories/${id}/flashcards`);
  },
  createCategory(category) {
    return apiClient.post('/categories', category);
  },
  updateCategory(id, category) {
    return apiClient.put(`/categories/${id}`, category);
  },
  deleteCategory(id) {
    return apiClient.delete(`/categories/${id}`);
  }
};

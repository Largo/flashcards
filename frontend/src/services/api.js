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

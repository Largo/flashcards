import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView
    },
    {
      path: '/flashcards',
      name: 'flashcards',
      component: () => import('../views/FlashcardsView.vue')
    },
    {
      path: '/flashcards/new',
      name: 'new-flashcard',
      component: () => import('../views/FlashcardFormView.vue')
    },
    {
      path: '/flashcards/:id',
      name: 'edit-flashcard',
      component: () => import('../views/FlashcardFormView.vue'),
      props: true
    },
    {
      path: '/study',
      name: 'study',
      component: () => import('../views/StudyView.vue')
    }
  ]
})

export default router

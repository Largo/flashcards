import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import auth from '../services/auth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('../views/LoginView.vue')
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('../views/RegisterView.vue')
    },
    {
      path: '/flashcards',
      name: 'flashcards',
      component: () => import('../views/FlashcardsView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/flashcards/new',
      name: 'new-flashcard',
      component: () => import('../views/FlashcardFormView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/flashcards/:id',
      name: 'edit-flashcard',
      component: () => import('../views/FlashcardFormView.vue'),
      props: true,
      meta: { requiresAuth: true }
    },
    {
      path: '/study',
      name: 'study',
      component: () => import('../views/StudyView.vue'),
      meta: { requiresAuth: true }
    }
  ]
})

// Navigation guard to check authentication
router.beforeEach((to, from, next) => {
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth)
  const isAuthenticated = auth.isAuthenticated()
  
  if (requiresAuth && !isAuthenticated) {
    next('/login')
  } else {
    next()
  }
})

export default router

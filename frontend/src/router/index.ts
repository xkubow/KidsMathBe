import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', redirect: '/dashboard' },
    { path: '/login', name: 'login', component: () => import('../pages/LoginPage.vue') },
    { path: '/register', name: 'register', component: () => import('../pages/RegisterPage.vue') },
    { path: '/dashboard', name: 'dashboard', component: () => import('../pages/ParentDashboardPage.vue'), meta: { requiresAuth: true } },
    { path: '/dashboard/add-child', name: 'parent-add-child', component: () => import('../pages/AddChildPage.vue'), meta: { requiresAuth: true } },
    { path: '/dashboard/child/:studentId/sessions', name: 'parent-session-history', component: () => import('../pages/ParentSessionHistoryPage.vue'), meta: { requiresAuth: true } },
    { path: '/dashboard/session/:sessionId', name: 'parent-session-detail', component: () => import('../pages/ParentSessionDetailPage.vue'), meta: { requiresAuth: true } },
    { path: '/student/select', name: 'student-select', component: () => import('../pages/StudentSelectPage.vue'), meta: { requiresAuth: true } },
    { path: '/student/home', name: 'student-home', component: () => import('../pages/StudentHomePage.vue'), meta: { requiresAuth: true, requiresStudent: true } },
    { path: '/student/tasks', name: 'task-selection', component: () => import('../pages/TaskSelectionPage.vue'), meta: { requiresAuth: true, requiresStudent: true } },
    { path: '/student/session/:sessionId', name: 'exercise-session', component: () => import('../pages/ExerciseSessionPage.vue'), meta: { requiresAuth: true, requiresStudent: true } },
    { path: '/student/result/:sessionId', name: 'session-result', component: () => import('../pages/SessionResultPage.vue'), meta: { requiresAuth: true, requiresStudent: true } },
    { path: '/student/worksheet/:sessionId', name: 'worksheet', component: () => import('../pages/WorksheetPage.vue'), meta: { requiresAuth: true, requiresStudent: true } },
    { path: '/student/achievements', name: 'achievements', component: () => import('../pages/AchievementsPage.vue'), meta: { requiresAuth: true, requiresStudent: true } }
  ]
})

router.beforeEach((to) => {
  const auth = useAuthStore()
  if (to.meta.requiresAuth && !auth.isAuthenticated) return { name: 'login' }
  if (to.meta.requiresStudent && !auth.activeStudentId) return { name: 'student-select' }
})

export default router

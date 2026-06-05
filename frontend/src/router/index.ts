import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', redirect: '/dashboard' },
    { path: '/login', name: 'login', component: () => import('../pages/LoginPage.vue'), meta: { backLabelKey: 'register', backTo: { name: 'register' } } },
    { path: '/register', name: 'register', component: () => import('../pages/RegisterPage.vue'), meta: { backLabelKey: 'login', backTo: { name: 'login' } } },
    { path: '/dashboard', name: 'dashboard', component: () => import('../pages/ParentDashboardPage.vue'), meta: { requiresAuth: true, hideBack: true } },
    { path: '/dashboard/add-child', name: 'parent-add-child', component: () => import('../pages/AddChildPage.vue'), meta: { requiresAuth: true, backLabelKey: 'parentDashboard', backTo: { name: 'dashboard' } } },
    {
      path: '/dashboard/child/:studentId/reset-pin',
      name: 'parent-reset-pin',
      component: () => import('../pages/ResetPinPage.vue'),
      meta: {
        requiresAuth: true,
        backTo: (route) =>
          route.query.from === 'dashboard'
            ? { name: 'dashboard' }
            : {
                name: 'student-select',
                query: {
                  studentId: route.params.studentId as string,
                  name: route.query.name as string | undefined,
                  avatar: route.query.avatar as string | undefined
                }
              }
      }
    },
    { path: '/dashboard/child/:studentId/sessions', name: 'parent-session-history', component: () => import('../pages/ParentSessionHistoryPage.vue'), meta: { requiresAuth: true, backLabelKey: 'parentDashboard', backTo: { name: 'dashboard' } } },
    {
      path: '/dashboard/session/:sessionId',
      name: 'parent-session-detail',
      component: () => import('../pages/ParentSessionDetailPage.vue'),
      meta: {
        requiresAuth: true,
        backTo: (route) => ({
          name: 'parent-session-history',
          params: { studentId: route.query.studentId as string },
          query: { name: route.query.name as string | undefined }
        })
      }
    },
    { path: '/admin/tasks', name: 'admin-tasks', component: () => import('../pages/AdminTaskDefinitionsPage.vue'), meta: { requiresAuth: true, requiresAdmin: true, backLabelKey: 'parentDashboard', backTo: { name: 'dashboard' } } },
    { path: '/admin/tasks/:id/config', name: 'admin-task-config', component: () => import('../pages/AdminTaskConfigJsonPage.vue'), meta: { requiresAuth: true, requiresAdmin: true, backTo: { name: 'admin-tasks' } } },
    { path: '/student/select', name: 'student-select', component: () => import('../pages/StudentSelectPage.vue'), meta: { requiresAuth: true, backLabelKey: 'parentDashboard', backTo: { name: 'dashboard' } } },
    { path: '/student/home', name: 'student-home', component: () => import('../pages/StudentHomePage.vue'), meta: { requiresAuth: true, requiresStudent: true, backLabelKey: 'selectChild', backTo: { name: 'student-select' } } },
    { path: '/student/tasks', name: 'task-selection', component: () => import('../pages/TaskSelectionPage.vue'), meta: { requiresAuth: true, requiresStudent: true, backLabelKey: 'backHome', backTo: { name: 'student-home' } } },
    { path: '/student/session/:sessionId', name: 'exercise-session', component: () => import('../pages/ExerciseSessionPage.vue'), meta: { requiresAuth: true, requiresStudent: true, backLabelKey: 'chooseTopic', backTo: { name: 'task-selection' } } },
    { path: '/student/result/:sessionId', name: 'session-result', component: () => import('../pages/SessionResultPage.vue'), meta: { requiresAuth: true, requiresStudent: true, backLabelKey: 'backHome', backTo: { name: 'student-home' } } },
    {
      path: '/student/worksheet/:sessionId',
      name: 'worksheet',
      component: () => import('../pages/WorksheetPage.vue'),
      meta: {
        requiresAuth: true,
        requiresStudent: true,
        backTo: (route) => ({ name: 'session-result', params: { sessionId: route.params.sessionId as string } })
      }
    },
    { path: '/student/achievements', name: 'achievements', component: () => import('../pages/AchievementsPage.vue'), meta: { requiresAuth: true, requiresStudent: true, backLabelKey: 'backHome', backTo: { name: 'student-home' } } }
  ]
})

router.beforeEach((to) => {
  const auth = useAuthStore()
  if (to.meta.requiresAuth && !auth.isAuthenticated) return { name: 'login' }
  if (to.meta.requiresAdmin && (!auth.isAdmin || !auth.isAdminMode)) return { name: 'dashboard' }
  if (to.meta.requiresStudent && !auth.activeStudentId) return { name: 'student-select' }
})

export default router

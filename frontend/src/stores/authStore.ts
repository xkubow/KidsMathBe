import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import api from '../api/client'

export const useAuthStore = defineStore('auth', () => {
  const parentToken = ref<string | null>(localStorage.getItem('parentToken'))
  const studentToken = ref<string | null>(localStorage.getItem('studentToken'))
  const adminToken = ref<string | null>(localStorage.getItem('adminToken'))
  const userId = ref<string | null>(localStorage.getItem('userId'))
  const displayName = ref<string | null>(localStorage.getItem('displayName'))
  const isAdmin = ref(localStorage.getItem('isAdmin') === 'true')
  const activeStudentId = ref<string | null>(localStorage.getItem('activeStudentId'))
  const activeStudentName = ref<string | null>(localStorage.getItem('activeStudentName'))

  const activeToken = computed(() => studentToken.value ?? adminToken.value ?? parentToken.value)
  const isAuthenticated = computed(() => !!activeToken.value)
  const isStudentMode = computed(() => !!studentToken.value)
  const isAdminMode = computed(() => !!adminToken.value)

  async function register(email: string, password: string, name: string) {
    const { data } = await api.post('/api/auth/register', { email, password, displayName: name })
    setParentSession(data.token, data.userId, data.displayName, data.isAdmin)
  }

  async function login(email: string, password: string) {
    const { data } = await api.post('/api/auth/login', { email, password })
    setParentSession(data.token, data.userId, data.displayName, data.isAdmin)
  }

  function setParentSession(token: string, id: string, name: string, admin = false) {
    exitStudentMode()
    exitAdminMode()
    parentToken.value = token
    userId.value = id
    displayName.value = name
    isAdmin.value = admin
    localStorage.setItem('parentToken', token)
    localStorage.setItem('userId', id)
    localStorage.setItem('displayName', name)
    localStorage.setItem('isAdmin', admin ? 'true' : 'false')
  }

  function setAdminSession(token: string, id: string, name: string) {
    exitStudentMode()
    parentToken.value = null
    localStorage.removeItem('parentToken')
    adminToken.value = token
    userId.value = id
    displayName.value = name
    isAdmin.value = true
    localStorage.setItem('adminToken', token)
    localStorage.setItem('userId', id)
    localStorage.setItem('displayName', name)
    localStorage.setItem('isAdmin', 'true')
  }

  function setStudentSession(token: string, studentId: string, studentName: string) {
    studentToken.value = token
    activeStudentId.value = studentId
    activeStudentName.value = studentName
    localStorage.setItem('studentToken', token)
    localStorage.setItem('activeStudentId', studentId)
    localStorage.setItem('activeStudentName', studentName)
  }

  function exitStudentMode() {
    studentToken.value = null
    activeStudentId.value = null
    activeStudentName.value = null
    localStorage.removeItem('studentToken')
    localStorage.removeItem('activeStudentId')
    localStorage.removeItem('activeStudentName')
    localStorage.removeItem('activeStudentAvatar')
  }

  function exitAdminMode() {
    adminToken.value = null
    localStorage.removeItem('adminToken')
  }

  async function switchToParent() {
    const { data } = await api.post('/api/auth/switch-to-parent')
    exitStudentMode()
    exitAdminMode()
    setParentSession(data.token, data.userId, data.displayName, data.isAdmin)
  }

  async function switchToAdmin() {
    const { data } = await api.post('/api/auth/switch-to-admin')
    setAdminSession(data.token, data.userId, data.displayName)
  }

  async function refreshProfile() {
    const { data } = await api.get('/api/auth/me')
    isAdmin.value = !!data.isAdmin
    localStorage.setItem('isAdmin', data.isAdmin ? 'true' : 'false')
  }

  function logout() {
    parentToken.value = null
    studentToken.value = null
    adminToken.value = null
    userId.value = null
    displayName.value = null
    isAdmin.value = false
    activeStudentId.value = null
    activeStudentName.value = null
    localStorage.clear()
  }

  return {
    parentToken,
    studentToken,
    adminToken,
    userId,
    displayName,
    isAdmin,
    activeStudentId,
    activeStudentName,
    activeToken,
    isAuthenticated,
    isStudentMode,
    isAdminMode,
    register,
    login,
    setParentSession,
    setAdminSession,
    setStudentSession,
    exitStudentMode,
    exitAdminMode,
    switchToParent,
    switchToAdmin,
    refreshProfile,
    logout
  }
})

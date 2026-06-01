import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import api from '../api/client'

export const useAuthStore = defineStore('auth', () => {
  const parentToken = ref<string | null>(localStorage.getItem('parentToken'))
  const studentToken = ref<string | null>(localStorage.getItem('studentToken'))
  const userId = ref<string | null>(localStorage.getItem('userId'))
  const displayName = ref<string | null>(localStorage.getItem('displayName'))
  const activeStudentId = ref<string | null>(localStorage.getItem('activeStudentId'))
  const activeStudentName = ref<string | null>(localStorage.getItem('activeStudentName'))

  const activeToken = computed(() => studentToken.value ?? parentToken.value)
  const isAuthenticated = computed(() => !!activeToken.value)
  const isStudentMode = computed(() => !!studentToken.value)

  async function register(email: string, password: string, name: string) {
    const { data } = await api.post('/api/auth/register', { email, password, displayName: name })
    setParentSession(data.token, data.userId, data.displayName)
  }

  async function login(email: string, password: string) {
    const { data } = await api.post('/api/auth/login', { email, password })
    setParentSession(data.token, data.userId, data.displayName)
  }

  function setParentSession(token: string, id: string, name: string) {
    exitStudentMode()
    parentToken.value = token
    userId.value = id
    displayName.value = name
    localStorage.setItem('parentToken', token)
    localStorage.setItem('userId', id)
    localStorage.setItem('displayName', name)
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

  async function switchToParent() {
    const { data } = await api.post('/api/auth/switch-to-parent')
    exitStudentMode()
    setParentSession(data.token, data.userId, data.displayName)
  }

  function logout() {
    parentToken.value = null
    studentToken.value = null
    userId.value = null
    displayName.value = null
    activeStudentId.value = null
    activeStudentName.value = null
    localStorage.clear()
  }

  return {
    parentToken,
    studentToken,
    userId,
    displayName,
    activeStudentId,
    activeStudentName,
    activeToken,
    isAuthenticated,
    isStudentMode,
    register,
    login,
    setParentSession,
    setStudentSession,
    exitStudentMode,
    switchToParent,
    logout
  }
})

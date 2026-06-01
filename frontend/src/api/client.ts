import axios from 'axios'
import { useAuthStore } from '../stores/authStore'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? ''
})

api.interceptors.request.use((config) => {
  const auth = useAuthStore()
  const token = auth.activeToken
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  const lang = localStorage.getItem('lang') ?? 'cs'
  config.params = { ...config.params, lang }
  return config
})

export default api

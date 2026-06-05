import axios, { type AxiosError } from 'axios'
import { useAuthStore } from '../stores/authStore'
import { useUiStore } from '../stores/uiStore'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? ''
})

function isPublicAuthRequest(url?: string): boolean {
  if (!url) return false
  return url.includes('/api/auth/login') || url.includes('/api/auth/register')
}

api.interceptors.request.use((config) => {
  const auth = useAuthStore()
  const ui = useUiStore()
  ui.beginRequest()
  ;(config as { __km_pending?: boolean }).__km_pending = true
  const token = auth.activeToken
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  const lang = localStorage.getItem('lang') ?? 'cs'
  config.params = { ...config.params, lang }
  return config
})

api.interceptors.response.use(
  (response) => {
    const ui = useUiStore()
    if ((response.config as { __km_pending?: boolean }).__km_pending) ui.endRequest()
    return response
  },
  async (error: AxiosError) => {
    const ui = useUiStore()
    const auth = useAuthStore()
    const config = error.config as { __km_pending?: boolean; url?: string } | undefined
    if (config?.__km_pending) ui.endRequest()

    const status = error.response?.status
    const url = config?.url ?? ''

    if (status === 401 && !isPublicAuthRequest(url)) {
      auth.logout()
      ui.showFlash('sessionExpired')
      const { default: router } = await import('../router')
      const name = router.currentRoute.value.name
      if (name !== 'login' && name !== 'register') {
        await router.push({ name: 'login' })
      }
    } else if (status === 403) {
      ui.showFlash('unauthorized')
      if (url.includes('/api/auth/switch-to-admin')) {
        auth.isAdmin = false
        localStorage.setItem('isAdmin', 'false')
      }
    }

    return Promise.reject(error)
  }
)

export function isUnauthorized(error: unknown): boolean {
  return axios.isAxiosError(error) && error.response?.status === 403
}

export function isUnauthenticated(error: unknown): boolean {
  return axios.isAxiosError(error) && error.response?.status === 401
}

export default api

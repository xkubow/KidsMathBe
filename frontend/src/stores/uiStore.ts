import { defineStore } from 'pinia'
import { computed, ref } from 'vue'

export const useUiStore = defineStore('ui', () => {
  const pendingRequests = ref(0)
  const flashKey = ref<string | null>(null)
  let flashTimer: ReturnType<typeof setTimeout> | undefined

  const isLoading = computed(() => pendingRequests.value > 0)

  function beginRequest() {
    pendingRequests.value++
  }

  function endRequest() {
    pendingRequests.value = Math.max(0, pendingRequests.value - 1)
  }

  function resetRequests() {
    pendingRequests.value = 0
  }

  function showFlash(key: string, durationMs = 6000) {
    flashKey.value = key
    if (flashTimer) clearTimeout(flashTimer)
    flashTimer = setTimeout(() => {
      flashKey.value = null
    }, durationMs)
  }

  function clearFlash() {
    flashKey.value = null
    if (flashTimer) clearTimeout(flashTimer)
  }

  return {
    pendingRequests,
    isLoading,
    flashKey,
    beginRequest,
    endRequest,
    resetRequests,
    showFlash,
    clearFlash
  }
})


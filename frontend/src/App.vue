<template>
  <div class="app-shell">
    <div v-if="ui.isLoading" class="loading-overlay" aria-live="polite" aria-busy="true">
      <LoadingSpinner />
    </div>
    <div
      v-if="ui.flashKey"
      class="flash-banner"
      role="alert"
      @click="ui.clearFlash()"
    >
      {{ t(ui.flashKey as 'unauthorized') }}
    </div>
    <header v-if="showHeader" class="app-header">
      <h1>🧮 Matika</h1>
      <div class="header-actions">
        <select v-model="locale" class="lang-select" @change="saveLocale">
          <option value="cs">Čeština</option>
          <option value="en">English</option>
        </select>
        <button v-if="auth.isStudentMode || auth.isAdminMode" class="btn btn-ghost" @click="switchToParent">
          ← {{ t('parentDashboard') }}
        </button>
        <button v-if="auth.isAuthenticated" class="btn btn-ghost" @click="logout">
          {{ t('logout') }}
        </button>
      </div>
    </header>
    <main class="app-main">
      <PageBackNav />
      <RouterView v-slot="{ Component }">
        <Transition name="page" mode="out-in">
          <component :is="Component" />
        </Transition>
      </RouterView>
    </main>
  </div>
</template>

<script setup lang="ts">
import { computed, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from './stores/authStore'
import { useUiStore } from './stores/uiStore'
import { useI18n } from './composables/useI18n'
import LoadingSpinner from './components/LoadingSpinner.vue'
import PageBackNav from './components/PageBackNav.vue'

const auth = useAuthStore()
const ui = useUiStore()
const route = useRoute()
const router = useRouter()
const { locale, t, setLocale } = useI18n()

locale.value = (localStorage.getItem('lang') ?? 'cs').startsWith('en') ? 'en' : 'cs'

const showHeader = computed(() => !['login', 'register'].includes(route.name as string))

function saveLocale() {
  setLocale(locale.value)
}

async function switchToParent() {
  try {
    await auth.switchToParent()
    router.push({ name: 'dashboard' })
  } catch {
    // 401/403 handled in api client
  }
}

function logout() {
  auth.logout()
  router.push({ name: 'login' })
}

watch(() => auth.isAuthenticated, (ok) => {
  if (!ok && route.meta.requiresAuth) router.push({ name: 'login' })
})
</script>

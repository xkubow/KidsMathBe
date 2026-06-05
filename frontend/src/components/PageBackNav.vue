<template>
  <nav v-if="show" class="page-back-nav">
    <button class="btn btn-ghost page-back-btn" type="button" @click="goBack">
      ← {{ label }}
    </button>
  </nav>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute, useRouter, type RouteLocationRaw } from 'vue-router'
import { useI18n } from '../composables/useI18n'

type BackTo = RouteLocationRaw | ((route: ReturnType<typeof useRoute>) => RouteLocationRaw) | 'history'

const route = useRoute()
const router = useRouter()
const { t } = useI18n()

const show = computed(() => !route.meta.hideBack)

const label = computed(() => {
  if (route.name === 'parent-reset-pin') {
    return route.query.from === 'dashboard' ? t('parentDashboard') : t('enterPin')
  }
  const key = route.meta.backLabelKey
  return key ? t(key as 'back') : t('back')
})

function resolveBackTo(): BackTo | undefined {
  if (route.meta.backTo) return route.meta.backTo as BackTo

  const defaults: Record<string, BackTo> = {
    login: { name: 'register' },
    register: { name: 'login' },
    'parent-add-child': { name: 'dashboard' },
    'parent-session-history': { name: 'dashboard' },
    'admin-tasks': { name: 'dashboard' },
    'admin-task-config': { name: 'admin-tasks' },
    'student-select': { name: 'dashboard' },
    'student-home': { name: 'student-select' },
    'task-selection': { name: 'student-home' },
    'exercise-session': { name: 'task-selection' },
    'session-result': { name: 'student-home' },
    achievements: { name: 'student-home' }
  }

  return route.name ? defaults[route.name as string] : undefined
}

function goBack() {
  const target = resolveBackTo()
  if (!target) {
    router.back()
    return
  }
  if (target === 'history') {
    router.back()
    return
  }
  const location = typeof target === 'function' ? target(route) : target
  router.push(location)
}
</script>

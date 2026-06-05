<template>
  <div>
    <div class="card">
      <h2>{{ t('practiceHistory') }}</h2>
      <p>{{ childName }}</p>
    </div>

    <p v-if="loading">{{ t('loading') }}</p>
    <p v-else-if="!sessions.length">{{ t('noSessions') }}</p>

    <div v-for="s in sessions" :key="s.id" class="card session-row">
      <div>
        <strong>{{ formatDate(s.startedAtUtc) }}</strong>
        <span class="muted"> · {{ s.taskType }}</span>
      </div>
      <p>{{ s.correctAnswers }} / {{ s.totalQuestions }} ✓ · {{ s.status }}</p>
      <button class="btn btn-primary" @click="openSession(s.id)">{{ t('viewDetails') }}</button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '../api/client'
import { useI18n } from '../composables/useI18n'

interface SessionSummary {
  id: string
  startedAtUtc: string
  taskType: string
  correctAnswers: number
  wrongAnswers: number
  totalQuestions: number
  status: string
}

const route = useRoute()
const router = useRouter()
const { t } = useI18n()
const studentId = route.params.studentId as string
const childName = (route.query.name as string) ?? ''
const sessions = ref<SessionSummary[]>([])
const loading = ref(true)

onMounted(async () => {
  try {
    const { data } = await api.get('/api/exercise-sessions', { params: { studentId } })
    sessions.value = data
  } finally {
    loading.value = false
  }
})

function formatDate(iso: string) {
  return new Date(iso).toLocaleString()
}

function openSession(sessionId: string) {
  router.push({
    name: 'parent-session-detail',
    params: { sessionId },
    query: { studentId, name: childName }
  })
}
</script>

<style scoped>
.session-row { margin-bottom: 0.75rem; }
.muted { color: #64748b; }
</style>

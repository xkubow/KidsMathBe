<template>
  <div v-if="session">
    <div class="card">
      <h2>{{ t('sessionDetail') }}</h2>
      <p>{{ childName }} · {{ formatDate(session.startedAtUtc) }}</p>
      <p class="big-number">{{ session.correctAnswers }} / {{ session.totalQuestions }} ✓</p>
    </div>

    <div v-for="a in session.attempts" :key="a.id" class="card question-card">
      <h3>{{ a.questionOrder }}. {{ a.questionText }}</h3>
      <p v-if="a.isResolved" :class="a.isCorrect ? 'ok' : 'bad'">
        {{ t('finalAnswer') }}: {{ a.studentAnswer ?? '—' }}
        <span v-if="!a.isCorrect"> ({{ t('correctWas') }}: {{ a.correctAnswer }})</span>
      </p>
      <ul class="submission-list">
        <li v-for="s in a.submissions" :key="s.attemptNumber" :class="s.isCorrect ? 'ok' : 'bad'">
          <span class="try-num">#{{ s.attemptNumber }}</span>
          {{ s.answer }}
          <span>{{ s.isCorrect ? '✓' : '✗' }}</span>
          <time class="muted">{{ formatTime(s.submittedAtUtc) }}</time>
        </li>
      </ul>
      <p v-if="!a.submissions.length" class="muted">{{ t('noAttempts') }}</p>
    </div>
  </div>
  <p v-else-if="loading">{{ t('loading') }}</p>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import api from '../api/client'
import { useI18n } from '../composables/useI18n'

interface Submission {
  attemptNumber: number
  answer: string
  isCorrect: boolean
  submittedAtUtc: string
}

interface AttemptRow {
  id: string
  questionOrder: number
  questionText: string
  studentAnswer: string | null
  correctAnswer: string | null
  isCorrect: boolean | null
  isResolved: boolean
  submissions: Submission[]
}

const route = useRoute()
const { t } = useI18n()
const childName = (route.query.name as string) ?? ''
const loading = ref(true)
const session = ref<{
  startedAtUtc: string
  correctAnswers: number
  totalQuestions: number
  attempts: AttemptRow[]
} | null>(null)

onMounted(async () => {
  try {
    const { data } = await api.get(`/api/exercise-sessions/${route.params.sessionId}`)
    session.value = data
  } finally {
    loading.value = false
  }
})

function formatDate(iso: string) {
  return new Date(iso).toLocaleString()
}

function formatTime(iso: string) {
  return new Date(iso).toLocaleTimeString()
}
</script>

<style scoped>
.question-card { margin-bottom: 0.75rem; }
.submission-list { list-style: none; padding: 0; margin: 0.5rem 0 0; }
.submission-list li { padding: 0.35rem 0; border-bottom: 1px solid #f1f5f9; display: flex; gap: 0.5rem; align-items: baseline; flex-wrap: wrap; }
.try-num { font-weight: 700; min-width: 2rem; }
.ok { color: var(--success); }
.bad { color: var(--warning); }
.muted { color: #94a3b8; font-size: 0.85rem; }
</style>

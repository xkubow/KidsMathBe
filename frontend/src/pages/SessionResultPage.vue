<template>
  <div v-if="session" class="card animate-pop">
    <h2>{{ t('sessionResult') }}</h2>
    <p class="big-number">{{ session.correctAnswers }} / {{ session.totalQuestions }} ✓</p>
    <ul class="result-list">
      <li v-for="a in session.attempts" :key="a.id" :class="a.isCorrect ? 'ok' : 'bad'">
        <div class="result-question">
          {{ a.questionText }} → {{ a.studentAnswer ?? '—' }}
          <span>{{ a.isCorrect ? '✓' : '✗' }}</span>
        </div>
        <ul v-if="a.submissions?.length > 1" class="submission-history">
          <li v-for="s in a.submissions" :key="s.attemptNumber" :class="s.isCorrect ? 'ok' : 'bad'">
            {{ s.attemptNumber }}. {{ s.answer }}
            <span>{{ s.isCorrect ? '✓' : '✗' }}</span>
          </li>
        </ul>
      </li>
    </ul>
    <button class="btn btn-primary btn-block" @click="router.push({ name: 'worksheet', params: { sessionId: route.params.sessionId } })">
      🖨️ {{ t('printWorksheet') }}
    </button>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '../api/client'
import { useI18n } from '../composables/useI18n'

interface Submission {
  attemptNumber: number
  answer: string
  isCorrect: boolean
}

interface AttemptRow {
  id: string
  questionText: string
  studentAnswer: string | null
  isCorrect: boolean
  submissions: Submission[]
}

const route = useRoute()
const router = useRouter()
const { t } = useI18n()
const session = ref<{
  correctAnswers: number
  totalQuestions: number
  attempts: AttemptRow[]
} | null>(null)

onMounted(async () => {
  const { data } = await api.get(`/api/exercise-sessions/${route.params.sessionId}`)
  session.value = data
})
</script>

<style scoped>
.result-list { list-style: none; padding: 0; }
.result-list > li { padding: .75rem 0; border-bottom: 1px solid #e2e8f0; }
.result-list li.ok { color: var(--success); }
.result-list li.bad { color: var(--warning); }
.submission-history {
  list-style: none;
  padding: 0.35rem 0 0 1rem;
  margin: 0;
  font-size: 0.9rem;
  opacity: 0.9;
}
.submission-history li { padding: 0.15rem 0; }
</style>

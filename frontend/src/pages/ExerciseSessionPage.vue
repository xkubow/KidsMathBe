<template>
  <div v-if="current" class="card exercise-card" :class="cardClass">
    <p>Úloha {{ index + 1 }} / {{ attempts.length }}</p>
    <div class="progress-bar"><span :style="{ width: progress + '%' }" /></div>
    <p class="big-number">{{ current.questionText }}</p>
    <p v-if="!isResolved" class="attempts-hint">{{ attemptsUsed }} / {{ maxAttempts }}</p>
    <FeedbackBurst :show="feedback === 'ok'" type="ok" />
    <FeedbackBurst :show="feedback === 'bad'" type="bad" />
    <input
      v-model="answer"
      type="text"
      inputmode="numeric"
      :disabled="isResolved"
      @keyup.enter="submit"
    />
    <p v-if="feedback === 'ok'" class="feedback-ok">{{ t('correct') }}</p>
    <p v-else-if="feedback === 'bad' && !isResolved" class="feedback-warn">{{ t('tryAgain') }}</p>
    <p v-else-if="feedback === 'bad' && isResolved" class="feedback-warn">
      {{ t('attemptsExhausted') }}: {{ revealedCorrect }}
    </p>
    <button class="btn btn-primary btn-block" @click="submit">{{ isResolved ? t('next') : 'OK' }}</button>
    <button v-if="allDone" class="btn btn-success btn-block" @click="finish">{{ t('finish') }}</button>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '../api/client'
import { useI18n } from '../composables/useI18n'
import FeedbackBurst from '../components/FeedbackBurst.vue'

interface Submission {
  attemptNumber: number
  answer: string
  isCorrect: boolean
  submittedAtUtc: string
}

interface Attempt {
  id: string
  questionText: string
  isCorrect: boolean | null
  isResolved: boolean
  studentAnswer: string | null
  attemptsUsed: number
  maxAttempts: number
  submissions: Submission[]
}

const route = useRoute()
const router = useRouter()
const { t } = useI18n()
const sessionId = route.params.sessionId as string
const attempts = ref<Attempt[]>([])
const maxAttempts = ref(10)
const index = ref(0)
const answer = ref('')
const feedback = ref<'ok' | 'bad' | null>(null)
const cardClass = ref('')
const revealedCorrect = ref('')

const current = computed(() => attempts.value[index.value])
const isResolved = computed(() => current.value?.isResolved ?? false)
const attemptsUsed = computed(() => current.value?.attemptsUsed ?? 0)
const allDone = computed(() => attempts.value.every(a => a.isResolved))
const progress = computed(() => ((index.value + 1) / attempts.value.length) * 100)

onMounted(load)

async function load() {
  const { data } = await api.get(`/api/exercise-sessions/${sessionId}`)
  maxAttempts.value = data.maxAttemptsPerQuestion ?? 10
  attempts.value = data.attempts
  const firstOpen = attempts.value.findIndex(a => !a.isResolved)
  index.value = firstOpen >= 0 ? firstOpen : 0
}

function goNext() {
  if (index.value < attempts.value.length - 1) {
    index.value++
    answer.value = ''
    feedback.value = null
    cardClass.value = ''
    revealedCorrect.value = ''
  }
}

async function submit() {
  if (!current.value) return
  if (current.value.isResolved) {
    goNext()
    return
  }
  const { data } = await api.post(`/api/exercise-sessions/${sessionId}/answer`, {
    attemptId: current.value.id,
    answer: answer.value
  })
  current.value.attemptsUsed = data.attemptsUsed
  current.value.isResolved = data.questionResolved
  if (data.questionResolved) {
    current.value.isCorrect = data.finalOutcome
    current.value.studentAnswer = data.studentAnswer
  }
  feedback.value = data.isCorrect ? 'ok' : 'bad'
  cardClass.value = data.isCorrect ? 'card-success' : 'card-shake'

  if (data.questionResolved && data.isCorrect && index.value < attempts.value.length - 1) {
    setTimeout(goNext, 700)
  } else if (data.questionResolved && !data.isCorrect) {
    revealedCorrect.value = data.correctAnswer ?? ''
    answer.value = ''
  } else if (!data.questionResolved) {
    answer.value = ''
  }
}

async function finish() {
  await api.post(`/api/exercise-sessions/${sessionId}/finish`)
  router.push({ name: 'session-result', params: { sessionId } })
}
</script>

<style scoped>
.attempts-hint {
  font-size: 0.9rem;
  color: #64748b;
  margin: 0.25rem 0;
}
</style>

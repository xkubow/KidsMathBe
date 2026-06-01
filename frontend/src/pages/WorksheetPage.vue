<template>
  <div class="worksheet-wrap">
    <div v-if="session" class="worksheet card" id="worksheet-print">
      <header class="worksheet-header">
        <span class="worksheet-avatar">{{ avatarEmoji(avatarKey) }}</span>
        <div>
          <h1>{{ t('worksheetTitle') }}</h1>
          <p>{{ auth.activeStudentName }} · {{ new Date(session.startedAtUtc).toLocaleDateString() }}</p>
        </div>
      </header>
      <ol class="worksheet-list">
        <li v-for="(a, i) in session.attempts" :key="a.id">
          <span class="q-num">{{ i + 1 }}.</span>
          <span class="q-text">{{ a.questionText }}</span>
          <span class="q-answer">{{ a.studentAnswer ?? '______' }}</span>
          <span class="q-mark">{{ a.isCorrect ? '✓' : a.isCorrect === false ? '✗' : '' }}</span>
        </li>
      </ol>
      <p class="worksheet-score">{{ t('sessionResult') }}: {{ session.correctAnswers }}/{{ session.totalQuestions }}</p>
    </div>
    <div class="no-print actions">
      <button class="btn btn-primary btn-block" @click="printSheet">🖨️ {{ t('printWorksheet') }}</button>
      <button class="btn btn-ghost btn-block" @click="router.back()">←</button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '../api/client'
import { useAuthStore } from '../stores/authStore'
import { useI18n } from '../composables/useI18n'
import { avatarEmoji } from '../constants/avatars'

const route = useRoute()
const router = useRouter()
const auth = useAuthStore()
const { t } = useI18n()
const avatarKey = localStorage.getItem('activeStudentAvatar') ?? 'fox'
const session = ref<{
  startedAtUtc: string
  correctAnswers: number
  totalQuestions: number
  attempts: { id: string; questionText: string; studentAnswer: string | null; isCorrect: boolean | null }[]
} | null>(null)

onMounted(async () => {
  const { data } = await api.get(`/api/exercise-sessions/${route.params.sessionId}`)
  session.value = data
})

function printSheet() {
  window.print()
}
</script>

<style scoped>
@media print {
  .no-print { display: none !important; }
  .worksheet { box-shadow: none; }
}
.worksheet-header { display: flex; gap: 1rem; align-items: center; margin-bottom: 1rem; }
.worksheet-avatar { font-size: 3rem; }
.worksheet-list { line-height: 2; font-size: 1.15rem; }
.q-num { font-weight: 700; margin-right: .5rem; }
.q-answer { margin-left: 1rem; border-bottom: 2px solid #333; min-width: 3rem; display: inline-block; }
.q-mark { margin-left: .5rem; }
.worksheet-score { font-weight: 700; margin-top: 1rem; }
</style>

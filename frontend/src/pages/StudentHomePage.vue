<template>
  <div>
    <div class="card animate-pop student-card">
      <span class="student-avatar">{{ avatarEmoji(avatarKey) }}</span>
      <div>
        <h2>👋 {{ auth.activeStudentName }}</h2>
        <button class="btn btn-primary btn-block" @click="router.push('/student/tasks')">{{ t('chooseTopic') }}</button>
        <button class="btn btn-ghost btn-block" @click="router.push('/student/achievements')">{{ t('achievements') }}</button>
        <button class="btn btn-ghost btn-block" @click="loadSummary">{{ t('summary') }}</button>
      </div>
    </div>

    <div v-if="summary" class="card animate-pop">
      <h3>{{ t('summary') }}</h3>
      <p>{{ summary.totalCorrect }} / {{ summary.totalAnswered }} ✓</p>
      <div class="progress-bar"><span :style="{ width: percent + '%' }" /></div>
      <div v-if="summary.achievements?.length" class="badges-wrap">
        <AchievementBadge
          v-for="a in summary.achievements"
          :key="a.code"
          :display-name="a.displayName"
        />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '../api/client'
import { useAuthStore } from '../stores/authStore'
import { useI18n } from '../composables/useI18n'
import { avatarEmoji } from '../constants/avatars'
import AchievementBadge from '../components/AchievementBadge.vue'

const auth = useAuthStore()
const router = useRouter()
const { t } = useI18n()
const avatarKey = localStorage.getItem('activeStudentAvatar') ?? 'fox'
const summary = ref<{ totalCorrect: number; totalAnswered: number; achievements: { code: string; displayName: string }[] } | null>(null)
const percent = computed(() => {
  if (!summary.value?.totalAnswered) return 0
  return Math.round((summary.value.totalCorrect / summary.value.totalAnswered) * 100)
})

async function loadSummary() {
  const { data } = await api.get(`/api/students/${auth.activeStudentId}/summary`)
  summary.value = data
}
</script>

<style scoped>
.badges-wrap { display: flex; flex-wrap: wrap; gap: .5rem; margin-top: .75rem; }
</style>

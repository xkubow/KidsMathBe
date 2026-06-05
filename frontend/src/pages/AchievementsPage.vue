<template>
  <div class="card animate-pop">
    <h2>{{ t('achievements') }}</h2>
    <p v-if="!items.length">{{ t('loading') }}</p>
    <div class="badges-wrap">
      <AchievementBadge
        v-for="a in items"
        :key="a.code"
        :display-name="a.displayName"
        :description="a.description"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import api from '../api/client'
import { useAuthStore } from '../stores/authStore'
import { useI18n } from '../composables/useI18n'
import AchievementBadge from '../components/AchievementBadge.vue'

const auth = useAuthStore()
const { t } = useI18n()
const items = ref<{ code: string; displayName: string; description: string }[]>([])

onMounted(async () => {
  const { data } = await api.get(`/api/students/${auth.activeStudentId}/achievements`)
  items.value = data
})
</script>

<style scoped>
.badges-wrap { display: flex; flex-wrap: wrap; gap: .5rem; margin: 1rem 0; }
</style>

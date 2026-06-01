<template>
  <div class="card">
    <h2>{{ t('chooseTopic') }}</h2>
    <p v-if="loading">{{ t('loading') }}</p>
    <div class="grid">
      <button
        v-for="task in tasks"
        :key="task.id"
        class="btn btn-primary btn-block"
        @click="start(task.id)"
      >
        {{ task.displayName }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '../api/client'
import { useAuthStore } from '../stores/authStore'
import { useI18n } from '../composables/useI18n'

interface TaskDef { id: string; displayName: string }

const auth = useAuthStore()
const router = useRouter()
const { t } = useI18n()
const tasks = ref<TaskDef[]>([])
const loading = ref(true)

onMounted(async () => {
  const student = await api.get(`/api/students/${auth.activeStudentId}`)
  const { data } = await api.get('/api/math/task-definitions', { params: { grade: student.data.grade } })
  tasks.value = data
  loading.value = false
})

async function start(taskDefinitionId: string) {
  const { data } = await api.post('/api/exercise-sessions/start', {
    studentProfileId: auth.activeStudentId,
    taskDefinitionId,
    questionCount: 10
  })
  router.push({ name: 'exercise-session', params: { sessionId: data.id } })
}
</script>

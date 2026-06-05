<template>
  <div class="card animate-pop">
    <h2>{{ t('configEditorTitle') }}</h2>
    <p class="admin-subtitle">{{ t('configEditorSubtitle') }}</p>

    <p v-if="loading">{{ t('loading') }}</p>

    <div v-else-if="task && config">
      <div class="admin-task-row" style="border-bottom: 0; padding-top: 0;">
        <div class="admin-task-main">
          <strong>{{ task.displayNameCs }}</strong>
          <span class="admin-task-meta">
            {{ t('grade') }} {{ task.grade }} · {{ task.taskType }} · {{ t('difficulty') }} {{ task.difficultyLevel }}
          </span>
        </div>
      </div>

      <TaskConfigForm v-model="config" :task-type="taskType" />

      <p v-if="error" class="feedback-warn">{{ error }}</p>

      <button class="btn btn-primary btn-block" type="button" @click="save">
        {{ t('saveTask') }}
      </button>
    </div>

    <p v-else-if="error" class="feedback-warn">{{ error }}</p>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '../api/client'
import TaskConfigForm from '../components/admin/TaskConfigForm.vue'
import { useI18n } from '../composables/useI18n'
import type { AnyTaskConfig, TaskType } from '../types/taskConfig'
import { parseTaskConfig, serializeTaskConfig } from '../utils/taskConfigSerde'

interface TaskDefinition {
  id: string
  grade: number
  taskType: string
  difficultyLevel: number
  displayNameCs: string
  displayNameEn: string
  descriptionCs?: string | null
  descriptionEn?: string | null
  configJson: string
  isActive: boolean
  createdAtUtc: string
}

const { t } = useI18n()
const route = useRoute()
const router = useRouter()

const loading = ref(true)
const error = ref('')
const task = ref<TaskDefinition | null>(null)
const config = ref<AnyTaskConfig | null>(null)

const taskType = computed(() => task.value?.taskType as TaskType)

onMounted(load)

async function load() {
  loading.value = true
  error.value = ''
  try {
    const id = String(route.params.id ?? '')
    const { data } = await api.get<TaskDefinition>(`/api/admin/task-definitions/${id}`)
    task.value = data
    config.value = parseTaskConfig(data.taskType as TaskType, data.configJson || '{}')
  } catch {
    error.value = t('failedToLoad')
  } finally {
    loading.value = false
  }
}

async function save() {
  error.value = ''
  if (!task.value || !config.value) return

  if (config.value.minNumber > config.value.maxNumber) {
    error.value = t('minMaxInvalid')
    return
  }

  const configJson = serializeTaskConfig(taskType.value, config.value)

  const payload = {
    grade: task.value.grade,
    taskType: task.value.taskType,
    difficultyLevel: task.value.difficultyLevel,
    displayNameCs: task.value.displayNameCs,
    displayNameEn: task.value.displayNameEn,
    descriptionCs: task.value.descriptionCs ?? null,
    descriptionEn: task.value.descriptionEn ?? null,
    configJson,
    isActive: task.value.isActive
  }

  try {
    await api.put(`/api/admin/task-definitions/${task.value.id}`, payload)
    router.push({ name: 'admin-tasks' })
  } catch {
    error.value = t('saveFailed')
  }
}
</script>

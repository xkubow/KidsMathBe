<template>
  <div>
    <div class="card animate-pop">
      <h2>{{ t('adminTitle') }}</h2>
      <p class="admin-subtitle">{{ t('adminSubtitle') }}</p>

      <button class="btn btn-primary btn-block" type="button" @click="openCreate">
        {{ t('addTask') }}
      </button>

      <p v-if="loading">{{ t('loading') }}</p>
      <p v-else-if="!tasks.length">{{ t('noTasks') }}</p>

      <div v-for="task in tasks" :key="task.id" class="admin-task-row">
        <div class="admin-task-main">
          <strong>{{ task.displayNameCs }}</strong>
          <span class="admin-task-meta">
            {{ t('grade') }} {{ task.grade }} · {{ task.taskType }} · {{ t('difficulty') }} {{ task.difficultyLevel }}
          </span>
          <span v-if="!task.isActive" class="admin-badge-inactive">{{ t('inactive') }}</span>
        </div>
        <div class="admin-task-actions">
          <button class="btn btn-ghost" type="button" @click="openEdit(task)">{{ t('editTask') }}</button>
          <button class="btn btn-ghost" type="button" @click="editConfig(task)">{{ t('editConfigJson') }}</button>
        </div>
      </div>
    </div>

    <div v-if="editorOpen" class="card admin-editor">
      <h3>{{ editingId ? t('editTask') : t('addTask') }}</h3>
      <form @submit.prevent="save">
        <label>{{ t('grade') }}</label>
        <select v-model.number="form.grade">
          <option :value="1">1</option>
          <option :value="2">2</option>
          <option :value="3">3</option>
        </select>

        <label>{{ t('taskType') }}</label>
        <select v-model="form.taskType" @change="onTaskTypeChange">
          <option v-for="tt in taskTypes" :key="tt" :value="tt">{{ tt }}</option>
        </select>

        <label>{{ t('difficulty') }}</label>
        <input v-model.number="form.difficultyLevel" type="number" min="1" max="5" required />

        <label>{{ t('nameCs') }}</label>
        <input v-model="form.displayNameCs" required />

        <label>{{ t('nameEn') }}</label>
        <input v-model="form.displayNameEn" required />

        <p v-if="editingId" class="admin-subtitle">
          {{ t('configEditHint') }}
          <button class="btn btn-ghost" type="button" @click="editConfigById(editingId)">
            {{ t('editConfigJson') }}
          </button>
        </p>

        <label class="admin-checkbox">
          <input v-model="form.isActive" type="checkbox" />
          {{ t('active') }}
        </label>

        <p v-if="formError" class="feedback-warn">{{ formError }}</p>

        <button class="btn btn-primary btn-block" type="submit">{{ t('saveTask') }}</button>
        <button class="btn btn-ghost btn-block" type="button" @click="closeEditor">{{ t('cancel') }}</button>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import api from '../api/client'
import { useI18n } from '../composables/useI18n'
import { TASK_TYPES, type TaskType } from '../types/taskConfig'
import { defaultTaskConfig, serializeTaskConfig } from '../utils/taskConfigSerde'

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
const router = useRouter()
const tasks = ref<TaskDefinition[]>([])
const loading = ref(true)
const editorOpen = ref(false)
const editingId = ref<string | null>(null)
const formError = ref('')
const taskTypes = TASK_TYPES

const emptyForm = () => ({
  grade: 1,
  taskType: 'Addition' as TaskType,
  difficultyLevel: 1,
  displayNameCs: '',
  displayNameEn: '',
  descriptionCs: '',
  descriptionEn: '',
  configJson: serializeTaskConfig('Addition', defaultTaskConfig('Addition')),
  isActive: true
})

const form = reactive(emptyForm())
let configJsonLocked = false

onMounted(load)

async function load() {
  loading.value = true
  try {
    const { data } = await api.get<TaskDefinition[]>('/api/admin/task-definitions')
    tasks.value = data
  } finally {
    loading.value = false
  }
}

function editConfig(task: TaskDefinition) {
  router.push({ name: 'admin-task-config', params: { id: task.id } })
}

function editConfigById(id: string) {
  router.push({ name: 'admin-task-config', params: { id } })
}

function openCreate() {
  editingId.value = null
  configJsonLocked = false
  Object.assign(form, emptyForm())
  formError.value = ''
  editorOpen.value = true
}

function openEdit(task: TaskDefinition) {
  editingId.value = task.id
  configJsonLocked = true
  form.grade = task.grade
  form.taskType = task.taskType as TaskType
  form.difficultyLevel = task.difficultyLevel
  form.displayNameCs = task.displayNameCs
  form.displayNameEn = task.displayNameEn
  form.descriptionCs = task.descriptionCs ?? ''
  form.descriptionEn = task.descriptionEn ?? ''
  form.configJson = task.configJson
  form.isActive = task.isActive
  formError.value = ''
  editorOpen.value = true
}

function closeEditor() {
  editorOpen.value = false
  editingId.value = null
  configJsonLocked = false
}

function onTaskTypeChange() {
  if (configJsonLocked) return
  form.configJson = serializeTaskConfig(form.taskType, defaultTaskConfig(form.taskType))
}

watch(
  () => form.taskType,
  () => {
    if (!editorOpen.value || configJsonLocked) return
    onTaskTypeChange()
  }
)

async function save() {
  formError.value = ''

  const payload = {
    grade: form.grade,
    taskType: form.taskType,
    difficultyLevel: form.difficultyLevel,
    displayNameCs: form.displayNameCs,
    displayNameEn: form.displayNameEn,
    descriptionCs: form.descriptionCs || null,
    descriptionEn: form.descriptionEn || null,
    configJson: form.configJson,
    isActive: form.isActive
  }

  if (editingId.value) {
    await api.put(`/api/admin/task-definitions/${editingId.value}`, payload)
    closeEditor()
    await load()
  } else {
    const { data } = await api.post('/api/admin/task-definitions', payload)
    closeEditor()
    await load()
    router.push({ name: 'admin-task-config', params: { id: data.id } })
  }
}
</script>

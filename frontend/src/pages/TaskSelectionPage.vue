<template>
  <div
    class="rounded-2xl border-4 bg-gradient-to-b p-6 shadow-card-game transition-all"
    :class="[presentation.cardBorderClass, presentation.cardAccentClass]"
  >
    <div v-if="theme !== 'Default'" class="mb-4 flex justify-center">
      <span
        class="inline-flex items-center gap-1.5 rounded-full px-4 py-1.5 text-sm font-black"
        :class="presentation.badgeClass"
      >
        {{ themeBadgeText }}
      </span>
    </div>

    <h2 class="mb-4 text-center text-2xl font-black text-slate-800">
      {{ t('chooseTopic') }}
    </h2>

    <label for="theme-select" class="mb-1 block text-sm font-bold text-slate-600">
      {{ t('chooseTheme') }}
    </label>
    <select
      id="theme-select"
      v-model="selectedTheme"
      class="mb-4 w-full rounded-2xl border-4 bg-white p-3 text-lg font-bold text-slate-800 outline-none transition-all"
      :class="presentation.cardBorderClass"
    >
      <option v-for="themeOption in TEMPLATE_THEMES" :key="themeOption" :value="themeOption">
        {{ themeOptionLabel(themeOption) }}
      </option>
    </select>

    <p v-if="loading" class="mb-3 text-center text-slate-500">{{ t('loading') }}</p>
    <div class="grid gap-3">
      <button
        v-for="task in tasks"
        :key="task.id"
        class="w-full rounded-2xl border-b-8 bg-gradient-to-r py-4 text-lg font-black text-white shadow-game-btn transition-all duration-75 active:translate-y-2 active:border-b-0 [border-bottom-color:rgb(0_0_0_/_0.25)]"
        :class="presentation.progressGradientClass"
        @click="start(task.id)"
      >
        {{ task.displayName }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import api from '../api/client'
import { getThemePresentation } from '../constants/templateThemePresentation'
import { useAuthStore } from '../stores/authStore'
import { useI18n } from '../composables/useI18n'
import { useTemplateTheme } from '../composables/useTemplateTheme'
import {
  TEMPLATE_THEMES,
  loadPracticeTheme,
  savePracticeTheme,
  type TemplateTheme,
} from '../types/templateTheme'

interface TaskDef { id: string; displayName: string }

const auth = useAuthStore()
const router = useRouter()
const { t } = useI18n()
const tasks = ref<TaskDef[]>([])
const loading = ref(true)
const selectedTheme = ref<TemplateTheme>(loadPracticeTheme(auth.activeStudentId))
const { theme, presentation, themeBadgeText } = useTemplateTheme(selectedTheme)

watch(selectedTheme, themeValue => {
  savePracticeTheme(auth.activeStudentId, themeValue)
})

watch(
  () => auth.activeStudentId,
  studentId => {
    selectedTheme.value = loadPracticeTheme(studentId)
  },
)

onMounted(async () => {
  const student = await api.get(`/api/students/${auth.activeStudentId}`)
  const { data } = await api.get('/api/math/task-definitions', { params: { grade: student.data.grade } })
  tasks.value = data
  loading.value = false
})

function themeOptionLabel(themeOption: TemplateTheme) {
  const optionPresentation = getThemePresentation(themeOption)
  return `${optionPresentation.emoji} ${t(optionPresentation.labelKey)}`
}

async function start(taskDefinitionId: string) {
  const { data } = await api.post('/api/exercise-sessions/start', {
    studentProfileId: auth.activeStudentId,
    taskDefinitionId,
    questionCount: 10,
    theme: selectedTheme.value
  })
  router.push({ name: 'exercise-session', params: { sessionId: data.id } })
}
</script>

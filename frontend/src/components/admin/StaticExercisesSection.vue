<template>
  <div class="card admin-static-section">
    <h4>{{ t('staticExercises') }}</h4>

    <label>{{ t('staticChance') }}</label>
    <input
      :value="chance"
      type="number"
      min="0"
      max="100"
      @input="emit('update:chance', Number(($event.target as HTMLInputElement).value))"
    />
    <p class="admin-subtitle">{{ t('staticChanceHint') }}</p>

    <div v-for="(ex, idx) in exercises" :key="idx" class="admin-task-row admin-static-row">
      <div class="admin-static-fields">
        <label>{{ t('questionCs') }}</label>
        <input :value="ex.questionTextCs" @input="patch(idx, 'questionTextCs', ($event.target as HTMLInputElement).value)" />
        <label>{{ t('questionEn') }}</label>
        <input :value="ex.questionTextEn" @input="patch(idx, 'questionTextEn', ($event.target as HTMLInputElement).value)" />
        <label>{{ t('correctAnswer') }}</label>
        <input :value="ex.correctAnswer" @input="patch(idx, 'correctAnswer', ($event.target as HTMLInputElement).value)" />
        <label>{{ t('questionDataOptional') }}</label>
        <textarea
          :value="questionDataText(ex)"
          rows="2"
          @input="patchQuestionData(idx, ($event.target as HTMLTextAreaElement).value)"
        />
        <p v-if="errors[idx]" class="feedback-warn">{{ errors[idx] }}</p>
      </div>
      <button class="btn btn-ghost" type="button" @click="remove(idx)">{{ t('remove') }}</button>
    </div>

    <button class="btn btn-ghost btn-block" type="button" @click="add">{{ t('addStaticExercise') }}</button>
  </div>
</template>

<script setup lang="ts">
import { reactive } from 'vue'
import type { StaticExerciseConfig } from '../../types/taskConfig'
import { useI18n } from '../../composables/useI18n'

const props = defineProps<{
  chance: number
  exercises: StaticExerciseConfig[]
}>()

const emit = defineEmits<{
  'update:chance': [value: number]
  'update:exercises': [value: StaticExerciseConfig[]]
}>()

const { t } = useI18n()
const errors = reactive<Record<number, string>>({})

function questionDataText(ex: StaticExerciseConfig): string {
  return ex.questionData ? JSON.stringify(ex.questionData, null, 2) : ''
}

function patch(idx: number, key: keyof StaticExerciseConfig, value: string) {
  const next = props.exercises.map((ex, i) => (i === idx ? { ...ex, [key]: value } : ex))
  emit('update:exercises', next)
}

function patchQuestionData(idx: number, text: string) {
  delete errors[idx]
  const next = props.exercises.map((ex, i) => {
    if (i !== idx) return ex
    if (!text.trim()) {
      const { questionData: _, ...rest } = ex
      return rest as StaticExerciseConfig
    }
    try {
      return { ...ex, questionData: JSON.parse(text) as Record<string, unknown> }
    } catch {
      errors[idx] = t('invalidQuestionDataJson')
      return ex
    }
  })
  emit('update:exercises', next)
}

function add() {
  emit('update:exercises', [
    ...props.exercises,
    { questionTextCs: '', questionTextEn: '', correctAnswer: '' }
  ])
}

function remove(idx: number) {
  delete errors[idx]
  emit(
    'update:exercises',
    props.exercises.filter((_, i) => i !== idx)
  )
}
</script>

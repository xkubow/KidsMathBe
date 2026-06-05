<template>
  <div class="task-config-form">
    <h4>{{ t('generatorSettings') }}</h4>

    <label>{{ t('minNumber') }}</label>
    <input v-model.number="model.minNumber" type="number" />

    <label>{{ t('maxNumber') }}</label>
    <input v-model.number="model.maxNumber" type="number" />

    <template v-if="taskType === 'Addition'">
      <label class="admin-checkbox">
        <input v-model="addition.allowCarry" type="checkbox" />
        {{ t('allowCarry') }}
      </label>
      <label class="admin-checkbox">
        <input v-model="addition.allowNegativeResult" type="checkbox" />
        {{ t('allowNegativeResult') }}
      </label>
    </template>

    <template v-else-if="taskType === 'Subtraction'">
      <label class="admin-checkbox">
        <input v-model="subtraction.allowBorrow" type="checkbox" />
        {{ t('allowBorrow') }}
      </label>
      <label class="admin-checkbox">
        <input v-model="subtraction.allowNegativeResult" type="checkbox" />
        {{ t('allowNegativeResult') }}
      </label>
    </template>

    <template v-else-if="taskType === 'NumberSequence'">
      <label>{{ t('sequenceStep') }}</label>
      <input v-model.number="numberSequence.sequenceStep" type="number" min="1" />
    </template>

    <template v-else-if="taskType === 'Multiplication'">
      <label>{{ t('multipliers') }}</label>
      <input v-model="multipliersText" type="text" :placeholder="t('multipliersHint')" />
    </template>

    <template v-else-if="taskType === 'FractionsBasic'">
      <label>{{ t('fractions') }}</label>
      <input v-model="fractionsText" type="text" :placeholder="t('fractionsHint')" />
    </template>

    <StaticExercisesSection
      :chance="model.staticExerciseChancePercent"
      :exercises="model.staticExercises"
      @update:chance="model.staticExerciseChancePercent = $event"
      @update:exercises="model.staticExercises = $event"
    />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type {
  AdditionConfig,
  AnyTaskConfig,
  FractionsBasicConfig,
  MultiplicationConfig,
  NumberSequenceConfig,
  SubtractionConfig,
  TaskType
} from '../../types/taskConfig'
import { useI18n } from '../../composables/useI18n'
import StaticExercisesSection from './StaticExercisesSection.vue'

const props = defineProps<{
  taskType: TaskType
  modelValue: AnyTaskConfig
}>()

const emit = defineEmits<{
  'update:modelValue': [value: AnyTaskConfig]
}>()

const { t } = useI18n()

const model = computed({
  get: () => props.modelValue,
  set: (v) => emit('update:modelValue', v)
})

const addition = computed(() => model.value as AdditionConfig)
const subtraction = computed(() => model.value as SubtractionConfig)
const numberSequence = computed(() => model.value as NumberSequenceConfig)
const multiplication = computed(() => model.value as MultiplicationConfig)
const fractionsBasic = computed(() => model.value as FractionsBasicConfig)

const multipliersText = computed({
  get: () => multiplication.value.multipliers.join(', '),
  set: (text: string) => {
    const multipliers = text
      .split(/[,;\s]+/)
      .map((s) => Number(s.trim()))
      .filter((n) => !Number.isNaN(n) && n > 0)
    multiplication.value.multipliers = multipliers.length ? multipliers : [2, 5, 10]
  }
})

const fractionsText = computed({
  get: () => fractionsBasic.value.fractions.join(', '),
  set: (text: string) => {
    const fractions = text
      .split(/[,;]+/)
      .map((s) => s.trim())
      .filter(Boolean)
    fractionsBasic.value.fractions = fractions.length ? fractions : ['1/2', '1/3', '1/4']
  }
})
</script>

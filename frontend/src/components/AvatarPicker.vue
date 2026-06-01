<template>
  <div class="avatar-picker">
    <button
      v-for="a in AVATARS"
      :key="a.key"
      type="button"
      class="avatar-option"
      :class="{ selected: modelValue === a.key }"
      :title="locale === 'en' ? a.labelEn : a.labelCs"
      @click="$emit('update:modelValue', a.key)"
    >
      <span class="avatar-emoji">{{ a.emoji }}</span>
    </button>
  </div>
</template>

<script setup lang="ts">
import { AVATARS } from '../constants/avatars'
import { useI18n } from '../composables/useI18n'

defineProps<{ modelValue: string }>()
defineEmits<{ 'update:modelValue': [value: string] }>()
const { locale } = useI18n()
</script>

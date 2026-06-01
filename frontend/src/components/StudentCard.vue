<template>
  <div class="student-card card animate-pop">
    <div class="student-avatar">{{ avatarEmoji(student.avatarKey) }}</div>
    <div>
      <strong>{{ student.name }}</strong>
      <p class="muted">{{ t('grade') }}: {{ student.grade }}</p>
    </div>
    <slot />
  </div>
</template>

<script setup lang="ts">
import { avatarEmoji } from '../constants/avatars'
import { useI18n } from '../composables/useI18n'

defineProps<{ student: { name: string; grade: number; avatarKey?: string | null } }>()
const { t } = useI18n()
</script>

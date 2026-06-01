<template>
  <div v-if="show" class="feedback-burst" :class="type">
    <span v-if="type === 'ok'">⭐</span>
    <span v-else>💪</span>
  </div>
</template>

<script setup lang="ts">
defineProps<{ show: boolean; type: 'ok' | 'bad' }>()
</script>

<template>
  <span class="badge animate-bounce-in" :title="description">{{ displayName }}</span>
</template>

<script setup lang="ts">
defineProps<{ displayName: string; description?: string }>()
</script>

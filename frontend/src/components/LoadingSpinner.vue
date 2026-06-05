<template>
  <div class="spinner" role="status" aria-label="Loading" />
</template>

<style scoped>
.spinner {
  width: 44px;
  height: 44px;
  border-radius: 50%;
  border: 4px solid rgba(2, 132, 199, 0.25);
  border-top-color: var(--primary-dark);
  animation: spin 0.85s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}
</style>


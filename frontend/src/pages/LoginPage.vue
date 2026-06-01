<template>
  <div class="card">
    <h2>{{ t('login') }}</h2>
    <form @submit.prevent="submit">
      <label>{{ t('email') }}</label>
      <input v-model="email" type="email" required />
      <label>{{ t('password') }}</label>
      <input v-model="password" type="password" required />
      <p v-if="error" class="feedback-warn">{{ error }}</p>
      <button class="btn btn-primary btn-block" type="submit">{{ t('login') }}</button>
    </form>
    <RouterLink to="/register">{{ t('register') }}</RouterLink>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/authStore'
import { useI18n } from '../composables/useI18n'

const auth = useAuthStore()
const router = useRouter()
const { t } = useI18n()
const email = ref('')
const password = ref('')
const error = ref('')

async function submit() {
  try {
    await auth.login(email.value, password.value)
    router.push('/dashboard')
  } catch {
    error.value = 'Invalid credentials'
  }
}
</script>

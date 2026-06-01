<template>
  <div class="card">
    <h2>{{ t('register') }}</h2>
    <form @submit.prevent="submit">
      <label>{{ t('displayName') }}</label>
      <input v-model="displayName" required />
      <label>{{ t('email') }}</label>
      <input v-model="email" type="email" required />
      <label>{{ t('password') }}</label>
      <input v-model="password" type="password" required minlength="6" />
      <p v-if="error" class="feedback-warn">{{ error }}</p>
      <button class="btn btn-primary btn-block" type="submit">{{ t('register') }}</button>
    </form>
    <RouterLink to="/login">{{ t('login') }}</RouterLink>
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
const displayName = ref('')
const email = ref('')
const password = ref('')
const error = ref('')

async function submit() {
  try {
    await auth.register(email.value, password.value, displayName.value)
    router.push('/dashboard')
  } catch {
    error.value = 'Registration failed'
  }
}
</script>

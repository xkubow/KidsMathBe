<template>
  <div class="card animate-pop">
    <div class="student-avatar" style="text-align:center;font-size:4rem">{{ avatarEmoji(avatarKey) }}</div>
    <h2>{{ t('resetPin') }}</h2>
    <p v-if="studentName"><strong>{{ studentName }}</strong></p>

    <form @submit.prevent="resetPin">
      <label>{{ t('newPin') }}</label>
      <input
        v-model="pin"
        type="password"
        maxlength="6"
        minlength="4"
        inputmode="numeric"
        class="pin-input"
        required
      />

      <label>{{ t('confirmPin') }}</label>
      <input
        v-model="confirmPin"
        type="password"
        maxlength="6"
        minlength="4"
        inputmode="numeric"
        class="pin-input"
        required
      />

      <p v-if="error" class="feedback-warn card-shake">{{ error }}</p>
      <button class="btn btn-primary btn-block" type="submit" :disabled="busy">
        {{ t('resetPin') }}
      </button>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '../api/client'
import { useI18n } from '../composables/useI18n'
import { avatarEmoji } from '../constants/avatars'

const route = useRoute()
const router = useRouter()
const { t } = useI18n()

const pin = ref('')
const confirmPin = ref('')
const error = ref('')
const busy = ref(false)

const studentId = route.params.studentId as string
const studentName = (route.query.name as string) ?? ''
const avatarKey = (route.query.avatar as string) ?? 'fox'
const fromDashboard = route.query.from === 'dashboard'

async function resetPin() {
  error.value = ''
  if (pin.value !== confirmPin.value) {
    error.value = t('pinMismatch')
    return
  }

  busy.value = true
  try {
    await api.post(`/api/students/${studentId}/reset-pin`, { pin: pin.value })
    if (fromDashboard) {
      router.push({ name: 'dashboard' })
      return
    }
    router.push({
      name: 'student-select',
      query: { studentId, name: studentName, avatar: avatarKey, pinReset: '1' },
    })
  } catch {
    error.value = t('saveFailed')
  } finally {
    busy.value = false
  }
}
</script>

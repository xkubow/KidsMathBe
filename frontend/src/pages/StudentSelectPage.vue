<template>
  <div class="card animate-pop">
    <div class="student-avatar" style="text-align:center;font-size:4rem">{{ avatarEmoji(avatarKey) }}</div>
    <h2>{{ t('selectChild') }}</h2>
    <p v-if="studentName"><strong>{{ studentName }}</strong></p>
    <label>{{ t('enterPin') }}</label>
    <input v-model="pin" type="password" maxlength="6" inputmode="numeric" class="pin-input" />
    <p v-if="success" class="feedback-ok">{{ t('pinResetSuccess') }}</p>
    <p v-if="error" class="feedback-warn card-shake">{{ error }}</p>
    <button class="btn btn-primary btn-block" @click="verify">{{ t('startPractice') }}</button>
    <button class="btn btn-ghost btn-block" type="button" @click="forgotPin">{{ t('forgotPin') }}</button>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '../api/client'
import { useAuthStore } from '../stores/authStore'
import { useI18n } from '../composables/useI18n'
import { avatarEmoji } from '../constants/avatars'

const route = useRoute()
const router = useRouter()
const auth = useAuthStore()
const { t } = useI18n()
const pin = ref('')
const error = ref('')
const success = ref(route.query.pinReset === '1')
const studentId = (route.query.studentId as string) ?? auth.activeStudentId ?? ''
const studentName = (route.query.name as string) ?? auth.activeStudentName ?? ''
const avatarKey = (route.query.avatar as string) ?? 'fox'

function forgotPin() {
  if (!studentId) {
    error.value = 'Select a child first'
    return
  }
  router.push({
    name: 'parent-reset-pin',
    params: { studentId },
    query: { name: studentName, avatar: avatarKey, from: 'pin' },
  })
}

async function verify() {
  if (!studentId) {
    error.value = 'Select a child first'
    return
  }
  try {
    const { data } = await api.post(`/api/students/${studentId}/verify-pin`, { pin: pin.value })
    auth.setStudentSession(data.token, studentId, studentName || data.displayName)
    localStorage.setItem('activeStudentAvatar', avatarKey)
    router.push('/student/home')
  } catch {
    error.value = 'Wrong PIN'
  }
}
</script>

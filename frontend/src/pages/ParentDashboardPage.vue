<template>
  <div>
    <div class="card animate-pop">
      <h2>{{ t('parentDashboard') }}</h2>
      <p>{{ auth.displayName }}</p>
      <button class="btn btn-primary" @click="router.push('/student/select')">{{ t('selectChild') }}</button>
    </div>

    <div class="card animate-pop">
      <h3>{{ t('addChild') }}</h3>
      <form @submit.prevent="addChild">
        <label>{{ t('childName') }}</label>
        <input v-model="form.name" required />
        <label>{{ t('grade') }}</label>
        <select v-model.number="form.grade">
          <option :value="1">1</option>
          <option :value="2">2</option>
          <option :value="3">3</option>
        </select>
        <label>{{ t('avatar') }}</label>
        <AvatarPicker v-model="form.avatarKey" />
        <label>{{ t('pin') }}</label>
        <input v-model="form.pin" type="password" maxlength="6" minlength="4" required />
        <button class="btn btn-primary btn-block" type="submit">{{ t('addChild') }}</button>
      </form>
    </div>

    <div class="card">
      <h3>{{ t('selectChild') }}</h3>
      <p v-if="!students.length">{{ t('noStudents') }}</p>
      <StudentCard v-for="s in students" :key="s.id" :student="s">
        <button class="btn btn-primary" @click="goToPin(s)">{{ t('selectChild') }}</button>
        <button class="btn btn-ghost" @click="viewHistory(s)">{{ t('practiceHistory') }}</button>
      </StudentCard>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '../api/client'
import { useAuthStore } from '../stores/authStore'
import { useI18n } from '../composables/useI18n'
import AvatarPicker from '../components/AvatarPicker.vue'
import StudentCard from '../components/StudentCard.vue'

interface Student { id: string; name: string; grade: number; avatarKey?: string | null }

const auth = useAuthStore()
const router = useRouter()
const { t } = useI18n()
const students = ref<Student[]>([])
const form = reactive({ name: '', grade: 1, pin: '', avatarKey: 'fox' })

onMounted(load)

async function load() {
  const { data } = await api.get('/api/students')
  students.value = data
}

async function addChild() {
  await api.post('/api/students', form)
  form.name = ''
  form.pin = ''
  form.avatarKey = 'fox'
  await load()
}

function goToPin(s: Student) {
  router.push({ name: 'student-select', query: { studentId: s.id, name: s.name, avatar: s.avatarKey ?? 'fox' } })
}

function viewHistory(s: Student) {
  router.push({ name: 'parent-session-history', params: { studentId: s.id }, query: { name: s.name } })
}
</script>

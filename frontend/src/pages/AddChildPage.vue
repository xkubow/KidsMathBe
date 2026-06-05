<template>
  <div class="card animate-pop">
    <h2>{{ t('addChild') }}</h2>

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
</template>

<script setup lang="ts">
import { reactive } from 'vue'
import { useRouter } from 'vue-router'
import api from '../api/client'
import { useI18n } from '../composables/useI18n'
import AvatarPicker from '../components/AvatarPicker.vue'

const router = useRouter()
const { t } = useI18n()

const form = reactive({ name: '', grade: 1, pin: '', avatarKey: 'fox' })

async function addChild() {
  await api.post('/api/students', form)
  router.push({ name: 'dashboard' })
}
</script>

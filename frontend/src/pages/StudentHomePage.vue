<template>
  <div>
    <div class="card animate-pop student-home-card">
      <div class="student-info">
        <span class="student-avatar">{{ avatarEmoji(avatarKey) }}</span>
        <div class="student-name">
          <h2>👋 {{ auth.activeStudentName }}</h2>
        </div>
      </div>
      <div class="actions actions-wrap">
        <button
          class="btn btn-primary btn-block"
          @click="router.push('/student/tasks')"
        >
          {{ t("chooseTopic") }}
        </button>
        <button
          class="btn btn-ghost btn-block"
          @click="router.push('/student/achievements')"
        >
          {{ t("achievements") }}
        </button>
        <button class="btn btn-ghost btn-block" @click="loadSummary">
          {{ t("summary") }}
        </button>
      </div>
    </div>

    <div v-if="summary" class="card animate-pop">
      <h3>{{ t("summary") }}</h3>
      <p>{{ summary.totalCorrect }} / {{ summary.totalAnswered }} ✓</p>
      <div class="progress-bar"><span :style="{ width: percent + '%' }" /></div>
      <div v-if="summary.achievements?.length" class="badges-wrap">
        <AchievementBadge
          v-for="a in summary.achievements"
          :key="a.code"
          :display-name="a.displayName"
        />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
import { useRouter } from "vue-router";
import api from "../api/client";
import { useAuthStore } from "../stores/authStore";
import { useI18n } from "../composables/useI18n";
import { avatarEmoji } from "../constants/avatars";
import AchievementBadge from "../components/AchievementBadge.vue";

const auth = useAuthStore();
const router = useRouter();
const { t } = useI18n();
const avatarKey = localStorage.getItem("activeStudentAvatar") ?? "fox";
const summary = ref<{
  totalCorrect: number;
  totalAnswered: number;
  achievements: { code: string; displayName: string }[];
} | null>(null);
const percent = computed(() => {
  if (!summary.value?.totalAnswered) return 0;
  return Math.round(
    (summary.value.totalCorrect / summary.value.totalAnswered) * 100,
  );
});

async function loadSummary() {
  const { data } = await api.get(
    `/api/students/${auth.activeStudentId}/summary`,
  );
  summary.value = data;
}
</script>

<style scoped>
.badges-wrap {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-top: 0.75rem;
}
.actions-wrap {
  display: flex;
  flex-direction: column;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-top: 0.75rem;
}
.student-info {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}
.student-avatar {
  font-size: 2.5rem;
}
.student-name {
  flex-basis: 100%;
  font-size: 1.25rem;
  font-weight: bold;
}
.student-home-card {
  text-align: center;
}
</style>

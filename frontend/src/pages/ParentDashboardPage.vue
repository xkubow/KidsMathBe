<template>
  <div>
    <div class="card animate-pop">
      <h3>{{ t("parentDashboard") }}</h3>
      <h1>{{ auth.displayName }}</h1>

      <button
        class="btn btn-primary btn-block"
        @click="router.push({ name: 'parent-add-child' })"
      >
        {{ t("addChild") }}
      </button>
      <h3>{{ t("selectChild") }}</h3>
      <p v-if="!students.length">{{ t("noStudents") }}</p>
      <StudentCard v-for="s in students" :key="s.id" :student="s">
        <button class="btn btn-primary" @click="goToPin(s)">
          {{ t("selectChild") }}
        </button>
        <button class="btn btn-ghost" @click="viewHistory(s)">
          {{ t("practiceHistory") }}
        </button>
      </StudentCard>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from "vue";
import { useRouter } from "vue-router";
import api from "../api/client";
import { useAuthStore } from "../stores/authStore";
import { useI18n } from "../composables/useI18n";
import StudentCard from "../components/StudentCard.vue";

interface Student {
  id: string;
  name: string;
  grade: number;
  avatarKey?: string | null;
}

const auth = useAuthStore();
const router = useRouter();
const { t } = useI18n();
const students = ref<Student[]>([]);

onMounted(load);

async function load() {
  const { data } = await api.get("/api/students");
  students.value = data;
}

function goToPin(s: Student) {
  router.push({
    name: "student-select",
    query: { studentId: s.id, name: s.name, avatar: s.avatarKey ?? "fox" },
  });
}

function viewHistory(s: Student) {
  router.push({
    name: "parent-session-history",
    params: { studentId: s.id },
    query: { name: s.name },
  });
}
</script>

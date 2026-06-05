<template>
  <div
    v-if="current"
    class="rounded-2xl border-4 bg-gradient-to-b p-6 shadow-card-game transition-all"
    :class="[presentation.cardBorderClass, presentation.cardAccentClass, cardAnimClass]"
  >
    <div
      v-if="theme !== 'Default'"
      class="mb-4 flex justify-center"
    >
      <span
        class="inline-flex items-center gap-1.5 rounded-full px-4 py-1.5 text-sm font-black"
        :class="presentation.badgeClass"
      >
        {{ themeBadgeText }}
      </span>
    </div>

    <div class="mb-4 flex items-center justify-between gap-3">
      <p class="text-sm font-bold uppercase tracking-wide text-slate-500">
        Úloha {{ index + 1 }} / {{ attempts.length }}
      </p>
      <div
        v-if="!isResolved"
        class="flex items-center gap-1 rounded-full bg-rose-100 px-3 py-1 text-xs font-black text-rose-600"
        aria-label="Zbývající pokusy"
      >
        <span v-for="n in livesRemaining" :key="n" class="text-sm">❤️</span>
        <span v-if="livesRemaining === 0" class="text-sm">💔</span>
      </div>
    </div>

    <div class="mb-6 h-6 rounded-full bg-slate-100 p-1 shadow-inner">
      <div
        class="h-full rounded-full bg-gradient-to-r transition-all duration-300"
        :class="presentation.progressGradientClass"
        :style="{ width: progress + '%' }"
      />
    </div>

    <p class="mb-6 text-center text-4xl font-black leading-tight text-slate-800">
      <template v-for="(segment, i) in questionSegments" :key="i">
        <span
          v-if="segment.isOperator"
          class="mx-1 inline-block rounded-xl px-2 py-0.5"
          :class="presentation.operatorPillClass"
        >{{ segment.text }}</span>
        <span v-else>{{ segment.text }}</span>
      </template>
    </p>

    <FeedbackBurst :show="feedback === 'ok'" type="ok" />

    <input
      ref="answerInput"
      v-model="answer"
      type="text"
      inputmode="numeric"
      :disabled="isResolved"
      class="mb-4 w-full rounded-2xl border-4 bg-white p-5 text-center text-5xl font-black text-slate-800 outline-none transition-all disabled:bg-slate-50"
      :class="[presentation.cardBorderClass, { 'animate-shake': inputShake }]"
      @keyup.enter="submit"
    />

    <p
      v-if="feedback === 'ok'"
      class="mb-3 text-center text-xl font-black text-green-600"
    >
      {{ t("correct") }} ⭐
    </p>
    <p
      v-else-if="feedback === 'bad' && !isResolved"
      class="mb-3 text-center text-lg font-bold text-amber-600"
    >
      {{ t("tryAgain") }}
    </p>
    <p
      v-else-if="feedback === 'bad' && isResolved"
      class="mb-3 text-center text-lg font-bold text-amber-600"
    >
      {{ t("attemptsExhausted") }}: {{ revealedCorrect }}
    </p>

    <button
      class="mb-2 w-full rounded-2xl py-4 text-xl font-black text-white transition-all duration-75 select-none bg-[var(--primary)] border-b-8 border-[var(--primary)] filter brightness-100 [border-bottom-color:rgb(0_0_0_/_0.25)] active:translate-y-[6px] active:border-b-2 disabled:opacity-60"
      :disabled="submitting"
      @click="submit"
    >
      {{ isResolved ? t("next") : "OK" }}
    </button>

    <button
      class="w-full rounded-2xl border-b-4 py-3 text-lg font-bold transition-all active:translate-y-1 active:border-b-0"
      :class="
        allDone
          ? 'border-green-700 bg-green-500 text-white shadow-game-btn'
          : 'border-transparent bg-transparent text-indigo-600'
      "
      @click="finish"
    >
      {{ t("finish") }}
    </button>
  </div>
</template>

<script setup lang="ts">
import confetti from "canvas-confetti";
import { computed, nextTick, onMounted, ref, watch } from "vue";
import { useRoute, useRouter } from "vue-router";
import api from "../api/client";
import { useI18n } from "../composables/useI18n";
import { useTemplateTheme } from "../composables/useTemplateTheme";
import FeedbackBurst from "../components/FeedbackBurst.vue";
import { parseTemplateTheme, type TemplateTheme } from "../types/templateTheme";
import { parseQuestionSegments } from "../utils/questionText";

interface Submission {
  attemptNumber: number;
  answer: string;
  isCorrect: boolean;
  submittedAtUtc: string;
}

interface Attempt {
  id: string;
  questionText: string;
  theme?: TemplateTheme;
  isCorrect: boolean | null;
  isResolved: boolean;
  studentAnswer: string | null;
  attemptsUsed: number;
  maxAttempts: number;
  submissions: Submission[];
}

const route = useRoute();
const router = useRouter();
const { t } = useI18n();
const sessionId = route.params.sessionId as string;
const attempts = ref<Attempt[]>([]);
const sessionTheme = ref<TemplateTheme>("Default");
const maxAttempts = ref(10);
const index = ref(0);
const answer = ref("");
const feedback = ref<"ok" | "bad" | null>(null);
const cardAnimClass = ref("");
const inputShake = ref(false);
const revealedCorrect = ref("");
const answerInput = ref<HTMLInputElement | null>(null);
const submitting = ref(false);

const current = computed(() => attempts.value[index.value]);
const activeThemeSource = computed(
  () => current.value?.theme ?? sessionTheme.value,
);
const { theme, presentation, themeBadgeText } = useTemplateTheme(activeThemeSource);

const isResolved = computed(() => current.value?.isResolved ?? false);
const attemptsUsed = computed(() => current.value?.attemptsUsed ?? 0);
const livesRemaining = computed(() =>
  Math.max(0, maxAttempts.value - attemptsUsed.value),
);
const allDone = computed(() => attempts.value.every((a) => a.isResolved));
const progress = computed(
  () => ((index.value + 1) / attempts.value.length) * 100,
);
const questionSegments = computed(() =>
  parseQuestionSegments(current.value?.questionText ?? ""),
);

onMounted(load);

watch(index, () => focusAnswerInput());

async function load() {
  const { data } = await api.get(`/api/exercise-sessions/${sessionId}`);
  maxAttempts.value = data.maxAttemptsPerQuestion ?? 10;
  sessionTheme.value = parseTemplateTheme(data.theme);
  attempts.value = data.attempts.map((attempt: Attempt) => ({
    ...attempt,
    theme: parseTemplateTheme(attempt.theme),
  }));
  const firstOpen = attempts.value.findIndex((a) => !a.isResolved);
  index.value = firstOpen >= 0 ? firstOpen : 0;
  await focusAnswerInput();
}

function focusAnswerInput() {
  return nextTick(() => {
    if (!isResolved.value) {
      answerInput.value?.focus();
    }
  });
}

function goNext() {
  if (index.value < attempts.value.length - 1) {
    index.value++;
    answer.value = "";
    feedback.value = null;
    cardAnimClass.value = "";
    inputShake.value = false;
    revealedCorrect.value = "";
  }
}

function fireConfetti() {
  confetti({
    particleCount: 80,
    spread: 70,
    origin: { y: 0.65 },
    colors: presentation.value.confettiColors,
  });
}

function triggerInputShake() {
  inputShake.value = false;
  void nextTick(() => {
    inputShake.value = true;
    setTimeout(() => {
      inputShake.value = false;
    }, 400);
  });
}

async function submit() {
  if (!current.value || submitting.value) return;
  if (current.value.isResolved) {
    goNext();
    return;
  }

  submitting.value = true;
  try {
    const { data } = await api.post(
      `/api/exercise-sessions/${sessionId}/answer`,
      {
        attemptId: current.value.id,
        answer: answer.value,
      },
    );
    current.value.attemptsUsed = data.attemptsUsed;
    current.value.isResolved = data.questionResolved;
    if (data.questionResolved) {
      current.value.isCorrect = data.finalOutcome;
      current.value.studentAnswer = data.studentAnswer;
    }
    feedback.value = data.isCorrect ? "ok" : "bad";

    if (data.isCorrect) {
      cardAnimClass.value = "animate-bounce-in";
      fireConfetti();
    } else {
      cardAnimClass.value = "";
      triggerInputShake();
    }

    if (
      data.questionResolved &&
      data.isCorrect &&
      index.value < attempts.value.length - 1
    ) {
      setTimeout(goNext, 900);
    } else if (data.questionResolved && !data.isCorrect) {
      revealedCorrect.value = data.correctAnswer ?? "";
      answer.value = "";
    } else if (!data.questionResolved) {
      answer.value = "";
      await focusAnswerInput();
    }
  } finally {
    submitting.value = false;
  }
}

async function finish() {
  await api.post(`/api/exercise-sessions/${sessionId}/finish`);
  router.push({ name: "session-result", params: { sessionId } });
}
</script>

<style scoped>
@keyframes shake-input {
  0%,
  100% {
    transform: translateX(0);
  }
  20% {
    transform: translateX(-8px);
  }
  40% {
    transform: translateX(8px);
  }
  60% {
    transform: translateX(-6px);
  }
  80% {
    transform: translateX(6px);
  }
}

.animate-shake {
  animation: shake-input 0.4s ease-out;
  border-color: #fbbf24 !important;
}
</style>

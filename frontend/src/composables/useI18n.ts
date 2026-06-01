import { ref } from 'vue'

const messages = {
  cs: {
    logout: 'Odhlásit',
    login: 'Přihlásit',
    register: 'Registrovat',
    email: 'E-mail',
    password: 'Heslo',
    displayName: 'Jméno rodiče',
    parentDashboard: 'Přehled rodiče',
    addChild: 'Přidat dítě',
    childName: 'Jméno dítěte',
    grade: 'Třída',
    pin: 'PIN',
    selectChild: 'Vyber profil',
    enterPin: 'Zadej PIN',
    startPractice: 'Začít cvičení',
    chooseTopic: 'Vyber úlohu',
    correct: 'Správně!',
    tryAgain: 'Zkus to znovu',
    attemptsExhausted: 'Správná odpověď',
    practiceHistory: 'Historie cvičení',
    viewDetails: 'Detail',
    sessionDetail: 'Detail cvičení',
    finalAnswer: 'Výsledná odpověď',
    correctWas: 'správně',
    noSessions: 'Zatím žádná cvičení.',
    noAttempts: 'Bez odpovědi',
    finish: 'Dokončit',
    next: 'Další',
    achievements: 'Odznaky',
    progress: 'Pokrok',
    summary: 'Shrnutí',
    sessionResult: 'Výsledek',
    noStudents: 'Zatím nemáš žádné děti.',
    loading: 'Načítám…',
    avatar: 'Avatar',
    printWorksheet: 'Tisk pracovního listu',
    worksheetTitle: 'Pracovní list – Matika',
    backHome: 'Domů'
  },
  en: {
    logout: 'Log out',
    login: 'Log in',
    register: 'Register',
    email: 'Email',
    password: 'Password',
    displayName: 'Parent name',
    parentDashboard: 'Parent dashboard',
    addChild: 'Add child',
    childName: 'Child name',
    grade: 'Grade',
    pin: 'PIN',
    selectChild: 'Choose profile',
    enterPin: 'Enter PIN',
    startPractice: 'Start practice',
    chooseTopic: 'Choose task',
    correct: 'Correct!',
    tryAgain: 'Try again',
    attemptsExhausted: 'Correct answer',
    practiceHistory: 'Practice history',
    viewDetails: 'Details',
    sessionDetail: 'Session detail',
    finalAnswer: 'Final answer',
    correctWas: 'correct was',
    noSessions: 'No practice sessions yet.',
    noAttempts: 'No answers',
    finish: 'Finish',
    next: 'Next',
    achievements: 'Achievements',
    progress: 'Progress',
    summary: 'Summary',
    sessionResult: 'Result',
    noStudents: 'No children yet.',
    loading: 'Loading…',
    avatar: 'Avatar',
    printWorksheet: 'Print worksheet',
    worksheetTitle: 'Math worksheet',
    backHome: 'Home'
  }
} as const

type Locale = keyof typeof messages

const locale = ref<Locale>('cs')

export function useI18n() {
  function t(key: keyof (typeof messages)['cs']): string {
    return messages[locale.value][key] ?? key
  }

  function setLocale(lang: string) {
    locale.value = lang.startsWith('en') ? 'en' : 'cs'
    localStorage.setItem('lang', locale.value)
  }

  return { locale, t, setLocale }
}

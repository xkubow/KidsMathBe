import type { TemplateTheme } from '../types/templateTheme'

export interface ThemePresentation {
  emoji: string
  labelKey: 'themeDefault' | 'themeSpace' | 'themePirates' | 'themeAnimals'
  cardBorderClass: string
  cardAccentClass: string
  progressGradientClass: string
  badgeClass: string
  operatorPillClass: string
  confettiColors: string[]
}

export const THEME_PRESENTATION: Record<TemplateTheme, ThemePresentation> = {
  Default: {
    emoji: '✨',
    labelKey: 'themeDefault',
    cardBorderClass: 'border-slate-100',
    cardAccentClass: 'from-slate-50 to-white',
    progressGradientClass: 'from-blue-400 to-indigo-500',
    badgeClass: 'bg-slate-100 text-slate-600',
    operatorPillClass: 'bg-indigo-100 text-indigo-600',
    confettiColors: ['#4ade80', '#60a5fa', '#fbbf24', '#f472b6'],
  },
  Space: {
    emoji: '🚀',
    labelKey: 'themeSpace',
    cardBorderClass: 'border-indigo-200',
    cardAccentClass: 'from-indigo-50 to-violet-50',
    progressGradientClass: 'from-indigo-400 to-violet-600',
    badgeClass: 'bg-indigo-100 text-indigo-700',
    operatorPillClass: 'bg-violet-100 text-violet-700',
    confettiColors: ['#818cf8', '#a78bfa', '#38bdf8', '#c084fc'],
  },
  Pirates: {
    emoji: '🏴‍☠️',
    labelKey: 'themePirates',
    cardBorderClass: 'border-amber-200',
    cardAccentClass: 'from-amber-50 to-orange-50',
    progressGradientClass: 'from-amber-400 to-orange-500',
    badgeClass: 'bg-amber-100 text-amber-800',
    operatorPillClass: 'bg-orange-100 text-orange-700',
    confettiColors: ['#fbbf24', '#f97316', '#fcd34d', '#fb923c'],
  },
  Animals: {
    emoji: '🐾',
    labelKey: 'themeAnimals',
    cardBorderClass: 'border-emerald-200',
    cardAccentClass: 'from-emerald-50 to-lime-50',
    progressGradientClass: 'from-emerald-400 to-lime-500',
    badgeClass: 'bg-emerald-100 text-emerald-700',
    operatorPillClass: 'bg-lime-100 text-lime-800',
    confettiColors: ['#4ade80', '#a3e635', '#34d399', '#86efac'],
  },
}

export function getThemePresentation(theme: TemplateTheme): ThemePresentation {
  return THEME_PRESENTATION[theme] ?? THEME_PRESENTATION.Default
}

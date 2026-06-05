export const TEMPLATE_THEMES = ['Default', 'Space', 'Pirates', 'Animals'] as const

export type TemplateTheme = (typeof TEMPLATE_THEMES)[number]

export function parseTemplateTheme(value: unknown): TemplateTheme {
  if (typeof value === 'string') {
    const match = TEMPLATE_THEMES.find(
      theme => theme.toLowerCase() === value.toLowerCase(),
    )
    if (match) return match
  }

  return 'Default'
}

const practiceThemeKey = (studentId: string) => `practiceTheme:${studentId}`

export function loadPracticeTheme(studentId: string | null | undefined): TemplateTheme {
  if (!studentId) return 'Default'
  return parseTemplateTheme(localStorage.getItem(practiceThemeKey(studentId)))
}

export function savePracticeTheme(studentId: string | null | undefined, theme: TemplateTheme) {
  if (!studentId) return
  localStorage.setItem(practiceThemeKey(studentId), theme)
}

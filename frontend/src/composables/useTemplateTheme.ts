import { computed, type MaybeRefOrGetter, toValue } from 'vue'
import { getThemePresentation } from '../constants/templateThemePresentation'
import { parseTemplateTheme, type TemplateTheme } from '../types/templateTheme'
import { useI18n } from './useI18n'

export function useTemplateTheme(themeSource: MaybeRefOrGetter<TemplateTheme | string | undefined>) {
  const { t } = useI18n()

  const theme = computed(() => parseTemplateTheme(toValue(themeSource)))

  const presentation = computed(() => getThemePresentation(theme.value))

  const themeLabel = computed(() => t(presentation.value.labelKey))

  const themeBadgeText = computed(
    () => `${presentation.value.emoji} ${themeLabel.value}`,
  )

  return {
    theme,
    presentation,
    themeLabel,
    themeBadgeText,
  }
}

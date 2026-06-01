export const AVATARS = [
  { key: 'fox', emoji: '🦊', labelCs: 'Liška', labelEn: 'Fox' },
  { key: 'cat', emoji: '🐱', labelCs: 'Kočka', labelEn: 'Cat' },
  { key: 'bear', emoji: '🐻', labelCs: 'Medvěd', labelEn: 'Bear' },
  { key: 'rabbit', emoji: '🐰', labelCs: 'Králík', labelEn: 'Rabbit' },
  { key: 'owl', emoji: '🦉', labelCs: 'Sova', labelEn: 'Owl' },
  { key: 'frog', emoji: '🐸', labelCs: 'Žába', labelEn: 'Frog' }
] as const

export function avatarEmoji(key?: string | null) {
  return AVATARS.find((a) => a.key === key)?.emoji ?? '🧒'
}

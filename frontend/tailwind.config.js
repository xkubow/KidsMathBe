/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{vue,js,ts,jsx,tsx}'],
  theme: {
    extend: {
      fontFamily: {
        sans: ['Nunito', 'sans-serif'],
      },
      boxShadow: {
        'game-btn': '0 8px 0 0 rgba(0,0,0,0.15)',
        'card-game': '0 12px 0 0 rgba(0,0,0,0.05)',
      },
    },
  },
  plugins: [],
}

/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./**/*.{razor,html,cshtml}",
    "./Pages/**/*.{razor,html}",
    "./Components/**/*.{razor,html}",
    "./Layout/**/*.{razor,html}",
    "./wwwroot/index.html"
  ],
  theme: {
    extend: {
      colors: {
        navy: {
          50: '#F0F4F9',
          100: '#D9E2EC',
          200: '#BCCCDC',
          300: '#9FB6CD',
          400: '#627D98',
          500: '#334E68',
          600: '#243B53',
          700: '#1D3044',
          800: '#16325B',
          900: '#0F223D',
          950: '#0B192C',
        },
        'academic-green': {
          50: '#EBF7F0',
          100: '#D1EFE0',
          200: '#A7D8BE',
          500: '#1E7E5A',
          600: '#166534',
          700: '#14532D',
        },
        'academic-amber': {
          50: '#FEF3C7',
          100: '#FDE68A',
          500: '#D97706',
          600: '#B45309',
        },
        'academic-blue': {
          50: '#EFF6FF',
          100: '#DBEAFE',
          200: '#BFDBFE',
          500: '#2563EB',
          600: '#1D4ED8',
        }
      },
      fontFamily: {
        serif: ['Newsreader', 'Georgia', 'serif'],
        sans: ['Inter', 'system-ui', '-apple-system', 'sans-serif'],
        mono: ['"JetBrains Mono"', 'monospace'],
      }
    },
  },
  plugins: [],
}

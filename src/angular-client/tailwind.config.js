/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    fontFamily: {
      'expine': ['Expine'],
      'feature': ['FeatureMono'],
      'rg': ['RG'],
      'esqadero': ['Esqadero'],
    },
    extend: {
      colors: {
        // Светлая тема
        'light-main-accent': '#56828C', // Основной акцент
        'light-dark-accent': '#395359', // Темный акцент для заголовков
        'light-light-accent': '#A6A6A6', // Светлый акцент для границ, кнопок
        'light-bg-main': '#F2F2F2', // Основной фон
        'light-bg-light': '#FFFFFF', // Белый фон (например, карточки)
        'light-bg-card': '#E8E8E8', // Серый фон для карточек
        'light-divider': '#D9D9D9', // Разделительные линии
        'light-text-main': '#0D0D0D', // Основной текст
        'light-text-secondary': '#395359', // Второстепенный текст
        'light-text-placeholder': '#A6A6A6', // Плейсхолдеры
        'light-hover': '#4D7077', // Ховеры
        'light-disabled': '#D1D1D1', // Отключенные элементы

        // Темная тема
        'dark-main-accent': '#A6A6A6', // Основной акцент
        'dark-dark-accent': '#56828C', // Темный акцент
        'dark-light-accent': '#D1D1D1', // Светлый акцент
        'dark-bg-main': '#0D0D0D', // Основной фон
        'dark-bg-card': '#1A1A1A', // Фон карточек
        'dark-divider': '#2F2F2F', // Разделительные линии
        'dark-text-main': '#FFFFFF', // Основной текст
        'dark-text-secondary': '#A6A6A6', // Второстепенный текст
        'dark-text-placeholder': '#395359', // Плейсхолдеры
        'dark-hover': '#719198', // Ховеры
        'dark-disabled': '#3C3C3C', // Отключенные элементы

        // Статусы
        'status-success-light': '#36C399', // Успех (светлая тема)
        'status-error-light': '#D32F2F', // Ошибка (светлая тема)
        'status-warning-light': '#FFA000', // Предупреждение (светлая тема)
        'status-success-dark': '#5CE6C7', // Успех (темная тема)
        'status-error-dark': '#FF7A7F', // Ошибка (темная тема)
        'status-warning-dark': '#FFD76F', // Предупреждение (темная тема)

        // Прогресс бары и скролл
        'scroll-track-light': '#E8E8E8',
        'scroll-thumb-light': '#56828C',
        'scroll-track-dark': '#1A1A1A',
        'scroll-thumb-dark': '#A6A6A6',
        'progress-track-light': '#F2F2F2',
        'progress-bar-light': '#395359',
        'progress-track-dark': '#0D0D0D',
        'progress-bar-dark': '#719198',

        // Тени
        'shadow-card-light': 'rgba(0, 0, 0, 0.1)', // Легкая тень
        'shadow-hover-light': 'rgba(0, 0, 0, 0.2)',
        'shadow-card-dark': 'rgba(255, 255, 255, 0.1)', // Светлая тень
        'shadow-hover-dark': 'rgba(255, 255, 255, 0.2)',
      },
      keyframes: {
        scroll: {
          '0%': { width: '0' },
          '100%': { width: '100%' },
        }
      },
      animation: {
        scroll: 'scroll 20s linear infinite',
        scrollSlow: 'scroll 40s linear infinite',
      },
    },
  },
  darkMode: 'selector',
  plugins: [],
}


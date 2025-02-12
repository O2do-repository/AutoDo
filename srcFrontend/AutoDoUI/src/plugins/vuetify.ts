// Styles
import '@mdi/font/css/materialdesignicons.css'
import 'vuetify/styles'

// Composables
import { createVuetify } from 'vuetify'

// Création de Vuetify avec gestion du thème
export default createVuetify({
  theme: {
    defaultTheme: 'light', // Thème par défaut
    themes: {
      light: {
        dark: false,
        colors: {
          background: '#ffffff',
          surface: '#ffffff',
          primary: '#756de2',
          secondary: '#4343c4',
        },
      },
      dark: {
        dark: true,
        colors: {
          background: '#756de2',
          surface: '#756de2',
          primary: '#BB86FC',
          secondary: '#03DAC6',
        },
      },
    },
  },
})

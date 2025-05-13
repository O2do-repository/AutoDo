/**
 * router/index.ts
 *
 * Automatic routes for `./src/pages/*.vue`
 */

// Composables
import { createRouter, createWebHashHistory } from 'vue-router'
import { setupLayouts } from 'virtual:generated-layouts'
import { routes } from 'vue-router/auto-routes'

const router = createRouter({
  history: createWebHashHistory(import.meta.env.BASE_URL),
  routes: setupLayouts(routes),
})

// Workaround for https://github.com/vitejs/vite/issues/11804
router.onError((err, to) => {
  if (err?.message?.includes?.('Failed to fetch dynamically imported module')) {
    if (!localStorage.getItem('vuetify:dynamic-reload')) {
      console.log('Reloading page to fix dynamic import error')
      localStorage.setItem('vuetify:dynamic-reload', 'true')
      location.assign(to.fullPath)
    } else {
      console.error('Dynamic import error, reloading page did not fix it', err)
    }
  } else {
    console.error(err)
  }
})

router.isReady().then(() => {
  localStorage.removeItem('vuetify:dynamic-reload')
})
router.beforeEach(async (to, from, next) => {
  const apiKey = localStorage.getItem('apiKey')

  // Autorise l'accès libre à /login
  if (to.path === '/login') {
    return next()
  }

  // Si pas de clé → redirection
  if (!apiKey) {
    return next('/login')
  }

  // Vérifie la clé auprès de l'API
  try {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/api/validate-key`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ apiKey })
    })

    if (!response.ok) {
      localStorage.removeItem('apiKey')
      return next('/login')
    }

    const data = await response.json()

    if (data.valid !== true) {
      localStorage.removeItem('apiKey')
      return next('/login')
    }

    // Clé valide, autorise la navigation
    next()
  } catch (error) {
    console.error('Erreur de validation API :', error)
    localStorage.removeItem('apiKey')
    return next('/login')
  }
})

export default router

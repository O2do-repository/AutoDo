<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';

const route = useRoute();
const backendBaseUrl = import.meta.env.VITE_API_URL || 'https://votre-api.azurewebsites.net';
const authCheckUrl = `${backendBaseUrl}/user/me`;

// Redirection avec retour vers l'URL d'origine
function redirectToLogin() {
  const currentUrl = window.location.href;
  const redirectTo = `${backendBaseUrl}/user/redirect?return_to=${encodeURIComponent(currentUrl)}`;
  window.location.href = `${backendBaseUrl}/.auth/login/github?post_login_redirect_uri=${encodeURIComponent(redirectTo)}`;
}

// Vérifie si l'utilisateur est déjà connecté
onMounted(async () => {
  try {
    const res = await fetch(authCheckUrl, { credentials: 'include' });
    if (res.status === 401) return; // Pas connecté, attendre clic

    const user = await res.json();
    console.log("Connecté :", user.login);
    // Ici, tu peux stocker l'utilisateur dans un store (Pinia par exemple)
  } catch (err) {
    console.error("Erreur de récupération utilisateur", err);
  }

  // Si on a été redirigé avec des query params login/email
  const login = route.query.login;
  const email = route.query.email;
  if (login && email) {
    console.log("Redirigé depuis Azure avec :", login, email);
    // Stocker ou router ici
  }
});
</script>

<template>
  <v-container class="fill-height d-flex align-center justify-center">
    <v-card class="pa-8" elevation="12" rounded="2xl" max-width="400">
      <v-card-title class="text-h5 text-center mb-4">Connexion</v-card-title>
      <v-card-text class="text-center">
        <v-icon icon="mdi-github" size="56" color="primary" class="mb-4" />
        <p class="mb-6">
          Connecte-toi avec ton compte GitHub (membre O2do requis).
        </p>
        <v-btn color="primary" size="large" rounded @click="redirectToLogin">
          <v-icon start icon="mdi-login" />
          Se connecter
        </v-btn>
      </v-card-text>
    </v-card>
  </v-container>
</template>

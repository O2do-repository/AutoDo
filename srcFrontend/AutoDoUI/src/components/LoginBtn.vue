<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';

const backendBaseUrl = import.meta.env.VITE_API_URL || 'https://votre-api.azurewebsites.net';
const authCheckUrl = `${backendBaseUrl}/user/me`;

// Fonction de redirection manuelle vers Azure Auth
function redirectToLogin() {
  const redirectUrl = `${backendBaseUrl}/user/token`; // c’est maintenant la route qui redirige vers GitHub Pages avec un "token"
  window.location.href = `${backendBaseUrl}/.auth/login/github?post_login_redirect_uri=${encodeURIComponent(redirectUrl)}`;
}

// Vérifie si l'utilisateur est déjà authentifié
onMounted(async () => {
  const urlParams = new URLSearchParams(window.location.hash.split('?')[1]);
  const token = urlParams.get('token');

  if (token) {
    localStorage.setItem('easyauth_token', token);
    // Nettoie l'URL
    window.location.hash = '#/consultant/list-consultant';
  }

  const savedToken = localStorage.getItem('easyauth_token');
  if (!savedToken) return;

  try {
    const res = await fetch(authCheckUrl, {
      headers: {
        Authorization: `Bearer ${savedToken}`
      }
    });

    if (res.status === 401) return;

    const user = await res.json();
    console.log("Connecté :", user.login);
  } catch (err) {
    console.error("Erreur de récupération utilisateur", err);
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

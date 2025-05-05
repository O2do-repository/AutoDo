<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';

const backendBaseUrl = import.meta.env.VITE_API_URL || 'https://votre-api.azurewebsites.net';
const authCheckUrl = `${backendBaseUrl}/user/me`;

// Fonction de redirection manuelle vers Azure Auth
function redirectToLogin() {
  window.location.href = `${backendBaseUrl}/.auth/login/github?post_login_redirect_uri=${encodeURIComponent(window.location.href)}`;
}

// Vérifie si l'utilisateur est déjà authentifié
onMounted(async () => {
  try {
    const res = await fetch(authCheckUrl, { credentials: 'include' });
    if (res.status === 401) {
      // Laisse l'utilisateur cliquer sur "Se connecter"
      return;
    }
    const user = await res.json();
    console.log("Connecté :", user.login);
    // Ici, tu peux router ou stocker l'utilisateur dans un store global
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

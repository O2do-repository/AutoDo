<script setup lang="ts">
const FRONTEND_REDIRECT_URL = encodeURIComponent(`${window.location.origin}/connexion-ok`);
const backendBaseUrl = `${import.meta.env.VITE_API_URL}/.auth/login/github?post_login_redirect_uri=${FRONTEND_REDIRECT_URL}`;
const errorMessage = ref('');

function redirectToLogin() {
  try {
    window.location.href = backendBaseUrl;
  } catch (e) {
    errorMessage.value = "Erreur lors de la redirection.";
  }
}


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

        <v-alert v-if="errorMessage" type="error" class="mb-4" dense>
          {{ errorMessage }}
        </v-alert>

        <v-btn color="primary" size="large" rounded @click="redirectToLogin">
          <v-icon start icon="mdi-login" />
          Se connecter
        </v-btn>
      </v-card-text>
    </v-card>
  </v-container>
</template>

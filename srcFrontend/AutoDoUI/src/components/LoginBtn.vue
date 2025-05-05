<script setup lang="ts">
import { ref, onMounted } from 'vue';

// Define base URLs
const API_URL = import.meta.env.VITE_API_URL || 'https://autodo-fpe3azgvbvcxbgcx.westeurope-01.azurewebsites.net';
const FRONTEND_REDIRECT_URL = "https://o2do-repository.github.io/AutoDo/#/consultant/list-consultant";

// Construct login URL
const backendBaseUrl = `${API_URL}/.auth/login/github?post_login_redirect_uri=${encodeURIComponent(`${API_URL}/login`)}`;

const errorMessage = ref('');
const isLoading = ref(false);

function redirectToLogin() {
  try {
    isLoading.value = true;
    console.log("Redirecting to:", backendBaseUrl);
    window.location.href = backendBaseUrl;
  } catch (e) {
    console.error("Redirect error:", e);
    errorMessage.value = "Erreur lors de la redirection: " + (e instanceof Error ? e.message : "Erreur inconnue");
    isLoading.value = false;
  }
}

// Check URL parameters for error information
onMounted(() => {
  const urlParams = new URLSearchParams(window.location.search);
  const error = urlParams.get('error');
  if (error) {
    errorMessage.value = `Erreur d'authentification: ${error}`;
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
        <v-alert v-if="errorMessage" type="error" class="mb-4" dense>
          {{ errorMessage }}
        </v-alert>
        <v-btn color="primary" size="large" rounded @click="redirectToLogin" :loading="isLoading" :disabled="isLoading">
          <v-icon start icon="mdi-login" />
          Se connecter
        </v-btn>
      </v-card-text>
    </v-card>
  </v-container>
</template>
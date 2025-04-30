<script setup lang="ts">
import { onMounted, ref } from 'vue';

const frontendRedirectUri = encodeURIComponent("https://o2do-repository.github.io/AutoDo/#/consultant/list-consultant");
const backendBaseUrl = import.meta.env.VITE_API_URL;

const errorMessage = ref("");

const redirectToLogin = () => {
  const completeLoginUri = `/login/complete?redirect=${encodeURIComponent(frontendRedirectUri)}`;
  window.location.href = `${backendBaseUrl}/.auth/login/github?post_login_redirect_uri=${completeLoginUri}`;
};

onMounted(() => {
  const params = new URLSearchParams(window.location.search);
  const error = params.get("error");

  if (error === "no-user") {
    errorMessage.value = "Aucun utilisateur connecté via Azure.";
  } else if (error === "not-in-org") {
    errorMessage.value = "Vous n'êtes pas membre de l'organisation O2do sur GitHub.";
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

        <v-btn color="primary" size="large" rounded @click="redirectToLogin">
          <v-icon start icon="mdi-login" />
          Se connecter
        </v-btn>
      </v-card-text>
    </v-card>
  </v-container>
</template>

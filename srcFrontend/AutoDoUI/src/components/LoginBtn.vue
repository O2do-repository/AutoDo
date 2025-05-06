<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';

const backendBaseUrl = import.meta.env.VITE_API_URL || 'https://votre-api.azurewebsites.net';
const authCheckUrl = `${backendBaseUrl}/user/me`;

const router = useRouter();
const user = ref<{ login: string; provider: string } | null>(null);
const loading = ref(true);

// Redirection vers Azure EasyAuth GitHub
function redirectToLogin() {
  window.location.href = `${backendBaseUrl}/.auth/login/github`;
}

// Vérifie l'authentification à l'arrivée
onMounted(async () => {
  try {
    const res = await fetch(authCheckUrl, {
      credentials: 'include'
    });

    if (res.status === 401) {
      loading.value = false;
      return;
    }

    const data = await res.json();
    user.value = data;
    console.log("Connecté :", data);

    // Redirection automatique si déjà loggé
    router.push('/consultant/list-consultant');
  } catch (err) {
    console.error("Erreur lors de la récupération de l'utilisateur :", err);
  } finally {
    loading.value = false;
  }
});
</script>

<template>
  <v-container class="fill-height d-flex align-center justify-center">
    <v-card
      class="pa-8"
      elevation="12"
      rounded="2xl"
      max-width="400"
    >
      <v-card-title class="text-h5 text-center mb-4">
        Connexion
      </v-card-title>

      <v-card-text class="text-center">
        <v-icon icon="mdi-github" size="56" color="primary" class="mb-4" />

        <p class="mb-6">
          Connecte-toi avec ton compte GitHub (membre O2do requis).
        </p>

        <v-btn
          color="primary"
          size="large"
          rounded
          @click="redirectToLogin"
          :loading="loading"
        >
          <v-icon start icon="mdi-login" />
          Se connecter
        </v-btn>
      </v-card-text>
    </v-card>
  </v-container>
</template>

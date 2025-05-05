<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';

// URL de base pour le backend
const backendBaseUrl = import.meta.env.VITE_API_URL || 'https://votre-api.azurewebsites.net';
// URL d'authentification personnalisée que nous avons créée
const authUrl = `${backendBaseUrl}/auth/login`;
const errorMessage = ref('');
const isLoading = ref(false);
const route = useRoute();

// Fonction pour gérer le processus de connexion
function login() {
  isLoading.value = true;
  try {
    window.location.href = authUrl;
  } catch (e) {
    isLoading.value = false;
    errorMessage.value = "Erreur lors de la redirection vers le service d'authentification.";
    console.error(e);
  }
}

// Vérifier si nous venons d'être redirigés avec une erreur
onMounted(() => {
  // Vérifier si le paramètre d'erreur est présent dans l'URL
  const errorParam = route.query.error;
  if (errorParam) {
    errorMessage.value = typeof errorParam === 'string' 
      ? errorParam 
      : "Erreur d'authentification";
  }
  
  // Vérifier si login/email sont présents (utilisateur déjà authentifié)
  const login = route.query.login;
  const email = route.query.email;
  
  if (login) {
    // Si l'utilisateur est déjà authentifié, vous pouvez définir des variables d'état ici
    // ou faire un commit dans votre store
    console.log("Utilisateur connecté:", login);
  }
});
</script>

<template>
  <v-container class="fill-height d-flex align-center justify-center">
    <v-card class="pa-8" elevation="12" rounded="xl" max-width="450">
      <v-card-title class="text-h5 text-center mb-4">
        <span class="font-weight-bold">AutoDo</span>
      </v-card-title>
      
      <v-card-text class="text-center">
        <v-icon icon="mdi-github" size="56" color="primary" class="mb-4" />
        
        <p class="mb-6 text-body-1">
          Connectez-vous avec votre compte GitHub pour accéder à l'application.
          <br>
          <span class="text-caption text-medium-emphasis">
            (Réservé aux membres de l'organisation O2do)
          </span>
        </p>
        
        <v-alert
          v-if="errorMessage"
          type="error"
          class="mb-4"
          variant="tonal"
          border="start"
          closable
        >
          {{ errorMessage }}
        </v-alert>
        
        <v-btn
          color="primary"
          size="large"
          block
          rounded="pill"
          @click="login"
          :loading="isLoading"
          min-width="200"
        >
          <v-icon start icon="mdi-github" class="mr-2" />
          Se connecter avec GitHub
        </v-btn>
      </v-card-text>
      
      <v-card-text class="text-caption text-center pt-4">
        En vous connectant, vous acceptez les conditions d'utilisation d'AutoDo.
      </v-card-text>
    </v-card>
  </v-container>
</template>

<style scoped>
.v-card {
  transition: transform 0.3s;
}
.v-card:hover {
  transform: translateY(-5px);
}
</style>
<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';

const backendBaseUrl = import.meta.env.VITE_API_URL || 'https://votre-api.azurewebsites.net';
const router = useRouter();
const user = ref<{ login: string; provider: string; userId?: string } | null>(null);
const loading = ref(true);
const error = ref<string | null>(null);

// Redirection vers Azure EasyAuth GitHub
function redirectToLogin() {
  // Rediriger vers l'authentification GitHub d'Azure
  window.location.href = `${backendBaseUrl}/.auth/login/github?post_login_redirect_uri=${encodeURIComponent(`${backendBaseUrl}/user/token`)}`;
}

// Vérifie l'authentification à l'arrivée
onMounted(async () => {
  try {
    // Vérifier si on a déjà un token
    const storedToken = localStorage.getItem('autodo_token');
    
    if (storedToken) {
      // Si on a déjà un token, vérifier s'il est valide
      const res = await fetch(`${backendBaseUrl}/user/me`, {
        headers: {
          Authorization: `Bearer ${storedToken}`
        }
      });
      
      if (res.ok) {
        // Token valide, récupérer les infos utilisateur
        const userData = await res.json();
        user.value = userData;
        router.push('/consultant/list-consultant');
        return;
      } else {
        // Token invalide ou expiré, on le supprime
        localStorage.removeItem('autodo_token');
      }
    }
    
    // Si on est redirigé après login, on est censé avoir un token
    const urlParams = new URLSearchParams(window.location.search);
    const redirected = urlParams.get('redirected');
    
    if (redirected === 'true') {
      // Récupérer le token après redirection d'EasyAuth
      try {
        const tokenRes = await fetch(`${backendBaseUrl}/user/token`, {
          credentials: 'include' // important pour EasyAuth
        });
        
        if (!tokenRes.ok) {
          throw new Error("Impossible de récupérer le token");
        }
        
        const data = await tokenRes.json();
        localStorage.setItem('autodo_token', data.token);
        
        // Vérifier l'identité avec le nouveau token
        const meRes = await fetch(`${backendBaseUrl}/user/me`, {
          headers: {
            Authorization: `Bearer ${data.token}`
          }
        });
        
        if (meRes.ok) {
          const userData = await meRes.json();
          user.value = userData;
          router.push('/consultant/list-consultant');
        } else {
          throw new Error("Token invalide");
        }
      } catch (err) {
        console.error("Erreur d'authentification:", err);
        error.value = "Échec de l'authentification";
      }
    }
  } catch (err) {
    console.error("Erreur d'authentification:", err);
    error.value = "Une erreur s'est produite";
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
        <div v-if="error" class="text-error mb-4">
          {{ error }}
        </div>
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
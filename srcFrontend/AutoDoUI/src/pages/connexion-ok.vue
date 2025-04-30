<script setup lang="ts">
import { onMounted } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();

onMounted(async () => {
  try {
    const res = await fetch(`${import.meta.env.VITE_API_URL}/login`, {
      credentials: 'include' // Nécessaire pour envoyer le cookie d'auth Azure
    });

    if (res.ok) {
      const user = await res.json();
      console.log('Utilisateur connecté :', user);

      // Tu peux stocker l’utilisateur dans le store ici si tu utilises Pinia/Vuex

      router.push('/dashboard'); // Rediriger vers une page protégée par exemple
    }
  } catch (error) {
    console.error('Erreur pendant l’authentification :', error);
    router.push('/erreur-auth');
  }
});
</script>

<template>
  <div class="text-center mt-10">
    <h2>Connexion en cours...</h2>
  </div>
</template>

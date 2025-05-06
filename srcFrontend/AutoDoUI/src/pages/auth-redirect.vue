<script setup lang="ts">
import { onMounted } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();

onMounted(() => {
  const urlParams = new URLSearchParams(window.location.search);
  const token = urlParams.get('token');

  if (token) {
    localStorage.setItem('autodo_token', token);

    // Nettoyage de l'URL et redirection
    router.replace('/consultant/list-consultant');
  } else {
    // Pas de token → rediriger vers login
    router.replace('/');
  }
});
</script>

<template>
  <div class="text-center pa-10">
    <h2>Connexion en cours...</h2>
  </div>
</template>

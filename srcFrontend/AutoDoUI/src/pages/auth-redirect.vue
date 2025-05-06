<script setup lang="ts">
import { onMounted } from 'vue';
import { useAuth } from '@/composables/useauth';
import { useRouter } from 'vue-router';

const { fetchUser } = useAuth();
const router = useRouter();

onMounted(async () => {
  try {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/user/token`, {
      credentials: 'include'
    });

    const data = await response.json();
    if (!data.token) throw new Error("Token manquant");

    localStorage.setItem('autodo_token', data.token);
    await fetchUser();
    router.replace('/consultant/list-consultant');
  } catch (err) {
    console.error("Échec auth redirect", err);
    router.replace('/');
  }
});
</script>

<template>
  <div class="text-center pa-10">
    <h2>Connexion en cours...</h2>
  </div>
</template>

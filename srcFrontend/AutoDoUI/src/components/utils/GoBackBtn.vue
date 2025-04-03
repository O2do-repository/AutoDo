<template>
    <v-btn @click="goBack" outlined color="primary" size="small" icon>
      <v-icon>mdi-arrow-left</v-icon>
    </v-btn>
  </template>
  
  <script setup lang="ts">
  import { useRouter } from 'vue-router';
  
  // Instanciation du router
  const router = useRouter();
  
  // Fonction pour revenir à la page précédente
  const goBack = () => {
    router.back();
  };
  </script>
  
  <style scoped>
  /* Personnalisation du style du bouton si nécessaire */
  .v-btn {
    font-size: 16px;
    display: flex;
    align-items: center;
  }
  </style>
  
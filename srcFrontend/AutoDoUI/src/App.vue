<template>
  <v-app>
    <v-container>
      <v-btn to="/table-profile" class="mr-2">Liste des Profils</v-btn>
      <v-btn to="/add-profile" color="primary">Ajouter un Profil</v-btn>
    <router-view />
    </v-container>
    
  </v-app>
</template>

<script lang="ts" setup>
  //
</script>

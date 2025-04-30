<template>
  <v-app>
    <SideMenu />
    <SideImage />
    <v-main>
      <v-container fluid>
        <slot></slot>
      </v-container>
    </v-main>
  </v-app>
</template>

<script lang="ts" setup>
import SideMenu from '../components/utils/SideMenu.vue'
import SideImage from '../components/utils/SiteImage.vue'
</script>

<style scoped>
/* Ajoute ton style ici si besoin */
</style>

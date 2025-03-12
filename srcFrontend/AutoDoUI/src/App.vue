<template>
  <v-app>
    <SideMenu />
    <SideImage />
    <v-container fluid>
      <router-view />
    </v-container>
    
  </v-app>
</template>

<script lang="ts" setup>
import SideMenu from './components/utils/SideMenu.vue';
import SideImage from './components/utils/SiteImage.vue'; 
</script>

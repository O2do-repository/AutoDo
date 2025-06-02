<template>
  <v-navigation-drawer app>
    <v-list class="d-flex flex-column fill-height justify-space-between">
      
      <!-- Haut : logo + navigation -->
      <div>
        <v-list-item>
          <v-list-item-title class="text-h6 font-weight-bold">
            AutoDo by O2do
          </v-list-item-title>
        </v-list-item>

        <v-divider class="my-2"></v-divider>

        <!-- Admin -->
        <v-list-item to="/admin" link>
          <v-list-item-title class="font-weight-bold">Admin</v-list-item-title>
        </v-list-item>

        <!-- Consultant -->
        <v-list-item to="/consultant/list-consultant" link>
          <v-list-item-title class="font-weight-bold">Consultants</v-list-item-title>
        </v-list-item>

        <!-- Dashboard -->
        <v-list-item to="/table-matching" link>
          <v-list-item-title class="font-weight-bold">Dashboard</v-list-item-title>
        </v-list-item>
      </div>
      
      <!-- Bas : bouton déconnexion -->
      <div class="pa-2">
        <v-divider :thickness="3" color="black" class="pb-3"></v-divider>
        <v-tooltip text="Se déconnecter"  location="right">
          <template #activator="{ props }">
            <v-btn  icon color="primary" v-bind="props" @click="logout">
              <v-icon>mdi-logout</v-icon>
            </v-btn>
          </template>
        </v-tooltip>
      </div>
      
    </v-list>
  </v-navigation-drawer>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()

function logout() {
  localStorage.removeItem('apiKey')
  router.push('/login')
}
</script>

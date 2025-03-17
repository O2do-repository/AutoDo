<template>
    <v-navigation-drawer app>
      <v-list>
        <v-list-item>
          <v-list-item-title class="text-h6 font-weight-bold">AutoDo by O2do</v-list-item-title>
        </v-list-item>
  
        <v-divider></v-divider>
  
        <!-- Admin -->
        <v-list-item to="/" link>
          <v-list-item-title class="font-weight-bold">RFP</v-list-item-title>
        </v-list-item>
  
        <!-- Consultant avec sous-menu -->
        <v-list-group>
          <template #activator="{ props }">
            <v-list-item v-bind="props">
              <v-list-item-title>Consultant</v-list-item-title>
            </v-list-item>
          </template>
  
          <v-list-item to="/table-profile" link >
            <v-list-item-title class="text-primary">Liste des profils</v-list-item-title>
          </v-list-item>
          <v-list-item to="/add-profile" link >
            <v-list-item-title class="text-primary">Ajouter un profil</v-list-item-title>
          </v-list-item>
        </v-list-group>
  
        <!-- Dashboard -->
        <v-list-item to="/table-matching" link>
          <v-list-item-title class="font-weight-bold">Dashboard</v-list-item-title>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>
  </template>
  
  <script setup lang="ts">
  </script>
  
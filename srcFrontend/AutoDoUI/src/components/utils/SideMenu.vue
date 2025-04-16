<template>
    <v-navigation-drawer app>
      <v-list>
        <v-list-item>
          <v-list-item-title class="text-h6 font-weight-bold">AutoDo by O2do</v-list-item-title>
        </v-list-item>
  
        <v-divider></v-divider>
  
        <!-- Admin -->
        <v-list-item to="/admin" link>
          <v-list-item-title class="font-weight-bold">Admin</v-list-item-title>
        </v-list-item>
  
        <!-- Consultant avec sous-menu -->
        <v-list-item to="/consultant/list-consultant" link >
          <v-list-item-title class="font-weight-bold">Consultants</v-list-item-title>
        </v-list-item>
  
        <!-- Dashboard -->
        <v-list-item to="/table-matching" link>
          <v-list-item-title class="font-weight-bold">Dashboard</v-list-item-title>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>
  </template>
  
  <script setup lang="ts">
  </script>
  
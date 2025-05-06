<script setup lang="ts">
import { useAuth } from '@/composables/useauth';

const { login } = useAuth();
</script>

<template>
  <v-btn color="primary" @click="login">Se connecter</v-btn>
</template>

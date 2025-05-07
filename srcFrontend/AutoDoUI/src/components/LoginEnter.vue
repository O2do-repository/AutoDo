<template>
    <v-container class="fill-height d-flex justify-center align-center" fluid>
      <v-card elevation="8" max-width="500" class="pa-8">
        <v-card-title class="text-h5 font-weight-bold text-primary">
          Authentification
        </v-card-title>
  
        <v-card-text>
          <v-form @submit.prevent="submit">
            <v-text-field
              v-model="apiKey"
              label="Clé API"
              type="password"
              prepend-inner-icon="mdi-key"
              variant="outlined"
              color="primary"
              class="mb-4 text-field-lg"
              :rules="[v => !!v || 'Clé requise']"
              required
            />
  
            <v-btn
              type="submit"
              color="primary"
              block
              size="large"
              :loading="loading"
            >
              Valider
            </v-btn>
  
            <v-alert
              v-if="error"
              type="error"
              class="mt-4"
              border="start"
              variant="tonal"
            >
              {{ error }}
            </v-alert>
          </v-form>
        </v-card-text>
      </v-card>
    </v-container>
  </template>
  
  <script setup lang="ts">
  import { ref } from 'vue'
  import { useRouter } from 'vue-router'
  
  const apiKey = ref('')
  const error = ref('')
  const loading = ref(false)
  const router = useRouter()
  
  const submit = async () => {
    error.value = ''
    loading.value = true
  
    try {
      const response = await fetch(`${import.meta.env.VITE_API_URL}/api/validate-key`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ apiKey: apiKey.value }),
      })
  
      if (!response.ok) {
        error.value = 'Erreur lors de la validation de la clé API.'
        return
      }
  
      const data = await response.json()
  
      if (data.valid === true) {
        localStorage.setItem('apiKey', apiKey.value)
        router.push('/consultant/list-consultant')
      } else {
        error.value = 'Clé API invalide'
      }
    } catch (err) {
      error.value = 'Erreur réseau. Veuillez réessayer.'
      console.error(err)
    } finally {
      loading.value = false
    }
  }
  </script>
  
  <style scoped>
  .text-field-lg input {
    font-size: 1.2rem;
    padding: 14px 16px;
    height: 56px !important;
  }
  </style>
  
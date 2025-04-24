<template>
    <div>
      <!-- Bouton de suppression -->
      <v-btn color="error" variant="text" icon="mdi-delete" density="comfortable" @click="dialog = true" />
  
      <!-- Dialog de confirmation -->
      <v-dialog v-model="dialog" max-width="500">
        <v-card>
          <v-card-title class="headline">Confirmer la suppression</v-card-title>
          <v-card-text>
            Êtes-vous sûr de vouloir supprimer ce consultant ?
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="grey" @click="dialog = false">Annuler</v-btn>
            <v-btn color="red" :loading="loading" @click="deleteConsultant">Supprimer</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
  
      <!-- Snackbar -->
      <v-snackbar v-model="snackbar" :color="snackbarColor" timeout="3000">
        {{ snackbarMessage }}
      </v-snackbar>
    </div>
  </template>
  
  <script setup lang="ts">
  import { ref } from 'vue';
  
  const props = defineProps<{ consultantUuid: string }>();

  
  const emit = defineEmits<{
    (e: 'consultantDeleted', payload: { uuid: string; message: string }): void;
  }>();

  const dialog = ref(false);
  const loading = ref(false);
  const snackbar = ref(false);
  const snackbarMessage = ref('');
  const snackbarColor = ref('');
  
  const deleteConsultant = async () => {
  if (!props.consultantUuid) {
    snackbarMessage.value = 'UUID consultant manquant';
    snackbarColor.value = 'red';
    snackbar.value = true;
    return;
  }

  loading.value = true;

  try {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/consultant/${props.consultantUuid}`, {
      method: 'DELETE',
    });

    const rawText = await response.text();

    let result: any;
    try {
      result = JSON.parse(rawText);
    } catch (e) {
      throw new Error('Erreur serveur : réponse invalide');
    }

    if (!response.ok || !result.success) {
      throw new Error(result.message || 'Erreur lors de la suppression');
    }

    emit('consultantDeleted', {
      uuid: props.consultantUuid, 
      message: result.message || 'Consultant supprimé avec succès',
    });

  } catch (error: any) {
    snackbarMessage.value = error.message || 'Erreur inconnue';
    snackbarColor.value = 'red';
  } finally {
    snackbar.value = true;
    loading.value = false;
  }
};

  </script>
  
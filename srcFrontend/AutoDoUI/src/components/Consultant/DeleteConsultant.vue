<template>
    <div>
      <!-- Bouton de suppression -->
      <v-btn color="red" icon="mdi-delete" density="comfortable" @click="dialog = true" />
  
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
    (e: 'consultantDeleted', uuid: string): void;
  }>();
  
  const dialog = ref(false);
  const loading = ref(false);
  const snackbar = ref(false);
  const snackbarMessage = ref('');
  const snackbarColor = ref('');
  
  const deleteConsultant = async () => {
    console.log('Consultant UUID à supprimer:', props.consultantUuid);  // Vérifie ici l'UUID
    loading.value = true;
    try {
      const response = await fetch(`${import.meta.env.VITE_API_URL}/consultant/${props.consultantUuid}`, {
        method: 'DELETE',
      });
  
      if (!response.ok) throw new Error('Erreur lors de la suppression');
  
      emit('consultantDeleted', props.consultantUuid);
      snackbarMessage.value = 'Consultant supprimé avec succès';
      snackbarColor.value = 'green';
      dialog.value = false;
    } catch (error: any) {
      snackbarMessage.value = error.message || 'Erreur inconnue';
      snackbarColor.value = 'red';
    } finally {
      snackbar.value = true;
      loading.value = false;
    }
  };
  </script>
  
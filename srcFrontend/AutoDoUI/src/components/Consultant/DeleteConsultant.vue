<template>
    <div>
      <!-- Delete button -->
      <v-btn color="error" variant="text" icon="mdi-delete" density="comfortable" @click="dialog = true" />
  
      <!-- Confirmation dialog -->
      <v-dialog v-model="dialog" max-width="500">
        <v-card>
          <v-card-title class="headline">Confirm deletion</v-card-title>
          <v-card-text>
            Are you sure you want to delete this consultant?
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="grey" @click="dialog = false">Cancel</v-btn>
            <v-btn color="red" :loading="loading" @click="deleteConsultant">Delete</v-btn>
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
  import { fetchWithApiKey } from '@/utils/fetchWithApiKey';
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
    snackbarMessage.value = 'Missing consultant UUID';
    snackbarColor.value = 'red';
    snackbar.value = true;
    return;
  }
  loading.value = true;
  try {
    const response = await fetchWithApiKey(`${import.meta.env.VITE_API_URL}/consultant/${props.consultantUuid}`, {
      method: 'DELETE',
    });
    const rawText = await response.text();
    let result: any;
    try {
      result = JSON.parse(rawText);
    } catch (e) {
      throw new Error('Server error: invalid response');
    }
    if (!response.ok || !result.success) {
      throw new Error(result.message);
    }
    emit('consultantDeleted', {
      uuid: props.consultantUuid, 
      message: result.message,
    });
  } catch (error: any) {
    snackbarMessage.value = error.message || 'Unknown error';
    snackbarColor.value = 'red';
  } finally {
    snackbar.value = true;
    loading.value = false;
  }
};
  </script>
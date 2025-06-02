<template>
    <div>
    <v-btn color="error" icon="mdi-delete" density="comfortable" @click="dialog = true">
    </v-btn>
  
    <v-dialog v-model="dialog" max-width="500">
        <v-card>
            <v-card-title class="headline">Confirmer la suppression</v-card-title>
            <v-card-text>
                Êtes-vous sûr de vouloir supprimer ce profil ?
            </v-card-text>
            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="grey"  @click="dialog = false">Annuler</v-btn>
                <v-btn color="red"  :loading="loading" @click="deleteProfile">Supprimer</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
    <v-snackbar v-model="snackbar" :color="snackbarColor" timeout="3000">
        {{ snackbarMessage }}
    </v-snackbar>

    </div>
</template>
  
<script setup lang="ts">
    import { fetchWithApiKey } from '@/utils/fetchWithApiKey';
import { ref } from 'vue';
    import { useRouter } from 'vue-router';



  
    const props = defineProps<{
        profileUuid: string;
    }>();
  
    const emit = defineEmits<{
    (e: 'profileDeleted', payload: { uuid: string; message: string }): void;
  }>();
  
    const dialog = ref(false);
    const loading = ref(false);
    const snackbar = ref(false);
    const snackbarMessage = ref('');
    const snackbarColor = ref('');
    const router = useRouter();
    
    const deleteProfile= async () => {
    if (!props.profileUuid) {
      snackbarMessage.value = 'UUID profil manquant';
      snackbarColor.value = 'red';
      snackbar.value = true;
      return;
    }

    loading.value = true;

    try {
      const response = await fetchWithApiKey(`${import.meta.env.VITE_API_URL}/profil/${props.profileUuid}`, {

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
        throw new Error(result.message);
      }

      emit('profileDeleted', {
        uuid: props.profileUuid, 
        message: result.message,
      });

    } catch (error: any) {
      snackbarMessage.value = error.message;
      snackbarColor.value = 'red';
    } finally {
      snackbar.value = true;
      loading.value = false;
    }
  };



</script>
  
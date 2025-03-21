<template>
    <div>
    <v-btn color="red" icon="mdi-delete" density="comfortable" @click="dialog = true">
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
    import { ref } from 'vue';
    import { useRouter } from 'vue-router';

    const snackbar = ref(false);
    const snackbarMessage = ref('');
    const snackbarColor = ref('');

  
    const props = defineProps<{
        profileUuid: string;
    }>();
  
    const emit = defineEmits<{
        (e: 'profileDeleted', payload: { message: string; color: string }): void;
    }>();
  
    const dialog = ref(false);
    const loading = ref(false);
    const router = useRouter();
    
    const deleteProfile = async () => {
  loading.value = true;
  try {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/profil/${props.profileUuid}`, {
      method: 'DELETE',
      headers: { 'Content-Type': 'application/json' },
    });

    if (!response.ok) {
      throw new Error(`Erreur ${response.status}: Suppression échouée`);
    }

    dialog.value = false;
    emit('profileDeleted', { message: 'Profil supprimé avec succès', color: 'green' });

  } catch (error) {
    console.error(error);
    const errorMessage = error instanceof Error ? error.message : 'Erreur inattendue';
    emit('profileDeleted', { message: errorMessage, color: 'red' });
  } finally {
    loading.value = false;
  }
};



</script>
  
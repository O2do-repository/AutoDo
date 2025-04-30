<script setup lang="ts">
import { ref, onMounted } from 'vue'

interface Entreprise {
  enterpriseUuid: string;
  name: string;
}

const newEntreprise = ref<string>('')
const entreprises = ref<Entreprise[]>([])
const selectedEntrepriseIndex = ref<number | null>(null)
const error = ref<string | null>(null);
const success = ref(false);


const dialog = ref(false)
const loading = ref(false)
const snackbar = ref(false)
const snackbarMessage = ref('')
const snackbarColor = ref('')

const fetchEntreprises = async () => {
  try {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/enterprise`);
    const resData = await response.json();

    if (!response.ok || !resData.success) {
      throw new Error(resData.message || "Erreur lors de la récupération des entreprises");
    }

    entreprises.value = resData.data;
  } catch (error) {
    console.error(error instanceof Error ? error.message : 'Une erreur est survenue');
  }
}


const submitEntreprise = async () => {
  const name = newEntreprise.value.trim();
  if (!name) return;

  loading.value = true;
  error.value = null;
  success.value = false;

  try {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/enterprise`, {
      credentials: 'include',
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ name })
    });

    const result = await response.json();

    if (!response.ok || !result.success) {
      throw new Error(result.message);
    }

    entreprises.value.push(result.data); // bien récupérer le `data` retourné par le controller
    newEntreprise.value = '';
    success.value = true;
    snackbarMessage.value = result.message;
    snackbarColor.value = 'green';
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Une erreur est survenue';
    snackbarMessage.value = error.value;
    snackbarColor.value = 'red';
  } finally {
    snackbar.value = true;
    loading.value = false;
  }
};


const confirmDeleteEntreprise = (index: number) => {
  selectedEntrepriseIndex.value = index
  dialog.value = true
}

const removeEntreprise = async (uuid: string) => {
  loading.value = true;
  error.value = null;
  success.value = false;
  console.log("UUID à supprimer:", uuid); // Ajoutez ceci pour déboguer

  try {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/enterprise/${uuid}`, {
      
      method: 'DELETE'
    });

    const result = await response.json();

    if (!response.ok || !result.success) {
      throw new Error(result.message);
    }

    entreprises.value = entreprises.value.filter(e => e.enterpriseUuid !== uuid);
    success.value = true;
    snackbarMessage.value = result.message;
    snackbarColor.value = 'green';
    dialog.value = false;
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Une erreur est survenue';
    snackbarMessage.value = error.value;
    snackbarColor.value = 'red';
  } finally {
    snackbar.value = true;
    loading.value = false;
  }
};


onMounted(fetchEntreprises)
</script>

<template>
  <v-container>
    <v-row class="mt-5">
      <v-col cols="4">
        <v-text-field 
            v-model="newEntreprise" 
            label="Ajouter une entreprise" 
            variant="outlined"
            @keyup.enter="submitEntreprise"
            hide-details
            density="comfortable"
        />
      </v-col>
      <v-col cols="2">
        <v-btn 
            color="primary" 
            @click="submitEntreprise"
            icon
            variant="text"
            :disabled="!newEntreprise.trim()"
        >
          <v-icon large>mdi-plus-circle</v-icon>
        </v-btn>
      </v-col>
      <v-col cols="6">
        <v-card
          elevation="3"
          rounded="lg"
          class="skills-card"
        >
          <v-card-title class="text-h6 py-3 px-4 bg-primary text-white d-flex align-center">
            Entreprises
            <v-chip class="ms-2" size="small" color="white" text-color="primary">{{ entreprises.length }}</v-chip>
          </v-card-title>
          
          <div class="scroll-container">
            <v-list density="compact" bg-color="transparent">
              <v-list-item
                v-for="(entreprise, index) in entreprises"
                :key="entreprise.enterpriseUuid"
                class="skill-item"
                :class="{ 'even-item': index % 2 === 0 }"
              >
                <v-list-item-title>{{ entreprise.name }}</v-list-item-title>
                
                <template v-slot:append>
                  <v-btn
                    icon="mdi-delete"
                    variant="text"
                    color="error"
                    size="small"
                    @click="confirmDeleteEntreprise(index)"
                  ></v-btn>
                </template>
              </v-list-item>
            </v-list>

            <!-- Dialog de confirmation -->
            <v-dialog v-model="dialog" max-width="500">
              <v-card>
                <v-card-title class="headline">Confirmer la suppression</v-card-title>
                <v-card-text>
                  Êtes-vous sûr de vouloir supprimer cette entreprise ?
                </v-card-text>
                <v-card-actions>
                  <v-spacer></v-spacer>
                  <v-btn color="grey" @click="dialog = false">Annuler</v-btn>
                  <v-btn
                    color="red"
                    :loading="loading"
                    @click="() => selectedEntrepriseIndex !== null && removeEntreprise(entreprises[selectedEntrepriseIndex].enterpriseUuid)"
                  >
                    Supprimer
                  </v-btn>

                </v-card-actions>
              </v-card>
            </v-dialog>

            <!-- Snackbar -->
            <v-snackbar v-model="snackbar" :color="snackbarColor" timeout="3000">
              {{ snackbarMessage }}
            </v-snackbar>
          </div>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<style scoped>
.scroll-container {
  max-height: 250px;
  overflow-y: auto;
  scrollbar-width: thin;
}

.scroll-container::-webkit-scrollbar {
  width: 6px;
}

.scroll-container::-webkit-scrollbar-thumb {
  background-color: rgba(0, 0, 0, 0.2);
  border-radius: 3px;
}

.skill-item {
  transition: background-color 0.2s;
}

.skill-item:hover {
  background-color: rgba(0, 0, 0, 0.05);
}

.even-item {
  background-color: rgba(0, 0, 0, 0.02);
}
</style>
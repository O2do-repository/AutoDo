<script setup lang="ts">
import { ref, onMounted } from 'vue'

interface Enterprise {
  enterpriseUuid: string;
  name: string;
}

const newEnterprise = ref<string>('')
const enterprises = ref<Enterprise[]>([])
const selectedEnterpriseIndex = ref<number | null>(null)

const dialog = ref(false)
const loading = ref(false)
const snackbar = ref(false)
const snackbarMessage = ref('')
const snackbarColor = ref('')

const fetchEnterprises = async () => {
  try {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/enterprise`)
    if (!response.ok) throw new Error("Erreur lors de la récupération des entreprises")

    enterprises.value = await response.json()
  } catch (error) {
    console.error(error instanceof Error ? error.message : 'Une erreur est survenue')
  }
}

const submitEnterprise = async () => {
  const name = newEnterprise.value.trim()
  if (!name) return

  try {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/enterprise`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ name })
    })

    if (!response.ok) throw new Error("Erreur lors de l'ajout de l'entreprise")

    const createdEnterprise = await response.json()
    enterprises.value.push(createdEnterprise)
    newEnterprise.value = ''
  } catch (error) {
    console.error(error instanceof Error ? error.message : 'Une erreur est survenue')
  }
}

const confirmDeleteEnterprise = (index: number) => {
  selectedEnterpriseIndex.value = index
  dialog.value = true
}

const removeEnterprise = async () => {
  if (selectedEnterpriseIndex.value === null) return
  const index = selectedEnterpriseIndex.value
  const enterprise = enterprises.value[index]
  loading.value = true

  try {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/enterprise/${enterprise.enterpriseUuid}`, {
      method: 'DELETE',
    })

    if (!response.ok) throw new Error("Erreur lors de la suppression")

    enterprises.value.splice(index, 1)
    snackbarMessage.value = 'Entreprise supprimée avec succès'
    snackbarColor.value = 'green'
    dialog.value = false
  } catch (error: any) {
    snackbarMessage.value = error.message || 'Erreur inconnue'
    snackbarColor.value = 'red'
  } finally {
    snackbar.value = true
    loading.value = false
    selectedEnterpriseIndex.value = null
  }
}

onMounted(fetchEnterprises)
</script>

<template>
  <v-container>
    <v-row class="mt-5">
      <v-col cols="4">
        <v-text-field 
          v-model="newEnterprise" 
          label="Ajouter une entreprise" 
          variant="outlined"
          @keyup.enter="submitEnterprise"
          hide-details
          density="comfortable"
        />
      </v-col>
      <v-col cols="2">
        <v-btn 
          color="primary" 
          @click="submitEnterprise"
          icon
          variant="text"
          :disabled="!newEnterprise.trim()"
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
            <v-chip class="ms-2" size="small" color="white" text-color="primary">{{ enterprises.length }}</v-chip>
          </v-card-title>

          <div class="scroll-container">
            <v-list density="compact" bg-color="transparent">
              <v-list-item
                v-for="(enterprise, index) in enterprises"
                :key="enterprise.enterpriseUuid"
                class="skill-item"
                :class="{ 'even-item': index % 2 === 0 }"
              >
                <v-list-item-title>{{ enterprise.name }}</v-list-item-title>
                
                <template v-slot:append>
                  <v-btn
                    icon="mdi-delete"
                    variant="text"
                    color="error"
                    size="small"
                    @click="confirmDeleteEnterprise(index)"
                  ></v-btn>
                </template>
              </v-list-item>
            </v-list>

            <!-- Dialog -->
            <v-dialog v-model="dialog" max-width="500">
              <v-card>
                <v-card-title class="headline">Confirmer la suppression</v-card-title>
                <v-card-text>
                  Êtes-vous sûr de vouloir supprimer cette entreprise ?
                </v-card-text>
                <v-card-actions>
                  <v-spacer></v-spacer>
                  <v-btn color="grey" @click="dialog = false">Annuler</v-btn>
                  <v-btn color="red" :loading="loading" @click="removeEnterprise">Supprimer</v-btn>
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

<script setup lang="ts">
import { ref, onMounted } from 'vue'

interface Keyword {
  keywordUuid: string;
  name: string;
}

const newKeyword = ref<string>('')
const keywords = ref<Keyword[]>([])
const selectedKeywordIndex = ref<number | null>(null)

const dialog = ref(false)
const loading = ref(false)
const snackbar = ref(false)
const snackbarMessage = ref('')
const snackbarColor = ref('')

const fetchKeywords = async () => {
  try {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/keyword`)
    if (!response.ok) throw new Error("Erreur lors de la récupération des mots-clés")

    keywords.value = await response.json()
  } catch (error) {
    console.error(error instanceof Error ? error.message : 'Une erreur est survenue')
  }
}

const submitKeyword = async () => {
  const name = newKeyword.value.trim()
  if (!name) return

  try {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/keyword`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ name })
    })

    if (!response.ok) throw new Error("Erreur lors de l'ajout du mot-clé")

    const createdKeyword = await response.json()
    keywords.value.push(createdKeyword)
    newKeyword.value = ''
  } catch (error) {
    console.error(error instanceof Error ? error.message : 'Une erreur est survenue')
  }
}

const confirmDeleteKeyword = (index: number) => {
  selectedKeywordIndex.value = index
  dialog.value = true
}

const removeKeyword = async () => {
  if (selectedKeywordIndex.value === null) return
  const index = selectedKeywordIndex.value
  const keyword = keywords.value[index]
  loading.value = true

  try {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/keyword/${keyword.keywordUuid}`, {
      method: 'DELETE',
    })

    if (!response.ok) throw new Error("Erreur lors de la suppression")

    keywords.value.splice(index, 1)
    snackbarMessage.value = 'Mot-clé supprimé avec succès'
    snackbarColor.value = 'green'
    dialog.value = false
  } catch (error: any) {
    snackbarMessage.value = error.message || 'Erreur inconnue'
    snackbarColor.value = 'red'
  } finally {
    snackbar.value = true
    loading.value = false
    selectedKeywordIndex.value = null
  }
}

onMounted(fetchKeywords)
</script>

<template>
  <v-container>
    <v-row class="mt-5">
      <v-col cols="4">
        <v-text-field 
            v-model="newKeyword" 
            label="Ajouter un mot-clé" 
            variant="outlined"
            @keyup.enter="submitKeyword"
            hide-details
            density="comfortable"
        />
      </v-col>
      <v-col cols="2">
        <v-btn 
            color="primary" 
            @click="submitKeyword"
            icon
            variant="text"
            :disabled="!newKeyword.trim()"
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
            Mots-clés
            <v-chip class="ms-2" size="small" color="white" text-color="primary">{{ keywords.length }}</v-chip>
          </v-card-title>
          
          <div class="scroll-container">
            <v-list density="compact" bg-color="transparent">
              <v-list-item
                v-for="(keyword, index) in keywords"
                :key="keyword.keywordUuid"
                class="skill-item"
                :class="{ 'even-item': index % 2 === 0 }"
              >
                <v-list-item-title>{{ keyword.name }}</v-list-item-title>
                
                <template v-slot:append>
                  <v-btn
                    icon="mdi-delete"
                    variant="text"
                    color="error"
                    size="small"
                    @click="confirmDeleteKeyword(index)"
                  ></v-btn>
                </template>
              </v-list-item>
            </v-list>

            <!-- Dialog de confirmation -->
            <v-dialog v-model="dialog" max-width="500">
              <v-card>
                <v-card-title class="headline">Confirmer la suppression</v-card-title>
                <v-card-text>
                  Êtes-vous sûr de vouloir supprimer ce mot-clé ?
                </v-card-text>
                <v-card-actions>
                  <v-spacer></v-spacer>
                  <v-btn color="grey" @click="dialog = false">Annuler</v-btn>
                  <v-btn color="red" :loading="loading" @click="removeKeyword">Supprimer</v-btn>
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
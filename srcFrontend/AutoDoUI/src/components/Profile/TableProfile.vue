<script setup lang="ts">
import { ref, onMounted } from 'vue';

interface Profile {
  consultantUuid: string;
  RateHour: number;
  CV: string;
  CVDate: string;
  JobTitle: string;
  ExperienceLevel: string;
  skills: string[];
  keywords: string[];
}

const profiles = ref<Profile[]>([]);
const error = ref<string | null>(null);
const search = ref('');

const headers = ref([
  { key: 'jobTitle', title: 'Titre du poste', align: 'start' as const },
  { key: 'experienceLevel', title: 'Expérience', align: 'start' as const },
  { key: 'rateHour', title: 'Tarif / heure', align: 'end' as const },
  { key: 'cv', title: 'Lien CV', align: 'center' as const },
  { key: 'cvDate', title: 'Date CV', align: 'end' as const },
  { key: 'skills', title: 'Compétences', align: 'start' as const },
  { key: 'keywords', title: 'Mots-clés', align: 'start' as const }
]);

const fetchProfiles = async () => {
  try {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/profil`);
    if (!response.ok) {
      throw new Error("Erreur lors de la récupération des profils");
    }
    const data = await response.json();
    console.log("Données reçues :", data); // Vérifier le format ici
    profiles.value = Array.isArray(data) ? data : [data];
  } catch (err) {
    error.value = err instanceof Error ? err.message : "Une erreur est survenue";
  }
};


onMounted(fetchProfiles);
</script>

<template>
  <v-container>
    <h1 class="text-center mb-6">Liste des Profils</h1>
    
    <v-btn to="/add-profile" color="primary" class="mb-4">Ajouter un profil</v-btn>

    <v-alert v-if="error" type="error" variant="outlined" class="mb-4">
      {{ error }}
    </v-alert>

    <v-text-field v-model="search" label="Rechercher un profil" prepend-inner-icon="mdi-magnify" variant="outlined" hide-details single-line class="mb-4"></v-text-field>

    <v-data-table :headers="headers" :items="profiles" :search="search" item-value="consultantUuid" class="elevation-2" dense>
      <template v-slot:item.CV="{ item }">
        <a v-if="item.CV" :href="item.CV" target="_blank">Voir le CV</a>
        <span v-else>Non disponible</span>
      </template>

      <template v-slot:item.skills="{ item }">
        {{ item.skills && item.skills.length ? item.skills.join(', ') : 'Aucune compétence' }}
      </template>

      <template v-slot:item.keywords="{ item }">
        {{ item.keywords && item.keywords.length ? item.keywords.join(', ') : 'Aucun mot-clé' }}
      </template>
    </v-data-table>

    <v-alert v-if="!profiles.length && !error" type="info" class="mt-4">
      Aucun profil disponible.
    </v-alert>
  </v-container>
</template>

<style scoped>
.v-container {
  margin: auto;
}

a {
  text-decoration: none;
  color: #1976d2;
  font-weight: bold;
}

a:hover {
  text-decoration: underline;
}
</style>

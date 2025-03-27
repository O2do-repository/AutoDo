<template>
  <v-container>
    <h1 class="text-center mb-6">Liste des RFP</h1>

    <v-alert v-if="error" type="error" variant="outlined" class="mb-4">
      {{ error }}
    </v-alert>

    <v-text-field
      v-model="search"
      label="Rechercher un RFP"
      prepend-inner-icon="mdi-magnify"
      variant="outlined"
      hide-details
      single-line
      class="mb-4"
    ></v-text-field>

    <v-data-table
      :headers="headers"
      :items="rfps"
      :search="search"
      item-value="Uuid"
      class="elevation-2"
      dense
    >
      <template v-slot:item.rfpUrl="{ item }">
        <a v-if="item.rfpUrl" :href="item.rfpUrl" target="_blank">Voir l'offre</a>
        <span v-else>Non disponible</span>
      </template>


    </v-data-table>

    <v-alert v-if="!rfps.length && !error" type="info" class="mt-4">
      Aucun RFP disponible.
    </v-alert>
  </v-container>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';

interface RFP {
  Uuid: string;
  deadlineDate: string;
  descriptionBrut: string;
  experienceLevel: string;
  jobTitle: string;
  publicationDate: string;
  rfpPriority: string;
  rfpUrl: string;
  skills: string[];
  workplace: string;
}

const rfps = ref<RFP[]>([]);
const error = ref<string | null>(null);
const search = ref('');

const headers = ref([
  { key: 'jobTitle', title: 'Titre du poste', align: 'start' as const },
  { key: 'publicationDate', title: 'Publiée le', align: 'end' as const, sortable: true },
  { key: 'deadlineDate', title: 'Date limite', align: 'end' as const, sortable: true },
  { key: 'experienceLevel', title: 'Expérience', align: 'start' as const },
  { key: 'rfpPriority', title: 'Priorité', align: 'center' as const },
  { key: 'workplace', title: 'Lieu', align: 'start' as const },
  { key: 'rfpUrl', title: 'Lien', align: 'center' as const, sortable: false },
]);

const fetchRFPList = async () => {
  try {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/rfp`);

    if (!response.ok) {
      throw new Error("Erreur lors de la récupération des RFP ");
    }

    const data = await response.json();
    console.log("Données reçues du backend :", data);

    rfps.value = Array.isArray(data) ? data : [data];
  } catch (err) {
    error.value = err instanceof Error ? err.message : "Une erreur inconnue  est survenue";
    console.error(error.value);
  }
};

onMounted(fetchRFPList);



</script>

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

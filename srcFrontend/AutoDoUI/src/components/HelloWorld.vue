<template>
  <v-container>
    <h1>Liste des RFP</h1>
    <v-row>
      <v-col v-for="rfp in rfps" :key="rfp.Uuid" cols="12" md="4">
        <v-card>
          <v-card-title>{{ rfp.Title }}</v-card-title>
          <v-card-subtitle>{{ rfp.Reference }}</v-card-subtitle>
          <v-card-text>
            <p><strong>Description :</strong> {{ rfp.Description }}</p>
            <p><strong>Client :</strong> {{ rfp.Customer.Name }}</p>
            <p><strong>Date de réponse :</strong> {{ new Date(rfp.ResponseDate).toLocaleDateString() }}</p>
          </v-card-text>
          <v-card-actions>
            <v-btn color="primary" @click="viewRFP(rfp.Uuid)">Voir les détails</v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
    <v-alert v-if="error" type="error">{{ error }}</v-alert>
  </v-container>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';

interface Customer {
  Name: string;
}


interface RFP {
  Uuid: string;
  Title: string;
  Reference: string;
  Description: string;
  Customer: Customer;
  ResponseDate: string;  // Date au format ISO 8601
  DeadlineDate: string;  // Date au format ISO 8601
  DescriptionBrut: string;
  ExperienceLevel: string;
  RfpPriority: string;
  PublicationDate: string;  // Date au format ISO 8601
  Skills: string[];  // Liste des compétences
  JobTitle: string;
  RfpUrl: string;
  Workplace: string;
  RfpId: string;
}

const rfps = ref<RFP[]>([]);
const error = ref<string | null>(null); 

const fetchRFPList = async () => {
  try {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/rfp`);

    
    if (!response.ok) {
      throw new Error('Erreur lors de la récupération des RFP');
    }

    const data = await response.json();
    rfps.value = data;
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Une erreur inconnue est survenue';
    console.error(error.value);
  }
};

onMounted(fetchRFPList);

const viewRFP = (uuid: string) => {
  window.location.href = `/rfp/${uuid}`;
};
</script>


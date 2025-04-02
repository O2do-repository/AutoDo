<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';

interface Consultant {
  Email: string;
  AvailabilityDate: string;
  ExpirationDateCI: string;
  Intern: boolean | null;
  Name: string;
  Surname: string;
  enterprise: number | null;
  Phone: string;
  CopyCI: string;
  Picture: string;
}

const consultant = ref<Consultant>({
  Email: '',
  AvailabilityDate: '',
  ExpirationDateCI: '',
  Intern: null,
  Name: '',
  Surname: '',
  enterprise: null,
  Phone: '',
  CopyCI: '',
  Picture: ''
});

const router = useRouter();

// Liste des entreprises disponibles
const enterprises = [
  { id: 0, name: 'Winch' },
  { id: 1, name: 'O2do' },
  { id: 2, name: 'MI6' }
];

const submitConsultant = async () => {
  try {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/consultant`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(consultant.value)
    });

    if (!response.ok) {
      throw new Error('Erreur lors de l\'ajout du consultant');
    }

    setTimeout(() => {
      router.push('/consultant/list-consultant');
    }, 1000);
    
  } catch (error) {
    console.error(error instanceof Error ? error.message : 'Une erreur est survenue');
  }
};
</script>

<template>
  <v-container>
    <v-card class="pa-4">
      <v-card-title>Nouveau Consultant</v-card-title>
      <v-card-text>
        <v-text-field label="Email *" v-model="consultant.Email" required></v-text-field>
        <v-text-field label="Date de disponibilité *" v-model="consultant.AvailabilityDate" type="datetime-local" required></v-text-field>
        <v-text-field label="Expiration CI *" v-model="consultant.ExpirationDateCI" type="datetime-local" required></v-text-field>
        <v-switch label="Stagiaire ?" v-model="consultant.Intern"></v-switch>
        <v-text-field label="Nom *" v-model="consultant.Name" required></v-text-field>
        <v-text-field label="Prénom *" v-model="consultant.Surname" required></v-text-field>

        <!-- Combo box pour entreprise -->
        <v-select
          label="Entreprise *"
          v-model="consultant.enterprise"
          :items="enterprises"
          item-title="name"
          item-value="id"
          required
        ></v-select>

        <v-text-field label="Téléphone *" v-model="consultant.Phone" required></v-text-field>
        <v-text-field label="Lien Copie CI *" v-model="consultant.CopyCI" required></v-text-field>
        <v-text-field label="Lien Photo *" v-model="consultant.Picture" required></v-text-field>
      </v-card-text>
      <v-card-actions>
        <v-btn color="primary" @click="submitConsultant()">Publier</v-btn>
      </v-card-actions>
    </v-card>
  </v-container>
</template>

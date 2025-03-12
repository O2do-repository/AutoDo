<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router'; // Importer le router pour la redirection

interface Profile {
  consultantUuid: string;
  RateHour: number | null;
  CV: string;
  CVDate: string;
  JobTitle: string;
  ExperienceLevel: string;
  Skills: string[];
  keywords: string[];
}

const profile = ref<Profile>({
  consultantUuid: '50bcf3a7-d5d9-4017-9df4-f0da847bfe5a',
  RateHour: null,
  CV: '',
  CVDate: '',
  JobTitle: '',
  ExperienceLevel: '',
  Skills: [],
  keywords: []
});

const experienceLevels: string[] = ['Junior', 'Medior', 'Senior'];
const availableSkills: string[] = ['C#', 'ASP.NET', 'Vue.js', 'SQL'];
const availableKeywords: string[] = ['Backend', 'Frontend', 'API'];

const errorMessage = ref<string | null>(null); 

const router = useRouter();

const submitProfile = async () => {
  
  errorMessage.value = null;
  if (!profile.value.JobTitle || !profile.value.CV || !profile.value.CVDate || !profile.value.ExperienceLevel) {
        errorMessage.value = 'Veuillez remplir tous les champs obligatoires.';
        return;
    }

  try {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/profil`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(profile.value)
    });

    if (!response.ok) {
      throw new Error('Erreur lors de l\'ajout du profil');
    }

    // Redirection vers la liste des profils après un délai de 1 seconde
    setTimeout(() => {
      router.push('/table-profile'); // Remplacer '/table-profil' par la route correcte de ta liste des profils
    }, 1000);

  } catch (error) {
    console.error(error instanceof Error ? error.message : 'Une erreur est survenue');
  }
};
</script>

<template>
  <v-container>
    <v-alert closable v-if="errorMessage" type="error" variant="outlined">
        {{ errorMessage }}
    </v-alert>  
    <v-card class="pa-4">
      <v-card-title>Nouvel Profil</v-card-title>
      <v-card-text>
        <v-text-field label="Job Title *" v-model="profile.JobTitle" required></v-text-field>
        <v-select label="Niveau d'expérience *" v-model="profile.ExperienceLevel" :items="experienceLevels" required></v-select>
        <v-text-field label="Tarif / heure *" v-model.number="profile.RateHour" type="number"></v-text-field>
        <v-text-field label="CV URL *" v-model="profile.CV" required></v-text-field>
        <v-text-field label="Date CV *" v-model="profile.CVDate" type="datetime-local" required></v-text-field>
        <v-select
          label="Compétences"
          v-model="profile.Skills"
          :items="availableSkills"
          item-text="name"
          item-value="name"
          multiple
          required
        ></v-select>
        <v-select
          label="Mots-clés"
          v-model="profile.keywords"
          :items="availableKeywords"
          item-text="name"
          item-value="name"
          multiple
          required
        ></v-select>
      </v-card-text>
      <v-card-actions>
        <v-btn color="primary" @click="submitProfile()">Publier</v-btn>
      </v-card-actions>
    </v-card>
  </v-container>
</template>
  

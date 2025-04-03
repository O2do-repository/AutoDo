<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import GoBackBtn from '@/components/utils/GoBackBtn.vue'; 

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
  consultantUuid: '', // Initialement vide, sera rempli après récupération de l'UUID
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

const router = useRouter();

// Récupère l'UUID du consultant depuis sessionStorage
onMounted(() => {
  const storedUuid = sessionStorage.getItem('selectedConsultantUuid');
  if (storedUuid) {
    profile.value.consultantUuid = storedUuid; // Met à jour l'UUID du consultant dans le profil
  } else {
    console.error('Aucun consultant sélectionné');
  }
});

const submitProfile = async () => {
  try {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/profil`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(profile.value)
    });

    if (!response.ok) {
      throw new Error('Erreur lors de l\'ajout du profil');
    }

    setTimeout(() => {
      router.go(-1);
    }, 1000);

  } catch (error) {
    console.error(error instanceof Error ? error.message : 'Une erreur est survenue');
  }
};
</script>

<template>
  <v-container>
    
    <v-card class="pa-4">
      <GoBackBtn/>
      <v-card-title>Nouveau Profil</v-card-title>
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

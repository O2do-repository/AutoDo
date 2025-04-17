<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import GoBackBtn from '@/components/utils/GoBackBtn.vue';

interface Profile {
  profileUuid: string;
  rateHour: number | null;
  cv: string;
  cvDate: string;
  jobTitle: string;
  experienceLevel: string;
  skills: string[];
  keywords: string[];
}

const storedProfile = localStorage.getItem('selectedProfile');

const profile = ref<Profile>(storedProfile ? JSON.parse(storedProfile) : {
  profileUuid: '',
  rateHour: null,
  cv: '',
  cvDate: '',
  jobTitle: '',
  experienceLevel: '',
  skills: [],
  keywords: []
});

const router = useRouter();

const experienceLevels: string[] = ['Junior', 'Medior', 'Senior'];
const availableSkills = ref<string[]>([]);
const availableKeywords = ref<string[]>([]);

const formRef = ref(); // Référence du formulaire
const errorMessage = ref<string | null>(null);

// Règles de validation
const required = (v: any) => !!v || 'Champ obligatoire';
const numberRule = (v: any) => !isNaN(v) && Number(v) >= 0 || 'Entier positif requis';
const urlRule = (value: string) => /^(https?:\/\/)[^\s$.?#].[^\s]*$/.test(value) || "Lien invalide (ex: https://...)";
const dateRule = (v: string) => !!v || 'Date requise';

// Récupération des données dynamiques
const fetchSkills = async () => {
  try {
    const res = await fetch(`${import.meta.env.VITE_API_URL}/skill`);
    if (!res.ok) throw new Error('Erreur récupération des skills');
    const data = await res.json();
    availableSkills.value = data.map((item: any) => item.name);
  } catch (error) {
    console.error('Erreur skills :', error);
  }
};

const fetchKeywords = async () => {
  try {
    const res = await fetch(`${import.meta.env.VITE_API_URL}/keyword`);
    if (!res.ok) throw new Error('Erreur récupération des keywords');
    const data = await res.json();
    availableKeywords.value = data.map((item: any) => item.name);
  } catch (error) {
    console.error('Erreur keywords :', error);
  }
};

onMounted(() => {
  fetchSkills();
  fetchKeywords();
});

const submitProfile = async () => {
  errorMessage.value = null;

  if (!formRef.value) return;
  const result: { valid: boolean } = await formRef.value.validate();
  if (!result.valid) return;

  try {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/profil`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(profile.value)
    });

    if (!response.ok) throw new Error('Erreur lors de la mise à jour du profil');

    setTimeout(() => {
      router.go(-1);
    }, 1000);

  } catch (error) {
    console.error(error instanceof Error ? error.message : 'Une erreur est survenue');
    errorMessage.value = 'Erreur lors de la mise à jour';
  }
};
</script>


<template>
    <v-container>
      
  
      <v-alert
        v-if="errorMessage"
        closable
        type="error"
        variant="outlined"
        class="mb-4"
      >
        {{ errorMessage }}
      </v-alert>
  
      <v-card class="pa-4">
        <GoBackBtn />
        <v-card-title class="text-h5 font-weight-bold">Modifier le Profil</v-card-title>
        <v-card-text>
          <v-form ref="formRef">
            <v-row>
              <v-col cols="6">
                <v-text-field 
                  label="Job Title *" 
                  v-model="profile.jobTitle" 
                  :rules="[required]" 
                  variant="outlined" 
                  color="primary" 
                  required 
                />
              </v-col>
  
              <v-col cols="6">
                <v-select 
                  label="Niveau d'expérience *" 
                  v-model="profile.experienceLevel" 
                  :items="experienceLevels" 
                  :rules="[required]" 
                  variant="outlined" 
                  color="primary" 
                  required 
                />
              </v-col>
  
              <v-col cols="6">
                <v-text-field 
                  label="Tarif / heure *" 
                  v-model.number="profile.rateHour" 
                  type="number" 
                  :rules="[required, numberRule]" 
                  variant="outlined" 
                  color="primary" 
                  required 
                />
              </v-col>
  
              <v-col cols="6">
                <v-text-field 
                  label="CV URL *" 
                  v-model="profile.cv" 
                  :rules="[required, urlRule]" 
                  variant="outlined" 
                  color="primary" 
                  required 
                />
              </v-col>
  
              <v-col cols="6">
                <v-text-field 
                  label="Date du CV *" 
                  v-model="profile.cvDate" 
                  type="date" 
                  :rules="[required, dateRule]" 
                  variant="outlined" 
                  color="primary" 
                  required 
                />
              </v-col>
  
              <v-col cols="6">
                <v-select 
                  label="Compétences" 
                  v-model="profile.skills" 
                  :items="availableSkills" 
                  multiple 
                  chips 
                  closable-chips 
                  :rules="[required]" 
                  variant="outlined" 
                  color="primary" 
                />
              </v-col>
  
              <v-col cols="12">
                <v-select 
                  label="Mots-clés" 
                  v-model="profile.keywords" 
                  :items="availableKeywords" 
                  multiple 
                  chips 
                  closable-chips 
                  :rules="[required]" 
                  variant="outlined" 
                  color="primary" 
                />
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>
  
        <v-card-actions class="d-flex justify-end">
          <v-btn color="primary" @click="submitProfile">Publier</v-btn>
        </v-card-actions>
      </v-card>
    </v-container>
  </template>
  
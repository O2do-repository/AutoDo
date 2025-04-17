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
  consultantUuid: '',
  RateHour: null,
  CV: '',
  CVDate: '',
  JobTitle: '',
  ExperienceLevel: '',
  Skills: [],
  keywords: []
});

const router = useRouter();

const experienceLevels: string[] = ['Junior', 'Medior', 'Senior'];
const availableSkills = ref<string[]>([]);
const availableKeywords = ref<string[]>([]);
const formRef = ref(); 


// Règles de validation
const required = (v: any) => !!v || 'Champ obligatoire';
const numberRule = (v: any) => !isNaN(v) && Number(v) >= 0 || 'Entier positif requis';
const urlRule = (value: string) => /^(https?:\/\/)[^\s$.?#].[^\s]*$/.test(value) || "Lien invalide (ex: https://...)";

const dateRule = (v: string) => !!v || 'Date requise';

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
  const storedUuid = sessionStorage.getItem('selectedConsultantUuid');
  if (storedUuid) {
    profile.value.consultantUuid = storedUuid;
  } else {
    console.error('Aucun consultant sélectionné');
  }

  fetchSkills();
  fetchKeywords();
});


// ajouter un profile
const submitProfile = async (event: SubmitEvent): Promise<void> => {
  event.preventDefault();
  if (!formRef.value) return;

  const { valid } = await formRef.value.validate();
  if (!valid) return;

  try {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/profil`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(profile.value)
    });

    if (!response.ok) throw new Error('Erreur lors de l\'ajout du profil');

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
      <GoBackBtn class="mb-4" />

      <v-card-title class="text-h5 font-weight-bold">Nouveau Profil</v-card-title>
      <v-card-text>
        <v-form ref="formRef">
          <v-row>
            <v-col cols="6">
              <v-text-field 
                label="Job Title *" 
                v-model="profile.JobTitle" 
                :rules="[required]" 
                variant="outlined" 
                color="primary"
                required 
              />
            </v-col>

            <v-col cols="6">
              <v-select 
                label="Niveau d'expérience *" 
                v-model="profile.ExperienceLevel" 
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
                v-model.number="profile.RateHour" 
                type="number" 
                :rules="[required, numberRule]" 
                variant="outlined" 
                color="primary"
                required 
              />
            </v-col>

            <v-col cols="6">
              <v-text-field 
                label="Lien du CV *" 
                v-model="profile.CV" 
                :rules="[required, urlRule]" 
                variant="outlined" 
                color="primary"
                required 
              />
            </v-col>

            <v-col cols="6">
              <v-text-field 
                label="Date du CV *" 
                v-model="profile.CVDate" 
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
                v-model="profile.Skills" 
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

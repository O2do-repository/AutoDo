<script lang="ts">
import { defineComponent, ref, onMounted, watch } from 'vue';
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

export default defineComponent({
  name: 'EditProfile',
  components: {
    GoBackBtn
  },
  setup() {
    // Récupération du profil stocké dans le localStorage
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
    const formRef = ref<any>(null);
    const errorMessage = ref<string | null>(null);

    // 🧪 CV placeholder et validation
    const placeholderCV = 'https://example.com/default-cv';
    const validCV = ref('');

    // 🧪 Règles de validation
    const required = (v: any) => !!v || 'Champ obligatoire';
    const numberRule = (v: any) => !isNaN(v) && Number(v) >= 0 || 'Entier positif requis';
    const urlRule = (value: string) => /^(https?:\/\/)[^\s$.?#].[^\s]*$/.test(value) || "Lien invalide (ex: https://...)";
    const dateRule = (v: string) => !!v || 'Date requise';

    // Vérifier si le lien CV est valide
    const checkCV = () => {
      if (!profile.value.cv || profile.value.cv.trim() === '') {
        validCV.value = '';
        return;
      }

      // Pour un PDF on ne peut pas vraiment vérifier avec l'API Image
      // mais on pourrait implémenter une vérification d'URL
      validCV.value = profile.value.cv;
    };

    // Gérer le placeholder pour le CV
    const clearPlaceholder = () => {
      if (profile.value.cv === placeholderCV) {
        profile.value.cv = '';
      }
    };

    const restorePlaceholder = () => {
      if (!profile.value.cv) {
        profile.value.cv = placeholderCV;
      }
    };

    // Surveiller les changements du CV
    watch(() => profile.value.cv, checkCV);

    // Récupération des compétences
    const fetchSkills = async () => {
      try {
        const res = await fetch(`${import.meta.env.VITE_API_URL}/skill`);
        if (!res.ok) throw new Error('Erreur récupération des skills');
        const data = await res.json();
        availableSkills.value = data.data.map((item: any) => item.name);
      } catch (error) {
        console.error('Erreur skills :', error);
      }
    };

    // Récupération des mots-clés
    const fetchKeywords = async () => {
      try {
        const res = await fetch(`${import.meta.env.VITE_API_URL}/keyword`);
        if (!res.ok) throw new Error('Erreur récupération des keywords');
        const data = await res.json();
        availableKeywords.value = data.data.map((item: any) => item.name);
      } catch (error) {
        console.error('Erreur keywords :', error);
      }
    };

    // Appel des fonctions au montage du composant
    onMounted(() => {
      fetchSkills();
      fetchKeywords();
      checkCV();
    });

    const loading = ref(false);
    const error = ref<string | null>(null);
    const success = ref(false);


    // Soumettre le profil
    const submitProfile = async () => {
      loading.value = true;
      error.value = null;
      success.value = false;
      try {
        if (!profile.value.cv || profile.value.cv.trim() === '') {
          profile.value.cv = placeholderCV;
        }

        if (!formRef.value) return;

        const { valid } = await formRef.value.validate();
        if (!valid) return;

        const response = await fetch(`${import.meta.env.VITE_API_URL}/profil`, {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(profile.value)
        });

        const data = await response.json();
        if (!response.ok) {
          throw new Error(data.message);
        }

        success.value = data.message;
        setTimeout(() => {
          router.push('/consultant/consultant-info');
        }, 1000);

      } catch (err) {
        error.value = err instanceof Error ? err.message : 'Une erreur est survenue';
      } finally {
        loading.value = false;
      }
    };

    return {
      profile,
      experienceLevels,
      availableSkills,
      availableKeywords,
      formRef,
      required,
      numberRule,
      urlRule,
      dateRule,
      placeholderCV,
      validCV,
      clearPlaceholder,
      restorePlaceholder,
      submitProfile,
      errorMessage,
      loading,
      error,
      success
    };
  }
});
</script>

<template>
  <v-container>
    <v-alert v-if="errorMessage" closable type="error" variant="outlined" class="mb-4">
      {{ errorMessage }}
    </v-alert>

    <v-card class="pa-4">
      <GoBackBtn class="mb-4" />
      
      <v-card-title class="text-h5 font-weight-bold">Modifier le Profil</v-card-title>
      <v-card-text>
        <v-form ref="formRef">
          <v-row>
            <!-- Lien du CV -->
            <v-col cols="12">
              <v-text-field 
                label="CV URL *" 
                v-model="profile.cv" 
                variant="outlined" 
                color="primary"
                :placeholder="placeholderCV"
                :rules="[required, urlRule]" 
                @focus="clearPlaceholder"
                @blur="restorePlaceholder"
                required 
              />
            </v-col>

            <!-- Job Title -->
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

            <!-- Niveau d'expérience -->
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

            <!-- Tarif / heure -->
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

            <!-- Date du CV -->
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

            <!-- Compétences -->
            <v-col cols="12">
              <v-select
                label="Compétences *"
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

            <!-- Mots-clés -->
            <v-col cols="12">
              <v-select
                label="Mots-clés *"
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
      <v-alert v-if="loading" type="info" class="mt-4">Chargement en cours...</v-alert>
      <v-alert v-if="error" type="error" class="mt-4">{{ error }}</v-alert>
      <v-alert v-if="success" type="success" class="mt-4">{{ success }}</v-alert>

      <v-card-actions class="d-flex justify-end">
        <v-btn color="primary" @click="submitProfile">Publier</v-btn>
      </v-card-actions>
    </v-card>
  </v-container>
</template>
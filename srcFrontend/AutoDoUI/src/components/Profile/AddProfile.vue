<script lang="ts">
import { defineComponent, ref, onMounted, watch } from 'vue';
import { useRouter } from 'vue-router';
import GoBackBtn from '@/components/utils/GoBackBtn.vue';

// Définition de l'interface Profile
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

export default defineComponent({
  name: 'AddProfile',
  components: {
    GoBackBtn
  },
  setup() {
    // Déclaration des variables
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
    const formRef = ref<any>(null);

    // CV placeholder
    const placeholderCV = 'https://example.com/default-cv.pdf';
    const validCV = ref('');

    // Règles de validation
    const required = (v: any) => !!v || 'Champ obligatoire';
    const numberRule = (v: any) => !isNaN(v) && Number(v) >= 0 || 'Entier positif requis';
    const urlRule = (value: string) => /^(https?:\/\/)[^\s$.?#].[^\s]*$/.test(value) || "Lien invalide (ex: https://...)";
    const dateRule = (v: string) => !!v || 'Date requise';

    // Fonction pour récupérer les compétences
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

    // Fonction pour récupérer les mots-clés
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

    // Vérifier si le lien CV est valide
    const checkCV = () => {
      if (!profile.value.CV || profile.value.CV.trim() === '') {
        validCV.value = '';
        return;
      }


      validCV.value = profile.value.CV;
    };

    // Gérer le placeholder pour le CV
    const clearPlaceholder = () => {
      if (profile.value.CV === placeholderCV) {
        profile.value.CV = '';
      }
    };

    const restorePlaceholder = () => {
      if (!profile.value.CV) {
        profile.value.CV = placeholderCV;
      }
    };

    // Surveiller les changements du CV
    watch(() => profile.value.CV, checkCV);

    // Récupérer le consultant UUID
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

    const loading = ref(false);
    const error = ref<string | null>(null);
    const success = ref(false);


    // Soumettre le profil
    const submitProfile = async () => {
      loading.value = true;
      error.value = null;
      success.value = false;
      try {
        if (!profile.value.CV || profile.value.CV.trim() === '') {
          profile.value.CV = placeholderCV;
        }

        if (!formRef.value) return;

        const { valid } = await formRef.value.validate();
        if (!valid) return;

        const response = await fetch(`${import.meta.env.VITE_API_URL}/profil`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(profile.value)
        });

        const data = await response.json();
        if (!response.ok) {
          throw new Error(data.message || "Erreur lors de l'ajout du profil");
        }

        success.value = data.message || 'Profil modifié avec succès !';
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
      loading,
      error,
      success
    };
  }
});
</script>

<template>
  <v-container>
    <v-card class="pa-4">
      <GoBackBtn class="mb-4" />

      <v-card-title class="text-h5 font-weight-bold">Nouveau Profil</v-card-title>
      <v-card-text>
        <v-form ref="formRef">
          <v-row>
            <!-- Lien du CV -->
            <v-col cols="12">
              <v-text-field 
                label="Lien du CV *" 
                v-model="profile.CV" 
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
                v-model="profile.JobTitle" 
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
                v-model="profile.ExperienceLevel" 
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
                v-model.number="profile.RateHour" 
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
                v-model="profile.CVDate" 
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
      <v-alert v-if="success" type="success" class="mt-4">Consultant ajouté avec succès !</v-alert>

      <!-- Bouton Publier -->
      <v-card-actions class="d-flex justify-end">
        <v-btn color="primary" @click="submitProfile">Publier</v-btn>
      </v-card-actions>
    </v-card>
  </v-container>
</template>
<script lang="ts">
import { defineComponent, ref, onMounted, watch } from 'vue';
import { useRouter } from 'vue-router';
import GoBackBtn from '@/components/utils/GoBackBtn.vue';
import { fetchWithApiKey } from '@/utils/fetchWithApiKey';

interface Profile {
  profileUuid: string;
  consultantUuid: string;
  rateHour: number | null;
  cv: string;
  cvDate: string;
  jobTitle: string;
  experienceLevel: string;
  skills: string[];     // UUIDs
  keywords: string[];   // UUIDs
}

export default defineComponent({
  name: 'EditProfile',
  components: { GoBackBtn },
  setup() {
    const storedProfile = localStorage.getItem('selectedProfile');

    const profile = ref<Profile>(storedProfile ? JSON.parse(storedProfile) : {
      profileUuid: '',
      consultantUuid: '',
      rateHour: null,
      cv: '',
      cvDate: '',
      jobTitle: '',
      experienceLevel: '',
      skills: [],
      keywords: []
    });

    const router = useRouter();
    const experienceLevels = ['Junior', 'Medior', 'Senior'];
    const availableSkills = ref<{ name: string; uuid: string }[]>([]);
    const availableKeywords = ref<{ name: string; uuid: string }[]>([]);
    const formRef = ref<any>(null);
    const errorMessage = ref<string | null>(null);
    const placeholderCV = 'https://example.com/default-cv';
    const validCV = ref('');
    const loading = ref(false);
    const error = ref<string | null>(null);
    const success = ref(false);

    const required = (v: any) => !!v || 'Champ obligatoire';
    const numberRule = (v: any) => !isNaN(v) && Number(v) >= 0 || 'Entier positif requis';
    const urlRule = (v: string) => /^(https?:\/\/)[^\s$.?#].[^\s]*$/.test(v) || "Lien invalide (ex: https://...)";
    const dateRule = (v: string) => !!v || 'Date requise';

    const checkCV = () => {
      validCV.value = profile.value.cv || '';
    };
    const clearPlaceholder = () => {
      if (profile.value.cv === placeholderCV) profile.value.cv = '';
    };
    const restorePlaceholder = () => {
      if (!profile.value.cv) profile.value.cv = placeholderCV;
    };
    watch(() => profile.value.cv, checkCV);

    const fetchSkills = async () => {
      try {
        const res = await fetchWithApiKey(`${import.meta.env.VITE_API_URL}/skill`);
        const data = await res.json();
        availableSkills.value = data.data.map((item: any) => ({
          name: item.name,
          uuid: item.skillUuid
        }));
      } catch (error) {
        console.error('Erreur skills :', error);
      }
    };

    const fetchKeywords = async () => {
      try {
        const res = await fetchWithApiKey(`${import.meta.env.VITE_API_URL}/keyword`);
        const data = await res.json();
        availableKeywords.value = data.data.map((item: any) => ({
          name: item.name,
          uuid: item.keywordUuid
        }));
      } catch (error) {
        console.error('Erreur keywords :', error);
      }
    };

    onMounted(() => {
      fetchSkills();
      fetchKeywords();
      checkCV();
    });

    const submitProfile = async () => {
      loading.value = true;
      error.value = null;
      success.value = false;
      try {
        if (!formRef.value) return;
        if (!profile.value.cv || profile.value.cv.trim() === '') {
          profile.value.cv = placeholderCV;
        }

        const { valid } = await formRef.value.validate();
        if (!valid) return;

        const payload = {
          profileUuid: profile.value.profileUuid,
          consultantUuid: profile.value.consultantUuid,
          rateHour: profile.value.rateHour,
          cv: profile.value.cv,
          cvDate: profile.value.cvDate,
          jobTitle: profile.value.jobTitle,
          experienceLevel: profile.value.experienceLevel,
          skillUuids: profile.value.skills,
          keywordUuids: profile.value.keywords
        };

        const response = await fetchWithApiKey(`${import.meta.env.VITE_API_URL}/profil`, {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(payload)
        });

        const data = await response.json();
        if (!response.ok) throw new Error(data.message);

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
                label="Date du CV *"
                v-model="profile.cvDate"
                type="date"
                :rules="[required, dateRule]"
                variant="outlined"
                color="primary"
                required
              />
            </v-col>
            <v-col cols="12">
              <v-autocomplete 
                label="Compétences *"
                v-model="profile.skills"
                :items="availableSkills"
                item-title="name"
                item-value="uuid"
                multiple
                chips
                closable-chips
                :rules="[required]"
                variant="outlined"
                color="primary"
              />
            </v-col>
            <v-col cols="12">
              <v-autocomplete 
                label="Mots-clés *"
                v-model="profile.keywords"
                :items="availableKeywords"
                item-title="name"
                item-value="uuid"
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

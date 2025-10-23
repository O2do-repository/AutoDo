<script lang="ts">
import { defineComponent, ref, onMounted, watch } from 'vue';
import { useRouter } from 'vue-router';
import GoBackBtn from '@/components/utils/GoBackBtn.vue';
import { fetchWithApiKey } from '@/utils/fetchWithApiKey';

interface ProfilePayload {
  consultantUuid: string;
  RateHour: number | null;
  CV: string;
  CVDate: string;
  JobTitle: string;
  ExperienceLevel: string;
  SkillUuids: string[];
  KeywordUuids: string[];
}

export default defineComponent({
  name: 'AddProfile',
  components: { GoBackBtn },
  setup() {
    const router = useRouter();

    const profile = ref<ProfilePayload>({
      consultantUuid: '',
      RateHour: null,
      CV: '',
      CVDate: '',
      JobTitle: '',
      ExperienceLevel: '',
      SkillUuids: [],
      KeywordUuids: []
    });

    const experienceLevels: string[] = ['Junior', 'Medior', 'Senior'];
    const availableSkills = ref<{ name: string; uuid: string }[]>([]);
    const availableKeywords = ref<{ name: string; uuid: string }[]>([]);

    const selectedSkillUuids = ref<string[]>([]);
    const selectedKeywordUuids = ref<string[]>([]);

    const formRef = ref<any>(null);
    const placeholderCV = 'https://example.com/default-cv';
    const validCV = ref('');

    const loading = ref(false);
    const error = ref<string | null>(null);
    const success = ref(false);

    // ✅ Validation rules
    const required = (v: any) => !!v || 'Champ obligatoire';
    const numberRule = (v: any) =>
      (!isNaN(v) && Number(v) >= 0) || 'Entier positif requis';
    const urlRule = (value: string) =>
      /^(https?:\/\/)[^\s$.?#].[^\s]*$/.test(value) ||
      'Lien invalide (ex: https://...)';
    const dateRule = (v: string) => !!v || 'Date requise';

    // ✅ Fetch Skills & Keywords
    const fetchSkills = async () => {
      try {
        const res = await fetchWithApiKey(`${import.meta.env.VITE_API_URL}/skill`);
        if (!res.ok) throw new Error('Erreur récupération des skills');
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
        if (!res.ok) throw new Error('Erreur récupération des keywords');
        const data = await res.json();
        availableKeywords.value = data.data.map((item: any) => ({
          name: item.name,
          uuid: item.keywordUuid
        }));
      } catch (error) {
        console.error('Erreur keywords :', error);
      }
    };

    const checkCV = () => {
      validCV.value = profile.value.CV?.trim() ? profile.value.CV : '';
    };

    const clearPlaceholder = () => {
      if (profile.value.CV === placeholderCV) profile.value.CV = '';
    };

    const restorePlaceholder = () => {
      if (!profile.value.CV) profile.value.CV = placeholderCV;
    };

    watch(() => profile.value.CV, checkCV);

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

    const submitProfile = async () => {
      loading.value = true;
      error.value = null;
      success.value = false;

      try {
        if (!profile.value.CV || profile.value.CV.trim() === '') {
          profile.value.CV = placeholderCV;
        }

        const { valid } = await formRef.value.validate();
        if (!valid) return;

        // ✅ Construire payload
        const payload = {
          consultantUuid: profile.value.consultantUuid,
          RateHour: profile.value.RateHour,
          CV: profile.value.CV,
          CVDate: profile.value.CVDate,
          JobTitle: profile.value.JobTitle,
          ExperienceLevel: profile.value.ExperienceLevel,
          SkillUuids: selectedSkillUuids.value,
          KeywordUuids: selectedKeywordUuids.value
        };

        const response = await fetchWithApiKey(`${import.meta.env.VITE_API_URL}/profil`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(payload)
        });

        const data = await response.json();
        if (!response.ok) throw new Error(data.message || 'Erreur lors de l’envoi');

        success.value = true;
        setTimeout(() => router.push('/consultant/consultant-info'), 1000);
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
      selectedSkillUuids,
      selectedKeywordUuids,
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
            <!-- CV -->
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

            <!-- Tarif -->
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

            <!-- Date CV -->
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

            <!-- Skills -->
            <v-col cols="12">
              <v-autocomplete
                label="Compétences *"
                v-model="selectedSkillUuids"
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

            <!-- Keywords -->
            <v-col cols="12">
              <v-autocomplete
                label="Mots-clés *"
                v-model="selectedKeywordUuids"
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

      <v-alert v-if="loading" type="info" class="mt-4">Chargement...</v-alert>
      <v-alert v-if="error" type="error" class="mt-4">{{ error }}</v-alert>
      <v-alert v-if="success" type="success" class="mt-4">Profil créé avec succès</v-alert>

      <v-card-actions class="d-flex justify-end">
        <v-btn color="primary" @click="submitProfile">Publier</v-btn>
      </v-card-actions>
    </v-card>
  </v-container>
</template>

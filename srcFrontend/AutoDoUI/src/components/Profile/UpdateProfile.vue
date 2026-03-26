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

    // Helper function to extract UUIDs from skills/keywords
    const extractUuids = (items: any[]): string[] => {
      if (!Array.isArray(items)) return [];
      
      return items.map(item => {
        // If it's already a UUID string
        if (typeof item === 'string') {
          // Check if it looks like a UUID (contains hyphens and proper format)
          if (/^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(item)) {
            return item;
          }
          return null; // It's a name string, skip it
        }
        // If it's an object with uuid/skillUuid/keywordUuid property
        if (typeof item === 'object' && item !== null) {
          return item.uuid || item.skillUuid || item.keywordUuid || item.SkillUuid || item.KeywordUuid || null;
        }
        return null;
      }).filter(uuid => uuid !== null) as string[];
    };

    let parsedProfile: Profile = {
      profileUuid: '',
      consultantUuid: '',
      rateHour: null,
      cv: '',
      cvDate: '',
      jobTitle: '',
      experienceLevel: '',
      skills: [],
      keywords: []
    };

    if (storedProfile) {
      try {
        const raw = JSON.parse(storedProfile);
        console.log('Loaded profile from localStorage:', raw); // Debug log
        
        parsedProfile = {
          profileUuid: raw.profileUuid || raw.ProfileUuid || '',
          consultantUuid: raw.consultantUuid || raw.ConsultantUuid || '',
          rateHour: raw.rateHour || raw.RateHour || null,
          cv: raw.cv || raw.Cv || '',
          cvDate: raw.cvDate || raw.CvDate || '',
          jobTitle: raw.jobTitle || raw.JobTitle || '',
          experienceLevel: raw.experienceLevel || raw.ExperienceLevel || '',
          skills: extractUuids(raw.skills || raw.Skills || []),
          keywords: extractUuids(raw.keywords || raw.Keywords || [])
        };
        
        console.log('Normalized profile:', parsedProfile); // Debug log
      } catch (e) {
        console.error('Error parsing stored profile:', e);
      }
    }

    const profile = ref<Profile>(parsedProfile);

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
        console.log('Skills API response:', data.data); // Debug log
        availableSkills.value = data.data.map((item: any) => ({
          name: item.name,
          uuid: item.skillUuid || item.SkillUuid // Handle both camelCase and PascalCase
        }));
        console.log('Mapped skills:', availableSkills.value); // Debug log
      } catch (error) {
        console.error('Erreur skills :', error);
      }
    };

    const fetchKeywords = async () => {
      try {
        const res = await fetchWithApiKey(`${import.meta.env.VITE_API_URL}/keyword`);
        const data = await res.json();
        console.log('Keywords API response:', data.data); // Debug log
        availableKeywords.value = data.data.map((item: any) => ({
          name: item.name,
          uuid: item.keywordUuid || item.KeywordUuid // Handle both camelCase and PascalCase
        }));
        console.log('Mapped keywords:', availableKeywords.value); // Debug log
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
        if (!valid) {
          loading.value = false;
          return;
        }

        // Ensure skills and keywords are valid UUID arrays
        const uuidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
        
        const skillUuids = Array.isArray(profile.value.skills) 
          ? profile.value.skills.filter(uuid => 
              uuid && 
              typeof uuid === 'string' && 
              uuid.trim() !== '' &&
              uuidRegex.test(uuid) // Only include valid UUIDs
            )
          : [];
        
        const keywordUuids = Array.isArray(profile.value.keywords)
          ? profile.value.keywords.filter(uuid => 
              uuid && 
              typeof uuid === 'string' && 
              uuid.trim() !== '' &&
              uuidRegex.test(uuid) // Only include valid UUIDs
            )
          : [];

        console.log('Filtered skillUuids:', skillUuids); // Debug log
        console.log('Filtered keywordUuids:', keywordUuids); // Debug log

        // Use PascalCase to match C# DTO expectations
        const payload = {
          ProfileUuid: profile.value.profileUuid,
          ConsultantUuid: profile.value.consultantUuid,
          RateHour: profile.value.rateHour,
          Cv: profile.value.cv,
          CvDate: profile.value.cvDate,
          JobTitle: profile.value.jobTitle,
          ExperienceLevel: profile.value.experienceLevel,
          SkillUuids: skillUuids,
          KeywordUuids: keywordUuids
        };

        console.log('Sending payload:', payload); // Debug log

        const response = await fetchWithApiKey(`${import.meta.env.VITE_API_URL}/profil`, {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(payload)
        });

        const data = await response.json();
        if (!response.ok) throw new Error(data.message || 'Erreur lors de la mise à jour');

        success.value = data.message || 'Profil mis à jour avec succès';
        setTimeout(() => {
          router.push('/consultant/consultant-info');
        }, 1000);

      } catch (err) {
        error.value = err instanceof Error ? err.message : 'Une erreur est survenue';
        console.error('Error submitting profile:', err);
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
      <v-card-title class="text-h5 font-weight-bold">Edit Profile</v-card-title>
      <v-card-text>
        <v-form ref="formRef">
          <v-row>
            <v-col cols="12">
              <v-text-field 
                label="CV Link *" 
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
                label="Experience Level *"
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
                label="Hourly Rate (€) *"
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
                label="CV Date *"
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
                label="Skills *"
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
                label="Keywords *"
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

      <v-alert v-if="loading" type="info" class="mt-4">Loading...</v-alert>
      <v-alert v-if="error" type="error" class="mt-4">{{ error }}</v-alert>
      <v-alert v-if="success" type="success" class="mt-4">{{ success }}</v-alert>

      <v-card-actions class="d-flex justify-end">
        <v-btn color="primary" @click="submitProfile" :disabled="loading">Save Changes</v-btn>
      </v-card-actions>
    </v-card>
  </v-container>
</template>
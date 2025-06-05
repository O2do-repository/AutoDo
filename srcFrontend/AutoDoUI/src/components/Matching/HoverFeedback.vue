<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { fetchWithApiKey } from '@/utils/fetchWithApiKey';

const props = defineProps<{ matchingUuid: string }>();

const feedback = ref<{
  totalScore: number;
  jobTitleScore: number;
  experienceScore: number;
  skillsScore: number;
  locationScore: number;
  jobTitleFeedback: string;
  experienceFeedback: string;
  skillsFeedback: string;
  locationFeedback: string;
  createdAt: string;
  lastUpdatedAt: string;
} | null>(null);

const loading = ref(false);
const error = ref<string | null>(null);
const dialog = ref(false);

const fetchFeedback = async () => {
  loading.value = true;
  error.value = null;
  try {
    const response = await fetchWithApiKey(`${import.meta.env.VITE_API_URL}/matchingfeedback/${props.matchingUuid}`);
    const data = await response.json();
    if (!response.ok) throw new Error(data?.message || "Erreur de récupération du feedback");
    feedback.value = data?.data;
  } catch (err) {
    error.value = err instanceof Error ? err.message : "Impossible de charger le feedback.";
  } finally {
    loading.value = false;
  }
};

onMounted(fetchFeedback);
</script>

<template>
    <v-icon color="primary" size="small" @click="dialog = true">
        mdi-comment-text-multiple-outline
    </v-icon>

    <v-dialog v-model="dialog" max-width="800">
        <v-card>
            
            <v-btn @click="dialog = false" outlined color="primary" size="small" class="ml-3 mt-2" icon>
                <v-icon>mdi-arrow-left</v-icon>
            </v-btn>
            <v-card-title>
                Feedback de matching
                
            </v-card-title>

      <v-card-text>
        <template v-if="loading">
          <v-progress-linear indeterminate color="primary" />
        </template>
        <template v-else-if="error">
          <v-alert type="error" dense>{{ error }}</v-alert>
        </template>
        <template v-else-if="feedback">
          <div style="white-space: pre-line;">
            <strong>Score total :</strong> {{ feedback.totalScore }}

            <v-divider class="my-2" />
            <strong>Job Title ({{ feedback.jobTitleScore }}/20)</strong>
            <div class="mb-4">{{ feedback.jobTitleFeedback }}</div>

            <strong>Expérience ({{ feedback.experienceScore }}/20)</strong>
            <div class="mb-4">{{ feedback.experienceFeedback }}</div>

            <strong>Compétences ({{ feedback.skillsScore }}/40)</strong>
            <div class="mb-4">{{ feedback.skillsFeedback }}</div>

            <strong>Localisation ({{ feedback.locationScore }}/20)</strong>
            <div class="mb-4">{{ feedback.locationFeedback }}</div>

            <em class="text-grey">Dernière mise à jour : {{ new Date(feedback.lastUpdatedAt).toLocaleString('fr-FR') }}</em>
          </div>
        </template>
        <template v-else>
          <span>Aucun feedback disponible.</span>
        </template>
      </v-card-text>
    </v-card>
  </v-dialog>
</template>

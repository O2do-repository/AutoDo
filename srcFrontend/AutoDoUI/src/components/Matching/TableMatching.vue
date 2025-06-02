<script setup lang="ts">
import { fetchWithApiKey } from '@/utils/fetchWithApiKey';
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';

interface Matching {
  matchingUuid: string;
  rfpUuid: string;
  profileUuid: string;
  jobTitle: string;
  rfpUrl: string;
  consultantName: string;
  consultantSurname: string;
  rfpReference: string;
  score: number;
  publicationDate: string;
  comment: string;
  statutMatching: string;
  editable: boolean;
}

interface JobTitleGroup {
  jobTitle: string;
  matchings: Matching[];
}

interface ConsultantGroup {
  name: string;
  fullName: string;
  jobTitleGroups: JobTitleGroup[];
}

const router = useRouter();
const profiles = ref<Matching[]>([]);
const error = ref<string | null>(null);
const search = ref('');
const expandedConsultants = ref<string[]>([]);

const snackbar = ref(false);
const snackbarMessage = ref('');
const snackbarColor = ref('success');

const headers = ref([
  { key: 'edit', title: '', align: 'center' as const },
  { key: 'rfpReference', title: 'Référence RFP', align: 'start' as const },
  { key: 'score', title: 'Score', align: 'end' as const },
  { key: 'publicationDate', title: "Date de publication", align: 'start' as const },
  { key: 'statutMatching', title: 'Statut', align: 'start' as const },
  { key: 'comment', title: 'Commentaire', align: 'start' as const },
]);

const fetchMatchings = async () => {
  try {
    const response = await fetchWithApiKey(`${import.meta.env.VITE_API_URL}/matching`,

    );
    if (!response.ok) throw new Error("Erreur, impossible de récupérer les matchings");
    
    const data = await response.json();
    profiles.value = (Array.isArray(data.data) ? data.data : [data.data]).map((m: any) => ({
      ...m,
      editable: false 
    }));
    expandedConsultants.value = consultantGroups.value.map(group => group.name);
  } catch (err) {
    error.value = err instanceof Error ? err.message : "Erreur, impossible de récupérer les matchings";
  }
};

const formatDate = (dateString: string) => {
  const date = new Date(dateString);
  return date.toLocaleDateString('fr-FR');
};

const consultantGroups = computed(() => {
  const groupedData: Record<string, ConsultantGroup> = {};

  profiles.value.forEach(profile => {
    const consultantKey = `${profile.consultantSurname}_${profile.consultantName}`;
    const fullName = `${profile.consultantSurname} ${profile.consultantName}`;

    if (!groupedData[consultantKey]) {
      groupedData[consultantKey] = {
        name: consultantKey,
        fullName: fullName,
        jobTitleGroups: []
      };
    }

    let jobGroup = groupedData[consultantKey].jobTitleGroups.find(j => j.jobTitle === profile.jobTitle);

    if (!jobGroup) {
      jobGroup = {
        jobTitle: profile.jobTitle,
        matchings: []
      };
      groupedData[consultantKey].jobTitleGroups.push(jobGroup);
    }

    jobGroup.matchings.push(profile);
  });

  return Object.values(groupedData);
});

const filteredGroups = computed(() => {
  if (!search.value) return consultantGroups.value;
  const searchLower = search.value.toLowerCase();

  return consultantGroups.value.map(group => {
    const consultantMatches = group.fullName.toLowerCase().includes(searchLower);

    const filteredJobTitleGroups = group.jobTitleGroups.map(jobGroup => {
      const jobTitleMatches = jobGroup.jobTitle.toLowerCase().includes(searchLower);

      const filteredMatchings = jobGroup.matchings.filter(matching =>
        matching.jobTitle.toLowerCase().includes(searchLower) ||
        matching.rfpReference.toLowerCase().includes(searchLower) ||
        matching.comment?.toLowerCase().includes(searchLower) ||
        matching.score.toString().includes(searchLower)
      );

      // ✅ Si consultant OU jobTitle match → on garde tout le groupe
      if (consultantMatches || jobTitleMatches) {
        return { jobTitle: jobGroup.jobTitle, matchings: jobGroup.matchings };
      }

      // Sinon on garde les matchings filtrés
      if (filteredMatchings.length > 0) {
        return { jobTitle: jobGroup.jobTitle, matchings: filteredMatchings };
      }

      return null;
    }).filter(Boolean) as JobTitleGroup[];

    // ✅ Afficher le consultant s'il match ou s'il a des job titles filtrés
    if (consultantMatches || filteredJobTitleGroups.length > 0) {
      return { ...group, jobTitleGroups: filteredJobTitleGroups };
    }

    return null;
  }).filter(Boolean) as ConsultantGroup[];
});



const isExpanded = (consultantName: string) => expandedConsultants.value.includes(consultantName);

const toggleExpand = (consultantName: string) => {
  if (isExpanded(consultantName)) {
    expandedConsultants.value = expandedConsultants.value.filter(name => name !== consultantName);
  } else {
    expandedConsultants.value.push(consultantName);
  }
};

const expandAll = () => expandedConsultants.value = consultantGroups.value.map(group => group.name);
const collapseAll = () => expandedConsultants.value = [];

const saveMatchingGroup = async (matchings: Matching[]) => {
  for (let item of matchings) {
    if (item.editable) {
      try {
        const response = await fetchWithApiKey(`${import.meta.env.VITE_API_URL}/matching/${item.matchingUuid}`, {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            statutMatching: item.statutMatching,
            comment: item.comment,
            score: item.score,
            profileUuid: item.profileUuid,
            rfpUuid: item.rfpUuid
          }),
        });
        
        if (response.ok) {
          item.editable = false;
          snackbarMessage.value = 'Modifications sauvegardées avec succès!';
          snackbarColor.value = 'success';
          snackbar.value = true;
        } else {
          throw new Error("Erreur de sauvegarde");
        }
      } catch (error) {
        console.error("Erreur de sauvegarde :", error);
        snackbarMessage.value = 'Échec de la sauvegarde.';
        snackbarColor.value = 'error';
        snackbar.value = true;
      }
    }
  }
};

const hasEditableItems = computed(() => {
  return consultantGroups.value.some(group =>
    group.jobTitleGroups.some(jobGroup =>
      jobGroup.matchings.some(matching => matching.editable)
    )
  );
});

onMounted(fetchMatchings);
</script>

<template>
  <v-container>
    <h1 class="text-center mb-6">Liste des Matchings</h1>
    <v-alert type="info" variant="outlined" class="mb-4 d-flex align-center">
      Vous devez être connecté pour accéder aux liens RFP sur le site Connecting-Expertise.

      <v-btn
        color="info"
        variant="outlined"
        href="https://customer.connecting-expertise.com/"
        target="_blank"
        rel="noopener noreferrer"
        size="small"
      >

        Se connecter
      </v-btn>
    </v-alert>


    <div class="d-flex align-center mb-4">
      <v-text-field 
        v-model="search" 
        label="Rechercher un matching" 
        prepend-inner-icon="mdi-magnify" 
        variant="outlined" 
        hide-details 
        single-line 
        class="flex-grow-1"
      ></v-text-field>

      <v-btn-group class="ml-4">
        <v-btn @click="expandAll" size="small" variant="outlined">
          <v-icon>mdi-unfold-more-horizontal</v-icon> Tout développer
        </v-btn>
        <v-btn @click="collapseAll" size="small" variant="outlined">
          <v-icon>mdi-unfold-less-horizontal</v-icon> Tout réduire
        </v-btn>
      </v-btn-group>
    </div>

    <v-alert v-if="error" type="error" variant="outlined" class="mb-4">
      {{ error }}
    </v-alert>

    <v-alert v-if="!profiles.length && !error" type="info" class="mt-4">
      Aucun matching disponible.
    </v-alert>

    <div v-for="group in filteredGroups" :key="group.name" class="mb-6">
      <v-card>
        <v-card-title 
          class="consultant-header d-flex align-center"
          @click="toggleExpand(group.name)"
          :class="{'cursor-pointer': true}"
        >
          <v-icon size="large" class="mr-2">mdi-account</v-icon>
          <span class="text-h5">{{ group.fullName }}</span>
          <v-spacer></v-spacer>
          <v-chip class="ml-2 mr-2" color="primary">
            {{ group.jobTitleGroups.reduce((acc, jg) => acc + jg.matchings.length, 0) }} matching{{ group.jobTitleGroups.reduce((acc, jg) => acc + jg.matchings.length, 0) > 1 ? 's' : '' }}
          </v-chip>
          <v-icon>{{ isExpanded(group.name) ? 'mdi-chevron-up' : 'mdi-chevron-down' }}</v-icon>
        </v-card-title>

        <v-expand-transition>
          <v-card-text v-if="isExpanded(group.name)">
            <div v-for="jobGroup in group.jobTitleGroups" :key="jobGroup.jobTitle" class="mb-4">
              <h3 class="text-subtitle-1 mb-2">
                <v-icon class="mr-1">mdi-briefcase</v-icon> {{ jobGroup.jobTitle }}
              </h3>

              <v-data-table
                :headers="headers"
                :items="jobGroup.matchings"
                class="elevation-1 mb-4"
                density="compact"
              >
              <template v-slot:item.rfpReference="{ item }">
                <div>
                  <a
                    :href="item.rfpUrl"
                    target="_blank"
                    rel="noopener noreferrer"
                    style="color: #1976d2; text-decoration: underline;"
                  >
                    {{ item.rfpReference }}

                  </a>

                </div>
              </template>

                <!-- Checkbox -->
                <template v-slot:item.edit="{ item }">
                  <v-checkbox v-model="item.editable" hide-details />
                </template>

                <!-- Date -->
                <template v-slot:item.publicationDate="{ item }">
                  {{ formatDate(item.publicationDate) }}
                </template>

                <!-- Score -->
                <template v-slot:item.score="{ item }">
                  <v-chip
                    :color="item.score >= 30 ? 'success' : item.score >= 10 ? 'warning' : 'error'"
                    :class="item.score >= 30 ? 'text-white' : ''"
                    size="small"
                  >
                    {{ item.score }}
                  </v-chip>
                </template>

                <!-- Comment -->
                <template v-slot:item.comment="{ item }">
                  <v-textarea
                    v-model="item.comment"
                    :disabled="!item.editable"
                    density="compact"
                    variant="outlined"
                    hide-details
                    rows="2"
                    style="width: 300px; text-align: right;"
                    class="comment-textarea"
                  />
                </template>

                <!-- Statut -->
                <template v-slot:item.statutMatching="{ item }">
                  <v-select
                    v-model="item.statutMatching"
                    :items="['Apply', 'NotApply', 'Rejected']"
                    :disabled="!item.editable"
                    variant="outlined"
                    density="compact"
                    hide-details
                    style="max-width: 150px"
                  />
                </template>
              </v-data-table>

              <!-- Bouton Sauvegarder conditionnel -->
              <div v-if="jobGroup.matchings.some(item => item.editable)">
                <v-btn size="small" color="primary" @click="saveMatchingGroup(jobGroup.matchings)">
                  Sauvegarder
                </v-btn>
              </div>
            </div>
          </v-card-text>
        </v-expand-transition>
      </v-card>
    </div>
  </v-container>
</template>

<style scoped>
.v-container { margin: auto; max-width: 1200px; }
.consultant-header { background-color: #f5f5f5; }
.cursor-pointer { cursor: pointer; }

.comment-textarea { width: 100%; max-width: 500px; min-height: 50px; }

</style>

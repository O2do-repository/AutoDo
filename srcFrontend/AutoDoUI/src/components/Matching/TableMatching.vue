<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';

interface Matching {
  jobTitle: string;
  consultantName: string;
  consultantSurname: string;
  rfpReference: string;
  score: number;
  offerDate: string;
  comment: string;
}

interface ConsultantGroup {
  name: string;
  fullName: string;
  matchings: Matching[];
}

const router = useRouter();
const profiles = ref<Matching[]>([]);
const error = ref<string | null>(null);
const search = ref('');
const expandedConsultants = ref<string[]>([]);

const headers = ref([
  { key: 'jobTitle', title: 'Titre du poste', align: 'start' as const },
  { key: 'rfpReference', title: 'Référence RFP', align: 'start' as const },
  { key: 'score', title: 'Score', align: 'end' as const },
  { key: 'comment', title: 'Commentaire', align: 'start' as const },
  { key: 'offerDate', title: "Date de l'offre", align: 'start' as const },
]);

const fetchMatchings = async () => {
  try {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/matching`);
    if (!response.ok) {
      throw new Error("Erreur lors de la récupération des matchings");
    }
    const data = await response.json();
    console.log("Données reçues :", data);
    profiles.value = Array.isArray(data) ? data : [data];
    
    // Par défaut, tous les consultants sont développés
    expandedConsultants.value = consultantGroups.value.map(group => group.name);
  } catch (err) {
    error.value = err instanceof Error ? err.message : "Une erreur est survenue";
  }
};

const formatDate = (dateString: string) => {
  const date = new Date(dateString);
  return date.toLocaleDateString('fr-FR');
};

// Grouper les matchings par consultant
const consultantGroups = computed(() => {
  const groupedData: Record<string, ConsultantGroup> = {};
  
  profiles.value.forEach(profile => {
    const key = `${profile.consultantSurname}_${profile.consultantName}`;
    const fullName = `${profile.consultantSurname} ${profile.consultantName}`;
    
    if (!groupedData[key]) {
      groupedData[key] = {
        name: key,
        fullName: fullName,
        matchings: []
      };
    }
    
    groupedData[key].matchings.push(profile);
  });
  
  // Convertir l'objet en tableau pour l'affichage
  return Object.values(groupedData);
});

// Filtrer les groupes selon la recherche
const filteredGroups = computed(() => {
  if (!search.value) return consultantGroups.value;
  
  const searchLower = search.value.toLowerCase();
  return consultantGroups.value.map(group => {
    // Filtrer les matchings dans chaque groupe
    const filteredMatchings = group.matchings.filter(matching => 
      matching.jobTitle.toLowerCase().includes(searchLower) ||
      matching.rfpReference.toLowerCase().includes(searchLower) ||
      matching.comment?.toLowerCase().includes(searchLower) ||
      matching.score.toString().includes(searchLower)
    );
    
    // Ne retourner que les groupes avec des matchings correspondants
    if (filteredMatchings.length > 0) {
      return {
        ...group,
        matchings: filteredMatchings
      };
    }
    return null;
  }).filter(Boolean) as ConsultantGroup[];
});

// Vérifier si un consultant est développé
const isExpanded = (consultantName: string) => {
  return expandedConsultants.value.includes(consultantName);
};

// Basculer l'état développé/réduit d'un consultant
const toggleExpand = (consultantName: string) => {
  if (isExpanded(consultantName)) {
    expandedConsultants.value = expandedConsultants.value.filter(name => name !== consultantName);
  } else {
    expandedConsultants.value.push(consultantName);
  }
};

// Développer tous les consultants
const expandAll = () => {
  expandedConsultants.value = consultantGroups.value.map(group => group.name);
};

// Réduire tous les consultants
const collapseAll = () => {
  expandedConsultants.value = [];
};

onMounted(fetchMatchings);
</script>

<template>
  <v-container>
    <h1 class="text-center mb-6">Liste des Matchings</h1>
    
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
          <v-icon>mdi-unfold-more-horizontal</v-icon>
          Tout développer
        </v-btn>
        <v-btn @click="collapseAll" size="small" variant="outlined">
          <v-icon>mdi-unfold-less-horizontal</v-icon>
          Tout réduire
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
            {{ group.matchings.length }} matching{{ group.matchings.length > 1 ? 's' : '' }}
          </v-chip>
          <v-icon>{{ isExpanded(group.name) ? 'mdi-chevron-up' : 'mdi-chevron-down' }}</v-icon>
        </v-card-title>
        
        <v-expand-transition>
          <v-card-text v-if="isExpanded(group.name)">
            <v-data-table
              :headers="headers"
              :items="group.matchings"
              class="elevation-1"
              density="compact"
            >
              <!-- Formatter la date -->
              <template v-slot:item.offerDate="{ item }">
                {{ formatDate(item.offerDate) }}
              </template>
              
              <!-- Afficher le score avec une couleur selon la valeur -->
              <template v-slot:item.score="{ item }">
                <v-chip
                  :color="item.score >= 30 ? 'success' : item.score >= 10 ? 'warning' : 'error'"
                  :class="item.score >= 30 ? 'text-white' : ''"
                  size="small"
                >
                  {{ item.score }}
                </v-chip>
              </template>
              
              <!-- Afficher le commentaire ou un message par défaut -->
              <template v-slot:item.comment="{ item }">
                {{ item.comment || 'Aucun commentaire' }}
              </template>
            </v-data-table>
          </v-card-text>
        </v-expand-transition>
      </v-card>
    </div>
  </v-container>
</template>

<style scoped>
.v-container {
  margin: auto;
  max-width: 1200px;
}

.v-data-table {
  margin-bottom: 0;
}

.consultant-header {
  background-color: #f5f5f5;
}

.cursor-pointer {
  cursor: pointer;
}

.gap-2 {
  gap: 8px;
}
</style>
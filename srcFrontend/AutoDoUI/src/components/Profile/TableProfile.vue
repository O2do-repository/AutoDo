<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import DeleteProfile from "./DeleteProfile.vue";

interface Profile {
  profileUuid: string;
  consultantUuid: string;
  RateHour: number;
  cv: string;
  CVDate: string;
  JobTitle: string;
  ExperienceLevel: string;
  skills: string[];
  keywords: string[];
}

interface Consultant {
  consultantUuid: string;
}

const router = useRouter();
const profiles = ref<Profile[]>([]);
const error = ref<string | null>(null);
const search = ref('');
const snackbar = ref(false);
const snackbarMessage = ref('');
const snackbarColor = ref('');
const consultantUuid = ref<string | null>(null);
const consultant = ref<Consultant | null>(null);

// Récupère les données du consultant depuis sessionStorage
onMounted(() => {
  const storedData = sessionStorage.getItem("selectedConsultant");
  if (storedData) {
    consultant.value = JSON.parse(storedData);
    fetchProfiles(); // Récupère les profils dès que le consultant est récupéré
  } else {
    error.value = "Aucun consultant sélectionné.";
  }
});

const goToAddProfile = () => {
  if (consultant.value) {
    // Stocke l'UUID du consultant dans sessionStorage
    sessionStorage.setItem('selectedConsultantUuid', consultant.value.consultantUuid);

    // Redirige vers la page d'ajout de profil
    router.push('/profile/add-profile');
  } else {
    error.value = "Aucun consultant sélectionné.";
  }
};

const handleProfileDeleted = ({ uuid, message }: { uuid: string; message: string }) => {
  if (uuid) {
    // Remove the deleted profile from the profiles list
    profiles.value = profiles.value.filter(p => p.profileUuid !== uuid);
    snackbarColor.value = 'success';
  } else {
    // This is an error message
    snackbarColor.value = 'error';
  }
  
  snackbarMessage.value = message;
  snackbar.value = true;
  
  // Refresh profiles only on successful deletion
  if (uuid) {
    fetchProfiles();
  }
};

const headers = ref([
  { key: 'jobTitle', title: 'Titre du poste', align: 'start' as const },
  { key: 'experienceLevel', title: 'Expérience', align: 'start' as const },
  { key: 'rateHour', title: 'Tarif / heure', align: 'end' as const },
  { key: 'cv', title: 'Lien CV', align: 'center' as const },
  { key: 'cvDate', title: 'Date CV', align: 'end' as const },
  { key: 'skills', title: 'Compétences', align: 'start' as const },
  { key: 'keywords', title: 'Mots-clés', align: 'start' as const },
  { key: 'actions', title: 'Modifier', align: 'center' as const },
  { key: 'actionsSupp', title: 'Supprimer', align: 'center' as const }
]);

// get profils by consultantUuid
const fetchProfiles = async () => {
  if (!consultant.value) {
    error.value = "Aucun consultant trouvé.";
    return;
  }

  try {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/profil/consultant/${consultant.value.consultantUuid}`);
    if (!response.ok) throw new Error('Erreur lors de la récupération des profils');
    
    const { data } = await response.json(); 
    profiles.value = data; 
  } catch (err) {
    error.value = err instanceof Error ? err.message : "Une erreur est survenue";
  }
};

// Handle button to edit profil
const editProfile = (profile: Profile) => {
  localStorage.setItem('selectedProfile', JSON.stringify(profile));
  router.push({ path: `/profile/edit-profile` });
};
</script>
<template>
  <v-container>
    <h1 class="text-center mb-6">Liste des Profils</h1>

    <v-btn @click="goToAddProfile" icon="mdi-plus" color="primary"  class="mb-4"></v-btn>

    <v-alert v-if="error" type="error" variant="outlined" class="mb-4">
      {{ error }}
    </v-alert>

    <v-text-field
      v-model="search"
      label="Rechercher un profil"
      prepend-inner-icon="mdi-magnify"
      variant="outlined"
      hide-details
      single-line
      class="mb-4"
    ></v-text-field>

    <v-data-table :headers="headers" :items="profiles" :search="search" item-value="consultantUuid" class="elevation-2">
      <template v-slot:item.cv="{ item }">
        <a v-if="item.cv && item.cv.trim() !== ''" :href="item.cv" target="_blank">Voir le CV</a>
        <span v-else>Non disponible</span>
      </template>
      <template v-slot:item.skills="{ item }">
        {{ item.skills?.length ? item.skills.join(', ') : 'Aucune compétence' }}
      </template>

      <template v-slot:item.keywords="{ item }">
        {{ item.keywords?.length ? item.keywords.join(', ') : 'Aucun mot-clé' }}
      </template>

      <template v-slot:item.actions="{ item }">
        <v-btn color="primary" @click="editProfile(item)" icon="mdi-pencil" density="comfortable"></v-btn>
      </template>
      <template v-slot:item.actionsSupp="{ item }">
        <DeleteProfile :profileUuid="item.profileUuid" @profileDeleted="handleProfileDeleted" />
      </template>
    </v-data-table>

    <v-alert v-if="!profiles.length && !error" type="info" class="mt-4">
      Aucun profil disponible.
    </v-alert>
  </v-container>

  <v-snackbar v-model="snackbar" :color="snackbarColor" timeout="3000">
    {{ snackbarMessage }}
  </v-snackbar>
</template>

<style scoped>
.v-container {
  margin: auto;
}

a {
  text-decoration: none;
  color: #1976d2;
  font-weight: bold;
}

a:hover {
  text-decoration: underline;
}
</style>

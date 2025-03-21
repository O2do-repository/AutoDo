<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import DeleteProfile from "./DeleteProfile.vue";



interface Profile {
  profileUuid: string,
  consultantUuid: string;
  RateHour: number;
  CV: string;
  CVDate: string;
  JobTitle: string;
  ExperienceLevel: string;
  skills: string[];
  keywords: string[];
}


const router = useRouter();
const profiles = ref<Profile[]>([]);
const error = ref<string | null>(null);
const search = ref('');

const snackbar = ref(false);
const snackbarMessage = ref('');
const snackbarColor = ref('');


const handleProfileDeleted = (payload: { message: string; color: string }) => {
  snackbarMessage.value = payload.message;
  snackbarColor.value = payload.color;
  snackbar.value = true;
  fetchProfiles(); // Rafraîchir la liste après suppression
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

const fetchProfiles = async () => {
  try {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/profil`);
    if (!response.ok) {
      throw new Error("Erreur lors de la récupération des profils");
    }
    const data = await response.json();
    console.log("Données reçues :", data); 
    profiles.value = Array.isArray(data) ? data : [data];
  } catch (err) {
    error.value = err instanceof Error ? err.message : "Une erreur est survenue";
  }
};

const editProfile = (profile: Profile) => {
  localStorage.setItem('selectedProfile', JSON.stringify(profile));
  router.push({ path: `/edit-profile/${profile.profileUuid}` });
};



onMounted(fetchProfiles);
</script>

<template>
  <v-container>
    <h1 class="text-center mb-6">Liste des Profils</h1>
    
    <v-btn  @click="$router.push('/add-profile')" color="primary" class="mb-4" >Ajouter un profil</v-btn>

    <v-alert v-if="error" type="error" variant="outlined" class="mb-4">
      {{ error }}
    </v-alert>

    <v-text-field v-model="search" label="Rechercher un profil" prepend-inner-icon="mdi-magnify" variant="outlined" hide-details single-line class="mb-4"></v-text-field>

    <v-data-table :headers="headers" :items="profiles" :search="search" item-value="consultantUuid" class="elevation-2">
      <template v-slot:item.CV="{ item }">
        <a v-if="item.CV" :href="item.CV" target="_blank">Voir le CV</a>
        <span v-else>Non disponible</span>
      </template>

      <template v-slot:item.skills="{ item }">
        {{ item.skills && item.skills.length ? item.skills.join(', ') : 'Aucune compétence' }}
      </template>

      <template v-slot:item.keywords="{ item }">
        {{ item.keywords && item.keywords.length ? item.keywords.join(', ') : 'Aucun mot-clé' }}
      </template>

      <template v-slot:item.actions="{ item }">
        <v-btn color="primary" @click="editProfile(item)" icon="mdi-pencil" density="comfortable"></v-btn>
      </template>
      <template v-slot:item.actionsSupp="{ item }">
        <DeleteProfile
          :profileUuid="item.profileUuid"
          @profileDeleted="handleProfileDeleted"
        />
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

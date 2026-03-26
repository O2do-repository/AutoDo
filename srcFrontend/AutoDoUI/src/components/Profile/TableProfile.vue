<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import DeleteProfile from "./DeleteProfile.vue";
import { fetchWithApiKey } from '@/utils/fetchWithApiKey';

interface Skill {
  name: string;
}
interface Keyword {
  name: string;
}
interface Profile {
  profileUuid: string;
  consultantUuid: string;
  RateHour: number;
  cv: string;
  CVDate: string;
  JobTitle: string;
  ExperienceLevel: string;
  skills: Skill[];
  keywords: Keyword[];
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
const consultant = ref<Consultant | null>(null);

onMounted(() => {
  const storedData = sessionStorage.getItem("selectedConsultant");
  if (storedData) {
    consultant.value = JSON.parse(storedData);
    fetchProfiles();
  } else {
    error.value = "No consultant selected.";
  }
});

const goToAddProfile = () => {
  if (consultant.value) {
    sessionStorage.setItem('selectedConsultantUuid', consultant.value.consultantUuid);
    router.push('/profile/add-profile');
  } else {
    error.value = "No consultant selected.";
  }
};

const handleProfileDeleted = ({ uuid, message }: { uuid: string; message: string }) => {
  if (uuid) {
    profiles.value = profiles.value.filter(p => p.profileUuid !== uuid);
    snackbarColor.value = 'success';
    fetchProfiles();
  } else {
    snackbarColor.value = 'error';
  }
  snackbarMessage.value = message;
  snackbar.value = true;
};

const headers = ref([
  { key: 'jobTitle', title: 'Job Title', align: 'start' as const },
  { key: 'experienceLevel', title: 'Experience', align: 'start' as const },
  { key: 'rateHour', title: 'Rate / hour', align: 'end' as const },
  { key: 'cv', title: 'CV Link', align: 'center' as const },
  { key: 'cvDate', title: 'CV Date', align: 'end' as const },
  { key: 'skills', title: 'Skills', align: 'start' as const },
  { key: 'keywords', title: 'Keywords', align: 'start' as const },
  { key: 'actions', title: 'Edit', align: 'center' as const },
  { key: 'actionsSupp', title: 'Delete', align: 'center' as const }
]);

const fetchProfiles = async () => {
  if (!consultant.value) {
    error.value = "No consultant found.";
    return;
  }

  try {
    const response = await fetchWithApiKey(`${import.meta.env.VITE_API_URL}/profil/consultant/${consultant.value.consultantUuid}`);
    if (!response.ok) throw new Error('Error retrieving profiles');

    const { data } = await response.json();
    profiles.value = data;
  } catch (err) {
    error.value = err instanceof Error ? err.message : "An error occurred";
  }
};

const editProfile = (profile: Profile) => {
  localStorage.setItem('selectedProfile', JSON.stringify(profile));
  router.push({ path: `/profile/edit-profile` });
};
</script>

<template>
  <v-container>
    <h1 class="text-center mb-6">Profile List</h1>

    <v-btn @click="goToAddProfile" icon="mdi-plus" color="primary" class="mb-4"></v-btn>

    <v-alert v-if="error" type="error" variant="outlined" class="mb-4">
      {{ error }}
    </v-alert>

    <v-text-field
      v-model="search"
      label="Search a profile"
      prepend-inner-icon="mdi-magnify"
      variant="outlined"
      hide-details
      single-line
      class="mb-4"
    ></v-text-field>

    <v-data-table
      :headers="headers"
      :items="profiles"
      :search="search"
      item-value="consultantUuid"
      class="elevation-2"
    >
      <template v-slot:item.cv="{ item }">
        <a v-if="item.cv && item.cv.trim() !== ''" :href="item.cv" target="_blank">View CV</a>
        <span v-else>Not available</span>
      </template>

      <template v-slot:item.skills="{ item }">
        {{ item.skills?.length ? item.skills.map(s => s.name).join(', ') : 'No skills' }}
      </template>

      <template v-slot:item.keywords="{ item }">
        {{ item.keywords?.length ? item.keywords.map(k => k.name).join(', ') : 'No keywords' }}
      </template>

      <template v-slot:item.actions="{ item }">
        <v-btn color="primary" @click="editProfile(item)" icon="mdi-pencil" density="comfortable"></v-btn>
      </template>

      <template v-slot:item.actionsSupp="{ item }">
        <DeleteProfile :profileUuid="item.profileUuid" @profileDeleted="handleProfileDeleted" />
      </template>
    </v-data-table>

    <v-alert v-if="!profiles.length && !error" type="info" class="mt-4">
      No profiles available.
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
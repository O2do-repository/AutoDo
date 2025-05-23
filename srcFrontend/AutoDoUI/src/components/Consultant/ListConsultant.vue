<template>
  <v-container>
    <v-row justify="center" class="mb-4">
    <v-col cols="12" md="6">
      <v-text-field
        v-model="search"
        label="Rechercher un consultant"
        prepend-inner-icon="mdi-magnify"
        clearable
        variant="outlined"
      />
    </v-col>
  </v-row>

    <v-row justify="center">
      <v-col cols="12" class="text-center mb-6">
        <h2 class="purple--text">A perfect blend of genius and caffeine</h2>
      </v-col>

      <v-col
      v-for="consultant in filteredConsultants"
      :key="consultant.consultantUuid"
      cols="12"
      sm="4"
      md="2"
      class="text-center"
      >
        <v-card 
          class="consultant-card"
          elevation="3"
          @click="goToConsultantProfiles(consultant)"
          :color="consultant.enterprise === 'O2do' ? 'primary' : undefined"
        >
        <DeleteConsultant
          :consultantUuid="consultant.consultantUuid"
          @click.stop
          @consultantDeleted="handleConsultantDeleted"
        />


          <v-card-text  class="d-flex flex-column align-center justify-center text-truncate text-center">
            <!-- Avatar avec image-->
            <v-avatar size="150">
              <v-img 
                :src="consultant.picture ? consultant.picture : defaultImage" 
                alt="Photo Consultant"
                contain

              ></v-img>
            </v-avatar>


            <div class="consultant-name">{{ consultant.name }} {{ consultant.surname }}</div>
            <div class="consultant-email text-caption grey--text">{{ consultant.email }}</div>
          </v-card-text>
        </v-card>
      </v-col>

      <!-- Ajouter un Consultant -->
      <v-col cols="12" sm="4" md="2" class="text-center">
        <v-card outlined class="consultant-card d-flex flex-column align-center justify-center">
          <v-btn @click="$router.push('/consultant/add-consultant')"  icon color="primary">
            <v-icon large>mdi-plus-circle</v-icon>
          </v-btn>
        </v-card>
      </v-col>
    </v-row>

    <!-- Loading & Error -->
    <v-row justify="center" v-if="loading">
      <v-col cols="12" class="text-center">
        <v-progress-circular indeterminate color="primary" />
      </v-col>
    </v-row>
    <v-row justify="center" v-if="error">
      <v-col cols="12" class="text-center red--text">
        {{ error }}
      </v-col>
    </v-row>
    <v-snackbar v-model="snackbar" :color="snackbarColor" timeout="3000">
      {{ snackbarMessage }}
    </v-snackbar>

  </v-container>
</template>

<script lang="ts">
import { defineComponent, ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import DeleteConsultant from '@/components/Consultant/DeleteConsultant.vue';
import { fetchWithApiKey } from '@/utils/fetchWithApiKey'


interface Consultant {
  consultantUuid: string;
  email: string;
  name: string;
  surname: string;
  availabilityDate: string;
  expirationDateCI: string;
  intern: boolean;
  copyCI: string;
  picture: string;
  enterprise: string;
  phone: string;
}

export default defineComponent({
  name: 'ConsultantGrid',
  setup() {
    const consultants = ref<Consultant[]>([]);
    const loading = ref(false);
    const error = ref<string | null>(null);
    const router = useRouter();
    const snackbar = ref(false);
    const snackbarMessage = ref('');
    const snackbarColor = ref('green');


    // Image par défaut
    const defaultImage = 'https://via.placeholder.com/150?text=No+Image';

    const search = ref('');
    const filteredConsultants = computed(() => {
      const term = search.value.toLowerCase();

      return consultants.value
        .filter((c) =>  
          c.name.toLowerCase().includes(term) ||
          c.surname.toLowerCase().includes(term) ||
          c.email.toLowerCase().includes(term)
        )
        .sort((a, b) => a.name.toLowerCase().localeCompare(b.name || '', 'fr', {
        sensitivity: 'base',
        ignorePunctuation: true,
      }));
    });



    const fetchConsultants = async () => {
    loading.value = true;
    try {
      const apiKey = localStorage.getItem('apiKey');

      
      const response = await fetchWithApiKey(`${import.meta.env.VITE_API_URL}/consultant`, {
        method: 'GET',

      });
      
      if (!response.ok) throw new Error('Failed to fetch consultants');

      const data = await response.json();
      if (data.success && Array.isArray(data.data)) {
        consultants.value = data.data; 
      } else {
        throw new Error('Invalid data format');
      }
    } catch (err: any) {
      error.value = err.message || 'Unknown error';
    } finally {
      loading.value = false;
    }
  };

  const handleConsultantDeleted = ({ uuid, message }: { uuid: string; message: string }) => {
    consultants.value = consultants.value.filter(c => c.consultantUuid !== uuid);
    snackbarMessage.value = message;
    snackbarColor.value = 'green';
    snackbar.value = true;
  };


    const deleteConsultant = async (uuid: string) => {
      try {
        const response = await fetchWithApiKey(`${import.meta.env.VITE_API_URL}/consultant/${uuid}`, {
          method: 'DELETE',
        });

        if (!response.ok) throw new Error('Erreur lors de la suppression');

        consultants.value = consultants.value.filter((c) => c.consultantUuid !== uuid);
      } catch (err: any) {
        console.error(err.message);
      }
    };

    const goToConsultantProfiles = (consultant: Consultant) => {
      sessionStorage.setItem("selectedConsultant", JSON.stringify(consultant));
      router.push("/consultant/consultant-info");
    };

      
    const setPlaceholder = (event: Event) => {
  (event.target as HTMLImageElement).src = defaultImage;
};

    onMounted(() => {
      fetchConsultants();
    });

    return {
      consultants,
      loading,
      error,
      snackbar,
      snackbarMessage,
      snackbarColor,
      deleteConsultant,
      goToConsultantProfiles,
      defaultImage,
      setPlaceholder,
      handleConsultantDeleted,
      search,
      filteredConsultants
    };
  },
});
</script>

<style scoped>
.v-card.consultant-card {
  border-radius: 20px;
  height: 250px;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  cursor: pointer;
  transition: transform 0.2s ease-in-out;
}

.v-card.consultant-card:hover {
  transform: scale(1.05);
}

.v-avatar {
  background-color: #f5f5f5;
}

.consultant-image {
  width: 100%;
  height: auto;
  object-fit: cover;
  border-radius: 10px;
}

.v-card-actions {
  margin-top: auto;
}

.delete-btn {
  position: absolute;
  top: 5px;
  right: 5px;
}
.consultant-name {
  max-width: 100%;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  font-weight: bold;
}

.consultant-email {
  max-width: 100%;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

</style>

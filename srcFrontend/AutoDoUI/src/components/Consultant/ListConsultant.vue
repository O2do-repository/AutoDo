<template>
  <v-container>
    <v-row justify="center">
      <v-col cols="12" class="text-center mb-6">
        <h2 class="purple--text">A perfect blend of genius and caffeine</h2>
      </v-col>

      <v-col
        v-for="consultant in consultants"
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
        >
          <!-- Bouton de suppression -->
          <v-btn
            icon
            size="x-small"
            color="red"
            class="delete-btn"
            @click.stop="deleteConsultant(consultant.consultantUuid)"
          >
            <v-icon>mdi-close</v-icon>
          </v-btn>

          <v-card-text class="d-flex flex-column align-center justify-center">
            <!-- Avatar avec image-->
            <v-avatar size="150">
              <v-img 
                :src="consultant.picture ? consultant.picture : defaultImage" 
                alt="Photo Consultant"
                contain

              ></v-img>
            </v-avatar>


            <div>{{ consultant.name }} {{ consultant.surname }}</div>
            <div class="text-caption grey--text">{{ consultant.email }}</div>
          </v-card-text>
        </v-card>
      </v-col>

      <!-- Ajouter un Consultant -->
      <v-col cols="12" sm="4" md="2" class="text-center">
        <v-card outlined class="consultant-card d-flex flex-column align-center justify-center">
          <v-btn @click="$router.push('/consultant/add-consultant')" icon color="primary">
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
  </v-container>
</template>

<script lang="ts">
import { defineComponent, ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';

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
  enterprise: number;
  phone: string;
}

export default defineComponent({
  name: 'ConsultantGrid',
  setup() {
    const consultants = ref<Consultant[]>([]);
    const loading = ref(false);
    const error = ref<string | null>(null);
    const router = useRouter();

    // Image par défaut
    const defaultImage = 'https://via.placeholder.com/150?text=No+Image';

    const fetchProfiles = async () => {
      loading.value = true;
      try {
        const response = await fetch(`${import.meta.env.VITE_API_URL}/consultant`);
        if (!response.ok) throw new Error('Failed to fetch consultants');
        const data = await response.json();
        consultants.value = data;
      } catch (err: any) {
        error.value = err.message || 'Unknown error';
      } finally {
        loading.value = false;
      }
    };

    const deleteConsultant = async (uuid: string) => {
      try {
        const response = await fetch(`${import.meta.env.VITE_API_URL}/consultant/${uuid}`, {
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
      router.push("/consultant/table-profile");
    };

      
    const setPlaceholder = (event: Event) => {
  (event.target as HTMLImageElement).src = defaultImage;
};

    onMounted(() => {
      fetchProfiles();
    });

    return {
      consultants,
      loading,
      error,
      deleteConsultant,
      goToConsultantProfiles,
      defaultImage,
      setPlaceholder
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
</style>

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
          <v-card class="consultant-card" elevation="3">
            <v-card-text class="d-flex flex-column align-center justify-center">
              <v-avatar size="80" class="my-4">
                <v-icon size="80">mdi-account</v-icon>
              </v-avatar>
              <div>{{ consultant.name }} {{ consultant.surname }}</div>
              <div class="text-caption grey--text">{{ consultant.email }}</div>
            </v-card-text>
            <v-card-actions class="justify-center mt-auto mb-2">
              <v-btn small color="red" @click="deleteConsultant(consultant.consultantUuid)">delete</v-btn>
              <v-btn small color="blue">Edit</v-btn>
            </v-card-actions>
          </v-card>
        </v-col>
  
        <!-- Add Consultant Card -->
        <v-col cols="12" sm="4" md="2" class="text-center">
          <v-card outlined class="consultant-card d-flex flex-column align-center justify-center">
            <v-btn icon color="primary">
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

interface Consultant {
  consultantUuid: string;
  email: string;
  availabilityDate: string;
  expirationDateCI: string;
  intern: boolean;
  name: string;
  surname: string;
  enterprise: number;
  phone: string;
}
export default defineComponent({
  name: 'ConsultantGrid',
  setup() {
    const consultants = ref<Consultant[]>([]);
    const loading = ref(false);
    const error = ref<string | null>(null);

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

    const deleteConsultant = (uuid: string) => {
      consultants.value = consultants.value.filter((c) => c.consultantUuid !== uuid);
    };

    onMounted(() => {
      fetchProfiles();
    });

    return {
      consultants,
      loading,
      error,
      deleteConsultant,
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
}

.v-avatar {
  background-color: #f5f5f5;
}

.v-card-actions {
  margin-top: auto;
}
</style>

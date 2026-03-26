<script setup lang="ts">
import { fetchWithApiKey } from '@/utils/fetchWithApiKey';
import { ref, onMounted } from 'vue'

interface Enterprise {
  enterpriseUuid: string;
  name: string;
}

const newEnterprise = ref<string>('')
const enterprises = ref<Enterprise[]>([])
const selectedEnterpriseIndex = ref<number | null>(null)
const error = ref<string | null>(null);
const success = ref(false);

const dialog = ref(false)
const loading = ref(false)
const snackbar = ref(false)
const snackbarMessage = ref('')
const snackbarColor = ref('')

const fetchEnterprises = async () => {
  try {
    const response = await fetchWithApiKey(`${import.meta.env.VITE_API_URL}/enterprise`);
    const resData = await response.json();

    if (!response.ok || !resData.success) {
      throw new Error(resData.message || "Error while fetching enterprises");
    }

    enterprises.value = resData.data;
  } catch (error) {
    console.error(error instanceof Error ? error.message : 'An error occurred');
  }
}

const submitEnterprise = async () => {
  const name = newEnterprise.value.trim();
  if (!name) return;

  loading.value = true;
  error.value = null;
  success.value = false;

  try {
    const response = await fetchWithApiKey(`${import.meta.env.VITE_API_URL}/enterprise`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ name })
    });

    const result = await response.json();

    if (!response.ok || !result.success) {
      throw new Error(result.message);
    }

    enterprises.value.push(result.data);
    newEnterprise.value = '';
    success.value = true;
    snackbarMessage.value = result.message;
    snackbarColor.value = 'green';
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'An error occurred';
    snackbarMessage.value = error.value;
    snackbarColor.value = 'red';
  } finally {
    snackbar.value = true;
    loading.value = false;
  }
};

const confirmDeleteEnterprise = (index: number) => {
  selectedEnterpriseIndex.value = index
  dialog.value = true
}

const removeEnterprise = async (uuid: string) => {
  loading.value = true;
  error.value = null;
  success.value = false;

  try {
    const response = await fetchWithApiKey(`${import.meta.env.VITE_API_URL}/enterprise/${uuid}`, {
      method: 'DELETE'
    });

    const result = await response.json();

    if (!response.ok || !result.success) {
      throw new Error(result.message);
    }

    enterprises.value = enterprises.value.filter(e => e.enterpriseUuid !== uuid);
    success.value = true;
    snackbarMessage.value = result.message;
    snackbarColor.value = 'green';
    dialog.value = false;
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'An error occurred';
    snackbarMessage.value = error.value;
    snackbarColor.value = 'red';
  } finally {
    snackbar.value = true;
    loading.value = false;
  }
};

onMounted(fetchEnterprises)
</script>

<template>
  <v-container>
    <v-row class="mt-5">
      <v-col cols="4">
        <v-text-field 
            v-model="newEnterprise" 
            label="Add a company" 
            variant="outlined"
            @keyup.enter="submitEnterprise"
            hide-details
            density="comfortable"
        />
      </v-col>
      <v-col cols="2">
        <v-btn 
          color="primary" 
          @click="submitEnterprise"
          icon
          variant="text"
          :disabled="!newEnterprise.trim() || loading"
          :loading="loading"
        >
          <template v-slot:loader>
            <v-progress-circular indeterminate color="Primary" size="20"></v-progress-circular>
          </template>
          <v-icon large>mdi-plus-circle</v-icon>
        </v-btn>

      </v-col>
      <v-col cols="6">
        <v-card
          elevation="3"
          rounded="lg"
          class="skills-card"
        >
          <v-card-title class="text-h6 py-3 px-4 bg-primary text-white d-flex align-center">
            Companies
            <v-chip class="ms-2" size="small" color="white" text-color="primary">{{ enterprises.length }}</v-chip>
          </v-card-title>
          
          <div class="scroll-container">
            <v-list density="compact" bg-color="transparent">
              <v-list-item
                v-for="(enterprise, index) in enterprises"
                :key="enterprise.enterpriseUuid"
                class="skill-item"
                :class="{ 'even-item': index % 2 === 0 }"
              >
                <v-list-item-title>{{ enterprise.name }}</v-list-item-title>
                
                <template v-slot:append>
                  <v-btn
                    icon="mdi-delete"
                    variant="text"
                    color="error"
                    size="small"
                    @click="confirmDeleteEnterprise(index)"
                  ></v-btn>
                </template>
              </v-list-item>
            </v-list>

            <!-- Confirmation dialog -->
            <v-dialog v-model="dialog" max-width="500">
              <v-card>
                <v-card-title class="headline">Confirm deletion</v-card-title>
                <v-card-text>
                  Are you sure you want to delete this company?
                </v-card-text>
                <v-card-actions>
                  <v-spacer></v-spacer>
                  <v-btn color="grey" @click="dialog = false">Cancel</v-btn>
                  <v-btn
                    color="red"
                    :loading="loading"
                    @click="() => selectedEnterpriseIndex !== null && removeEnterprise(enterprises[selectedEnterpriseIndex].enterpriseUuid)"
                  >
                    Delete
                  </v-btn>

                </v-card-actions>
              </v-card>
            </v-dialog>

            <!-- Snackbar -->
            <v-snackbar v-model="snackbar" :color="snackbarColor" timeout="3000">
              {{ snackbarMessage }}
            </v-snackbar>
          </div>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<style scoped>
.scroll-container {
  max-height: 250px;
  overflow-y: auto;
  scrollbar-width: thin;
}

.scroll-container::-webkit-scrollbar {
  width: 6px;
}

.scroll-container::-webkit-scrollbar-thumb {
  background-color: rgba(0, 0, 0, 0.2);
  border-radius: 3px;
}

.skill-item {
  transition: background-color 0.2s;
}

.skill-item:hover {
  background-color: rgba(0, 0, 0, 0.05);
}

.even-item {
  background-color: rgba(0, 0, 0, 0.02);
}
</style>
  
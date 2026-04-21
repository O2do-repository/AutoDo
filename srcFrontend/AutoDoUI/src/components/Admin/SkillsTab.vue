<script setup lang="ts">
import { fetchWithApiKey } from '@/utils/fetchWithApiKey';
import { ref, computed, onMounted } from 'vue'

interface Skill {
  skillUuid: string;
  name: string;
}

const newSkill = ref<string>('')
const skills = ref<Skill[]>([])
const error = ref<string | null>(null)
const success = ref(false)

const loading = ref(false)
const snackbar = ref(false)
const snackbarMessage = ref('')
const snackbarColor = ref('')
const dialog = ref(false)
const selectedSkillUuid = ref<string | null>(null)

const filteredSkills = computed(() => {
  const q = newSkill.value.trim().toLowerCase()
  if (!q) return skills.value
  return skills.value.filter(s => s.name.toLowerCase().includes(q))
})

const isDuplicate = computed(() =>
  skills.value.some(s => s.name.toLowerCase() === newSkill.value.trim().toLowerCase())
)

const fetchSkills = async () => {
  try {
    const response = await fetchWithApiKey(`${import.meta.env.VITE_API_URL}/skill`)
    const resData = await response.json()
    if (!response.ok || !resData.success) throw new Error(resData.message || "Error while fetching skills")
    skills.value = resData.data
  } catch (error) {
    console.error(error instanceof Error ? error.message : 'An error occurred')
  }
}

const submitSkill = async () => {
  const name = newSkill.value.trim()
  if (!name || isDuplicate.value) return

  loading.value = true
  error.value = null

  try {
    const response = await fetchWithApiKey(`${import.meta.env.VITE_API_URL}/skill`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ name })
    })

    const result = await response.json()
    if (!response.ok || !result.success) throw new Error(result.message)

    skills.value.push(result.data)
    newSkill.value = ''
    snackbarMessage.value = result.message
    snackbarColor.value = 'green'
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'An error occurred'
    snackbarMessage.value = error.value
    snackbarColor.value = 'red'
  } finally {
    snackbar.value = true
    loading.value = false
  }
}

const confirmDeleteSkill = (uuid: string) => {
  selectedSkillUuid.value = uuid
  dialog.value = true
}

const removeSkill = async () => {
  if (!selectedSkillUuid.value) return
  loading.value = true

  try {
    const response = await fetchWithApiKey(`${import.meta.env.VITE_API_URL}/skill/${selectedSkillUuid.value}`, {
      method: 'DELETE'
    })

    const result = await response.json()
    if (!response.ok || !result.success) throw new Error(result.message)

    skills.value = skills.value.filter(s => s.skillUuid !== selectedSkillUuid.value)
    snackbarMessage.value = result.message
    snackbarColor.value = 'green'
    dialog.value = false
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'An error occurred'
    snackbarMessage.value = error.value
    snackbarColor.value = 'red'
  } finally {
    snackbar.value = true
    loading.value = false
  }
}

onMounted(fetchSkills)
</script>

<template>
  <v-container>
    <v-row class="mt-5">
      <v-col cols="12" md="10" class="mx-auto">

        <div class="d-flex align-center gap-2 mb-4">
          <v-text-field
            v-model="newSkill"
            label="Add or filter skills"
            variant="outlined"
            hide-details
            density="comfortable"
            clearable
            @keyup.enter="submitSkill"
            :error="isDuplicate"
          />
          <v-btn
            color="primary"
            icon
            variant="text"
            :disabled="!newSkill.trim() || isDuplicate || loading"
            :loading="loading"
            @click="submitSkill"
          >
            <template v-slot:loader>
              <v-progress-circular indeterminate color="primary" size="20" />
            </template>
            <v-icon large>mdi-plus-circle</v-icon>
          </v-btn>
        </div>

        <div class="d-flex align-center justify-space-between mb-2">
          <span class="text-body-2 text-medium-emphasis">
            {{ filteredSkills.length }} / {{ skills.length }} skills
          </span>
          <v-chip v-if="isDuplicate" color="warning" size="small" variant="tonal">
            Already exists
          </v-chip>
        </div>

        <div class="chips-container">
          <TransitionGroup name="chip">
            <v-chip
              v-for="skill in filteredSkills"
              :key="skill.skillUuid"
              closable
              variant="tonal"
              color="primary"
              style="min-width: 120px; justify-content: space-between;"
              @click:close="confirmDeleteSkill(skill.skillUuid)"
            >
              {{ skill.name }}
            </v-chip>
          </TransitionGroup>

          <div v-if="filteredSkills.length === 0" class="text-center text-medium-emphasis py-4 text-body-2">
            No skills match "{{ newSkill }}"
          </div>
        </div>

      </v-col>
    </v-row>

    <v-dialog v-model="dialog" max-width="400">
      <v-card>
        <v-card-title>Confirm deletion</v-card-title>
        <v-card-text>Are you sure you want to delete this skill?</v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn color="grey" @click="dialog = false">Cancel</v-btn>
          <v-btn color="red" :loading="loading" @click="removeSkill">Delete</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-snackbar v-model="snackbar" :color="snackbarColor" timeout="3000">
      {{ snackbarMessage }}
    </v-snackbar>
  </v-container>
</template>

<style scoped>
.chips-container {
  min-height: 60px;
  padding: 12px;
  border: 0.5px solid rgba(0,0,0,0.12);
  border-radius: 8px;
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.chip-enter-active, .chip-leave-active {
  transition: all 0.2s ease;
}
.chip-enter-from, .chip-leave-to {
  opacity: 0;
  transform: scale(0.8);
}
</style>
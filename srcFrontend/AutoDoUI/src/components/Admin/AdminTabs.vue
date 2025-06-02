<template>
  <v-container fluid>
    <!-- Onglets avec un peu de marge pour les descendre -->
    <v-row justify="center" class="mt-8 mb-4">
      <v-col cols="12" md="8">
        <v-tabs
          v-model="tab"
          grow
          bg-color="transparent"
          class="elevation-1 rounded"
        >
          <v-tab v-for="(item, index) in tabs" :key="index">
            {{ item.label }}
          </v-tab>
        </v-tabs>
      </v-col>
    </v-row>

    <!-- Contenu des onglets -->
    <v-row justify="center">
      <v-col cols="12" md="10" lg="8">
        <v-window v-model="tab" class="mt-4">
          <v-window-item :value="0">
            <EntrepriseTab />
          </v-window-item>
          <v-window-item :value="1">
            <SkillsTab />
          </v-window-item>
          <v-window-item :value="2">
            <KeywordsTab />
          </v-window-item>
        </v-window>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import EntrepriseTab from './EnterpriseTab.vue'
import SkillsTab from './SkillsTab.vue'
import KeywordsTab from './KeywordsTab.vue'

interface TabItem {
  label: string
}

const tab = ref<number>(0)
const tabs: TabItem[] = [
  { label: 'Entreprise' },
  { label: 'Compétences' },
  { label: 'Mot-clés' }
]
</script>

<style scoped>
/* Plus besoin de .content-wrapper car on utilise v-col */
</style>

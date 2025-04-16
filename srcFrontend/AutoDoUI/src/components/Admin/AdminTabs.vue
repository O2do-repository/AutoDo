<template>
  <v-container>
    <v-tabs v-model="tab" grow bg-color="transparent">
      <v-tab v-for="(item, index) in tabs" :key="index">{{ item.label }}</v-tab>
    </v-tabs>
    
    <v-window v-model="tab">
      <v-window-item :value="0">
        <div class="d-flex justify-center">
          <div class="content-wrapper">
            <EntrepriseTab />
          </div>
        </div>
      </v-window-item>
      <v-window-item :value="1">
        <div class="d-flex justify-center">
          <div class="content-wrapper">
            <SkillsTab />
          </div>
        </div>
      </v-window-item>
      <v-window-item :value="2">
        <div class="d-flex justify-center">
          <div class="content-wrapper">
            <KeywordsTab />
          </div>
        </div>
      </v-window-item>
    </v-window>
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
.content-wrapper {
  width: 80%;
  max-width: 1000px;
}
</style>
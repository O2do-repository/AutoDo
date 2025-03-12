<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';

interface Profile {
    profileUuid: string;
    rateHour: number | null;
    cv: string;
    cvDate: string;
    jobTitle: string;
    experienceLevel: string;
    skills: string[];
    keywords: string[];
}


const storedProfile = localStorage.getItem('selectedProfile');

const profile = ref<Profile>(storedProfile ? JSON.parse(storedProfile) : {
    profileUuid: '',
    rateHour: null,
    cv: '',
    cvDate: '',
    jobTitle: '',
    experienceLevel: '',
    skills: [],
    keywords: []
});

console.log("Profil après parsing :", profile.value);

const router = useRouter();
const experienceLevels: string[] = ['Junior', 'Medior', 'Senior'];
const availableSkills: string[] = ['C#', 'ASP.NET', 'Vue.js', 'SQL'];
const availableKeywords: string[] = ['Backend', 'Frontend', 'API'];

const errorMessage = ref<string | null>(null); 

const submitProfile = async () => {

    errorMessage.value = null;

    if (!profile.value.jobTitle || !profile.value.cv || !profile.value.cvDate || !profile.value.experienceLevel) {
        errorMessage.value = 'Veuillez remplir tous les champs obligatoires.';
        return;
    }
    

    try {
        const response = await fetch(`${import.meta.env.VITE_API_URL}/profil`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(profile.value)
        });

        if (!response.ok) {
            throw new Error('Erreur lors de la mise à jour du profil');
        }

        setTimeout(() => {
            router.push('/table-profile');
        }, 1000);

    } catch (error) {
        console.error(error instanceof Error ? error.message : 'Une erreur est survenue');
    }
};
</script>

<template>
    <v-container>


        <v-alert closable v-if="errorMessage" type="error" variant="outlined">
            {{ errorMessage }}
        </v-alert>  

        <v-card class="pa-4">
            <v-card-title>Modifier Profil</v-card-title>
            <v-card-text>
                <v-text-field label="Job Title*" v-model="profile.jobTitle" required></v-text-field>
                <v-select label="Niveau d'expérience" v-model="profile.experienceLevel" :items="experienceLevels" required></v-select>
                <v-text-field label="Tarif / heure" v-model.number="profile.rateHour" type="number"></v-text-field>
                <v-text-field label="CV URL*" v-model="profile.cv" required></v-text-field>
                <v-text-field label="Date CV*" v-model="profile.cvDate" type="datetime-local" required></v-text-field>
                <v-select
                    label="Compétences"
                    v-model="profile.skills"
                    :items="availableSkills"
                    multiple
                    required
                ></v-select>
                <v-select
                    label="Mots-clés"
                    v-model="profile.keywords"
                    :items="availableKeywords"
                    multiple
                    required
                ></v-select>

            </v-card-text>
            <v-card-actions>
                <v-btn color="primary" @click="submitProfile()">Publier</v-btn>
            </v-card-actions>
        </v-card>
    </v-container>
</template>
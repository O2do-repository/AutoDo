<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import GoBackBtn from '@/components/utils/GoBackBtn.vue';

const router = useRouter();

interface Consultant {
  consultantUuid: string;
  name: string;
  surname: string;
  email: string;
  phone: string;
  enterprise: string;
  intern: boolean;
  availabilityDate: string;
  expirationDateCI: string;
  picture: string;
  copyCI: string;
}

const consultant = ref<Consultant | null>(null);

const editConsultant = () => {
  if (consultant.value) {
    sessionStorage.setItem("editConsultant", JSON.stringify(consultant.value));
    router.push("/consultant/edit-consultant");
  }
};

onMounted(() => {
  const storedData = sessionStorage.getItem("selectedConsultant");
  if (storedData) {
    consultant.value = JSON.parse(storedData);
  }
});
</script>

<template>
  <v-card v-if="consultant" class="pa-4 mb-4">
    <GoBackBtn />
    
    <v-card-title>
      Informations du Consultant
    </v-card-title>

    <v-card-text>
      <v-row>
        <v-col cols="12">
          <v-avatar size="150">
            <v-img 
              :src="consultant.picture" 
              alt="Photo Consultant"
              contain
            ></v-img>
          </v-avatar>
        </v-col>

        <v-col cols="12" sm="6">
          <strong>Nom :</strong> {{ consultant.name }} {{ consultant.surname }}
        </v-col>
        <v-col cols="12" sm="6">
          <strong>Email :</strong> {{ consultant.email }}
        </v-col>
        <v-col cols="12" sm="6">
          <strong>Téléphone :</strong> {{ consultant.phone }}
        </v-col>
        <v-col cols="12" sm="6">
          <strong>Entreprise :</strong> {{ consultant.enterprise }}
        </v-col>
        <v-col cols="12" sm="6">
          <strong>Interne :</strong> {{ consultant.intern ? 'Oui' : 'Non' }}
        </v-col>
        <v-col cols="12" sm="6">
          <strong>Date de disponibilité :</strong> {{ consultant.availabilityDate }}
        </v-col>
        <v-col cols="12" sm="6">
          <strong>Date d'expiration CI :</strong> {{ consultant.expirationDateCI }}
        </v-col>
        <v-col cols="12">
          <strong>Copie de la carte d'identité : </strong> 
          <v-btn 
            v-if="consultant.copyCI" 
            :href="consultant.copyCI" 
            prepend-icon="mdi-file" 
            target="_blank" 
            variant="outlined" 
            color="primary"
          >
            Voir le document
          </v-btn>
          <span v-else>Non disponible</span>
        </v-col>
      </v-row>
    </v-card-text>

    <v-card-actions class="d-flex justify-end">
      <v-btn color="primary" variant="tonal" @click="editConsultant">
        Modifier
      </v-btn>
    </v-card-actions>
  </v-card>
</template>

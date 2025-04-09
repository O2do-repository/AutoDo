<script setup lang="ts">
import { ref, watch, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import GoBackBtn from '@/components/utils/GoBackBtn.vue'; 

interface Consultant {
  email: string;
  availabilityDate: string;
  expirationDateCI: string;
  intern: boolean | null;
  name: string;
  surname: string;
  enterprise: string | null;
  phone: string;
  copyCI: string;
  picture: string;
}

const consultant = ref<Consultant>({
  email: '',
  availabilityDate: '',
  expirationDateCI: '',
  intern: false,
  name: '',
  surname: '',
  enterprise: null,
  phone: '',
  copyCI: '',
  picture: ''
});

const router = useRouter();

const enterprises = [
  { id: '0', name: 'O2do' },
  { id: '1', name: 'Winch' },
  { id: '2', name: 'MI6' }
];

// Validation rules
const required = (value: string) => {
  return value !== null && value !== undefined && value !== '' || 'Champ obligatoire';
};
const emailRule = (value: string) =>
  /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value) || 'E-mail invalide';
const phoneRule = (value: string) =>
  /^[0-9]{10}$/.test(value) || 'Numéro de téléphone invalide (10 chiffres)';
const dateRule = (value: string) =>
  !!value || 'Veuillez choisir une date';

// Gestion de l'upload d'image
const placeholderImage = 'https://static.vecteezy.com/system/resources/thumbnails/003/337/584/small/default-avatar-photo-placeholder-profile-icon-vector.jpg';
const validPicture = ref(placeholderImage);

const checkImage = () => {
  if (!consultant.value.picture || consultant.value.picture.trim() === '') {
    validPicture.value = placeholderImage;
    return;
  }

  const img = new Image();
  img.src = consultant.value.picture;
  img.onload = () => {
    validPicture.value = consultant.value.picture;
  };
  img.onerror = () => {
    validPicture.value = placeholderImage;
  };
};

const clearPlaceholder = () => {
  if (consultant.value.picture === placeholderImage) {
    consultant.value.picture = '';
  }
};

const restorePlaceholder = () => {
  if (!consultant.value.picture) {
    consultant.value.picture = placeholderImage;
  }
};

const setPlaceholder = () => {
  validPicture.value = placeholderImage;
};

watch(() => consultant.value.picture, checkImage);

const submitConsultant = async () => {
  try {
    if (!consultant.value.picture || consultant.value.picture.trim() === '') {
      consultant.value.picture = placeholderImage;
    }

    const response = await fetch(`${import.meta.env.VITE_API_URL}/consultant`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(consultant.value)
    });

    if (!response.ok) {
      throw new Error("Erreur lors de la modification du consultant");
    }

    setTimeout(() => {
      router.push('/consultant/list-consultant');
    }, 1000);

  } catch (error) {
    console.error(error instanceof Error ? error.message : 'Une erreur est survenue');
  }
};

onMounted(() => {
  const storedConsultant = sessionStorage.getItem("editConsultant");
  if (storedConsultant) {
    const parsedConsultant = JSON.parse(storedConsultant);

    // Corriger le format des dates pour les inputs type="date"
    if (parsedConsultant.expirationDateCI) {
      parsedConsultant.expirationDateCI = parsedConsultant.expirationDateCI.split('T')[0];
    }
    if (parsedConsultant.availabilityDate) {
      parsedConsultant.availabilityDate = parsedConsultant.availabilityDate.split('T')[0];
    }

    Object.assign(consultant.value, parsedConsultant);
    checkImage();
  }
});

</script>

<template>
  <v-container>
    <v-card class="pa-4">
      <GoBackBtn class="mb-4"/>

      <v-card-title class="text-h5 font-weight-bold">Modifier Consultant</v-card-title>
      <v-card-text>
        <v-row>
          <!-- Avatar + Image Preview -->
          <v-col cols="12" class="d-flex flex-column align-center">
            <v-avatar size="150">
              <v-img 
                :src="validPicture" 
                alt="Photo Consultant"
                contain
                @error="setPlaceholder"
              ></v-img>
            </v-avatar>
          </v-col>

          <!-- Champ de texte pour modifier l'image -->
          <v-col cols="12">
            <v-text-field 
              variant="outlined"
              color="primary"
              label="Lien Photo *" 
              v-model="consultant.picture" 
              :placeholder="placeholderImage"
              @focus="clearPlaceholder"
              @blur="restorePlaceholder"
              @input="checkImage"
            ></v-text-field>
          </v-col>

          <v-col cols="6">
            <v-text-field variant="outlined" color="primary" label="Nom *" v-model="consultant.name" :rules="[required]" required></v-text-field>
          </v-col>
          <v-col cols="6">
            <v-text-field variant="outlined" color="primary" label="Prénom *" v-model="consultant.surname" :rules="[required]" required></v-text-field>
          </v-col>

          <v-col cols="6">
            <v-text-field variant="outlined" color="primary" label="Lien Copie CI *" v-model="consultant.copyCI" :rules="[required]" required></v-text-field>
          </v-col>
          <v-col cols="6">
            <v-text-field variant="outlined" color="primary" label="Expiration CI *" v-model="consultant.expirationDateCI" type="date"></v-text-field>
          </v-col>

          <v-col cols="6">
            <v-switch variant="outlined" color="primary" label="Stagiaire *" v-model="consultant.intern"></v-switch>
          </v-col>
          <v-col cols="6">
            <v-select
              label="Entreprise *"
              v-model="consultant.enterprise"
              :items="enterprises"
              item-title="name"
              item-value="id"
              :rules="[required]"
              variant="outlined" color="primary"
            ></v-select>
          </v-col>

          <v-col cols="6">
            <v-text-field variant="outlined" color="primary" label="Email *" v-model="consultant.email" :rules="[required, emailRule]" required></v-text-field>
          </v-col>
          <v-col cols="6">
            <v-text-field variant="outlined" color="primary" label="Téléphone *" v-model="consultant.phone" :rules="[required, phoneRule]" required></v-text-field>
          </v-col>

          <v-col cols="6">
            <v-text-field variant="outlined" color="primary" label="Date de disponibilité *" v-model="consultant.availabilityDate" type="date" :rules="[dateRule]" required></v-text-field>
          </v-col>
        </v-row>
      </v-card-text>

      <v-card-actions class="d-flex justify-end">
        <v-btn color="primary" @click="submitConsultant">Modifier</v-btn>
      </v-card-actions>
    </v-card>
  </v-container>
</template>

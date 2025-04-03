<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import GoBackBtn from '@/components/utils/GoBackBtn.vue'; 

interface Consultant {
  Email: string;
  AvailabilityDate: string;
  ExpirationDateCI: string;
  Intern: boolean | null;
  Name: string;
  Surname: string;
  enterprise: number | null;
  Phone: string;
  CopyCI: string;
  Picture: string;
}

const consultant = ref<Consultant>({
  Email: '',
  AvailabilityDate: '',
  ExpirationDateCI: '',
  Intern: false,
  Name: '',
  Surname: '',
  enterprise: null,
  Phone: '',
  CopyCI: '',
  Picture: ''
});

const router = useRouter();

const enterprises = [
  { id: 0, name: 'O2do' },
  { id: 1, name: 'Winch' },
  { id: 2, name: 'MI6' }
];

// Validation rules
const required = (value: string) => {
  return value !== null && value !== undefined  && value !== '' || 'Champ obligatoire';
};
const emailRule = (value: string) =>
  /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value) || 'E-mail invalide';
const phoneRule = (value: string) =>
  /^[0-9]{10}$/.test(value) || 'Numéro de téléphone invalide (10 chiffres)';
const dateRule = (value: string) =>
  !!value || 'Veuillez choisir une date';


  // Gestion de l'upload d'image
  const placeholderImage = 'https://static.vecteezy.com/system/resources/thumbnails/003/337/584/small/default-avatar-photo-placeholder-profile-icon-vector.jpg';
// Vérification et remplacement si l'image est invalide
const validPicture = ref(placeholderImage);

const checkImage = () => {
  if (!consultant.value.Picture || consultant.value.Picture.trim() === '') {
    validPicture.value = placeholderImage;
    return;
  }

  const img = new Image();
  img.src = consultant.value.Picture;
  img.onload = () => {
    validPicture.value = consultant.value.Picture;
  };
  img.onerror = () => {
    validPicture.value = placeholderImage;
  };
};


// Supprime le placeholder du champ quand l'utilisateur clique dessus
const clearPlaceholder = () => {
  if (consultant.value.Picture === placeholderImage) {
    consultant.value.Picture = '';
  }
};

// Restaure le placeholder si le champ est vide
const restorePlaceholder = () => {
  if (!consultant.value.Picture) {
    consultant.value.Picture = placeholderImage;
  }
};

// Remplace l’image en cas d’erreur
const setPlaceholder = () => {
  validPicture.value = placeholderImage;
};

// Surveille le champ Picture et met à jour l’image
watch(() => consultant.value.Picture, checkImage);

const submitConsultant = async () => {
  try {
    // Si aucune image n'est fournie, on met le placeholder par défaut
    if (!consultant.value.Picture || consultant.value.Picture.trim() === '') {
      consultant.value.Picture = placeholderImage;
    }

    const response = await fetch(`${import.meta.env.VITE_API_URL}/consultant`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(consultant.value)
    });

    if (!response.ok) {
      throw new Error("Erreur lors de l'ajout du consultant");
    }

    setTimeout(() => {
      router.push('/consultant/list-consultant');
    }, 1000);

  } catch (error) {
    console.error(error instanceof Error ? error.message : 'Une erreur est survenue');
  }
};

</script>


<template>
  <v-container>
    <v-card class="pa-4">
      <GoBackBtn class="mb-4"/>

      <v-card-title class="text-h5 font-weight-bold">Nouveau Consultant</v-card-title>
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
              v-model="consultant.Picture" 
              :placeholder="placeholderImage"
              @focus="clearPlaceholder"
              @blur="restorePlaceholder"
              @input="checkImage"
            ></v-text-field>
          </v-col>
          <v-col cols="6">
            <v-text-field  variant="outlined" color="primary" label="Nom *" v-model="consultant.Name" :rules="[required]" required></v-text-field>
          </v-col>
          <v-col cols="6">
            <v-text-field variant="outlined" color="primary"  label="Prénom *" v-model="consultant.Surname" :rules="[required]" required></v-text-field>
          </v-col>

          <v-col cols="6">
            <v-text-field variant="outlined" color="primary"  label="Lien Copie CI *" v-model="consultant.CopyCI" :rules="[required]" required></v-text-field>
          </v-col>
          <v-col cols="6">
            <v-text-field variant="outlined" color="primary"  label="Expiration CI *" v-model="consultant.ExpirationDateCI" type="date" ></v-text-field>
          </v-col>

          <v-col cols="6">
            <v-switch variant="outlined" color="primary"  label="Stagiaire *" v-model="consultant.Intern"></v-switch>
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
            <v-text-field variant="outlined" color="primary"  label="Email *" v-model="consultant.Email" :rules="[required, emailRule]" required></v-text-field>
          </v-col>
          <v-col cols="6">
            <v-text-field variant="outlined" color="primary"  label="Téléphone *" v-model="consultant.Phone" :rules="[required, phoneRule]" required></v-text-field>
          </v-col>

          <v-col cols="6">
            <v-text-field variant="outlined" color="primary"  label="Date de disponibilité *" v-model="consultant.AvailabilityDate" type="date" :rules="[dateRule]" required></v-text-field>
          </v-col>
        </v-row>
      </v-card-text>

      <v-card-actions class="d-flex justify-end">
        <v-btn color="primary" @click="submitConsultant">Publier</v-btn>
      </v-card-actions>
    </v-card>
  </v-container>
</template>


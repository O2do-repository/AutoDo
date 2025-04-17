<script setup lang="ts">
import { ref, watch, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import GoBackBtn from '@/components/utils/GoBackBtn.vue';

// set interface
interface Consultant {
  Email: string;
  AvailabilityDate: string;
  ExpirationDateCI: string;
  Intern: boolean | null;
  Name: string;
  Surname: string;
  enterprise: string;
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
  enterprise: '',
  Phone: '',
  CopyCI: '',
  Picture: ''
});

interface Enterprise {
  enterpriseUuid: string;
  name: string;
}

const router = useRouter();

// recupération des entreprises pour le v-select
const enterprises = ref<Enterprise[]>([]);

const fetchEnterprises = async () => {
  try {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/enterprise`);
    if (!response.ok) throw new Error("Erreur lors du chargement des entreprises");

    const data = await response.json();
    enterprises.value = data;
  } catch (error) {
    console.error("Erreur fetch entreprises :", error);
  }
};

// 🧪 Validation Rules
const required = (value: string) => value?.trim() !== '' || 'Champ obligatoire';
const emailRule = (value: string) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value) || "Format invalide (ex: exemple@mail.com)";
const phoneRule = (value: string) => /^\+?[0-9]{9,15}$/.test(value) || "Format invalide (ex: +32444332211)";
const urlRule = (value: string) => /^(https?:\/\/)[^\s$.?#].[^\s]*$/.test(value) || "Lien invalide (ex: https://...)";
const imageUrlRule = (value: string) => /^(https?:\/\/.*\.(?:png|jpg|jpeg|gif|webp))$/i.test(value) || "Lien image invalide (ex: https://site.com/photo.jpg)";
const dateRule = (value: string) => !!value || 'Veuillez choisir une date';

// Si le consultant est interne alors il sera d'O2do
watch(() => consultant.value.Intern, (newVal) => {
  if (newVal) {
    consultant.value.enterprise = 'O2do';
  } else {
    consultant.value.enterprise = '';
  }
});

const formRef = ref<any>(null);

// Image de remplacement
const placeholderImage = 'https://static.vecteezy.com/system/resources/thumbnails/003/337/584/small/default-avatar-photo-placeholder-profile-icon-vector.jpg';
const validPicture = ref(placeholderImage);

// Check si l'image est valide
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

const clearPlaceholder = () => {
  if (consultant.value.Picture === placeholderImage) {
    consultant.value.Picture = '';
  }
};

const restorePlaceholder = () => {
  if (!consultant.value.Picture) {
    consultant.value.Picture = placeholderImage;
  }
};

const setPlaceholder = () => {
  validPicture.value = placeholderImage;
};

watch(() => consultant.value.Picture, checkImage);

// Post Consultant dans la database
const submitConsultant = async (event: SubmitEvent): Promise<void> => {
  try {
    if (!consultant.value.Picture || consultant.value.Picture.trim() === '') {
      consultant.value.Picture = placeholderImage;
    }

    if (!formRef.value) return;

    const { valid } = await formRef.value.validate();
    if (!valid) return;

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

onMounted(() => {
  fetchEnterprises();
});
</script>



<template>
  <v-container>
    <v-card class="pa-4">
      <GoBackBtn class="mb-4" />

      <v-card-title class="text-h5 font-weight-bold">Nouveau Consultant</v-card-title>
      <v-card-text>
        <v-form ref="formRef" >

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

            <!-- Lien de l'image -->
            <v-col cols="12">
              <v-text-field 
                variant="outlined"
                color="primary"
                label="Lien Photo *" 
                v-model="consultant.Picture" 
                :placeholder="placeholderImage"
                :rules="[required, imageUrlRule]"
                @focus="clearPlaceholder"
                @blur="restorePlaceholder"
                @input="checkImage"
              ></v-text-field>
            </v-col>

            <!-- Prénom -->
            <v-col cols="6">
              <v-text-field 
                variant="outlined" 
                color="primary"  
                label="Prénom *" 
                v-model="consultant.Surname" 
                :rules="[required]" 
                required
              ></v-text-field>
            </v-col>

            <!-- Nom -->
            <v-col cols="6">
              <v-text-field  
                variant="outlined" 
                color="primary" 
                label="Nom *" 
                v-model="consultant.Name" 
                :rules="[required]" 
                required
              ></v-text-field>
            </v-col>

            <!-- Lien de la copie de la carte d'identité -->
            <v-col cols="6">
              <v-text-field 
                variant="outlined" 
                color="primary"  
                label="Lien Copie CI *" 
                v-model="consultant.CopyCI" 
                :rules="[required, urlRule]" 
                required
              ></v-text-field>
            </v-col>

            <!-- Expiration de la CI -->
            <v-col cols="6">
              <v-text-field 
                variant="outlined" 
                color="primary"  
                label="Expiration CI *" 
                v-model="consultant.ExpirationDateCI" 
                type="date"
                :rules="[required, dateRule]"
              ></v-text-field>
            </v-col>

            <!-- Interne -->
            <v-col cols="6">
              <v-switch 
                variant="outlined" 
                color="primary"  
                label="Interne" 
                v-model="consultant.Intern"
              ></v-switch>
            </v-col>

            <!-- Entreprise -->
            <v-col cols="6">
              <v-select
                label="Entreprise *"
                v-model="consultant.enterprise"
                :items="enterprises"
                item-title="name"
                item-value="name"
                :rules="[required]"
                variant="outlined"
                color="primary"
                :disabled="!!consultant.Intern"
              />
            </v-col>

            <!-- Email -->
            <v-col cols="6">
              <v-text-field 
                variant="outlined" 
                color="primary"  
                label="Email *" 
                v-model="consultant.Email" 
                :rules="[required, emailRule]" 
                required
              ></v-text-field>
            </v-col>

            <!-- Téléphone -->
            <v-col cols="6">
              <v-text-field 
                variant="outlined" 
                color="primary"  
                label="Téléphone *" 
                v-model="consultant.Phone" 
                :rules="[required, phoneRule]" 
                required
              ></v-text-field>
            </v-col>

            <!-- Date de disponibilité -->
            <v-col cols="6">
              <v-text-field 
                variant="outlined" 
                color="primary"  
                label="Date de disponibilité *" 
                v-model="consultant.AvailabilityDate" 
                type="date" 
                :rules="[dateRule]" 
                required
              ></v-text-field>
            </v-col>
          </v-row>
        </v-form>
      </v-card-text>

      <!-- Bouton Publier -->
      <v-card-actions class="d-flex justify-end">
        <v-btn color="primary" @click="submitConsultant">Publier</v-btn>
      </v-card-actions>
    </v-card>
  </v-container>
</template>


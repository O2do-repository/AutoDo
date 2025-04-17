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
  enterprise: string;
  phone: string;
  copyCI: string;
  picture: string;
}

interface Enterprise {
  enterpriseUuid: string;
  name: string;
}

const router = useRouter();
const consultant = ref<Consultant>({
  email: '',
  availabilityDate: '',
  expirationDateCI: '',
  intern: false,
  name: '',
  surname: '',
  enterprise: '',
  phone: '',
  copyCI: '',
  picture: ''
});

const enterprises = ref<Enterprise[]>([]);

// Validation rules
const required = (value: string) => value?.trim() !== '' || 'Champ obligatoire';
const emailRule = (value: string) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value) || "Format invalide (ex: exemple@mail.com)";
const phoneRule = (value: string) => /^\+?[0-9]{9,15}$/.test(value) || "Format invalide (ex: +32444332211)";
const urlRule = (value: string) => /^(https?:\/\/)[^\s$.?#].[^\s]*$/.test(value) || "Lien invalide (ex: https://...)";
const imageUrlRule = (value: string) => /^(https?:\/\/.*\.(?:png|jpg|jpeg|gif|webp))$/i.test(value) || "Lien image invalide (ex: https://site.com/photo.jpg)";
const dateRule = (value: string) => !!value || 'Veuillez choisir une date';

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

// Si le consultant est interne alors il sera d'O2do
watch(() => consultant.value.intern, (newVal) => {
  if (newVal) {
    consultant.value.enterprise = 'O2do';
  } else {
    consultant.value.enterprise = '';
  }
});

const formRef = ref<any>(null);

// Fetch des entreprises
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

const submitConsultant = async (): Promise<void> => {
  try {
    if (!consultant.value.picture || consultant.value.picture.trim() === '') {
      consultant.value.picture = placeholderImage;
    }

    if (!formRef.value) return;

    const { valid } = await formRef.value.validate();
    if (!valid) return;

    const response = await fetch(`${import.meta.env.VITE_API_URL}/consultant`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(consultant.value)
    });

    if (!response.ok) {
      throw new Error("Erreur lors de la modification du consultant");
    }

    setTimeout(() => {
      router.push('/consultant/consultant-info');
    }, 1000);
  } catch (error) {
    console.error(error instanceof Error ? error.message : 'Une erreur est survenue');
  }
};

onMounted(() => {
  fetchEnterprises();

  const storedConsultant = sessionStorage.getItem("editConsultant");
  if (storedConsultant) {
    const parsed = JSON.parse(storedConsultant);
    parsed.expirationDateCI = parsed.expirationDateCI?.split('T')[0] || '';
    parsed.availabilityDate = parsed.availabilityDate?.split('T')[0] || '';
    Object.assign(consultant.value, parsed);
    checkImage();
  }
});
</script>

<template>
  <v-container>
    <v-card class="pa-4">
      <GoBackBtn class="mb-4" />

      <v-card-title class="text-h5 font-weight-bold">Modifier Consultant</v-card-title>
      <v-card-text>
        <v-form ref="formRef">
          <v-row>
            <!-- Image Preview -->
            <v-col cols="12" class="d-flex flex-column align-center">
              <v-avatar size="150">
                <v-img :src="validPicture" alt="Photo Consultant" contain @error="setPlaceholder"></v-img>
              </v-avatar>
            </v-col>

            <!-- Lien de l'image -->
            <v-col cols="12">
              <v-text-field 
                variant="outlined" color="primary"
                label="Lien Photo *"
                v-model="consultant.picture"
                :placeholder="placeholderImage"
                :rules="[required, imageUrlRule]"
                @focus="clearPlaceholder"
                @blur="restorePlaceholder"
                @input="checkImage"
              />
            </v-col>
            <v-col cols="6">
              <v-text-field label="Prénom *" v-model="consultant.surname" :rules="[required]" required variant="outlined" color="primary" />
            </v-col>
            <v-col cols="6">
              <v-text-field label="Nom *" v-model="consultant.name" :rules="[required]" required variant="outlined" color="primary" />
            </v-col>


            <v-col cols="6">
              <v-text-field label="Lien Copie CI *" v-model="consultant.copyCI" :rules="[required, urlRule]" required variant="outlined" color="primary" />
            </v-col>
            <v-col cols="6">
              <v-text-field label="Expiration CI *" v-model="consultant.expirationDateCI" type="date" :rules="[required, dateRule]" variant="outlined" color="primary" />
            </v-col>

            <v-col cols="6">
              <v-switch label="Interne" v-model="consultant.intern" color="primary" />
            </v-col>
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
                :disabled="!!consultant.intern"
              />
            </v-col>

            <v-col cols="6">
              <v-text-field label="Email *" v-model="consultant.email" :rules="[required, emailRule]" required variant="outlined" color="primary" />
            </v-col>
            <v-col cols="6">
              <v-text-field label="Téléphone *" v-model="consultant.phone" :rules="[required, phoneRule]" required variant="outlined" color="primary" />
            </v-col>

            <v-col cols="6">
              <v-text-field label="Date de disponibilité *" v-model="consultant.availabilityDate" type="date" :rules="[required, dateRule]" required variant="outlined" color="primary" />
            </v-col>
          </v-row>
        </v-form>
      </v-card-text>

      <v-card-actions class="d-flex justify-end">
        <v-btn color="primary" @click="submitConsultant">Modifier</v-btn>
      </v-card-actions>
    </v-card>
  </v-container>
</template>

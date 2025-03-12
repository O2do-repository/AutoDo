<template>
    <UpdateProfile :profileId="profileId" />
</template>
  
<script setup lang="ts">
import { useRoute } from 'vue-router';
import UpdateProfile from '@/components/Profile/UpdateProfile.vue';

const route = useRoute();
const profileId = (route.params as { profileUuid: string }).profileUuid;


</script>

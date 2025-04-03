<template>

<v-container class="go-back-btn">
    <GoBackBtn />
</v-container>

<ConsultantInfo/>
<TableProfile /> 


</template>
  
<script lang="ts" setup>
    import GoBackBtn from '@/components/utils/GoBackBtn.vue'; 

    import ConsultantInfo from '@/components/Consultant/ConsultantInfo.vue';
    import TableProfile from '@/components/Profile/TableProfile.vue';
</script>

<style scoped>
.go-back-btn {
  padding-bottom: 16px; 
}
</style>
  
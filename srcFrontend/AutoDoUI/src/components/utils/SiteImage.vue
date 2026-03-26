<template>
    <v-img
      src="@/assets/logo.png" 
      alt="Logo"
      class="fixed-image"
      width="150"
      height="150"
      contain
    />
</template>

<style scoped>
.fixed-image {
  position: absolute;
  top: -20px;
  right: 20px;
  z-index: 1000;
}
</style>
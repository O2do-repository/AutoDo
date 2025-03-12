<template>
    <v-img
      src="@/assets/logo.jpg" 
      alt="Logo"
      class="fixed-image"
      width="100"
      height="100"
      contain
    />
</template>

<style scoped>
.fixed-image {
  position: absolute;
  top: 10px;
  right: 10px;
  z-index: 1000;
}
</style>
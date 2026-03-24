<template>
  <div class="site-wrap">
    <TheHeader />

    <main>
      <router-view />
    </main>

    <TheFooter />
  </div>
</template>

<script setup>
import TheHeader from '../components/TheHeader.vue';
import TheFooter from '../components/TheFooter.vue';
</script>
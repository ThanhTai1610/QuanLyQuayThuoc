<template>
  <router-view></router-view>

  <ChatbotTuVan />
</template>

<script setup>
import { defineAsyncComponent } from 'vue';

// Sử dụng defineAsyncComponent để giải quyết cảnh báo [INEFFECTIVE_DYNAMIC_IMPORT]
// và giúp Vercel không bị lỗi khi build.
const ChatbotTuVan = defineAsyncComponent(() =>
  import('./components/ChatbotTuVan.vue')
);
</script>
<template>
  <div class="admin-container">
    <Sidebar />

    <div class="main-content">
      <Topbar />

      <div class="page-body p-4">
        <router-view></router-view>
      </div>

      <FooterAdmin />
    </div>
  </div>
</template>

<script setup>
import Sidebar from '../components/Admin/Sidebar.vue';
import Topbar from '../components/Admin/Topbar.vue';
import FooterAdmin from '../components/Admin/FooterAdmin.vue';
</script>

<style scoped>
/* CSS để chia khung: Sidebar cố định bên trái, Main bên phải */
.admin-container {
  display: flex;
  min-height: 100vh;
}
.main-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  background-color: #f4f7f6; /* Màu nền nhẹ cho admin */
}
</style>
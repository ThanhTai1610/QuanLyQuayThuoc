<template>
  <footer class="sticky-footer bg-white">
    <div class="container my-auto">
      <div class="copyright text-center my-auto">
        <span>Bản quyền &copy; Hệ thống quản trị {{ currentYear }}</span>
      </div>
    </div>
  </footer>
</template>

<script setup>
import { ref } from 'vue';

// Tự động cập nhật năm hiện tại để Tài không cần sửa tay mỗi năm
const currentYear = ref(new Date().getFullYear());
</script>

<style scoped>
@import "../../assets/css_admin/sb-admin-2.min.css";
@import "../../assets/css_admin/sidebar.css";
/* Đảm bảo footer luôn hiển thị chuyên nghiệp trong trang Admin */
.sticky-footer {
  padding: 2rem 0;
  flex-shrink: 0;
}
.copyright span {
  font-size: 0.8rem;
  color: #858796;
}
</style>
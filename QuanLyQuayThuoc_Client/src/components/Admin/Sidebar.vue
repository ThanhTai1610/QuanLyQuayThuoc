<template>
  <ul class="navbar-nav bg-gradient-primary sidebar sidebar-dark accordion" id="accordionSidebar">
    <router-link class="sidebar-brand d-flex align-items-center justify-content-center" :to="{ name: 'BaoCao' }">
      <div class="sidebar-brand-icon rotate-n-15">
        <i class="fas fa-laugh-wink"></i>
      </div>
      <div class="sidebar-brand-text mx-3">Bá đạo</div>
    </router-link>

    <hr class="sidebar-divider my-0">

    <li class="nav-item" :class="{ active: $route.name === 'QuanLyNguoiDung' }">
      <router-link class="nav-link" :to="{ name: 'QuanLyNguoiDung' }">
        <i class="fas fa-fw fa-users"></i>
        <span>Quản lý người dùng</span>
      </router-link>
    </li>

    <li class="nav-item" :class="{ active: $route.name === 'QuanLyDanhMuc' }">
      <router-link class="nav-link" :to="{ name: 'QuanLyDanhMuc' }">
        <i class="fas fa-fw fa-sitemap"></i>
        <span>Quản lý danh mục</span>
      </router-link>
    </li>

    <li class="nav-item" :class="{ active: $route.name === 'BaoCao' }">
      <router-link class="nav-link" :to="{ name: 'BaoCao' }">
        <i class="fas fa-fw fa-chart-line"></i>
        <span>Báo cáo & Phân tích</span>
      </router-link>
    </li>

    <li class="nav-item" :class="{ active: $route.name === 'XuLyDonHang' }">
      <router-link class="nav-link" :to="{ name: 'XuLyDonHang' }">
        <i class="fas fa-fw fa-clipboard-check"></i>
        <span>Xử lý đơn hàng</span>
      </router-link>
    </li>

    <li class="nav-item" :class="{ active: $route.name === 'KiemKe' }">
      <router-link class="nav-link" :to="{ name: 'KiemKe' }">
        <i class="fas fa-fw fa-clipboard-list"></i>
        <span>Kiểm kê</span>
      </router-link>
    </li>

    <li class="nav-item" :class="{ active: $route.name === 'BanHang' }">
      <router-link class="nav-link" :to="{ name: 'BanHang' }">
        <i class="fas fa-fw fa-cash-register"></i>
        <span>Bán hàng tại quầy (POS)</span>
      </router-link>
    </li>

    <li class="nav-item" :class="{ active: $route.name === 'QuanLyKho' || $route.name === 'QuanLyLoHang' }">
      <router-link class="nav-link" :to="{ name: 'QuanLyKho' }">
        <i class="fas fa-fw fa-boxes"></i>
        <span>Quản lý Kho & Lô hàng</span>
      </router-link>
    </li>

    <hr class="sidebar-divider d-none d-md-block">

    <div class="text-center d-none d-md-inline">
      <button class="rounded-circle border-0" id="sidebarToggle" @click="toggleSidebar"></button>
    </div>
  </ul>
</template>

<script setup>
const toggleSidebar = () => {
  document.body.classList.toggle('sidebar-toggled');
  const sidebar = document.querySelector('.sidebar');
  if (sidebar) {
    sidebar.classList.toggle('toggled');
  }
};
</script>

<style scoped>
/* Giữ nguyên class từ CSS cũ của Tài */
.nav-item.active .nav-link {
  font-weight: bold;
}
</style>
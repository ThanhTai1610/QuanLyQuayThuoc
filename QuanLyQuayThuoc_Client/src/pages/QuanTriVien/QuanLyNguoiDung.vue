<template>
  <div class="container-fluid">

    <div class="d-sm-flex align-items-center justify-content-between mb-4">
      <h1 class="h3 mb-0 text-gray-800">Quản lý người dùng &amp; nhân viên</h1>
      <button class="d-none d-sm-inline-block btn btn-sm btn-primary shadow-sm" @click="cuonDenThemSua">
        <i class="fas fa-user-plus fa-sm text-white-50"></i> Thêm người dùng
      </button>
    </div>

    <div class="qlnd-quick-nav">
      <ul class="nav nav-pills flex-wrap mb-0">
        <li class="nav-item"><a class="nav-link" href="#phan-tong-quan">1. Tổng quan</a></li>
        <li class="nav-item"><a class="nav-link" href="#phan-them-sua">2. Thêm / Sửa</a></li>
        <li class="nav-item"><a class="nav-link" href="#phan-quyen-chi-tiet">3. Phân quyền</a></li>
        <li class="nav-item"><a class="nav-link" href="#phan-lich-su">4. Lịch sử hoạt động</a></li>
      </ul>
    </div>

    <DanhSachNguoiDung ref="dsRef" @chinh-sua="xuLyChinhSua" />
    <ThemSuaNguoiDung  :du-lieu-sua="duLieuSua" @refresh="dsRef.loadData()" />
    <PhanQuyenNhanVien />
    <LichSuHoatDong />

  </div>
</template>

<script setup>
import { ref } from 'vue';
import DanhSachNguoiDung from './DanhSachNguoiDung.vue';
import ThemSuaNguoiDung  from './ThemSuaNguoiDung.vue';

const dsRef      = ref(null);
const duLieuSua  = ref(null);

const xuLyChinhSua = (nd) => {
  duLieuSua.value = nd;
  cuonDenThemSua();
};

const cuonDenThemSua = () => {
  const el = document.getElementById('phan-them-sua');
  if (el) window.scrollTo({ top: el.offsetTop, behavior: 'smooth' });
};
</script>
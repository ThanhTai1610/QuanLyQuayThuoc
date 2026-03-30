<template>
  <div class="container-fluid">
    <h1 class="h3 mb-4 text-gray-800">Quản lý người dùng</h1>

    <div id="phan-them-sua">
      <ThemSuaNguoiDung 
        :duLieuSua="userDangSua" 
        @refresh="dsRef.loadData()" 
        @huy-sua="userDangSua = null" 
      />
    </div>

    <div id="phan-tong-quan">
      <DanhSachNguoiDung ref="dsRef" @chinh-sua="xuLyChinhSua" />
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import DanhSachNguoiDung from './DanhSachNguoiDung.vue';
import ThemSuaNguoiDung from './ThemSuaNguoiDung.vue';

const dsRef = ref(null);
const userDangSua = ref(null);

const xuLyChinhSua = (nd) => {
  // Khi bấm nút sửa ở dưới, gán dữ liệu vào biến này
  userDangSua.value = { ...nd }; 
  // Cuộn lên mượt mà
  const el = document.getElementById('phan-them-sua');
  if (el) window.scrollTo({ top: el.offsetTop - 20, behavior: 'smooth' });
};
</script>
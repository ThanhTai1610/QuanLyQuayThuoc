<template>
  <div class="container-fluid">

    <!-- Header -->
    <div class="d-sm-flex align-items-center justify-content-between mb-4">
      <div>
        <h1 class="h3 mb-0 text-gray-800">Báo cáo &amp; Phân tích</h1>
        <p class="mb-0 text-muted small">Trực quan hóa tình hình kinh doanh nhà thuốc.</p>
      </div>
      <span class="d-none d-sm-inline text-sm text-gray-500">
        <i class="far fa-calendar-alt mr-1"></i> Kỳ: {{ kyHienTai }}
      </span>
    </div>

    <!-- 1. Chỉ số chính -->
    <BcChiSoChinh :du-lieu="chiSo" />

    <!-- 2. Biểu đồ Doanh thu & Lợi nhuận -->
    <BcDoanhThuLoiNhuan />

    <!-- 3. Kho & Sản phẩm -->
    <div class="row">
      <div class="col-lg-6 mb-4"><BcTopBanChay /></div>
      <div class="col-lg-6 mb-4"><BcTopXemNhieu /></div>
    </div>

    <!-- 4. Cảnh báo -->
    <div class="row mb-4">
      <div class="col-lg-6 mb-4"><BcCanhBaoHanDung /></div>
      <div class="col-lg-6 mb-4"><BcTonKhoThap /></div>
    </div>

    <!-- 5. Đơn hàng & Khách hàng -->
    <div class="row mb-4">
      
      <div class="col-lg-4 mb-4"><BcNhanVien /></div>
    </div>

  </div>
</template>

<script setup>
import '../../assets/css_admin/bao-cao-phan-tich.css';
import { ref, onMounted } from 'vue';
import axiosClient from '../../api/axiosClient';

import BcChiSoChinh       from './Bcchisochinh.vue';
import BcDoanhThuLoiNhuan from './Bcdoanhthuloinhuan.vue';
import BcTopBanChay       from './Bctopbanchay.vue';
import BcTopXemNhieu      from './Bctopxemnhieu.vue';
import BcCanhBaoHanDung   from './Bccanhbaohandung.vue';
import BcTonKhoThap       from './Bctonkhothap.vue';
import BcNhanVien         from './Bcnhanvien.vue';

const kyHienTai = ref('');
const chiSo     = ref(null);

const loadChiSo = async () => {
  try {
    const res = await axiosClient.get('/BaoCao/chi-so-chinh');
    chiSo.value     = res;
    kyHienTai.value = res.kyBaoCao || '';
  } catch (err) {
    console.error('Lỗi tải chỉ số chính:', err);
  }
};

onMounted(loadChiSo);
</script>
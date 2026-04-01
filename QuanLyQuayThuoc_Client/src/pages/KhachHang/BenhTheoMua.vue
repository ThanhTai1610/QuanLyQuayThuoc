<template>
  <section class="home-section home-season-disease">
    <div class="container">
      <div class="season-disease-title">
        <span class="icon-heartbeat"></span>
        <h2>Bệnh theo mùa</h2>
      </div>
      <p class="season-disease-sub">
        Gợi ý giải pháp chăm sóc sức khỏe theo từng nhóm bệnh thường gặp khi giao mùa.
      </p>

      <!-- Tabs — ánh xạ với ChuDeSucKhoe -->
      <div class="season-tabs">
        <button
          v-for="tab in danhSachChuDe"
          :key="tab.maChuDe"
          type="button"
          class="season-tab"
          :class="{ active: tabHienTai === tab.maChuDe }"
          @click="doiTab(tab.maChuDe)"
        >
          {{ tab.tenChuDe }}
        </button>
      </div>

      <!-- Loading -->
      <div v-if="dangTai" class="text-center py-4">
        <div class="spinner-border text-primary" role="status">
          <span class="sr-only">Đang tải...</span>
        </div>
      </div>

      <div v-else-if="chuDeHienTai" class="season-disease-board">

        <!-- Card giới thiệu — NoiDungGiaiPhap + TieuDePhu từ ChuDeSucKhoe -->
        <div class="season-intro-card">
          <img
            :src="getImageUrl(chuDeHienTai.hinhAnh)"
            :alt="chuDeHienTai.tenChuDe"
            class="season-intro-bg"
          />
          <div class="season-intro-content">
            <h3>{{ chuDeHienTai.tieuDePhu }}</h3>
            <p>{{ chuDeHienTai.noiDungGiaiPhap }}</p>
            <router-link
              :to="{ path: '/san-pham', query: { chuDe: chuDeHienTai.maChuDe } }"
              class="btn season-intro-btn"
            >
              Khám phá ngay giải pháp
            </router-link>
          </div>
        </div>

        <!-- Danh sách sản phẩm theo chủ đề — từ Thuoc_ChuDe JOIN Thuoc + DonViTinh -->
        <div class="season-product-list">
          <article
            class="season-product-card"
            v-for="sp in sanPhamChuDe"
            :key="sp.maThuoc"
          >
            <div class="season-product-head">
              <span class="origin">{{ sp.nuocSanXuat }}</span>
              <span v-if="sp.phanTramGiam" class="discount">-{{ sp.phanTramGiam }}%</span>
            </div>
            <img
              :src="getImageUrl(sp.hinhAnhChinh)"
              :alt="sp.tenThuoc"
              class="season-product-image"
            />
            <h4>{{ sp.tenThuoc }}</h4>
            <div class="season-price">
              {{ formatGia(sp.giaBan) }} <span>/ {{ sp.tenDonVi }}</span>
            </div>
            <div v-if="sp.giaCu" class="season-old-price">{{ formatGia(sp.giaCu) }}</div>
            <div class="season-pack">{{ sp.quyCach }}</div>
            <router-link
              :to="{ name: 'ChiTietSanPham', params: { id: sp.maThuoc } }"
              class="btn season-buy-btn"
            >
              Chọn mua
            </router-link>
          </article>
        </div>

      </div>
    </div>
  </section>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import axiosClient from '../../api/axiosClient';

const danhSachChuDe  = ref([]);  // GET /ChuDeSucKhoe — TrangThai = 1
const sanPhamChuDe   = ref([]);  // GET /ChuDeSucKhoe/:id/san-pham
const tabHienTai     = ref(null);
const dangTai        = ref(false);

const chuDeHienTai = computed(() =>
  danhSachChuDe.value.find(c => c.maChuDe === tabHienTai.value)
);

// ── Load danh sách chủ đề ──
const loadChuDe = async () => {
  try {
    const res = await axiosClient.get('/ChuDeSucKhoe');
    danhSachChuDe.value = res.data;
    if (res.data.length > 0) {
      tabHienTai.value = res.data[0].maChuDe;
      await loadSanPham(res.data[0].maChuDe);
    }
  } catch (err) {
    console.error('Lỗi tải chủ đề sức khỏe:', err);
  }
};

// ── Load sản phẩm theo chủ đề — Thuoc_ChuDe JOIN Thuoc + DonViTinh ──
const loadSanPham = async (maChuDe) => {
  dangTai.value = true;
  try {
    const res = await axiosClient.get(`/ChuDeSucKhoe/${maChuDe}/san-pham`);
    sanPhamChuDe.value = res.data;
  } catch (err) {
    console.error('Lỗi tải sản phẩm chủ đề:', err);
  } finally {
    dangTai.value = false;
  }
};

const doiTab = async (maChuDe) => {
  if (tabHienTai.value === maChuDe) return;
  tabHienTai.value = maChuDe;
  await loadSanPham(maChuDe);
};

const getImageUrl = (path) => {
  if (!path) return '/images/no-image.png';
  if (path.startsWith('http')) return path;
  return `https://localhost:7070${path}`;
};

const formatGia = (value) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value ?? 0);

onMounted(loadChuDe);
</script>
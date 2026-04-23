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

      <div v-if="dangTai" class="text-center py-5">
        <div class="spinner-border text-primary" role="status">
          <span class="visually-hidden">Đang tải...</span>
        </div>
      </div>

      <div v-else-if="thongTinChuDe" class="season-disease-board">
        
        <div class="season-intro-card">
          <img
            :src="getImageUrl(thongTinChuDe.hinhAnh)"
            :alt="thongTinChuDe.tenChuDe"
            class="season-intro-bg"
          />
          <div class="season-intro-content">
            <h3>{{ thongTinChuDe.tieuDePhu }}</h3>
            <p>{{ thongTinChuDe.noiDungGiaiPhap }}</p>
            
            <router-link
              :to="{ path: '/san-pham', query: { chuDe: thongTinChuDe.maChuDe } }"
              class="btn season-intro-btn"
            >
              Khám phá ngay giải pháp
            </router-link>
          </div>
        </div>

        <div class="season-product-list">
          <article
            class="season-product-card"
            v-for="sp in sanPhamChuDe"
            :key="sp.maThuoc"
          >
            <div class="product-origin-badge">
              <img :src="getFlagUrl(sp.nuocSanXuat)" class="flag-icon" alt="flag" />
              <span class="origin-text">{{ sp.nuocSanXuat || 'Việt Nam' }}</span>
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
import { ref, onMounted } from 'vue';
import axiosClient from '../../api/axiosClient';

const danhSachChuDe = ref([]);
const thongTinChuDe  = ref(null);
const sanPhamChuDe   = ref([]);
const tabHienTai     = ref(null);
const dangTai        = ref(false);

// Hàm lấy link cờ đồng bộ với trang bán chạy
const getFlagUrl = (countryName) => {
  if (!countryName) return 'https://flagcdn.com/w40/vn.png';

  const name = countryName
    .toLowerCase()
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '');

  if (name.includes('viet nam') || name === 'vn') return 'https://flagcdn.com/w40/vn.png';
  if (name.includes('hoa ky') || name.includes('my') || name.includes('usa') || name === 'us') return 'https://flagcdn.com/w40/us.png';
  if (name.includes('phap') || name === 'fr') return 'https://flagcdn.com/w40/fr.png';
  if (name.includes('duc') || name === 'de') return 'https://flagcdn.com/w40/de.png';
  if (name.includes('nhat') || name === 'jp') return 'https://flagcdn.com/w40/jp.png';
  if (name.includes('anh') || name === 'uk' || name === 'gb') return 'https://flagcdn.com/w40/gb.png';
  if (name.includes('han quoc') || name === 'kr') return 'https://flagcdn.com/w40/kr.png';
  if (name.includes('canada') || name === 'ca') return 'https://flagcdn.com/w40/ca.png';
  if (name.includes('italy') || name.includes('italia') || name === 'it' || name === 'y') return 'https://flagcdn.com/w40/it.png';
  if (name.includes('nga') || name.includes('russia') || name === 'ru') return 'https://flagcdn.com/w40/ru.png';
  if (name.includes('philippines') || name.includes('philippin') || name.includes('phi lip pin') || name === 'ph') return 'https://flagcdn.com/w40/ph.png';

  return 'https://flagcdn.com/w40/un.png';
};

const loadTabs = async () => {
  try {
    const res = await axiosClient.get('/ChuDeSucKhoe');
    danhSachChuDe.value = res; 
    if (res && res.length > 0) {
      tabHienTai.value = res[0].maChuDe;
      await loadNoiDungChuDe(res[0].maChuDe);
    }
  } catch (err) {
    console.error('Lỗi tải danh sách tab:', err);
  }
};

const loadNoiDungChuDe = async (id) => {
  dangTai.value = true;
  try {
    const res = await axiosClient.get(`/ChuDeSucKhoe/${id}/san-pham`);
    thongTinChuDe.value = res.info; 
    sanPhamChuDe.value = res.products;
  } catch (err) {
    console.error('Lỗi tải dữ liệu chủ đề:', err);
  } finally {
    dangTai.value = false;
  }
};

const doiTab = async (id) => {
  if (tabHienTai.value === id) return;
  tabHienTai.value = id;
  await loadNoiDungChuDe(id);
};

const getImageUrl = (path) => {
  if (!path) return '/images/no-image.png';
  if (path.startsWith('http')) return path;
  return `${import.meta.env.VITE_API_URL.replace('/api', '')}${path}`;
};

const formatGia = (value) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value ?? 0);

onMounted(loadTabs);
</script>

<style scoped>
/* CSS bổ sung để badge hiển thị đẹp trong card Bệnh theo mùa */
.season-product-card {
  position: relative; /* Quan trọng để badge đè lên */
}

.product-origin-badge {
  position: absolute;
  top: 8px;
  left: 8px;
  z-index: 5;
  background: rgba(240, 224, 224, 0.9);
  padding: 2px 8px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  font-size: 10px;
  color: #666;
  border: 1px solid #f0f0f0;
  box-shadow: 0 1px 3px rgba(0,0,0,0.05);
}

.flag-icon {
  width: 14px !important;
  height: 10px !important;
  object-fit: cover;
  margin-right: 4px;
}

.origin-text {
  white-space: nowrap;
}
.season-intro-btn {
  color: #fff !important;
}

.season-intro-btn:hover {
  color: #fff !important;
}
</style>

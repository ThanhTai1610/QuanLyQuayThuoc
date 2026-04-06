<template>
  <div class="home-section home-best-seller py-5">
    <div class="container">
      <div class="row align-items-center mb-4">
        <div class="col-lg-8">
          <div class="title-section mb-0">
            <h2 class="mb-1"><strong class="text-primary">Sản phẩm</strong> bán chạy</h2>
            <p class="pharmacy-products-sub mb-0 text-muted">Giao nhanh 2h, chính hãng 100%, giá tốt mỗi ngày.</p>
          </div>
        </div>
      </div>

      <div class="best-seller-wrapper mt-4">
        <div v-if="loading" class="text-center p-5">
           <div class="spinner-border text-primary" role="status"></div>
           <p class="mt-2">Đang tải sản phẩm...</p>
        </div>

        <div v-else class="row no-gutters align-items-stretch best-seller-row">
          <div class="col-md-2 col-6 mb-4 px-2" v-for="item in bestSellers" :key="item.id">
            <div class="best-seller-card shadow-sm h-100">
              <div class="product-origin-badge">
                <img :src="getFlagUrl(item.origin)" class="flag-icon" />
                <span>{{ item.origin || 'Việt Nam' }}</span>
              </div>
              
              <router-link :to="{ name: 'ChiTietSanPham', params: { id: item.id }}" class="d-block text-center mt-4">
                <img :src="getImageUrl(item.image)" :alt="item.name" class="best-seller-image">
              </router-link>

              <div class="best-seller-content p-2 pt-0">
                <div class="best-seller-name mb-1">{{ item.name }}</div>
                
                <div class="best-seller-price mb-1">
                  <span class="current text-danger font-weight-bold">{{ formatPrice(item.price) }}</span>
                  <span class="unit text-muted small">/ {{ item.unit }}</span>
                </div>

                <div class="best-seller-sold mb-3">Đã bán: {{ item.totalSold }}</div>

                <button @click="handleBuy(item)" class="btn btn-primary btn-sm btn-block">Chọn mua</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axiosClient from '../../api/axiosClient';

const bestSellers = ref([]);
const loading = ref(true);

// 1. THÊM HÀM LẤY LINK CỜ (Copy từ trang danh sách của Tài)
const getFlagUrl = (countryName) => {
  if (!countryName) return 'https://flagcdn.com/w40/vn.png';
  const name = countryName.toLowerCase();
  if (name.includes('việt nam')) return 'https://flagcdn.com/w40/vn.png';
  if (name.includes('hoa kỳ') || name.includes('mỹ') || name.includes('usa')) return 'https://flagcdn.com/w40/us.png';
  if (name.includes('pháp')) return 'https://flagcdn.com/w40/fr.png';
  if (name.includes('đức')) return 'https://flagcdn.com/w40/de.png';
  if (name.includes('nhật')) return 'https://flagcdn.com/w40/jp.png';
  if (name.includes('anh')) return 'https://flagcdn.com/w40/gb.png';
  if (name.includes('hàn quốc')) return 'https://flagcdn.com/w40/kr.png';
  return 'https://flagcdn.com/w40/un.png'; // Mặc định nếu không khớp
};

const fetchBestSellers = async () => {
  try {
    const data = await axiosClient.get('/ThuocKhachHang/BestSellers');
    bestSellers.value = data;
  } catch (error) {
    console.error("Lỗi tải sản phẩm bán chạy:", error);
  } finally {
    loading.value = false;
  }
};

const getImageUrl = (path) => {
  if (!path) return 'https://via.placeholder.com/300x300.png?text=No+Image';
  if (path.trim().startsWith('http')) return path.trim(); 
  const cleanPath = path.startsWith('/') ? path : `/${path}`;
  return `https://localhost:7070${cleanPath}`;
};

const formatPrice = (price) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price);
};

const handleBuy = (item) => {
  console.log("Mua sản phẩm:", item.id);
};

onMounted(fetchBestSellers);
</script>

<style scoped>
/* 2. THÊM CSS CHO QUỐC KỲ (Đảm bảo giống hệt trang danh sách) */
.best-seller-card {
  position: relative;
  background: #fff;
  border-radius: 8px;
  padding: 10px;
  border: 1px solid #eee;
  height: 100%;
  display: flex;
  flex-direction: column;
}

.product-origin-badge {
  position: absolute;
  top: 10px;
  left: 10px;
  z-index: 10;
  background: rgba(255, 255, 255, 0.9);
  padding: 2px 8px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  font-size: 10px;
  color: #555;
  box-shadow: 0 1px 3px rgba(0,0,0,0.1);
  border: 1px solid #f0f0f0;
}

.flag-icon {
  width: 16px !important;
  height: 10px !important;
  object-fit: cover;
  margin-right: 4px;
  border-radius: 1px;
}

.best-seller-image {
  width: 100%;
  height: 140px;
  object-fit: contain;
  margin: 15px 0;
}

.best-seller-name {
  font-size: 13px;
  font-weight: 500;
  height: 38px;
  overflow: hidden;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  margin-bottom: 8px;
}

.best-seller-sold {
    font-size: 11px;
    color: #888;
    margin-bottom: 10px;
    font-style: italic;
}
span.current.text-danger.font-weight-bold {
  font-size: 18px;
  font-weight: 700 !important;
}
</style>
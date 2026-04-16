<template>
  <div class="site-wrap">
    <HomeBanner />

    <div class="site-section py-5">
      <div class="container">
        <div class="row">
          <div class="col-lg-4" v-for="f in features" :key="f.title">
            <div class="feature text-center">
              <span :class="['wrap-icon', f.icon]"></span>
              <h3>{{ f.title }}</h3>
              <p>{{ f.desc }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <SanPhamNhaThuoc />
    
    <SanPhamBanChay />

    <div class="home-section home-featured-categories bg-light py-5">
      <div class="container">
        <div class="home-section-header text-center mb-5">
          <h2 class="home-section-title mb-1 text-black">Danh mục <span class="text-primary">nổi bật</span></h2>
          <p class="home-section-sub">Khám phá nhanh các nhóm sản phẩm theo nhu cầu sức khỏe.</p>
        </div>
        
        <div v-if="loading" class="text-center py-5">
          <div class="spinner-border text-primary" role="status"></div>
          <p class="mt-2">Đang tải danh mục thuốc...</p>
        </div>

        <div class="row align-items-stretch" v-else-if="rootCategories.length > 0">
          <div class="col-md-3 col-6 mb-4" v-for="cat in rootCategories" :key="cat.maDanhMuc">
            <router-link :to="{ name: 'DanhSachSanPham', query: { maDanhMuc: cat.maDanhMuc } }" class="category-item-link">
              <div class="category-card text-center h-100 p-4">
                
                <div class="category-icon-wrap mb-3">
  <img v-if="isImageUrl(cat.icon)" 
       :src="getApiUrl(cat.icon)" 
       class="img-fluid img-icon" 
       :alt="cat.tenDanhMuc" />
  
  <i v-else-if="cat.icon && cat.icon.startsWith('fa-')" 
     :class="['fas', cat.icon, 'category-icon']"></i>

  <span v-else :class="[cat.icon || 'icon-flask', 'category-icon']"></span>
</div>

                <div class="category-name h6 text-black font-weight-bold">{{ cat.tenDanhMuc }}</div>
                <div class="category-count text-muted small">{{ cat.soSanPham }} sản phẩm</div>
              </div>
            </router-link>
          </div>
        </div>

        <div v-else class="text-center py-5">
          <p>Không có danh mục nào để hiển thị.</p>
        </div>
      </div>
    </div>

    <BenhTheoMua />
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'; // Đã thêm computed
import axios from 'axios';
import HomeBanner from './HomeBanner.vue';
import SanPhamNhaThuoc from './SanPhamNhaThuoc.vue';
import BenhTheoMua from './BenhTheoMua.vue';
import SanPhamBanChay from './SanPhamBanChay.vue';

const BASE_URL = import.meta.env.VITE_API_URL.replace('/api', '');

// 1. Dùng rawCategories để chứa toàn bộ dữ liệu từ API
const rawCategories = ref([]); 
const loading = ref(true);

const features = [
  { icon: 'flaticon-24-hours-drugs-delivery', title: 'Giao hàng 24/7', desc: 'Giao nhanh thuốc tận cửa.' },
  { icon: 'flaticon-medicine', title: 'Thuốc mới hằng ngày', desc: 'Cập nhật dược phẩm mới nhất.' },
  { icon: 'flaticon-test-tubes', title: 'Kiểm định nghiêm ngặt', desc: 'Sản phẩm chính hãng 100%.' }
];

// 2. Tạo rootCategories bằng computed để tự động lọc theo trangThai
const rootCategories = computed(() => {
  return rawCategories.value.filter(cat => cat.trangThai === 'hien').slice(0, 8);;
});

const fetchCategories = async () => {
  try {
    const response = await axios.get(`${BASE_URL}/api/DanhMuc/cay`);
    // 3. Gán dữ liệu vào rawCategories thay vì rootCategories
    rawCategories.value = response.data;
  } catch (error) {
    console.error("Lỗi lấy danh mục:", error);
  } finally {
    loading.value = false;
  }
};

const isImageUrl = (iconPath) => {
  return iconPath && (iconPath.includes('/') || iconPath.includes('.'));
};

const getApiUrl = (path) => {
  if (!path) return '';
  return path.startsWith('http') ? path : `${BASE_URL}${path}`;
};

onMounted(() => {
  fetchCategories();
});
</script>

<style scoped>
.category-item-link { text-decoration: none !important; display: block; transition: 0.3s; }
.category-item-link:hover { transform: translateY(-5px); }

.category-card { 
  background: #fff; 
  border-radius: 12px; 
  border: 1px solid #f0f0f0; 
  box-shadow: 0 4px 15px rgba(0,0,0,0.03); 
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

.category-icon-wrap { 
  width: 70px; 
  height: 70px; 
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto; 
}

.category-icon { 
  font-size: 32px;
  color: #51eaea; 
}

.img-icon { 
  max-width: 100%; 
  max-height: 100%; 
  object-fit: contain; 
}

.category-name { 
  min-height: 40px; 
  display: flex; 
  align-items: center; 
  justify-content: center; 
}
</style>
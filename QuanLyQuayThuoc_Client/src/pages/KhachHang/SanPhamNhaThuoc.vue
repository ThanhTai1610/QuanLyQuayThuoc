<template>
  <div class="site-section bg-light pharmacy-products">
    <div class="container">

      <div class="row align-items-center mb-4">
        <div class="col-lg-8">
          <div class="title-section mb-0">
            <h2><strong class="text-primary">Sản phẩm</strong> nhà thuốc</h2>
            <p class="pharmacy-products-sub mb-0">Giao nhanh 2h, chính hãng 100%, giá tốt mỗi ngày.</p>
          </div>
        </div>
        <div class="col-lg-4 text-lg-right mt-3 mt-lg-0">
          <router-link to="/san-pham" class="btn btn-outline-primary px-4">Xem tất cả sản phẩm</router-link>
        </div>
      </div>

      <div class="row">
        <div class="col-lg-3 col-md-6 mb-4" v-for="product in products" :key="product.id">
          <router-link
            :to="{ name: 'ChiTietSanPham', params: { id: product.id } }"
            class="pharmacy-product-card d-block"
          >
            <span class="pharmacy-product-discount" v-if="product.phanTramGiamGia">
              -{{ product.phanTramGiamGia }}%
            </span>

            <img :src="getImageUrl(product.hinhAnh)" :alt="product.tenThuoc" class="pharmacy-product-image">

            <div class="pharmacy-product-meta">{{ product.tenDanhMuc }}</div>
            <h3 class="pharmacy-product-name">{{ product.tenThuoc }}</h3>

            <div class="pharmacy-product-rating">
              {{ product.diemDanhGia }} ★ ({{ product.luotDanhGia }} đánh giá)
            </div>

            <div class="pharmacy-product-price">
              {{ formatPrice(product.giaBan) }}
              <span v-if="product.giaCu > product.giaBan">{{ formatPrice(product.giaCu) }}</span>
            </div>

            <div class="pharmacy-product-action">Xem chi tiết</div>
          </router-link>
        </div>
      </div>

    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axiosClient from '../../api/axiosClient';

const products = ref([]);

const taiDanhSachSanPham = async () => {
  try {
    const data = await axiosClient.get('/SanPham/trang-chu');
    products.value = data;
  } catch (error) {
    console.error("Lỗi khi tải sản phẩm:", error);
  }
};

const getImageUrl = (path) => {
  if (!path) return '/images/no-image.png';
  if (path.startsWith('http')) return path;
  return `https://localhost:7070${path}`;
};

const formatPrice = (value) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
};

onMounted(() => {
  taiDanhSachSanPham();
});
</script>
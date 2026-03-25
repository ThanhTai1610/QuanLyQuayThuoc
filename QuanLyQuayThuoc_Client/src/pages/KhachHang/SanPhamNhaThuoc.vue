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
            <span class="pharmacy-product-discount">{{ product.discount }}</span>
            <img :src="product.image" :alt="product.name" class="pharmacy-product-image">
            <div class="pharmacy-product-meta">{{ product.meta }}</div>
            <h3 class="pharmacy-product-name">{{ product.name }}</h3>
            <div class="pharmacy-product-rating">{{ product.rating }} ★ ({{ product.reviews }} đánh giá)</div>
            <div class="pharmacy-product-price">
              {{ product.price }} <span>{{ product.oldPrice }}</span>
            </div>
            <div class="pharmacy-product-action">Xem chi tiết</div>
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import product01 from '../../assets/images/product_01.png';
import product02 from '../../assets/images/product_02.png';
import product03 from '../../assets/images/product_03.png';
import product04 from '../../assets/images/product_04.png';

const products = [
  {
    id: 'omega3',
    discount: '-20%',
    image: product01,
    meta: 'Thực phẩm chức năng',
    name: 'Omega 3 Orihiro hỗ trợ tim mạch và não bộ (180 viên)',
    rating: '4.9',
    reviews: 142,
    price: '736.000đ',
    oldPrice: '920.000đ',
  },
  {
    id: 'jointcare',
    discount: '-15%',
    image: product02,
    meta: 'Xương khớp',
    name: 'Viên uống Joint Care hỗ trợ giảm đau nhức xương khớp',
    rating: '4.8',
    reviews: 97,
    price: '561.000đ',
    oldPrice: '660.000đ',
  },
  {
    id: 'probiotic',
    discount: '-10%',
    image: product03,
    meta: 'Tiêu hóa',
    name: 'Men vi sinh Bio Balance hỗ trợ tiêu hóa, giảm đầy hơi',
    rating: '4.7',
    reviews: 76,
    price: '315.000đ',
    oldPrice: '350.000đ',
  },
  {
    id: 'fishoil',
    discount: '-12%',
    image: product04,
    meta: 'Mắt - Não bộ',
    name: 'Fish Oil Premium hỗ trợ mắt sáng và tăng cường trí nhớ',
    rating: '4.8',
    reviews: 120,
    price: '461.550đ',
    oldPrice: '525.000đ',
  },
];
</script>
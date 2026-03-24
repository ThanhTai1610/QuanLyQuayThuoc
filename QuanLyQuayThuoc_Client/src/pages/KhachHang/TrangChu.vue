<template>
  <div class="site-wrap">
    <HomeBanner />

    

    <div class="site-section bg-light pharmacy-products">
      <div class="container">
        <div class="row align-items-center mb-4">
          <div class="col-lg-8">
            <div class="title-section mb-0">
              <h2><strong class="text-primary">Sản phẩm</strong> nhà thuốc</h2>
            </div>
          </div>
          <div class="col-lg-4 text-lg-right mt-3 mt-lg-0">
            <router-link to="/san-pham" class="btn btn-outline-primary px-4">Xem tất cả</router-link>
          </div>
        </div>
        <div class="row">
  <div class="col-lg-3 col-md-6 mb-4">
  <router-link :to="{ name: 'ChiTietSanPham', params: { id: 'jointcare' } }">
  
    <span class="pharmacy-product-discount">-20%</span>
    <img src="../../assets/images/product_01.png" alt="Product" class="pharmacy-product-image">
    <div class="pharmacy-product-meta">Thực phẩm chức năng</div>
    <h3 class="pharmacy-product-name">Omega 3 Orihiro hỗ trợ tim mạch</h3>
    <div class="pharmacy-product-price">736.000đ <span>920.000đ</span></div>
    <div class="pharmacy-product-action">Xem chi tiết</div>
  </router-link>
</div>
</div>
      </div>
    </div>

    <div class="home-section home-best-seller py-5">
      <div class="container">
        <div class="best-seller-wrapper">
          <div class="best-seller-title-badge">Sản phẩm bán chạy</div>
          <div class="row no-gutters align-items-stretch">
            <div class="col-md-2 col-6 mb-3" v-for="i in 6" :key="i">
              <div class="best-seller-card h-100 p-2 border">
                <img src="../images/product_01.png" class="img-fluid" alt="Bán chạy">
                <div class="best-seller-name small font-weight-bold">Sản phẩm {{i}}</div>
                <div class="text-primary small">736.000đ</div>
                <button class="btn btn-sm btn-primary btn-block mt-2">Chọn mua</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="home-section home-featured-categories py-5 bg-white">
      <div class="container">
        <div class="home-section-header mb-4 text-center">
          <h2 class="home-section-title">Danh mục <span class="text-primary">nổi bật</span></h2>
        </div>
        <div class="row">
          <div class="col-md-3 col-6 mb-3" v-for="cat in categories" :key="cat.id">
            <div class="category-card p-4 text-center border">
              <span :class="cat.icon" style="font-size: 2rem;"></span>
              <div class="category-name mt-2 font-weight-bold">{{ cat.name }}</div>
              <div class="category-count small text-muted">{{ cat.count }} sản phẩm</div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="home-section home-promise-strip py-4 bg-primary text-white">
      <div class="container">
        <div class="row text-center">
          <div class="col-md-3 col-6">
             <div class="promise-item"><span class="icon-shield mr-2"></span> Chính hãng 100%</div>
          </div>
          <div class="col-md-3 col-6">
             <div class="promise-item"><span class="icon-truck mr-2"></span> Miễn phí giao hàng</div>
          </div>
          <div class="col-md-3 col-6">
             <div class="promise-item"><span class="icon-undo2 mr-2"></span> Đổi trả 30 ngày</div>
          </div>
          <div class="col-md-3 col-6">
             <div class="promise-item"><span class="icon-thumbs-up mr-2"></span> Uy tín hàng đầu</div>
          </div>
        </div>
      </div>
    </div>

    <ChatbotTuVan />
  </div>
</template>

<script setup>
import { onMounted } from 'vue';
import ChatbotTuVan from './ChatbotTuVan.vue';
import heroBg1 from '../../assets/images/hero_1.jpg';
import HomeBanner from './HomeBanner.vue';
const categories = [
  { id: 1, name: 'Thần kinh não', count: 59, icon: 'icon-flask' },
  { id: 2, name: 'Vitamin', count: 80, icon: 'icon-flask' },
  { id: 3, name: 'Tim mạch', count: 23, icon: 'icon-heart' },
  { id: 4, name: 'Tiêu hóa', count: 81, icon: 'icon-local_hospital' },
];

onMounted(() => {
  if (window.$ && window.$.fn.owlCarousel) {
    window.$('.owl-single').owlCarousel({
      loop: true, autoplay: true, dots: true, items: 1, nav: false
    });
  }
});
</script>
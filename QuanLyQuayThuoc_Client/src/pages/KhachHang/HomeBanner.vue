<template>
  <div class="banner-wrapper">
    <div class="swiper-button-prev-custom" @click="goPrev">
      <span class="icon-chevron-left"></span>
    </div>
    <div class="swiper-button-next-custom" @click="goNext">
      <span class="icon-chevron-right"></span>
    </div>

    <swiper
      :modules="modules"
      :slides-per-view="1"
      :loop="true"
      :autoplay="{ delay: 5000, disableOnInteraction: false }"
      :pagination="{ clickable: true }"
      @swiper="onSwiper"
      class="main-banner-swiper"
    >
      <swiper-slide>
        <div class="site-blocks-cover overlay" :style="{ backgroundImage: `url(${heroBg1})` }">
          <div class="container">
            <div class="row align-items-center justify-content-center">
              <div class="col-lg-12 text-center">
                <div class="site-block-cover-content">
                  <h1 class="mb-0"><strong class="text-primary">Pharmative</strong> mở cửa 24/7</h1>
                  <p class="mb-5">Hệ thống nhà thuốc trực tuyến luôn sẵn sàng phục vụ bạn mọi lúc, mọi nơi.</p>
                  <p><a href="#" class="btn btn-primary px-5 py-3">Mua ngay</a></p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </swiper-slide>

      <swiper-slide>
        <div class="site-blocks-cover overlay" :style="{ backgroundImage: `url(${heroBg2})` }">
          <div class="container">
            <div class="row align-items-center justify-content-center">
              <div class="col-lg-12 text-center">
                <div class="site-block-cover-content">
                  <h1 class="mb-0">Thuốc mới <strong class="text-primary">mỗi ngày</strong></h1>
                  <p class="mb-5">Cập nhật liên tục các loại thuốc và thực phẩm chức năng chất lượng.</p>
                  <p><a href="#" class="btn btn-primary px-5 py-3">Mua ngay</a></p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </swiper-slide>
    </swiper>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { Swiper, SwiperSlide } from 'swiper/vue';
import { Autoplay, Pagination, Navigation } from 'swiper/modules';

// Import CSS của Swiper
import 'swiper/css';
import 'swiper/css/pagination';
import 'swiper/css/navigation';

import heroBg1 from '../../assets/images/banner1.jpg';
import heroBg2 from '../../assets/images/banner2.jpg';

const modules = [Autoplay, Pagination, Navigation];
const swiperInstance = ref(null);

const onSwiper = (swiper) => {
  swiperInstance.ref = swiper;
};

const goNext = () => {
  swiperInstance.ref?.slideNext();
};

const goPrev = () => {
  swiperInstance.ref?.slidePrev();
};
</script>

<style scoped>
.banner-wrapper {
  position: relative;
  width: 100%;
}

/* Nút bấm tùy chỉnh */
.swiper-button-prev-custom,
.swiper-button-next-custom {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  z-index: 100;
  width: 45px;
  height: 45px;
  background: rgba(0, 0, 0, 0.3);
  color: #fff;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: 0.3s;
}

.swiper-button-prev-custom { left: 20px; }
.swiper-button-next-custom { right: 20px; }

.swiper-button-prev-custom:hover,
.swiper-button-next-custom:hover {
  background: #28a745;
}

/* Tùy chỉnh dấu chấm chuyển slide (Pagination) */
:deep(.swiper-pagination-bullet) {
  width: 12px;
  height: 12px;
  background: #fff;
  opacity: 0.5;
}

:deep(.swiper-pagination-bullet-active) {
  background: #28a745;
  opacity: 1;
}

.site-blocks-cover {
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
  min-height: 600px;
  display: flex;
  align-items: center;
}
</style>
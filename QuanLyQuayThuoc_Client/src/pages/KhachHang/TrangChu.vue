<template>
  <div class="site-wrap">
    <HomeBanner />

    <!-- Features Strip -->
    <div class="site-section py-5">
      <div class="container">
        <div class="row">
          <div class="col-lg-4">
            <div class="feature">
              <span class="wrap-icon flaticon-24-hours-drugs-delivery"></span>
              <h3><a href="#">Giao hàng miễn phí</a></h3>
              <p>Miễn phí giao hàng cho đơn hàng trong khu vực hỗ trợ, giao nhanh và đúng hẹn.</p>
              <p><a href="#" class="d-flex align-items-center"><span class="mr-2">Tìm hiểu thêm</span> <span class="icon-keyboard_arrow_right"></span></a></p>
            </div>
          </div>
          <div class="col-lg-4">
            <div class="feature">
              <span class="wrap-icon flaticon-medicine"></span>
              <h3><a href="#">Thuốc mới mỗi ngày</a></h3>
              <p>Cập nhật liên tục các sản phẩm mới, đa dạng lựa chọn theo nhu cầu của bạn.</p>
              <p><a href="#" class="d-flex align-items-center"><span class="mr-2">Tìm hiểu thêm</span> <span class="icon-keyboard_arrow_right"></span></a></p>
            </div>
          </div>
          <div class="col-lg-4">
            <div class="feature">
              <span class="wrap-icon flaticon-test-tubes"></span>
              <h3><a href="#">Thuốc chính hãng đảm bảo</a></h3>
              <p>Sản phẩm chính hãng, có nguồn gốc rõ ràng, được bảo quản và phân phối theo đúng quy định.</p>
              <p><a href="#" class="d-flex align-items-center"><span class="mr-2">Tìm hiểu thêm</span> <span class="icon-keyboard_arrow_right"></span></a></p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <SanPhamNhaThuoc />

    <!-- Sản phẩm bán chạy -->
    <div class="home-section home-best-seller">
      <div class="container">
        <div class="best-seller-wrapper">
          <div class="best-seller-title-badge">Sản phẩm bán chạy</div>
          <div class="row no-gutters align-items-stretch best-seller-row">
            <div class="col-md-2 col-6 mb-3" v-for="item in bestSellers" :key="item.id">
              <div class="best-seller-card">
                <div class="best-seller-header">
                  <span class="best-seller-dot"></span>
                  <span class="best-seller-origin">{{ item.origin }}</span>
                  <span v-if="item.discount" :class="['best-seller-discount', item.discountClass]">{{ item.discount }}</span>
                </div>
                <img :src="item.image" :alt="item.name" class="best-seller-image">
                <div class="best-seller-name">{{ item.name }}</div>
                <div class="best-seller-price">
                  <span class="current">{{ item.price }}</span>
                  <span class="unit">/ {{ item.unit }}</span>
                </div>
                <div v-if="item.oldPrice" class="best-seller-old-price">{{ item.oldPrice }}</div>
                <div class="best-seller-pack" :class="{ 'best-seller-pack-tag': item.packTag }">{{ item.pack }}</div>
                <button class="btn best-seller-btn btn-block">Chọn mua</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Danh mục nổi bật -->
    <div class="home-section home-featured-categories">
      <div class="container">
        <div class="home-section-header mb-3">
          <h2 class="home-section-title mb-1">Danh mục <span class="text-primary">nổi bật</span></h2>
          <p class="home-section-sub">Khám phá nhanh các nhóm sản phẩm được quan tâm nhiều.</p>
        </div>
        <div class="row">
          <div class="col-md-3 col-6 mb-3" v-for="cat in categories" :key="cat.id">
            <div class="category-card">
              <span :class="[cat.icon, 'category-icon']"></span>
              <div class="category-name">{{ cat.name }}</div>
              <div class="category-count">{{ cat.count }} sản phẩm</div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Banner đăng ký -->
    <div class="site-section bg-image overlay" style="background-image: url('/images/hero_bg_2.jpg');">
      <div class="container">
        <div class="row justify-content-center text-center">
          <div class="col-lg-7">
            <h3 class="text-white">Đăng ký để nhận giảm giá lên đến 55%</h3>
            <p class="text-white">Nhận thông tin khuyến mãi, ưu đãi độc quyền và cập nhật sản phẩm mới mỗi ngày.</p>
            <p class="mb-0"><a href="#" class="btn btn-outline-white">Đăng ký</a></p>
          </div>
        </div>
      </div>
    </div>

    <!-- Testimonials + Tại sao chọn chúng tôi -->
    <div class="site-section">
      <div class="container">
        <div class="row justify-content-between">
          <div class="col-lg-6">
            <div class="title-section">
              <h2><strong class="text-primary">Khách hàng</strong> hài lòng</h2>
            </div>
            <div class="block-3 products-wrap">
              <div class="owl-single no-direction owl-carousel">
                <div class="testimony" v-for="t in testimonials" :key="t.author">
                  <blockquote>
                    <img :src="t.avatar" :alt="t.author" class="img-fluid">
                    <p>{{ t.content }}</p>
                  </blockquote>
                  <p class="author">&mdash; {{ t.author }}</p>
                </div>
              </div>
            </div>
          </div>
          <div class="col-lg-5">
            <div class="title-section">
              <h2 class="mb-5">Tại sao chọn <strong class="text-primary">chúng tôi</strong></h2>
              <div class="step-number d-flex mb-4">
                <span>1</span>
                <p>Đội ngũ dược sĩ giàu kinh nghiệm, tư vấn tận tâm và luôn ưu tiên sức khỏe của bạn.</p>
              </div>
              <div class="step-number d-flex mb-4">
                <span>2</span>
                <p>Sản phẩm đa dạng, chính hãng, được bảo quản theo đúng tiêu chuẩn của ngành dược.</p>
              </div>
              <div class="step-number d-flex mb-4">
                <span>3</span>
                <p>Hệ thống đặt hàng online đơn giản, giao hàng nhanh chóng và bảo mật thông tin khách hàng.</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Dải cam kết -->
    <div class="home-section home-promise-strip">
      <div class="container">
        <div class="row text-center">
          <div class="col-md-3 col-6 mb-3" v-for="p in promises" :key="p.title">
            <div class="promise-item">
              <span :class="[p.icon, 'promise-icon']"></span>
              <div class="promise-text">
                <div class="promise-title">{{ p.title }}</div>
                <div class="promise-sub">{{ p.sub }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <ChatbotTuVan />
  </div>
</template>

<script setup>
import { onMounted } from 'vue';
import HomeBanner from './HomeBanner.vue';
import SanPhamNhaThuoc from './SanPhamNhaThuoc.vue';
import ChatbotTuVan from './ChatbotTuVan.vue';

import product01 from '../../assets/images/product_01.png';
import product02 from '../../assets/images/product_02.png';
import product03 from '../../assets/images/product_03.png';
import product04 from '../../assets/images/product_04.png';
import product05 from '../../assets/images/product_05.png';
import product06 from '../../assets/images/product_06.png';
import person1 from '../../assets/images/person_1.jpg';
import person2 from '../../assets/images/person_2.jpg';
import person3 from '../../assets/images/person_3.jpg';
import person4 from '../../assets/images/person_4.jpg';

const bestSellers = [
  {
    id: 1, origin: 'Nhật Bản', discount: '-20%', discountClass: '',
    image: product01, name: 'Viên uống bổ sung Omega 3 hỗ trợ não, thị lực và tim mạch',
    price: '736.000đ', unit: 'Hộp', oldPrice: '920.000đ', pack: 'Hộp 120 viên', packTag: false,
  },
  {
    id: 2, origin: 'Nhật Bản', discount: '-20%', discountClass: '',
    image: product02, name: 'Viên uống giúp giảm đau khớp, bổ sung chất nhờn khớp',
    price: '768.000đ', unit: 'Hộp', oldPrice: '960.000đ', pack: 'Hộp 120 viên', packTag: false,
  },
  {
    id: 3, origin: 'Nhật Bản', discount: '-20%', discountClass: '',
    image: product03, name: 'Viên uống bổ sung lợi khuẩn đường ruột giảm rối loạn tiêu hóa cho lọ...',
    price: '792.000đ', unit: 'Hộp', oldPrice: '990.000đ', pack: 'Hộp 30 viên', packTag: false,
  },
  {
    id: 4, origin: 'Nhật Bản', discount: '-15%', discountClass: 'best-seller-discount--orange',
    image: product04, name: 'Viên uống hỗ trợ cho mắt và bổ não Orihiro Fish Oil (180 viên)',
    price: '461.550đ', unit: 'Hộp', oldPrice: '543.000đ', pack: 'Hộp 180 viên', packTag: false,
  },
  {
    id: 5, origin: 'Hàn Quốc', discount: null, discountClass: '',
    image: product05, name: 'Nước Hồng Sâm cô đặc hỗ trợ tăng cường sức khỏe',
    price: '1.200.000đ', unit: 'Hộp', oldPrice: null, pack: 'Hộp 3 Hộp lẻ x 10 Gói', packTag: true,
  },
  {
    id: 6, origin: 'Anh', discount: null, discountClass: '',
    image: product06, name: 'Chai xịt nhiệt miệng, tay chân miệng Aloclair Plus',
    price: '229.000đ', unit: 'Chai', oldPrice: null, pack: 'Hộp x 15ml', packTag: true,
  },
];

const categories = [
  { id: 1, name: 'Thần kinh não', count: 59, icon: 'icon-flask' },
  { id: 2, name: 'Vitamin & Khoáng chất', count: 80, icon: 'icon-flask' },
  { id: 3, name: 'Sinh lý - Nội tiết tố', count: 46, icon: 'icon-female' },
  { id: 4, name: 'Tim mạch - Huyết áp', count: 23, icon: 'icon-heart' },
  { id: 5, name: 'Miễn dịch - Đề kháng', count: 53, icon: 'icon-shield' },
  { id: 6, name: 'Tiêu hóa', count: 81, icon: 'icon-local_hospital' },
  { id: 7, name: 'Giải pháp làn da', count: 79, icon: 'icon-healing' },
  { id: 8, name: 'Chăm sóc da mặt', count: 191, icon: 'icon-face' },
];

const testimonials = [
  { author: 'Kelly Holmes', avatar: person1, content: '"Tôi luôn tin tưởng đặt mua thuốc tại Pharmative. Dược sĩ tư vấn rất tận tâm và giao hàng nhanh chóng."' },
  { author: 'Rebecca Morando', avatar: person2, content: '"Giá cả hợp lý, sản phẩm đa dạng và luôn có sẵn. Tôi rất hài lòng với chất lượng dịch vụ."' },
  { author: 'Lucas Gallone', avatar: person3, content: '"Đặt thuốc online tại Pharmative rất tiện lợi, tư vấn rõ ràng và giao thuốc đến tận nhà đúng giờ."' },
  { author: 'Andrew Neel', avatar: person4, content: '"Nhân viên hỗ trợ nhiệt tình, hướng dẫn sử dụng thuốc kỹ lưỡng. Tôi sẽ tiếp tục ủng hộ lâu dài."' },
];

const promises = [
  { icon: 'icon-shield', title: 'Thuốc chính hãng', sub: 'Đa dạng và chuyên sâu' },
  { icon: 'icon-undo2', title: 'Đổi trả trong 30 ngày', sub: 'Kể từ ngày mua hàng' },
  { icon: 'icon-thumbs-up', title: 'Cam kết 100%', sub: 'Chất lượng sản phẩm' },
  { icon: 'icon-truck', title: 'Miễn phí vận chuyển', sub: 'Theo chính sách giao hàng' },
];

onMounted(() => {
  if (window.$ && window.$.fn.owlCarousel) {
    window.$('.owl-single').owlCarousel({
      loop: true, autoplay: true, dots: true, items: 1, nav: false,
    });
  }
});
</script>
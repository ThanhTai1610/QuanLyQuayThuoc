<template>
  <div class="site-navbar py-0 lc-header">
    <div class="lc-header-top">
      <div class="container d-flex justify-content-between align-items-center">
        <div class="lc-header-top-left">
          <span>Trung tâm tiêm chủng Pharmative</span>
          <a href="#" class="lc-header-link ml-2">Tìm hiểu ngay</a>
        </div>
        <div class="lc-header-top-right d-none d-md-flex">
          <a href="#" class="lc-header-link mr-3">
            <span class="icon-smartphone mr-1"></span>
            Tải ứng dụng
          </a>
          <a href="tel:18006928" class="lc-header-link">
            <span class="icon-phone mr-1"></span>
            Tư vấn ngay: <strong>1800 6928</strong>
          </a>
        </div>
      </div>
    </div>

    <div class="lc-header-middle">
      <div class="container">
        <div class="d-flex align-items-center justify-content-between lc-header-main">
          <div class="lc-header-logo">
            <div class="site-logo">
              <router-link to="/" class="js-logo-clone">
                <strong class="text-white">Pharma</strong><span class="text-white">tive</span>
              </router-link>
            </div>
          </div>

          <div class="lc-header-search">
            <span class="icon-search mr-2"></span>
            <input type="text" class="form-control lc-header-search-input" placeholder="Giao hàng nhanh trong 1h, tìm sản phẩm, thuốc, danh mục...">
          </div>

          <div class="lc-header-actions">
            <div class="lc-header-account d-none d-md-block mr-2">
              <a href="#" class="lc-header-account-toggle">
                <span class="icon-user mr-1"></span>
                <span class="lc-header-account-name text-white">PHAM TH...</span>
                <span class="icon-keyboard_arrow_down ml-1"></span>
              </a>
              <div class="lc-header-account-menu">
                <router-link to="/ho-so"><span class="icon-user mr-2"></span> Thông tin cá nhân</router-link>
                <router-link to="/auth/orders"><span class="icon-list mr-2"></span> Đơn hàng của tôi</router-link>
                <router-link to="/auth/addresses"><span class="icon-map-marker mr-2"></span> Sổ địa chỉ nhận hàng</router-link>
              </div>
            </div>

            <router-link to="/gio-hang" class="btn btn-light lc-header-cart-btn">
              <span class="icon-shopping-bag mr-1"></span>
              Giỏ hàng
              <span class="lc-header-cart-count">2</span>
            </router-link>

            <a href="#" class="site-menu-toggle js-menu-toggle ml-2 d-inline-block d-lg-none text-white">
              <span class="icon-menu"></span>
            </a>
          </div>
        </div>
      </div>
    </div>

    <div class="lc-header-bottom d-none d-lg-block">
      <div class="container">
        <ul class="lc-header-nav mb-0">
          <li class="active"><router-link to="/thuc-pham-chuc-nang">Thực phẩm chức năng</router-link></li>
          <li><router-link to="/duoc-my-pham">Dược mỹ phẩm</router-link></li>
          <li><router-link to="/thuoc">Thuốc</router-link></li>
          <li><router-link to="/cham-soc-ca-nhan">Chăm sóc cá nhân</router-link></li>
          <li><router-link to="/thiet-bi-y-te">Thiết bị y tế</router-link></li>
          <li><router-link to="/tiem-chung">Tiêm chủng</router-link></li>
          <li><router-link to="/suc-khoe">Bệnh &amp; Góc sức khỏe</router-link></li>
          <li><router-link to="/he-thong-nha-thuoc">Hệ thống nhà thuốc</router-link></li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script setup>
// Giữ nguyên logic nếu cần
</script>
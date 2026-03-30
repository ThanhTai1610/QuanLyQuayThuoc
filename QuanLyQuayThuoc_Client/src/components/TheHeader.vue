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
            <span class="icon-smartphone mr-1"></span> Tải ứng dụng
          </a>
          <a href="tel:18006928" class="lc-header-link">
            <span class="icon-phone mr-1"></span> Tư vấn ngay: <strong>1800 6928</strong>
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
            <input type="text" class="form-control lc-header-search-input" placeholder="Giao hàng nhanh trong 1h, tìm sản phẩm...">
          </div>

          <div class="lc-header-actions">
            <div class="lc-header-account d-none d-md-block mr-2">
              
              <router-link v-if="!authState.user" :to="{ name: 'DangNhap' }" class="lc-header-account-toggle">
                <span class="icon-user mr-1"></span>
                <span class="lc-header-account-name text-white">Đăng nhập</span>
              </router-link>

              <template v-else>
                <div class="lc-header-account-wrapper">
                  <a href="javascript:void(0)" class="lc-header-account-toggle">
                    <span class="icon-user mr-1"></span>
                    <span class="lc-header-account-name text-white">
                      Chào, {{ authState.user.hoTen || authState.user.ten }}
                    </span>
                    <span class="icon-keyboard_arrow_down ml-1"></span>
                  </a>
                  
                  <div class="lc-header-account-menu">
                    <router-link to="/ho-so">
                      <i class="fas fa-user-circle mr-2"></i> Thông tin cá nhân
                    </router-link>
                    <router-link to="/lich-su-don-hang">
                      <i class="fas fa-history mr-2"></i> Đơn hàng của tôi
                    </router-link>
                    <router-link to="/auth/addresses">
                      <i class="fas fa-map-marker-alt mr-2"></i> Sổ địa chỉ
                    </router-link>
                    <div class="dropdown-divider"></div>
                    <a href="javascript:void(0)" @click="handleLogout" class="text-danger">
                      <i class="fas fa-sign-out-alt mr-2"></i> Đăng xuất
                    </a>
                  </div>
                </div>
              </template>
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
import { authState } from '../api/auth'; // Đảm bảo Tài đã tạo file auth.js như mình gửi trước đó
import { useRouter } from 'vue-router';
import Swal from 'sweetalert2';

const router = useRouter();

const handleLogout = () => {
  Swal.fire({
    title: 'Đăng xuất?',
    text: "Tài chắc chắn muốn thoát chứ?",
    icon: 'warning',
    showCancelButton: true,
    confirmButtonColor: '#28a745',
    cancelButtonColor: '#d33',
    confirmButtonText: 'Đăng xuất ngay',
    cancelButtonText: 'Ở lại'
  }).then((result) => {
    if (result.isConfirmed) {
      authState.logout();
      router.push({ name: 'DangNhap' });
      Swal.fire('Đã đăng xuất', 'Hẹn gặp lại Tài nhé!', 'success');
    }
  });
};
</script>

<style scoped>
/* Bổ sung CSS để xử lý Hover Dropdown nếu CSS cũ của Tài chưa có */
.lc-header-account-wrapper {
  position: relative;
  display: inline-block;
}

.lc-header-account-wrapper:hover .lc-header-account-menu {
  display: block;
  opacity: 1;
  visibility: visible;
  transform: translateY(0);
}

.lc-header-account-menu {
  position: absolute;
  top: 100%;
  right: 0;
  background: #fff;
  min-width: 220px;
  box-shadow: 0 8px 24px rgba(0,0,0,0.15);
  border-radius: 8px;
  padding: 10px 0;
  z-index: 999;
  display: none; /* Ẩn mặc định */
  transition: all 0.3s ease;
}

.lc-header-account-menu a {
  display: block;
  padding: 10px 20px;
  color: #333 !important;
  font-size: 14px;
  text-decoration: none;
  transition: background 0.2s;
}

.lc-header-account-menu a:hover {
  background: #f1f8f4;
  color: #28a745 !important;
}

.dropdown-divider {
  height: 0;
  margin: 8px 0;
  overflow: hidden;
  border-top: 1px solid #e9ecef;
}
</style>
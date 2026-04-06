<template>
  <div class="site-navbar py-0 lc-header">
    <!-- TOP BAR -->
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

    <!-- MIDDLE BAR -->
    <div class="lc-header-middle">
      <div class="container">
        <div class="d-flex align-items-center justify-content-between lc-header-main">

          <!-- LOGO -->
          <div class="lc-header-logo">
            <div class="site-logo">
              <router-link to="/" class="js-logo-clone">
                <strong class="text-white">Pharma</strong><span class="text-white">tive</span>
              </router-link>
            </div>
          </div>

          <!-- SEARCH -->
          <div class="lc-header-search" ref="searchWrapRef">
            <span class="icon-search mr-2" style="cursor:pointer;" @click="triggerSearch"></span>
            <input
              type="text"
              v-model="searchQuery"
              class="form-control lc-header-search-input"
              placeholder="Giao hàng nhanh trong 1h, tìm sản phẩm..."
              @input="onSearchInput"
              @keydown.enter="triggerSearch"
              @keydown.esc="closeDropdown"
              @focus="onFocus"
            />

            <div v-if="showDropdown" class="lc-search-dropdown">
  <div v-if="isLoading" class="lc-search-status">
    <span class="lc-search-spinner"></span> Đang tìm kiếm...
  </div>

  <template v-else-if="searchResults.length > 0">
    <router-link
      v-for="product in searchResults"
      :key="product.id"
      :to="{ name: 'ChiTietSanPham', params: { id: product.id } }"
      class="lc-search-item"
      @click="closeDropdown"
    >
      <img
        v-if="product.hinhAnhChinh"
        :src="getImageUrl(product.hinhAnhChinh)"
        class="lc-search-thumb"
        alt="thumb"
      />
      <div v-else class="lc-search-thumb-empty">💊</div>
      
      <div class="lc-search-info">
        <div class="lc-search-name">{{ product.tenThuoc }}</div>
        <div class="lc-search-cat">{{ product.tenDanhMuc }}</div>
      </div>
      <div class="lc-search-price">
        {{ formatPrice(product.giaBan) }}
      </div>
    </router-link>

    <div class="lc-search-viewall" @click="goToSearch">
      Xem tất cả kết quả cho "{{ searchQuery }}" →
    </div>
  </template>

  <div v-else class="lc-search-status">
    Không tìm thấy sản phẩm nào cho "<strong>{{ searchQuery }}</strong>"
  </div>
</div>
          </div>

          <!-- ACTIONS -->
          <div class="lc-header-actions">
            <!-- Account -->
            <div class="lc-header-account d-none d-md-block mr-2">
              <router-link
                v-if="!authState.user"
                :to="{ name: 'DangNhap' }"
                class="lc-header-account-toggle"
              >
                <span class="icon-user mr-1"></span>
                <span class="lc-header-account-name text-white">Đăng nhập</span>
              </router-link>

              <template v-else>
                <div class="lc-header-account-wrapper">
                  <a href="javascript:void(0)" class="lc-header-account-toggle">
                    <span class="icon-user mr-1"></span>
                    <span class="lc-header-account-name text-white">
                      {{ authState.user.hoTen || authState.user.ten }}
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
                    <router-link to="/dia-chi">
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

            <!-- Cart -->
            <router-link to="/gio-hang" class="btn btn-light lc-header-cart-btn">
              <span class="icon-shopping-bag mr-1"></span>
              Giỏ hàng
              <span class="lc-header-cart-count" v-if="cartState.totalQuantity > 0">
      {{ cartState.totalQuantity }}
    </span>
            </router-link>

            <!-- Mobile menu toggle -->
            <a
              href="#"
              class="site-menu-toggle js-menu-toggle ml-2 d-inline-block d-lg-none text-white"
            >
              <span class="icon-menu"></span>
            </a>
          </div>
        </div>
      </div>
    </div>

    <!-- BOTTOM NAV -->
    <div class="lc-header-bottom d-none d-lg-block">
      <div class="container">
        <ul class="lc-header-nav mb-0">
          <li class="active"><router-link to="/">Thực phẩm chức năng</router-link></li>
          <li><router-link to="/">Dược mỹ phẩm</router-link></li>
          <li><router-link to="/">Thuốc</router-link></li>
          <li><router-link to="/">Chăm sóc cá nhân</router-link></li>
          <li><router-link to="/">Thiết bị y tế</router-link></li>
          <li><router-link to="/">Tiêm chủng</router-link></li>
          <li><router-link to="/">Bệnh &amp; Góc sức khỏe</router-link></li>
          <li><router-link to="/">Hệ thống nhà thuốc</router-link></li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue';
import { useRouter } from 'vue-router';
import axiosClient from '../api/axiosClient';
import Swal from 'sweetalert2';
import { authState } from '../api/auth';
import { cartState } from '../api/cart';
const router = useRouter();

// ==================== LOGOUT ====================
const handleLogout = () => {
  Swal.fire({
    title: 'Đăng xuất?',
    text: 'Tài chắc chắn muốn thoát chứ?',
    icon: 'warning',
    showCancelButton: true,
    confirmButtonColor: '#28a745',
    cancelButtonColor: '#d33',
    confirmButtonText: 'Đăng xuất ngay',
    cancelButtonText: 'Ở lại',
  }).then((result) => {
    if (result.isConfirmed) {
      authState.logout();
      router.push({ name: 'DangNhap' });
      Swal.fire('Đã đăng xuất', 'Hẹn gặp lại Tài nhé!', 'success');
    }
  });
};

// ==================== SEARCH ====================
const searchQuery = ref('');
const searchResults = ref([]);
const showDropdown = ref(false);
const isLoading = ref(false);
const searchWrapRef = ref(null);
let searchTimer = null;

const formatPrice = (price) => {
  return price.toLocaleString('vi-VN') + 'đ';
};
const getImageUrl = (path) => {
  if (!path) return 'https://via.placeholder.com/100x100.png?text=Thuoc';
  if (path.startsWith('http')) return path;
  // Đảm bảo khớp với Port Backend của Tài
  return `https://localhost:7070${path.startsWith('/') ? '' : '/'}${path}`;
};
const onSearchInput = () => {
  clearTimeout(searchTimer);
  const q = searchQuery.value.trim();
  if (q.length < 2) {
    showDropdown.value = false;
    searchResults.value = [];
    return;
  }
  searchTimer = setTimeout(() => fetchSearch(q), 350);
};

const onFocus = () => {
  const q = searchQuery.value.trim();
  if (q.length >= 2 && searchResults.value.length > 0) {
    showDropdown.value = true;
  }
};

const fetchSearch = async (q) => {
  isLoading.value = true;
  showDropdown.value = true;
  try {
    const data = await axiosClient.get('/SanPham/search-quick', { params: { q } });
    searchResults.value = data;
  } catch (err) {
    console.error('Search error:', err);
    searchResults.value = [];
  } finally {
    isLoading.value = false;
  }
};

const triggerSearch = () => {
  const q = searchQuery.value.trim();
  if (q.length < 1) return;
  router.push({ name: 'DanhSachSanPham', query: { q } });
  closeDropdown();
  searchQuery.value = '';
};

const closeDropdown = () => {
  showDropdown.value = false;
};


const goToSearch = () => {
  router.push({ name: 'DanhSachSanPham', query: { q: searchQuery.value.trim() } });
  closeDropdown();
  searchQuery.value = '';
};
// Đóng dropdown khi click ra ngoài
const handleClickOutside = (e) => {
  if (searchWrapRef.value && !searchWrapRef.value.contains(e.target)) {
    closeDropdown();
  }
};

onMounted(() => document.addEventListener('click', handleClickOutside));
onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside);
  clearTimeout(searchTimer);
});
</script>


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

            <!-- SEARCH DROPDOWN -->
            <div v-if="showDropdown" class="lc-search-dropdown">
              <!-- Loading -->
              <div v-if="isLoading" class="lc-search-status">
                <span class="lc-search-spinner"></span> Đang tìm kiếm...
              </div>

              <!-- Results -->
              <template v-else-if="searchResults.length > 0">
                <a
                  v-for="product in searchResults"
                  :key="product.id"
                  :href="`/san-pham/${product.id}`"
                  class="lc-search-item"
                  @click="closeDropdown"
                >
                  <img
                    v-if="product.hinhAnhChinh"
                    :src="product.hinhAnhChinh"
                    class="lc-search-thumb"
                    alt=""
                    @error="(e) => (e.target.style.display = 'none')"
                  />
                  <div v-else class="lc-search-thumb-empty">💊</div>
                  <div class="lc-search-info">
                    <div class="lc-search-name">{{ product.tenThuoc }}</div>
                    <div class="lc-search-cat">{{ product.tenDanhMuc }}</div>
                  </div>
                  <div class="lc-search-price">
                    {{ formatPrice(product.giaBan) }}
                  </div>
                </a>
                <div class="lc-search-viewall" @click="goToSearch">
                  Xem tất cả kết quả cho "{{ searchQuery }}" →
                </div>
              </template>

              <!-- No results -->
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

            <!-- Cart -->
            <router-link to="/gio-hang" class="btn btn-light lc-header-cart-btn">
              <span class="icon-shopping-bag mr-1"></span>
              Giỏ hàng
              <span class="lc-header-cart-count">2</span>
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
import { ref, onMounted, onUnmounted } from 'vue';
import { useRouter } from 'vue-router';
import axiosClient from '../api/axiosClient';
import Swal from 'sweetalert2';
import { authState } from '../api/auth';

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

<style scoped>
/* ===== ACCOUNT DROPDOWN ===== */
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
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.15);
  border-radius: 8px;
  padding: 10px 0;
  z-index: 999;
  display: none;
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

/* ===== SEARCH DROPDOWN ===== */
.lc-header-search {
  position: relative;
}

.lc-search-dropdown {
  position: absolute;
  top: calc(100% + 8px);
  left: 0;
  right: 0;
  background: #fff;
  border-radius: 10px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.18);
  z-index: 9999;
  overflow: hidden;
  min-width: 420px;
  animation: dropdownFadeIn 0.18s ease;
}

@keyframes dropdownFadeIn {
  from { opacity: 0; transform: translateY(-6px); }
  to   { opacity: 1; transform: translateY(0); }
}

.lc-search-status {
  padding: 16px;
  font-size: 13px;
  color: #888;
  text-align: center;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
}

/* Loading spinner */
.lc-search-spinner {
  display: inline-block;
  width: 14px;
  height: 14px;
  border: 2px solid #e0e0e0;
  border-top-color: #28a745;
  border-radius: 50%;
  animation: spin 0.6s linear infinite;
}
@keyframes spin {
  to { transform: rotate(360deg); }
}

.lc-search-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 10px 14px;
  border-bottom: 1px solid #f3f3f3;
  text-decoration: none;
  transition: background 0.15s;
  cursor: pointer;
}
.lc-search-item:last-of-type {
  border-bottom: none;
}
.lc-search-item:hover {
  background: #f6fbf8;
}

.lc-search-thumb {
  width: 48px;
  height: 48px;
  border-radius: 8px;
  object-fit: cover;
  border: 1px solid #f0f0f0;
  flex-shrink: 0;
}
.lc-search-thumb-empty {
  width: 48px;
  height: 48px;
  border-radius: 8px;
  background: #e9f5ee;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 22px;
  flex-shrink: 0;
}

.lc-search-info {
  flex: 1;
  min-width: 0;
}
.lc-search-name {
  font-size: 14px;
  color: #222;
  font-weight: 500;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
.lc-search-cat {
  font-size: 12px;
  color: blue;
  margin-top: 2px;
}
.lc-search-price {
  font-size: 14px;
  font-weight: 600;
  color: red;
  white-space: nowrap;
  flex-shrink: 0;
}

.lc-search-viewall {
  display: block;
  text-align: center;
  padding: 12px 16px;
  background: #f6fbf8;
  color: black;
  font-size: 13px;
  font-weight: 500;
  cursor: pointer;
  border-top: 1px solid #e9f5ee;
  transition: background 0.15s;
}
.lc-search-viewall:hover {
  background: #e4f5ec;
}
</style>
<template>
  <div class="account-breadcrumb">
    <div class="container">
      <nav aria-label="breadcrumb">
        <ol class="breadcrumb mb-0">
          <li class="breadcrumb-item"><router-link to="/">Trang chủ</router-link></li>
          <li class="breadcrumb-item"><router-link to="/ho-so">Cá nhân</router-link></li>
          <li class="breadcrumb-item active" aria-current="page">{{ activeTitle }}</li>
        </ol>
      </nav>
    </div>
  </div>

  <div class="col-lg-3 mb-4 mb-lg-0">
    <div class="account-sidebar">
      <div class="account-user-card">
        <div class="account-avatar" @click="$emit('open-avatar-modal')" style="cursor: pointer">
          <img v-if="user.anhDaiDien" 
               :src="getFullUrl(user.anhDaiDien)" 
               class="img-fluid rounded-circle" 
               alt="Avatar">
          <span v-else class="icon-user"></span>
        </div>
        <div class="account-user-info">
          <div class="account-user-name">{{ user.hoTen || 'Đang tải...' }}</div>
          <div class="account-user-phone">{{ user.soDienThoai }}</div>
        </div>
      </div>
      <ul class="account-menu list-unstyled">
        <li :class="{ active: activeMenu === 'profile' }">
          <router-link to="/ho-so"><span class="icon-user mr-2"></span> Thông tin cá nhân</router-link>
        </li>
        <li :class="{ active: activeMenu === 'orders' }">
          <router-link to="/lich-su-don-hang"><span class="icon-list mr-2"></span> Đơn hàng của tôi</router-link>
        </li>
        <li :class="{ active: activeMenu === 'address' }">
          <router-link to="/dia-chi"><span class="icon-map-marker mr-2"></span> Quản lý sổ địa chỉ</router-link>
        </li>
        <li>
          <a href="#" @click.prevent="$emit('logout')"><span class="icon-exit_to_app mr-2"></span> Đăng xuất</a>
        </li>
      </ul>
    </div>
  </div>
</template>

<script setup>
defineProps({
  user: Object,
  activeMenu: String, // Để tô đậm mục đang chọn
  activeTitle: String // Chữ hiển thị ở Breadcrumb
});

defineEmits(['open-avatar-modal', 'logout']);

const getFullUrl = (path) => {
  if (!path) return '';
  return path.startsWith('http') ? path : `https://localhost:7070${path}`;
};

</script>
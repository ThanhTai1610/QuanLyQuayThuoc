<template>
  <nav class="navbar navbar-expand navbar-light bg-white topbar mb-4 static-top shadow">
    <button id="sidebarToggleTop" class="btn btn-link d-md-none rounded-circle mr-3" @click="toggleSidebar">
      <i class="fa fa-bars"></i>
    </button>

    <ul class="navbar-nav ml-auto">
      <div class="topbar-divider d-none d-sm-block"></div>

      <li class="nav-item dropdown no-arrow">
        <a 
          class="nav-link dropdown-toggle" 
          href="#" 
          id="userDropdown" 
          role="button" 
          data-toggle="dropdown"
          aria-haspopup="true" 
          aria-expanded="false"
        >
          <span class="mr-2 d-none d-lg-inline text-gray-600 small">
            {{ userRole }}
          </span>
          <img class="img-profile rounded-circle" :src="profileImg">
        </a>
        
        <div class="dropdown-menu dropdown-menu-right shadow animated--grow-in" aria-labelledby="userDropdown">
          <router-link class="dropdown-item" :to="{ name: 'ThongTinCaNhan' }">
            <i class="fas fa-user fa-sm fa-fw mr-2 text-gray-400"></i>
            Hồ sơ
          </router-link>
          
          <div class="dropdown-divider"></div>
          
          <a class="dropdown-item" href="#" @click.prevent="handleLogout">
            <i class="fas fa-sign-out-alt fa-sm fa-fw mr-2 text-gray-400"></i>
            Đăng xuất
          </a>
        </div>
      </li>
    </ul>
  </nav>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import defaultAvatar from '../../assets/img/undraw_profile.svg'; // Tài kiểm tra lại đường dẫn ảnh nhé

const router = useRouter();
const userRole = ref('Quản trị viên'); // Tài có thể dùng logic để đổi thành "Nhân viên"
const profileImg = ref(defaultAvatar);

const toggleSidebar = () => {
  // Logic để đóng/mở sidebar trên mobile (nếu Tài dùng jQuery thì gọi ở đây)
  document.body.classList.toggle('sidebar-toggled');
  const sidebar = document.querySelector('.sidebar');
  if (sidebar) sidebar.classList.toggle('toggled');
};

const handleLogout = () => {
  // Xóa token và về trang đăng nhập
  localStorage.removeItem('token');
  router.push({ name: 'DangNhap' });
};
</script>

<style scoped>
/* Không cần viết CSS thêm vì Tài đã có css template sẵn */
.img-profile {
  height: 2rem;
  width: 2rem;
}
</style>
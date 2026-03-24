<template>
  <div class="site-wrap">
    <div class="account-breadcrumb">
      <div class="container">
        <nav aria-label="breadcrumb">
          <ol class="breadcrumb mb-0">
            <li class="breadcrumb-item"><router-link to="/">Trang chủ</router-link></li>
            <li class="breadcrumb-item"><router-link to="/ho-so">Cá nhân</router-link></li>
            <li class="breadcrumb-item active" aria-current="page">Thông tin cá nhân</li>
          </ol>
        </nav>
      </div>
    </div>

    <div class="account-wrapper">
      <div class="container">
        <div class="row">
          <div class="col-lg-3 mb-4 mb-lg-0">
            <div class="account-sidebar">
              <div class="account-user-card">
                <div class="account-avatar">
                  <span class="icon-user"></span>
                </div>
                <div class="account-user-info">
                  <div class="account-user-name">{{ user.fullName }}</div>
                  <div class="account-user-phone">{{ user.phone }}</div>
                </div>
              </div>
              <ul class="account-menu list-unstyled">
                <li class="active">
                  <router-link to="/ho-so">
                    <span class="icon-user mr-2"></span> Thông tin cá nhân
                  </router-link>
                </li>
                <li>
                  <router-link to="/don-hang">
                    <span class="icon-list mr-2"></span> Đơn hàng của tôi
                  </router-link>
                </li>
                <li>
                  <router-link to="/dia-chi">
                    <span class="icon-map-marker mr-2"></span> Quản lý sổ địa chỉ
                  </router-link>
                </li>
                <li>
                  <a href="#" @click.prevent="">
                    <span class="icon-event_available mr-2"></span> Lịch hẹn tiêm chủng
                  </a>
                </li>
                <li>
                  <a href="#" @click.prevent="">
                    <span class="icon-healing mr-2"></span> Đơn thuốc của tôi
                  </a>
                </li>
                <li>
                  <a href="#" @click.prevent="logout">
                    <span class="icon-exit_to_app mr-2"></span> Đăng xuất
                  </a>
                </li>
              </ul>
            </div>
          </div>

          <div class="col-lg-9">
            <div class="account-content-card">
              <div class="account-content-header">
                <h2 class="mb-0">Thông tin cá nhân</h2>
              </div>
              <div class="account-profile-body">
                <div class="account-profile-avatar text-center mb-4">
                  <div class="avatar-circle">
                    <span class="icon-user"></span>
                  </div>
                </div>
                <div class="table-responsive">
                  <table class="table account-profile-table">
                    <tbody>
                      <tr>
                        <td class="label-col">Họ và tên</td>
                        <td class="value-col text-right">{{ user.fullName }}</td>
                      </tr>
                      <tr>
                        <td class="label-col">Số điện thoại</td>
                        <td class="value-col text-right">{{ user.phone }}</td>
                      </tr>
                      <tr>
                        <td class="label-col">Giới tính</td>
                        <td class="value-col text-right">{{ user.gender }}</td>
                      </tr>
                      <tr>
                        <td class="label-col">Ngày sinh</td>
                        <td class="value-col text-right text-primary" style="cursor: pointer">
                          {{ user.birthday || 'Thêm thông tin' }}
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
                <div class="text-center mt-4">
                  <button type="button" class="btn btn-primary px-5 py-2 rounded-pill">
                    Chỉnh sửa thông tin
                  </button>
                </div>
              </div>
            </div>
          </div>

        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();

// Dữ liệu mẫu (sau này Tài sẽ lấy từ API qua axios)
const user = ref({
  fullName: 'PHẠM THÀNH TÀI',
  phone: '0816354009',
  gender: 'Nam',
  birthday: ''
});

const logout = () => {
  // Logic đăng xuất
  localStorage.removeItem('token');
  router.push('/auth/dang-nhap');
};
</script>

<style scoped>
/* Không cần viết CSS ở đây vì Tài đã có các file CSS cũ.
   Vue sẽ tự động nhận CSS từ các file bạn đã import trong main.js 
   hoặc App.vue như account-profile.css, style.css... */
</style>
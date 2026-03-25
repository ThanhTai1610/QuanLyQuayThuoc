<script setup>
import '../../assets/css/forgot-password.css';
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import axiosClient from '../../api/axiosClient';

const router = useRouter();
const matKhauMoi = ref('');
const xacNhanMatKhau = ref('');
const loi = ref('');
const dangGui = ref(false);
const email = ref('');

onMounted(() => {
  const emailFromState = history.state?.email;
  if (emailFromState) {
    email.value = emailFromState;
  } else {
    router.push({ name: 'QuenMatKhau' });
  }
});

const luuMatKhau = async () => {
  loi.value = '';

  if (matKhauMoi.value.length < 6) {
    loi.value = 'Mật khẩu phải có ít nhất 6 ký tự.';
    return;
  }

  if (matKhauMoi.value !== xacNhanMatKhau.value) {
    loi.value = 'Mật khẩu xác nhận không khớp.';
    return;
  }

  dangGui.value = true;
  try {
    await axiosClient.post('/NguoiDung/dat-lai-mat-khau', {
      email: email.value,
      matKhauMoi: matKhauMoi.value,
    });

    alert('Đổi mật khẩu thành công! Vui lòng đăng nhập lại.');
    router.push({ name: 'DangNhap' });
  } catch (error) {
    loi.value = error.response?.data?.message || 'Có lỗi xảy ra. Vui lòng thử lại.';
  } finally {
    dangGui.value = false;
  }
};
</script>

<template>
  <div class="site-section auth-section forgot-step-section">
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-md-8 col-lg-5">
          <div class="auth-card forgot-auth-card">

            <div class="otp-stepper">
              <div class="otp-step-item completed"><div class="otp-step-circle">1</div></div>
              <div class="otp-step-item completed"><div class="otp-step-circle">2</div></div>
              <div class="otp-step-item active"><div class="otp-step-circle">3</div></div>
            </div>

            <h2 class="mb-2 text-center">Đặt mật khẩu mới</h2>
            <p class="mb-4 text-center sub-text">
              Thiết lập mật khẩu mới cho email:<br>
              <strong>{{ email }}</strong>
            </p>

            <div v-if="loi" class="alert alert-danger py-2 small text-center mb-3">
              {{ loi }}
            </div>

            <div class="form-group mb-3">
              <label for="newPassword">Mật khẩu mới</label>
              <input
                type="password"
                class="form-control"
                id="newPassword"
                v-model="matKhauMoi"
                :disabled="dangGui"
                placeholder="Nhập mật khẩu mới"
              />
            </div>

            <div class="form-group mb-3">
              <label for="confirmPassword">Xác nhận mật khẩu</label>
              <input
                type="password"
                class="form-control"
                id="confirmPassword"
                v-model="xacNhanMatKhau"
                :disabled="dangGui"
                placeholder="Nhập lại mật khẩu mới"
                @keyup.enter="luuMatKhau"
              />
            </div>

            <button
              type="button"
              class="btn btn-primary btn-block w-100 mt-2"
              :disabled="dangGui || !matKhauMoi || !xacNhanMatKhau"
              @click="luuMatKhau"
            >
              <span v-if="dangGui">
                <i class="fas fa-spinner fa-spin mr-2"></i> Đang cập nhật...
              </span>
              <span v-else>ĐỔI MẬT KHẨU</span>
            </button>

            <div class="small-links mt-4 text-center">
              <router-link to="/auth/dang-nhap" class="text-muted">Hủy bỏ</router-link>
            </div>

          </div>
        </div>
      </div>
    </div>
  </div>
</template>
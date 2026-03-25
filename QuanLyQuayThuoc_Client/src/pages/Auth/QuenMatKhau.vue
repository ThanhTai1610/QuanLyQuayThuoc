<template>
  <div class="site-section auth-section forgot-step-section">
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-md-8 col-lg-5">
          <div class="auth-card forgot-auth-card">

            <!-- Step indicator -->
            <div class="otp-stepper" aria-label="Quên mật khẩu - các bước">
              <div class="otp-step-item active">
                <div class="otp-step-circle">1</div>
              </div>
              <div class="otp-step-item">
                <div class="otp-step-circle">2</div>
              </div>
              <div class="otp-step-item">
                <div class="otp-step-circle">3</div>
              </div>
            </div>

            <h2 class="mb-2 text-center">Quên mật khẩu?</h2>
            <p class="mb-4 text-center sub-text">
              Nhập email của bạn để nhận mã OTP
            </p>

            <div class="form-group mb-3">
              <label for="email">Email</label>
              <input
                type="email"
                class="form-control"
                id="email"
                v-model="email"
                placeholder="Email"
                @keyup.enter="guiMaOtp"
              />
              <p v-if="loi" class="text-danger small mt-1">{{ loi }}</p>
            </div>

            <button
              type="button"
              class="btn btn-primary btn-block w-100 mt-2"
              :disabled="dangGui"
              @click="guiMaOtp"
            >
              <span v-if="dangGui">Đang gửi...</span>
              <span v-else>GỬI MÃ OTP</span>
            </button>

            <div class="small-links mt-4 text-center">
              <router-link to="/auth/dang-nhap">Quay lại trang đăng nhập</router-link>
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
import axiosClient from '../../api/axiosClient';
import '../../assets/css/forgot-password.css';

const router = useRouter();
const email = ref('');
const loi = ref('');
const dangGui = ref(false);

const guiMaOtp = async () => {
  loi.value = '';
  const emailTrim = email.value.trim();

  if (!emailTrim) {
    loi.value = 'Vui lòng nhập email.';
    return;
  }

  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!emailRegex.test(emailTrim)) {
    loi.value = 'Email không hợp lệ.';
    return;
  }

  dangGui.value = true;
  try {
    await axiosClient.post('/NguoiDung/quen-mat-khau', { email: emailTrim });
    router.push({
      name: 'QuenMatKhauOtp',
      state: { email: emailTrim }
    });
  } catch (error) {
    if (error.response?.status === 404) {
      loi.value = 'Email không tồn tại trong hệ thống.';
    } else {
      loi.value = 'Có lỗi xảy ra. Vui lòng thử lại.';
    }
  } finally {
    dangGui.value = false;
  }
};
</script>
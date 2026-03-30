<template>
  <div class="site-section auth-section forgot-step-section">
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-md-8 col-lg-5">
          <div class="auth-card forgot-auth-card">
            <div class="otp-stepper" aria-label="Quên mật khẩu - các bước">
              <div class="otp-step-item completed">
                <div class="otp-step-circle">1</div>
              </div>
              <div class="otp-step-item active">
                <div class="otp-step-circle">2</div>
              </div>
              <div class="otp-step-item">
                <div class="otp-step-circle">3</div>
              </div>
            </div>

            <h2 class="mb-2 text-center">Nhập mã OTP</h2>
            <p class="mb-3 text-center sub-text">
              Nhập OTP đã được gửi tới email của bạn
            </p>
            <p class="mb-4 text-center sub-text" style="margin-top: -6px;">
              <span class="text-muted">Email: </span><strong>{{ email }}</strong>
            </p>

            <div class="otp-error" :class="{ show: loi }">{{ loi }}</div>

            <div class="form-group mb-3">
              <label for="otp">Mã OTP</label>
              <input
                type="text"
                class="form-control"
                id="otp"
                v-model="otp"
                placeholder="Nhập mã OTP"
                maxlength="10"
                @keyup.enter="xacNhan"
              />
            </div>

            <button
              type="button"
              class="btn btn-primary btn-block w-100 mt-2"
              :disabled="dangGui"
              @click="xacNhan"
            >
              <span v-if="dangGui">Đang xác nhận...</span>
              <span v-else>XÁC NHẬN</span>
            </button>

            <div class="small-links mt-4 text-center">
              <router-link to="/quen-mat-khau">Quay lại bước 1</router-link>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import '../../assets/css/forgot-password.css';
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import axiosClient from '../../api/axiosClient';

const router = useRouter();
const email = ref('');
const otp = ref('');
const loi = ref('');
const dangGui = ref(false);

onMounted(() => {
  email.value = localStorage.getItem('pharmative_reset_email') || '(chưa có)';
});

const xacNhan = async () => {
  loi.value = '';
  const otpTrim = otp.value.trim();

  if (!otpTrim || otpTrim.length < 4) {
    loi.value = 'Vui lòng nhập mã OTP hợp lệ.';
    return;
  }

  dangGui.value = true;
  try {
    // Lưu ý: Key 'Email' và 'Otp' phải khớp với DTO ở Backend của bạn
    await axiosClient.post('/NguoiDung/xac-nhan-otp', {
      Email: email.value,
      Otp: otpTrim,
    });

    // LƯU VÀO LOCALSTORAGE TRƯỚC KHI CHUYỂN TRANG
    localStorage.setItem('pharmative_temp_otp', otpTrim);
    
    router.push({ name: 'DatLaiMatKhau' });
  } catch (error) {
    loi.value = error.response?.data?.message || 'Mã OTP không đúng hoặc đã hết hạn.';
  } finally {
    dangGui.value = false;
  }
};
</script>
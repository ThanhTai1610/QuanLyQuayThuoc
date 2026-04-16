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

  dangGui.value = true;
  try {
    // Gọi API và đợi phản hồi từ server
    const response = await axiosClient.post('/NguoiDung/quen-mat-khau', { email: emailTrim });

    // Khi xuống được đây (không nhảy vào catch) tức là API đã gọi thành công
    localStorage.setItem('pharmative_reset_email', emailTrim);
    // Chuyển sang Bước 2: Nhập OTP
    router.push({ name: 'QuenMatKhauOTP' });
  } catch (error) {
    // 3. Hiển thị lỗi chi tiết từ Backend (ví dụ: "Email không tồn tại" hoặc "Lỗi Database")
    console.error("Lỗi gửi OTP:", error);
    
    if (error.response) {
      // Lỗi từ server trả về (404, 500, 400)
      loi.value = error.response.data.message || 'Server đang gặp sự cố kết nối Database.';
    } else {
      // Lỗi mạng hoặc không kết nối được server
      loi.value = 'Không thể kết nối đến máy chủ. Tài kiểm tra lại Backend nhé!';
    }
  } finally {
    dangGui.value = false;
  }
};
</script>
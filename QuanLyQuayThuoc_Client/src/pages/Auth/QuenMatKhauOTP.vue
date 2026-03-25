<template>
  <div class="auth-card p-4 shadow">
    <h2 class="text-center">Xác nhận OTP</h2>
    <p class="text-center">Mã gửi đến: <strong>{{ email }}</strong></p>

    <div class="form-group mb-3">
      <label class="fw-bold">Nhập 6 số OTP</label>
      <input
        type="text"
        class="form-control text-center fw-bold"
        style="letter-spacing: 5px; font-size: 1.2rem;"
        v-model="otp" 
        placeholder="000000"
        maxlength="6"
        @keyup.enter="xacNhanOtp"
      />
      <p v-if="loi" class="text-danger small mt-2">{{ loi }}</p>
    </div>

    <button class="btn btn-primary w-100 fw-bold" :disabled="dangGui" @click="xacNhanOtp">
      <span v-if="dangGui">ĐANG XÁC NHẬN...</span>
      <span v-else>XÁC NHẬN</span>
    </button>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import axiosClient from '../../api/axiosClient';

const router = useRouter();
const email = ref(''); 
const otp = ref('');
const loi = ref('');
const dangGui = ref(false); // <--- TÀI THIẾU DÒNG NÀY NÊN KHÔNG BẤM ĐƯỢC

onMounted(() => {
  // Lấy từ LocalStorage - Nơi chắc chắn có dữ liệu (theo hình Tài gửi)
  const savedEmail = localStorage.getItem('pharmative_reset_email');
  if (savedEmail) {
    email.value = savedEmail;
  } else {
    loi.value = "Không tìm thấy email. Hãy quay lại bước 1.";
  }
});

const xacNhanOtp = async () => {
  loi.value = '';
  
  // Lấy trực tiếp từ LocalStorage ngay lúc này
  const emailChinhXac = localStorage.getItem('pharmative_reset_email');
  const otpValue = otp.value.trim();

  console.log("Dữ liệu chuẩn bị gửi:", { email: emailChinhXac, otp: otpValue });

  if (!emailChinhXac) {
    loi.value = "Lỗi: Mất email. Quay lại B1.";
    return;
  }
  if (!otpValue) {
    loi.value = "Vui lòng nhập mã OTP.";
    return;
  }

  dangGui.value = true;
  try {
    // GỬI CHÍNH XÁC OBJECT NÀY
    await axiosClient.post('/NguoiDung/xac-nhan-otp', {
      email: emailChinhXac, 
      otp: otpValue
    });
    
    // Lưu OTP để dùng cho Bước 3
    localStorage.setItem('pharmative_temp_otp', otpValue);
    
    // Chuyển sang Bước 3
    router.push({ name: 'DatLaiMatKhau' });
  } catch (error) {
    console.error("Lỗi API:", error.response?.data);
    loi.value = error.response?.data?.message || "OTP không đúng hoặc đã hết hạn!";
  } finally {
    dangGui.value = false;
  }
};
</script>
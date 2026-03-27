<template>
  <div class="site-section auth-section">
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-md-10 col-lg-7">
          <div class="auth-card">
            <h2 class="mb-4 text-center">{{ isStepOtp ? 'Xác thực OTP' : 'Đăng ký tài khoản' }}</h2>
            
            <div class="otp-error" :class="{ show: loi }" v-if="loi">{{ loi }}</div>

            <div v-if="!isStepOtp">
              <div class="form-row">
                <div class="form-group col-md-6">
                  <label>Họ</label>
                  <input type="text" class="form-control" v-model="form.ho" placeholder="Nhập họ" />
                </div>
                <div class="form-group col-md-6">
                  <label>Tên</label>
                  <input type="text" class="form-control" v-model="form.ten" placeholder="Nhập tên" />
                </div>
              </div>

              <div class="form-group">
                <label>Email</label>
                <input type="email" class="form-control" v-model="form.email" placeholder="Nhập địa chỉ email" />
              </div>

              <div class="form-group">
                <label>Số điện thoại</label>
                <input type="tel" class="form-control" v-model="form.soDienThoai" placeholder="Nhập số điện thoại" />
              </div>

              <div class="form-row">
                <div class="form-group col-md-6">
                  <label>Mật khẩu</label>
                  <input type="password" class="form-control" v-model="form.matKhau" placeholder="Tạo mật khẩu" />
                </div>
                <div class="form-group col-md-6">
                  <label>Xác nhận mật khẩu</label>
                  <input type="password" class="form-control" v-model="form.xacNhanMatKhau" placeholder="Nhập lại mật khẩu" />
                </div>
              </div>

              <button type="button" class="btn btn-primary btn-block py-2 mt-2" :disabled="dangGui" @click="handleGuiOtp">
                <span v-if="dangGui">Đang xử lý...</span>
                <span v-else>Tiếp tục</span>
              </button>
            </div>

            <div v-else>
              <p class="text-center">Mã OTP đã được gửi đến: <b>{{ form.email }}</b></p>
              <div class="form-group">
                <input type="text" class="form-control text-center" style="font-size: 24px; letter-spacing: 10px;" 
                       v-model="otpInput" maxlength="6" placeholder="******" />
              </div>
              <button type="button" class="btn btn-primary btn-block py-2 mt-2" :disabled="dangGui" @click="hoanTatDangKy">
                <span v-if="dangGui">Đang xác thực...</span>
                <span v-else>Xác nhận & Đăng ký</span>
              </button>
              <button type="button" class="btn btn-link btn-block btn-sm mt-2" @click="isStepOtp = false">Quay lại</button>
            </div>

            <p class="mt-4 mb-0 text-center small" v-if="!isStepOtp">
              Đã có tài khoản? <router-link :to="{ name: 'DangNhap' }">Đăng nhập ngay</router-link>
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import '../../assets/css/register.css';
import { ref, reactive } from 'vue';
import { useRouter } from 'vue-router';
import axiosClient from '../../api/axiosClient';

const router = useRouter();
const loi = ref('');
const dangGui = ref(false);
const isStepOtp = ref(false); 
const otpInput = ref('');

const form = reactive({
  ho: '',
  ten: '',
  email: '',
  soDienThoai: '',
  matKhau: '',
  xacNhanMatKhau: '',
});

// HÀM BƯỚC 1: KIỂM TRA THÔNG TIN & GỬI OTP
const handleGuiOtp = async () => {
  loi.value = '';

  // 1. Kiểm tra trống các trường cơ bản
  if (!form.ho.trim() || !form.ten.trim()) {
    loi.value = 'Vui lòng nhập đầy đủ Họ và Tên.';
    return;
  }

  // 2. Validate Email
  const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
  if (!emailRegex.test(form.email.trim())) {
    loi.value = 'Email không đúng định dạng (VD: tai@gmail.com).';
    return;
  }

  // 3. Validate Số điện thoại (Đầu số VN, 10 số)
  const phoneRegex = /^(0[3|5|7|8|9])([0-9]{8})$/;
  if (!phoneRegex.test(form.soDienThoai.trim())) {
    loi.value = 'Số điện thoại không hợp lệ (Phải có 10 số, đầu 03, 05, 07, 08, 09).';
    return;
  }

  // 4. Validate Mật khẩu (Tối thiểu 6 ký tự)
  if (form.matKhau.length < 6) {
    loi.value = 'Mật khẩu phải từ 6 ký tự trở lên để đảm bảo an toàn.';
    return;
  }

  // 5. Kiểm tra khớp mật khẩu
  if (form.matKhau !== form.xacNhanMatKhau) {
    loi.value = 'Mật khẩu xác nhận không trùng khớp.';
    return;
  }

  dangGui.value = true;
  try {
    // API gửi OTP (Backend của Tài cần kiểm tra Email tồn tại chưa ở đây luôn)
    await axiosClient.post('/NguoiDung/gui-otp-dang-ky', { 
        email: form.email.trim() 
    });
    isStepOtp.value = true;
  } catch (error) {
    loi.value = error.response?.data?.message || 'Email này đã được sử dụng hoặc lỗi hệ thống.';
  } finally {
    dangGui.value = false;
  }
};

// HÀM BƯỚC 2: XÁC THỰC & TẠO TÀI KHOẢN
const hoanTatDangKy = async () => {
  loi.value = '';

  if (!/^\d{6}$/.test(otpInput.value)) {
    loi.value = 'Mã OTP phải là 6 chữ số.';
    return;
  }

  dangGui.value = true;
  try {
    await axiosClient.post('/NguoiDung/dang-ky-otp', {
      ho: form.ho.trim(),
      ten: form.ten.trim(),
      email: form.email.trim(),
      soDienThoai: form.soDienThoai.trim(),
      matKhau: form.matKhau,
      maOtp: otpInput.value.trim()
    });
    
    alert('Chúc mừng Tài! Đăng ký tài khoản Pharmative thành công.');
    router.push({ name: 'DangNhap' });
  } catch (error) {
    loi.value = error.response?.data?.message || 'Mã xác thực không đúng. Vui lòng kiểm tra lại.';
  } finally {
    dangGui.value = false;
  }
};
</script>
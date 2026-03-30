<template>
  <div class="site-section auth-section register-section">
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-md-10 col-lg-7">
          <div class="auth-card shadow-lg p-4 bg-white rounded">
            
            <h2 class="mb-4 text-center font-weight-bold text-primary">
              {{ isStepOtp ? 'XÁC THỰC OTP' : 'ĐĂNG KÝ TÀI KHOẢN' }}
            </h2>

            <div v-if="!isStepOtp">
              <div class="form-row">
                <div class="form-group col-md-6">
                  <label>Họ <span class="text-danger">*</span></label>
                  <input type="text" class="form-control" v-model="form.ho" placeholder="Nhập họ" :disabled="dangGui" />
                </div>
                <div class="form-group col-md-6">
                  <label>Tên <span class="text-danger">*</span></label>
                  <input type="text" class="form-control" v-model="form.ten" placeholder="Nhập tên" :disabled="dangGui" />
                </div>
              </div>

              <div class="form-group">
                <label>Email <span class="text-danger">*</span></label>
                <input type="email" class="form-control" v-model="form.email" placeholder="nguyenvan@gmail.com" :disabled="dangGui" />
              </div>

              <div class="form-group">
                <label>Số điện thoại <span class="text-danger">*</span></label>
                <input type="tel" class="form-control" v-model="form.soDienThoai" placeholder="03xxxxxxx" :disabled="dangGui" />
              </div>

              <div class="form-row">
                <div class="form-group col-md-6">
                  <label>Mật khẩu <span class="text-danger">*</span></label>
                  <input type="password" class="form-control" v-model="form.matKhau" placeholder="Tối thiểu 6 ký tự" :disabled="dangGui" />
                </div>
                <div class="form-group col-md-6">
                  <label>Xác nhận mật khẩu <span class="text-danger">*</span></label>
                  <input type="password" class="form-control" v-model="form.xacNhanMatKhau" placeholder="Nhập lại mật khẩu" :disabled="dangGui" />
                </div>
              </div>

              <div v-if="loi" class="alert alert-danger py-2 mb-3 small">
                <i class="fas fa-exclamation-triangle mr-2"></i> {{ loi }}
              </div>

              <button type="button" class="btn btn-primary btn-block py-2 font-weight-bold" :disabled="dangGui" @click="handleGuiOtp">
                <span v-if="dangGui"><i class="fas fa-spinner fa-spin mr-2"></i> ĐANG GỬI MÃ...</span>
                <span v-else>TIẾP TỤC ĐĂNG KÝ</span>
              </button>
            </div>

            <div v-else>
              <div class="alert alert-info text-center small mb-4">
                Mã xác thực đã được gửi đến:<br>
                <strong>{{ form.email }}</strong>
              </div>
              
              <div class="form-group text-center">
                <label class="d-block mb-3">Nhập mã OTP (6 chữ số)</label>
                <input type="text" class="form-control text-center mx-auto" 
                       style="font-size: 28px; letter-spacing: 10px; font-weight: bold; width: 250px;" 
                       v-model="otpInput" maxlength="6" placeholder="000000" 
                       :disabled="dangGui"
                       @keyup.enter="hoanTatDangKy" />
              </div>

              <div v-if="loi" class="alert alert-danger py-2 mb-3 text-center small">
                {{ loi }}
              </div>

              <button type="button" class="btn btn-success btn-block py-2 font-weight-bold" 
                      :disabled="dangGui || otpInput.length < 6" @click="hoanTatDangKy">
                <span v-if="dangGui"><i class="fas fa-spinner fa-spin mr-2"></i> ĐANG XÁC THỰC...</span>
                <span v-else>XÁC NHẬN & HOÀN TẤT</span>
              </button>
              
              <button type="button" class="btn btn-link btn-block btn-sm mt-3 text-muted" 
                      @click="isStepOtp = false" :disabled="dangGui">
                 <i class="fas fa-arrow-left mr-1"></i> Quay lại sửa thông tin
              </button>
            </div>

            <p class="mt-4 mb-0 text-center small" v-if="!isStepOtp">
              Đã có tài khoản? <router-link :to="{ name: 'DangNhap' }" class="text-primary font-weight-bold">Đăng nhập ngay</router-link>
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import '../../assets/css/register.css';
import { ref, reactive, watch } from 'vue';
import { useRouter } from 'vue-router';
import axiosClient from '../../api/axiosClient';
import Swal from 'sweetalert2';

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

// Xóa lỗi khi người dùng bắt đầu gõ lại
watch([form, otpInput], () => { if (loi.value) loi.value = ''; }, { deep: true });

const handleGuiOtp = async () => {
  loi.value = '';

  // --- VALIDATE FRONTEND ---
  if (!form.ho.trim() || !form.ten.trim()) {
    loi.value = 'Vui lòng điền đầy đủ Họ và Tên.';
    return;
  }

  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!emailRegex.test(form.email.trim())) {
    loi.value = 'Email không hợp lệ.';
    return;
  }

  const sdtRegex = /^(0[3|5|7|8|9])([0-9]{8})$/;
  if (!sdtRegex.test(form.soDienThoai.trim())) {
    loi.value = 'Số điện thoại phải 10 số (03, 05, 07, 08, 09).';
    return;
  }

  if (form.matKhau.length < 6) {
    loi.value = 'Mật khẩu phải từ 6 ký tự trở lên.';
    return;
  }

  if (form.matKhau !== form.xacNhanMatKhau) {
    loi.value = 'Mật khẩu xác nhận không khớp.';
    return;
  }

  dangGui.value = true;
  try {
    // Gọi API gửi Mail OTP
    const res = await axiosClient.post('/NguoiDung/gui-otp-dang-ky', { 
        Email: form.email.trim() 
    });
    
    // Bóc tách data linh hoạt (phòng trường hợp axiosClient trả về res.data hoặc res)
    const dataRes = res.data || res;
    
    // Lưu OTP vào localStorage (Lưu ý: Backend của Tài cần trả về đúng trường này)
    if (dataRes.otpXacThuc || dataRes.otp) {
      localStorage.setItem('temp_reg_otp', dataRes.otpXacThuc || dataRes.otp);
      isStepOtp.value = true;
      Swal.fire('Thông báo', 'Mã OTP đã được gửi vào Email của Tài!', 'info');
    } else {
      loi.value = 'Không nhận được mã xác thực từ máy chủ.';
    }

  } catch (error) {
    // Hiển thị lỗi thật từ Backend (Ví dụ: Email đã tồn tại)
    loi.value = error.response?.data?.message || 'Lỗi kết nối máy chủ khi gửi OTP.';
  } finally {
    dangGui.value = false;
  }
};

const hoanTatDangKy = async () => {
  loi.value = '';
  const otpGoc = localStorage.getItem('temp_reg_otp');

  if (otpInput.value.trim() !== String(otpGoc)) {
    loi.value = 'Mã OTP không chính xác.';
    return;
  }

  dangGui.value = true;
  try {
    await axiosClient.post('/NguoiDung/dang-ky-otp', {
      Ho: form.ho.trim(),
      Ten: form.ten.trim(),
      Email: form.email.trim(),
      SoDienThoai: form.soDienThoai.trim(),
      MatKhau: form.matKhau
    });
    
    localStorage.removeItem('temp_reg_otp');
    
    await Swal.fire('Thành công', 'Tài khoản của Tài đã được tạo thành công!', 'success');
    router.push({ name: 'DangNhap' });

  } catch (error) {
    loi.value = error.response?.data?.message || 'Đăng ký thất bại. Vui lòng thử lại.';
  } finally {
    dangGui.value = false;
  }
};
</script>
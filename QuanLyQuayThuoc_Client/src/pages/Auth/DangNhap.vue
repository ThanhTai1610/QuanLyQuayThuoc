<template>
    
  <div class="site-section auth-section">
    
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-md-8 col-lg-5">
          <div class="auth-card">
            <h2 class="mb-4 text-center">Đăng nhập</h2>
            <p class="mb-4 text-center text-muted">Vui lòng đăng nhập để tiếp tục mua sắm tại Pharmative.</p>
            
            <form @submit.prevent="handleLogin">
              <div class="form-group">
                <label for="email">Email</label>
                <input 
                  v-model="auth.email"
                  type="email" 
                  class="form-control" 
                  id="email" 
                  placeholder="Nhập địa chỉ email"
                  required
                >
              </div>
              <div class="form-group">
                <label for="password">Mật khẩu</label>
                <input 
                  v-model="auth.password"
                  type="password" 
                  class="form-control" 
                  id="password" 
                  placeholder="Nhập mật khẩu"
                  required
                >
              </div>
              
              <div class="d-flex justify-content-between align-items-center mb-3 mt-2">
                <div class="custom-control custom-checkbox">
                  <input 
                    v-model="auth.remember"
                    type="checkbox" 
                    class="custom-control-input" 
                    id="remember"
                  >
                  <label class="custom-control-label" for="remember">Ghi nhớ đăng nhập</label>
                </div>
                <router-link to="/auth/quen-mat-khau" class="small">Quên mật khẩu?</router-link>
              </div>
              
              <button type="submit" class="btn btn-primary btn-block py-2">Đăng nhập</button>
            </form>
            
            <p class="mt-4 mb-0 text-center small">
              Chưa có tài khoản?
              <router-link :to="{ name: 'DangKy' }">Đăng ký ngay</router-link>
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue';
import { useRouter } from 'vue-router';
import axiosClient from '../../api/axiosClient';

const router = useRouter();
const loi = ref(''); // Hiển thị thông báo lỗi lên màn hình
const dangGui = ref(false); // Trạng thái loading

const auth = reactive({
  email: '',
  password: '',
  remember: false
});

const handleLogin = async () => {
  // 1. Reset trạng thái
  loi.value = '';
  const emailTrim = auth.email.trim();

  // 2. Validate phía Client (Chặn trước khi gọi API)
  if (!emailTrim) {
    loi.value = 'Vui lòng nhập email.';
    return;
  }

  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!emailRegex.test(emailTrim)) {
    loi.value = 'Email không đúng định dạng.';
    return;
  }

  if (auth.password.length < 6) {
    loi.value = 'Mật khẩu phải có ít nhất 6 ký tự.';
    return;
  }

  // 3. Bắt đầu gửi dữ liệu
  dangGui.value = true;
  try {
    const response = await axiosClient.post('/NguoiDung/dang-nhap', {
      email: emailTrim,
      matKhau: auth.password
    });

    if (response.token) {
      localStorage.setItem('token', response.token);
      localStorage.setItem('user', JSON.stringify(response.user));
      
      const roleId = response.user.maVaiTro; 

      // Điều hướng dựa trên vai trò
      if (roleId === 1) router.push('/admin/thong-ke');
      else if (roleId === 2) router.push('/nhan-vien/ban-hang');
      else router.push('/');
    } else {
      loi.value = "Không nhận được mã xác thực từ máy chủ.";
    }
  } catch (error) {
    // Hiển thị lỗi từ Backend trả về
    loi.value = error.response?.data?.message || "Email hoặc mật khẩu không chính xác!";
  } finally {
    dangGui.value = false;
  }
};
</script>

<style scoped>
/* Giữ nguyên các class từ login.css của bạn */
.auth-section {
  padding: 80px 0;
  min-height: 80vh;
  display: flex;
  align-items: center;
}

.auth-card {
  background: #fff;
  padding: 40px;
  border-radius: 10px;
  box-shadow: 0 15px 35px rgba(0, 0, 0, 0.1);
}
</style>
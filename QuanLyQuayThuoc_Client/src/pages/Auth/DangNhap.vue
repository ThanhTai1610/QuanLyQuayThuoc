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
              <router-link to="/auth/dang-ky">Đăng ký ngay</router-link>
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive } from 'vue';
import { useRouter } from 'vue-router';
import axiosClient from '../../api/axiosClient'; // Kiểm tra lại đường dẫn file này

const router = useRouter();
const auth = reactive({
  email: '',
  password: '',
  remember: false
});

const handleLogin = async () => {
  try {
    const data = await axiosClient.post('/NguoiDung/dang-nhap', {
      email: auth.email,
      matKhau: auth.password
    });

    if (data.token) {
      localStorage.setItem('token', data.token);
      localStorage.setItem('user', JSON.stringify(data.user));
      
      const roleId = data.user.maVaiTro; // Lấy mã vai trò từ Backend trả về

      // PHÂN QUYỀN ĐIỀU HƯỚNG
      switch (roleId) {
        case 1: // Giả sử 1 là Admin
          alert("Chào mừng Quản trị viên: " + data.user.hoTen);
          router.push('/admin/thong-ke');
          break;
        case 2: // Giả sử 2 là Nhân viên
          alert("Chào mừng Nhân viên: " + data.user.hoTen);
          router.push('/nhan-vien/ban-hang');
          break;
        case 3: // Giả sử 3 là Khách hàng
          alert("Chào mừng " + data.user.hoTen);
          router.push('/');
          break;
        default:
          router.push('');
          break;
      }
    }
  } catch (error) {
    console.error(error);
    alert(error.response?.data?.message || "Đăng nhập thất bại!");
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
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
import axiosClient from '../../api/axiosClient';
 // Kiểm tra lại đường dẫn file này

const router = useRouter();
const auth = reactive({
  email: '',
  password: '',
  remember: false
});

const handleLogin = async () => {
  try {
    // Gửi request đăng nhập
    const response = await axiosClient.post('/NguoiDung/dang-nhap', {
      email: auth.email,
      matKhau: auth.password
    });

    // Kiểm tra nếu có token (Backend phải trả về trường này)
    if (response.token) {
      localStorage.setItem('token', response.token);
      localStorage.setItem('user', JSON.stringify(response.user));
      
      const roleId = response.user.maVaiTro; 

      // Điều hướng dựa trên MaVaiTro từ database
      if (roleId === 1) {
        router.push('/admin/thong-ke');
      } else if (roleId === 2) {
        router.push('/nhan-vien/ban-hang');
      } else {
        router.push('/');
      }
    } else {
      alert("Đăng nhập thành công nhưng không nhận được mã xác thực!");
    }
  } catch (error) {
    console.error("Lỗi đăng nhập:", error);
    alert(error.response?.data?.message || "Email hoặc mật khẩu không đúng!");
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
<template>
  <div class="site-section auth-section">
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-md-8 col-lg-5">
          <div class="auth-card">
            <h2 class="mb-4 text-center">Đăng nhập</h2>
            <p class="mb-4 text-center text-muted">Vui lòng đăng nhập để tiếp tục mua sắm tại Pharmative.</p>
            
            <div v-if="loi" class="alert alert-danger py-2 mb-3 small text-center">
              <i class="fas fa-exclamation-circle mr-1"></i> {{ loi }}
            </div>

            <form @submit.prevent="handleLogin">
              <div class="form-group">
                <label for="email">Email</label>
                <input 
                  v-model="auth.email"
                  type="email" 
                  class="form-control" 
                  id="email" 
                  placeholder="Nhập địa chỉ email"
                  :disabled="dangGui"
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
                  :disabled="dangGui"
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
              
              <button type="submit" class="btn btn-primary btn-block py-2 font-weight-bold" :disabled="dangGui">
                <span v-if="dangGui"><i class="fas fa-spinner fa-spin mr-2"></i> ĐANG XỬ LÝ...</span>
                <span v-else>ĐĂNG NHẬP</span>
              </button>
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
import Swal from 'sweetalert2';
import { authState } from '../../api/auth'; // Import để cập nhật tên lên Header ngay lập tức

const router = useRouter();
const loi = ref(''); 
const dangGui = ref(false); 

const auth = reactive({
  email: '',
  password: '',
  remember: false
});

const handleLogin = async () => {
  loi.value = '';
  const emailTrim = auth.email.trim();

  // Validate đơn giản phía Client
  if (!emailTrim || auth.password.length < 6) {
    loi.value = 'Vui lòng kiểm tra lại Email và Mật khẩu (tối thiểu 6 ký tự).';
    return;
  }

  dangGui.value = true;
  try {
    const response = await axiosClient.post('/NguoiDung/dang-nhap', {
      email: emailTrim,
      matKhau: auth.password
    });

    // Bóc tách dữ liệu linh hoạt (phòng trường hợp axiosClient trả về response.data)
    const dataRes = response.data || response;

    if (dataRes.token) {
      // 1. Lưu vào LocalStorage
      localStorage.setItem('token', dataRes.token);
      localStorage.setItem('user', JSON.stringify(dataRes.user));
      
      // 2. Cập nhật State Global (Để Header đổi chữ "Đăng nhập" thành "Chào Tài")
      authState.login(dataRes.user);

      // 3. Thông báo thành công giống Đăng xuất
      await Swal.fire({
        title: 'Đăng nhập thành công!',
        text: `Chào mừng ${dataRes.user.hoTen || 'bạn'} quay trở lại!`,
        icon: 'success',
        timer: 2000,
        showConfirmButton: false
      });

      // 4. Điều hướng dựa trên vai trò
      const roleId = dataRes.user.maVaiTro; 
      if (roleId === 1) router.push('/admin/thong-ke');
      else if (roleId === 2) router.push('/nhan-vien/ban-hang');
      else router.push('/');
      
    } else {
      loi.value = "Máy chủ không phản hồi mã xác thực.";
    }
  } catch (error) {
    loi.value = error.response?.data?.message || "Email hoặc mật khẩu không chính xác!";
    // Thông báo lỗi kiểu SweetAlert nếu muốn
    Swal.fire('Thất bại', loi.value, 'error');
  } finally {
    dangGui.value = false;
  }
};
</script>
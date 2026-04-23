<template>
  <div class="site-section auth-section">
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-md-8 col-lg-5">
          <div class="auth-card">
            <h2 class="mb-4 text-center">Đăng nhập</h2>
            <p class="mb-4 text-center text-muted">
              Vui lòng đăng nhập để tiếp tục mua sắm tại Pharmative.
            </p>

            <div v-if="loi" class="alert alert-danger py-2 mb-3 small text-center">
              <i class="fas fa-exclamation-circle mr-1"></i> {{ loi }}
            </div>

            <form @submit.prevent="handleLogin">
              <div class="form-group">
                <label for="email">Email</label>
                <input
                  id="email"
                  v-model="auth.email"
                  type="email"
                  class="form-control"
                  placeholder="Nhập địa chỉ email"
                  :disabled="dangGui || googleDangGui"
                  required
                >
              </div>

              <div class="form-group">
                <label for="password">Mật khẩu</label>
                <input
                  id="password"
                  v-model="auth.password"
                  type="password"
                  class="form-control"
                  placeholder="Nhập mật khẩu"
                  :disabled="dangGui || googleDangGui"
                  required
                >
              </div>

              <div class="d-flex justify-content-between align-items-center mb-3 mt-2">
                <div class="custom-control custom-checkbox">
                  <input
                    id="remember"
                    v-model="auth.remember"
                    type="checkbox"
                    class="custom-control-input"
                    :disabled="dangGui || googleDangGui"
                  >
                  <label class="custom-control-label" for="remember">Ghi nhớ đăng nhập</label>
                </div>

                <router-link to="/auth/quen-mat-khau" class="small">Quên mật khẩu?</router-link>
              </div>

              <button
                type="submit"
                class="btn btn-primary btn-block py-2 font-weight-bold"
                :disabled="dangGui || googleDangGui"
              >
                <span v-if="dangGui"><i class="fas fa-spinner fa-spin mr-2"></i> ĐANG XỬ LÝ...</span>
                <span v-else>ĐĂNG NHẬP</span>
              </button>
            </form>

            <div class="login-divider">
              <span>hoặc</span>
            </div>

            <div v-if="googleDaCauHinh" class="google-login-shell">
              <div ref="googleButtonRef" class="google-button-host"></div>
              <small v-if="googleDangGui" class="text-muted d-block mt-2">
                Đang xác thực Google...
              </small>
            </div>

            <div v-else class="google-config-note">
              Cần cấu hình `VITE_GOOGLE_CLIENT_ID` để bật đăng nhập Google.
            </div>

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
import { computed, onBeforeUnmount, onMounted, reactive, ref } from 'vue';
import { useRouter } from 'vue-router';
import Swal from 'sweetalert2';
import axiosClient from '../../api/axiosClient';
import { authState } from '../../api/auth';

const router = useRouter();
const loi = ref('');
const dangGui = ref(false);
const googleDangGui = ref(false);
const googleButtonRef = ref(null);

const googleClientId = (import.meta.env.VITE_GOOGLE_CLIENT_ID || '').trim();
const googleDaCauHinh = computed(() => Boolean(googleClientId));

const auth = reactive({
  email: '',
  password: '',
  remember: false
});

const dieuHuongTheoVaiTro = (roleId) => {
  if (roleId === 1) {
    router.push('/admin/thong-ke');
    return;
  }

  if (roleId === 2) {
    router.push('/nhan-vien/ban-hang');
    return;
  }

  router.push('/');
};

const xuLyDangNhapThanhCong = async (dataRes) => {
  localStorage.setItem('token', dataRes.token);
  localStorage.setItem('user', JSON.stringify(dataRes.user));
  authState.login(dataRes.user);

  await Swal.fire({
    title: 'Đăng nhập thành công!',
    text: `Chào mừng ${dataRes.user.hoTen || 'bạn'} quay trở lại!`,
    icon: 'success',
    timer: 2000,
    showConfirmButton: false
  });

  dieuHuongTheoVaiTro(dataRes.user.maVaiTro);
};

const loadGoogleScript = () =>
  new Promise((resolve, reject) => {
    if (window.google?.accounts?.id) {
      resolve(window.google);
      return;
    }

    let script = document.querySelector('script[data-google-identity]');
    if (script) {
      script.addEventListener('load', () => resolve(window.google), { once: true });
      script.addEventListener('error', () => reject(new Error('Không tải được Google Sign-In script.')), { once: true });
      return;
    }

    script = document.createElement('script');
    script.src = 'https://accounts.google.com/gsi/client';
    script.async = true;
    script.defer = true;
    script.dataset.googleIdentity = 'true';
    script.addEventListener('load', () => resolve(window.google), { once: true });
    script.addEventListener('error', () => reject(new Error('Không tải được Google Sign-In script.')), { once: true });
    document.head.appendChild(script);
  });

const handleGoogleCredential = async (response) => {
  if (!response?.credential) {
    loi.value = 'Google không trả về thông tin đăng nhập hợp lệ.';
    return;
  }

  loi.value = '';
  googleDangGui.value = true;

  try {
    const dataRes = await axiosClient.post('/NguoiDung/dang-nhap-google', {
      idToken: response.credential
    });

    if (!dataRes?.token) {
      loi.value = 'Máy chủ không phản hồi mã xác thực.';
      return;
    }

    await xuLyDangNhapThanhCong(dataRes);
  } catch (error) {
    loi.value = error.response?.data?.message || 'Đăng nhập Google thất bại.';
    Swal.fire('Thất bại', loi.value, 'error');
  } finally {
    googleDangGui.value = false;
  }
};

const khoiTaoDangNhapGoogle = async () => {
  if (!googleDaCauHinh.value || !googleButtonRef.value) {
    return;
  }

  try {
    await loadGoogleScript();

    googleButtonRef.value.innerHTML = '';
    window.google.accounts.id.initialize({
      client_id: googleClientId,
      callback: handleGoogleCredential
    });

    window.google.accounts.id.renderButton(googleButtonRef.value, {
      theme: 'outline',
      size: 'large',
      width: googleButtonRef.value.offsetWidth || 360,
      text: 'signin_with',
      shape: 'rectangular'
    });
  } catch (error) {
    loi.value = error.message || 'Không thể khởi tạo đăng nhập Google.';
  }
};

const handleLogin = async () => {
  loi.value = '';
  const emailTrim = auth.email.trim();

  if (!emailTrim || auth.password.length < 6) {
    loi.value = 'Vui lòng kiểm tra lại Email và Mật khẩu (tối thiểu 6 ký tự).';
    return;
  }

  dangGui.value = true;

  try {
    const dataRes = await axiosClient.post('/NguoiDung/dang-nhap', {
      email: emailTrim,
      matKhau: auth.password
    });

    if (!dataRes?.token) {
      loi.value = 'Máy chủ không phản hồi mã xác thực.';
      return;
    }

    await xuLyDangNhapThanhCong(dataRes);
  } catch (error) {
    loi.value = error.response?.data?.message || 'Email hoặc mật khẩu không chính xác!';
    Swal.fire('Thất bại', loi.value, 'error');
  } finally {
    dangGui.value = false;
  }
};

onMounted(() => {
  khoiTaoDangNhapGoogle();
});

onBeforeUnmount(() => {
  if (window.google?.accounts?.id?.cancel) {
    window.google.accounts.id.cancel();
  }
});
</script>

<style scoped>
.login-divider {
  display: flex;
  align-items: center;
  gap: 12px;
  margin: 24px 0 18px;
  color: #7c8798;
  font-size: 0.9rem;
}

.login-divider::before,
.login-divider::after {
  content: '';
  flex: 1;
  height: 1px;
  background: #e5e9f2;
}

.google-login-shell {
  min-height: 48px;
}

.google-button-host {
  display: flex;
  justify-content: center;
}

.google-config-note {
  padding: 10px 12px;
  border: 1px dashed #d6dcea;
  border-radius: 10px;
  background: #f7f9fc;
  color: #5a6780;
  font-size: 0.9rem;
  text-align: center;
}
</style>

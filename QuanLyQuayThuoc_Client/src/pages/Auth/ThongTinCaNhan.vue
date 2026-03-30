<template>
  <div class="site-wrap">
    <div class="account-wrapper py-5 bg-light">
      <div class="container">
        <div class="row">
          <AccountSidebar 
            :user="nguoiDung" 
            activeMenu="profile" 
            activeTitle="Thông tin cá nhân"
            @open-avatar-modal="moModalTuyChonAnh"
            @logout="dangXuat"
          />

          <div class="col-lg-9">
            <div class="account-content-card shadow-sm border-0 bg-white">
              <div class="account-content-header border-bottom p-4 d-flex justify-content-between align-items-center">
                <div>
                  <h2 class="mb-0 h4 font-weight-bold text-dark">Hồ Sơ Của Tôi</h2>
                  <p class="text-muted small mb-0">Quản lý thông tin hồ sơ để bảo mật tài khoản</p>
                </div>
                <span class="badge bg-soft-primary text-primary px-3 py-2 rounded-pill">
                  <i class="fa fa-shield-alt me-1"></i> {{ nguoiDung.tenVaiTro }}
                </span>
              </div>
              
              <div class="account-profile-body p-4">
                <div class="account-profile-avatar text-center mb-5">
                  <div class="avatar-wrapper position-relative d-inline-block">
                    <div class="avatar-circle shadow-sm" @click="moModalTuyChonAnh">
                      <img v-if="nguoiDung.anhDaiDien" 
                           :src="getFullUrl(nguoiDung.anhDaiDien)" 
                           class="img-fluid rounded-circle">
                      <div v-else class="avatar-placeholder">
                        <i class="fa fa-user text-secondary"></i>
                      </div>
                    </div>
                    <button class="btn-change-avatar shadow" @click="moModalTuyChonAnh" title="Đổi ảnh đại diện">
                      <i class="fa fa-camera"></i>
                    </button>
                  </div>
                  <h5 class="mt-3 font-weight-bold mb-0">{{ nguoiDung.hoTen }}</h5>
                  <p class="text-muted small">{{ nguoiDung.email }}</p>
                </div>
                
                <div class="info-grid mb-5">
                  <div class="row g-0 border-bottom py-3">
                    <div class="col-sm-4 text-muted"><i class="fa fa-id-card me-2"></i> Họ và tên</div>
                    <div class="col-sm-8 font-weight-bold text-dark text-sm-end">{{ nguoiDung.hoTen }}</div>
                  </div>
                  <div class="row g-0 border-bottom py-3">
                    <div class="col-sm-4 text-muted"><i class="fa fa-phone me-2"></i> Số điện thoại</div>
                    <div class="col-sm-8 text-dark text-sm-end">{{ nguoiDung.soDienThoai }}</div>
                  </div>
                  <div class="row g-0 border-bottom py-3">
                    <div class="col-sm-4 text-muted"><i class="fa fa-envelope me-2"></i> Email</div>
                    <div class="col-sm-8 text-dark text-sm-end">{{ nguoiDung.email }}</div>
                  </div>
                  <div class="row g-0 border-bottom py-3">
                    <div class="col-sm-4 text-muted"><i class="fa fa-venus-mars me-2"></i> Giới tính</div>
                    <div class="col-sm-8 text-sm-end">{{ nguoiDung.gioiTinh || 'Chưa cập nhật' }}</div>
                  </div>
                  <div class="row g-0 border-bottom py-3">
                    <div class="col-sm-4 text-muted"><i class="fa fa-calendar-alt me-2"></i> Ngày sinh</div>
                    <div class="col-sm-8 text-primary text-sm-end font-weight-bold">{{ dinhDangNgay(nguoiDung.ngaySinh) }}</div>
                  </div>
                </div>

                <div class="text-center">
                  <button class="btn btn-primary px-5 py-2 rounded-pill shadow" @click="moModalCapNhat">
                    <i class="fa fa-edit me-2"></i> Chỉnh sửa thông tin
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="modal fade" id="modalTuyChonAnh" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog modal-sm modal-dialog-centered">
        <div class="modal-content border-0 shadow-lg overflow-hidden" style="border-radius: 15px;">
          <div class="list-group list-group-flush text-center">
            <button v-if="nguoiDung.anhDaiDien" class="list-group-item list-group-item-action py-3 fw-bold" @click="xemAnhFull">
              <i class="fa fa-image me-2 text-info"></i> Xem ảnh đại diện
            </button>
            <button class="list-group-item list-group-item-action py-3 text-primary fw-bold" @click="triggerUpload">
              <i class="fa fa-upload me-2"></i> Chọn ảnh mới
            </button>
            <button v-if="nguoiDung.anhDaiDien" class="list-group-item list-group-item-action py-3 text-danger fw-bold" @click="xuLyXoaAvatar">
              <i class="fa fa-trash-alt me-2"></i> Xóa ảnh hiện tại
            </button>
            <button class="list-group-item list-group-item-action py-3 text-secondary" data-bs-dismiss="modal">Hủy</button>
          </div>
        </div>
      </div>
    </div>

    <div class="modal fade" id="modalXemAnh" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered modal-md">
        <div class="modal-content bg-transparent border-0">
          <div class="modal-body p-0 text-center position-relative">
            <button type="button" class="btn-close btn-close-white position-absolute shadow" style="top: -40px; right: 0; z-index: 9999" data-bs-dismiss="modal"></button>
            <img :src="getFullUrl(nguoiDung.anhDaiDien)" class="img-fluid rounded shadow-lg border border-white border-4" style="max-height: 80vh;">
          </div>
        </div>
      </div>
    </div>

    <div class="modal fade" id="modalChinhSua" data-bs-backdrop="static" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content border-0 shadow-lg" style="border-radius: 15px;">
          <div class="modal-header bg-light border-0 py-3">
            <h5 class="modal-title font-weight-bold text-dark"><i class="fa fa-user-edit me-2 text-primary"></i> Cập nhật hồ sơ</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
          </div>
          <div class="modal-body p-4">
            <form @submit.prevent="xuLyCapNhat" novalidate>
              <div class="mb-3">
                <label class="form-label small font-weight-bold text-uppercase text-muted">Họ và tên <span class="text-danger">*</span></label>
                <div class="input-group has-validation">
                  <span class="input-group-text bg-light border-end-0"><i class="fa fa-user text-muted"></i></span>
                  <input type="text" class="form-control bg-light border-start-0" 
                         v-model="formCapNhat.hoTen" 
                         @blur="validateTen"
                         :class="{'is-invalid': errors.hoTen}"
                         placeholder="Nhập họ tên của bạn">
                  <div class="invalid-feedback">{{ errors.hoTen }}</div>
                </div>
              </div>

              <div class="mb-3">
                <label class="form-label small font-weight-bold text-uppercase text-muted">Email <span class="text-danger">*</span></label>
                <div class="input-group has-validation">
                  <span class="input-group-text bg-light border-end-0"><i class="fa fa-envelope text-muted"></i></span>
                  <input type="email" class="form-control bg-light border-start-0" 
                         v-model="formCapNhat.email" 
                         @blur="validateEmail"
                         :class="{'is-invalid': errors.email}"
                         placeholder="example@gmail.com">
                  <div class="invalid-feedback">{{ errors.email }}</div>
                </div>
              </div>

              <div class="row">
                <div class="col-md-6 mb-3">
                  <label class="form-label small font-weight-bold text-uppercase text-muted">Giới tính</label>
                  <select class="form-select bg-light" v-model="formCapNhat.gioiTinh">
                    <option value="Nam">Nam</option>
                    <option value="Nữ">Nữ</option>
                    <option value="Khác">Khác</option>
                  </select>
                </div>
                <div class="col-md-6 mb-3">
                  <label class="form-label small font-weight-bold text-uppercase text-muted">Ngày sinh</label>
                  <input type="date" class="form-control bg-light" 
                         v-model="formCapNhat.ngaySinh"
                         :max="maxDate">
                </div>
              </div>

              <div class="alert alert-secondary border-0 small mt-2 py-2">
                <i class="fa fa-info-circle me-1"></i> Số điện thoại đăng ký không thể thay đổi.
              </div>

              <div class="d-grid gap-2 mt-4">
                <button type="submit" class="btn btn-primary py-2 fw-bold shadow-sm" :disabled="loading">
                  <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
                  LƯU THÔNG TIN
                </button>
                <button type="button" class="btn btn-link text-decoration-none text-muted" data-bs-dismiss="modal">Hủy bỏ</button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>

    <input type="file" ref="fileInput" class="d-none" accept="image/*" @change="xuLyUploadAvatar">
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import axiosClient from '../../api/axiosClient'; 
import Swal from 'sweetalert2';
import * as bootstrap from 'bootstrap'; 
import AccountSidebar from '../../components/AccountSidebar.vue';

const router = useRouter();
const loading = ref(false);
const fileInput = ref(null);
const errors = ref({ hoTen: '', email: '' });

// Profile data
const nguoiDung = ref({ 
  maNguoiDung: null, anhDaiDien: '', hoTen: '', soDienThoai: '', email: '', gioiTinh: '', ngaySinh: null, tenVaiTro: '' 
});

// Form data cho update
const formCapNhat = ref({ hoTen: '', email: '', gioiTinh: '', ngaySinh: '' });

// Toast notification
const Toast = Swal.mixin({
  toast: true, position: 'top-end', showConfirmButton: false, timer: 2000, timerProgressBar: true
});

// Giới hạn ngày sinh (không chọn tương lai)
const maxDate = computed(() => new Date().toISOString().split('T')[0]);

// --- Tải dữ liệu ---
const taiThongTinHoSo = async () => {
  try {
    const data = await axiosClient.get('/HoSo/thong-tin');
    if (data) nguoiDung.value = data;
  } catch (err) {
    if (err.response?.status === 401) {
      localStorage.clear();
      router.push('/auth/dang-nhap');
    }
  }
};

// --- Xử lý Ảnh đại diện ---
const getFullUrl = (path) => {
  if (!path) return '';
  return path.startsWith('http') ? path : `https://localhost:7070${path}`;
};

const moModalTuyChonAnh = () => toggleModal('modalTuyChonAnh', 'show');

const xemAnhFull = () => {
  toggleModal('modalTuyChonAnh', 'hide');
  setTimeout(() => toggleModal('modalXemAnh', 'show'), 300);
};

const triggerUpload = () => {
  toggleModal('modalTuyChonAnh', 'hide');
  fileInput.value.click();
};

const xuLyUploadAvatar = async (event) => {
  const file = event.target.files[0];
  if (!file) return;

  // Validate file size (ví dụ < 2MB)
  if (file.size > 2 * 1024 * 1024) {
    Toast.fire({ icon: 'error', title: 'Ảnh không được vượt quá 2MB' });
    return;
  }

  const formData = new FormData();
  formData.append('File', file);

  try {
    loading.value = true;
    await axiosClient.put('/HoSo/cap-nhat-avatar', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });
    Toast.fire({ icon: 'success', title: 'Cập nhật ảnh đại diện thành công' });
    await taiThongTinHoSo();
  } catch (err) {
    Toast.fire({ icon: 'error', title: 'Lỗi khi tải ảnh lên' });
  } finally {
    loading.value = false;
    event.target.value = ''; // Reset input
  }
};

const xuLyXoaAvatar = async () => {
  toggleModal('modalTuyChonAnh', 'hide');
  const res = await Swal.fire({
    title: 'Xác nhận xóa?',
    text: "Ảnh đại diện của bạn sẽ quay về mặc định",
    icon: 'warning',
    showCancelButton: true,
    confirmButtonColor: '#d33',
    confirmButtonText: 'Có, xóa nó!',
    cancelButtonText: 'Hủy'
  });

  if (res.isConfirmed) {
    try {
      await axiosClient.delete(`/HoSo/xoa-avatar/${nguoiDung.value.maNguoiDung}`);
      Toast.fire({ icon: 'success', title: 'Đã xóa ảnh đại diện' });
      await taiThongTinHoSo();
    } catch (err) {
      Toast.fire({ icon: 'error', title: 'Không thể xóa ảnh' });
    }
  }
};

// --- Xử lý Cập nhật thông tin ---
const moModalCapNhat = () => {
  formCapNhat.value = { 
    ...nguoiDung.value, 
    ngaySinh: nguoiDung.value.ngaySinh?.split('T')[0] || '' 
  };
  errors.value = { hoTen: '', email: '' }; // Reset lỗi
  toggleModal('modalChinhSua', 'show');
};

const validateTen = () => {
  const val = formCapNhat.value.hoTen?.trim();
  if (!val) errors.value.hoTen = "Họ và tên không được để trống";
  else if (val.length < 5) errors.value.hoTen = "Vui lòng nhập đầy đủ họ và tên (ít nhất 5 ký tự)";
  else errors.value.hoTen = "";
};

const validateEmail = () => {
  const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  const val = formCapNhat.value.email?.trim();
  if (!val) errors.value.email = "Email không được để trống";
  else if (!regex.test(val)) errors.value.email = "Định dạng email không hợp lệ";
  else errors.value.email = "";
};

const xuLyCapNhat = async () => {
  validateTen(); 
  validateEmail();

  if (errors.value.hoTen || errors.value.email) return;

  try {
    loading.value = true;
    await axiosClient.put('/HoSo/cap-nhat', { 
      ...formCapNhat.value, 
      maNguoiDung: nguoiDung.value.maNguoiDung 
    });
    
    Swal.fire({ 
      icon: 'success', title: 'Thành công', 
      text: 'Thông tin hồ sơ đã được cập nhật', 
      timer: 1500, showConfirmButton: false 
    });
    
    await taiThongTinHoSo();
    toggleModal('modalChinhSua', 'hide');
  } catch (err) {
    Swal.fire({ 
      icon: 'error', title: 'Thất bại', 
      text: err.response?.data?.message || 'Có lỗi xảy ra, vui lòng thử lại' 
    });
  } finally {
    loading.value = false;
  }
};

// --- Tiện ích ---
const toggleModal = (modalId, action) => {
  const el = document.getElementById(modalId);
  if (el) {
    const instance = bootstrap.Modal.getOrCreateInstance(el);
    action === 'show' ? instance.show() : instance.hide();
  }
};

const dinhDangNgay = (val) => {
  if (!val) return 'Chưa cập nhật';
  return new Date(val).toLocaleDateString('vi-VN');
};

const dangXuat = () => {
  Swal.fire({
    title: 'Bạn muốn đăng xuất?',
    icon: 'question',
    showCancelButton: true,
    confirmButtonText: 'Đồng ý',
    cancelButtonText: 'Ở lại'
  }).then((result) => {
    if (result.isConfirmed) {
      localStorage.clear();
      router.push('/auth/dang-nhap');
    }
  });
};

onMounted(taiThongTinHoSo);
</script>

<style scoped>
/* Layout */
.account-wrapper { min-height: 80vh; }
.account-content-card { border-radius: 15px; }

/* Avatar */
.avatar-circle {
  width: 140px; height: 140px; margin: 0 auto;
  border-radius: 50%; overflow: hidden;
  border: 5px solid #fff; cursor: pointer;
  background: #f8f9fa; display: flex; align-items: center; justify-content: center;
  transition: all 0.3s ease;
}
.avatar-circle:hover { filter: brightness(0.9); transform: scale(1.02); }
.avatar-circle img { width: 100%; height: 100%; object-fit: cover; }
.avatar-placeholder i { font-size: 60px; color: #dee2e6; }

.btn-change-avatar {
  position: absolute; bottom: 8px; right: 8px;
  width: 38px; height: 38px; border-radius: 50%;
  background: #007bff; color: #fff; border: 3px solid #fff;
  display: flex; align-items: center; justify-content: center;
  transition: all 0.2s;
}
.btn-change-avatar:hover { background: #0056b3; transform: scale(1.1); }

/* Info list */
.info-grid .row:last-child { border-bottom: none !important; }
.bg-soft-primary { background-color: rgba(0, 123, 255, 0.1); }

/* Form styles */
.input-group-text { border: none; }
.form-control, .form-select { border: none; padding: 10px 15px; }
.form-control:focus, .form-select:focus { box-shadow: none; background: #fff !important; border: 1px solid #007bff; }
.font-weight-bold { font-weight: 600 !important; }
</style>
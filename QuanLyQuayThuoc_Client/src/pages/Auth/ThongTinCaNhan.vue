<template>
  <div class="site-wrap">
    <div class="account-breadcrumb">
      <div class="container">
        <nav aria-label="breadcrumb">
          <ol class="breadcrumb mb-0">
            <li class="breadcrumb-item"><router-link to="/">Trang chủ</router-link></li>
            <li class="breadcrumb-item"><router-link to="/ho-so">Cá nhân</router-link></li>
            <li class="breadcrumb-item active" aria-current="page">Thông tin cá nhân</li>
          </ol>
        </nav>
      </div>
    </div>

    <div class="account-wrapper">
      <div class="container">
        <div class="row">
          <div class="col-lg-3 mb-4 mb-lg-0">
            <div class="account-sidebar">
              <div class="account-user-card">
                <div class="account-avatar" @click="moModalTuyChonAnh" style="cursor: pointer">
                  <img v-if="nguoiDung.anhDaiDien" 
                  :src="getFullUrl(nguoiDung.anhDaiDien)" 
                  class="img-fluid rounded-circle" 
                  alt="Avatar">
                  <span v-else class="icon-user"></span>
                </div>
                <div class="account-user-info">
                  <div class="account-user-name">{{ nguoiDung.hoTen || 'Đang tải...' }}</div>
                  <div class="account-user-phone">{{ nguoiDung.soDienThoai }}</div>
                </div>
              </div>
              <ul class="account-menu list-unstyled">
                <li class="active">
                  <router-link to="/ho-so"><span class="icon-user mr-2"></span> Thông tin cá nhân</router-link>
                </li>
                <li>
                  <router-link to="/don-hang"><span class="icon-list mr-2"></span> Đơn hàng của tôi</router-link>
                </li>
                <li>
                  <router-link to="/dia-chi"><span class="icon-map-marker mr-2"></span> Quản lý sổ địa chỉ</router-link>
                </li>
                <li>
                  <a href="#" @click.prevent="dangXuat"><span class="icon-exit_to_app mr-2"></span> Đăng xuất</a>
                </li>
              </ul>
            </div>
          </div>

          <div class="col-lg-9">
            <div class="account-content-card">
              <div class="account-content-header">
                <h2 class="mb-0">Thông tin cá nhân</h2>
              </div>
              <div class="account-profile-body">
                <div class="account-profile-avatar text-center mb-4">
                  <div class="avatar-circle" @click="moModalTuyChonAnh" style="cursor: pointer">
                    <img v-if="nguoiDung.anhDaiDien" 
                    :src="getFullUrl(nguoiDung.anhDaiDien)" 
                    class="img-fluid rounded-circle">
                    <span v-else class="icon-user"></span>
                  </div>
                </div>
                <div class="table-responsive">
                  <table class="table account-profile-table">
                    <tbody>
                      <tr>
                        <td class="label-col">Họ và tên</td>
                        <td class="value-col text-right">{{ nguoiDung.hoTen }}</td>
                      </tr>
                      <tr>
                        <td class="label-col">Số điện thoại</td>
                        <td class="value-col text-right">{{ nguoiDung.soDienThoai }}</td>
                      </tr>
                      <tr>
                        <td class="label-col">Email</td>
                        <td class="value-col text-right">{{ nguoiDung.email }}</td>
                      </tr>
                      <tr>
                        <td class="label-col">Giới tính</td>
                        <td class="value-col text-right">{{ nguoiDung.gioiTinh || 'Chưa cập nhật' }}</td>
                      </tr>
                      <tr>
                        <td class="label-col">Ngày sinh</td>
                        <td class="value-col text-right text-primary">{{ dinhDangNgay(nguoiDung.ngaySinh) }}</td>
                      </tr>
                      <tr>
                        <td class="label-col">Vai trò</td>
                        <td class="value-col text-right"><span class="badge badge-info">{{ nguoiDung.tenVaiTro }}</span></td>
                      </tr>
                    </tbody>
                  </table>
                </div>
                <div class="text-center mt-4">
                  <button type="button" class="btn btn-primary px-5 py-2 rounded-pill shadow" 
                          data-bs-toggle="modal" data-bs-target="#modalChinhSua" @click="moModalCapNhat">
                    Chỉnh sửa thông tin
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
        <div class="modal-content border-0 shadow-lg">
          <div class="modal-body p-0">
            <div class="list-group list-group-flush text-center">
              <button v-if="nguoiDung.anhDaiDien" class="list-group-item list-group-item-action py-3" @click="xemAnhFull">
                Xem hình ảnh
              </button>
              <button class="list-group-item list-group-item-action py-3 text-primary font-weight-bold" @click="triggerUpload">
                Chọn ảnh mới
              </button>
              <button v-if="nguoiDung.anhDaiDien" class="list-group-item list-group-item-action py-3 text-danger" @click="xuLyXoaAvatar">
                Xóa ảnh hiện tại
              </button>
              <button class="list-group-item list-group-item-action py-3 text-secondary" data-bs-dismiss="modal">
                Hủy
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="modal fade" id="modalXemAnh" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered modal-lg">
        <div class="modal-content bg-transparent border-0">
          <div class="modal-body p-0 text-center position-relative">
            <button type="button" class="close text-white position-absolute" style="top: -30px; right: 0; font-size: 2rem;" data-dismiss="modal">&times;</button>
            <img :src="getFullUrl(nguoiDung.anhDaiDien)" class="img-fluid rounded shadow-lg" style="max-height: 85vh;">
          </div>
        </div>
      </div>
    </div>

    <div class="modal fade" id="modalChinhSua" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Cập nhật thông tin cá nhân</h5>
            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="xuLyCapNhat">
              <div class="form-group">
                <label>Họ và tên</label>
                <input type="text" class="form-control" v-model="formCapNhat.hoTen" @input="validateTen" :class="{'is-invalid': errors.hoTen}">
                <small class="text-danger" v-if="errors.hoTen">{{ errors.hoTen }}</small>
              </div>
              <div class="form-group">
                <label>Email</label>
                <input type="email" class="form-control" v-model="formCapNhat.email" @input="validateEmail" :class="{'is-invalid': errors.email}">
                <small class="text-danger" v-if="errors.email">{{ errors.email }}</small>
              </div>
              <div class="form-group">
                <label>Số điện thoại</label>
                <input type="text" class="form-control" v-model="formCapNhat.soDienThoai" disabled>
              </div>
              <div class="form-group">
                <label class="font-weight-bold">Giới tính</label>
                <select class="form-control" v-model="formCapNhat.gioiTinh">
                  <option value="Nam">Nam</option>
                  <option value="Nữ">Nữ</option>
                  <option value="Khác">Khác</option>
                </select>
              </div>
              <div class="form-group">
                <label class="font-weight-bold">Ngày sinh</label>
                <input type="date" class="form-control" v-model="formCapNhat.ngaySinh">
              </div>
              <div class="text-right mt-4">
                <button type="button" class="close" data-bs-dismiss="modal" aria-label="Close">
                  
                </button>

                <button type="button" class="btn btn-secondary mr-2" data-bs-dismiss="modal">Hủy</button>
                <button type="submit" class="btn btn-primary" :disabled="loading">
                  {{ loading ? 'Đang lưu...' : 'Lưu thay đổi' }}
                </button>
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
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import axiosClient from '../../api/axiosClient'; 
import Swal from 'sweetalert2';
// Import bootstrap để truy cập trực tiếp các Class Modal
import * as bootstrap from 'bootstrap'; 

// --- KHAI BÁO BIẾN ---
const loading = ref(false);
const router = useRouter();
const fileInput = ref(null);

const Toast = Swal.mixin({
  toast: true,
  position: 'top-end',
  showConfirmButton: false,
  timer: 3000,
  timerProgressBar: true
});

const nguoiDung = ref({ 
  maNguoiDung: null,
  anhDaiDien: '', 
  hoTen: '', 
  soDienThoai: '', 
  email: '', 
  gioiTinh: '', 
  ngaySinh: null, 
  tenVaiTro: '' 
});

const formCapNhat = ref({ 
  hoTen: '', 
  email: '', 
  soDienThoai: '', 
  gioiTinh: '', 
  ngaySinh: '' 
});

const errors = ref({ hoTen: '', email: '' });

// --- UTILITIES ---
const getFullUrl = (path) => {
  if (!path) return '';
  return path.startsWith('http') ? path : `https://localhost:7070${path}`;
};

const dinhDangNgay = (chuoi) => chuoi ? new Date(chuoi).toLocaleDateString('vi-VN') : 'Chưa cập nhật';

// Hàm điều khiển Modal bằng Bootstrap Native để fix lỗi console
// Cập nhật hàm toggleModal để chắc chắn hơn
const toggleModal = (modalId, action) => {
  const modalElement = document.getElementById(modalId);
  if (modalElement) {
    const modalInstance = bootstrap.Modal.getOrCreateInstance(modalElement);
    if (action === 'show') {
      modalInstance.show();
    } else {
      modalInstance.hide();
      // Đôi khi backdrop bị kẹt, dòng này sẽ dọn dẹp nó
      const backdrop = document.querySelector('.modal-backdrop');
      if (backdrop) backdrop.remove();
      document.body.classList.remove('modal-open');
      document.body.style.paddingRight = '';
    }
  }
};
// --- XỬ LÝ ẢNH (AVATAR) ---
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

  const formData = new FormData();
  formData.append('File', file);

  try {
    loading.value = true;
    // Gửi yêu cầu cập nhật ảnh đến server
    await axiosClient.put('/HoSo/cap-nhat-avatar', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });
    
    // Hiển thị thông báo khi thêm mới/cập nhật thành công
    Toast.fire({ 
      icon: 'success', 
      title: 'Cập nhật ảnh đại diện thành công' 
    });

    // Tải lại thông tin để hiển thị ảnh mới trên giao diện
    await taiThongTinHoSo();
  } catch (loi) {
    console.error("Lỗi upload:", loi);
    Toast.fire({ 
      icon: 'error', 
      title: 'Không thể tải ảnh lên' 
    });
  } finally {
    loading.value = false;
    event.target.value = ''; // Reset input file để có thể chọn lại cùng 1 file nếu muốn
  }
};

const xuLyXoaAvatar = async () => {
  // Đóng modal lựa chọn trước khi hiện hộp thoại xác nhận
  toggleModal('modalTuyChonAnh', 'hide');

  const result = await Swal.fire({
    title: 'Xác nhận xóa?',
    text: "Ảnh đại diện của bạn sẽ trở về mặc định.",
    icon: 'warning',
    showCancelButton: true,
    confirmButtonColor: '#d33',
    confirmButtonText: 'Xóa ngay',
    cancelButtonText: 'Hủy'
  });

  if (result.isConfirmed) {
    try {
      loading.value = true;

      // Gọi API xóa ảnh dựa trên mã người dùng
      await axiosClient.delete(`/HoSo/xoa-avatar/${nguoiDung.value.maNguoiDung}`); 

      // Hiển thị thông báo sau khi xác nhận xóa thành công
      Toast.fire({ 
        icon: 'success', 
        title: 'Đã xóa ảnh đại diện thành công' 
      });

      // Tải lại dữ liệu để cập nhật lại avatar mặc định
      await taiThongTinHoSo(); 
    } catch (loi) {
      console.error("Lỗi xóa avatar:", loi);
      const message = loi.response?.data?.message || 'Lỗi khi thực hiện xóa ảnh';
      Toast.fire({ 
        icon: 'error', 
        title: message 
      });
    } finally {
      loading.value = false;
    }
  }
};

// --- XỬ LÝ DỮ LIỆU ---
const taiThongTinHoSo = async () => {
  try {
    const data = await axiosClient.get('/HoSo/thong-tin');
    if (data) {
      nguoiDung.value = data;
    }
  } catch (loi) {
    console.error("Lỗi lấy thông tin:", loi);
    if (loi.response?.status === 401) {
      router.push('/auth/dang-nhap');
    }
  }
};

const moModalCapNhat = () => {
  formCapNhat.value = { 
    ...nguoiDung.value, 
    ngaySinh: nguoiDung.value.ngaySinh ? nguoiDung.value.ngaySinh.split('T')[0] : '' 
  };
  errors.value = { hoTen: '', email: '' };
  toggleModal('modalChinhSua', 'show');
};

const validateTen = () => {
  const regexTen = /^[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂưăạảấầẩẫậắằẳẵặẹẻẽềềểỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪỬỮỰỵỷỹýỳỷỹý\s]+$/;
  errors.value.hoTen = !formCapNhat.value.hoTen?.trim() ? "Họ tên không được trống" : 
                       !regexTen.test(formCapNhat.value.hoTen.trim()) ? "Tên không chứa số/ký tự đặc biệt" : "";
};

const validateEmail = () => {
  const regexEmail = /^[^\s@]+@[^\s@]+\.(com|vn|net|edu|org)$/i;
  errors.value.email = !formCapNhat.value.email?.trim() ? "Email không được trống" : 
                        !regexEmail.test(formCapNhat.value.email.trim()) ? "Email sai định dạng" : "";
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
    
    Swal.fire({ icon: 'success', title: 'Thành công!', timer: 1500, showConfirmButton: false });
    await taiThongTinHoSo();
    toggleModal('modalChinhSua', 'hide');
  } catch (loi) {
    Swal.fire({ icon: 'error', title: 'Thất bại', text: loi.response?.data?.message || "Có lỗi xảy ra" });
  } finally {
    loading.value = false;
  }
};

const dangXuat = () => {
  Swal.fire({
    title: 'Đăng xuất?',
    icon: 'warning',
    showCancelButton: true,
    confirmButtonText: 'Đồng ý',
    cancelButtonText: 'Hủy'
  }).then((result) => {
    if (result.isConfirmed) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      router.push('/auth/dang-nhap');
    }
  });
};

onMounted(taiThongTinHoSo);
</script>

<style scoped>
/* CSS cho avatar */
.avatar-circle, .account-avatar {
  overflow: hidden;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #f8f9fa;
  border: 2px solid #eee;
  transition: all 0.3s ease;
}

.avatar-circle:hover, .account-avatar:hover {
  border-color: #007bff;
  transform: scale(1.02);
}

.avatar-circle img, .account-avatar img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

/* CSS cho Modal 3 nút */
#modalTuyChonAnh .modal-content {
  border-radius: 12px;
}

#modalTuyChonAnh .list-group-item {
  border-left: none;
  border-right: none;
  cursor: pointer;
}

#modalTuyChonAnh .list-group-item:first-child { border-top: none; }
#modalTuyChonAnh .list-group-item:last-child { border-bottom: none; }
</style>
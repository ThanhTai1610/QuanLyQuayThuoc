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
                <div class="account-avatar">
                  <img v-if="nguoiDung.anhDaiDien" :src="nguoiDung.anhDaiDien" class="img-fluid rounded-circle" alt="Avatar">
                  <span v-else class="icon-user"></span>
                </div>
                <div class="account-user-info">
                  <div class="account-user-name">{{ nguoiDung.hoTen || 'Đang tải...' }}</div>
                  <div class="account-user-phone">{{ nguoiDung.soDienThoai }}</div>
                </div>
              </div>
              <ul class="account-menu list-unstyled">
                <li class="active">
                  <router-link to="/ho-so">
                    <span class="icon-user mr-2"></span> Thông tin cá nhân
                  </router-link>
                </li>
                <li>
                  <router-link to="/don-hang">
                    <span class="icon-list mr-2"></span> Đơn hàng của tôi
                  </router-link>
                </li>
                <li>
                  <router-link to="/dia-chi">
                    <span class="icon-map-marker mr-2"></span> Quản lý sổ địa chỉ
                  </router-link>
                </li>
                <li>
                  <a href="#" @click.prevent="">
                    <span class="icon-event_available mr-2"></span> Lịch hẹn tiêm chủng
                  </a>
                </li>
                <li>
                  <a href="#" @click.prevent="">
                    <span class="icon-healing mr-2"></span> Đơn thuốc của tôi
                  </a>
                </li>
                <li>
                  <a href="#" @click.prevent="dangXuat">
                    <span class="icon-exit_to_app mr-2"></span> Đăng xuất
                  </a>
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
                  <div class="avatar-circle">
                    <img v-if="nguoiDung.anhDaiDien" :src="nguoiDung.anhDaiDien" class="img-fluid rounded-circle">
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
                        <td class="value-col text-right text-primary" style="cursor: pointer">
                          {{ dinhDangNgay(nguoiDung.ngaySinh) }}
                        </td>
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
                          data-toggle="modal" data-target="#modalChinhSua" @click="moModalCapNhat">
                    Chỉnh sửa thông tin
                  </button>
                </div>
              </div>
            </div>
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
             <input type="text" 
                  class="form-control" 
                  v-model="formCapNhat.hoTen" 
                  @input="validateTen"
                  :class="{'is-invalid': errors.hoTen}">
              <small class="text-danger" v-if="errors.hoTen">{{ errors.hoTen }}</small>
            </div>

            <div class="form-group">
              <label>Email</label>
              <input type="email" 
                    class="form-control" 
                    v-model="formCapNhat.email" 
                    @input="validateEmail"
                    :class="{'is-invalid': errors.email}">
              <small class="text-danger" v-if="errors.email">{{ errors.email }}</small>
            </div>
            <div class="form-group">
              <label>Số điện thoại</label>
              <input type="text" class="form-control" v-model="formCapNhat.soDienThoai" disabled>
              <small class="text-muted">Không thể thay đổi số điện thoại</small>
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
              <button type="button" class="btn btn-secondary mr-2" data-dismiss="modal">Hủy</button>
              <button type="submit" class="btn btn-primary" :disabled="loading">
                {{ loading ? 'Đang lưu...' : 'Lưu thay đổi' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
  </div>
  
  
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import axiosClient from '../../api/axiosClient'; 
import Swal from 'sweetalert2'; // Import thư viện vào đây

const loading = ref(false);
const router = useRouter();

// Cấu hình Toast để hiển thị thông báo nhanh ở góc màn hình
const Toast = Swal.mixin({
  toast: true,
  position: 'top-end',
  showConfirmButton: false,
  timer: 3000,
  timerProgressBar: true
});

const formCapNhat = ref({
  hoTen: '',
  email: '',
  soDienThoai: '',
  gioiTinh: '',
  ngaySinh: ''
});

const nguoiDung = ref({
  hoTen: '',
  soDienThoai: '',
  email: '',
  anhDaiDien: '',
  gioiTinh: '',
  ngaySinh: null,
  tenVaiTro: ''
});

const errors = ref({
  hoTen: '',
  email: ''
});

const moModalCapNhat = () => {
  formCapNhat.value = {
    hoTen: nguoiDung.value.hoTen,
    email: nguoiDung.value.email,
    soDienThoai: nguoiDung.value.soDienThoai,
    gioiTinh: nguoiDung.value.gioiTinh || 'Nam',
    ngaySinh: nguoiDung.value.ngaySinh ? nguoiDung.value.ngaySinh.split('T')[0] : ''
  };
  // Reset lỗi khi mở lại modal
  errors.value = { hoTen: '', email: '' };
};

const taiThongTinHoSo = async () => {
  try {
    const token = localStorage.getItem('token');
    if (!token) {
      router.push('/auth/dang-nhap');
      return;
    }
    const data = await axiosClient.get('/HoSo/thong-tin');
    if (data) {
      nguoiDung.value = data;
    }
  } catch (loi) {
    console.error("Lỗi khi lấy thông tin người dùng:", loi);
  }
};

const dinhDangNgay = (chuoiNgay) => {
  if (!chuoiNgay) return 'Chưa cập nhật';
  const ngay = new Date(chuoiNgay);
  return ngay.toLocaleDateString('vi-VN');
};

// Cập nhật hàm Đăng xuất có xác nhận
const dangXuat = () => {
  Swal.fire({
    title: 'Bạn muốn đăng xuất?',
    text: "Phiên làm việc của bạn sẽ kết thúc!",
    icon: 'warning',
    showCancelButton: true,
    confirmButtonColor: '#3085d6',
    cancelButtonColor: '#d33',
    confirmButtonText: 'Đồng ý',
    cancelButtonText: 'Hủy'
  }).then((result) => {
    if (result.isConfirmed) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      router.push('/auth/dang-nhap');
      Toast.fire({
        icon: 'success',
        title: 'Đã đăng xuất thành công'
      });
    }
  });
};

const validateTen = () => {
  const regexTen = /^[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂưăạảấầẩẫậắằẳẵặẹẻẽềềểỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪỬỮỰỵỷỹýỳỷỹý\s]+$/;
  if (!formCapNhat.value.hoTen?.trim()) {
    errors.value.hoTen = "Họ tên không được để trống";
  } else if (!regexTen.test(formCapNhat.value.hoTen.trim())) {
    errors.value.hoTen = "Tên không được chứa số hoặc ký tự đặc biệt";
  } else {
    errors.value.hoTen = "";
  }
};

const validateEmail = () => {
  const regexEmail = /^[^\s@]+@[^\s@]+\.(com|vn|net|edu|org)$/i;
  if (!formCapNhat.value.email?.trim()) {
    errors.value.email = "Email không được để trống";
  } else if (!regexEmail.test(formCapNhat.value.email.trim())) {
    errors.value.email = "Email phải đúng định dạng (vd: .com, .vn)";
  } else {
    errors.value.email = "";
  }
};

const xuLyCapNhat = async () => {
  validateTen();
  validateEmail();

  if (errors.value.hoTen || errors.value.email) {
    Toast.fire({
      icon: 'error',
      title: 'Vui lòng kiểm tra lại thông tin!'
    });
    return;
  }

  try {
    loading.value = true;
    const dataGuiDi = {
      maNguoiDung: nguoiDung.value.maNguoiDung,
      hoTen: formCapNhat.value.hoTen.trim(),
      email: formCapNhat.value.email.trim(),
      soDienThoai: formCapNhat.value.soDienThoai,
      gioiTinh: formCapNhat.value.gioiTinh, 
      ngaySinh: formCapNhat.value.ngaySinh  
    };

    const response = await axiosClient.put('/HoSo/cap-nhat', dataGuiDi);

    if (response) {
      // Thông báo thành công
      Swal.fire({
        icon: 'success',
        title: 'Thành công!',
        text: 'Thông tin của bạn đã được cập nhật.',
        showConfirmButton: false,
        timer: 1500
      });

      await taiThongTinHoSo();
      
      // Đóng modal
      const closeButton = document.querySelector('#modalChinhSua [data-dismiss="modal"]');
      if (closeButton) closeButton.click();
    }
  } catch (loi) {
    console.error("Lỗi cập nhật:", loi);
    const msg = loi.response?.data?.message || "Có lỗi xảy ra khi lưu thông tin!";
    Swal.fire({
      icon: 'error',
      title: 'Thất bại',
      text: msg
    });
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  taiThongTinHoSo();
});
</script>
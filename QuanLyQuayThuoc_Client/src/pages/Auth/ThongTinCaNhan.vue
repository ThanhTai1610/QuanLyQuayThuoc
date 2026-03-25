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
              <input type="text" class="form-control" v-model="formCapNhat.hoTen" required>
            </div>
            <div class="form-group">
              <label>Email</label>
              <input type="email" class="form-control" v-model="formCapNhat.email" required>
            </div>
            <div class="form-group">
              <label>Số điện thoại</label>
              <input type="text" class="form-control" v-model="formCapNhat.soDienThoai" disabled>
              <small class="text-muted">Không thể thay đổi số điện thoại</small>
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
// QUAN TRỌNG: Dùng axiosClient Tài đã viết để tự động đính kèm Token từ localStorage
import axiosClient from '../../api/axiosClient'; 
const loading = ref(false);
const router = useRouter();

const formCapNhat = ref({
  hoTen: '',
  email: '',
  soDienThoai: ''
});

const moModalCapNhat = () => {
  formCapNhat.value = {
    hoTen: nguoiDung.value.hoTen,
    email: nguoiDung.value.email,
    soDienThoai: nguoiDung.value.soDienThoai
  };
};
// Khởi tạo object để hứng dữ liệu từ API
const nguoiDung = ref({
  hoTen: '',
  soDienThoai: '',
  email: '',
  anhDaiDien: '',
  gioiTinh: '',
  ngaySinh: null,
  tenVaiTro: ''
});

// Hàm gọi API lấy dữ liệu hồ sơ
const taiThongTinHoSo = async () => {
  try {
    // 1. Kiểm tra token trước khi gọi
    const token = localStorage.getItem('token');
    if (!token) {
      router.push('/auth/dang-nhap');
      return;
    }

    // 2. Gọi API thông qua axiosClient (đã đổi port thành 7070 cho khớp Backend của Tài)
    // Lưu ý: Route phải khớp với [HttpGet("thong-tin")] trong HoSoController
    const data = await axiosClient.get('/HoSo/thong-tin');

    // 3. Gán dữ liệu vào ref
    if (data) {
      nguoiDung.value = data;
      console.log("Dữ liệu hồ sơ:", data);
    }
  } catch (loi) {
    console.error("Lỗi khi lấy thông tin người dùng:", loi);
    // Nếu lỗi 401 (Hết hạn token), axiosClient sẽ tự đá về trang login
  }
};

// Hàm định dạng ngày tháng hiển thị
const dinhDangNgay = (chuoiNgay) => {
  if (!chuoiNgay) return 'Chưa cập nhật';
  const ngay = new Date(chuoiNgay);
  return ngay.toLocaleDateString('vi-VN');
};

const dangXuat = () => {
  localStorage.removeItem('token');
  localStorage.removeItem('user');
  router.push('/auth/dang-nhap');
};

const xuLyCapNhat = async () => {
  try {
    loading.value = true;
    
    const dataGuiDi = {
      maNguoiDung: nguoiDung.value.maNguoiDung,
      hoTen: formCapNhat.value.hoTen,
      email: formCapNhat.value.email,
      soDienThoai: nguoiDung.value.soDienThoai,
      gioiTinh: formCapNhat.value.gioiTinh || nguoiDung.value.gioiTinh
    };

    const response = await axiosClient.put('/HoSo/cap-nhat', dataGuiDi);

    if (response) {
      alert("Cập nhật thông tin thành công!");
      await taiThongTinHoSo();
      
      // Sửa lỗi "modal is not a function" ở đây
      // Cách 1: Dùng jQuery nếu đã có thư viện
      if (typeof window.$ !== 'undefined' && typeof window.$.fn.modal !== 'undefined') {
        window.$('#modalChinhSua').modal('hide');
      } else {
        // Cách 2: Nếu không có jQuery, ta ép modal đóng bằng cách tìm nút đóng hoặc reload
        const closeButton = document.querySelector('#modalChinhSua [data-dismiss="modal"]');
        if (closeButton) closeButton.click();
      }
    }
  } catch (loi) {
    // Nếu API đã thành công (status 200) nhưng vẫn nhảy vào catch do lỗi đóng modal
    // thì ta không nên hiện alert lỗi
    if (loi.message && loi.message.includes('modal is not a function')) {
        console.warn("Lưu thành công nhưng không đóng được modal tự động.");
    } else {
        console.error("Lỗi cập nhật:", loi);
        alert("Có lỗi xảy ra khi lưu thông tin!");
    }
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  taiThongTinHoSo();
});
</script>
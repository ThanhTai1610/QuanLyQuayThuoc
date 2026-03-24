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
                  <button type="button" class="btn btn-primary px-5 py-2 rounded-pill">
                    Chỉnh sửa thông tin
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';

const router = useRouter();

// Khởi tạo đối tượng người dùng theo chuẩn Backend đã code
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
    const token = localStorage.getItem('token');
    if (!token) {
      router.push('/auth/dang-nhap');
      return;
    }

    const phanHoi = await axios.get('https://localhost:7xxx/api/HoSo/thong-tin-ca-nhan', {
      headers: {
        'Authorization': `Bearer ${token}`
      }
    });

    if (phanHoi.data) {
      nguoiDung.value = phanHoi.data;
    }
  } catch (loi) {
    console.error("Lỗi khi lấy thông tin người dùng:", loi);
    if (loi.response && loi.response.status === 401) {
      dangXuat();
    }
  }
};

// Hàm định dạng ngày tháng hiển thị
const dinhDangNgay = (chuoiNgay) => {
  if (!chuoiNgay) return 'Thêm thông tin';
  const ngay = new Date(chuoiNgay);
  return ngay.toLocaleDateString('vi-VN');
};

const dangXuat = () => {
  localStorage.removeItem('token');
  router.push('/auth/dang-nhap');
};

// Gọi API ngay khi component được gắn vào DOM
onMounted(() => {
  taiThongTinHoSo();
});
</script>
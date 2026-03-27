<script setup>
import { reactive, ref } from 'vue';
import axiosClient from '../../api/axiosClient';

const emit = defineEmits(['refresh']);
const dangXuLy = ref(false);

// Khởi tạo form trống
const form = reactive({
  hoTen: '',
  email: '',
  matKhau: '',
  soDienThoai: '',
  diaChi: '',
  gioiTinh: 'Nam'
});
const resetForm = () => {
  form.hoTen = '';
  form.email = '';
  form.matKhau = '';
  form.soDienThoai = '';
  form.diaChi = '';
  form.gioiTinh = 'Nam';
};
const taoMatKhauNgauNhien = () => {
  form.matKhau = Math.random().toString(36).slice(-8); // Tạo pass 8 ký tự
};

const handleThemNhanVien = async () => {
  dangXuLy.value = true;
  try {
    // Chỉ lấy các trường cần thiết để gửi
    const dataToSend = {
      hoTen: form.hoTen,
      email: form.email,
      matKhau: form.matKhau,
      soDienThoai: form.soDienThoai,
      gioiTinh: form.gioiTinh || "Nam"
    };

    const res = await axiosClient.post('/admin/AdminNguoiDung/them-nhan-vien', dataToSend);
    
    alert(res.message);
    emit('refresh'); // Gọi lại hàm loadData ở DanhSachNguoiDung.vue
    resetForm();
  } catch (err) {
    console.error(err);
    alert(err.response?.data?.message || "Lỗi khi lưu dữ liệu");
  } finally {
    dangXuLy.value = false;
  }
};
</script>

<template>
  <div class="card shadow mb-4" id="phan-them-sua">
    <div class="card-header py-3">
      <h6 class="m-0 font-weight-bold text-primary">2. Thêm mới Nhân viên</h6>
    </div>
    <div class="card-body">
      <div class="row">
        <div class="col-md-6 form-group">
          <label>Họ tên <span class="text-danger">*</span></label>
          <input v-model="form.hoTen" type="text" class="form-control" placeholder="Nguyễn Văn A">
        </div>
        <div class="col-md-6 form-group">
          <label>Email <span class="text-danger">*</span></label>
          <input v-model="form.email" type="email" class="form-control" placeholder="nhanvien@gmail.com">
        </div>
        <div class="col-md-6 form-group">
          <label>Mật khẩu <span class="text-danger">*</span></label>
          <div class="input-group">
            <input v-model="form.matKhau" type="text" class="form-control">
            <div class="input-group-append">
              <button @click="taoMatKhauNgauNhien" class="btn btn-outline-secondary" type="button">Tạo mã</button>
            </div>
          </div>
        </div>
        <div class="col-md-6 form-group">
          <label>Số điện thoại</label>
          <input v-model="form.soDienThoai" type="text" class="form-control">
        </div>
      </div>
      <button 
        @click="handleThemNhanVien" 
        :disabled="dangXuLy" 
        class="btn btn-primary px-4"
      >
        <i v-if="dangXuLy" class="fas fa-spinner fa-spin mr-2"></i>
        LƯU NHÂN VIÊN
      </button>
    </div>
  </div>
</template>
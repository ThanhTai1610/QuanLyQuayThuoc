<script setup>
import { reactive, ref, watch, computed } from 'vue';
import axiosClient from '../../api/axiosClient';
import Swal from 'sweetalert2';

const props = defineProps(['duLieuSua']);
const emit = defineEmits(['refresh', 'huy-sua']);
const dangXuLy = ref(false);
const daBamLuu = ref(false); // Biến để kiểm soát việc hiện lỗi sau khi bấm nút

const form = reactive({
  maNguoiDung: 0,
  hoTen: '',
  email: '',
  matKhau: '',
  soDienThoai: '',
  gioiTinh: 'Nam',
  maVaiTro: 3,
  trangThai: 'Hoạt động'
});

// --- LOGIC VALIDATE (Computed để tự động cập nhật lỗi) ---
const errors = computed(() => {
  const errs = {};
  
  // Validate Họ tên
  if (!form.hoTen.trim()) errs.hoTen = "Họ tên không được để trống";
  else if (form.hoTen.length < 3) errs.hoTen = "Họ tên phải ít nhất 3 ký tự";

  // Validate Email
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!form.email.trim()) errs.email = "Email là bắt buộc";
  else if (!emailRegex.test(form.email)) errs.email = "Email không đúng định dạng";

  // Validate Mật khẩu (Nếu thêm mới thì bắt buộc, nếu sửa thì chỉ check khi có nhập)
  if (form.maNguoiDung === 0) {
    if (!form.matKhau) errs.matKhau = "Mật khẩu là bắt buộc khi thêm mới";
    else if (form.matKhau.length < 6) errs.matKhau = "Mật khẩu phải từ 6 ký tự";
  } else {
    if (form.matKhau && form.matKhau.length < 6) errs.matKhau = "Mật khẩu mới phải từ 6 ký tự";
  }

  // Validate Số điện thoại (Định dạng VN)
  const phoneRegex = /(84|0[3|5|7|8|9])+([0-9]{8})\b/g;
  if (form.soDienThoai && !form.soDienThoai.match(phoneRegex)) {
    errs.soDienThoai = "Số điện thoại không đúng định dạng Việt Nam";
  }

  return errs;
});

const formHopLe = computed(() => Object.keys(errors.value).length === 0);

const resetForm = () => {
  form.maNguoiDung = 0;
  form.hoTen = '';
  form.email = '';
  form.matKhau = '';
  form.soDienThoai = '';
  form.gioiTinh = 'Nam';
  form.maVaiTro = 3;
  form.trangThai = 'Hoạt động';
  daBamLuu.value = false;
};

const huySua = () => {
  emit('huy-sua');
  resetForm();
};

watch(() => props.duLieuSua, (newVal) => {
  if (newVal) {
    Object.assign(form, newVal);
    form.matKhau = ''; 
    daBamLuu.value = false;
  } else {
    resetForm();
  }
}, { deep: true, immediate: true });

const handleLuu = async () => {
  daBamLuu.value = true; // Bắt đầu hiện lỗi đỏ nếu có

  if (!formHopLe.value) {
    return Swal.fire('Thông báo', 'Vui lòng kiểm tra lại các thông tin còn lỗi!', 'warning');
  }

  dangXuLy.value = true;
  try {
    const res = await axiosClient.post('/admin/AdminNguoiDung/luu', form);
    Swal.fire('Thành công', res.message || 'Đã lưu dữ liệu', 'success');
    emit('refresh');
    huySua();
  } catch (err) {
    Swal.fire('Lỗi', err.response?.data?.message || 'Email đã tồn tại hoặc lỗi hệ thống', 'error');
  } finally {
    dangXuLy.value = false;
  }
};
</script>

<template>
  <div class="card shadow mb-4 border-left-primary">
    <div class="card-header py-3">
      <h6 class="m-0 font-weight-bold text-primary">
        {{ form.maNguoiDung === 0 ? 'THÊM MỚI NGƯỜI DÙNG' : 'ĐANG CHỈNH SỬA: ' + form.hoTen }}
      </h6>
    </div>
    <div class="card-body">
      <div class="row">
        <div class="col-md-6 mb-3">
          <label>Họ và tên <span class="text-danger">*</span></label>
          <input v-model="form.hoTen" type="text" class="form-control" :class="{'is-invalid': daBamLuu && errors.hoTen}">
          <div class="invalid-feedback">{{ errors.hoTen }}</div>
        </div>

        <div class="col-md-6 mb-3">
          <label>Email <span class="text-danger">*</span></label>
          <input v-model="form.email" type="email" class="form-control" 
                 :class="{'is-invalid': daBamLuu && errors.email}" 
                 :disabled="form.maNguoiDung !== 0">
          <div class="invalid-feedback">{{ errors.email }}</div>
        </div>

        <div class="col-md-6 mb-3">
          <label>Mật khẩu {{ form.maNguoiDung !== 0 ? '(Bỏ trống nếu không đổi)' : '*' }}</label>
          <input v-model="form.matKhau" type="password" class="form-control" :class="{'is-invalid': daBamLuu && errors.matKhau}">
          <div class="invalid-feedback">{{ errors.matKhau }}</div>
        </div>

        <div class="col-md-6 mb-3">
          <label>Số điện thoại</label>
          <input v-model="form.soDienThoai" type="text" class="form-control" :class="{'is-invalid': daBamLuu && errors.soDienThoai}">
          <div class="invalid-feedback">{{ errors.soDienThoai }}</div>
        </div>

        <div class="col-md-4">
          <label>Vai trò</label>
          <select v-model="form.maVaiTro" class="form-control">
            <option :value="2">Nhân viên</option>
            <option :value="3">Khách hàng</option>
          </select>
        </div>
      </div>

      <hr>
      <button @click="handleLuu" :disabled="dangXuLy" class="btn btn-primary mr-2">
        <i v-if="dangXuLy" class="fas fa-spinner fa-spin mr-1"></i>
        {{ form.maNguoiDung === 0 ? 'XÁC NHẬN THÊM' : 'LƯU CẬP NHẬT' }}
      </button>
      <button v-if="form.maNguoiDung !== 0" @click="huySua" class="btn btn-secondary">HỦY SỬA</button>
    </div>
  </div>
</template>

<style scoped>
.invalid-feedback {
  display: block; /* Hiện lỗi ngay cả khi không dùng form tag */
  font-size: 0.85rem;
}
.is-invalid {
  border-color: #e74a3b !important;
}
</style>
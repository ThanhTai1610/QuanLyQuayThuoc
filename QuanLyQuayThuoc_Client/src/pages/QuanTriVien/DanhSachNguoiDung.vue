<template>
  <div class="card shadow mb-4">
    <div class="card-header py-3 d-flex flex-wrap align-items-center justify-content-between">
      <h6 class="m-0 font-weight-bold text-primary">Danh sách người dùng</h6>
      
      <div class="d-flex flex-wrap mt-2 mt-md-0">
        <div class="input-group input-group-sm mr-2" style="width: 250px;">
          <input v-model="boLoc.tuKhoa" type="text" class="form-control" placeholder="Tìm tên, email, sđt...">
          <div class="input-group-append">
            <span class="input-group-text"><i class="fas fa-search"></i></span>
          </div>
        </div>

        <select v-model="boLoc.vaiTro" class="form-control form-control-sm mr-2" style="width: 130px;">
          <option :value="0">-- Vai trò --</option>
          <option :value="2">Nhân viên</option>
          <option :value="3">Khách hàng</option>
        </select>

        <select v-model="boLoc.trangThai" class="form-control form-control-sm" style="width: 130px;">
          <option value="">-- Trạng thái --</option>
          <option value="Hoạt động">Hoạt động</option>
          <option value="Bị khóa">Bị khóa</option>
        </select>
      </div>
    </div>

    <div class="card-body">
      <div v-if="dangTai" class="text-center py-4"><i class="fas fa-spinner fa-spin fa-2x"></i></div>
      
      <div v-else class="table-responsive">
        <table class="table table-bordered table-hover">
          <thead class="thead-light">
            <tr>
              <th>ID</th><th>Họ tên</th><th>Email</th><th>Vai trò</th><th>Trạng thái</th><th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="nd in danhSachHienThi" :key="nd.maNguoiDung">
              <td>{{ nd.maNguoiDung }}</td>
              <td class="font-weight-bold">{{ nd.hoTen }}</td>
              <td>{{ nd.email }}</td>
              <td>
                <span class="badge badge-info">
                   {{ nd.tenVaiTro || (nd.maVaiTro === 2 ? 'Nhân viên' : 'Khách hàng') }}
                </span>
              </td>
              <td>
                <span :class="nd.trangThai === 'Hoạt động' ? 'badge badge-success' : 'badge badge-danger'">
                  {{ nd.trangThai }}
                </span>
              </td>
              <td>
                <button class="btn btn-warning btn-sm mr-1" @click="$emit('chinh-sua', nd)"><i class="fas fa-edit"></i></button>
                <button class="btn btn-sm" :class="nd.trangThai === 'Hoạt động' ? 'btn-outline-danger' : 'btn-outline-success'"
                        @click="handleDoiTrangThai(nd)" :disabled="nd.dangXuLy">
                  <i :class="nd.trangThai === 'Hoạt động' ? 'fas fa-lock' : 'fas fa-unlock'"></i>
                </button>
              </td>
            </tr>
            <tr v-if="danhSachHienThi.length === 0">
              <td colspan="6" class="text-center">Không tìm thấy kết quả phù hợp.</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue';
import axiosClient from '../../api/axiosClient';
import Swal from 'sweetalert2';

const emit = defineEmits(['chinh-sua']);
const danhSach = ref([]);
const dangTai = ref(false);

// 1. Khai báo các biến bộ lọc
const boLoc = reactive({
  tuKhoa: '',
  vaiTro: 0,
  trangThai: ''
});

// 2. Logic Lọc Dữ Liệu (Computed tự nhảy khi boLoc thay đổi)
const danhSachHienThi = computed(() => {
  return danhSach.value.filter(item => {
    // Lọc theo từ khóa (Tên, Email hoặc SĐT)
    const matchTuKhoa = !boLoc.tuKhoa || 
      item.hoTen?.toLowerCase().includes(boLoc.tuKhoa.toLowerCase()) ||
      item.email?.toLowerCase().includes(boLoc.tuKhoa.toLowerCase()) ||
      item.soDienThoai?.includes(boLoc.tuKhoa);

    // Lọc theo Vai trò
    const matchVaiTro = boLoc.vaiTro === 0 || item.maVaiTro === boLoc.vaiTro;

    // Lọc theo Trạng thái
    const matchTrangThai = !boLoc.trangThai || item.trangThai === boLoc.trangThai;

    return matchTuKhoa && matchVaiTro && matchTrangThai;
  });
});

const loadData = async () => {
  dangTai.value = true;
  try {
    const res = await axiosClient.get('/admin/AdminNguoiDung');
    danhSach.value = res.data || res;
  } finally {
    dangTai.value = false;
  }
};

const handleDoiTrangThai = async (nd) => {
  let lyDo = "";
  if (nd.trangThai === 'Hoạt động') {
    const { value } = await Swal.fire({
      title: 'Lý do khóa?',
      input: 'select',
      inputOptions: { 'Vi phạm': 'Vi phạm', 'Nghỉ việc': 'Nghỉ việc', 'Khác': 'Khác' },
      showCancelButton: true
    });
    if (!value) return;
    lyDo = value;
  }
  nd.dangXuLy = true;
  try {
    const res = await axiosClient.put(`/admin/AdminNguoiDung/doi-trang-thai/${nd.maNguoiDung}`, { LyDo: lyDo });
    const data = res.data || res;
    nd.trangThai = data.trangThaiMoi;
    Swal.fire('Thành công', `Đã ${nd.trangThai}`, 'success');
  } finally { nd.dangXuLy = false; }
};

onMounted(loadData);
defineExpose({ loadData });
</script>
<template>
  <div class="card shadow mb-4" id="phan-tong-quan">
    <div class="card-header py-3">
      <h6 class="m-0 font-weight-bold text-primary">1. Tổng quan — Danh sách người dùng</h6>
      <p class="qlnd-hint mb-0 mt-1">Màn hình đầu tiên khi Admin mở mục Quản lý người dùng.</p>
    </div>
    <div class="card-body">

      <div class="qlnd-toolbar">
        <div class="form-group">
          <label>Lọc theo vai trò</label>
          <select class="form-control form-control-sm" v-model="locVaiTro">
            <option value="">— Tất cả vai trò —</option>
            <option>Admin</option>
            <option>Nhân viên</option>
            <option>Khách hàng</option>
          </select>
        </div>
        <div class="form-group">
          <label>Lọc theo trạng thái</label>
          <select class="form-control form-control-sm" v-model="locTrangThai">
            <option value="">— Tất cả trạng thái —</option>
            <option>Đang hoạt động</option>
            <option>Bị khóa</option>
          </select>
        </div>
        <div class="qlnd-search-wrap form-group">
          <label>Tìm kiếm</label>
          <div class="input-group input-group-sm">
            <input type="search" class="form-control" v-model="tuKhoa"
              placeholder="Tên, email hoặc số điện thoại…" />
            <div class="input-group-append">
              <button class="btn btn-primary" type="button">
                <i class="fas fa-search fa-sm"></i>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Loading -->
      <div v-if="dangTai" class="text-center py-4">
        <div class="spinner-border text-primary" role="status">
          <span class="sr-only">Đang tải...</span>
        </div>
      </div>

      <div v-else class="table-responsive">
        <table class="table table-bordered table-hover">
          <thead class="thead-light">
            <tr>
              <th>ID</th>
              <th>Ảnh đại diện</th>
              <th>Họ tên</th>
              <th>Số điện thoại</th>
              <th>Email</th>
              <th>Vai trò</th>
              <th>Trạng thái</th>
              <th style="min-width:200px;">Hành động</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="nd in nguoiDungDaLoc" :key="nd.maNguoiDung">
              <td>{{ nd.maNguoiDung }}</td>
              <td><img class="qlnd-avatar" :src="nd.anhDaiDien || '/img/undraw_profile.svg'" alt="" /></td>
              <td>{{ nd.hoTen }}</td>
              <td>{{ nd.soDienThoai }}</td>
              <td>{{ nd.email }}</td>
              <td>
                <span class="badge" :class="badgeVaiTro(nd.vaiTro)">{{ nd.vaiTro }}</span>
              </td>
              <td>
                <span class="badge" :class="nd.trangThai === 'Đang hoạt động' ? 'badge-success' : 'badge-danger'">
                  {{ nd.trangThai }}
                </span>
              </td>
              <td class="qlnd-actions">
                <button class="btn btn-info btn-sm" title="Xem chi tiết">
                  <i class="fas fa-eye"></i>
                </button>
                <button class="btn btn-warning btn-sm" title="Chỉnh sửa" @click="$emit('chinh-sua', nd)">
                  <i class="fas fa-edit"></i>
                </button>
                <button
                  v-if="nd.trangThai === 'Đang hoạt động'"
                  class="btn btn-secondary btn-sm"
                  title="Khóa tài khoản"
                  :disabled="nd.dangXuLy"
                  @click="doiTrangThai(nd)"
                >
                  <i class="fas fa-lock"></i>
                </button>
                <button
                  v-else
                  class="btn btn-success btn-sm"
                  title="Mở khóa tài khoản"
                  :disabled="nd.dangXuLy"
                  @click="doiTrangThai(nd)"
                >
                  <i class="fas fa-unlock"></i>
                </button>
              </td>
            </tr>
            <tr v-if="nguoiDungDaLoc.length === 0">
              <td colspan="8" class="text-center text-muted">Không tìm thấy kết quả phù hợp.</td>
            </tr>
          </tbody>
        </table>
      </div>

    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import axiosClient from '../../api/axiosClient';

defineEmits(['chinh-sua']);

const danhSach    = ref([]);
const dangTai     = ref(false);
const locVaiTro   = ref('');
const locTrangThai = ref('');
const tuKhoa      = ref('');

const loadData = async () => {
  dangTai.value = true;
  try {
    const res = await axiosClient.get('/admin/AdminNguoiDung');
    
    // Sửa dòng này: Gán thẳng res vào danhSach.value
    danhSach.value = res; 
    
    console.log("Dữ liệu đã nhận:", danhSach.value); 
  } catch (err) {
    console.error('Lỗi tải danh sách:', err);
  } finally {
    dangTai.value = false;
  }
};

const doiTrangThai = async (nd) => {
  nd.dangXuLy = true;
  try {
    const res = await axiosClient.put(`/admin/AdminNguoiDung/doi-trang-thai/${nd.maNguoiDung}`);
    nd.trangThai = res.data.trangThaiMoi;
  } catch (err) {
    console.error('Lỗi đổi trạng thái:', err);
  } finally {
    nd.dangXuLy = false;
  }
};

const nguoiDungDaLoc = computed(() => {
  if (!Array.isArray(danhSach.value)) return [];

  return danhSach.value.filter(nd => {
    // 1. So sánh vai trò (Khớp với "KhachHang" hoặc "NhanVien" từ Swagger)
    const vaitroValue = locVaiTro.value === 'Khách hàng' ? 'KhachHang' : 
                        locVaiTro.value === 'Nhân viên' ? 'NhanVien' : locVaiTro.value;
    
    const khopVaiTro = !locVaiTro.value || nd.tenVaiTro === vaitroValue;
    
    // 2. So sánh trạng thái (Swagger trả về "Hoạt động", code cũ có thể là "Đang hoạt động")
    const khopTrangThai = !locTrangThai.value || nd.trangThai === locTrangThai.value;
    
    // 3. Tìm kiếm
    const search = tuKhoa.value.toLowerCase();
    const khopTuKhoa = !tuKhoa.value
      || nd.hoTen?.toLowerCase().includes(search)
      || nd.email?.toLowerCase().includes(search)
      || nd.soDienThoai?.includes(tuKhoa.value);

    return khopVaiTro && khopTrangThai && khopTuKhoa;
  });
});

const badgeVaiTro = (vaiTro) => ({
  'qlnd-badge-admin':    vaiTro === 'Admin',
  'qlnd-badge-staff':    vaiTro === 'Nhân viên',
  'qlnd-badge-customer': vaiTro === 'Khách hàng',
});

onMounted(loadData);
defineExpose({ loadData });
</script>
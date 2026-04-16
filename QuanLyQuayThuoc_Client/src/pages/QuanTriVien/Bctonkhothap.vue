<template>
  <div class="card shadow border-left-warning h-100">
    <div class="card-header py-3 bg-white">
      <h6 class="m-0 font-weight-bold text-warning">
        <i class="fas fa-boxes mr-1"></i> Tồn kho thấp
      </h6>
      <small class="text-muted">
        Tổng <strong>SoLuongTon</strong> chạm ngưỡng tối thiểu.
      </small>
    </div>
    <div class="card-body p-0">
      <div v-if="dangTai" class="text-center py-4">
        <div class="spinner-border text-warning" role="status"></div>
      </div>
      <div v-else class="table-responsive">
        <table class="table table-hover mb-0 bc-alert-table">
          <thead class="thead-light">
            <tr>
              <th class="pl-4">Tên thuốc</th>
              <th>Tồn hiện tại</th>
              <th>Ngưỡng</th>
              <th>Trạng thái</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="sp in danhSach" :key="sp.maThuoc">
              <td class="pl-4 font-weight-bold">{{ sp.tenThuoc }}</td>
              <td>
                <span :class="sp.tonHienTai < sp.nguongToiThieu ? 'text-danger' : 'text-warning'">
                  {{ sp.tonHienTai }}
                </span>
              </td>
              <td>{{ sp.nguongToiThieu }}</td>
              <td>
                <span class="badge" :class="badgeTrangThai(sp)">
                  {{ sp.trangThai }}
                </span>
              </td>
            </tr>
            <tr v-if="danhSach.length === 0">
              <td colspan="4" class="text-center text-muted py-4">
                <i class="fas fa-check-circle text-success mr-1"></i> Tồn kho ổn định.
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axiosClient from '../../api/axiosClient';

const danhSach = ref([]);
const dangTai   = ref(false);

const badgeTrangThai = (sp) => {
  if (sp.tonHienTai === 0) return 'badge-danger'; // Màu đỏ đậm
  if (sp.tonHienTai < sp.nguongToiThieu) return 'badge-warning text-dark'; // Màu vàng cảnh báo
  return 'badge-info'; // Màu xanh sắp hết
};

const loadData = async () => {
  dangTai.value = true;
  try {
    const res = await axiosClient.get('/BaoCao/ton-kho-thap');
    danhSach.value = res;
  } catch (err) { 
    console.error('Lỗi lấy dữ liệu tồn kho:', err); 
  }
  finally { dangTai.value = false; }
};

onMounted(loadData);
</script>

<style scoped>
.bc-alert-table {
  font-size: 0.85rem;
}
.bc-alert-table thead th {
  border-top: none;
  text-transform: uppercase;
  font-size: 0.75rem;
  letter-spacing: 0.5px;
}
.badge {
  padding: 0.5em 0.75em;
  border-radius: 10px;
}
</style>
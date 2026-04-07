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
              <th>Tên thuốc</th>
              <th>Tồn hiện tại</th>
              <th>Ngưỡng tối thiểu</th>
              <th>Trạng thái</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="sp in danhSach" :key="sp.maThuoc">
              <td>{{ sp.tenThuoc }}</td>
              <td><strong>{{ sp.tonHienTai }}</strong></td>
              <td>{{ sp.nguongToiThieu }}</td>
              <td>
                <span class="badge" :class="badgeTrangThai(sp)">
                  {{ sp.trangThai }}
                </span>
              </td>
            </tr>
            <tr v-if="danhSach.length === 0">
              <td colspan="4" class="text-center text-muted py-3">Tồn kho ổn định.</td>
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
const dangTai  = ref(false);

const badgeTrangThai = (sp) => {
  if (sp.tonHienTai === 0)                       return 'badge-danger';
  if (sp.tonHienTai < sp.nguongToiThieu)         return 'bc-badge-stock';
  if (sp.tonHienTai < sp.nguongToiThieu * 1.2)   return 'badge-warning text-dark';
  return 'badge-secondary';
};

onMounted(async () => {
  dangTai.value = true;
  try {
    const res = await axiosClient.get('/BaoCao/ton-kho-thap');
    danhSach.value = res.data;
  } catch (err) { console.error(err); }
  finally { dangTai.value = false; }
});
</script>
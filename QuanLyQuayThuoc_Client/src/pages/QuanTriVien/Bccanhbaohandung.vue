<template>
  <div class="card shadow border-left-danger h-100">
    <div class="card-header py-3 bg-white">
      <h6 class="m-0 font-weight-bold text-danger">
        <i class="fas fa-calendar-times mr-1"></i> Cảnh báo hạn sử dụng
      </h6>
      <small class="text-muted">
        Các lô hàng còn dưới <strong>6 tháng</strong> sử dụng.
      </small>
    </div>
    <div class="card-body p-0">
      <div v-if="dangTai" class="text-center py-4">
        <div class="spinner-border text-danger" role="status"></div>
      </div>
      <div v-else class="table-responsive">
        <table class="table table-hover mb-0 bc-alert-table">
          <thead class="thead-light">
            <tr>
              <th class="pl-4">Tên thuốc</th>
              <th>Số lô</th>
              <th>Hạn dùng</th>
              <th>Còn lại</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="lo in danhSach" :key="lo.maLo">
              <td class="pl-4 font-weight-bold">{{ lo.tenThuoc }}</td>
              <td><code class="text-dark">{{ lo.soLo }}</code></td>
              <td>{{ lo.hanSuDung }}</td>
              <td>
                <span class="badge" :class="lo.conLaiThang <= 3 ? 'badge-danger' : 'badge-warning text-dark'">
                  {{ lo.conLaiThang <= 0 ? 'Sắp hết hạn' : lo.conLaiThang + ' tháng' }}
                </span>
              </td>
            </tr>
            <tr v-if="danhSach.length === 0">
              <td colspan="4" class="text-center text-muted py-4">
                <i class="fas fa-check-circle text-success mr-1"></i> Tất cả các lô còn hạn dài.
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

const loadData = async () => {
  dangTai.value = true;
  try {
    const res = await axiosClient.get('/BaoCao/canh-bao-han-dung');
    danhSach.value = res;
  } catch (err) { 
    console.error('Lỗi lấy hạn sử dụng:', err); 
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
  font-size: 0.75rem;
  text-transform: uppercase;
}
.badge {
  padding: 0.5em 0.7em;
  min-width: 70px;
}
</style>
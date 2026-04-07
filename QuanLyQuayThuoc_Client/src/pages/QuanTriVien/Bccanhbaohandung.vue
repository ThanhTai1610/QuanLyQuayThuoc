<template>
  <div class="card shadow border-left-danger h-100">
    <div class="card-header py-3 bg-white">
      <h6 class="m-0 font-weight-bold text-danger">
        <i class="fas fa-calendar-times mr-1"></i> Cảnh báo hạn sử dụng
      </h6>
      <small class="text-muted">
        Nguồn: <strong>HanSuDung</strong> trên lô — còn dưới 3 hoặc 6 tháng.
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
              <th>Tên thuốc</th>
              <th>Số lô</th>
              <th>Hạn dùng</th>
              <th>Còn lại</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="lo in danhSach" :key="lo.maLo">
              <td>{{ lo.tenThuoc }}</td>
              <td><code>{{ lo.soLo }}</code></td>
              <td>{{ lo.hanSuDung }}</td>
              <td>
                <span class="badge" :class="lo.conLaiThang <= 3 ? 'bc-badge-expiry--3' : 'bc-badge-expiry--6'">
                  {{ lo.conLaiThang }} tháng
                </span>
              </td>
            </tr>
            <tr v-if="danhSach.length === 0">
              <td colspan="4" class="text-center text-muted py-3">Không có cảnh báo.</td>
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

onMounted(async () => {
  dangTai.value = true;
  try {
    const res = await axiosClient.get('/BaoCao/canh-bao-han-dung');
    danhSach.value = res.data;
  } catch (err) { console.error(err); }
  finally { dangTai.value = false; }
});
</script>
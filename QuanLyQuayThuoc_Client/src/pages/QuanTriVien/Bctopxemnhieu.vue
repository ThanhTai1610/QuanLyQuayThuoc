<template>
  <div class="card shadow h-100">
    <div class="card-header py-3">
      <h6 class="m-0 font-weight-bold text-primary">Top xem nhiều nhất</h6>
      <small class="text-muted">Theo cột <strong>LuotXem</strong> trong bảng Thuoc.</small>
    </div>
    <div class="card-body">
      <div v-if="dangTai" class="text-center py-5">
        <div class="spinner-border text-primary" role="status"></div>
      </div>
      
      <div v-else>
        <div class="bc-rank-item mb-4" v-for="(sp, i) in danhSach" :key="sp.maThuoc"
          :class="{ 'mb-0': i === danhSach.length - 1 }">
          <div class="bc-rank-item__top d-flex justify-content-between mb-1">
            <span class="bc-rank-item__name font-weight-bold small">{{ sp.tenThuoc }}</span>
            <span class="bc-rank-item__views badge badge-light text-dark">{{ sp.luotXem.toLocaleString('vi-VN') }} lượt</span>
          </div>
          <div class="progress progress-sm">
            <div class="progress-bar" 
              :class="mauBar(i)"
              role="progressbar"
              :style="{ width: phanTramBar(sp.luotXem) + '%' }"
              :aria-valuenow="phanTramBar(sp.luotXem)" 
              aria-valuemin="0" 
              aria-valuemax="100">
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axiosClient from '../../api/axiosClient';

const danhSach = ref([]);
const dangTai   = ref(false);

// Màu sắc cho các thanh: 2 thằng đầu màu xanh đậm, 2 thằng sau màu xanh nhạt, còn lại xám
const mauBar = (i) => i < 2 ? 'bg-primary' : i < 4 ? 'bg-info' : 'bg-secondary';

// Tính phần trăm dựa trên thằng cao nhất (để thằng cao nhất luôn là 100% chiều ngang)
const phanTramBar = (luotXem) => {
  if (danhSach.value.length === 0) return 0;
  const max = danhSach.value[0]?.luotXem || 1;
  return Math.round((luotXem / max) * 100);
};

const loadData = async () => {
  dangTai.value = true;
  try {
    const res = await axiosClient.get('/BaoCao/top-xem-nhieu');
    danhSach.value = res;
  } catch (err) { 
    console.error('Lỗi lấy top lượt xem:', err); 
  }
  finally { dangTai.value = false; }
};

onMounted(loadData);
</script>

<style scoped>
.bc-rank-item__name {
  color: #5a5c69;
}
.progress {
  height: 0.5rem; /* Làm thanh progress thanh mảnh hơn */
  background-color: #eaecf4;
}
.progress-bar {
  transition: width 0.6s ease; /* Hiệu ứng chạy thanh khi load */
}
</style>
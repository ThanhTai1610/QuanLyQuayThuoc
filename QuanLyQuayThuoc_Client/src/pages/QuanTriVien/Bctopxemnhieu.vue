<template>
  <div class="card shadow h-100">
    <div class="card-header py-3">
      <h6 class="m-0 font-weight-bold text-primary">Top xem nhiều nhất</h6>
      <small class="text-muted">Theo cột <strong>LuotXem</strong> trong bảng Thuoc.</small>
    </div>
    <div class="card-body">
      <div v-if="dangTai" class="text-center py-3">
        <div class="spinner-border text-primary" role="status"></div>
      </div>
      <div v-else>
        <div class="bc-rank-item" v-for="(sp, i) in danhSach" :key="sp.maThuoc"
          :class="{ 'mb-0': i === danhSach.length - 1 }">
          <div class="bc-rank-item__top">
            <span class="bc-rank-item__name">{{ sp.tenThuoc }}</span>
            <span class="bc-rank-item__views">{{ sp.luotXem.toLocaleString('vi-VN') }}</span>
          </div>
          <div class="progress">
            <div class="progress-bar" :class="mauBar(i)"
              :style="{ width: phanTramBar(sp.luotXem) + '%' }">
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
const dangTai  = ref(false);
const mauBar   = (i) => i < 2 ? 'bg-primary' : i < 4 ? 'bg-info' : 'bg-secondary';

const phanTramBar = (luotXem) => {
  const max = danhSach.value[0]?.luotXem || 1;
  return Math.round((luotXem / max) * 100);
};

onMounted(async () => {
  dangTai.value = true;
  try {
    const res = await axiosClient.get('/BaoCao/top-xem-nhieu');
    danhSach.value = res.data;
  } catch (err) { console.error(err); }
  finally { dangTai.value = false; }
});
</script>
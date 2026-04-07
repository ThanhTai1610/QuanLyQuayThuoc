<template>
  <div class="card shadow h-100">
    <div class="card-header py-3">
      <h6 class="m-0 font-weight-bold text-primary">Top 5 sản phẩm bán chạy</h6>
      <small class="text-muted">Theo số lượng bán — tỷ trọng %.</small>
    </div>
    <div class="card-body">
      <div class="bc-chart-canvas-wrap bc-chart-canvas-wrap--pie">
        <canvas ref="canvasRef"></canvas>
      </div>
      <ul class="list-unstyled small mt-3 mb-0">
        <li v-for="(sp, i) in danhSach" :key="sp.tenThuoc">
          <span class="badge mr-1" :style="{ background: mauSac[i] }">&nbsp;</span>
          {{ sp.tenThuoc }} — {{ sp.phanTram }}%
        </li>
      </ul>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue';
import axiosClient from '../../api/axiosClient';

const canvasRef = ref(null);
const danhSach  = ref([]);
const mauSac    = ['#4e73df','#1cc88a','#36b9cc','#f6c23e','#e74a3b'];
let chartInstance = null;

const loadData = async () => {
  try {
    const res = await axiosClient.get('/BaoCao/top-ban-chay');
    danhSach.value = res.data;
    veChart(res.data);
  } catch (err) { console.error(err); }
};

const veChart = (data) => {
  if (!canvasRef.value || !window.Chart) return;
  if (chartInstance) { chartInstance.destroy(); chartInstance = null; }
  chartInstance = new window.Chart(canvasRef.value.getContext('2d'), {
    type: 'doughnut',
    data: {
      labels: data.map(d => d.tenThuoc),
      datasets: [{ data: data.map(d => d.phanTram), backgroundColor: mauSac }],
    },
    options: { responsive: true, maintainAspectRatio: true, plugins: { legend: { display: false } } },
  });
};

onMounted(loadData);
onBeforeUnmount(() => { if (chartInstance) chartInstance.destroy(); });
</script>
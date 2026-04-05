<template>
  <div class="card shadow h-100">
    <div class="card-header py-3">
      <h6 class="m-0 font-weight-bold text-primary">Hiệu suất nhân viên (quầy)</h6>
      <small class="text-muted">Số đơn chốt thành công tại quầy trong tháng.</small>
    </div>
    <div class="card-body">
      <div class="bc-chart-canvas-wrap bc-chart-canvas-wrap--sm">
        <canvas ref="canvasRef"></canvas>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue';
import axiosClient from '../../api/axiosClient';

const canvasRef = ref(null);
let chartInstance = null;

onMounted(async () => {
  try {
    // GET /BaoCao/hieu-suat-nhan-vien → [{ hoTen, soDon }]
    const res = await axiosClient.get('/BaoCao/hieu-suat-nhan-vien');
    if (canvasRef.value && window.Chart) {
      chartInstance = new window.Chart(canvasRef.value.getContext('2d'), {
        type: 'bar',
        data: {
          labels: res.data.map(d => d.hoTen),
          datasets: [{
            label: 'Số đơn',
            data: res.data.map(d => d.soDon),
            backgroundColor: 'rgba(78,115,223,0.7)',
            borderColor: 'rgba(78,115,223,1)',
            borderWidth: 1,
          }],
        },
        options: {
          indexAxis: 'y',
          responsive: true,
          maintainAspectRatio: true,
          plugins: { legend: { display: false } },
          scales: { x: { beginAtZero: true } },
        },
      });
    }
  } catch (err) { console.error(err); }
});

onBeforeUnmount(() => { if (chartInstance) chartInstance.destroy(); });
</script>
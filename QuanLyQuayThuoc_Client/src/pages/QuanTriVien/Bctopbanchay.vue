<template>
  <div class="card shadow h-100">
    <div class="card-header py-3">
      <h6 class="m-0 font-weight-bold text-primary">Top 5 sản phẩm bán chạy</h6>
      <small class="text-muted">Theo số lượng bán — tỷ trọng %.</small>
    </div>
    <div class="card-body">
      <div class="bc-chart-canvas-wrap">
        <canvas ref="canvasRef"></canvas>
      </div>
      
      <ul class="list-unstyled small mt-4 mb-0">
        <li v-for="(sp, i) in danhSach" :key="sp.tenThuoc" class="mb-2 d-flex justify-content-between align-items-center">
          <span>
            <i class="fas fa-circle mr-2" :style="{ color: mauSac[i] }"></i>
            {{ sp.tenThuoc }}
          </span>
          <span class="font-weight-bold">{{ sp.phanTram }}%</span>
        </li>
      </ul>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount, nextTick } from 'vue';
import axiosClient from '../../api/axiosClient';
import Chart from 'chart.js/auto'; // Import chuẩn thư viện đã cài

const canvasRef = ref(null);
const danhSach = ref([]);
const mauSac = ['#4e73df', '#1cc88a', '#36b9cc', '#f6c23e', '#e74a3b'];
let chartInstance = null;

const loadData = async () => {
  try {
    const res = await axiosClient.get('/BaoCao/top-ban-chay');
    danhSach.value = res;
    
    // Đợi Vue render xong HTML rồi mới vẽ Chart
    await nextTick();
    veChart(res);
  } catch (err) {
    console.error('Lỗi tải Top bán chạy:', err);
  }
};

const veChart = (data) => {
  if (!canvasRef.value) return;

  if (chartInstance) {
    chartInstance.destroy();
  }

  const ctx = canvasRef.value.getContext('2d');
  
  chartInstance = new Chart(ctx, {
    type: 'doughnut', // Biểu đồ vòng khuyết trông hiện đại hơn
    data: {
      labels: data.map(d => d.tenThuoc),
      datasets: [{
        data: data.map(d => d.phanTram),
        backgroundColor: mauSac,
        hoverBorderColor: "rgba(234, 236, 244, 1)",
      }],
    },
    options: {
      maintainAspectRatio: false,
      tooltips: {
        backgroundColor: "rgb(255,255,255)",
        bodyFontColor: "#858796",
        borderColor: '#dddfeb',
        borderWidth: 1,
        xPadding: 15,
        yPadding: 15,
        displayColors: false,
        caretPadding: 10,
      },
      plugins: {
        legend: {
          display: false // Ẩn legend mặc định vì mình đã làm danh sách li ở dưới
        },
        cutout: '70%', // Độ rộng của vòng khuyết
      }
    },
  });
};

onMounted(loadData);

onBeforeUnmount(() => {
  if (chartInstance) chartInstance.destroy();
});
</script>

<style scoped>
.bc-chart-canvas-wrap {
  position: relative;
  height: 200px; /* Chiều cao vừa đủ cho biểu đồ tròn */
  width: 100%;
}
.fa-circle {
  font-size: 0.7rem;
}
</style>
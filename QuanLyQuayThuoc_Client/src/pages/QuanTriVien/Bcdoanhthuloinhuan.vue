<template>
  <div class="card shadow mb-4">
    <div class="card-header py-3 d-flex flex-wrap align-items-center justify-content-between">
      <div>
        <h6 class="m-0 font-weight-bold text-primary">Doanh thu &amp; Lợi nhuận</h6>
        <span class="bc-chart-legend-hint">
          Lợi nhuận = Doanh thu − Giá vốn (từ bảng <strong>LoHang</strong>).
        </span>
      </div>
    </div>
    <div class="card-body">
      <div class="bc-chart-toolbar">
        <span class="small font-weight-bold text-gray-600 mr-1">Xem theo:</span>
        <div class="btn-group btn-group-sm" role="group">
          <button v-for="ky in danhSachKy" :key="ky.value"
            type="button" class="btn btn-outline-primary"
            :class="{ active: kyChon === ky.value }"
            @click="doiKy(ky.value)">
            {{ ky.label }}
          </button>
        </div>
      </div>

      <div class="bc-chart-canvas-wrap">
        <canvas ref="canvasRef"></canvas>
      </div>

      <p class="bc-data-note mb-0">
        API trả về số liệu theo kỳ đã chọn — <strong>GET /BaoCao/doanh-thu-loi-nhuan?ky=thang</strong>.
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue';
import axiosClient from '../../api/axiosClient';

const canvasRef = ref(null);
const kyChon    = ref('thang');
let chartInstance = null;

const danhSachKy = [
  { value: 'ngay',  label: 'Ngày'  },
  { value: 'tuan',  label: 'Tuần'  },
  { value: 'thang', label: 'Tháng' },
  { value: 'quy',   label: 'Quý'   },
];

const loadData = async () => {
  try {
    const res = await axiosClient.get('/BaoCao/doanh-thu-loi-nhuan', { params: { ky: kyChon.value } });
    veChart(res.data);
  } catch (err) {
    console.error('Lỗi tải biểu đồ doanh thu:', err);
  }
};

const veChart = (data) => {
  if (!canvasRef.value) return;
  if (chartInstance) { chartInstance.destroy(); chartInstance = null; }

  const Chart = window.Chart;
  if (!Chart) return;

  chartInstance = new Chart(canvasRef.value.getContext('2d'), {
    type: 'bar',
    data: {
      labels: data.nhan,
      datasets: [
        {
          label: 'Doanh thu',
          data: data.doanhThu,
          backgroundColor: 'rgba(78,115,223,0.7)',
          borderColor: 'rgba(78,115,223,1)',
          borderWidth: 1,
        },
        {
          label: 'Lợi nhuận',
          data: data.loiNhuan,
          backgroundColor: 'rgba(28,200,138,0.7)',
          borderColor: 'rgba(28,200,138,1)',
          borderWidth: 1,
        },
      ],
    },
    options: {
      responsive: true,
      maintainAspectRatio: true,
      scales: { y: { beginAtZero: true } },
    },
  });
};

const doiKy = (ky) => { kyChon.value = ky; loadData(); };

onMounted(loadData);
onBeforeUnmount(() => { if (chartInstance) chartInstance.destroy(); });
</script>
<template>
  <div class="row mb-4">
    <div class="col-xl-3 col-md-6 mb-4" v-for="item in danhSach" :key="item.key">
      <div class="card bc-metric-card shadow h-100 py-2" :class="'border-left-' + item.color">
        <div class="card-body">
          <div class="row no-gutters align-items-center">
            <div class="col mr-2">
              <div class="bc-metric-label mb-1">{{ item.label }}</div>
              <div class="bc-metric-value">{{ item.value }}</div>
              <div class="bc-trend" :class="item.trendClass">
                <i :class="'fas fa-arrow-' + item.trendIcon"></i>
                {{ item.trendText }}
                <small>{{ item.trendNote }}</small>
              </div>
            </div>
            <div class="col-auto">
              <i :class="['fa-2x text-gray-300 fas', item.icon]"></i>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';

const props = defineProps({ duLieu: { type: Object, default: null } });

const formatGia = (v) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v ?? 0);

// Ánh xạ từ API /BaoCao/chi-so-chinh
const danhSach = computed(() => {
  const d = props.duLieu;
  if (!d) return [];
  return [
    {
      key: 'doanhthu', color: 'primary', icon: 'fa-coins',
      label: 'Tổng doanh thu',
      value: formatGia(d.tongDoanhThu),
      trendClass: 'bc-trend--up', trendIcon: 'up',
      trendText: `+${d.phanTramDoanhThu}%`, trendNote: 'so với tháng trước',
    },
    {
      key: 'donhang', color: 'success', icon: 'fa-shopping-bag',
      label: 'Đơn hàng thành công',
      value: d.soLuongDonHang?.toLocaleString('vi-VN'),
      trendClass: 'bc-trend--up', trendIcon: 'up',
      trendText: `+${d.phanTramDonHang}%`, trendNote: 'so với tháng trước',
    },
    {
      key: 'khachhang', color: 'info', icon: 'fa-user-plus',
      label: 'Khách hàng mới',
      value: d.khachHangMoi?.toLocaleString('vi-VN'),
      trendClass: 'bc-trend--up', trendIcon: 'up',
      trendText: `+${d.phanTramKhachHang}%`, trendNote: 'so với tháng trước',
    },
    {
      key: 'huydon', color: 'warning', icon: 'fa-times-circle',
      label: 'Tỷ lệ hủy đơn',
      value: `${d.tyLeHuyDon}%`,
      trendClass: 'bc-trend--good-down', trendIcon: 'down',
      trendText: `−${d.chenhLechHuyDon} điểm %`, trendNote: 'thấp hơn là tốt',
    },
  ];
});
</script>
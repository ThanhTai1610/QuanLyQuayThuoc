<template>
  <div class="pos-right">
    <div class="pos-card">
      <div class="pos-card__header">
        <h5 class="pos-card__title mb-0">Thanh toán</h5>
        <span class="badge badge-light text-gray-600" id="posMaDonHienThi">{{ maDonHang }}</span>
      </div>
      <div class="pos-summary p-3">
        <div class="mb-3">
          <label for="posSdt" class="small text-muted mb-1">SĐT khách (tích điểm)</label>
          <div class="input-group">
            <input type="tel" v-model="paymentInfo.sdtKhach" class="form-control" placeholder="0xxx xxx xxx">
            <div class="input-group-append">
              <button class="btn btn-outline-secondary" type="button" data-toggle="modal" data-target="#modalThemKhach">
                <i class="fas fa-user-plus"></i>
              </button>
            </div>
          </div>
        </div>

        <div class="mb-2 d-flex align-items-center justify-content-between">
          <span class="small text-muted">Tổng tiền hàng</span>
          <span class="pos-kpi-value">{{ formatMoney(tongTienHang) }}</span>
        </div>

        <div class="mb-2">
          <label class="small text-muted mb-1" for="posGiamGia">
            Chiết khấu / Giảm giá (đ)
          </label>
          <input type="number" v-model.number="paymentInfo.giamGia" class="form-control" min="0">
        </div>

        <div class="pos-highlight-due mb-2 p-2 bg-light rounded">
          <div class="d-flex align-items-center justify-content-between">
            <span class="small text-muted font-weight-bold">Khách cần trả</span>
            <span class="pos-kpi-value text-primary h5 mb-0">{{ formatMoney(khachCanTra) }}</span>
          </div>
        </div>

        <div class="mb-2">
          <label class="small text-muted mb-1">Tiền khách đưa (tiền mặt)</label>
          <input type="number" v-model.number="paymentInfo.tienKhachDua" class="form-control" min="0">
          <div class="small text-muted mt-1">
            Tiền thừa trả: <strong :class="{'text-danger': tienThua < 0}">{{ formatMoney(tienThua) }}</strong>
          </div>
        </div>

        <div class="mb-3">
          <label class="small text-muted mb-1">Phương thức</label>
          <div class="btn-group btn-group-toggle d-flex" role="group">
            <button 
              type="button" 
              class="btn btn-sm" 
              :class="paymentInfo.phuongThuc === 'tien-mat' ? 'btn-primary' : 'btn-outline-primary'"
              @click="paymentInfo.phuongThuc = 'tien-mat'"
            >
              <i class="fas fa-money-bill-wave mr-1"></i> Tiền mặt
            </button>
            <button 
              type="button" 
              class="btn btn-sm" 
              :class="paymentInfo.phuongThuc === 'chuyen-khoan' ? 'btn-primary' : 'btn-outline-primary'"
              @click="paymentInfo.phuongThuc = 'chuyen-khoan'"
            >
              <i class="fas fa-qrcode mr-1"></i> Chuyển khoản
            </button>
          </div>
        </div>

        <div v-if="paymentInfo.phuongThuc === 'chuyen-khoan'" class="mb-3 border rounded p-2 text-center bg-white">
          <div class="small text-muted mb-2">QR thanh toán (demo)</div>
          <img src="https://api.qrserver.com/v1/create-qr-code/?size=120x120&data=DemoPayment" alt="QR" style="width: 120px; height: 120px;">
          <div class="small text-muted mt-2">Quét QR để thanh toán</div>
        </div>

        <div class="d-flex flex-wrap align-items-center justify-content-between">
          <button type="button" class="btn btn-success flex-grow-1 mr-2" @click="handleCheckOut" :disabled="tongTienHang <= 0">
            <i class="fas fa-print mr-1"></i> F10 - Thanh toán
          </button>
          <button type="button" class="btn btn-outline-secondary" @click="$emit('clear-cart')">
            <i class="fas fa-trash"></i>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, computed } from 'vue';

const props = defineProps({
  tongTienHang: {
    type: Number,
    default: 0
  },
  maDonHang: {
    type: String,
    default: 'POS-0001'
  }
});

const emit = defineEmits(['checkout', 'clear-cart']);

// Dữ liệu thanh toán
const paymentInfo = reactive({
  sdtKhach: '',
  giamGia: 0,
  tienKhachDua: 0,
  phuongThuc: 'tien-mat'
});

// Tính toán khách cần trả
const khachCanTra = computed(() => {
  const amount = props.tongTienHang - paymentInfo.giamGia;
  return amount > 0 ? amount : 0;
});

// Tính tiền thừa
const tienThua = computed(() => {
  return paymentInfo.tienKhachDua - khachCanTra.value;
});

// Format tiền
const formatMoney = (val) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val);
};

// Gửi lệnh thanh toán ra trang chính
const handleCheckOut = () => {
  emit('checkout', {
    ...paymentInfo,
    khachCanTra: khachCanTra.value,
    tienThua: tienThua.value
  });
};
</script>
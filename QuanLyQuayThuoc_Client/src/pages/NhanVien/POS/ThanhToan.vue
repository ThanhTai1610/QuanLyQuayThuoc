<template>
  <div class="pos-right">
    <div class="pos-card">
      <div class="pos-card__header">
        <h5 class="pos-card__title mb-0">Thanh toán</h5>
        <span class="badge badge-light text-gray-600" id="posMaDonHienThi">{{ maDonHang }}</span>
      </div>
      <div class="pos-summary p-3">
        <div class="mb-2 d-flex align-items-center justify-content-between">
          <span class="small text-muted">Tổng tiền hàng</span>
          <span class="pos-kpi-value">{{ dinhDangTien(tongTienHang) }}</span>
        </div>

        <div class="mb-2">
          <label class="small text-muted mb-1" for="posGiamGia">
            Chiết khấu / Giảm giá (đ)
          </label>
          <input type="number" v-model.number="thongTinThanhToan.giamGia" class="form-control" min="0">
        </div>

        <div class="pos-highlight-due mb-2 p-2 bg-light rounded">
          <div class="d-flex align-items-center justify-content-between">
            <span class="small text-muted font-weight-bold">Khách cần trả</span>
            <span class="pos-kpi-value text-primary h5 mb-0">{{ dinhDangTien(khachCanTra) }}</span>
          </div>
        </div>

        <div class="mb-3">
          <label class="small text-muted mb-1 font-weight-bold">Tiền khách đưa (tiền mặt)</label>
          <div class="input-group mb-2">
            <input type="number" v-model.number="thongTinThanhToan.tienKhachDua"
              class="form-control text-right h5 mb-0 font-weight-bold" placeholder="0">
            <div class="input-group-append">
              <button class="btn btn-outline-danger" type="button" @click="datLaiTienKhachDua" title="Xóa tất cả">
                <span class="font-weight">Xoá</span>
              </button>
            </div>
          </div>

          <div class="row no-gutters">
            <div v-for="menhGia in cacMenhGiaNhanh" :key="menhGia" class="col-4 p-1">
              <button type="button" class="btn btn-sm btn-light border w-100 py-2 shadow-sm"
                @click="themTienKhachDua(menhGia)">
                +{{ menhGia >= 1000 ? (menhGia / 1000) + 'K' : menhGia }}
              </button>
            </div>
            <div class="col-12 p-1">
              <button type="button" class="btn btn-info w-100 font-weight-bold shadow-sm py-2" @click="datTienVuaDu">
                KHÁCH ĐƯA ĐỦ
              </button>
            </div>
          </div>

          <div class="mt-3 p-2 bg-light rounded d-flex justify-content-between align-items-center">
            <span class="small text-muted">Tiền thừa trả khách:</span>
            <strong :class="tienThua < 0 ? 'text-danger' : 'text-success'" style="font-size: 1.2rem;">
              {{ dinhDangTien(tienThua) }}
            </strong>
          </div>
        </div>
        <div class="mb-3">
          <label class="small text-muted mb-1">Phương thức</label>
          <div class="btn-group btn-group-toggle d-flex" role="group">
            <button type="button" class="btn btn-sm"
              :class="thongTinThanhToan.phuongThuc === 'tien-mat' ? 'btn-primary' : 'btn-outline-primary'"
              @click="thongTinThanhToan.phuongThuc = 'tien-mat'">
              <i class="fas fa-money-bill-wave mr-1"></i> Tiền mặt
            </button>
            <button type="button" class="btn btn-sm"
              :class="thongTinThanhToan.phuongThuc === 'chuyen-khoan' ? 'btn-primary' : 'btn-outline-primary'"
              @click="thongTinThanhToan.phuongThuc = 'chuyen-khoan'">
              <i class="fas fa-qrcode mr-1"></i> Chuyển khoản
            </button>
          </div>
        </div>

        <div v-if="thongTinThanhToan.phuongThuc === 'chuyen-khoan'"
          class="mb-3 border rounded p-2 text-center bg-white">
          <div class="small text-muted mb-2">QR thanh toán (demo)</div>
          <img src="https://api.qrserver.com/v1/create-qr-code/?size=120x120&data=DemoPayment" alt="QR"
            style="width: 120px; height: 120px;">
          <div class="small text-muted mt-2">Quét QR để thanh toán</div>
        </div>

        <div class="d-flex flex-wrap align-items-center justify-content-between">
          <button type="button" class="btn btn-success flex-grow-1 mr-2" @click="xuLyThanhToan"
            :disabled="tongTienHang <= 0">
            <i class="fas fa-print mr-1"></i> Thanh toán
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

const thongTinThanhToan = reactive({
  sdtKhach: '',
  giamGia: 0,
  tienKhachDua: 0,
  phuongThuc: 'tien-mat'
});

const khachCanTra = computed(() => {
  const soTien = props.tongTienHang - thongTinThanhToan.giamGia;
  return soTien > 0 ? soTien : 0;
});

// Danh sách các mệnh giá phổ biến
const cacMenhGiaNhanh = [1000, 2000, 5000, 10000, 20000, 50000, 100000, 200000, 500000];

const themTienKhachDua = (giaTri) => {
  thongTinThanhToan.tienKhachDua += giaTri;
};

const datLaiTienKhachDua = () => {
  thongTinThanhToan.tienKhachDua = 0;
};

const datTienVuaDu = () => {
  thongTinThanhToan.tienKhachDua = khachCanTra.value;
};

const tienThua = computed(() => {
  return thongTinThanhToan.tienKhachDua - khachCanTra.value;
});

const dinhDangTien = (giaTri) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(giaTri);
};

// Gửi lệnh thanh toán ra trang chính
const xuLyThanhToan = () => {
  emit('checkout', {
    ...thongTinThanhToan,
    khachCanTra: khachCanTra.value,
    tienThua: tienThua.value
  });
};
</script>
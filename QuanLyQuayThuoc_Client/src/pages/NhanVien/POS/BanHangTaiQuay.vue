<template>
  <div class="container-fluid pos-root">
    <TimKiem @add-to-cart="addToCart" />

    <div class="row mt-3">
      <div class="col-xl-8 col-lg-7">
        <GioHang 
          :cartItems="cartItems" 
          @remove-item="removeItem" 
          @update-quantity="updateQuantity" 
        />
      </div>

      <div class="col-xl-4 col-lg-5">
        <ThanhToan 
          :tongTienHang="totalAmount" 
          :maDonHang="maDonHang"
          @checkout="openInvoice" 
          @clear-cart="clearCart"
        />
      </div>
    </div>

    <Modals 
      :invoiceData="invoiceData"
      @add-quick-item="addToCart"
      @finish-payment="resetPage"
    />
  </div>
</template>

<script setup>
import { ref, computed, reactive } from 'vue';
import TimKiem from '../../NhanVien/POS/TimKiem.vue';
import GioHang from '../../NhanVien/POS/GioHang.vue';
import ThanhToan from '../../NhanVien/POS/ThanhToan.vue';
import Modals from '../../NhanVien/POS/Modals.vue';

// 1. Quản lý trạng thái giỏ hàng
const cartItems = ref([]);
const maDonHang = ref('POS-' + Math.floor(Math.random() * 10000).toString().padStart(4, '0'));

// 2. Tính tổng tiền tự động (Computed)
const totalAmount = computed(() => {
  return cartItems.value.reduce((sum, item) => sum + (item.giaBan * item.soLuong), 0);
});

// 3. Dữ liệu chuẩn bị cho Hóa đơn
const invoiceData = reactive({
  maHd: maDonHang.value,
  thoiGian: '',
  khachHang: '',
  cartItems: [],
  tongTienHang: 0,
  giamGia: 0,
  canTra: 0,
  phuongThuc: 'Tiền mặt'
});

// --- CÁC HÀM XỬ LÝ (LOGIC) ---

// Thêm thuốc vào giỏ
const addToCart = (product) => {
  const existing = cartItems.value.find(i => i.id === product.id && product.id !== undefined);
  if (existing) {
    existing.soLuong += product.soLuong;
  } else {
    cartItems.value.push({ ...product });
  }
};

// Cập nhật số lượng (+/-)
const updateQuantity = ({ index, change }) => {
  const item = cartItems.value[index];
  if (item) {
    item.soLuong += change;
    if (item.soLuong <= 0) removeItem(index);
  }
};

// Xóa 1 dòng
const removeItem = (index) => {
  cartItems.value.splice(index, 1);
};

// Xóa sạch giỏ
const clearCart = () => {
  if (confirm('Bạn có chắc muốn xóa toàn bộ giỏ hàng?')) {
    cartItems.value = [];
  }
};

// Mở hóa đơn (Khi bấm F10 ở ThanhToan.vue)
const openInvoice = (paymentDetail) => {
  invoiceData.maHd = maDonHang.value;
  invoiceData.thoiGian = new Date().toLocaleString('vi-VN');
  invoiceData.cartItems = [...cartItems.value];
  invoiceData.tongTienHang = totalAmount.value;
  invoiceData.giamGia = paymentDetail.giamGia;
  invoiceData.canTra = paymentDetail.khachCanTra;
  invoiceData.phuongThuc = paymentDetail.phuongThuc === 'tien-mat' ? 'Tiền mặt' : 'Chuyển khoản';
  
  // Kích hoạt Modal bằng Bootstrap (Nếu dùng jQuery)
  // eslint-disable-next-line no-undef
  $('#modalHoaDon').modal('show');
};

// Reset sau khi thanh toán xong
const resetPage = () => {
  cartItems.value = [];
  maDonHang.value = 'POS-' + Math.floor(Math.random() * 10000).toString().padStart(4, '0');
  // eslint-disable-next-line no-undef
  $('.modal').modal('hide');
};

</script>

<style scoped>
/* Gắn CSS đặc thù của trang POS */
@import "../../../assets/css_admin/pos.css";

/* Bổ sung một số style để giao diện cân đối hơn trong Layout Admin */
.pos-root {
  padding-top: 1rem;
  padding-bottom: 2rem;
  background-color: #f8f9fc;
  min-height: 100vh;
}
</style>
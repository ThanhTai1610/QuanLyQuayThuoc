<template>
  <div class="container-fluid pos-root">
    <TimKiem @add-to-cart="addToCart" />

    <div class="row mt-3">
      <div class="col-xl-8 col-lg-7">
        <GioHang :cartItems="cartItems" @remove-item="removeItem" @update-quantity="updateQuantity" />
      </div>

      <div class="col-xl-4 col-lg-5">
        <ThanhToan :tongTienHang="totalAmount" :maDonHang="maDonHang" @checkout="openInvoice" @clear-cart="clearCart" />
      </div>
    </div>

    <Modals :invoiceData="invoiceData" @add-quick-item="addToCart" @finish-payment="handleFinishPayment" />
  </div>
</template>

<script setup>
import { ref, computed, reactive } from 'vue';
import axios from 'axios';
import TimKiem from '../../NhanVien/POS/TimKiem.vue';
import GioHang from '../../NhanVien/POS/GioHang.vue';
import ThanhToan from '../../NhanVien/POS/ThanhToan.vue';
import Modals from '../../NhanVien/POS/Modals.vue';
import Swal from 'sweetalert2';

// STATE
const cartItems = ref([]);
const maDonHang = ref('POS-' + Math.floor(Math.random() * 10000).toString().padStart(4, '0'));

const invoiceData = reactive({
  maHd: maDonHang.value,
  thoiGian: '',
  khachHang: '',
  cartItems: [],
  tongTienHang: 0,
  giamGia: 0,
  canTra: 0,
  phuongThuc: 'Tiền mặt',
  _paymentDetail: null
});

// COMPUTED
const totalAmount = computed(() => {
  return cartItems.value.reduce((sum, item) => sum + (item.giaBan * item.soLuong), 0);
});

// CART LOGIC
const addToCart = (product) => {
  const existing = cartItems.value.find(i => i.maThuoc === product.maThuoc);

  if (existing) {
    existing.soLuong += 1;
  } else {
    cartItems.value.push({
      ...product,
      soLuong: 1,
      maDvtSelected: product.maDvtSelected || product.danhSachDonVi[0]?.maDvt,
      loHangSelected: product.loHangSelected || product.danhSachLo[0]?.maLo
    });
  }
};

const updateQuantity = ({ index, change }) => {
  const item = cartItems.value[index];
  if (item) {
    item.soLuong += change;
    if (item.soLuong <= 0) removeItem(index);
  }
};

const removeItem = (index) => {
  cartItems.value.splice(index, 1);
};

const clearCart = () => {
  if (confirm('Bạn có chắc muốn xóa toàn bộ giỏ hàng?')) {
    cartItems.value = [];
  }
};


// OPEN INVOICE
const openInvoice = (paymentDetail) => {
  if (cartItems.value.length === 0) {
    alert("Giỏ hàng đang trống");
    return;
  }

  invoiceData.maHd = maDonHang.value;
  invoiceData.thoiGian = new Date().toLocaleString('vi-VN');
  invoiceData.cartItems = [...cartItems.value];
  invoiceData.tongTienHang = totalAmount.value;
  invoiceData.giamGia = paymentDetail.giamGia;
  invoiceData.canTra = paymentDetail.khachCanTra;
  invoiceData.phuongThuc = paymentDetail.phuongThuc === 'tien-mat' ? 'Tiền mặt' : 'Chuyển khoản';
  invoiceData._paymentDetail = paymentDetail;

  const modalElement = document.getElementById('modalHoaDon');
  if (modalElement) {
    const myModal = window.bootstrap.Modal.getOrCreateInstance(modalElement);
    myModal.show();
  }
};

// CALL API THANH TOÁN
const thanhToanApi = async (paymentDetail) => {
  if (cartItems.value.length === 0) return;

  try {
    const dto = {
      maKhachHang: 0,
      phuongThucThanhToan: paymentDetail.phuongThuc === 'tien-mat' ? 'tienmat' : 'chuyenkhoan',
      giamGia: paymentDetail.giamGia || 0,
      chiTiet: cartItems.value.map(item => ({
        maLo: item.loHangSelected,
        maDVT: item.maDvtSelected,
        soLuong: item.soLuong,
        giaBan: item.giaBan
      }))
    };

    const res = await axios.post(
      'https://localhost:7070/api/BanHang/thanh-toan',
      dto
    );

    if (res.data.success) {
      Swal.fire({
        title: 'Thành công!',
        text: `Hóa đơn ${res.data.maDonHang} đã được lưu hệ thống.`,
        icon: 'success',
        confirmButtonText: 'Đóng',
        confirmButtonColor: '#28a745',
        timer: 2500,
        timerProgressBar: true
      });

      resetPage();
    }
  } catch (err) {
    console.error(err);
    Swal.fire({
      title: 'Lỗi thanh toán',
      text: err.response?.data?.message || 'Không thể kết nối Server, vui lòng thử lại!',
      icon: 'error',
      confirmButtonText: 'Kiểm tra lại'
    });
  }
};

const handleFinishPayment = () => {
  thanhToanApi(invoiceData._paymentDetail);
};

const resetPage = () => {
  cartItems.value = [];
  maDonHang.value = 'POS-' + Math.floor(Math.random() * 10000).toString().padStart(4, '0');
  const modalElement = document.getElementById('modalHoaDon');
  if (modalElement) {
    const modalInstance = window.bootstrap.Modal.getInstance(modalElement);
    if (modalInstance) {
      modalInstance.hide();
    }
  }

  const backdrop = document.querySelector('.modal-backdrop');
  if (backdrop) {
    backdrop.remove();
    document.body.classList.remove('modal-open');
    document.body.style.overflow = '';
    document.body.style.paddingRight = '';
  }
};
</script>

<style scoped>
@import "../../../assets/css_admin/pos.css";

.pos-root {
  padding-top: 1rem;
  padding-bottom: 2rem;
  background-color: #f8f9fc;
  min-height: 100vh;
}
</style>
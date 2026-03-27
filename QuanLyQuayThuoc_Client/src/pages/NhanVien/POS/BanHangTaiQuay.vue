<template>
  <div class="container-fluid pos-root">
    <TimKiem @add-to-cart="themVaoGioHang" />

    <div class="row mt-3">
      <div class="col-xl-8 col-lg-7">
        <GioHang :cartItems="cacSanPhamTrongGio" @remove-item="xoaSanPham" @update-quantity="capNhatSoLuong" />
      </div>

      <div class="col-xl-4 col-lg-5">
        <ThanhToan :tongTienHang="tongTienHang" :maDonHang="maDonHang" @checkout="moHoaDon" @clear-cart="xoaGioHang" />
      </div>
    </div>

    <Modals :invoiceData="duLieuHoaDon" @add-quick-item="themVaoGioHang" @finish-payment="xuLyHoanThanhThanhToan" />
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
const cacSanPhamTrongGio = ref([]);
const maDonHang = ref('POS-' + Math.floor(Math.random() * 10000).toString().padStart(4, '0'));

const duLieuHoaDon = reactive({
  maHd: maDonHang.value,
  thoiGian: '',
  khachHang: '',
  cacSanPhamTrongGio: [],
  tongTienHang: 0,
  giamGia: 0,
  canTra: 0,
  phuongThuc: 'Tiền mặt',
  _chiTietThanhToan: null
});

// COMPUTED
const tongTienHang = computed(() => {
  return cacSanPhamTrongGio.value.reduce((tong, sanPham) => tong + (sanPham.giaBan * sanPham.soLuong), 0);
});

// CART LOGIC
const themVaoGioHang = (sanPham) => {
  const sanPhamHienCo = cacSanPhamTrongGio.value.find(i => i.maThuoc === sanPham.maThuoc);

  if (sanPhamHienCo) {
    sanPhamHienCo.soLuong += 1;
  } else {
    cacSanPhamTrongGio.value.push({
      ...sanPham,
      soLuong: 1,
      maDvtSelected: sanPham.maDvtSelected || sanPham.danhSachDonVi[0]?.maDvt,
      loHangSelected: sanPham.loHangSelected || sanPham.danhSachLo[0]?.maLo
    });
  }
};

const capNhatSoLuong = ({ index, change }) => { 
  const sanPham = cacSanPhamTrongGio.value[index];
  if (sanPham) {
    sanPham.soLuong += change;
    if (sanPham.soLuong <= 0) xoaSanPham(index);
  }
};

const xoaSanPham = (viTri) => {
  cacSanPhamTrongGio.value.splice(viTri, 1);
};

const xoaGioHang = () => {
  if (cacSanPhamTrongGio.value.length === 0) return;

  Swal.fire({
    title: 'Xác nhận xóa?',
    text: "Toàn bộ thuốc trong giỏ sẽ bị loại bỏ",
    icon: 'warning',
    showCancelButton: true,
    confirmButtonColor: '#d33',
    cancelButtonColor: '#3085d6',
    confirmButtonText: 'Xoá',
    cancelButtonText: 'Hoàn tác'
  }).then((ketQua) => {
    if (ketQua.isConfirmed) {
      cacSanPhamTrongGio.value = [];
      Swal.fire(
        'Đã xóa!',
        'Giỏ hàng của bạn hiện đang trống.',
        'success'
      );
    }
  });
};


// OPEN INVOICE
const moHoaDon = (chiTietThanhToan) => {
  if (cacSanPhamTrongGio.value.length === 0) {
    Swal.fire({
      icon: 'info',
      title: 'Giỏ hàng trống',
      text: 'Vui lòng chọn ít nhất một loại thuốc để thanh toán!',
      confirmButtonColor: '#3085d6'
    });
    return;
  }

  duLieuHoaDon.maHd = maDonHang.value;
  duLieuHoaDon.thoiGian = new Date().toLocaleString('vi-VN');
  duLieuHoaDon.cartItems = [...cacSanPhamTrongGio.value];
  duLieuHoaDon.tongTienHang = tongTienHang.value;
  duLieuHoaDon.giamGia = chiTietThanhToan.giamGia;
  duLieuHoaDon.canTra = chiTietThanhToan.khachCanTra;
  duLieuHoaDon.phuongThuc = chiTietThanhToan.phuongThuc === 'tien-mat' ? 'Tiền mặt' : 'Chuyển khoản';
  duLieuHoaDon._chiTietThanhToan = chiTietThanhToan;

  const phanTuModal = document.getElementById('modalHoaDon');
  if (phanTuModal) {
    const modalCuaToi = window.bootstrap.Modal.getOrCreateInstance(phanTuModal);
    modalCuaToi.show();
  }
};

// CALL API THANH TOÁN
const goiApiThanhToan = async (chiTietThanhToan) => {
  if (cacSanPhamTrongGio.value.length === 0) return;

  try {
    const dto = {
      maKhachHang: 0,
      phuongThucThanhToan: chiTietThanhToan.phuongThuc === 'tien-mat' ? 'tienmat' : 'chuyenkhoan',
      giamGia: chiTietThanhToan.giamGia || 0,
      chiTiet: cacSanPhamTrongGio.value.map(sanPham => ({
        maLo: sanPham.loHangSelected,
        maDVT: sanPham.maDvtSelected,
        soLuong: sanPham.soLuong,
        giaBan: sanPham.giaBan
      }))
    };

    const ketQua = await axios.post(
      'https://localhost:7070/api/BanHang/thanh-toan',
      dto
    );

    if (ketQua.data.success) {
      Swal.fire({
        title: 'Thành công!',
        text: `Hóa đơn ${ketQua.data.maDonHang} đã được lưu hệ thống.`,
        icon: 'success',
        confirmButtonText: 'Đóng',
        confirmButtonColor: '#28a745',
        timer: 2500,
        timerProgressBar: true
      });

      datLaiTrang();
    }
  } catch (loi) {
    console.error(loi);
    Swal.fire({
      title: 'Lỗi thanh toán',
      text: loi.response?.data?.message || 'Không thể kết nối Server, vui lòng thử lại!',
      icon: 'error',
      confirmButtonText: 'Kiểm tra lại'
    });
  }
};

const xuLyHoanThanhThanhToan = () => {
  goiApiThanhToan(duLieuHoaDon._chiTietThanhToan);
};

const datLaiTrang = () => {
  cacSanPhamTrongGio.value = [];
  maDonHang.value = 'POS-' + Math.floor(Math.random() * 10000).toString().padStart(4, '0');
  const phanTuModal = document.getElementById('modalHoaDon');
  if (phanTuModal) {
    const phienBanModal = window.bootstrap.Modal.getInstance(phanTuModal);
    if (phienBanModal) {
      phienBanModal.hide();
    }
  }

  const nenModal = document.querySelector('.modal-backdrop');
  if (nenModal) {
    nenModal.remove();
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
<template>
  <div class="container-fluid pos-root">
    <TimKiem @add-to-cart="themVaoGioHang" />

    <div class="row mt-3">
      <div class="col-xl-8 col-lg-7">
        <GioHang :cartItems="cacSanPhamTrongGio" @remove-item="xoaSanPham" @update-quantity="capNhatSoLuong"
          @add-to-cart="themVaoGioHang" />
      </div>

      <div class="col-xl-4 col-lg-5">
        <ThanhToan :tongTienHang="tongTienHang" :maDonHang="maDonHang" @checkout="moHoaDon" @clear-cart="xoaGioHang" />
      </div>
    </div>

    <Modals :invoiceData="duLieuHoaDon" :isMoMo="isMoMoComplete" @add-quick-item="themVaoGioHang"
      @finish-payment="xuLyHoanThanhThanhToan" />
  </div>
</template>

<script setup>
import { ref, computed, reactive, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import axios from 'axios';
import TimKiem from '../../NhanVien/POS/TimKiem.vue';
import GioHang from '../../NhanVien/POS/GioHang.vue';
import ThanhToan from '../../NhanVien/POS/ThanhToan.vue';
import Modals from '../../NhanVien/POS/Modals.vue';
import { useMomo } from '../../../services/useMomo';
import { Modal } from 'bootstrap';
import Swal from 'sweetalert2';

const route = useRoute();
const router = useRouter();
const { createPayment } = useMomo();
const isMoMoComplete = ref(false);

// ─── STATE ────────────────────────────────────────────────────────────────────
const cacSanPhamTrongGio = ref([]);
const maDonHang = ref('POS-' + Math.floor(Math.random() * 10000).toString().padStart(4, '0'));

const duLieuHoaDon = reactive({
  maHd: '',
  thoiGian: '',
  khachHang: '',
  cartItems: [],
  tongTienHang: 0,
  giamGia: 0,
  canTra: 0,
  phuongThuc: 'Tiền mặt',
  _chiTietThanhToan: null
});

// ─── COMPUTED ─────────────────────────────────────────────────────────────────
const tongTienHang = computed(() =>
  cacSanPhamTrongGio.value.reduce((sum, sp) => sum + sp.giaBan * sp.soLuong, 0)
);

// ─── CART LOGIC ───────────────────────────────────────────────────────────────
const themVaoGioHang = (sanPham) => {
  const hiCo = cacSanPhamTrongGio.value.find(i => i.maThuoc === sanPham.maThuoc);
  if (hiCo) {
    hiCo.soLuong += 1;
  } else {
    cacSanPhamTrongGio.value.push({
      ...sanPham,
      soLuong: sanPham.soLuong || 1,
      maDvtSelected: sanPham.maDvtSelected || sanPham.danhSachDonVi?.[0]?.maDvt,
      loHangSelected: sanPham.loHangSelected || sanPham.danhSachLo?.[0]?.maLo
    });
  }
};

const capNhatSoLuong = ({ index, change }) => {
  const sp = cacSanPhamTrongGio.value[index];
  if (sp) {
    sp.soLuong += change;
    if (sp.soLuong <= 0) xoaSanPham(index);
  }
};

const xoaSanPham = (viTri) => { cacSanPhamTrongGio.value.splice(viTri, 1); };

const xoaGioHang = () => {
  if (!cacSanPhamTrongGio.value.length) return;
  Swal.fire({
    title: 'Xác nhận xóa?', text: 'Toàn bộ thuốc trong giỏ sẽ bị loại bỏ',
    icon: 'warning', showCancelButton: true,
    confirmButtonColor: '#d33', cancelButtonColor: '#3085d6',
    confirmButtonText: 'Xoá', cancelButtonText: 'Hoàn tác'
  }).then(r => {
    if (r.isConfirmed) {
      cacSanPhamTrongGio.value = [];
      Swal.fire('Đã xóa!', 'Giỏ hàng trống.', 'success');
    }
  });
};

// ─── CHECKOUT ROUTER ─────────────────────────────────────────────────────────
const moHoaDon = (chiTietThanhToan) => {
  if (!cacSanPhamTrongGio.value.length) {
    Swal.fire({ icon: 'info', title: 'Giỏ hàng trống', text: 'Vui lòng chọn ít nhất một loại thuốc!', confirmButtonColor: '#3085d6' });
    return;
  }

  if (chiTietThanhToan.phuongThuc === 'momo') {
    xuLyMoMo(chiTietThanhToan);
    return;
  }

  // Tiền mặt → mở modal hóa đơn
  dienDuLieuHoaDon(chiTietThanhToan, tongTienHang.value, [...cacSanPhamTrongGio.value], 'Tiền mặt');
  Modal.getOrCreateInstance(document.getElementById('modalHoaDon')).show();
};

// ─── HELPER ──────────────────────────────────────────────────────────────────
const dienDuLieuHoaDon = (chiTiet, tongTien, danhSachHang, tenPhuongThuc) => {
  duLieuHoaDon.maHd = maDonHang.value;
  duLieuHoaDon.thoiGian = new Date().toLocaleString('vi-VN');
  duLieuHoaDon.cartItems = danhSachHang;
  duLieuHoaDon.tongTienHang = tongTien;
  duLieuHoaDon.giamGia = chiTiet?.giamGia ?? 0;
  duLieuHoaDon.canTra = tongTien - (chiTiet?.giamGia ?? 0);
  duLieuHoaDon.phuongThuc = tenPhuongThuc;
  duLieuHoaDon._chiTietThanhToan = chiTiet;
};

// ─── FLOW MOMO: tạo đơn hàng → redirect sang trang MoMo ─────────────────────
const xuLyMoMo = async (chiTietThanhToan) => {
  // ✅ Lưu ngay trước khi làm bất cứ điều gì
  const soTien = Math.round(tongTienHang.value - (chiTietThanhToan.giamGia || 0));
  const snapshot = [...cacSanPhamTrongGio.value];
  const tongTienSnapshot = tongTienHang.value;

  try {
    const token = localStorage.getItem('token');

    // Bước 1: Tạo đơn hàng trên backend
    const dto = {
      phuongThucThanhToan: 'Momo',
      giamGia: chiTietThanhToan.giamGia || 0,
      chiTiet: snapshot.map(sp => ({
        maLo: sp.loHangSelected,
        maDVT: sp.maDvtSelected,
        soLuong: sp.soLuong,
        giaBan: sp.giaBan
      }))
    };

    const ketQua = await axios.post(
      'https://localhost:7070/api/BanHang/thanh-toan',
      dto,
      { headers: { Authorization: `Bearer ${token}` } }
    );

    // ✅ Hỗ trợ cả chữ hoa (C#) lẫn chữ thường
    const isSuccess = ketQua.data?.Success === true || ketQua.data?.success === true;
    const maDonHangMoi = ketQua.data?.MaDonHang ?? ketQua.data?.maDonHang;

    if (!isSuccess || maDonHangMoi == null) {
      throw new Error(ketQua.data?.Message || ketQua.data?.message || 'Tạo đơn hàng thất bại');
    }

    // Chuẩn bị dữ liệu hóa đơn để hiện sau khi MoMo redirect về
    // (lưu vào sessionStorage để dùng lại khi trang reload)
    sessionStorage.setItem('momoInvoice', JSON.stringify({
      maHd: String(maDonHangMoi),
      thoiGian: new Date().toLocaleString('vi-VN'),
      cartItems: snapshot,
      tongTienHang: tongTienSnapshot,
      giamGia: chiTietThanhToan.giamGia || 0,
      canTra: soTien,
      phuongThuc: 'Ví MoMo'
    }));

    // Bước 2: ✅ Gọi useMomo.createPayment → redirect thẳng sang trang MoMo
    // (giống hệt DatHang.vue, chỉ khác UserType = 'NhanVien')
    await createPayment(
      soTien,
      `Bán tại quầy - HD: ${maDonHangMoi}`,
      String(maDonHangMoi),
      'NhanVien'
    );

    // Nếu createPayment thành công thì window.location.href đã chuyển trang rồi,
    // code dưới đây sẽ không chạy

  } catch (err) {
    console.error('Lỗi MoMo:', err);
    const msg = err.response?.data?.detail || err.response?.data?.message || err.message || 'Lỗi không xác định';
    Swal.fire({ icon: 'error', title: 'Thanh toán MoMo thất bại', text: msg });
  }
};

// ─── TIỀN MẶT: gọi API thanh toán khi bấm "Thanh toán xong" ─────────────────
const goiApiThanhToan = async (chiTietThanhToan) => {
  if (!cacSanPhamTrongGio.value.length) return;
  try {
    const token = localStorage.getItem('token');
    const dto = {
      phuongThucThanhToan: 'TienMat',
      giamGia: chiTietThanhToan?.giamGia || 0,
      chiTiet: cacSanPhamTrongGio.value.map(sp => ({
        maLo: sp.loHangSelected,
        maDVT: sp.maDvtSelected,
        soLuong: sp.soLuong,
        giaBan: sp.giaBan
      }))
    };

    const ketQua = await axios.post(
      'https://localhost:7070/api/BanHang/thanh-toan',
      dto,
      { headers: { Authorization: `Bearer ${token}` } }
    );

    const isSuccess = ketQua.data?.Success === true || ketQua.data?.success === true;
    const maDonHangMoi = ketQua.data?.MaDonHang ?? ketQua.data?.maDonHang;

    if (isSuccess && maDonHangMoi != null) {
      Swal.fire({
        title: 'Thành công!', text: `Hóa đơn #${maDonHangMoi} đã được lưu.`,
        icon: 'success', confirmButtonText: 'Đóng', confirmButtonColor: '#28a745',
        timer: 2500, timerProgressBar: true
      });
      datLaiTrang();
    } else {
      Swal.fire({ title: 'Lỗi', text: ketQua.data?.Message || 'Thanh toán thất bại.', icon: 'error' });
    }
  } catch (loi) {
    Swal.fire({ title: 'Lỗi', text: loi.response?.data?.Message || loi.message || 'Không thể kết nối Server', icon: 'error' });
  }
};

const xuLyHoanThanhThanhToan = () => {
  // MoMo đã xong → chỉ reset, không gọi API thêm
  if (duLieuHoaDon.phuongThuc === 'Ví MoMo') {
    datLaiTrang();
    return;
  }
  goiApiThanhToan(duLieuHoaDon._chiTietThanhToan);
};

const datLaiTrang = () => {
  cacSanPhamTrongGio.value = [];
  isMoMoComplete.value = false;
  maDonHang.value = 'POS-' + Math.floor(Math.random() * 10000).toString().padStart(4, '0');
  const el = document.getElementById('modalHoaDon');
  if (el) Modal.getInstance(el)?.hide();
  const backdrop = document.querySelector('.modal-backdrop');
  if (backdrop) {
    backdrop.remove();
    document.body.classList.remove('modal-open');
    document.body.style.overflow = '';
    document.body.style.paddingRight = '';
  }
};

// ─── CALLBACK KHI MOMO REDIRECT VỀ /ban-hang?orderId=...&status=... ──────────
onMounted(() => {
  const { orderId, status } = route.query;

  // Kiểm tra nếu có tham số trả về từ MoMo
  if (orderId && status) {
    if (status === 'success') {
      // 1. Lấy dữ liệu hóa đơn đã lưu tạm trước khi đi thanh toán
      const saved = sessionStorage.getItem('momoInvoice');
      if (saved) {
        try {
          const invoice = JSON.parse(saved);

          // Đổ dữ liệu vào object reactive dùng cho Modal hóa đơn
          duLieuHoaDon.maHd = invoice.maHd;
          duLieuHoaDon.thoiGian = invoice.thoiGian;
          duLieuHoaDon.cartItems = invoice.cartItems;
          duLieuHoaDon.tongTienHang = invoice.tongTienHang;
          duLieuHoaDon.giamGia = invoice.giamGia;
          duLieuHoaDon.canTra = invoice.canTra;
          duLieuHoaDon.phuongThuc = 'Ví MoMo';

          // ✅ QUAN TRỌNG: Bật cờ này để Modal biết đây là đơn đã thanh toán xong
          isMoMoComplete.value = true;

          // Xóa dữ liệu tạm trong session
          sessionStorage.removeItem('momoInvoice');
        } catch (e) {
          console.error("Lỗi xử lý dữ liệu hóa đơn:", e);
        }
      }

      // 2. HIỆN THẲNG MODAL HÓA ĐƠN
      setTimeout(() => {
        const modalEl = document.getElementById('modalHoaDon');
        if (modalEl) {
          const modalInstance = Modal.getOrCreateInstance(modalEl);
          modalInstance.show();
        }
      }, 300);

      // 3. Xóa các tham số trên URL để thanh địa chỉ sạch sẽ (tránh refresh hiện lại)
      router.replace({ query: {} });

    } else {
      // Nếu thất bại thì xóa session và báo lỗi
      sessionStorage.removeItem('momoInvoice');
      router.replace({ query: {} });
      Swal.fire({
        icon: 'error',
        title: 'Thanh toán MoMo thất bại',
        text: 'Giao dịch bị hủy hoặc không thành công.'
      });
    }
  }
});
</script>

<style scoped>
@import '../../../assets/css_admin/pos.css';

.pos-root {
  padding-top: 1rem;
  padding-bottom: 2rem;
  background-color: #f8f9fc;
  min-height: 100vh;
}
</style>
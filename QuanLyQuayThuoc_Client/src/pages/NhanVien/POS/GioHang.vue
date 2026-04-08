<template>
  <div class="pos-left">
    <div class="pos-card mb-3">
      <div class="pos-card__header d-flex justify-content-between align-items-center">
        <div>
          <h5 class="pos-card__title mb-0">Giỏ hàng đang bán</h5>
          <div class="small text-muted">
            <span class="pos-cart-count">{{ props.cartItems.length }}</span> món
          </div>
        </div>
        <button class="btn btn-primary btn-sm shadow-sm" @click="moModalQuet">
          <i class="fas fa-barcode mr-1"></i> Quét mã vạch
        </button>
      </div>

      <div class="p-3">
        <div class="table-responsive">
          <table class="table table-bordered mb-0 pos-cart-table">
            <thead class="thead-light">
              <tr>
                <th style="min-width: 200px;">Tên thuốc</th>
                <th style="min-width: 130px;">Đơn vị</th>
                <th style="min-width: 150px;">Số lượng</th>
                <th style="min-width: 220px;">Lô hàng (FEFO)</th>
                <th style="min-width: 120px;">Thành tiền</th>
                <th style="min-width: 50px;">Xóa</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(sanPham, viTri) in props.cartItems" :key="viTri">
                <td>
                  <div class="font-weight-bold text-primary">{{ sanPham.tenThuoc }}</div>
                  <small class="text-muted">Giá: {{ dinhDangTien(sanPham.giaBan) }}</small>
                </td>
                <td>
                  <select class="form-control form-control-sm" v-model="sanPham.maDvtSelected"
                    @change="capNhatGiaTheoDonVi(sanPham)">
                    <option v-for="donVi in sanPham.danhSachDonVi" :key="donVi.maDvt" :value="donVi.maDvt">
                      {{ donVi.tenDonVi }} - {{ dinhDangTien(donVi.giaBan) }}
                    </option>
                  </select>
                </td>
                <td>
                  <div class="input-group input-group-sm">
                    <div class="input-group-prepend">
                      <button class="btn btn-outline-secondary" @click="capNhatSoLuong(viTri, -1)">-</button>
                    </div>
                    <input type="number" class="form-control text-center" v-model.number="sanPham.soLuong" min="1">
                    <div class="input-group-append">
                      <button class="btn btn-outline-secondary" @click="capNhatSoLuong(viTri, 1)">+</button>
                    </div>
                  </div>
                </td>
                <td>
                  <select class="form-control form-control-sm" v-model="sanPham.loHangSelected">
                    <option v-for="lo in sanPham.danhSachLo" :key="lo.maLo" :value="lo.maLo">
                      Lô: {{ lo.maLo }} - HSD: {{ dinhDangNgay(lo.hanSuDung) }} (Tồn: {{ lo.soLuongTon }})
                    </option>
                  </select>
                  <div v-if="!sanPham.danhSachLo || sanPham.danhSachLo.length === 0" class="small text-danger">
                    Hết hàng!
                  </div>
                </td>
                <td class="text-right font-weight-bold">
                  {{ dinhDangTien(sanPham.giaBan * sanPham.soLuong) }}
                </td>
                <td class="text-center">
                  <button class="btn btn-sm btn-outline-danger" @click="xoaSanPham(viTri)">
                    <i class="fas fa-trash"></i>
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <div v-if="props.cartItems.length === 0" class="text-center text-muted py-4">
          Chưa có sản phẩm. Hãy tìm hoặc quét mã vạch để thêm.
        </div>
      </div>
    </div>

    <!-- Modal Quét Mã Vạch -->
    <div class="modal fade" :class="{ 'show d-block': hienModalScanner }" tabindex="-1" role="dialog"
      style="background: rgba(0,0,0,0.5)">
      <div class="modal-dialog modal-dialog-centered" role="document">
        <div class="modal-content border-0 shadow">
          <div class="modal-header bg-primary text-white">
            <h5 class="modal-title"><i class="fas fa-camera mr-2"></i>Quét mã vạch sản phẩm</h5>
            <button type="button" class="close text-white" @click="dongModalQuet">
              <span>&times;</span>
            </button>
          </div>

          <!-- Camera + overlay loading -->
          <div class="modal-body p-0 bg-dark text-center" style="min-height: 300px; position: relative;">
            <!-- Vùng hiển thị camera -->
            <div id="reader" style="width: 100%;"></div>

            <!-- Spinner khi camera đang khởi động -->
            <div v-if="dangTaiCamera" class="py-5 text-white">
              <div class="spinner-border spinner-border-sm mr-2"></div> Đang khởi động Camera...
            </div>

            <!-- Overlay khi đang gọi API tìm thuốc -->
            <div v-if="dangTimThuoc" style="position: absolute; inset: 0; background: rgba(0,0,0,0.65);
                     display: flex; flex-direction: column; align-items: center;
                     justify-content: center; z-index: 10;">
              <div class="spinner-border text-light mb-2" style="width: 2.5rem; height: 2.5rem;"></div>
              <div class="text-white font-weight-bold">Đang tìm thuốc...</div>
              <div class="text-white-50 small mt-1">Mã: {{ scannedResult }}</div>
            </div>
          </div>

          <div class="modal-footer bg-light">
            <div class="mr-auto small font-italic text-muted" v-if="scannedResult && !dangTimThuoc">
              Mã vừa quét: <strong class="text-primary">{{ scannedResult }}</strong>
            </div>
            <button type="button" class="btn btn-secondary" @click="dongModalQuet">Đóng</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onBeforeUnmount, nextTick } from 'vue';
import { Html5QrcodeScanner } from 'html5-qrcode';
import axios from 'axios';
import Swal from 'sweetalert2';

const props = defineProps({
  cartItems: { type: Array, default: () => [] }
});

const emit = defineEmits(['remove-item', 'update-quantity', 'update-unit', 'add-to-cart']);

// --- Khai báo biến ---
const scannedResult = ref('');
const hienModalScanner = ref(false);
const dangTaiCamera = ref(false);
const dangTimThuoc = ref(false);
let scannerInstance = null;

// --- Mở modal và khởi động camera ---
const moModalQuet = async () => {
  hienModalScanner.value = true;
  dangTaiCamera.value = true;
  scannedResult.value = '';

  // Đợi render xong mới khởi tạo scanner
  await nextTick();

  scannerInstance = new Html5QrcodeScanner(
    'reader',
    {
      fps: 10,
      qrbox: { width: 250, height: 150 },
      aspectRatio: 1.0
    },
    false
  );

  scannerInstance.render(onScanSuccess, onScanFailure);
  dangTaiCamera.value = false;
};

// --- Xử lý khi quét thành công: gọi API → thêm giỏ hàng → đóng modal ---
const onScanSuccess = async (decodedText) => {

  if (dangTimThuoc.value) return;

  scannedResult.value = decodedText;
  dangTimThuoc.value = true;

  if (navigator.vibrate) navigator.vibrate(100);

  try {
    const token = localStorage.getItem('token');
    const response = await axios.get(
      `https://localhost:7070/api/BanHang/tim-thuoc-barcode/${decodedText}`,
      { headers: { Authorization: `Bearer ${token}` } }
    );

    const thuocMoi = response.data;

    if (!thuocMoi || !thuocMoi.maThuoc) {
      Swal.fire({
        icon: 'info',
        title: 'Không tìm thấy',
        text: `Không có thuốc nào với mã vạch: ${decodedText}`,
        timer: 2000,
        showConfirmButton: false
      });
      dangTimThuoc.value = false;
      return;
    }

    emit('add-to-cart', {
      ...thuocMoi,
      soLuong: 1,
      maDvtSelected: thuocMoi.maDvtMacDinh || thuocMoi.danhSachDonVi?.[0]?.maDvt,
      loHangSelected: thuocMoi.danhSachLo?.[0]?.maLo || null,
      giaBan: thuocMoi.giaBanMacDinh || thuocMoi.giaBan
    });

    Swal.fire({
      icon: 'success',
      title: 'Đã thêm vào giỏ!',
      text: thuocMoi.tenThuoc,
      toast: true,
      position: 'top-end',
      timer: 1500,
      showConfirmButton: false,
      timerProgressBar: true
    });

    dongModalQuet();

  } catch (error) {
    console.error('Lỗi quét mã:', error);
    Swal.fire({
      icon: 'error',
      title: 'Lỗi kết nối',
      text: 'Không thể tìm thuốc, vui lòng thử lại!',
      timer: 2000,
      showConfirmButton: false
    });
  } finally {
    dangTimThuoc.value = false;
  }
};

const onScanFailure = (_error) => {
};

// --- Đóng modal và dọn dẹp camera ---
const dongModalQuet = () => {
  if (scannerInstance) {
    scannerInstance.clear().catch(err => console.error('Lỗi đóng scanner:', err));
    scannerInstance = null;
  }
  hienModalScanner.value = false;
  dangTimThuoc.value = false;
};

// --- Định dạng ---
const dinhDangTien = (giaTri) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(giaTri || 0);

const dinhDangNgay = (chuoiNgay) => {
  if (!chuoiNgay) return 'N/A';
  return new Date(chuoiNgay).toLocaleDateString('vi-VN');
};

// --- Hành động giỏ hàng ---
const xoaSanPham = (viTri) => emit('remove-item', viTri);
const capNhatSoLuong = (viTri, thayDoi) => emit('update-quantity', { index: viTri, change: thayDoi });

const capNhatGiaTheoDonVi = (sanPham) => {
  const donVi = sanPham.danhSachDonVi.find(d => d.maDvt === sanPham.maDvtSelected);
  if (donVi) sanPham.giaBan = donVi.giaBan;
};

// Dọn dẹp camera khi chuyển trang
onBeforeUnmount(() => {
  if (scannerInstance) scannerInstance.clear();
});
</script>

<style scoped>
input::-webkit-outer-spin-button,
input::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

input[type=number] {
  -moz-appearance: textfield;
}

.modal.show {
  display: block;
  padding-right: 17px;
}

#reader {
  border: none !important;
}

#reader__dashboard_section_csr button {
  padding: 5px 10px;
  border-radius: 4px;
  border: 1px solid #4e73df;
  background: #4e73df;
  color: white;
  font-size: 13px;
}

#reader video {
  object-fit: cover;
}
</style>
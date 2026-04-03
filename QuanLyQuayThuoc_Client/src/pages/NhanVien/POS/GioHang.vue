<template>
  <div class="pos-left">
    <div class="pos-card mb-3">
      <div class="pos-card__header">
        <h5 class="pos-card__title mb-0">Giỏ hàng đang bán</h5>
        <div class="small text-muted">
          <span class="pos-cart-count">{{ props.cartItems.length }}</span> món
        </div>
      </div>
      <div class="p-3">
        <!-- <div class="scanner-wrapper mb-3 text-center">
           <div id="reader" style="width: 100%; max-width: 400px; margin: auto; border: 1px solid #ddd; border-radius: 8px; overflow: hidden;"></div>
           <p v-if="scannedResult" class="mt-2 badge badge-success">Mã vừa quét: {{ scannedResult }}</p>
        </div> -->

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
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue';
import { Html5QrcodeScanner } from "html5-qrcode";

const props = defineProps({
  cartItems: { type: Array, default: () => [] }
});

const emit = defineEmits(['remove-item', 'update-quantity', 'update-unit']);

// --- Khai báo biến ---
const scannedResult = ref("");
let html5QrcodeScanner = null;

// --- Định dạng ---
const dinhDangTien = (giaTri) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(giaTri || 0);
};

const dinhDangNgay = (chuoiNgay) => {
  if (!chuoiNgay) return 'N/A';
  const ngay = new Date(chuoiNgay);
  return ngay.toLocaleDateString('vi-VN');
};

// --- Hành động ---
const xoaSanPham = (viTri) => {
  emit('remove-item', viTri);
};

const capNhatSoLuong = (viTri, thayDoi) => {
  emit('update-quantity', { index: viTri, change: thayDoi });
};

const capNhatGiaTheoDonVi = (sanPham) => {
  const donVi = sanPham.danhSachDonVi.find(d => d.maDvt === sanPham.maDvtSelected);
  if (donVi) {
    sanPham.giaBan = donVi.giaBan;
  }
};

// --- Xử lý Quét Mã Vạch ---
/* const onScanSuccess = async (decodedText) => {
  scannedResult.value = decodedText;
  console.log("Mã vạch quét được:", decodedText);
  
  try {
    // Gọi API Webhook để xử lý mã vạch (Localhost)
    const response = await fetch(`https://localhost:7070/api/Webhook/scan?maVach=${decodedText}`, {
      method: 'POST'
    });
    
    if (response.ok) {
      console.log("Đã gửi mã vạch lên Backend");
    }
  } catch (error) {
    console.error("Lỗi khi gửi mã vạch:", error);
  }
};

  onMounted(() => {
  // Khởi tạo máy quét
  html5QrcodeScanner = new Html5QrcodeScanner("reader", {
    fps: 10,
    qrbox: { width: 250, height: 150 }, // Khung chữ nhật cho mã vạch
    aspectRatio: 1.777778 // 16:9
  });
  
  html5QrcodeScanner.render(onScanSuccess);
});

onBeforeUnmount(() => {
  if (html5QrcodeScanner) {
    html5QrcodeScanner.clear().catch(error => {
      console.error("Failed to clear html5QrcodeScanner. ", error);
    });
  }
}); */
</script>

<style scoped>
input::-webkit-outer-spin-button,
input::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}
.scanner-wrapper {
  background: #f8f9fa;
  padding: 10px;
  border-radius: 8px;
}
</style>
<template>
  <!-- Modal Thêm nhanh -->
  <div class="modal fade" id="modalThemNhanh" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-lg modal-dialog-scrollable">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Thêm nhanh (không có mã vạch)</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
        </div>
        <div class="modal-body">
          <div class="row">
            <div class="col-md-6 mb-3">
              <label for="posNhanhTen" class="small text-muted">Tên thuốc / món</label>
              <input v-model="themNhanh.tenThuoc" id="posNhanhTen" class="form-control" placeholder="Ví dụ: Khẩu trang lẻ" />
            </div>
            <div class="col-md-3 mb-3">
              <label for="posNhanhDvt" class="small text-muted">Đơn vị tính</label>
              <select v-model="themNhanh.donVi" id="posNhanhDvt" class="form-control">
                <option value="Lẻ">Lẻ</option>
                <option value="Gói">Gói</option>
                <option value="Hộp">Hộp</option>
                <option value="Vỉ">Vỉ</option>
                <option value="Chai">Chai</option>
              </select>
            </div>
            <div class="col-md-3 mb-3">
              <label for="posNhanhGia" class="small text-muted">Giá bán (đ)</label>
              <input v-model.number="themNhanh.giaBan" id="posNhanhGia" type="number" class="form-control" min="0" />
            </div>
          </div>
          <div class="row">
            <div class="col-md-4 mb-2">
              <label class="small text-muted" for="posNhanhSoLuong">Số lượng</label>
              <input v-model.number="themNhanh.soLuong" id="posNhanhSoLuong" type="number" class="form-control" min="1">
            </div>
            <div class="col-md-8 mb-2">
              <label class="small text-muted" for="posNhanhGhiChu">Ghi chú lô</label>
              <input v-model="themNhanh.ghiChu" id="posNhanhGhiChu" class="form-control" placeholder="Ví dụ: — (không áp dụng)">
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Hủy</button>
          <button type="button" class="btn btn-primary" @click="xuLyThemNhanh">
            <i class="fas fa-plus mr-1"></i> Thêm vào giỏ
          </button>
        </div>
      </div>
    </div>
  </div>

  <!-- Modal Thêm khách -->
  <div class="modal fade" id="modalThemKhach" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Thêm khách / nhập SĐT</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
        </div>
        <div class="modal-body">
          <label class="small text-muted" for="posKhachSdt">SĐT Khách hàng</label>
          <input v-model="sdtKhachHang" id="posKhachSdt" class="form-control" placeholder="0xxx xxx xxx">
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Hủy</button>
          <button type="button" class="btn btn-primary" @click="luuKhachHang" data-bs-dismiss="modal">
            <i class="fas fa-save mr-1"></i> Lưu
          </button>
        </div>
      </div>
    </div>
  </div>

  <!-- Modal Hóa đơn -->
  <div class="modal fade" id="modalHoaDon" tabindex="-1" aria-hidden="true" data-bs-backdrop="static">
    <div class="modal-dialog modal-lg modal-dialog-scrollable">
      <div class="modal-content">
        <div class="modal-header">
          <!-- ✅ Tiêu đề đổi theo phương thức thanh toán -->
          <h5 class="modal-title d-flex align-items-center">
            <span v-if="isMoMo" class="badge badge-success mr-2" style="font-size: 0.8rem;">
              ✓ Đã thanh toán MoMo
            </span>
            Hóa đơn thanh toán
          </h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"
            @click="$emit('finish-payment')"></button>
        </div>

        <div class="modal-body">
          <!-- ✅ Banner xanh nếu là MoMo đã thanh toán xong -->
          <div v-if="isMoMo" class="alert alert-success d-flex align-items-center mb-3 py-2">
            <i class="fas fa-check-circle mr-2"></i>
            <span>Khách đã thanh toán thành công qua <strong>Ví MoMo</strong>. Bấm <strong>In hóa đơn</strong> để in và hoàn tất.</span>
          </div>

          <div id="posHoaDonPrint" class="border rounded p-4 bg-white shadow-sm"
            style="max-width: 650px; margin: 0 auto; color: #333; font-family: 'Courier New', Courier, monospace;">
            <div class="text-center mb-4">
              <div class="font-weight-bold h5 mb-0">NHÀ THUỐC BÁ ĐẠO</div>
              <div class="small">Đ/C: 123 Lê Lợi, TP. Buôn Ma Thuột</div>
              <div class="small">Hotline: 09xx xxx xxx</div>
              <div class="mt-2 text-uppercase font-weight-bold">Hóa Đơn Bán Lẻ</div>
              <div class="small mt-1">Mã HD: <strong>{{ invoiceData.maHd }}</strong></div>
              <div class="small">Ngày: <strong>{{ invoiceData.thoiGian }}</strong></div>
            </div>

            <div class="mb-3 small">
              <div>Khách hàng: <strong>{{ invoiceData.khachHang || 'Khách vãng lai' }}</strong></div>
              <div>Hình thức: <strong>{{ invoiceData.phuongThuc }}</strong></div>
            </div>

            <div class="table-responsive">
              <table class="table table-sm border-bottom mb-0">
                <thead style="border-top: 1px dashed #000; border-bottom: 1px dashed #000;">
                  <tr>
                    <th>Tên thuốc</th>
                    <th>ĐVT</th>
                    <th class="text-right">SL</th>
                    <th class="text-right">T.Tiền</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(sanPham, viTri) in invoiceData.cartItems" :key="viTri">
                    <td>{{ sanPham.tenThuoc }}</td>
                    <td>{{ sanPham.danhSachDonVi?.find(d => d.maDvt === sanPham.maDvtSelected)?.tenDonVi || 'Lẻ' }}</td>
                    <td class="text-right">{{ sanPham.soLuong }}</td>
                    <td class="text-right">{{ dinhDangTien(sanPham.giaBan * sanPham.soLuong) }}</td>
                  </tr>
                </tbody>
              </table>
            </div>

            <div class="mt-3 border-top pt-2">
              <div class="d-flex justify-content-between">
                <span>Tổng tiền hàng:</span>
                <span>{{ dinhDangTien(invoiceData.tongTienHang) }}</span>
              </div>
              <div class="d-flex justify-content-between text-danger">
                <span>Chiết khấu:</span>
                <span>-{{ dinhDangTien(invoiceData.giamGia) }}</span>
              </div>
              <div class="d-flex justify-content-between h6 font-weight-bold mt-1">
                <span>Tổng tiền:</span>
                <span class="text-primary">{{ dinhDangTien(invoiceData.canTra) }}</span>
              </div>
            </div>

            <div class="text-center mt-4 small italic">
              Cảm ơn quý khách. Hẹn gặp lại!
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <!-- ✅ Nút Đóng: khi là MoMo thì emit finish-payment luôn để reset trang -->
          <button
            type="button"
            class="btn btn-outline-secondary"
            @click="isMoMo ? $emit('finish-payment') : null"
            :data-bs-dismiss="isMoMo ? undefined : 'modal'"
          >
            Đóng
          </button>

          <button type="button" class="btn btn-success" @click="inHoaDon">
            <i class="fas fa-print mr-1"></i> In hóa đơn
          </button>

          <!-- ✅ Ẩn nút "Thanh toán xong" khi là MoMo (đã TT rồi, không cần gọi API nữa) -->
          <button
            v-if="!isMoMo"
            type="button"
            class="btn btn-primary"
            @click="xacNhanThanhToan"
          >
            <i class="fas fa-check mr-1"></i> Thanh toán xong
          </button>

          <!-- ✅ Thay bằng nút "Hoàn tất" khi MoMo -->
          <button
            v-else
            type="button"
            class="btn btn-primary"
            @click="$emit('finish-payment')"
          >
            <i class="fas fa-check-double mr-1"></i> Hoàn tất & Đơn mới
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue';

const props = defineProps({
  invoiceData: {
    type: Object,
    default: () => ({
      maHd: 'POS-0001',
      thoiGian: '',
      khachHang: '',
      cartItems: [],
      tongTienHang: 0,
      giamGia: 0,
      canTra: 0,
      phuongThuc: 'Tiền mặt'
    })
  },
  // ✅ Prop mới: true khi là thanh toán MoMo (đã hoàn tất), ẩn nút "Thanh toán xong"
  isMoMo: {
    type: Boolean,
    default: false
  }
});

const emit = defineEmits(['add-quick-item', 'save-customer', 'finish-payment']);

const themNhanh = reactive({
  tenThuoc: '', donVi: 'Lẻ', giaBan: 0, soLuong: 1, ghiChu: '—'
});

const sdtKhachHang = ref('');

const xuLyThemNhanh = () => {
  if (!themNhanh.tenThuoc) { alert('Vui lòng nhập tên món đồ'); return; }
  emit('add-quick-item', {
    maThuoc: 0, tenThuoc: themNhanh.tenThuoc, tenDonVi: themNhanh.donVi,
    giaBan: themNhanh.giaBan, soLuong: themNhanh.soLuong,
    maDvtSelected: 1, loHangSelected: 0, hamLuong: 'Hàng thêm nhanh', soLuongTon: 999
  });
  themNhanh.tenThuoc = ''; themNhanh.giaBan = 0; themNhanh.soLuong = 1;
  const el = document.getElementById('modalThemNhanh');
  if (window.bootstrap) window.bootstrap.Modal.getInstance(el)?.hide();
};

const luuKhachHang = () => {
  emit('save-customer', sdtKhachHang.value);
  sdtKhachHang.value = '';
};

const xacNhanThanhToan = () => { emit('finish-payment'); };

const inHoaDon = () => {
  const noiDungIn  = document.getElementById('posHoaDonPrint').innerHTML;
  const noiDungGoc = document.body.innerHTML;
  document.body.innerHTML = noiDungIn;
  window.print();
  document.body.innerHTML = noiDungGoc;
  window.location.reload();
};

const dinhDangTien = (v) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v || 0);
</script>

<style scoped>
@media print {
  body * { visibility: hidden; }
  #posHoaDonPrint, #posHoaDonPrint * { visibility: visible; }
  #posHoaDonPrint { position: absolute; left: 0; top: 0; width: 100%; border: none !important; }
}
.italic { font-style: italic; }
</style>
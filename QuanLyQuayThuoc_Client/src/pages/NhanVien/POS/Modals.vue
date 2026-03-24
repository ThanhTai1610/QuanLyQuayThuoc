<template>
  <div class="modal fade" id="modalThemNhanh" tabindex="-1" role="dialog" aria-hidden="true">
    <div class="modal-dialog modal-lg modal-dialog-scrollable" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Thêm nhanh (không có mã vạch)</h5>
          <button type="button" class="close" data-dismiss="modal" aria-label="Đóng">
            <span aria-hidden="true">×</span>
          </button>
        </div>
        <div class="modal-body">
          <div class="row">
            <div class="col-md-6 mb-3">
              <label for="posNhanhTen" class="small text-muted">Tên thuốc / món</label>
              <input v-model="quickAdd.tenThuoc" id="posNhanhTen" class="form-control" placeholder="Ví dụ: Khẩu trang lẻ" />
            </div>
            <div class="col-md-3 mb-3">
              <label for="posNhanhDvt" class="small text-muted">Đơn vị tính</label>
              <select v-model="quickAdd.donVi" id="posNhanhDvt" class="form-control">
                <option value="Lẻ">Lẻ</option>
                <option value="Gói">Gói</option>
                <option value="Hộp">Hộp</option>
                <option value="Vỉ">Vỉ</option>
                <option value="Chai">Chai</option>
              </select>
            </div>
            <div class="col-md-3 mb-3">
              <label for="posNhanhGia" class="small text-muted">Giá bán (đ)</label>
              <input v-model.number="quickAdd.giaBan" id="posNhanhGia" type="number" class="form-control" min="0" />
            </div>
          </div>
          <div class="row">
            <div class="col-md-4 mb-2">
              <label class="small text-muted" for="posNhanhSoLuong">Số lượng</label>
              <input v-model.number="quickAdd.soLuong" id="posNhanhSoLuong" type="number" class="form-control" min="1">
            </div>
            <div class="col-md-8 mb-2">
              <label class="small text-muted" for="posNhanhGhiChu">Ghi chú lô (demo)</label>
              <input v-model="quickAdd.ghiChu" id="posNhanhGhiChu" class="form-control" placeholder="Ví dụ: — (không áp dụng)">
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-secondary" data-dismiss="modal">Hủy</button>
          <button type="button" class="btn btn-primary" @click="handleQuickAdd">
            <i class="fas fa-plus mr-1"></i> Thêm vào giỏ
          </button>
        </div>
      </div>
    </div>
  </div>

  <div class="modal fade" id="modalThemKhach" tabindex="-1" role="dialog" aria-hidden="true">
    <div class="modal-dialog" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Thêm khách / nhập SĐT</h5>
          <button type="button" class="close" data-dismiss="modal" aria-label="Đóng">
            <span aria-hidden="true">×</span>
          </button>
        </div>
        <div class="modal-body">
          <label class="small text-muted" for="posKhachSdt">SĐT</label>
          <input v-model="customerSdt" id="posKhachSdt" class="form-control" placeholder="0xxx xxx xxx">
          <div class="small text-muted mt-2">
            Demo: không gọi API, chỉ dùng cho hiển thị trên hóa đơn.
          </div>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-secondary" data-dismiss="modal">Hủy</button>
          <button type="button" class="btn btn-primary" @click="saveCustomer">
            <i class="fas fa-save mr-1"></i> Lưu
          </button>
        </div>
      </div>
    </div>
  </div>

  <div class="modal fade" id="modalHoaDon" tabindex="-1" role="dialog" aria-hidden="true" data-backdrop="static">
    <div class="modal-dialog modal-lg modal-dialog-scrollable" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Hóa đơn thanh toán (demo)</h5>
          <button type="button" class="close" data-dismiss="modal" aria-label="Đóng">
            <span aria-hidden="true">×</span>
          </button>
        </div>
        <div class="modal-body">
          <div id="posHoaDonPrint" class="border rounded p-3" style="max-width: 720px; margin: 0 auto;">
            <div class="text-center mb-2">
              <div class="font-weight-bold" style="font-size: 14px;">NHÀ THUỐC BÁ ĐẠO</div>
              <div class="text-muted small">Địa chỉ: 123 Lê Lợi • Hotline: 1900 xxxx</div>
              <div class="small text-muted mt-1">Mã hóa đơn: <strong>{{ invoiceData.maHd }}</strong></div>
              <div class="small text-muted">Thời gian: <strong>{{ invoiceData.thoiGian }}</strong></div>
            </div>

            <div class="mb-2">
              <div class="small text-muted">Khách: <strong>{{ invoiceData.khachHang || 'Khách vãng lai' }}</strong></div>
            </div>

            <div class="table-responsive">
              <table class="table table-sm table-bordered mb-0" style="background:#fff;">
                <thead class="thead-light">
                  <tr>
                    <th>Tên</th>
                    <th>ĐVT</th>
                    <th>Lô</th>
                    <th class="text-right">SL</th>
                    <th class="text-right">Thành tiền</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(item, index) in invoiceData.cartItems" :key="index">
                    <td>{{ item.tenThuoc }}</td>
                    <td>{{ item.donVi }}</td>
                    <td>{{ item.loHangSelected || '—' }}</td>
                    <td class="text-right">{{ item.soLuong }}</td>
                    <td class="text-right">{{ formatMoney(item.giaBan * item.soLuong) }}</td>
                  </tr>
                </tbody>
              </table>
            </div>

            <div class="mt-3">
              <div class="d-flex justify-content-between">
                <span class="text-muted small">Tổng tiền hàng</span>
                <strong>{{ formatMoney(invoiceData.tongTienHang) }}</strong>
              </div>
              <div class="d-flex justify-content-between">
                <span class="text-muted small">Giảm giá</span>
                <strong>{{ formatMoney(invoiceData.giamGia) }}</strong>
              </div>
              <hr>
              <div class="d-flex justify-content-between">
                <span class="font-weight-bold">Khách cần trả</span>
                <strong style="color:#4e73df;">{{ formatMoney(invoiceData.canTra) }}</strong>
              </div>
              <div class="d-flex justify-content-between">
                <span class="text-muted small">PT thanh toán</span>
                <strong>{{ invoiceData.phuongThuc }}</strong>
              </div>
            </div>
            <div class="text-center small text-muted mt-3">
              Cảm ơn quý khách!
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-outline-secondary" data-dismiss="modal">Đóng</button>
          <button type="button" class="btn btn-success" @click="printInvoice">
            <i class="fas fa-print mr-1"></i> In (demo)
          </button>
          <button type="button" class="btn btn-primary" @click="confirmPayment">
            <i class="fas fa-check mr-1"></i> Thanh toán xong
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue';

// 1. Dữ liệu cho Thêm nhanh
const quickAdd = reactive({
  tenThuoc: '',
  donVi: 'Lẻ',
  giaBan: 0,
  soLuong: 1,
  ghiChu: '—'
});

// 2. Dữ liệu cho Thêm khách
const customerSdt = ref('');

// 3. Dữ liệu cho Hóa đơn (Nhận từ Props trang cha)
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
  }
});

const emit = defineEmits(['add-quick-item', 'save-customer', 'finish-payment']);

// --- Các hàm xử lý ---

const handleQuickAdd = () => {
  // Gửi dữ liệu clone để tránh reference
  emit('add-quick-item', { ...quickAdd });
  // Reset form
  quickAdd.tenThuoc = '';
  quickAdd.giaBan = 0;
  quickAdd.soLuong = 1;
};

const saveCustomer = () => {
  emit('save-customer', customerSdt.value);
};

const confirmPayment = () => {
  emit('finish-payment');
};

const printInvoice = () => {
  window.print();
};

const formatMoney = (val) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val);
};
</script>
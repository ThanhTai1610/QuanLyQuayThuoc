<template>
  <div class="container-fluid">

    <!-- Header -->
    <div class="d-sm-flex align-items-center justify-content-between mb-4">
      <div>
        <h1 class="h3 mb-0 text-gray-800">Kiểm kê &amp; Đối soát tồn kho</h1>
        <p class="mb-0 text-muted small">Tạo phiếu kiểm kê theo lô, tính chênh lệch và lưu lịch sử.</p>
      </div>
    </div>

    <!-- A. Tạo phiếu kiểm kê -->
    <div class="card shadow mb-4">
      <div class="card-header py-3">
        <h6 class="m-0 font-weight-bold text-primary">A. Tạo phiếu kiểm kê</h6>
      </div>
      <div class="card-body">

        <!-- Thông tin phiếu -->
        <div class="row">
          <div class="col-md-4 mb-3">
            <label>Người thực hiện</label>
            <input type="text" class="form-control" v-model="phieu.nguoiThucHien" />
          </div>
          <div class="col-md-3 mb-3">
            <label>Ngày kiểm kê</label>
            <input type="date" class="form-control" v-model="phieu.ngay" />
          </div>
          <div class="col-md-5 mb-3">
            <label>&nbsp;</label>
            <div class="alert alert-warning mb-0">
              <i class="fas fa-info-circle mr-1"></i>
              Nhập số lượng thực tế — hệ thống tự tính <strong>Chênh lệch</strong>
              và yêu cầu <strong>Lý do</strong> khi khác 0.
            </div>
          </div>
        </div>

        <!-- Bộ lọc -->
        <div class="row kiemke-toolbar">
          <div class="col-md-4">
            <div class="form-group">
              <label>Lọc theo danh mục</label>
              <select class="form-control form-control-sm" v-model="locDanhMuc">
                <option value="">— Tất cả danh mục —</option>
                <option>Thuốc kháng sinh</option>
                <option>Thuốc giảm đau</option>
                <option>Hỗ trợ tiêu hóa</option>
              </select>
            </div>
          </div>
          <div class="col-md-4">
            <div class="form-group">
              <label>Lọc theo vị trí</label>
              <select class="form-control form-control-sm" v-model="locViTri">
                <option value="">— Tất cả vị trí —</option>
                <option>Tủ A1</option>
                <option>Tủ A2</option>
                <option>Kho lạnh B</option>
              </select>
            </div>
          </div>
          <div class="col-md-4">
            <div class="form-group">
              <label>&nbsp;</label>
              <button type="button" class="btn btn-outline-primary btn-sm btn-block" @click="apDungLocFilter">
                <i class="fas fa-filter mr-1"></i> Áp dụng bộ lọc
              </button>
            </div>
          </div>
        </div>

        <!-- Bảng kiểm kê -->
        <div class="table-responsive">
          <table class="table table-bordered table-hover mb-0 kiemke-table">
            <thead class="thead-light">
              <tr>
                <th style="min-width:260px;">Tên thuốc &amp; Số lô</th>
                <th style="min-width:150px;">Hạn sử dụng</th>
                <th style="min-width:160px;">Số lượng hệ thống</th>
                <th style="min-width:170px;">Số lượng thực tế</th>
                <th style="min-width:150px;">Chênh lệch</th>
                <th style="min-width:250px;">Lý do biến động</th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="danhSachHienThi.length === 0">
                <td colspan="6" class="text-center text-muted py-4">Không có dữ liệu phù hợp bộ lọc.</td>
              </tr>
              <tr v-for="item in danhSachHienThi" :key="item.id">
                <td>
                  <div class="font-weight-bold">{{ item.tenThuoc }}</div>
                  <div class="small text-muted">
                    <span class="kiemke-batch">{{ item.soLo }}</span>
                  </div>
                </td>
                <td class="kiemke-hsd">{{ item.hanSuDung }}</td>
                <td><strong>{{ item.soLuongTon }}</strong></td>
                <td>
                  <input type="number" min="0" step="1"
                    class="form-control form-control-sm"
                    v-model.number="item.soLuongThucTe"
                    @input="capNhatChenhLech(item)" />
                </td>
                <td>
                  <span class="kiemke-chenh-lech" :class="chenhClass(item.chenhLech)">
                    {{ item.chenhLech > 0 ? '+' : '' }}{{ item.chenhLech }}
                  </span>
                </td>
                <td>
                  <select class="form-control form-control-sm"
                    v-model="item.lyDo"
                    :disabled="item.chenhLech === 0"
                    :class="{ 'kiemke-reason-disabled': item.chenhLech === 0 }">
                    <option value="">— Chọn lý do —</option>
                    <option value="hong-vo">Hỏng / Vỡ</option>
                    <option value="het-han">Hết hạn</option>
                    <option value="nhap-sai">Nhập sai</option>
                    <option value="that-thoat">Thất thoát</option>
                    <option value="khac">Khác</option>
                  </select>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Tổng kết -->
        <div class="row mt-3">
          <div class="col-md-4 mb-2">
            <div class="small text-muted">Tổng chênh lệch số lượng</div>
            <div class="kiemke-summary-value" :class="chenhClass(tongChenhLechSL)">
              {{ tongChenhLechSL > 0 ? '+' : '' }}{{ tongChenhLechSL }}
            </div>
          </div>
          <div class="col-md-4 mb-2">
            <div class="small text-muted">Tổng giá trị chênh lệch (ước tính)</div>
            <div class="kiemke-summary-value" :class="chenhClass(tongChenhLechGT)">
              {{ formatGia(tongChenhLechGT) }}
            </div>
          </div>
          <div class="col-md-4 mb-2 d-flex align-items-end justify-content-end">
            <button type="button" class="btn btn-primary" @click="luuPhieu">
              <i class="fas fa-save mr-1"></i> Lưu phiếu kiểm kê
            </button>
          </div>
        </div>

        <!-- Toast -->
        <div class="kiemke-toast-wrap mt-3" aria-live="polite">
          <div v-for="(t, i) in toasts" :key="i" :class="['alert', 'shadow', 'mb-2', 'alert-' + t.type]">
            {{ t.message }}
          </div>
        </div>

      </div>
    </div>

    <!-- B. Lịch sử kiểm kê -->
    <div class="card shadow mb-4">
      <div class="card-header py-3">
        <h6 class="m-0 font-weight-bold text-primary">B. Lịch sử kiểm kê</h6>
      </div>
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-bordered table-sm mb-0">
            <thead class="thead-light">
              <tr>
                <th>Mã phiếu</th>
                <th>Thời gian</th>
                <th>Người thực hiện</th>
                <th>Tổng chênh lệch (SL)</th>
                <th>Tổng giá trị (ước tính)</th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="lichSu.length === 0">
                <td colspan="5" class="text-center text-muted py-4">Chưa có phiếu kiểm kê.</td>
              </tr>
              <tr v-for="h in lichSu" :key="h.ma">
                <td><strong>{{ h.ma }}</strong></td>
                <td class="text-nowrap">{{ h.thoiGian }}</td>
                <td>{{ h.nguoi }}</td>
                <td>
                  <span :class="chenhClass(h.tongSo)">
                    {{ h.tongSo > 0 ? '+' : '' }}{{ h.tongSo }}
                  </span>
                </td>
                <td>
                  <span :class="chenhClass(h.tongGia)">{{ formatGia(h.tongGia) }}</span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <p class="text-muted small mb-0 mt-2">
          <i class="fas fa-info-circle mr-1"></i>
          Bấm "Lưu phiếu" để thêm dòng mới vào danh sách.
        </p>
      </div>
    </div>

  </div>
</template>

<script setup>
import '../../assets/css_admin/kiem-ke.css';
import { ref, reactive, computed, onMounted } from 'vue';

// ── State ──
const locDanhMuc = ref('');
const locViTri   = ref('');
const toasts     = ref([]);
let soPhieuCounter = 4;

const phieu = reactive({
  nguoiThucHien: 'Nhân viên quầy',
  ngay:          new Date().toISOString().slice(0, 10),
  ghiChu:        '',
});

// Dữ liệu lô hàng — sau gắn API thay bằng axiosClient.get('/KiemKe/lo-hang')
// Ánh xạ với bảng LoHang: MaLo, SoLo, HanSuDung, SoLuongTon, GiaNhap + JOIN Thuoc.TenThuoc
const tatCaLo = ref([
  { id: 'it-1', tenThuoc: 'Amoxicillin 500mg',      soLo: 'LOT-2408-B', hanSuDung: '15/05/2026', danhMuc: 'Thuốc kháng sinh', viTri: 'Tủ A1',    soLuongTon: 120, donGia: 50000, soLuongThucTe: 120, chenhLech: 0, lyDo: '' },
  { id: 'it-2', tenThuoc: 'Smecta 3g',               soLo: 'LOT-SM-99',  hanSuDung: '18/07/2026', danhMuc: 'Thuốc kháng sinh', viTri: 'Tủ A2',    soLuongTon: 60,  donGia: 65000, soLuongThucTe: 60,  chenhLech: 0, lyDo: '' },
  { id: 'it-3', tenThuoc: 'Paracetamol 500mg',       soLo: 'LOT-2501-P', hanSuDung: '05/06/2026', danhMuc: 'Thuốc giảm đau',   viTri: 'Tủ A1',    soLuongTon: 90,  donGia: 35000, soLuongThucTe: 90,  chenhLech: 0, lyDo: '' },
  { id: 'it-4', tenThuoc: 'Vitamin C 1000mg',        soLo: 'LOT-VC-88',  hanSuDung: '30/04/2027', danhMuc: 'Thuốc giảm đau',   viTri: 'Kho lạnh B',soLuongTon: 35,  donGia: 45000, soLuongThucTe: 35,  chenhLech: 0, lyDo: '' },
  { id: 'it-5', tenThuoc: 'Enterogermina',            soLo: 'LOT-EN-55',  hanSuDung: '12/12/2026', danhMuc: 'Hỗ trợ tiêu hóa',  viTri: 'Tủ A2',    soLuongTon: 40,  donGia: 70000, soLuongThucTe: 40,  chenhLech: 0, lyDo: '' },
  { id: 'it-6', tenThuoc: 'Dung dịch sát khuẩn tay', soLo: 'LOT-SK-01',  hanSuDung: '10/10/2026', danhMuc: 'Hỗ trợ tiêu hóa',  viTri: 'Kho lạnh B',soLuongTon: 55,  donGia: 25000, soLuongThucTe: 55,  chenhLech: 0, lyDo: '' },
]);

// Lịch sử phiếu kiểm kê — sau gắn API thay bằng GET /KiemKe/lich-su (bảng PhieuKiemKe)
const lichSu = ref([
  { ma: 'KK-2026-0003', thoiGian: '20/03/2026 09:20', nguoi: 'Nhân viên quầy', tongSo: 0,  tongGia: 0       },
  { ma: 'KK-2026-0002', thoiGian: '18/03/2026 16:05', nguoi: 'Nhân viên quầy', tongSo: -6, tongGia: -180000 },
]);

// ── Lọc hiển thị ──
const danhSachHienThi = ref([...tatCaLo.value]);

const apDungLocFilter = () => {
  danhSachHienThi.value = tatCaLo.value.filter(item => {
    const okDM = !locDanhMuc.value || item.danhMuc === locDanhMuc.value;
    const okVT = !locViTri.value   || item.viTri   === locViTri.value;
    return okDM && okVT;
  });
};

// ── Tính chênh lệch khi nhập ──
const capNhatChenhLech = (item) => {
  item.chenhLech = item.soLuongThucTe - item.soLuongTon;
  if (item.chenhLech === 0) item.lyDo = '';
};

// ── Tổng kết computed ──
const tongChenhLechSL = computed(() =>
  danhSachHienThi.value.reduce((s, i) => s + i.chenhLech, 0)
);
const tongChenhLechGT = computed(() =>
  danhSachHienThi.value.reduce((s, i) => s + i.chenhLech * i.donGia, 0)
);

// ── Lưu phiếu — sau gắn API POST /KiemKe/luu-phieu (bảng PhieuKiemKe + ChiTietKiemKe) ──
const luuPhieu = () => {
  // Validate: chênh lệch ≠ 0 phải có lý do
  const chuaCoLyDo = danhSachHienThi.value.find(i => i.chenhLech !== 0 && !i.lyDo);
  if (chuaCoLyDo) {
    showToast('Có chênh lệch chưa chọn lý do.', 'danger');
    return;
  }

  const now   = new Date();
  const ngay  = phieu.ngay.split('-').reverse().join('/');
  const gio   = `${String(now.getHours()).padStart(2,'0')}:${String(now.getMinutes()).padStart(2,'0')}`;
  const ma    = `KK-2026-${String(soPhieuCounter).padStart(4,'0')}`;
  soPhieuCounter++;

  lichSu.value.unshift({
    ma,
    thoiGian: `${ngay} ${gio}`,
    nguoi:    phieu.nguoiThucHien || '—',
    tongSo:   tongChenhLechSL.value,
    tongGia:  tongChenhLechGT.value,
  });
  lichSu.value = lichSu.value.slice(0, 10);

  showToast(`Đã lưu phiếu ${ma} thành công.`, 'success');
};

// ── Helpers ──
const chenhClass = (val) => ({
  'kiemke-chenh--du':    val > 0,
  'kiemke-chenh--thieu': val < 0,
  'kiemke-chenh--bang':  val === 0,
});

const formatGia = (v) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v ?? 0);

const showToast = (message, type = 'info') => {
  toasts.value.push({ message, type });
  setTimeout(() => toasts.value.shift(), 3200);
};
</script>
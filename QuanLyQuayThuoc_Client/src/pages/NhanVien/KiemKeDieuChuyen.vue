<template>
  <div class="container-fluid">

    <div class="d-sm-flex align-items-center justify-content-between mb-4">
      <div>
        <h1 class="h3 mb-0 text-gray-800">Kiểm kê &amp; Đối soát tồn kho</h1>
        <p class="mb-0 text-muted small">Tạo phiếu kiểm kê theo lô, tính chênh lệch và lưu lịch sử.</p>
      </div>
    </div>

    <div class="card shadow mb-4">
      <div class="card-header py-3">
        <h6 class="m-0 font-weight-bold text-primary">A. Tạo phiếu kiểm kê</h6>
      </div>
      <div class="card-body">

        <div class="row">
          <div class="col-md-4 mb-3">
            <label>Người thực hiện</label>
            <input type="text" class="form-control" v-model="phieu.nguoiThucHien" readonly />
          </div>
          <div class="col-md-3 mb-3">
            <label>Ngày kiểm kê</label>
            <input type="date" class="form-control" v-model="phieu.ngay" />
          </div>
          <div class="col-md-5 mb-3">
            <label>&nbsp;</label>
            <div class="alert alert-warning mb-0">
              <i class="fas fa-info-circle mr-1"></i>
              Nhập số lượng thực tế — hệ thống tự tính <strong>Chênh lệch</strong>.
            </div>
          </div>
        </div>

        <div class="row kiemke-toolbar mb-3">
          <div class="col-md-8">
            <div class="form-group">
              <label>Lọc theo danh mục sản phẩm</label>
              <select class="form-control form-control-sm" v-model="locDanhMuc" @change="apDungLocFilter">
                <option value="">— Tất cả danh mục —</option>
                <option v-for="dm in danhSachDanhMuc" :key="dm" :value="dm">
                  {{ dm }}
                </option>
              </select>
            </div>
          </div>
          <div class="col-md-4">
            <div class="form-group">
              <label>&nbsp;</label>
              <div class="d-flex gap-2">
                <button type="button" :class="['btn btn-sm flex-grow-1', locSapHetHan ? 'btn-warning' : 'btn-outline-warning']" 
                  @click="toggleLocHetHan">
                  <i class="fas fa-exclamation-triangle mr-1"></i> {{ locSapHetHan ? 'Đang lọc sắp hết hạn' : 'Lọc lô sắp hết hạn' }}
                </button>
                <button type="button" class="btn btn-outline-secondary btn-sm" @click="resetLoc" title="Xóa tất cả lọc">
                  <i class="fas fa-sync-alt"></i>
                </button>
              </div>
            </div>
          </div>
        </div>

        <div class="table-responsive">
          <table class="table table-bordered table-hover mb-0 kiemke-table">
            <thead class="thead-light">
              <tr>
                <th style="min-width:260px;">Tên thuốc &amp; Số lô</th>
                <th style="min-width:150px;">Hạn sử dụng</th>
                <th style="min-width:120px;">Hệ thống</th>
                <th style="min-width:150px;">Thực tế</th>
                <th style="min-width:120px;">Chênh lệch</th>
                <th style="min-width:250px;">Lý do biến động</th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="duLieuTrangA.length === 0">
                <td colspan="6" class="text-center text-muted py-4">Không có dữ liệu phù hợp bộ lọc.</td>
              </tr>
              <tr v-for="item in duLieuTrangA" :key="item.id">
                <td>
                  <div class="font-weight-bold">{{ item.tenThuoc }}</div>
                  <div class="small text-muted"><span class="kiemke-batch">{{ item.soLo }}</span></div>
                </td>
                <td class="kiemke-hsd">
                  <div class="font-weight-bold">{{ item.hanSuDung }}</div>
                  <span v-if="getTinhTrangHSD(item.hanSuDung) === 'het-han'" class="badge badge-danger">Đã hết hạn</span>
                  <span v-else-if="getTinhTrangHSD(item.hanSuDung) === 'sap-het-han'" class="badge badge-warning">Sắp hết hạn</span>
                </td>
                <td><strong>{{ item.soLuongTon }}</strong></td>
                <td>
                  <template v-if="laLoHetHan(item)">
                    <button
                      type="button"
                      class="btn btn-sm btn-outline-danger btn-block"
                      :class="{ 'btn-danger text-white': daChonHuyLoHetHan(item) }"
                      @click="toggleHuyLoHetHan(item)"
                    >
                      <i class="fas fa-trash-alt mr-1"></i>
                      {{ daChonHuyLoHetHan(item) ? 'Bỏ chọn hủy lô' : 'Chọn hết hạn (hủy hết lô)' }}
                    </button>
                  </template>
                  <template v-else>
                    <input type="number" min="0" step="1" class="form-control form-control-sm"
                      v-model.number="item.soLuongThucTe" @input="capNhatChenhLech(item)" />
                  </template>
                </td>
                <td>
                  <span class="kiemke-chenh-lech" :class="chenhClass(item.chenhLech)">
                    {{ item.chenhLech > 0 ? '+' : '' }}{{ item.chenhLech }}
                  </span>
                </td>
                <td>
                  <template v-if="laLoHetHan(item)">
                    <span v-if="daChonHuyLoHetHan(item)" class="badge badge-danger">
                      Hết hạn - Hủy toàn bộ lô
                    </span>
                    <span v-else class="text-muted small">Nhấn nút "Chọn hết hạn" để hủy hết lô</span>
                  </template>
                  <template v-else>
                    <select class="form-control form-control-sm" v-model="item.lyDo" :disabled="item.chenhLech === 0"
                      :class="{ 'kiemke-reason-disabled': item.chenhLech === 0 }">
                      <option value="">— Chọn lý do —</option>
                      <template v-if="item.chenhLech > 0">
                        <option value="Nhập sai">Nhập sai (Tăng kho)</option>
                      </template>
                      <template v-else-if="item.chenhLech < 0">
                        <option value="Hỏng / Vỡ">Hỏng / Vỡ</option>
                        <option value="Hết hạn">Hết hạn</option>
                        <option value="Nhập sai">Nhập sai</option>
                        <option value="Thất thoát">Thất thoát</option>
                        <option value="Khác">Khác</option>
                      </template>
                    </select>
                  </template>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <div class="d-flex justify-content-between align-items-center mt-3" v-if="tongSoTrangA > 1">
          <small class="text-muted">Hiển thị trang {{ trangHienTaiA }} / {{ tongSoTrangA }}</small>
          <div class="btn-group">
            <button class="btn btn-sm btn-outline-primary" :disabled="trangHienTaiA === 1" @click="trangHienTaiA--">Trước</button>
            <button class="btn btn-sm btn-outline-primary" :disabled="trangHienTaiA === tongSoTrangA" @click="trangHienTaiA++">Sau</button>
          </div>
        </div>

        <div class="row mt-4 pt-3 border-top">
          <div class="col-md-8 mb-2">
            <div class="small text-muted">Tổng chênh lệch số lượng toàn quầy</div>
            <div class="kiemke-summary-value" :class="chenhClass(tongChenhLechSL)">
              {{ tongChenhLechSL > 0 ? '+' : '' }}{{ tongChenhLechSL }} sản phẩm
            </div>
          </div>
          <div class="col-md-4 mb-2 d-flex align-items-end justify-content-end">
            <button type="button" class="btn btn-primary btn-lg" @click="luuPhieu">
              <i class="fas fa-save mr-1"></i> Lưu phiếu kiểm kê
            </button>
          </div>
        </div>

        <div class="kiemke-toast-wrap mt-3" aria-live="polite">
          <div v-for="(t, i) in toasts" :key="i" :class="['alert', 'shadow', 'mb-2', 'alert-' + t.type]">
            {{ t.message }}
          </div>
        </div>
      </div>
    </div>

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
                <th style="min-width: 350px;">Biến động & Lý do</th>
                <th>Tổng chênh lệch</th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="duLieuTrangB.length === 0">
                <td colspan="5" class="text-center text-muted py-4">Chưa có dữ liệu lịch sử.</td>
              </tr>
              <tr v-for="h in duLieuTrangB" :key="h.ma">
                <td><strong>{{ h.ma }}</strong></td>
                <td class="text-nowrap">{{ h.thoiGian }}</td>
                <td>{{ h.nguoi }}</td>
                <td>
                  <div v-for="(item, idx) in h.chiTietThuoc" :key="idx" class="mb-2 pb-1 border-bottom last-no-border">
                    <div class="d-flex justify-content-between">
                      <span class="font-weight-bold small">{{ item.tenThuoc }}</span>
                      <span :class="item.chenhLech > 0 ? 'text-success' : 'text-danger'" class="small font-weight-bold">
                        {{ item.chenhLech > 0 ? '+' : '' }}{{ item.chenhLech }}
                      </span>
                    </div>
                    <div class="small text-muted italic">
                      <i class="fas fa-comment-dots mr-1"></i> Lý do: {{ item.lyDo || 'Không có' }}
                    </div>
                  </div>
                  <div v-if="!h.chiTietThuoc || h.chiTietThuoc.length === 0" class="small text-muted">Không chênh lệch</div>
                </td>
                <td>
                  <span :class="chenhClass(h.tongSo)" class="font-weight-bold">
                    {{ h.tongSo > 0 ? '+' : '' }}{{ h.tongSo }}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <div class="d-flex justify-content-between align-items-center mt-3" v-if="tongSoTrangB > 1">
          <small class="text-muted">Trang {{ trangHienTaiB }} / {{ tongSoTrangB }}</small>
          <div class="btn-group">
            <button class="btn btn-sm btn-outline-primary" :disabled="trangHienTaiB === 1" @click="trangHienTaiB--">Trước</button>
            <button class="btn btn-sm btn-outline-primary" :disabled="trangHienTaiB === tongSoTrangB" @click="trangHienTaiB++">Sau</button>
          </div>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import '../../assets/css_admin/kiem-ke.css';
import { ref, reactive, computed, onMounted } from 'vue';
import axiosClient from '../../api/axiosClient';

// ── State ──
const locDanhMuc = ref('');
const locSapHetHan = ref(false);
const toasts = ref([]);
const tatCaLo = ref([]); 
const lichSu = ref([]);
const danhSachDanhMuc = ref([]); 
const danhSachHienThi = ref([]);

const trangHienTaiA = ref(1);
const trangHienTaiB = ref(1);
const kichThuocTrang = 10;

const phieu = reactive({
  nguoiThucHien: 'Nhân viên quầy',
  ngay: new Date().toISOString().slice(0, 10),
  ghiChu: '',
});

// ── Computed Phân trang ──
const tongSoTrangA = computed(() => Math.ceil(danhSachHienThi.value.length / kichThuocTrang));
const duLieuTrangA = computed(() => {
  const batDau = (trangHienTaiA.value - 1) * kichThuocTrang;
  return danhSachHienThi.value.slice(batDau, batDau + kichThuocTrang);
});

const tongSoTrangB = computed(() => Math.ceil(lichSu.value.length / kichThuocTrang));
const duLieuTrangB = computed(() => {
  const batDau = (trangHienTaiB.value - 1) * kichThuocTrang;
  return lichSu.value.slice(batDau, batDau + kichThuocTrang);
});

// ── Load Data ──
const loadData = async () => {
  try {
    const resLo = await axiosClient.get('/KiemKe/danh-sach-lo');
    tatCaLo.value = resLo.map(item => ({
      ...item,
      soLuongThucTe: item.soLuongTon, 
      chenhLech: 0,
      lyDo: ''
    }));

    const categories = resLo.map(item => item.danhMuc).filter(v => v);
    danhSachDanhMuc.value = [...new Set(categories)];
    
    const resLS = await axiosClient.get('/KiemKe/lich-su');
    lichSu.value = resLS; 

    apDungLocFilter();
  } catch (error) {
    console.error("Lỗi tải dữ liệu:", error);
    showToast('Không thể tải dữ liệu', 'danger');
  }
};

onMounted(loadData);

const apDungLocFilter = () => {
  trangHienTaiA.value = 1;
  if (tatCaLo.value.length === 0) {
    danhSachHienThi.value = [];
    return;
  }
  danhSachHienThi.value = tatCaLo.value.filter(item => {
    const khớpDM = !locDanhMuc.value || item.danhMuc === locDanhMuc.value;
    const khớpHSD = !locSapHetHan.value || getTinhTrangHSD(item.hanSuDung) !== 'binh-thuong';
    return khớpDM && khớpHSD;    
  });
};

const toggleLocHetHan = () => {
  locSapHetHan.value = !locSapHetHan.value;
  apDungLocFilter();
};

const resetLoc = () => {
  locDanhMuc.value = '';
  locSapHetHan.value = false;
  apDungLocFilter();
};

const parseNgayHSD = (value) => {
  if (!value) return null;

  const raw = String(value).trim();
  const vietnameseDate = raw.match(/^(\d{1,2})[/-](\d{1,2})[/-](\d{4})$/);
  if (vietnameseDate) {
    const day = Number(vietnameseDate[1]);
    const month = Number(vietnameseDate[2]);
    const year = Number(vietnameseDate[3]);
    const parsed = new Date(year, month - 1, day);
    if (
      parsed.getFullYear() === year &&
      parsed.getMonth() === month - 1 &&
      parsed.getDate() === day
    ) {
      return parsed;
    }
    return null;
  }

  const parsed = new Date(raw);
  if (Number.isNaN(parsed.getTime())) return null;
  return new Date(parsed.getFullYear(), parsed.getMonth(), parsed.getDate());
};

const getTinhTrangHSD = (dateStr) => {
  const hsd = parseNgayHSD(dateStr);
  if (!hsd) return 'binh-thuong';

  const today = new Date();
  today.setHours(0, 0, 0, 0);
  if (hsd <= today) return 'het-han';
  
  const sixMonthsLater = new Date(today);
  sixMonthsLater.setMonth(sixMonthsLater.getMonth() + 6);
  if (hsd < sixMonthsLater) return 'sap-het-han';
  
  return 'binh-thuong';
};

const laLoHetHan = (item) => getTinhTrangHSD(item?.hanSuDung) === 'het-han';

const daChonHuyLoHetHan = (item) => {
  const soLuongTon = Number(item?.soLuongTon) || 0;
  return (
    laLoHetHan(item) &&
    item?.lyDo === 'Hết hạn' &&
    Number(item?.soLuongThucTe) === 0 &&
    Number(item?.chenhLech) === -soLuongTon
  );
};

const toggleHuyLoHetHan = (item) => {
  if (!laLoHetHan(item)) return;

  const soLuongTon = Number(item.soLuongTon) || 0;

  if (daChonHuyLoHetHan(item)) {
    item.soLuongThucTe = soLuongTon;
    item.chenhLech = 0;
    item.lyDo = '';
    return;
  }

  item.soLuongThucTe = 0;
  item.chenhLech = -soLuongTon;
  item.lyDo = 'Hết hạn';
};

const capNhatChenhLech = (item) => {
  const oldChenh = item.chenhLech;
  item.chenhLech = (item.soLuongThucTe || 0) - item.soLuongTon;

  if ((oldChenh <= 0 && item.chenhLech > 0) || (oldChenh >= 0 && item.chenhLech < 0)) {
    item.lyDo = '';
  }
  
  if (item.chenhLech === 0) item.lyDo = '';
};

const tongChenhLechSL = computed(() =>
  danhSachHienThi.value.reduce((s, i) => s + (i.chenhLech || 0), 0)
);

const luuPhieu = async () => {
    const dsBienDong = tatCaLo.value.filter(item => item.chenhLech !== 0);
    
    if (dsBienDong.length === 0) {
        showToast("Không có thay đổi nào để lưu!", "warning");
        return;
    }

    const loiLyDo = dsBienDong.find(i => !i.lyDo);
    if (loiLyDo) {
        showToast(`Thuốc ${loiLyDo.tenThuoc} chưa chọn lý do!`, 'danger');
        return;
    }

    if (!window.confirm("Xác nhận lưu phiếu và cập nhật kho?")) return;

    const payload = {
        ghiChu: phieu.ghiChu || `Kiểm kê ngày ${phieu.ngay}`,
        chiTiet: dsBienDong.map(item => ({
                maLo: item.id,
                soLuongHeThong: item.soLuongTon,
                soLuongThucTe: item.soLuongThucTe,
                lyDo: item.lyDo // Backend sẽ nhận qua DTO và gán vào LyDoLech
            }))
    };

    try {
        await axiosClient.post('/KiemKe/luu-phieu', payload);
        showToast('Lưu phiếu thành công!', 'success');
        await loadData(); 
        phieu.ghiChu = '';
        trangHienTaiB.value = 1; 
    } catch (error) {
        console.error(error);
        const msg = error.response?.data?.message || 'Lỗi khi lưu phiếu';
        showToast(msg, 'danger');
    }
};

const chenhClass = (val) => ({
  'kiemke-chenh--du': val > 0,
  'kiemke-chenh--thieu': val < 0,
  'kiemke-chenh--bang': val === 0 || !val,
});

const showToast = (message, type = 'info') => {
  toasts.value.push({ message, type });
  setTimeout(() => toasts.value.shift(), 3200);
};
</script>

<style scoped>
.last-no-border:last-child {
  border-bottom: none !important;
}
.italic {
  font-style: italic;
}
.kiemke-summary-value {
    font-size: 1.5rem;
    font-weight: bold;
}
.kiemke-chenh--du { color: #28a745; }
.kiemke-chenh--thieu { color: #dc3545; }
.kiemke-chenh--bang { color: #6c757d; }
</style>

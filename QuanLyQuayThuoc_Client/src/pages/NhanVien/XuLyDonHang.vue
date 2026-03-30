<template>
  <div class="container-fluid">

    <h1 class="h3 mb-2 text-gray-800">Xử lý đơn hàng</h1>
    <p class="text-muted small mb-4">Quản lý quy trình từ Website/App.</p>

    <!-- 1. Tab trạng thái -->
    <div class="card shadow mb-4">
      <div class="card-header py-3">
        <h6 class="m-0 font-weight-bold text-primary">Trạng thái đơn hàng</h6>
      </div>
      <div class="card-body">
        <ul class="nav nav-pills flex-wrap dh-order-tabs">
          <li class="nav-item" v-for="tab in tabs" :key="tab.value">
            <a class="nav-link dh-filter-tab" :class="{ active: tabHienTai === tab.value }"
              href="#" @click.prevent="tabHienTai = tab.value">
              {{ tab.label }}
              <span class="badge badge-light badge-counter" :class="tab.badgeClass">
                {{ demTheoTab(tab.value) }}
              </span>
            </a>
          </li>
        </ul>
        <p class="small text-muted mb-0 mt-2">
          <i class="fas fa-info-circle"></i> Hàng vàng/đỏ: đơn đặt lâu chưa xử lý.
        </p>
      </div>
    </div>

    <!-- 2. Danh sách -->
    <div class="card shadow mb-4">
      <div class="card-header py-3 d-flex justify-content-between align-items-center">
        <h6 class="m-0 font-weight-bold text-primary">Danh sách đơn hàng</h6>
        <span class="small text-muted">Cột thời gian nổi bật khi chờ quá lâu</span>
      </div>
      <div class="card-body p-0">
        <div v-if="dangTai" class="text-center py-4">
          <div class="spinner-border text-primary" role="status"><span class="sr-only">Đang tải...</span></div>
        </div>
        <div v-else class="table-responsive dh-table-wrap">
          <table class="table table-hover mb-0 dh-table">
            <thead class="thead-light">
              <tr>
                <th>Mã đơn</th>
                <th>Thời gian đặt</th>
                <th>Khách hàng &amp; SĐT</th>
                <th>Tổng tiền</th>
                <th>Loại đơn</th>
                <th style="min-width:110px;">Thao tác</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="don in donHangDaLoc" :key="don.maDonHang"
                :class="rowClass(don)">
                <td><strong>#{{ don.maDonHang }}</strong></td>
                <td class="dh-time-cell">
                  <div class="dh-time-main">{{ don.ngayDat }}</div>
                  <span v-if="don.capDoTre === 'urgent'" class="badge badge-danger dh-wait-badge">Chờ &gt; 45 phút</span>
                  <span v-else-if="don.capDoTre === 'warn'" class="badge badge-warning text-dark dh-wait-badge">Chờ &gt; 20 phút</span>
                </td>
                <td>
                  {{ don.tenKhachHang }}<br>
                  <small class="text-muted">{{ don.soDienThoaiNhan }}</small>
                </td>
                <td><strong>{{ formatGia(don.tongTien) }}</strong></td>
                <td>
                  <span v-if="don.laThuocKeDon" class="badge dh-badge-ke-don">Thuốc kê đơn</span>
                  <span v-else-if="don.trangThai === 'da-huy'" class="badge badge-secondary">Đã hủy</span>
                  <span v-else class="badge dh-badge-thuong">Thuốc thường</span>
                </td>
                <td>
                  <button type="button"
                    :class="['btn', 'btn-sm', don.trangThai === 'da-huy' ? 'btn-secondary' : 'btn-primary']"
                    @click="moChiTiet(don)">
                    {{ don.trangThai === 'da-huy' ? 'Xem' : 'Chi tiết' }}
                  </button>
                </td>
              </tr>
              <tr v-if="donHangDaLoc.length === 0">
                <td colspan="6" class="text-center text-muted py-4">Không có đơn hàng.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- ── MODAL: Chi tiết đơn ── -->
    <div class="modal fade" :class="{ show: hienChiTiet }" :style="hienChiTiet ? 'display:block' : ''"
      tabindex="-1" role="dialog" @click.self="hienChiTiet = false">
      <div class="modal-dialog modal-xl modal-dialog-scrollable" role="document">
        <div class="modal-content" v-if="donChon">
          <div class="modal-header">
            <h5 class="modal-title">Chi tiết đơn <strong>#{{ donChon.maDonHang }}</strong></h5>
            <button type="button" class="close" @click="hienChiTiet = false"><span>&times;</span></button>
          </div>
          <div class="modal-body">

            <div class="row">
              <div class="col-md-6 mb-3">
                <div class="dh-detail-section-title">Thông tin khách &amp; giao hàng</div>
                <p class="mb-1"><strong>{{ donChon.tenKhachHang }}</strong></p>
                <p class="mb-1"><i class="fas fa-phone text-primary"></i> {{ donChon.soDienThoaiNhan }}</p>
                <p class="mb-1"><i class="fas fa-map-marker-alt text-danger"></i> {{ donChon.diaChiGiaoHang }}</p>
                <p class="mb-0 text-muted small"><strong>Ghi chú:</strong> {{ donChon.ghiChu || 'Không có' }}</p>
              </div>
              <div class="col-md-6 mb-3">
                <div class="dh-detail-section-title">Loại đơn &amp; tổng</div>
                <p class="mb-1">
                  <span v-if="donChon.laThuocKeDon" class="badge dh-badge-ke-don">Thuốc kê đơn</span>
                  <span v-else class="badge dh-badge-thuong">Thuốc thường</span>
                </p>
                <p class="mb-0"><strong>Tổng thanh toán:</strong>
                  <span class="text-primary">{{ formatGia(donChon.tongTien) }}</span>
                </p>
              </div>
            </div>

            <!-- Ảnh đơn thuốc — AnhDonThuoc trong bảng DonHang -->
            <div v-if="donChon.laThuocKeDon" class="mb-4">
              <div class="dh-detail-section-title text-danger">
                <i class="fas fa-prescription-bottle-alt"></i> Kiểm tra đơn thuốc (kê đơn)
              </div>
              <div class="dh-prescription-box">
                <p class="small text-muted mb-2">Ảnh đơn thuốc khách upload — đối chiếu với giỏ hàng.</p>
                <img v-if="donChon.anhDonThuoc" :src="getImageUrl(donChon.anhDonThuoc)"
                  alt="Ảnh đơn thuốc" style="max-width:100%;" />
                <p v-else class="text-muted small">(Không có ảnh đơn thuốc)</p>
              </div>
            </div>

            <!-- Chi tiết sản phẩm + chọn lô (FEFO) — từ ChiTietDonHang JOIN LoHang -->
            <div class="dh-detail-section-title">Danh sách sản phẩm &amp; chọn lô (FEFO)</div>
            <p class="small text-muted mb-2">
              Hệ thống gợi ý lô có <strong>hạn dùng gần nhất trước</strong> (FEFO). Có thể đổi lô thủ công.
            </p>
            <div class="table-responsive">
              <table class="table table-bordered table-sm">
                <thead class="thead-light">
                  <tr>
                    <th>Tên thuốc</th>
                    <th>Đơn vị</th>
                    <th>Chọn lô hàng</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(sp, i) in donChon.chiTiet" :key="i">
                    <td>{{ sp.tenThuoc }}</td>
                    <td>{{ sp.tenDonVi }}</td>
                    <td>
                      <select class="form-control form-control-sm dh-batch-select" v-model="loChon[i]">
                        <option v-for="(lo, j) in sp.loHang" :key="j" :value="lo.maLo">
                          {{ lo.soLo }} | HSD {{ lo.hanSuDung }}{{ lo.fefo ? ' (FEFO — gợi ý)' : '' }}
                        </option>
                      </select>
                      <div class="dh-batch-hint mt-1">FEFO: ưu tiên lô có HSD gần nhất</div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
          <div class="modal-footer flex-wrap">
            <a :href="'tel:' + donChon.soDienThoaiNhan.replace(/\s/g,'')"
              class="btn btn-outline-success mr-auto">
              <i class="fas fa-phone-alt"></i> Liên hệ khách
            </a>
            <button type="button" class="btn btn-secondary" @click="hienChiTiet = false">Đóng</button>
            <button type="button" class="btn btn-danger"
              :disabled="donChon.trangThai === 'da-huy'"
              @click="moModalHuy">
              <i class="fas fa-times"></i> Hủy đơn
            </button>
            <button type="button" class="btn btn-primary"
              :disabled="donChon.trangThai === 'da-huy'"
              @click="xacNhanVaInHoaDon">
              <i class="fas fa-check"></i> Xác nhận &amp; In hóa đơn
            </button>
          </div>
        </div>
      </div>
    </div>
    <div v-if="hienChiTiet" class="modal-backdrop fade show"></div>

    <!-- ── MODAL: Hủy đơn ── -->
    <div class="modal fade" :class="{ show: hienHuyDon }" :style="hienHuyDon ? 'display:block' : ''"
      tabindex="-1" role="dialog" @click.self="hienHuyDon = false">
      <div class="modal-dialog" role="document">
        <div class="modal-content">
          <div class="modal-header bg-light">
            <h5 class="modal-title text-danger">Hủy đơn hàng</h5>
            <button type="button" class="close" @click="hienHuyDon = false"><span>&times;</span></button>
          </div>
          <div class="modal-body">
            <p class="small text-muted mb-2">Vui lòng chọn hoặc ghi rõ lý do (phục vụ báo cáo &amp; CSKH).</p>
            <div class="form-group">
              <label>Lý do phổ biến</label>
              <select class="form-control" v-model="lyDoHuyChon">
                <option value="">— Chọn —</option>
                <option>Hết hàng / không đủ tồn</option>
                <option>Khách không nghe máy</option>
                <option>Khách đổi ý / đặt nhầm</option>
                <option>Đơn thuốc không hợp lệ</option>
                <option>Khác</option>
              </select>
            </div>
            <div class="form-group mb-0">
              <label>Chi tiết thêm</label>
              <textarea class="form-control" v-model="lyDoHuyChiTiet" rows="2"
                placeholder="Ghi chú bổ sung…"></textarea>
            </div>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="hienHuyDon = false">Quay lại</button>
            <button type="button" class="btn btn-danger" :disabled="dangHuy" @click="xacNhanHuy">
              {{ dangHuy ? 'Đang hủy...' : 'Xác nhận hủy' }}
            </button>
          </div>
        </div>
      </div>
    </div>
    <div v-if="hienHuyDon" class="modal-backdrop fade show"></div>

    <!-- ── MODAL: Hóa đơn nhiệt 80mm ── -->
    <div class="modal fade" :class="{ show: hienHoaDon }" :style="hienHoaDon ? 'display:block' : ''"
      tabindex="-1" role="dialog">
      <div class="modal-dialog modal-dialog-centered" role="document" style="max-width:92vw;">
        <div class="modal-content">
          <div class="modal-header py-2">
            <h5 class="modal-title small font-weight-bold">Hóa đơn nhiệt (80mm)</h5>
            <button type="button" class="close" @click="hienHoaDon = false"><span>&times;</span></button>
          </div>
          <div class="modal-body p-2">
            <div class="dh-thermal-wrap">
              <div class="hoa-don-thermal" id="hoaDonThermalNoiDung">
                <div class="hoa-don-thermal__name">Nhà thuốc Pharmative</div>
                <div class="hoa-don-thermal__sub">{{ tenNhaThuoc }}</div>
                <div class="hoa-don-thermal__sub">Hotline: {{ hotline }}</div>
                <hr class="hoa-don-thermal__line">
                <div class="hoa-don-thermal__row"><span>Mã đơn:</span><span>#{{ donChon?.maDonHang }}</span></div>
                <div class="hoa-don-thermal__row"><span>Thời gian:</span><span>{{ donChon?.ngayDat }}</span></div>
                <div class="hoa-don-thermal__row"><span>Khách:</span><span>{{ donChon?.tenKhachHang }}</span></div>
                <hr class="hoa-don-thermal__line">
                <div class="hoa-don-thermal__items">
                  <div v-for="(sp, i) in donChon?.chiTiet" :key="i" class="hoa-don-thermal__item">
                    <div class="hoa-don-thermal__item-name">{{ sp.tenThuoc }} — {{ sp.tenDonVi }}</div>
                    <div class="hoa-don-thermal__item-meta">SL: {{ sp.soLuong }} × {{ formatGia(sp.giaBanTaiThoiDiem) }}</div>
                  </div>
                </div>
                <hr class="hoa-don-thermal__line">
                <div class="hoa-don-thermal__total">Tổng: {{ formatGia(donChon?.tongTien) }}</div>
                <div class="hoa-don-thermal__qr" v-if="donChon">
                  <img :src="qrUrl" :alt="'QR ' + donChon.maDonHang" />
                  <div class="hoa-don-thermal__qr-caption">Quét QR để xem lại đơn</div>
                </div>
                <div class="hoa-don-thermal__sub" style="margin-top:8px;">Cảm ơn quý khách!</div>
              </div>
            </div>
          </div>
          <div class="modal-footer py-2">
            <button type="button" class="btn btn-secondary btn-sm" @click="hienHoaDon = false">Đóng</button>
            <button type="button" class="btn btn-primary btn-sm" @click="inHoaDon">
              <i class="fas fa-print"></i> In phiếu
            </button>
          </div>
        </div>
      </div>
    </div>
    <div v-if="hienHoaDon" class="modal-backdrop fade show"></div>

  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import axiosClient from '../../api/axiosClient';

// ── Config nhà thuốc ──
const tenNhaThuoc = '123 Lê Lợi, Q.1, TP.HCM';
const hotline     = '1900 xxxx';

// ── State ──
const danhSachDon  = ref([]);
const dangTai      = ref(false);
const tabHienTai   = ref('cho-xac-nhan');
const donChon      = ref(null);
const loChon       = ref([]);   // Lô được chọn cho từng ChiTietDonHang

// Modals
const hienChiTiet  = ref(false);
const hienHuyDon   = ref(false);
const hienHoaDon   = ref(false);
const dangHuy      = ref(false);
const lyDoHuyChon     = ref('');
const lyDoHuyChiTiet  = ref('');

// ── Tabs — ánh xạ với cột TrangThai trong bảng DonHang ──
const tabs = [
  { value: 'cho-xac-nhan', label: 'Chờ xác nhận', badgeClass: 'text-primary' },
  { value: 'cho-lay-hang', label: 'Chờ lấy hàng', badgeClass: 'text-dark'    },
  { value: 'dang-giao',    label: 'Đang giao',     badgeClass: 'text-dark'    },
  { value: 'hoan-tat',     label: 'Hoàn tất',      badgeClass: 'text-success' },
  { value: 'da-huy',       label: 'Đã hủy',        badgeClass: 'text-danger'  },
];

// ── Load dữ liệu ──
// GET /DonHang/xu-ly — trả về danh sách JOIN NguoiDung, ChiTietDonHang, LoHang
const loadData = async () => {
  dangTai.value = true;
  try {
    const res = await axiosClient.get('/DonHang/xu-ly');
    danhSachDon.value = res.data;
  } catch (err) {
    console.error('Lỗi tải đơn hàng:', err);
  } finally {
    dangTai.value = false;
  }
};

const demTheoTab   = (status) => danhSachDon.value.filter(d => d.trangThai === status).length;
const donHangDaLoc = computed(() => danhSachDon.value.filter(d => d.trangThai === tabHienTai.value));

const rowClass = (don) => ({
  'dh-row--tre':         don.capDoTre === 'warn' || don.capDoTre === 'urgent',
  'dh-row--tre-urgent':  don.capDoTre === 'urgent',
});

// ── Mở chi tiết ──
// GET /DonHang/:id — trả về đủ ChiTietDonHang[] + LoHang[] gợi ý FEFO
const moChiTiet = async (don) => {
  try {
    const res = await axiosClient.get(`/DonHang/${don.maDonHang}`);
    donChon.value = res.data;
    // Mặc định chọn lô FEFO đầu tiên cho mỗi sản phẩm
    loChon.value = (res.data.chiTiet || []).map(sp => {
      const fefo = sp.loHang?.find(l => l.fefo);
      return fefo ? fefo.maLo : sp.loHang?.[0]?.maLo;
    });
    hienChiTiet.value = true;
  } catch (err) {
    console.error('Lỗi tải chi tiết đơn:', err);
  }
};

// ── Xác nhận + in hóa đơn ──
// PUT /DonHang/:id/xac-nhan — body: { chiTiet: [{ maChiTiet, maLo }] }
const xacNhanVaInHoaDon = async () => {
  try {
    await axiosClient.put(`/DonHang/${donChon.value.maDonHang}/xac-nhan`, {
      chiTiet: (donChon.value.chiTiet || []).map((sp, i) => ({
        maChiTiet: sp.maChiTiet,
        maLo:      loChon.value[i],
      })),
    });
    hienChiTiet.value = false;
    hienHoaDon.value  = true;
    loadData();
  } catch (err) {
    console.error('Lỗi xác nhận đơn:', err);
  }
};

// ── Hủy đơn ──
// PUT /DonHang/:id/huy — body: { lyDo }
const moModalHuy  = () => { lyDoHuyChon.value = ''; lyDoHuyChiTiet.value = ''; hienHuyDon.value = true; };
const xacNhanHuy  = async () => {
  dangHuy.value = true;
  try {
    await axiosClient.put(`/DonHang/${donChon.value.maDonHang}/huy`, {
      lyDo: [lyDoHuyChon.value, lyDoHuyChiTiet.value].filter(Boolean).join(' — '),
    });
    hienHuyDon.value  = false;
    hienChiTiet.value = false;
    loadData();
  } catch (err) {
    console.error('Lỗi hủy đơn:', err);
  } finally {
    dangHuy.value = false;
  }
};

// ── In hóa đơn ──
const inHoaDon = () => window.print();

const qrUrl = computed(() => {
  if (!donChon.value) return '';
  return `https://api.qrserver.com/v1/create-qr-code/?size=120x120&data=${encodeURIComponent('https://pharmative.vn/don-hang/' + donChon.value.maDonHang)}`;
});

const getImageUrl = (path) => {
  if (!path) return '';
  if (path.startsWith('http')) return path;
  return `https://localhost:7070${path}`;
};

const formatGia = (value) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value ?? 0);

onMounted(loadData);
</script>
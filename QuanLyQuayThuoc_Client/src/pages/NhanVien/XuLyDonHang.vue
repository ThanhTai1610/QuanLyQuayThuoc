<template>
  <div class="container-fluid">
    <h1 class="h3 mb-2 text-gray-800">Quản lý đơn hàng</h1>
    <p class="text-muted small mb-4">Theo dõi và cập nhật trạng thái xử lý đơn hàng từ hệ thống.</p>

    <div class="card shadow mb-4">
      <div class="card-body">
        <ul class="nav nav-pills flex-wrap">
          <li class="nav-item" v-for="tab in tabs" :key="tab.value">
            <a class="nav-link" :class="{ active: tabHienTai === tab.value }"
               href="#" @click.prevent="tabHienTai = tab.value">
              {{ tab.label }}
              <span class="badge badge-light ml-1">{{ demTheoTab(tab.value) }}</span>
            </a>
          </li>
        </ul>
      </div>
    </div>

    <div class="card shadow mb-4">
      <div class="card-body p-0">
        <div v-if="dangTai" class="text-center py-4">
          <div class="spinner-border text-primary"></div>
        </div>
        <div v-else class="table-responsive">
          <table class="table table-hover mb-0">
            <thead class="thead-light">
              <tr>
                <th>Mã đơn</th>
                <th>Thời gian</th>
                <th>Khách hàng</th>
                <th>SĐT nhận</th>
                <th>Tổng tiền</th>
                <th>Trạng thái</th>
                <th>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="don in donHangDaLoc" :key="don.maDonHang">
                <td><strong>#{{ don.maDonHang }}</strong></td>
                <td>{{ don.ngayDat }}</td>
                <td>{{ don.tenKhachHang }}</td>
                <td>{{ don.soDienThoaiNhan || '—' }}</td>
                <td><strong>{{ formatGia(don.tongTien) }}</strong></td>
                <td>
                  <span :class="['badge', getBadgeClass(don.trangThai)]">
                    {{ getLabelTrangThai(don.trangThai) }}
                  </span>
                </td>
                <td>
                  <button class="btn btn-sm btn-primary" @click="moChiTiet(don)">
                    Chi tiết
                  </button>
                </td>
              </tr>
              <tr v-if="donHangDaLoc.length === 0">
                <td colspan="7" class="text-center text-muted py-4">Không có đơn hàng ở mục này.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- MODAL CHI TIẾT -->
    <div class="modal fade" :class="{ show: hienChiTiet }" :style="hienChiTiet ? 'display:block' : ''" tabindex="-1" @click.self="hienChiTiet = false">
      <div class="modal-dialog modal-lg modal-dialog-scrollable">
        <div class="modal-content" v-if="donChon">
          <div class="modal-header">
            <h5 class="modal-title">Chi tiết đơn <strong>#{{ donChon.maDonHang }}</strong></h5>
            <button type="button" class="close" @click="hienChiTiet = false"><span>&times;</span></button>
          </div>
          <div class="modal-body">
            <div class="row mb-4">
              <div class="col-md-6">
                <h6 class="font-weight-bold text-primary border-bottom pb-1">Người nhận</h6>
                <p class="mb-1"><strong>{{ donChon.tenKhachHang }}</strong></p>
                <p class="mb-1"><i class="fas fa-phone"></i> {{ donChon.soDienThoaiNhan }}</p>
                <p class="mb-1"><i class="fas fa-map-marker-alt"></i> {{ donChon.diaChiGiaoHang }}</p>
              </div>
              <div class="col-md-6">
                <h6 class="font-weight-bold text-primary border-bottom pb-1">Thông tin đơn</h6>
                <p class="mb-1">Ngày đặt: {{ donChon.ngayDat }}</p>
                <p class="mb-1">Ghi chú: {{ donChon.ghiChu || 'Không có' }}</p>
                <p class="mb-0">Loại:
                  <span class="badge" :class="donChon.laThuocKeDon ? 'badge-warning' : 'badge-info'">
                    {{ donChon.laThuocKeDon ? 'Thuốc kê đơn' : 'Thuốc thường' }}
                  </span>
                </p>
              </div>
            </div>

            <h6 class="font-weight-bold">Sản phẩm đã đặt</h6>
            <div class="table-responsive mb-4">
              <table class="table table-bordered table-sm">
                <thead class="bg-light">
                  <tr>
                    <th>Tên thuốc</th>
                    <th class="text-center">SL</th>
                    <th class="text-right">Đơn giá</th>
                    <th class="text-right">Thành tiền</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(sp, i) in (donChon.chiTietSanPham || [])" :key="i">
                    <td>{{ sp.tenThuoc }}</td>
                    <td class="text-center">{{ sp.soLuong }} {{ sp.tenDonVi }}</td>
                    <td class="text-right">{{ formatGia(sp.giaBan) }}</td>
                    <td class="text-right">{{ formatGia(sp.thanhTien) }}</td>
                  </tr>
                  <tr class="font-weight-bold">
                    <td colspan="3" class="text-right">Tổng cộng:</td>
                    <td class="text-right text-primary">{{ formatGia(donChon.tongTien) }}</td>
                  </tr>
                </tbody>
              </table>
            </div>

            <div class="card bg-light border-left-primary">
              <div class="card-body">
                <h6 class="font-weight-bold">Cập nhật tiến độ đơn hàng</h6>
                <div class="row align-items-end">
                  <div class="col-md-8">
                    <label class="small">Chuyển trạng thái sang:</label>
                    <select class="form-control" v-model="trangThaiMoi">
                      <option v-for="t in tabs" :key="t.value" :value="t.value">{{ t.label }}</option>
                    </select>
                  </div>
                  <div class="col-md-4">
                    <button class="btn btn-primary btn-block" @click="capNhatTrangThai" :disabled="dangLuu">
                      {{ dangLuu ? 'Đang lưu...' : 'Lưu thay đổi' }}
                    </button>
                  </div>
                </div>
                <div class="mt-2" v-if="trangThaiMoi === 'Đã hủy'">
                  <label class="small text-danger">Lý do hủy đơn:</label>
                  <textarea class="form-control" v-model="lyDoHuy" rows="2" placeholder="Nhập lý do hủy..."></textarea>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div v-if="hienChiTiet" class="modal-backdrop fade show"></div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import axiosClient from '../../api/axiosClient';

// --- QUẢN LÝ TRẠNG THÁI ---
const danhSachDon = ref([]);
const dangTai = ref(false);
const donChon = ref(null);
const hienChiTiet = ref(false);
const trangThaiMoi = ref('');
const lyDoHuy = ref('');
const dangLuu = ref(false);

const tabs = [
  { value: 'Chờ xử lý',    label: 'Chờ xác nhận', badge: 'badge-primary' },
  { value: 'Chờ lấy hàng', label: 'Chờ lấy hàng', badge: 'badge-info' },
  { value: 'Đang giao',    label: 'Đang giao',     badge: 'badge-warning' },
  { value: 'Đã giao',      label: 'Hoàn tất',      badge: 'badge-success' },
  { value: 'Đã hủy',       label: 'Đã hủy',        badge: 'badge-danger' },
];

const tabHienTai = ref('Chờ xử lý');

// --- TẢI DỮ LIỆU ---
const loadData = async () => {
  dangTai.value = true;
  try {
    const res = await axiosClient.get('/XuLyDonHang/danh-sach');
    // axiosClient interceptor đã unwrap response.data rồi, nên res chính là data
    danhSachDon.value = Array.isArray(res) ? res : [];
  } catch (err) {
    console.error('Lỗi tải dữ liệu:', err);
    danhSachDon.value = [];
  } finally {
    dangTai.value = false;
  }
};

// --- LỌC & ĐẾM ---
const donHangDaLoc = computed(() =>
  danhSachDon.value.filter(d => d.trangThai === tabHienTai.value)
);

const demTheoTab = (status) =>
  danhSachDon.value.filter(d => d.trangThai === status).length;

// --- GIAO DIỆN ---
const getLabelTrangThai = (val) => tabs.find(t => t.value === val)?.label || val;
const getBadgeClass = (val) => tabs.find(t => t.value === val)?.badge || 'badge-secondary';

// --- CHI TIẾT & CẬP NHẬT ---
const moChiTiet = async (don) => {
  try {
    const res = await axiosClient.get(`/XuLyDonHang/chi-tiet/${don.maDonHang}`);
    // axiosClient interceptor đã unwrap rồi, res chính là object đơn hàng
    donChon.value = res;
    trangThaiMoi.value = res.trangThai;
    lyDoHuy.value = '';
    hienChiTiet.value = true;
  } catch (err) {
    alert("Không thể tải chi tiết đơn hàng");
  }
};

const capNhatTrangThai = async () => {
  if (!donChon.value) return;
  dangLuu.value = true;
  try {
    await axiosClient.put(`/XuLyDonHang/cap-nhat-trang-thai/${donChon.value.maDonHang}`, {
      trangThaiMoi: trangThaiMoi.value,
      lyDoHuy: lyDoHuy.value
    });
    hienChiTiet.value = false;
    await loadData();
    alert("Đã cập nhật trạng thái thành công!");
  } catch (err) {
    alert("Lỗi khi cập nhật trạng thái!");
  } finally {
    dangLuu.value = false;
  }
};

const formatGia = (v) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v ?? 0);

onMounted(loadData);
</script>
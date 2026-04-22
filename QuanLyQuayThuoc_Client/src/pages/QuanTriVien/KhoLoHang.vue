<template>
  <section>
    <div class="qlk-filters-card card mb-3">
      <div class="card-header py-3 d-flex justify-content-between align-items-center bg-white">
        <h6 class="m-0 font-weight-bold text-primary">2. Danh sách lô hàng</h6>
        <button class="btn btn-outline-primary btn-sm shadow-sm" @click="hienThongKe = !hienThongKe">
          <i class="fas" :class="hienThongKe ? 'fa-expand-alt' : 'fa-chart-pie'"></i>
          {{ hienThongKe ? ' Phóng to bảng (Ẩn thống kê)' : ' Xem thống kê nhanh' }}
        </button>
      </div>
      <div class="card-body">
        <div class="row">
          <div class="col-md-3 mb-3">
            <label class="small text-muted font-weight-bold">Tìm theo tên / hoạt chất</label>
            <input class="form-control" v-model="tuKhoa" @input="onFilter"
              placeholder="Ví dụ: Paracetamol..." />
          </div>
          <div class="col-md-3 mb-3">
            <label class="small text-muted font-weight-bold">Hạn dùng theo tháng</label>
            <input type="month" class="form-control" v-model="locThang" @change="onFilter" />
          </div>
          <div class="col-md-3 mb-3">
            <label class="small text-muted font-weight-bold">Bộ lọc nhanh</label>
            <select class="form-control" v-model="locTrangThai" @change="onFilter">
              <option value="all">Tất cả lô</option>
              <option value="expired">Chỉ đã hết hạn</option>
              <option value="soon">Còn dưới 6 tháng</option>
            </select>
          </div>
          <div class="col-md-3 mb-3">
            <label class="small text-muted font-weight-bold">Số dòng mỗi trang</label>
            <select class="form-control" v-model="soDongMoiTrang" @change="trangHienTai = 1">
              <option :value="10">10 dòng</option>
              <option :value="20">20 dòng</option>
              <option :value="50">50 dòng</option>
            </select>
          </div>
        </div>
        <p class="qlk-muted mb-0 small">
          <i class="fas fa-circle text-danger mr-1"></i> Đỏ: đã hết hạn —
          <i class="fas fa-circle text-warning mr-1"></i> Vàng: sắp hết hạn (< 6 tháng).
        </p>
      </div>
    </div>

    <div class="row">
      <div :class="hienThongKe ? 'col-lg-8' : 'col-lg-12'" class="mb-3 transition-layout">
        <div class="card shadow-sm">
          <div class="card-header py-3 d-flex justify-content-between align-items-center">
            <div>
              <div class="font-weight-bold text-primary">Danh sách lô hàng</div>
              <div class="small text-muted">Sắp xếp theo Hạn dùng tăng dần</div>
            </div>
            <span class="badge badge-primary px-3">{{ danhSach.length }} lô hàng</span>
          </div>
          <div class="card-body p-0">
            <div v-if="dangTai" class="text-center py-5">
              <div class="spinner-border text-primary" role="status"></div>
              <div class="mt-2 small text-muted">Đang tải dữ liệu kho...</div>
            </div>
            <div v-else class="table-responsive">
              <table class="table table-bordered table-hover mb-0 qlk-table">
                <thead class="thead-light">
                  <tr>
                    <th>Số lô</th>
                    <th>Hạn sử dụng</th>
                    <th>Ngày nhập</th>
                    <th>Tồn lô</th>
                    <th>Giá nhập</th>
                    <th>Thuốc</th>
                    <th v-if="isAdmin" class="text-center">Hành động</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="lo in danhSachHienThi" :key="lo.maLo" :class="rowClass(lo)">
                    <td>
                      <a href="#" class="text-primary font-weight-bold" @click.prevent="xemThuocCungLo(lo.soLo)">
                        {{ lo.soLo }}
                      </a>
                    </td>
                    <td>
                      {{ lo.hanSuDung }}
                      <span v-if="laHetHan(lo)" class="badge badge-danger ml-1">Hết hạn</span>
                      <span v-else-if="laSapHetHan(lo)" class="badge badge-warning text-dark ml-1">Sắp hết hạn</span>
                    </td>
                    <td>{{ lo.ngaySanXuat || lo.ngayNhap }}</td>
                    <td class="text-center font-weight-bold text-info">{{ lo.soLuongTon }}</td>
                    <td>{{ formatGia(lo.giaNhap) }}</td>
                    <td>{{ lo.tenThuoc }}</td>
                    <td v-if="isAdmin" class="text-center">
                      <button class="btn btn-warning btn-sm" @click="moModalSua(lo)">
                        <i class="fas fa-edit"></i> Sửa
                      </button>
                    </td>
                  </tr>
                  <tr v-if="danhSachHienThi.length === 0">
                    <td :colspan="isAdmin ? 7 : 6" class="text-center text-muted py-5">
                      <i class="fas fa-box-open fa-2x mb-2 d-block"></i>
                      Không tìm thấy lô hàng nào phù hợp.
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <div v-if="tongSoTrang > 1" class="d-flex justify-content-between align-items-center px-3 py-2 border-top bg-light">
              <div class="small text-muted font-italic">
                Hiển thị {{ batDau + 1 }}–{{ ketThuc }} trong tổng số {{ danhSach.length }} dòng
              </div>
              <ul class="pagination pagination-sm mb-0">
                <li class="page-item" :class="{ disabled: trangHienTai === 1 }">
                  <a class="page-link" href="#" @click.prevent="trangHienTai = 1">
                    <i class="fas fa-angle-double-left"></i>
                  </a>
                </li>
                <li class="page-item" :class="{ disabled: trangHienTai === 1 }">
                  <a class="page-link" href="#" @click.prevent="trangHienTai--">
                    <i class="fas fa-angle-left"></i>
                  </a>
                </li>
                <li v-for="trang in danhSachTrang" :key="trang"
                  class="page-item" :class="{ active: trang === trangHienTai }">
                  <a class="page-link" href="#" @click.prevent="trangHienTai = trang">{{ trang }}</a>
                </li>
                <li class="page-item" :class="{ disabled: trangHienTai === tongSoTrang }">
                  <a class="page-link" href="#" @click.prevent="trangHienTai++">
                    <i class="fas fa-angle-right"></i>
                  </a>
                </li>
                <li class="page-item" :class="{ disabled: trangHienTai === tongSoTrang }">
                  <a class="page-link" href="#" @click.prevent="trangHienTai = tongSoTrang">
                    <i class="fas fa-angle-double-right"></i>
                  </a>
                </li>
              </ul>
            </div>
          </div>
        </div>
      </div>

      <div v-if="hienThongKe" class="col-lg-4">
        <div class="qlk-stat-card shadow-sm border-left-primary">
          <div class="font-weight-bold text-primary mb-3 border-bottom pb-2">
            <i class="fas fa-chart-line mr-2"></i>Thống kê nhanh
          </div>
          <div class="mb-3">
            <span class="qlk-muted small">Tổng giá trị kho hiện tại</span>
            <div class="qlk-stat-value text-success">{{ formatGia(thongKe.tongGiaTri) }}</div>
          </div>
          <div class="mb-3">
            <span class="qlk-muted small">Số lô đã hết hạn sử dụng</span>
            <div class="qlk-stat-value text-danger">{{ thongKe.soLoHetHan }}</div>
          </div>
          <div class="mb-0">
            <span class="qlk-muted small">Số mặt hàng sắp hết tồn kho</span>
            <div class="qlk-stat-value text-warning">{{ thongKe.soMatHangSapHetTon }}</div>
          </div>
        </div>
      </div>
    </div>

    <div class="modal fade" :class="{ show: hienModal }" :style="hienModal ? 'display:block' : ''" tabindex="-1"
      role="dialog" @click.self="hienModal = false">
      <div class="modal-dialog shadow-lg" role="document">
        <div class="modal-content border-0">
          <div class="modal-header bg-primary text-white">
            <h5 class="modal-title">Chỉnh sửa thông tin lô (Admin)</h5>
            <button type="button" class="close text-white" @click="hienModal = false"><span>&times;</span></button>
          </div>
          <div class="modal-body p-4">
            <div class="row">
              <div class="col-md-6 mb-3">
                <label class="small text-muted font-weight-bold">Số lô</label>
                <input class="form-control" v-model="formSua.soLo" />
              </div>
              <div class="col-md-6 mb-3">
                <label class="small text-muted font-weight-bold">Hạn sử dụng</label>
                <input type="date" class="form-control" v-model="formSua.hanSuDung" />
              </div>
              <div class="col-md-6 mb-3">
                <label class="small text-muted font-weight-bold">Số lượng tồn</label>
                <input type="number" min="0" class="form-control" v-model.number="formSua.soLuongTon" />
              </div>
              <div class="col-md-6 mb-3">
                <label class="small text-muted font-weight-bold">Giá nhập</label>
                <input type="number" min="0" class="form-control" v-model.number="formSua.giaNhap" />
              </div>
            </div>
            <p v-if="loiModal" class="text-danger small mt-2"><i class="fas fa-exclamation-triangle mr-1"></i> {{ loiModal }}</p>
          </div>
          <div class="modal-footer bg-light">
            <button type="button" class="btn btn-secondary" @click="hienModal = false">Hủy</button>
            <button type="button" class="btn btn-primary px-4" :disabled="dangLuu" @click="luuSuaLo">
              <i class="fas fa-save mr-1"></i> {{ dangLuu ? 'Đang lưu...' : 'Lưu thay đổi' }}
            </button>
          </div>
        </div>
      </div>
    </div>
    <div v-if="hienModal" class="modal-backdrop fade show"></div>

    <!-- MODAL CHI TIẾT THUỐC CÙNG LÔ -->
    <div v-if="hienModalLo" class="custom-modal-overlay">
      <div class="custom-modal-content" style="width: 750px;">
        <div class="modal-header bg-dark text-white">
          <h5 class="m-0">Lô hàng: {{ soLoHienTai }}</h5>
          <button class="btn-close-white" @click="hienModalLo = false">&times;</button>
        </div>
        <div class="modal-body p-0">
          <div class="p-3 bg-light border-bottom">
            <span class="small text-muted">Danh sách các loại thuốc sử dụng chung số lô này</span>
          </div>
          <div class="table-responsive">
            <table class="table table-sm table-hover m-0">
              <thead class="bg-white">
                <tr>
                  <th class="pl-3">Tên thuốc</th>
                  <th>Hạn dùng</th>
                  <th class="text-right">Tồn kho</th>
                  <th class="text-right pr-3">Giá nhập</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(item, idx) in danhSachCungLo" :key="idx">
                  <td class="pl-3 py-2">
                    <i class="fas fa-capsules text-primary mr-1"></i>
                    <strong>{{ item.tenThuoc }}</strong>
                  </td>
                  <td>{{ item.hanSuDung }}</td>
                  <td class="text-right text-info font-weight-bold">{{ item.soLuongTon }}</td>
                  <td class="text-right pr-3">{{ formatGia(item.giaNhap) }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary px-4" @click="hienModalLo = false">Đóng</button>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import axiosClient from '../../api/axiosClient';

const props = defineProps({ isAdmin: { type: Boolean, default: false } });

const danhSach     = ref([]);
const dangTai      = ref(false);
const tuKhoa       = ref('');
const locThang     = ref('');
const locTrangThai = ref('all');
const thongKe      = ref({ tongGiaTri: 0, soLoHetHan: 0, soLoSapHetHan: 0, soMatHangSapHetTon: 0 });

const hienThongKe  = ref(false); // BIẾN ĐIỀU KHIỂN CÁCH 2: Mặc định ẩn thống kê

const hienModal = ref(false);
const dangLuu   = ref(false);
const loiModal  = ref('');
const formSua   = ref({ maLo: null, soLo: '', hanSuDung: '', soLuongTon: 0, giaNhap: 0 });

// ── PHÂN TRANG ──
const trangHienTai   = ref(1);
const soDongMoiTrang = ref(10);

const tongSoTrang = computed(() => Math.ceil(danhSach.value.length / soDongMoiTrang.value));
const batDau = computed(() => (trangHienTai.value - 1) * soDongMoiTrang.value);
const ketThuc = computed(() => Math.min(batDau.value + soDongMoiTrang.value, danhSach.value.length));
const danhSachHienThi = computed(() => danhSach.value.slice(batDau.value, ketThuc.value));

const danhSachTrang = computed(() => {
  const total = tongSoTrang.value;
  const current = trangHienTai.value;
  const start = Math.max(1, current - 2);
  const end   = Math.min(total, current + 2);
  const range = [];
  for (let i = start; i <= end; i++) range.push(i);
  return range;
});

const loadData = async () => {
  dangTai.value = true;
  try {
    const data = await axiosClient.get('/Kho/danh-sach-lo', {
      params: {
        search: tuKhoa.value     || undefined,
        thang:  locThang.value   || undefined,
        loai:   locTrangThai.value !== 'all' ? locTrangThai.value : undefined,
      },
    });
    danhSach.value = data?.items ?? [];
    thongKe.value  = data?.thongKe ?? { tongGiaTri: 0, soLoHetHan: 0, soMatHangSapHetTon: 0 };
  } catch (err) {
    console.error('Lỗi tải lô hàng:', err);
    danhSach.value = [];
  } finally {
    dangTai.value = false;
  }
};

const onFilter = () => {
  trangHienTai.value = 1;
  loadData();
};

const laHetHan    = (lo) => lo.hanSuDung && new Date(lo.hanSuDung) < new Date();
const laSapHetHan = (lo) => {
  if (!lo.hanSuDung) return false;
  const d = new Date(lo.hanSuDung);
  const sau6Thang = new Date();
  sau6Thang.setMonth(sau6Thang.getMonth() + 6);
  return d >= new Date() && d <= sau6Thang;
};

const rowClass = (lo) => ({
  'qlk-row--expired': laHetHan(lo),
  'qlk-row--warn':    laSapHetHan(lo),
});

const moModalSua = (lo) => {
  loiModal.value = '';
  const dateStr = lo.hanSuDung ? new Date(lo.hanSuDung).toISOString().split('T')[0] : '';
  formSua.value  = { maLo: lo.maLo, soLo: lo.soLo, hanSuDung: dateStr, soLuongTon: lo.soLuongTon, giaNhap: lo.giaNhap };
  hienModal.value = true;
};

const luuSuaLo = async () => {
  if (!formSua.value.soLo) { loiModal.value = 'Vui lòng nhập số lô.'; return; }
  loiModal.value = '';
  dangLuu.value  = true;
  try {
    await axiosClient.put(`/Kho/lo-hang/${formSua.value.maLo}`, formSua.value);
    hienModal.value = false;
    loadData();
  } catch (err) {
    loiModal.value = err.response?.data?.message || err.message || 'Có lỗi xảy ra.';
  } finally {
    dangLuu.value = false;
  }
};

const formatGia = (v) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v ?? 0);

const hienModalLo = ref(false);
const soLoHienTai = ref('');
const danhSachCungLo = ref([]);

const xemThuocCungLo = (soLo) => {
  soLoHienTai.value = soLo;
  // Lọc từ danh sách hiện tại (nếu muốn check toàn bộ hệ thống thì gọi API)
  danhSachCungLo.value = danhSach.value.filter(l => l.soLo === soLo);
  hienModalLo.value = true;
};

onMounted(loadData);
</script>

<style scoped>
.transition-layout {
  transition: all 0.3s ease-in-out;
}
/* CSS để các dòng có màu sắc cảnh báo */
.qlk-row--expired {
  background-color: #fff5f5 !important;
}
.qlk-row--warn {
  background-color: #fffdf0 !important;
}
.qlk-stat-card {
  padding: 1.25rem;
  background: #fff;
  border-radius: 0.5rem;
  border-left: 4px solid #4e73df;
}
.qlk-stat-value {
  font-size: 1.5rem;
  font-weight: 700;
}
</style>
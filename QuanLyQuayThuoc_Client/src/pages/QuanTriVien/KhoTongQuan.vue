<template>
  <section>
    <div class="qlk-filters-card card mb-3">
      <div class="card-header py-3 d-flex justify-content-between align-items-center bg-white">
        <h6 class="m-0 font-weight-bold text-primary">1. Tổng quan tồn kho</h6>
        <button class="btn btn-outline-primary btn-sm shadow-sm" @click="hienThongKe = !hienThongKe">
          <i class="fas" :class="hienThongKe ? 'fa-expand-alt' : 'fa-chart-pie'"></i>
          {{ hienThongKe ? ' Phóng to bảng (Ẩn thống kê)' : ' Xem thống kê nhanh' }}
        </button>
      </div>
      <div class="card-body">
        <div class="row">
          <div class="col-md-4 mb-3">
            <label class="small text-muted font-weight-bold">Danh mục sản phẩm</label>
            <select class="form-control" v-model="locDanhMuc" @change="onFilter">
              <option value="">— Tất cả danh mục —</option>
              <option v-for="dm in danhSachDanhMuc" :key="dm.maDanhMuc" :value="dm.maDanhMuc">
                {{ dm.tenDanhMuc }}
              </option>
            </select>
          </div>
          <div class="col-md-4 mb-3">
            <label class="small text-muted font-weight-bold">Tìm theo tên / hoạt chất</label>
            <input class="form-control" v-model="tuKhoa" placeholder="Ví dụ: Smecta / Diosmectite"
              @input="onFilter" />
          </div>
          <div class="col-md-4 mb-3">
            <label class="small text-muted font-weight-bold w-100">Số dòng mỗi trang</label>
            <select class="form-control" v-model="soDongMoiTrang" @change="trangHienTai = 1">
              <option :value="10">10 dòng</option>
              <option :value="20">20 dòng</option>
              <option :value="50">50 dòng</option>
            </select>
          </div>
        </div>
      </div>
    </div>

    <div class="row">
      <div :class="hienThongKe ? 'col-lg-8' : 'col-lg-12'" class="mb-3 transition-layout">
        <div class="card shadow-sm">
          <div class="card-header py-3 d-flex justify-content-between align-items-center">
            <div class="font-weight-bold text-primary">Danh sách sản phẩm tồn kho</div>
            <span class="badge badge-primary px-3">{{ danhSach.length }} mặt hàng</span>
          </div>
          <div class="card-body p-0">
            <div v-if="dangTai" class="text-center py-5">
              <div class="spinner-border text-primary" role="status"></div>
              <div class="mt-2 small text-muted">Đang truy xuất dữ liệu tồn kho...</div>
            </div>
            <div v-else class="table-responsive">
              <table class="table table-bordered table-hover mb-0 qlk-table">
                <thead class="thead-light">
                  <tr>
                    <th width="120">Mã thuốc</th>
                    <th>Tên thuốc</th>
                    <th>Danh mục</th>
                    <th class="text-center">Tổng tồn</th>
                    <th class="text-center">Trạng thái</th>
                    <!-- Thêm cột mã vạch -->
                    <th class="text-center" width="90">Mã vạch</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="sp in danhSachHienThi" :key="sp.maThuoc"
                    :class="sp.tongTon < 50 ? 'qlk-row--warn' : ''">
                    <td class="font-weight-bold text-secondary">#{{ sp.maThuoc }}</td>
                    <td class="font-weight-bold">{{ sp.tenThuoc }}</td>
                    <td>{{ sp.tenDanhMuc }}</td>
                    <td class="text-center font-weight-bold" :class="sp.tongTon < 50 ? 'text-danger' : 'text-primary'">
                      {{ sp.tongTon }}
                    </td>
                    <td class="text-center">
                      <span v-if="sp.tongTon === 0" class="badge badge-danger">Hết hàng</span>
                      <span v-else-if="sp.tongTon < 50" class="badge badge-warning text-dark">Sắp hết</span>
                      <span v-else class="badge badge-success">Còn hàng</span>
                    </td>
                    <!-- Nút xem mã vạch -->
                    <td class="text-center">
                      <button class="btn btn-outline-secondary btn-sm" 
                              :disabled="dangTaiMaVach === sp.maThuoc"
                              @click="xemMaVach(sp.maThuoc)"
                              title="Xem & In mã vạch">
                        <i class="fas" :class="dangTaiMaVach === sp.maThuoc ? 'fa-spinner fa-spin' : 'fa-barcode'"></i>
                      </button>
                    </td>
                  </tr>
                  <tr v-if="danhSachHienThi.length === 0">
                    <td colspan="6" class="text-center text-muted py-5">
                      <i class="fas fa-search fa-2x mb-2 d-block"></i>
                      Không tìm thấy sản phẩm nào phù hợp.
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <div v-if="tongSoTrang > 1" class="d-flex justify-content-between align-items-center px-3 py-2 border-top bg-light">
              <div class="small text-muted font-italic">
                Hiển thị {{ batDau + 1 }}–{{ ketThuc }} / {{ danhSach.length }} dòng
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
                  <a class="page-link" href="#" @click.prevent="trangHienTai = trang">
                    {{ trang }}
                  </a>
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
        <div class="qlk-stat-card shadow-sm border-left-primary bg-white">
          <div class="font-weight-bold text-primary mb-3 border-bottom pb-2">
            <i class="fas fa-chart-bar mr-2"></i>Thống kê tổng quan
          </div>
          <div class="mb-3">
            <span class="qlk-muted small text-uppercase font-weight-bold">Tổng giá trị tồn kho</span>
            <div class="qlk-stat-value font- text-success">{{ formatGia(thongKe?.tongGiaTri) }}</div>
          </div>
          <div class="mb-3">
            <span class="qlk-muted small text-uppercase font-weight-bold">Lô thuốc đã hết hạn</span>
            <div class="qlk-stat-value text-danger">{{ thongKe?.soLoHetHan || 0 }}</div>
          </div>
          <div class="mb-0">
            <span class="qlk-muted small text-uppercase font-weight-bold">Mặt hàng dưới ngưỡng tồn</span>
            <div class="qlk-stat-value text-warning">{{ thongKe?.soMatHangSapHetTon || 0 }}</div>
          </div>
        </div>
      </div>
    </div>

    <!-- MODAL XEM & IN MÃ VẠCH -->
    <div v-if="hienModalMaVach" class="modal-overlay" @click.self="hienModalMaVach = false">
      <div class="modal-box">
        <div class="modal-header bg-dark text-white">
          <h6 class="m-0">Mã vạch — {{ tenThuocDangXem }}</h6>
          <button class="btn-close-white" @click="hienModalMaVach = false">&times;</button>
        </div>
        <div class="modal-body p-3" id="vung-in-mavach">
          <div v-if="danhSachMaVach.length === 0" class="text-center text-muted py-4">
            Thuốc này chưa có mã vạch nào. Hãy nhập kho trước.
          </div>
          <div v-else class="barcode-grid">
            <div v-for="(mv, i) in danhSachMaVach" :key="i" class="barcode-item">
              <div class="tem-ten-thuoc">{{ mv.tenThuoc }}</div>
              <div class="small text-muted mb-1">{{ mv.tenDonVi }}</div>
              <img :src="mv.hinhAnhMaVach" class="tem-image" v-if="mv.hinhAnhMaVach" />
              <div class="tem-ma-so">{{ mv.maVach }}</div>
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary btn-sm" @click="hienModalMaVach = false">Đóng</button>
          <button class="btn btn-primary btn-sm px-4" @click="inMaVach" :disabled="danhSachMaVach.length === 0">
            <i class="fas fa-print mr-1"></i> In
          </button>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import axiosClient from '../../api/axiosClient';

const tuKhoa       = ref('');
const locDanhMuc   = ref('');
const danhSach     = ref([]);
const danhSachDanhMuc = ref([]);
const dangTai      = ref(false);
const hienThongKe  = ref(false);

const thongKe = ref({
  tongGiaTri: 0,
  soLoHetHan: 0,
  soLoSapHetHan: 0,
  soMatHangSapHetTon: 0
});

// ── PHÂN TRANG ──────────────────────────────────
const trangHienTai   = ref(1);
const soDongMoiTrang = ref(10);

const tongSoTrang = computed(() =>
  Math.ceil(danhSach.value.length / soDongMoiTrang.value)
);
const batDau = computed(() => (trangHienTai.value - 1) * soDongMoiTrang.value);
const ketThuc = computed(() =>
  Math.min(batDau.value + soDongMoiTrang.value, danhSach.value.length)
);
const danhSachHienThi = computed(() =>
  danhSach.value.slice(batDau.value, ketThuc.value)
);
const danhSachTrang = computed(() => {
  const total = tongSoTrang.value;
  const current = trangHienTai.value;
  const delta = 2;
  const range = [];
  for (let i = Math.max(1, current - delta); i <= Math.min(total, current + delta); i++)
    range.push(i);
  return range;
});

// ── MÃ VẠCH ─────────────────────────────────────
const hienModalMaVach  = ref(false);
const danhSachMaVach   = ref([]);
const tenThuocDangXem  = ref('');
const dangTaiMaVach    = ref(null); // lưu maThuoc đang loading để hiện spinner đúng dòng

const xemMaVach = async (maThuoc) => {
  const sp = danhSach.value.find(x => x.maThuoc === maThuoc);
  tenThuocDangXem.value = sp?.tenThuoc ?? '';
  dangTaiMaVach.value = maThuoc;
  danhSachMaVach.value = [];

  try {
    const data = await axiosClient.get(`/Kho/ma-vach/${maThuoc}`);
    danhSachMaVach.value = Array.isArray(data) ? data : [];
  } catch (err) {
    console.error('Lỗi tải mã vạch:', err);
  } finally {
    dangTaiMaVach.value = null;
    hienModalMaVach.value = true;
  }
};

const inMaVach = () => {
  const noidung = document.getElementById('vung-in-mavach').innerHTML;
  const w = window.open('', '_blank');
  w.document.write(`<html><head><style>
    body { font-family: sans-serif; }
    .barcode-grid { display: flex; flex-wrap: wrap; gap: 10px; justify-content: center; }
    .barcode-item { border: 1px solid #333; padding: 10px; width: 160px; text-align: center; }
    .tem-ten-thuoc { font-size: 11px; font-weight: bold; margin-bottom: 4px; }
    .tem-image { width: 100%; height: auto; }
    .tem-ma-so { font-size: 10px; margin-top: 3px; }
  </style></head><body>${noidung}</body></html>`);
  w.document.close();
  setTimeout(() => { w.print(); w.close(); }, 500);
};

// ── DATA ─────────────────────────────────────────
const loadData = async () => {
  dangTai.value = true;
  try {
    const data = await axiosClient.get('/Kho/tong-quan', {
      params: {
        search:    tuKhoa.value     || undefined,
        maDanhMuc: locDanhMuc.value || undefined,
      }
    });
    danhSach.value = data?.items ?? [];
    thongKe.value  = data?.thongKe ?? thongKe.value;
  } catch (err) {
    console.error('Lỗi tải dữ liệu kho:', err);
    danhSach.value = [];
  } finally {
    dangTai.value = false;
  }
};

const onFilter = () => { trangHienTai.value = 1; loadData(); };

const loadSidebar = async () => {
  try {
    const data = await axiosClient.get('/Kho/danh-muc');
    danhSachDanhMuc.value = Array.isArray(data) ? data : [];
  } catch (err) {
    console.error('Lỗi tải danh mục:', err);
  }
};

const formatGia = (v) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v ?? 0);

onMounted(() => { loadSidebar(); loadData(); });
</script>

<style scoped>
.transition-layout { transition: all 0.3s ease-in-out; }
.qlk-row--warn { background-color: #fffdf0 !important; }
.qlk-stat-card { padding: 1rem; border-radius: 0.5rem; border-left: 5px solid #4e73df; }
.qlk-stat-value { font-size: 1.25rem; font-weight: 800; line-height: 1.2; }
.qlk-muted { color: #858796; letter-spacing: 0.5px; font-size: 0.7rem; text-transform: uppercase; }
.table th { text-transform: uppercase; font-size: 0.75rem; letter-spacing: 0.5px; }

/* Modal */
.modal-overlay {
  position: fixed; inset: 0;
  background: rgba(0,0,0,0.5);
  display: flex; align-items: center; justify-content: center; z-index: 2000;
}
.modal-box { background: white; width: 600px; max-height: 80vh; border-radius: 8px; overflow: hidden; display: flex; flex-direction: column; }
.modal-body { overflow-y: auto; flex: 1; }
.btn-close-white { background: none; border: none; color: white; font-size: 22px; cursor: pointer; }

/* Tem mã vạch */
.barcode-grid { display: flex; flex-wrap: wrap; gap: 12px; justify-content: center; padding: 8px; }
.barcode-item { border: 1px dashed #ccc; padding: 10px; width: 160px; text-align: center; background: #f9f9f9; }
.tem-image { width: 100%; height: auto; margin: 4px 0; }
.tem-ten-thuoc { font-size: 11px; font-weight: bold; color: #333; overflow: hidden; white-space: nowrap; text-overflow: ellipsis; }
.tem-ma-so { font-size: 10px; color: #555; }
</style>
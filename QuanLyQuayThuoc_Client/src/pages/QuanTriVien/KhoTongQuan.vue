<template>
  <section>
    <!-- ═════════════════════════════
         BỘ LỌC TÌM KIẾM
         ═════════════════════════════ -->
    <div class="card border-0 shadow-sm mb-4" style="border-radius: 16px; overflow: hidden;">
      <div class="card-header bg-white py-3 border-0 d-flex justify-content-between align-items-center">
        <h6 class="m-0 font-weight-bold text-dark"><i class="fas fa-warehouse mr-2 text-primary"></i>Kiểm soát tồn kho tổng quan</h6>
        <button class="btn btn-outline-primary btn-sm rounded-pill px-3" @click="hienThongKe = !hienThongKe">
          <i class="fas" :class="hienThongKe ? 'fa-compress-alt' : 'fa-chart-pie'"></i>
          {{ hienThongKe ? ' Ẩn số liệu' : ' Xem thống kê nhanh' }}
        </button>
      </div>
      <div class="card-body bg-light-soft">
        <div class="row">
          <div class="col-md-4 mb-3">
            <label class="small text-muted font-weight-bold">Danh mục thuốc</label>
            <select class="form-control rounded-pill border-primary-light" v-model="locDanhMuc" @change="onFilter">
              <option value="">— Tất cả danh mục —</option>
              <option v-for="dm in danhSachDanhMuc" :key="dm.maDanhMuc" :value="dm.maDanhMuc">
                {{ dm.tenDanhMuc }}
              </option>
            </select>
          </div>
          <div class="col-md-5 mb-3">
            <label class="small text-muted font-weight-bold">Tìm kiếm sản phẩm</label>
            <div class="input-group">
              <div class="input-group-prepend">
                <span class="input-group-text bg-white border-right-0 rounded-left-pill border-primary-light">
                  <i class="fas fa-search text-muted"></i>
                </span>
              </div>
              <input class="form-control border-left-0 rounded-right-pill border-primary-light" v-model="tuKhoa" 
                placeholder="Nhập tên thuốc hoặc hoạt chất..." @input="onFilter" />
            </div>
          </div>
          <div class="col-md-3 mb-3">
            <label class="small text-muted font-weight-bold">Số dòng hiển thị</label>
            <select class="form-control rounded-pill border-primary-light" v-model="soDongMoiTrang" @change="trangHienTai = 1">
              <option :value="10">10 dòng</option>
              <option :value="20">20 dòng</option>
              <option :value="50">50 dòng</option>
            </select>
          </div>
        </div>
      </div>
    </div>

    <!-- ═════════════════════════════
         NỘI DUNG CHÍNH
         ═════════════════════════════ -->
    <div class="row">
      <div :class="hienThongKe ? 'col-lg-8' : 'col-lg-12'" class="mb-4 transition-layout">
        <div class="card border-0 shadow-sm" style="border-radius: 16px;">
          <div class="card-body p-0">
            <div v-if="dangTai" class="text-center py-5">
              <div class="spinner-grow text-primary" role="status"></div>
              <div class="mt-2 small text-muted font-weight-bold">Đang truy xuất dữ liệu kho...</div>
            </div>
            
            <div v-else class="table-responsive rounded-lg">
              <table class="premium-table">
                <thead>
                  <tr>
                    <th width="120">ID Thuốc</th>
                    <th>Tên thuốc</th>
                    <th>Danh mục</th>
                    <th class="text-center">Tổng tồn</th>
                    <th class="text-center">Trạng thái</th>
                    <th class="text-center" width="100">Barcode</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="sp in danhSachHienThi" :key="sp.maThuoc"
                    :class="sp.tongTon < 50 ? 'row--warn' : ''">
                    <td class="font-weight-bold text-muted">#{{ sp.maThuoc }}</td>
                    <td>
                      <div class="d-flex align-items-center">
                        <div class="icon-circle mr-3"><i class="fas fa-capsules"></i></div>
                        <span class="font-weight-bold text-dark">{{ sp.tenThuoc }}</span>
                      </div>
                    </td>
                    <td><span class="text-secondary">{{ sp.tenDanhMuc }}</span></td>
                    <td class="text-center font-weight-bold">
                      <span class="bubble-count" :class="sp.tongTon < 50 ? 'text-danger bg-danger-soft' : 'text-primary'">
                        {{ sp.tongTon }}
                      </span>
                    </td>
                    <td class="text-center">
                      <span v-if="sp.tongTon === 0" class="badge badge-danger">Hết hàng</span>
                      <span v-else-if="sp.tongTon < 50" class="badge badge-warning">Sắp hết</span>
                      <span v-else class="badge badge-success">Còn hàng</span>
                    </td>
                    <td class="text-center">
                      <button class="btn btn-light btn-sm rounded-circle shadow-none border" 
                              style="width: 36px; height: 36px; display: inline-flex; align-items: center; justify-content: center;"
                              :disabled="dangTaiMaVach === sp.maThuoc"
                              @click="xemMaVach(sp.maThuoc)"
                              title="Xem mã vạch">
                        <i class="fas" :class="dangTaiMaVach === sp.maThuoc ? 'fa-spinner fa-spin' : 'fa-barcode'"></i>
                      </button>
                    </td>
                  </tr>
                  <tr v-if="danhSachHienThi.length === 0">
                    <td colspan="6" class="text-center text-muted py-5">
                      <div class="empty-state">
                        <i class="fas fa-search fa-3x mb-3 text-light"></i>
                        <p class="font-weight-bold">Không tìm thấy sản phẩm nào phù hợp</p>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <!-- PHÂN TRANG -->
            <div v-if="tongSoTrang > 1" class="d-flex justify-content-between align-items-center px-4 py-3 bg-white border-top rounded-bottom">
              <div class="small text-muted font-italic">
                Hiển thị <strong>{{ batDau + 1 }}–{{ ketThuc }}</strong> / {{ danhSach.length }}
              </div>
              <ul class="pagination pagination-sm mb-0 custom-pagination">
                <li class="page-item" :class="{ disabled: trangHienTai === 1 }">
                  <a class="page-link" href="#" @click.prevent="trangHienTai = 1"><i class="fas fa-angle-double-left"></i></a>
                </li>
                <li class="page-item" v-for="trang in danhSachTrang" :key="trang" :class="{ active: trang === trangHienTai }">
                  <a class="page-link" href="#" @click.prevent="trangHienTai = trang">{{ trang }}</a>
                </li>
                <li class="page-item" :class="{ disabled: trangHienTai === tongSoTrang }">
                  <a class="page-link" href="#" @click.prevent="trangHienTai = tongSoTrang"><i class="fas fa-angle-double-right"></i></a>
                </li>
              </ul>
            </div>
          </div>
        </div>
      </div>

      <!-- SIDEBAR THỐNG KÊ -->
      <div v-if="hienThongKe" class="col-lg-4">
        <div class="stat-card shadow-sm mb-4 bg-white">
          <h6 class="font-weight-bold text-dark border-bottom pb-3 mb-3">
            <i class="fas fa-chart-pie mr-2 text-primary"></i>Tình hình kho thuốc
          </h6>
          <div class="stat-item mb-4">
            <span class="text-muted small uppercase font-weight-bold">Tổng tài sản tồn kho</span>
            <div class="h4 font-weight-bold text-success mt-1">{{ formatGia(thongKe?.tongGiaTri) }}</div>
            <div class="progress mt-2" style="height: 4px;">
              <div class="progress-bar bg-success" style="width: 100%"></div>
            </div>
          </div>
          <div class="stat-item mb-4">
            <span class="text-muted small uppercase font-weight-bold">Lô thuốc quá hạn hsd</span>
            <div class="h4 font-weight-bold text-danger mt-1">{{ thongKe?.soLoHetHan || 0 }} <small class="text-muted" style="font-size: 14px;">lô</small></div>
            <div class="progress mt-2" style="height: 4px;">
              <div class="progress-bar bg-danger" style="width: 35%"></div>
            </div>
          </div>
          <div class="stat-item">
            <span class="text-muted small uppercase font-weight-bold">Mặt hàng dưới ngưỡng tồn</span>
            <div class="h4 font-weight-bold text-warning mt-1">{{ thongKe?.soMatHangSapHetTon || 0 }} <small class="text-muted" style="font-size: 14px;">sản phẩm</small></div>
            <div class="progress mt-2" style="height: 4px;">
              <div class="progress-bar bg-warning" style="width: 60%"></div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- MODAL XEM & IN MÃ VẠCH -->
    <div v-if="hienModalMaVach" class="custom-modal-overlay">
      <div class="custom-modal-content" style="width: 700px;">
        <div class="modal-header">
          <h5 class="m-0"><i class="fas fa-barcode mr-2"></i>Mã vạch: {{ tenThuocDangXem }}</h5>
          <button class="btn-close-white" @click="hienModalMaVach = false"><i class="fas fa-times"></i></button>
        </div>
        <div class="modal-body p-4 bg-light-soft" id="vung-in-mavach">
          <div v-if="danhSachMaVach.length === 0" class="text-center text-muted py-5 empty-state">
            <i class="fas fa-ghost fa-2x mb-2"></i>
            <p>Chưa có dữ liệu tem nhãn cho loại thuốc này.</p>
          </div>
          <div v-else class="barcode-grid">
            <div v-for="(mv, i) in danhSachMaVach" :key="i" class="barcode-item">
              <div class="tem-ten-thuoc">{{ mv.tenThuoc }}</div>
              <div class="small text-muted mb-2 font-weight-bold">{{ mv.tenDonVi }}</div>
              <div class="tem-image-container">
                <img :src="mv.hinhAnhMaVach" class="tem-image" v-if="mv.hinhAnhMaVach" />
              </div>
              <div class="tem-ma-so mt-2">{{ mv.maVach }}</div>
            </div>
          </div>
        </div>
        <div class="modal-footer bg-white border-top-0">
          <button class="btn btn-light px-4 border" @click="hienModalMaVach = false">Đóng</button>
          <button class="btn btn-warning px-4 shadow-sm" @click="inMaVach" :disabled="danhSachMaVach.length === 0">
            <i class="fas fa-print mr-1"></i> In tất cả tem
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
const dangTaiMaVach    = ref(null); 

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
    body { font-family: sans-serif; padding: 20px; }
    .barcode-grid { display: flex; flex-wrap: wrap; gap: 15px; justify-content: center; }
    .barcode-item { border: 1px solid #ddd; padding: 12px; width: 180px; text-align: center; background: #fff; }
    .tem-ten-thuoc { font-size: 11px; font-weight: bold; margin-bottom: 4px; text-transform: uppercase; color: #1e293b; }
    .tem-image { width: 100%; height: auto; }
    .tem-ma-so { font-size: 10px; margin-top: 5px; color: #64748b; font-family: monospace; }
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
/* ══════════════════════════════════════════════
    PREMIUM UI DESIGN BY ANTIGRAVITY - KHO TỔNG QUAN
   ══════════════════════════════════════════════ */

.transition-layout { transition: all 0.35s cubic-bezier(0.4, 0, 0.2, 1); }
.bg-light-soft { background-color: #f8fafc; }
.border-primary-light { border-color: #e0e7ff; }

/* Modal Premium */
.custom-modal-overlay {
  position: fixed;
  top: 0; left: 0; width: 100%; height: 100%;
  background: rgba(15, 23, 42, 0.6);
  backdrop-filter: blur(8px);
  display: flex; align-items: center; justify-content: center;
  z-index: 2000;
  animation: fadeIn 0.3s ease;
}

.custom-modal-content {
  background: #ffffff;
  border-radius: 24px;
  overflow: hidden;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
  animation: slideUp 0.4s cubic-bezier(0.16, 1, 0.3, 1);
}

.modal-header {
  padding: 1.5rem;
  background: linear-gradient(135deg, #1e293b 0%, #334155 100%);
  color: white;
  border: none;
}

.btn-close-white {
  background: none; border: none; color: rgba(255, 255, 255, 0.8);
  font-size: 24px; transition: color 0.2s;
}
.btn-close-white:hover { color: white; }

/* Premium Table */
.premium-table {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
  border-bottom: 1px solid #e2e8f0;
}

.premium-table thead th {
  background: #f8fafc !important;
  color: #475569 !important;
  font-weight: 700;
  text-transform: uppercase;
  font-size: 0.7rem;
  letter-spacing: 0.05em;
  padding: 16px;
  border-bottom: 2px solid #e2e8f0;
}

.premium-table tbody td {
  padding: 16px;
  vertical-align: middle;
  border-bottom: 1px solid #f1f5f9;
  color: #334155;
  font-size: 0.9rem;
}

.premium-table tbody tr:hover { background-color: #f8fafc; }

/* Row Warning */
.row--warn { background-color: #fffbeb !important; }

/* Barcode Items */
.barcode-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 20px;
}

.barcode-item {
  background: #fff;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 16px;
  text-align: center;
  transition: all 0.2s;
}

.barcode-item:hover { transform: translateY(-3px); box-shadow: 0 10px 15px -3px rgba(0,0,0,0.1); }

.tem-ten-thuoc { font-size: 0.75rem; font-weight: 800; color: #1e293b; margin-bottom: 8px; text-transform: uppercase; }
.tem-image-container { background: #fff; padding: 4px; border-radius: 4px; }
.tem-image { width: 100%; height: 45px; object-fit: contain; }
.tem-ma-so { font-family: monospace; font-size: 0.8rem; color: #64748b; }

/* Badges */
.badge { padding: 6px 12px; border-radius: 8px; font-weight: 700; font-size: 0.65rem; text-transform: uppercase; }
.badge-danger { background: #fee2e2 !important; color: #b91c1c !important; }
.badge-warning { background: #fef3c7 !important; color: #92400e !important; }
.badge-success { background: #dcfce7 !important; color: #15803d !important; }

.bubble-count { padding: 6px 14px; border-radius: 999px; background: #f1f5f9; font-size: 0.85rem; }
.bg-danger-soft { background: #fef2f2 !important; }

/* Stats Card */
.stat-card { border-radius: 20px; padding: 1.5rem; border: 1px solid #eef2f6; }

/* Icon Circle */
.icon-circle {
  width: 38px; height: 38px; background: #f1f5f9; border-radius: 10px;
  display: flex; align-items: center; justify-content: center; color: #3b82f6;
}

/* Pagination */
.custom-pagination .page-link { border: none; background: transparent; color: #64748b; font-weight: 600; margin: 0 2px; border-radius: 8px !important; }
.custom-pagination .page-item.active .page-link { background: #3b82f6 !important; color: white !important; }

/* Animations */
@keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }
@keyframes slideUp { from { transform: translateY(20px); opacity: 0; } to { transform: translateY(0); opacity: 1; } }

.rounded-left-pill { border-top-left-radius: 50px; border-bottom-left-radius: 50px; }
.rounded-right-pill { border-top-right-radius: 50px; border-bottom-right-radius: 50px; }
.rounded-lg { border-radius: 16px; }
</style>
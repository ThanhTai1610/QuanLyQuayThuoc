<template>
  <section>
    <!-- ═════════════════════════════
         BỘ LỌC TÌM KIẾM
         ═════════════════════════════ -->
    <div class="card border-0 shadow-sm mb-4" style="border-radius: 16px; overflow: hidden;">
      <div class="card-header bg-white py-3 border-0 d-flex justify-content-between align-items-center">
        <h6 class="m-0 font-weight-bold text-dark"><i class="fas fa-filter mr-2 text-primary"></i>Bộ lọc lô hàng</h6>
        <button class="btn btn-outline-primary btn-sm rounded-pill px-3" @click="hienThongKe = !hienThongKe">
          <i class="fas" :class="hienThongKe ? 'fa-compress-alt' : 'fa-chart-pie'"></i>
          {{ hienThongKe ? ' Ẩn thống kê' : ' Xem thống kê nhanh' }}
        </button>
      </div>
      <div class="card-body bg-light-soft">
        <div class="row">
          <div class="col-md-4 mb-3">
            <label class="small text-muted font-weight-bold">Tên thuốc / Hoạt chất</label>
            <div class="input-group">
              <div class="input-group-prepend">
                <span class="input-group-text bg-white border-right-0 rounded-left-pill"><i class="fas fa-search text-muted"></i></span>
              </div>
              <input class="form-control border-left-0 rounded-right-pill" v-model="tuKhoa" @input="onFilter"
                placeholder="Tìm kiếm nhanh..." />
            </div>
          </div>
          <div class="col-md-3 mb-3">
            <label class="small text-muted font-weight-bold">Thời gian hết hạn</label>
            <input type="month" class="form-control rounded-pill" v-model="locThang" @change="onFilter" />
          </div>
          <div class="col-md-3 mb-3">
            <label class="small text-muted font-weight-bold">Trạng thái lô</label>
            <select class="form-control rounded-pill" v-model="locTrangThai" @change="onFilter">
              <option value="all">Tất cả lô hàng</option>
              <option value="expired">Đã hết hạn sử dụng</option>
              <option value="soon">Sắp hết hạn (< 6 tháng)</option>
            </select>
          </div>
          <div class="col-md-2 mb-3">
            <label class="small text-muted font-weight-bold">Hiển thị</label>
            <select class="form-control rounded-pill" v-model="soDongMoiTrang" @change="trangHienTai = 1">
              <option :value="10">10 dòng</option>
              <option :value="20">20 dòng</option>
              <option :value="50">50 dòng</option>
            </select>
          </div>
        </div>
      </div>
    </div>

    <!-- ═════════════════════════════
         DANH SÁCH VÀ THỐNG KÊ
         ═════════════════════════════ -->
    <div class="row">
      <div :class="hienThongKe ? 'col-lg-8' : 'col-lg-12'" class="mb-4 transition-layout">
        <div class="card border-0 shadow-sm" style="border-radius: 16px;">
          <div class="card-body p-0">
            <div v-if="dangTai" class="text-center py-5">
              <div class="spinner-grow text-primary" role="status"></div>
              <div class="mt-2 small text-muted font-weight-bold">Đang tải dữ liệu lô hàng...</div>
            </div>
            
            <div v-else class="table-responsive rounded-lg">
              <table class="premium-table">
                <thead>
                  <tr>
                    <th>Số lô</th>
                    <th>Hạn sử dụng</th>
                    <th>Ngày nhập</th>
                    <th class="text-center">Tồn lô</th>
                    <th>Giá nhập</th>
                    <th>Tên Thuốc</th>
                    <th v-if="isAdmin" class="text-center">Quản lý</th>
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
                      <div class="d-flex align-items-center">
                        <span class="mr-2">{{ lo.hanSuDung }}</span>
                        <span v-if="laHetHan(lo)" class="badge badge-danger">Hết hạn</span>
                        <span v-else-if="laSapHetHan(lo)" class="badge badge-warning">Sắp hết</span>
                      </div>
                    </td>
                    <td>{{ lo.ngaySanXuat || lo.ngayNhap }}</td>
                    <td class="text-center">
                      <span class="font-weight-bold text-dark bubble-count">{{ lo.soLuongTon }}</span>
                    </td>
                    <td class="font-weight-bold">{{ formatGia(lo.giaNhap) }}</td>
                    <td class="text-dark">{{ lo.tenThuoc }}</td>
                    <td v-if="isAdmin" class="text-center">
                      <button class="btn btn-warning btn-sm shadow-sm px-3" @click="moModalSua(lo)">
                        <i class="fas fa-edit mr-1"></i> Sửa
                      </button>
                    </td>
                  </tr>
                  <tr v-if="danhSachHienThi.length === 0">
                    <td :colspan="isAdmin ? 7 : 6" class="text-center text-muted py-5">
                      <div class="empty-state">
                        <i class="fas fa-box-open fa-3x mb-3 text-light"></i>
                        <p class="font-weight-bold">Không tìm thấy lô hàng nào</p>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <!-- PHÂN TRANG -->
            <div v-if="tongSoTrang > 1" class="d-flex justify-content-between align-items-center px-4 py-3 bg-white border-top rounded-bottom">
              <div class="small text-muted">
                Dòng <strong>{{ batDau + 1 }}</strong> đến <strong>{{ ketThuc }}</strong> / {{ danhSach.length }}
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
        <div class="stat-card shadow-sm mb-4">
          <h6 class="font-weight-bold text-dark border-bottom pb-3 mb-3">
            <i class="fas fa-chart-line mr-2 text-primary"></i>Thống kê tổng quan
          </h6>
          <div class="stat-item mb-4">
            <span class="text-muted small uppercase font-weight-bold">Tổng vốn tồn kho</span>
            <div class="h4 font-weight-bold text-success mt-1">{{ formatGia(thongKe.tongGiaTri) }}</div>
            <div class="progress mt-2" style="height: 4px;">
              <div class="progress-bar bg-success" style="width: 100%"></div>
            </div>
          </div>
          <div class="stat-item mb-4">
            <span class="text-muted small uppercase font-weight-bold">Lô hàng hết hạn</span>
            <div class="h4 font-weight-bold text-danger mt-1">{{ thongKe.soLoHetHan }} <small class="text-muted" style="font-size: 14px;">lô</small></div>
            <div class="progress mt-2" style="height: 4px;">
              <div class="progress-bar bg-danger" :style="{ width: (thongKe.soLoHetHan / (danhSach.length || 1) * 100) + '%' }"></div>
            </div>
          </div>
          <div class="stat-item">
            <span class="text-muted small uppercase font-weight-bold">Sắp hết tồn kho</span>
            <div class="h4 font-weight-bold text-warning mt-1">{{ thongKe.soMatHangSapHetTon }} <small class="text-muted" style="font-size: 14px;">mặt hàng</small></div>
            <div class="progress mt-2" style="height: 4px;">
              <div class="progress-bar bg-warning" style="width: 40%"></div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- MODAL CHI TIẾT THUỐC CÙNG LÔ -->
    <div v-if="hienModalLo" class="custom-modal-overlay">
      <div class="custom-modal-content" style="width: 800px;">
        <div class="modal-header">
          <h5 class="m-0"><i class="fas fa-boxes mr-2"></i>Chi tiết thuốc cùng số lô: {{ soLoHienTai }}</h5>
          <button class="btn-close-white" @click="hienModalLo = false"><i class="fas fa-times"></i></button>
        </div>
        <div class="modal-body p-0">
          <div class="p-3 bg-light-soft border-bottom">
            <span class="small font-weight-bold text-muted">Danh sách thuốc đang sử dụng chung số lô này trong kho</span>
          </div>
          <div class="table-responsive rounded-bottom">
            <table class="premium-table shadow-none border-0">
              <thead>
                <tr>
                  <th class="pl-4">Tên thuốc</th>
                  <th>Hạn dùng</th>
                  <th class="text-right">Tồn kho</th>
                  <th class="text-right pr-4">Giá nhập</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(item, idx) in danhSachCungLo" :key="idx">
                  <td class="pl-4 py-3">
                    <div class="d-flex align-items-center">
                      <div class="icon-circle mr-2"><i class="fas fa-capsules"></i></div>
                      <strong class="text-dark">{{ item.tenThuoc }}</strong>
                    </div>
                  </td>
                  <td><span class="text-muted">{{ item.hanSuDung }}</span></td>
                  <td class="text-right"><span class="badge badge-info">{{ item.soLuongTon }}</span></td>
                  <td class="text-right font-weight-bold pr-4">{{ formatGia(item.giaNhap) }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
        <div class="modal-footer bg-light-soft border-top-0">
          <button class="btn btn-secondary px-5 shadow-sm" @click="hienModalLo = false">Đóng cửa sổ</button>
        </div>
      </div>
    </div>

    <!-- MODAL SỬA LÔ (ADMIN) -->
    <div v-if="hienModal" class="custom-modal-overlay">
      <div class="custom-modal-content" style="width: 500px;">
        <div class="modal-header" style="background: linear-gradient(135deg, #4f46e5 0%, #3730a3 100%);">
          <h5 class="m-0 text-white"><i class="fas fa-edit mr-2"></i>Chỉnh sửa lô hàng</h5>
          <button class="btn-close-white" @click="hienModal = false"><i class="fas fa-times"></i></button>
        </div>
        <div class="modal-body p-4">
          <div class="form-group mb-3">
            <label class="small text-muted font-weight-bold">Số lô hàng</label>
            <input class="form-control rounded-lg shadow-none border-primary-light" v-model="formSua.soLo" />
          </div>
          <div class="row">
            <div class="col-6 mb-3">
              <label class="small text-muted font-weight-bold">Hạn sử dụng</label>
              <input type="date" class="form-control rounded-lg shadow-none" v-model="formSua.hanSuDung" />
            </div>
            <div class="col-6 mb-3">
              <label class="small text-muted font-weight-bold">Số lượng tồn</label>
              <input type="number" class="form-control rounded-lg shadow-none" v-model.number="formSua.soLuongTon" />
            </div>
            <div class="col-12">
              <label class="small text-muted font-weight-bold">Giá nhập đơn vị</label>
              <div class="input-group">
                <input type="number" class="form-control border-right-0 rounded-left-lg shadow-none" v-model.number="formSua.giaNhap" />
                <div class="input-group-append">
                  <span class="input-group-text bg-white border-left-0 rounded-right-lg">VNĐ</span>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="modal-footer border-top-0">
          <button class="btn btn-light px-4 border" @click="hienModal = false">Bỏ qua</button>
          <button class="btn btn-primary px-4 shadow-sm" :disabled="dangLuu" @click="luuSuaLo">
            <i class="fas fa-save mr-1"></i> {{ dangLuu ? 'Đang lưu...' : 'Lưu lại' }}
          </button>
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

const hienThongKe  = ref(false); 

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
  'row--expired': laHetHan(lo),
  'row--warn':    laSapHetHan(lo),
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
  danhSachCungLo.value = danhSach.value.filter(l => l.soLo === soLo);
  hienModalLo.value = true;
};

onMounted(loadData);
</script>

<style scoped>
/* ══════════════════════════════════════════════
    PREMIUM UI DESIGN BY ANTIGRAVITY - KHO LÔ HÀNG
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
  border-bottom: none;
  background: linear-gradient(135deg, #1e293b 0%, #334155 100%);
  color: white;
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
  border: 1px solid #e2e8f0;
}

.premium-table thead th {
  background: #f8fafc !important;
  color: #475569 !important;
  font-weight: 700;
  text-transform: uppercase;
  font-size: 0.7rem;
  letter-spacing: 0.05em;
  padding: 14px 16px;
  border-bottom: 2px solid #e2e8f0;
}

.premium-table tbody td {
  padding: 14px 16px;
  vertical-align: middle;
  border-bottom: 1px solid #f1f5f9;
  color: #334155;
  font-size: 0.9rem;
}

.premium-table tbody tr:hover { background-color: #f8fafc; }

/* Row Colors */
.row--expired { background-color: #fef2f2 !important; }
.row--warn { background-color: #fffbeb !important; }

/* Stats Card */
.stat-card {
  background: #fff;
  border-radius: 20px;
  padding: 1.5rem;
  border: 1px solid #eef2f6;
}

.stat-item .h4 { letter-spacing: -0.02em; }

/* Badges */
.badge {
  padding: 6px 12px;
  border-radius: 8px;
  font-weight: 700;
  font-size: 0.65rem;
  text-transform: uppercase;
}
.badge-primary { background: #e0e7ff; color: #4338ca; }
.badge-danger { background: #fee2e2; color: #b91c1c; }
.badge-warning { background: #fef3c7; color: #92400e; }
.badge-info { background: #e0f2fe; color: #0369a1; }

.bubble-count {
  background: #f1f5f9;
  padding: 4px 12px;
  border-radius: 999px;
  font-size: 0.85rem;
}

/* Icon Styles */
.icon-circle {
  width: 32px; height: 32px;
  background: #f1f5f9;
  border-radius: 8px;
  display: flex; align-items: center; justify-content: center;
  color: #3b82f6;
  font-size: 0.8rem;
}

/* Pagination */
.custom-pagination .page-link {
  border: none;
  background: transparent;
  color: #64748b;
  font-weight: 600;
  margin: 0 2px;
  border-radius: 8px !important;
}

.custom-pagination .page-item.active .page-link {
  background: #3b82f6 !important;
  color: white !important;
}

/* Animations */
@keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }
@keyframes slideUp { from { transform: translateY(20px); opacity: 0; } to { transform: translateY(0); opacity: 1; } }

.rounded-lg { border-radius: 12px; }
.rounded-left-pill { border-top-left-radius: 50px; border-bottom-left-radius: 50px; }
.rounded-right-pill { border-top-right-radius: 50px; border-bottom-right-radius: 50px; }
.rounded-left-lg { border-top-left-radius: 12px; border-bottom-left-radius: 12px; }
.rounded-right-lg { border-top-right-radius: 12px; border-bottom-right-radius: 12px; }
</style>
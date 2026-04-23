<template>
  <section>
    <!-- ═════════════════════════════
         BỘ LỌC TỐI GIẢN
         ═════════════════════════════ -->
    <div class="card border-0 shadow-sm mb-4" style="border-radius: 12px;">
      <div class="card-body p-3">
        <div class="row align-items-center">
          <div class="col-md-3">
            <h6 class="m-0 font-weight-bold text-dark">
              <i class="fas fa-exclamation-triangle text-danger mr-2"></i>Cảnh báo hạn dùng
            </h6>
          </div>
          <div class="col-md-3 text-right offset-md-6">
             <button class="btn btn-outline-primary btn-sm rounded-pill px-3 shadow-none" @click="hienThongKe = !hienThongKe">
              <i class="fas" :class="hienThongKe ? 'fa-compress-alt' : 'fa-chart-pie'"></i>
              {{ hienThongKe ? ' Ẩn số liệu' : ' Xem thống kê' }}
            </button>
          </div>
        </div>
        <div class="row align-items-center mt-3">
          <div class="col-md-4">
            <label class="small text-muted font-weight-bold mb-1">Lọc theo tháng HSD</label>
            <input type="month" class="form-control form-control-sm rounded-pill px-3" v-model="locThang" @change="onFilter" />
          </div>
          <div class="col-md-4">
            <label class="small text-muted font-weight-bold mb-1">Tình trạng lô hàng</label>
            <select class="form-control form-control-sm rounded-pill px-3" v-model="locLoai" @change="onFilter">
              <option value="all">Tất cả tình trạng</option>
              <option value="expired">Đã hết hạn</option>
              <option value="soon">Sắp hết hạn</option>
            </select>
          </div>
          <div class="col-md-4 text-right mt-4">
            <button class="btn btn-primary btn-sm rounded-pill px-4" @click="onFilter">
              <i class="fas fa-sync-alt mr-1"></i> Làm mới
            </button>
          </div>
        </div>
      </div>
    </div>

    <div class="row">
      <!-- DANH SÁCH BẢNG -->
      <div :class="hienThongKe ? 'col-lg-8' : 'col-lg-12'" class="mb-4 transition-layout">
        <div class="card border-0 shadow-sm" style="border-radius: 12px; overflow: hidden;">
          <div class="card-body p-0">
            <div v-if="dangTai" class="text-center py-5">
              <div class="spinner-grow text-primary" role="status"></div>
            </div>
            
            <div v-else class="table-responsive">
              <table class="premium-table">
                <thead>
                  <tr>
                    <th width="120">Số lô</th>
                    <th>Tên thuốc</th>
                    <th class="text-center">Hạn dùng</th>
                    <th class="text-center">Trạng thái</th>
                    <th class="text-center">Tồn</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="lo in danhSachHienThi" :key="lo.maLo">
                    <td class="font-weight-bold text-muted">{{ lo.soLo }}</td>
                    <td class="font-weight-bold text-dark">{{ lo.tenThuoc }}</td>
                    <td class="text-center text-secondary">{{ lo.hanSuDung }}</td>
                    <td class="text-center">
                      <span v-if="laHetHan(lo)" class="badge-custom bg-danger-soft text-danger">Đã hết hạn</span>
                      <span v-else-if="laSapHetHan(lo)" class="badge-custom bg-warning-soft text-warning">Sắp hết hạn</span>
                      <span v-else class="badge-custom bg-success-soft text-success">Bình thường</span>
                    </td>
                    <td class="text-center">
                      <span class="bubble-ton">{{ lo.soLuongTon }}</span>
                    </td>
                  </tr>
                  <tr v-if="danhSachHienThi.length === 0">
                    <td colspan="5" class="text-center text-muted py-5">
                      Không có cảnh báo nào cần xử lý.
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <!-- PHÂN TRANG -->
            <div v-if="tongSoTrang > 1" class="d-flex justify-content-between align-items-center px-4 py-3 bg-white border-top">
              <div class="small text-muted font-italic">
                Kết quả <strong>{{ batDau + 1 }}–{{ ketThuc }}</strong> trên {{ danhSach.length }}
              </div>
              <ul class="pagination pagination-sm mb-0 custom-pagination">
                <li class="page-item" :class="{ disabled: trangHienTai === 1 }">
                  <a class="page-link" href="#" @click.prevent="trangHienTai = 1"><i class="fas fa-angle-left"></i></a>
                </li>
                <li class="page-item" v-for="trang in danhSachTrang" :key="trang" :class="{ active: trang === trangHienTai }">
                  <a class="page-link" href="#" @click.prevent="trangHienTai = trang">{{ trang }}</a>
                </li>
                <li class="page-item" :class="{ disabled: trangHienTai === tongSoTrang }">
                  <a class="page-link" href="#" @click.prevent="trangHienTai = tongSoTrang"><i class="fas fa-angle-right"></i></a>
                </li>
              </ul>
            </div>
          </div>
        </div>
      </div>

      <!-- SIDEBAR THỐNG KÊ -->
      <div v-if="hienThongKe" class="col-lg-4 transition-layout">
        <div class="stat-card-simple shadow-sm bg-white mb-4">
          <div class="stat-label text-uppercase small font-weight-bold text-muted mb-2">Giá trị rủi ro</div>
          <div class="h3 font-weight-bold text-danger mb-0">{{ formatGia(thongKe.tongGiaTri) }}</div>
          <div class="progress mt-3" style="height: 6px; border-radius: 3px;">
            <div class="progress-bar bg-danger" style="width: 75%"></div>
          </div>
        </div>

        <div class="stat-card-simple shadow-sm bg-white mb-4">
          <div class="stat-label text-uppercase small font-weight-bold text-muted mb-2">Số lô đã hết hạn</div>
          <div class="h3 font-weight-bold text-dark mb-0">{{ thongKe.soLoHetHan }} <small class="text-muted">lô</small></div>
        </div>

        <div class="stat-card-simple shadow-sm bg-white">
          <div class="stat-label text-uppercase small font-weight-bold text-muted mb-2">Số lô sắp hết hạn</div>
          <div class="h3 font-weight-bold text-warning mb-0">{{ thongKe.soLoSapHetHan || 0 }} <small class="text-muted">lô</small></div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import axiosClient from '../../api/axiosClient';

const danhSach = ref([]);
const dangTai  = ref(false);
const locThang = ref('');
const locLoai  = ref('all');
const hienThongKe = ref(false);
const thongKe  = ref({ tongGiaTri: 0, soLoHetHan: 0, soLoSapHetHan: 0 });

// Phân trang
const trangHienTai   = ref(1);
const soDongMoiTrang = ref(10);
const tongSoTrang = computed(() => Math.ceil(danhSach.value.length / soDongMoiTrang.value));
const batDau = computed(() => (trangHienTai.value - 1) * soDongMoiTrang.value);
const ketThuc = computed(() => Math.min(batDau.value + soDongMoiTrang.value, danhSach.value.length));
const danhSachHienThi = computed(() => danhSach.value.slice(batDau.value, ketThuc.value));
const danhSachTrang = computed(() => {
  const total = tongSoTrang.value;
  const current = trangHienTai.value;
  const range = [];
  for (let i = Math.max(1, current - 2); i <= Math.min(total, current + 2); i++) range.push(i);
  return range;
});

const loadData = async () => {
  dangTai.value = true;
  try {
    const data = await axiosClient.get('/Kho/danh-sach-lo', {
      params: { thang: locThang.value || undefined, loai:  locLoai.value !== 'all' ? locLoai.value : undefined }
    });
    danhSach.value = data?.items ?? [];
    thongKe.value  = data?.thongKe ?? { tongGiaTri: 0, soLoHetHan: 0, soLoSapHetHan: 0 };
  } catch (err) {
    console.error('Lỗi tải cảnh báo:', err);
  } finally {
    dangTai.value = false;
  }
};

const onFilter = () => { trangHienTai.value = 1; loadData(); };
const parseNgay = (value) => {
  if (!value) return null;
  const raw = String(value).trim();
  const parsed = new Date(raw.includes('T') ? raw : `${raw}T00:00:00`);
  if (Number.isNaN(parsed.getTime())) return null;
  return new Date(parsed.getFullYear(), parsed.getMonth(), parsed.getDate());
};

const laHetHan = (lo) => {
  const hsd = parseNgay(lo?.hanSuDung);
  if (!hsd) return false;
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  return hsd < today;
};

const laSapHetHan = (lo) => {
  const hsd = parseNgay(lo?.hanSuDung);
  if (!hsd) return false;

  const today = new Date();
  today.setHours(0, 0, 0, 0);
  if (hsd < today) return false;

  const sixMonthsLater = new Date(today);
  sixMonthsLater.setMonth(sixMonthsLater.getMonth() + 6);
  return hsd < sixMonthsLater;
};
const formatGia = (v) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v ?? 0);

onMounted(loadData);
</script>

<style scoped>
.transition-layout { transition: all 0.35s cubic-bezier(0.4, 0, 0.2, 1); }
.premium-table { width: 100%; border-collapse: separate; border-spacing: 0; }
.premium-table thead th { 
  background: #f8fafc; padding: 14px 16px; font-size: 0.75rem; 
  text-transform: uppercase; color: #64748b; letter-spacing: 0.05em;
  border-bottom: 2px solid #e2e8f0;
}
.premium-table tbody td { padding: 14px 16px; border-bottom: 1px solid #f1f5f9; vertical-align: middle; }
.premium-table tbody tr:hover { background-color: #f8fafc; }

.badge-custom { padding: 4px 10px; border-radius: 6px; font-weight: 700; font-size: 0.7rem; }
.bg-danger-soft { background: #fee2e2; color: #b91c1c; }
.bg-warning-soft { background: #fef3c7; color: #92400e; }
.bg-success-soft { background: #dcfce7; color: #166534; }

.bubble-ton { 
  background: #f1f5f9; padding: 4px 12px; border-radius: 20px; 
  font-weight: 700; color: #334155; font-size: 0.85rem; 
}

.stat-card-simple { border-radius: 12px; padding: 1.25rem; border: 1px solid #e2e8f0; }

.custom-pagination .page-link { border: none; background: transparent; color: #64748b; font-weight: 600; margin: 0 2px; }
.custom-pagination .page-item.active .page-link { background: #3b82f6 !important; color: white !important; border-radius: 6px; }

.form-control-sm { height: 32px; font-size: 0.85rem; }
</style>

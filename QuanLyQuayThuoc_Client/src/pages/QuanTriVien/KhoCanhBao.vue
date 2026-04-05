<template>
  <section>
    <div class="qlk-filters-card card mb-3">
      <div class="card-header py-3">
        <h6 class="m-0 font-weight-bold text-primary">4. Cảnh báo hết hạn</h6>
      </div>
      <div class="card-body">
        <div class="row">
          <div class="col-md-4 mb-3">
            <label class="small text-muted">Xem theo tháng HSD</label>
            <input type="month" class="form-control" v-model="locThang" @change="onFilter" />
          </div>
          <div class="col-md-3 mb-3">
            <label class="small text-muted">Loại cảnh báo</label>
            <select class="form-control" v-model="locLoai" @change="onFilter">
              <option value="all">Cả đã hết + sắp hết</option>
              <option value="expired">Chỉ đã hết hạn</option>
              <option value="soon">Chỉ sắp hết hạn (&lt; 6 tháng)</option>
            </select>
          </div>
          <div class="col-md-2 mb-3">
            <label class="small text-muted">Số dòng / trang</label>
            <select class="form-control" v-model="soDongMoiTrang" @change="trangHienTai = 1">
              <option :value="10">10</option>
              <option :value="20">20</option>
              <option :value="50">50</option>
            </select>
          </div>
          <div class="col-md-3 mb-3 d-flex align-items-end">
            <button type="button" class="btn btn-outline-secondary btn-block" @click="onFilter">
              <i class="fas fa-sync-alt mr-1"></i> Làm mới
            </button>
          </div>
        </div>
        <p class="qlk-muted mb-0 small">
          <i class="fas fa-exclamation-triangle mr-1"></i>
          Dòng sắp hết được tô màu để đẩy lên đầu danh sách.
        </p>
      </div>
    </div>

    <div class="row">
      <div class="col-lg-8 mb-3">
        <div class="card">
          <div class="card-header py-3 d-flex justify-content-between align-items-center">
            <div>
              <div class="font-weight-bold text-primary">Danh sách cảnh báo</div>
              <div class="small text-muted">Sắp xếp theo Hạn dùng tăng dần</div>
            </div>
            <span class="badge badge-light text-gray-600">{{ danhSach.length }} dòng</span>
          </div>
          <div class="card-body p-0">
            <div v-if="dangTai" class="text-center py-4">
              <div class="spinner-border text-primary" role="status"></div>
            </div>
            <div v-else class="table-responsive">
              <table class="table table-bordered table-hover mb-0 qlk-table">
                <thead class="thead-light">
                  <tr>
                    <th>Số lô</th>
                    <th>Hạn sử dụng</th>
                    <th>Tồn</th>
                    <th>Thuốc</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="lo in danhSachHienThi" :key="lo.maLo" :class="rowClass(lo)">
                    <td>{{ lo.soLo }}</td>
                    <td>
                      {{ lo.hanSuDung }}
                      <span v-if="laHetHan(lo)" class="badge badge-danger ml-1">Hết hạn</span>
                      <span v-else class="badge badge-warning text-dark ml-1">Sắp hết</span>
                    </td>
                    <td>{{ lo.soLuongTon }}</td>
                    <td>{{ lo.tenThuoc }}</td>
                  </tr>
                  <tr v-if="danhSachHienThi.length === 0">
                    <td colspan="4" class="text-center text-muted py-3">Không có cảnh báo.</td>
                  </tr>
                </tbody>
              </table>
            </div>

            <!-- PHÂN TRANG -->
            <div v-if="tongSoTrang > 1" class="d-flex justify-content-between align-items-center px-3 py-2 border-top">
              <div class="small text-muted">
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

      <div class="col-lg-4">
        <div class="qlk-stat-card">
          <div class="font-weight-bold text-primary mb-2">Thống kê nhanh</div>
          <div class="mb-2">
            <span class="qlk-muted">Tổng giá trị kho</span>
            <div class="qlk-stat-value">{{ formatGia(thongKe.tongGiaTri) }}</div>
          </div>
          <div class="mb-2">
            <span class="qlk-muted">Lô đã hết hạn</span>
            <div class="qlk-stat-value">{{ thongKe.soLoHetHan }}</div>
          </div>
          <div class="mb-0">
            <span class="qlk-muted">Lô sắp hết</span>
            <div class="qlk-stat-value">{{ thongKe.soLoSapHetHan || 0 }}</div>
          </div>
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
const locLoai  = ref('soon');
const thongKe  = ref({ tongGiaTri: 0, soLoHetHan: 0, soLoSapHetHan: 0, soMatHangSapHetTon: 0 });

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
  const total   = tongSoTrang.value;
  const current = trangHienTai.value;
  const start   = Math.max(1, current - 2);
  const end     = Math.min(total, current + 2);
  const range   = [];
  for (let i = start; i <= end; i++) range.push(i);
  return range;
});
// ────────────────────────────────────────────────

const loadData = async () => {
  dangTai.value = true;
  try {
    const data = await axiosClient.get('/Kho/danh-sach-lo', {
      params: {
        thang: locThang.value || undefined,
        loai:  locLoai.value !== 'all' ? locLoai.value : 'soon',
      },
    });
    danhSach.value = data?.items ?? [];
    thongKe.value  = data?.thongKe ?? { tongGiaTri: 0, soLoHetHan: 0, soLoSapHetHan: 0 };
  } catch (err) {
    console.error('Lỗi tải cảnh báo:', err);
    danhSach.value = [];
  } finally {
    dangTai.value = false;
  }
};

const onFilter = () => {
  trangHienTai.value = 1;
  loadData();
};

const laHetHan = (lo) => lo.hanSuDung && new Date(lo.hanSuDung) < new Date();
const rowClass = (lo) => ({
  'qlk-row--expired': laHetHan(lo),
  'qlk-row--warn':    !laHetHan(lo),
});

const formatGia = (v) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v ?? 0);

onMounted(loadData);
</script>
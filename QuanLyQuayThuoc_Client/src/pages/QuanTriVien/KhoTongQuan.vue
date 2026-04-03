<template>
  <section>
    <div class="qlk-filters-card card mb-3">
      <div class="card-header py-3">
        <h6 class="m-0 font-weight-bold text-primary">1. Tổng quan tồn kho</h6>
      </div>
      <div class="card-body">
        <div class="row">
          <div class="col-md-4 mb-3">
            <label class="small text-muted">Danh mục</label>
            <select class="form-control" v-model="locDanhMuc" @change="onFilter">
              <option value="">— Tất cả danh mục —</option>
              <option v-for="dm in danhSachDanhMuc" :key="dm.maDanhMuc" :value="dm.maDanhMuc">
                {{ dm.tenDanhMuc }}
              </option>
            </select>
          </div>
          <div class="col-md-4 mb-3">
            <label class="small text-muted">Tìm theo tên / hoạt chất</label>
            <input class="form-control" v-model="tuKhoa" placeholder="Ví dụ: Smecta / Diosmectite"
              @input="onFilter" />
          </div>
          <div class="col-md-4 mb-3 d-flex align-items-end">
            <label class="small text-muted w-100">Số dòng mỗi trang</label>
            <select class="form-control" v-model="soDongMoiTrang" @change="trangHienTai = 1">
              <option :value="10">10</option>
              <option :value="20">20</option>
              <option :value="50">50</option>
            </select>
          </div>
        </div>
      </div>
    </div>

    <div class="row">
      <div class="col-lg-8 mb-3">
        <div class="card">
          <div class="card-header py-3 d-flex justify-content-between align-items-center">
            <div class="font-weight-bold text-primary">Danh sách sản phẩm</div>
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
                    <th>Mã thuốc</th>
                    <th>Tên thuốc</th>
                    <th>Danh mục</th>
                    <th>Tổng tồn</th>
                    <th>Trạng thái</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="sp in danhSachHienThi" :key="sp.maThuoc"
                    :class="sp.tongTon < 50 ? 'qlk-row--warn' : ''">
                    <td>{{ sp.maThuoc }}</td>
                    <td>{{ sp.tenThuoc }}</td>
                    <td>{{ sp.tenDanhMuc }}</td>
                    <td>{{ sp.tongTon }}</td>
                    <td>
                      <span v-if="sp.tongTon === 0" class="badge badge-danger">Hết hàng</span>
                      <span v-else-if="sp.tongTon < 50" class="badge badge-warning text-dark">Sắp hết</span>
                      <span v-else class="badge badge-success">Còn hàng</span>
                    </td>
                  </tr>
                  <tr v-if="danhSachHienThi.length === 0">
                    <td colspan="5" class="text-center text-muted py-3">Không có dữ liệu.</td>
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

      <div class="col-lg-4">
        <div class="qlk-stat-card">
          <div class="font-weight-bold text-primary mb-2">Thống kê nhanh</div>
          <div class="mb-2">
            <span class="qlk-muted">Tổng giá trị kho</span>
            <div class="qlk-stat-value">{{ formatGia(thongKe?.tongGiaTri) }}</div>
          </div>
          <div class="mb-2">
            <span class="qlk-muted">Lô đã hết hạn</span>
            <div class="qlk-stat-value">{{ thongKe?.soLoHetHan || 0 }}</div>
          </div>
          <div class="mb-0">
            <span class="qlk-muted">Mặt hàng sắp hết</span>
            <div class="qlk-stat-value">{{ thongKe?.soMatHangSapHetTon || 0 }}</div>
          </div>
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
const thongKe      = ref({
  tongGiaTri: 0,
  soLoHetHan: 0,
  soLoSapHetHan: 0,
  soMatHangSapHetTon: 0
});

// ── PHÂN TRANG ──────────────────────────────────
const trangHienTai  = ref(1);
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

// Hiển thị tối đa 5 nút trang
const danhSachTrang = computed(() => {
  const total = tongSoTrang.value;
  const current = trangHienTai.value;
  const delta = 2;
  const range = [];
  const start = Math.max(1, current - delta);
  const end   = Math.min(total, current + delta);
  for (let i = start; i <= end; i++) range.push(i);
  return range;
});
// ────────────────────────────────────────────────

const loadData = async () => {
  dangTai.value = true;
  try {
    const data = await axiosClient.get('/Kho/tong-quan', {
      params: {
        search:    tuKhoa.value    || undefined,
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

// Reset về trang 1 mỗi khi filter thay đổi
const onFilter = () => {
  trangHienTai.value = 1;
  loadData();
};

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

onMounted(() => {
  loadSidebar();
  loadData();
});
</script>
<template>
  <section>
    <!-- Bộ lọc -->
    <div class="qlk-filters-card card mb-3">
      <div class="card-header py-3">
        <h6 class="m-0 font-weight-bold text-primary">1. Tổng quan tồn kho</h6>
      </div>
      <div class="card-body">
        <div class="row">
          <div class="col-md-4 mb-3">
            <label class="small text-muted">Danh mục</label>
            <select class="form-control" v-model="locDanhMuc" @change="loadData">
              <option value="">— Tất cả danh mục —</option>
              <option v-for="dm in danhSachDanhMuc" :key="dm.maDanhMuc" :value="dm.maDanhMuc">
                {{ dm.tenDanhMuc }}
              </option>
            </select>
          </div>
          <div class="col-md-4 mb-3">
            <label class="small text-muted">Nhà sản xuất</label>
            <select class="form-control" v-model="locNSX" @change="loadData">
              <option value="">— Tất cả nhà sản xuất —</option>
              <option v-for="nsx in danhSachNSX" :key="nsx" :value="nsx">{{ nsx }}</option>
            </select>
          </div>
          <div class="col-md-4 mb-3">
            <label class="small text-muted">Tìm theo tên / hoạt chất</label>
            <input class="form-control" v-model="tuKhoa" placeholder="Ví dụ: Smecta / Diosmectite"
              @input="loadData" />
          </div>
        </div>
        <p class="qlk-muted mb-0 small">
          <i class="fas fa-info-circle mr-1"></i>
          Trạng thái "Sắp hết hàng" khi tổng tồn &lt; <strong>50</strong>.
        </p>
      </div>
    </div>

    <div class="row">
      <!-- Bảng sản phẩm -->
      <div class="col-lg-8 mb-3">
        <div class="card">
          <div class="card-header py-3 d-flex justify-content-between align-items-center">
            <div>
              <div class="font-weight-bold text-primary">Danh sách sản phẩm</div>
              <div class="small text-muted">Cộng dồn tất cả các lô</div>
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
                    <th>Mã thuốc</th>
                    <th>Tên thuốc</th>
                    <th>Danh mục</th>
                    <th>Tổng tồn</th>
                    <th>Trạng thái tồn</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="sp in danhSach" :key="sp.maThuoc"
                    :class="sp.tongTon < 50 ? 'qlk-row--warn' : ''">
                    <td>{{ sp.maThuoc }}</td>
                    <td>{{ sp.tenThuoc }}</td>
                    <td>{{ sp.tenDanhMuc }}</td>
                    <td>{{ sp.tongTon }}</td>
                    <td>
                      <span v-if="sp.tongTon === 0" class="badge badge-danger">Hết hàng</span>
                      <span v-else-if="sp.tongTon < 50" class="badge badge-warning text-dark">Sắp hết hàng</span>
                      <span v-else class="badge badge-success">Còn hàng</span>
                    </td>
                  </tr>
                  <tr v-if="danhSach.length === 0">
                    <td colspan="5" class="text-center text-muted py-3">Không có dữ liệu.</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>

      <!-- Thống kê nhanh -->
      <div class="col-lg-4">
        <div class="qlk-stat-card">
          <div class="font-weight-bold text-primary mb-2">Thống kê nhanh</div>
          <div class="mb-2">
            <span class="qlk-muted">Tổng giá trị kho</span>
            <div class="qlk-stat-value">{{ formatGia(thongKe.tongGiaTri) }}</div>
          </div>
          <div class="mb-2">
            <span class="qlk-muted">Số lô đã hết hạn</span>
            <div class="qlk-stat-value">{{ thongKe.soLoHetHan }}</div>
          </div>
          <div class="mb-0">
            <span class="qlk-muted">Số mặt hàng sắp hết</span>
            <div class="qlk-stat-value">{{ thongKe.soMatHangSapHet }}</div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axiosClient from '../../api/axiosClient';

const danhSach      = ref([]);
const danhSachDanhMuc = ref([]);
const danhSachNSX   = ref([]);
const dangTai       = ref(false);
const locDanhMuc    = ref('');
const locNSX        = ref('');
const tuKhoa        = ref('');
const thongKe       = ref({ tongGiaTri: 0, soLoHetHan: 0, soMatHangSapHet: 0 });

// GET /Kho/tong-quan?danhMuc=&nsx=&q=
const loadData = async () => {
  dangTai.value = true;
  try {
    const res = await axiosClient.get('/Kho/tong-quan', {
      params: { danhMuc: locDanhMuc.value || undefined, nsx: locNSX.value || undefined, q: tuKhoa.value || undefined },
    });
    danhSach.value = res.data.items;
    thongKe.value  = res.data.thongKe;
  } catch (err) {
    console.error('Lỗi tải tổng quan:', err);
  } finally {
    dangTai.value = false;
  }
};

const loadSidebar = async () => {
  try {
    const [resDM, resNSX] = await Promise.all([
      axiosClient.get('/DanhMuc'),
      axiosClient.get('/Thuoc/nha-san-xuat'),
    ]);
    danhSachDanhMuc.value = resDM.data;
    danhSachNSX.value     = resNSX.data;
  } catch (err) { console.error(err); }
};

const formatGia = (v) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v ?? 0);

onMounted(() => { loadSidebar(); loadData(); });
</script>
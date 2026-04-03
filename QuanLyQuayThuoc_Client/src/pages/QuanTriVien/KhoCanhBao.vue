<template>
  <section>
    <!-- Bộ lọc -->
    <div class="qlk-filters-card card mb-3">
      <div class="card-header py-3">
        <h6 class="m-0 font-weight-bold text-primary">4. Cảnh báo hết hạn</h6>
      </div>
      <div class="card-body">
        <div class="row">
          <div class="col-md-5 mb-3">
            <label class="small text-muted">Xem theo tháng HSD</label>
            <input type="month" class="form-control" v-model="locThang" @change="loadData" />
          </div>
          <div class="col-md-4 mb-3">
            <label class="small text-muted">Loại cảnh báo</label>
            <select class="form-control" v-model="locLoai" @change="loadData">
              <option value="all">Cả đã hết + sắp hết</option>
              <option value="expired">Chỉ đã hết hạn</option>
              <option value="soon">Chỉ sắp hết hạn (&lt; 6 tháng)</option>
            </select>
          </div>
          <div class="col-md-3 mb-3 d-flex align-items-end">
            <button type="button" class="btn btn-outline-secondary btn-block" @click="loadData">
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
      <!-- Bảng cảnh báo -->
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
                  <tr v-for="lo in danhSach" :key="lo.maLo" :class="rowClass(lo)">
                    <td>{{ lo.soLo }}</td>
                    <td>
                      {{ lo.hanSuDung }}
                      <span v-if="laHetHan(lo)" class="badge badge-danger ml-1">Hết hạn</span>
                      <span v-else class="badge badge-warning text-dark ml-1">Sắp hết</span>
                    </td>
                    <td>{{ lo.soLuongTon }}</td>
                    <td>{{ lo.tenThuoc }}</td>
                  </tr>
                  <tr v-if="danhSach.length === 0">
                    <td colspan="4" class="text-center text-muted py-3">Không có cảnh báo.</td>
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
            <span class="qlk-muted">Lô đã hết hạn</span>
            <div class="qlk-stat-value">{{ thongKe.soLoHetHan }}</div>
          </div>
          <div class="mb-0">
            <span class="qlk-muted">Lô sắp hết</span>
            <div class="qlk-stat-value">{{ thongKe.soLoSapHet }}</div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axiosClient from '../../api/axiosClient';

const danhSach = ref([]);
const dangTai  = ref(false);
const locThang = ref('');
const locLoai  = ref('all');
const thongKe  = ref({ tongGiaTri: 0, soLoHetHan: 0, soLoSapHet: 0 });

// GET /Kho/canh-bao?thang=&loai=
const loadData = async () => {
  dangTai.value = true;
  try {
    const res = await axiosClient.get('/Kho/canh-bao', {
      params: {
        thang: locThang.value || undefined,
        loai:  locLoai.value !== 'all' ? locLoai.value : undefined,
      },
    });
    danhSach.value = res.data.items;
    thongKe.value  = res.data.thongKe;
  } catch (err) {
    console.error('Lỗi tải cảnh báo:', err);
  } finally {
    dangTai.value = false;
  }
};

const laHetHan    = (lo) => new Date(lo.hanSuDung) < new Date();
const rowClass    = (lo) => ({
  'qlk-row--expired': laHetHan(lo),
  'qlk-row--warn':    !laHetHan(lo),
});

const formatGia = (v) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v ?? 0);

onMounted(loadData);
</script>
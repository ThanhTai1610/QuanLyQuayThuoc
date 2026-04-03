<template>
  <section>
    <div class="card qlk-filters-card">
      <div class="card-header py-3">
        <h6 class="m-0 font-weight-bold text-primary">3. Nhập kho (Import Goods)</h6>
      </div>
      <div class="card-body">

        <!-- Thông tin phiếu nhập -->
        <div class="row">
          <div class="col-md-6 mb-3">
            <label class="small text-muted">Nhà cung cấp</label>
            <input class="form-control" v-model="phieu.nhaCungCap" placeholder="Ví dụ: Công ty dược A" />
          </div>
          <div class="col-md-6 mb-3">
            <label class="small text-muted">Người nhập</label>
            <input class="form-control" v-model="phieu.nguoiNhap" />
          </div>
          <div class="col-md-4 mb-3">
            <label class="small text-muted">Ngày nhập</label>
            <input type="date" class="form-control" v-model="phieu.ngayNhap" />
          </div>
          <div class="col-md-8 mb-3">
            <label class="small text-muted">Ghi chú</label>
            <input class="form-control" v-model="phieu.ghiChu" placeholder="Ghi chú lô / phiếu nhập kho..." />
          </div>
        </div>

        <hr class="my-4">

        <!-- Chọn thuốc + thông tin lô -->
        <div class="row">
          <div class="col-md-8 mb-3">
            <label class="small text-muted">Chọn thuốc</label>
            <input class="form-control" v-model="timThuoc"
              placeholder="Gõ tên hoặc hoạt chất để tìm..." @input="timKiemThuoc" />
            <!-- Autocomplete -->
            <div v-if="ketQuaTim.length > 0" class="pos-autocomplete-list">
              <div v-for="sp in ketQuaTim" :key="sp.maThuoc" class="pos-autocomplete-item"
                @click="chonThuoc(sp)">
                {{ sp.tenThuoc }}
              </div>
            </div>
            <div class="small text-muted mt-2">
              {{ thuocChon ? `Đã chọn: ${thuocChon.tenThuoc}` : '—' }}
            </div>
          </div>
          <div class="col-md-4 mb-3">
            <label class="small text-muted">Đơn vị tính</label>
            <select class="form-control" v-model="dongNhap.tenDonVi">
              <option>Hộp</option><option>Vỉ</option><option>Viên</option>
              <option>Gói</option><option>Lọ</option><option>Chai</option>
            </select>
          </div>
        </div>

        <div class="row">
          <div class="col-md-4 mb-3">
            <label class="small text-muted">Số lô</label>
            <input class="form-control" v-model="dongNhap.soLo" placeholder="Ví dụ: BN9902" />
          </div>
          <div class="col-md-4 mb-3">
            <label class="small text-muted">Hạn sử dụng</label>
            <input type="date" class="form-control" v-model="dongNhap.hanSuDung" />
          </div>
          <div class="col-md-4 mb-3">
            <label class="small text-muted">Giá nhập (đ)</label>
            <input type="number" min="0" class="form-control" v-model="dongNhap.giaNhap" />
          </div>
          <div class="col-md-4 mb-3">
            <label class="small text-muted">Số lượng nhập</label>
            <input type="number" min="1" class="form-control" v-model="dongNhap.soLuong" />
          </div>
        </div>

        <p v-if="loiThem" class="text-danger small">{{ loiThem }}</p>

        <div class="d-flex flex-wrap align-items-center justify-content-between">
          <button type="button" class="btn btn-outline-primary" @click="themDong">
            <i class="fas fa-plus mr-1"></i> Thêm vào danh sách nhập
          </button>
          <button type="button" class="btn btn-primary" :disabled="dangLuu || danhSachNhap.length === 0"
            @click="hoanTatNhapKho">
            <i class="fas fa-check mr-1"></i>
            {{ dangLuu ? 'Đang lưu...' : 'Hoàn tất nhập kho' }}
          </button>
        </div>

        <!-- Danh sách sẽ nhập -->
        <div class="table-responsive mt-3">
          <table v-if="danhSachNhap.length > 0" class="table table-bordered table-sm mb-0 qlk-table">
            <thead class="thead-light">
              <tr>
                <th>Số lô</th><th>Hạn dùng</th><th>Số lượng</th>
                <th>Giá nhập</th><th>Thuốc</th><th>Xóa</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(d, i) in danhSachNhap" :key="i">
                <td>{{ d.soLo }}</td>
                <td>{{ d.hanSuDung }}</td>
                <td>{{ d.soLuong }}</td>
                <td>{{ formatGia(d.giaNhap) }}</td>
                <td>{{ d.tenThuoc }}</td>
                <td>
                  <button class="btn btn-danger btn-sm" @click="xoaDong(i)">
                    <i class="fas fa-trash"></i>
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
          <div v-else class="text-center text-muted py-4">Chưa có dòng nhập.</div>
        </div>

        <p v-if="loiHoanTat" class="text-danger small mt-2">{{ loiHoanTat }}</p>
        <p v-if="thanhCong" class="text-success small mt-2">{{ thanhCong }}</p>

      </div>
    </div>
  </section>
</template>

<script setup>
import { ref, reactive } from 'vue';
import axiosClient from '../../api/axiosClient';

const phieu = reactive({ nhaCungCap: '', nguoiNhap: '', ngayNhap: '', ghiChu: '' });

const timThuoc   = ref('');
const ketQuaTim  = ref([]);
const thuocChon  = ref(null);
const loiThem    = ref('');
const loiHoanTat = ref('');
const thanhCong  = ref('');
const dangLuu    = ref(false);
const danhSachNhap = ref([]);

const dongNhapRong = () => ({ soLo: '', hanSuDung: '', giaNhap: 0, soLuong: 1, tenDonVi: 'Hộp' });
const dongNhap = reactive(dongNhapRong());

// GET /Thuoc/tim-kiem?q=
let timer = null;
const timKiemThuoc = () => {
  clearTimeout(timer);
  if (!timThuoc.value.trim()) { ketQuaTim.value = []; return; }
  timer = setTimeout(async () => {
    try {
      const res = await axiosClient.get('/Thuoc/tim-kiem', { params: { q: timThuoc.value } });
      ketQuaTim.value = res.data.slice(0, 8);
    } catch (err) { console.error(err); }
  }, 300);
};

const chonThuoc = (sp) => {
  thuocChon.value   = sp;
  timThuoc.value    = sp.tenThuoc;
  ketQuaTim.value   = [];
};

const themDong = () => {
  loiThem.value = '';
  if (!thuocChon.value) { loiThem.value = 'Vui lòng chọn thuốc.'; return; }
  if (!dongNhap.soLo.trim()) { loiThem.value = 'Vui lòng nhập số lô.'; return; }
  if (!dongNhap.hanSuDung)   { loiThem.value = 'Vui lòng nhập hạn sử dụng.'; return; }

  danhSachNhap.value.push({
    maThuoc:    thuocChon.value.maThuoc,
    tenThuoc:   thuocChon.value.tenThuoc,
    soLo:       dongNhap.soLo,
    hanSuDung:  dongNhap.hanSuDung,
    giaNhap:    Number(dongNhap.giaNhap),
    soLuong:    Number(dongNhap.soLuong),
    tenDonVi:   dongNhap.tenDonVi,
  });

  // Reset dòng nhập
  Object.assign(dongNhap, dongNhapRong());
  thuocChon.value = null;
  timThuoc.value  = '';
};

const xoaDong = (i) => danhSachNhap.value.splice(i, 1);

// POST /Kho/nhap-kho
const hoanTatNhapKho = async () => {
  loiHoanTat.value = '';
  thanhCong.value  = '';
  dangLuu.value    = true;
  try {
    await axiosClient.post('/Kho/nhap-kho', {
      ...phieu,
      chiTiet: danhSachNhap.value,
    });
    danhSachNhap.value = [];
    thanhCong.value    = 'Nhập kho thành công!';
  } catch (err) {
    loiHoanTat.value = err.response?.data?.message || 'Có lỗi xảy ra.';
  } finally {
    dangLuu.value = false;
  }
};

const formatGia = (v) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v ?? 0);
</script>
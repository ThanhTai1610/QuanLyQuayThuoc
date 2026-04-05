<template>
  <section>
    <div class="card qlk-filters-card">
      <div class="card-header py-3">
        <h6 class="m-0 font-weight-bold text-primary">3. Nhập kho (Import Goods)</h6>
      </div>
      <div class="card-body">

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

        <div class="row">
          <div class="col-md-8 mb-3">
            <label class="small text-muted">Chọn thuốc</label>
            <input class="form-control" v-model="timThuoc" placeholder="Gõ tên hoặc hoạt chất để tìm..."
              @input="timKiemThuoc" />
            <div v-if="ketQuaTim.length > 0" class="pos-autocomplete-list">
              <div v-for="sp in ketQuaTim" :key="sp.maThuoc" class="pos-autocomplete-item" @click="chonThuoc(sp)">
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
              <option>Hộp</option>
              <option>Vỉ</option>
              <option>Viên</option>
              <option>Gói</option>
              <option>Lọ</option>
              <option>Chai</option>
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
            <input type="number" min="0" class="form-control" v-model.number="dongNhap.giaNhap" />
          </div>
          <div class="col-md-4 mb-3">
            <label class="small text-muted">Số lượng nhập</label>
            <input type="number" min="1" class="form-control" v-model.number="dongNhap.soLuong" />
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

        <div class="table-responsive mt-3">
          <table v-if="danhSachNhap.length > 0" class="table table-bordered table-sm mb-0 qlk-table">
            <thead class="thead-light">
              <tr>
                <th>Số lô</th>
                <th>Hạn dùng</th>
                <th>Số lượng</th>
                <th>Giá nhập</th>
                <th>Thuốc</th>
                <th>Xóa</th>
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

// Khởi tạo thông tin phiếu nhập
const phieu = reactive({
  nhaCungCap: '',
  nguoiNhap: '',
  ngayNhap: new Date().toISOString().split('T')[0],
  ghiChu: ''
});

const timThuoc = ref('');
const ketQuaTim = ref([]);
const thuocChon = ref(null);
const loiThem = ref('');
const loiHoanTat = ref('');
const thanhCong = ref('');
const dangLuu = ref(false);
const danhSachNhap = ref([]);

const dongNhapRong = () => ({ soLo: '', hanSuDung: '', giaNhap: 0, soLuong: 1, tenDonVi: 'Hộp' });
const dongNhap = reactive(dongNhapRong());

// Tìm kiếm thuốc
let timer = null;
const timKiemThuoc = () => {
  clearTimeout(timer);
  if (!timThuoc.value.trim()) { ketQuaTim.value = []; return; }
  timer = setTimeout(async () => {
    try {
      // ✅ Đổi route và tên param cho khớp BanHangController
      const data = await axiosClient.get('/BanHang/tim-kiem', { params: { tenThuoc: timThuoc.value } });
      ketQuaTim.value = Array.isArray(data) ? data.slice(0, 8) : [];
    } catch (err) { console.error(err); }
  }, 300);
};

const chonThuoc = (sp) => {
  thuocChon.value = sp;
  timThuoc.value = sp.tenThuoc;
  ketQuaTim.value = [];
};

// Thêm một dòng vào danh sách chờ nhập
const themDong = () => {
  loiThem.value = '';
  if (!thuocChon.value) { loiThem.value = 'Vui lòng chọn thuốc.'; return; }
  if (!dongNhap.soLo.trim()) { loiThem.value = 'Vui lòng nhập số lô.'; return; }
  if (!dongNhap.hanSuDung) { loiThem.value = 'Vui lòng nhập hạn sử dụng.'; return; }

  danhSachNhap.value.push({
    maThuoc: thuocChon.value.maThuoc,
    tenThuoc: thuocChon.value.tenThuoc,
    soLo: dongNhap.soLo,
    hanSuDung: dongNhap.hanSuDung,
    giaNhap: Number(dongNhap.giaNhap),
    soLuong: Number(dongNhap.soLuong),
    tenDonVi: dongNhap.tenDonVi,
  });

  // Reset dữ liệu dòng nhập sau khi thêm thành công
  Object.assign(dongNhap, dongNhapRong());
  thuocChon.value = null;
  timThuoc.value = '';
};

const xoaDong = (i) => danhSachNhap.value.splice(i, 1);

// Gửi toàn bộ phiếu nhập lên Backend
const hoanTatNhapKho = async () => {
  loiHoanTat.value = '';
  thanhCong.value = '';

  if (!phieu.nhaCungCap.trim()) {
    loiHoanTat.value = 'Vui lòng nhập Nhà cung cấp.';
    return;
  }

  dangLuu.value = true;
  try {
    // Payload khớp chính xác với DTO
    const payload = {
      nhaCungCap: phieu.nhaCungCap,
      nguoiNhap: phieu.nguoiNhap,
      ngayNhap: phieu.ngayNhap,
      ghiChu: phieu.ghiChu,
      chiTiet: danhSachNhap.value.map(item => ({
        maThuoc: item.maThuoc,
        soLo: item.soLo,
        hanSuDung: item.hanSuDung,
        giaNhap: item.giaNhap,
        soLuong: item.soLuong,
        tenDonVi: item.tenDonVi,
        maVach: "" // Backend tự sinh
      }))
    };

    await axiosClient.post('/Kho/nhap-kho', payload);
    thanhCong.value = 'Nhập kho thành công!';
    danhSachNhap.value = [];
    Object.assign(phieu, {
      nhaCungCap: '', nguoiNhap: '',
      ngayNhap: new Date().toISOString().split('T')[0],
      ghiChu: ''
    });
  } catch (err) {
  const errData = err.response?.data;
  console.error('Validation errors:', errData?.errors); // ← xem field nào lỗi
  
  // Hiện lỗi chi tiết ra UI
  if (errData?.errors) {
    const messages = Object.entries(errData.errors)
      .map(([field, msgs]) => `${field}: ${msgs.join(', ')}`)
      .join(' | ');
    loiHoanTat.value = messages;
  } else {
    loiHoanTat.value = errData?.message || err.message || 'Có lỗi xảy ra.';
  }
} finally {
    dangLuu.value = false;
  }
};

const formatGia = (v) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v ?? 0);
</script>
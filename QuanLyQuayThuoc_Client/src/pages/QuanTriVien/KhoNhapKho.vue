<template>
  <section>
    <div class="card qlk-filters-card shadow-sm">
      <div class="card-header py-3 bg-primary text-white d-flex justify-content-between align-items-center">
        <h6 class="m-0 font-weight-bold text-white">3. Nhập kho</h6>
        <!-- Toggle chế độ -->
        <div class="d-flex align-items-center">
          <span class="text-white small mr-2">Thuốc có sẵn</span>
          <div class="custom-switch-wrap" @click="chuyenCheDoc"
            :title="laThuocMoi ? 'Đang nhập thuốc mới' : 'Đang nhập thuốc có sẵn'">
            <div class="custom-switch-track" :class="{ active: laThuocMoi }">
              <div class="custom-switch-thumb"></div>
            </div>
          </div>
          <span class="text-white small ml-2">Thuốc mới</span>
        </div>
      </div>

      <div class="card-body">

        <!-- ══════════════════════════════════════════════
            PHẦN DÙNG CHUNG: Thông tin phiếu nhập
        ══════════════════════════════════════════════ -->
        <div class="row">
          <div class="col-md-6 mb-3">
            <label class="small text-muted font-weight-bold">Nhà cung cấp</label>
            <input class="form-control" v-model="phieu.nhaCungCap" placeholder="Ví dụ: Công ty dược A" />
          </div>
          <div class="col-md-6 mb-3">
            <label class="small text-muted font-weight-bold">Người nhập</label>
            <input class="form-control" v-model="phieu.nguoiNhap" />
          </div>
          <div class="col-md-4 mb-3">
            <label class="small text-muted font-weight-bold">Ngày nhập</label>
            <input type="date" class="form-control" v-model="phieu.ngayNhap" />
          </div>
          <div class="col-md-8 mb-3">
            <label class="small text-muted font-weight-bold">Ghi chú</label>
            <input class="form-control" v-model="phieu.ghiChu" placeholder="Ghi chú lô / phiếu nhập kho..." />
          </div>
        </div>

        <hr class="my-4">

        <!-- ══════════════════════════════════════════════
            CHẾ ĐỘ A: THUỐC CÓ SẴN (giữ nguyên giao diện cũ)
        ══════════════════════════════════════════════ -->
        <div v-if="!laThuocMoi">
          <div class="row bg-light p-3 rounded mb-3">
            <div class="col-md-8 mb-3">
              <label class="small text-muted font-weight-bold">Chọn thuốc</label>
              <input class="form-control" v-model="timThuoc" placeholder="Gõ tên hoặc hoạt chất..."
                @input="timKiemThuoc" />
              <div v-if="ketQuaTim.length > 0" class="pos-autocomplete-list shadow">
                <div v-for="sp in ketQuaTim" :key="sp.maThuoc" class="pos-autocomplete-item" @click="chonThuoc(sp)">
                  {{ sp.tenThuoc }}
                </div>
              </div>
              <div class="small text-primary mt-2" v-if="thuocChon">
                Đã chọn: <strong>{{ thuocChon.tenThuoc }}</strong>
              </div>
            </div>
            <div class="col-md-4 mb-3">
              <label class="small text-muted font-weight-bold">Đơn vị tính</label>
              <select class="form-control" v-model="dongNhap.tenDonVi">
                <option>Hộp</option>
                <option>Vỉ</option>
                <option>Viên</option>
                <option>Gói</option>
                <option>Lọ</option>
                <option>Chai</option>
              </select>
            </div>
            <div class="col-md-3 mb-3">
              <label class="small text-muted font-weight-bold">Số lô</label>
              <input class="form-control" v-model="dongNhap.soLo" placeholder="Ví dụ: BN9902" />
            </div>
            <div class="col-md-3 mb-3">
              <label class="small text-muted font-weight-bold">Hạn sử dụng</label>
              <input type="date" class="form-control" v-model="dongNhap.hanSuDung" />
            </div>
            <div class="col-md-3 mb-3">
              <label class="small text-muted font-weight-bold">Giá nhập (đ)</label>
              <input type="number" class="form-control" v-model.number="dongNhap.giaNhap" />
            </div>
            <div class="col-md-3 mb-3">
              <label class="small text-muted font-weight-bold">Số lượng</label>
              <input type="number" class="form-control" v-model.number="dongNhap.soLuong" />
            </div>
            <div class="col-12">
              <button type="button" class="btn btn-outline-primary" @click="themDong">
                <i class="fas fa-plus mr-1"></i> Thêm vào danh sách chờ
              </button>
              <span v-if="loiThem" class="text-danger ml-3 small">{{ loiThem }}</span>
            </div>
          </div>

          <!-- Bảng danh sách chờ -->
          <div class="table-responsive">
            <table v-if="danhSachNhap.length > 0" class="table table-bordered table-hover">
              <thead class="thead-light">
                <tr>
                  <th>Số lô</th>
                  <th>Hạn dùng</th>
                  <th>Số lượng</th>
                  <th>Giá nhập</th>
                  <th>Thuốc</th>
                  <th width="50">Xóa</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(d, i) in danhSachNhap" :key="i">
                  <td>{{ d.soLo }}</td>
                  <td>{{ d.hanSuDung }}</td>
                  <td>{{ d.soLuong }} {{ d.tenDonVi }}</td>
                  <td>{{ formatGia(d.giaNhap) }}</td>
                  <td>{{ d.tenThuoc }}</td>
                  <td class="text-center">
                    <button class="btn btn-danger btn-sm" @click="xoaDong(i)">
                      <i class="fas fa-trash"></i>
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
            <div v-else class="text-center text-muted py-4 border rounded bg-white">Chưa có dòng nhập nào.</div>
          </div>
        </div>

        <!-- ══════════════════════════════════════════════
              CHẾ ĐỘ B: THUỐC MỚI
        ══════════════════════════════════════════════ -->
        <div v-else>
          <div class="alert alert-info py-2 mb-3 small">
            <i class="fas fa-info-circle mr-1"></i>
            Điền thông tin thuốc mới bên dưới. Thuốc sẽ được tạo và nhập kho cùng lúc.
          </div>

          <div class="row bg-light p-3 rounded mb-3 border">
            <div class="col-12 mb-3">
              <span class="small font-weight-bold text-primary text-uppercase">
                <i class="fas fa-pills mr-1"></i> 1. Thông tin thuốc
              </span>
            </div>
            <div class="col-md-6 mb-3">
              <label class="small text-muted font-weight-bold">Tên thuốc <span class="text-danger">*</span></label>
              <input class="form-control" v-model="thuocMoi.tenThuoc" placeholder="Ví dụ: Paracetamol 500mg" />
            </div>
            <div class="col-md-6 mb-3">
              <label class="small text-muted font-weight-bold">Danh mục</label>
              <select class="form-control" v-model="thuocMoi.maDanhMuc">
                <option :value="null">— Chưa phân loại —</option>
                <option v-for="dm in danhSachDanhMuc" :key="dm.maDanhMuc" :value="dm.maDanhMuc">{{ dm.tenDanhMuc }}
                </option>
              </select>
            </div>
            <div class="col-md-4 mb-3">
              <label class="small text-muted font-weight-bold">Nhà sản xuất</label>
              <input class="form-control" v-model="thuocMoi.nhaSanXuat" />
            </div>
            <div class="col-md-4 mb-3">
              <label class="small text-muted font-weight-bold">Số đăng ký</label>
              <input class="form-control" v-model="thuocMoi.soDangKy" />
            </div>
            <div class="col-md-4 mb-3 d-flex align-items-end pb-2">
              <div class="custom-control custom-checkbox">
                <input type="checkbox" class="custom-control-input" v-model="thuocMoi.laThuocKeDon" id="chkKeDon">
                <label class="custom-control-label font-weight-bold small text-danger" for="chkKeDon">Thuốc kê đơn
                  (Rx)</label>
              </div>
            </div>
            <div class="col-md-12 mb-3">
              <label class="small text-muted font-weight-bold">Thành phần & Hàm lượng</label>
              <input class="form-control" v-model="thuocMoi.thanhPhan" placeholder="Ví dụ: Amoxicillin 500mg..." />
            </div>
          </div>

          <div class="row bg-white p-3 rounded mb-3 border shadow-sm">
            <div class="col-12 mb-3 border-bottom pb-2">
              <span class="small font-weight-bold text-primary text-uppercase">
                <i class="fas fa-cubes mr-1"></i> 2. Chi tiết nhập kho & Đơn vị
              </span>
            </div>

            <div class="col-md-3 mb-3">
              <label class="small text-muted font-weight-bold">Đơn vị tính <span class="text-danger">*</span></label>
              <select class="form-control" v-model="dongMoi.tenDonVi">
                <option>Hộp</option>
                <option>Vỉ</option>
                <option>Viên</option>
                <option>Gói</option>
                <option>Lọ</option>
                <option>Chai</option>
              </select>
            </div>
            <div class="col-md-3 mb-3">
              <label class="small text-muted font-weight-bold">Số lô nhập <span class="text-danger">*</span></label>
              <input class="form-control" v-model="dongMoi.soLo" placeholder="Ví dụ: LO2024" />
            </div>
            <div class="col-md-3 mb-3">
              <label class="small text-muted font-weight-bold">Hạn sử dụng <span class="text-danger">*</span></label>
              <input type="date" class="form-control" v-model="dongMoi.hanSuDung" />
            </div>
            <div class="col-md-3 mb-3">
              <label class="small text-muted font-weight-bold">Giá nhập (đ)</label>
              <input type="number" class="form-control font-weight-bold text-primary"
                v-model.number="dongMoi.giaNhap" />
            </div>

            <div class="col-md-2 mb-3">
              <label class="small text-muted font-weight-bold">Số lượng nhập</label>
              <input type="number" class="form-control" v-model.number="dongMoi.soLuong" />
            </div>
            <div class="col-md-2 mb-3">
              <label class="small text-muted font-weight-bold">Giá bán ra (đ)</label>
              <input type="number" class="form-control" v-model.number="dongMoi.giaBan" />
            </div>
            <div class="col-md-2 mb-3">
              <label class="small text-muted font-weight-bold">Tỷ lệ quy đổi</label>
              <input type="number" class="form-control" v-model.number="dongMoi.giaTriQuyDoi" />
            </div>
            <div class="col-md-3 mb-3 d-flex align-items-end pb-2">
              <div class="custom-control custom-switch">
                <input type="checkbox" class="custom-control-input" v-model="dongMoi.laDonViCoBan" id="chkCoBan">
                <label class="custom-control-label small font-weight-bold" for="chkCoBan">Đơn vị cơ bản (Nhỏ
                  nhất)</label>
              </div>
            </div>
            <div class="col-md-3 mb-3 d-flex align-items-end">
              <button type="button" class="btn btn-primary btn-block" @click="themDongMoi">
                <i class="fas fa-plus mr-1"></i> Thêm đơn vị
              </button>
            </div>

            <div class="col-12" v-if="loiThemMoi">
              <span class="text-danger small"><i class="fas fa-exclamation-triangle mr-1"></i> {{ loiThemMoi }}</span>
            </div>
          </div>

          <div class="table-responsive shadow-sm">
            <table v-if="danhSachDonViMoi.length > 0" class="table table-bordered table-hover bg-white">
            </table>
          </div>
        </div>

        <!-- ══════════════════════════════════════════════
             NÚT XÁC NHẬN + THÔNG BÁO (dùng chung)
        ══════════════════════════════════════════════ -->
        <div class="mt-4 d-flex justify-content-between align-items-center">
          <div>
            <p v-if="loiHoanTat" class="text-danger small mb-0">{{ loiHoanTat }}</p>
            <p v-if="thanhCong" class="text-success font-weight-bold mb-0">{{ thanhCong }}</p>
          </div>
          <button type="button" class="btn btn-success px-5 shadow"
            :disabled="dangLuu || (laThuocMoi ? danhSachDonViMoi.length === 0 : danhSachNhap.length === 0)"
            @click="hoanTat">
            <i class="fas fa-save mr-2"></i>
            {{ dangLuu ? 'Đang lưu...' : (laThuocMoi ? 'Tạo thuốc & Nhập kho' : 'Xác nhận nhập kho') }}
          </button>
        </div>

        <!-- Bảng lịch sử nhập kho -->
        <div v-if="lichSuNhapKho.length > 0" class="mt-5">
          <hr class="my-4">
          <h6 class="font-weight-bold text-secondary mb-3">
            <i class="fas fa-history mr-1"></i> Lịch sử nhập kho (phiên này)
          </h6>
          <div class="table-responsive">
            <table class="table table-bordered table-hover">
              <thead class="thead-light">
                <tr>
                  <th>#</th>
                  <th>Thời gian</th>
                  <th>Thuốc</th>
                  <th>Nhà cung cấp</th>
                  <th>Người nhập</th>
                  <th>Số mặt hàng</th>
                  <th class="text-center">Mã vạch</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(ls, i) in lichSuNhapKho" :key="i">
                  <td>{{ lichSuNhapKho.length - i }}</td>
                  <td>{{ ls.thoiGian }}</td>
                  <td>
                    <span v-if="ls.laThuocMoi" class="badge badge-info mr-1">Mới</span>
                    {{ ls.tenThuoc || '—' }}
                  </td>
                  <td>{{ ls.nhaCungCap || '—' }}</td>
                  <td>{{ ls.nguoiNhap || '—' }}</td>
                  <td>{{ ls.chiTiet.length }} mặt hàng</td>
                  <td class="text-center">
                    <button class="btn btn-warning btn-sm" @click="xemMaVach(ls.chiTiet)">
                      <i class="fas fa-barcode mr-1"></i> Xem & In
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

      </div>
    </div>

    <!-- MODAL IN MÃ VẠCH -->
    <div v-if="hienModalIn" class="custom-modal-overlay">
      <div class="custom-modal-content">
        <div class="modal-header bg-dark text-white">
          <h5 class="m-0">In mã vạch sản phẩm</h5>
          <button class="btn-close-white" @click="hienModalIn = false">&times;</button>
        </div>
        <div class="modal-body p-4" id="vung-in-tem">
          <div class="barcode-grid">
            <div v-for="(tem, idx) in danhSachTemIn" :key="idx" class="barcode-item">
              <div class="tem-ten-thuoc">{{ tem.tenThuoc }}</div>
              <img :src="tem.hinhAnhMaVach" class="tem-image" v-if="tem.hinhAnhMaVach" />
              <div class="tem-ma-so">{{ tem.maVach }}</div>
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary" @click="hienModalIn = false">Đóng</button>
          <button class="btn btn-primary px-4" @click="inTemAction">
            <i class="fas fa-print mr-1"></i> Bắt đầu in
          </button>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue';
import axiosClient from '../../api/axiosClient';

// PHIẾU NHẬP (dùng chung)
const phieu = reactive({
  nhaCungCap: '',
  nguoiNhap: '',
  ngayNhap: new Date().toISOString().split('T')[0],
  ghiChu: ''
});

// CHẾ ĐỘ
const laThuocMoi = ref(false);

const chuyenCheDoc = () => {
  laThuocMoi.value = !laThuocMoi.value;
  loiHoanTat.value = '';
  thanhCong.value = '';
};

// THUỐC CÓ SẴN
const timThuoc = ref('');
const ketQuaTim = ref([]);
const thuocChon = ref(null);
const danhSachNhap = ref([]);
const dongNhap = reactive({ soLo: '', hanSuDung: '', giaNhap: 0, soLuong: 1, tenDonVi: 'Hộp' });
const loiThem = ref('');

const dongNhapRong = () => ({ soLo: '', hanSuDung: '', giaNhap: 0, soLuong: 1, tenDonVi: 'Hộp' });

let timer = null;
const timKiemThuoc = () => {
  clearTimeout(timer);
  if (!timThuoc.value.trim()) { ketQuaTim.value = []; return; }
  timer = setTimeout(async () => {
    try {
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

const themDong = () => {
  loiThem.value = '';
  if (!thuocChon.value) { loiThem.value = 'Vui lòng chọn thuốc.'; return; }
  if (!dongNhap.soLo.trim()) { loiThem.value = 'Nhập số lô.'; return; }
  danhSachNhap.value.push({
    maThuoc: thuocChon.value.maThuoc,
    tenThuoc: thuocChon.value.tenThuoc,
    soLo: dongNhap.soLo,
    hanSuDung: dongNhap.hanSuDung,
    giaNhap: Number(dongNhap.giaNhap),
    soLuong: Number(dongNhap.soLuong),
    tenDonVi: dongNhap.tenDonVi,
  });
  Object.assign(dongNhap, dongNhapRong());
  thuocChon.value = null;
  timThuoc.value = '';
};

const xoaDong = (i) => danhSachNhap.value.splice(i, 1);

// THUỐC MỚI
const thuocMoi = reactive({
  tenThuoc: '', maDanhMuc: null, nhaSanXuat: '', nuocSanXuat: '',
  dangBaoChe: '', soDangKy: '', quyCach: '', thanhPhan: '',
  moTaNgan: '', laThuocKeDon: false
});

const dongMoiRong = () => ({
  tenDonVi: 'Hộp', giaBan: 0, giaTriQuyDoi: 1,
  laDonViCoBan: false, soLo: '', hanSuDung: '', giaNhap: 0, soLuong: 1
});
const dongMoi = reactive(dongMoiRong());
const danhSachDonViMoi = ref([]);
const loiThemMoi = ref('');
const danhSachDanhMuc = ref([]);

const themDongMoi = () => {
  loiThemMoi.value = '';
  if (!dongMoi.tenDonVi) { loiThemMoi.value = 'Chọn đơn vị tính.'; return; }
  if (!dongMoi.soLo.trim()) { loiThemMoi.value = 'Nhập số lô.'; return; }
  if (!dongMoi.hanSuDung) { loiThemMoi.value = 'Nhập hạn sử dụng.'; return; }
  if (dongMoi.soLuong <= 0) { loiThemMoi.value = 'Số lượng phải lớn hơn 0.'; return; }
  danhSachDonViMoi.value.push({ ...dongMoi });
  Object.assign(dongMoi, dongMoiRong());
};

const xoaDongMoi = (i) => danhSachDonViMoi.value.splice(i, 1);

// TRẠNG THÁI CHUNG
const dangLuu = ref(false);
const loiHoanTat = ref('');
const thanhCong = ref('');

// LỊCH SỬ & IN
const lichSuNhapKho = ref([]);
const hienModalIn = ref(false);
const danhSachTemIn = ref([]);

const xemMaVach = (chiTiet) => {
  danhSachTemIn.value = chiTiet;
  hienModalIn.value = true;
};

// HÀM GỬI
const hoanTat = () => laThuocMoi.value ? hoanTatThuocMoi() : hoanTatNhapKho();

// Nhập kho thuốc có sẵn (giữ nguyên logic cũ)
const hoanTatNhapKho = async () => {
  loiHoanTat.value = '';
  thanhCong.value = '';
  dangLuu.value = true;
  try {
    const payload = {
      NhaCungCap: phieu.nhaCungCap,
      NguoiNhap: phieu.nguoiNhap,
      NgayNhap: new Date(phieu.ngayNhap).toISOString(),
      GhiChu: phieu.ghiChu,
      ChiTiet: danhSachNhap.value.map(x => ({
        MaThuoc: x.maThuoc,
        SoLo: x.soLo,
        HanSuDung: new Date(x.hanSuDung).toISOString(),
        GiaNhap: x.giaNhap,
        SoLuong: x.soLuong,
        TenDonVi: x.tenDonVi
      }))
    };
    const response = await axiosClient.post('/Kho/nhap-kho', payload);
    const serverRes = response?.status ? response : response?.data;
    if (serverRes?.status === 'success') {
      const chiTiet = serverRes.data?.ChiTiet ?? serverRes.data?.chiTiet ?? [];
      lichSuNhapKho.value.unshift({
        thoiGian: new Date().toLocaleString('vi-VN'),
        tenThuoc: null,
        laThuocMoi: false,
        nhaCungCap: phieu.nhaCungCap,
        nguoiNhap: phieu.nguoiNhap,
        ghiChu: phieu.ghiChu,
        chiTiet
      });
      thanhCong.value = serverRes.message || 'Nhập kho thành công!';
      danhSachNhap.value = [];
    } else {
      loiHoanTat.value = 'Lưu thành công nhưng phản hồi sai cấu trúc.';
    }
  } catch (err) {
    console.error(err);
    loiHoanTat.value = err.response?.data?.message || 'Lỗi hệ thống khi lưu.';
  } finally {
    dangLuu.value = false;
  }
};

// Tạo thuốc mới + nhập kho
const hoanTatThuocMoi = async () => {
  loiHoanTat.value = '';
  thanhCong.value = '';

  if (!thuocMoi.tenThuoc.trim()) { loiHoanTat.value = 'Tên thuốc không được để trống.'; return; }
  if (danhSachDonViMoi.value.length === 0) { loiHoanTat.value = 'Thêm ít nhất một đơn vị tính.'; return; }
  if (!danhSachDonViMoi.value.some(x => x.laDonViCoBan)) {
    loiHoanTat.value = 'Phải đánh dấu ít nhất một đơn vị tính là cơ bản.'; return;
  }

  dangLuu.value = true;
  try {
    const payload = {
      NhaCungCap: phieu.nhaCungCap,
      NguoiNhap: phieu.nguoiNhap,
      NgayNhap: new Date(phieu.ngayNhap).toISOString(),
      GhiChu: phieu.ghiChu,
      TenThuoc: thuocMoi.tenThuoc,
      MaDanhMuc: thuocMoi.maDanhMuc,
      NhaSanXuat: thuocMoi.nhaSanXuat,
      NuocSanXuat: thuocMoi.nuocSanXuat,
      DangBaoChe: thuocMoi.dangBaoChe,
      SoDangKy: thuocMoi.soDangKy,
      QuyCach: thuocMoi.quyCach,
      ThanhPhan: thuocMoi.thanhPhan,
      MoTaNgan: thuocMoi.moTaNgan,
      LaThuocKeDon: thuocMoi.laThuocKeDon,
      ChiTiet: danhSachDonViMoi.value.map(x => ({
        TenDonVi: x.tenDonVi,
        GiaBan: x.giaBan,
        GiaTriQuyDoi: x.giaTriQuyDoi,
        LaDonViCoBan: x.laDonViCoBan,
        SoLo: x.soLo,
        HanSuDung: new Date(x.hanSuDung).toISOString(),
        GiaNhap: x.giaNhap,
        SoLuong: x.soLuong
      }))
    };

    const response = await axiosClient.post('/Kho/nhap-kho-thuoc-moi', payload);
    const serverRes = response?.status ? response : response?.data;

    if (serverRes?.status === 'success') {
      const chiTiet = serverRes.data?.chiTiet ?? serverRes.data?.ChiTiet ?? [];
      lichSuNhapKho.value.unshift({
        thoiGian: new Date().toLocaleString('vi-VN'),
        tenThuoc: thuocMoi.tenThuoc,
        laThuocMoi: true,
        nhaCungCap: phieu.nhaCungCap,
        nguoiNhap: phieu.nguoiNhap,
        ghiChu: phieu.ghiChu,
        chiTiet
      });
      thanhCong.value = serverRes.message || 'Tạo thuốc và nhập kho thành công!';

      // Reset form thuốc mới
      Object.assign(thuocMoi, {
        tenThuoc: '', maDanhMuc: null, nhaSanXuat: '', nuocSanXuat: '',
        dangBaoChe: '', soDangKy: '', quyCach: '', thanhPhan: '',
        moTaNgan: '', laThuocKeDon: false
      });
      danhSachDonViMoi.value = [];
    } else {
      loiHoanTat.value = 'Lưu thành công nhưng phản hồi sai cấu trúc.';
    }
  } catch (err) {
    console.error(err);
    loiHoanTat.value = err.response?.data?.message || 'Lỗi hệ thống khi lưu.';
  } finally {
    dangLuu.value = false;
  }
};

// HÀM IN
const inTemAction = () => {
  const vungIn = document.getElementById('vung-in-tem').innerHTML;
  const cuaSoIn = window.open('', '_blank');
  cuaSoIn.document.write(`
    <html><head><style>
      .barcode-grid { display: flex; flex-wrap: wrap; gap: 10px; justify-content: center; }
      .barcode-item { border: 1px solid #333; padding: 10px; width: 160px; text-align: center; margin: 5px; }
      .tem-ten-thuoc { font-size: 11px; font-weight: bold; margin-bottom: 5px; }
      .tem-image { width: 100%; height: auto; }
      .tem-ma-so { font-size: 10px; margin-top: 3px; }
    </style></head><body>${vungIn}</body></html>
  `);
  cuaSoIn.document.close();
  setTimeout(() => { cuaSoIn.print(); cuaSoIn.close(); }, 500);
};

const formatGia = (v) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v ?? 0);

// LOAD DANH MỤC (cho dropdown thuốc mới)
const loadDanhMuc = async () => {
  try {
    const data = await axiosClient.get('/Kho/danh-muc');
    danhSachDanhMuc.value = Array.isArray(data) ? data : [];
  } catch (err) { console.error(err); }
};

onMounted(loadDanhMuc);
</script>

<style scoped>
/* Modal Overlay */
.custom-modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2000;
}

.custom-modal-content {
  background: white;
  width: 700px;
  border-radius: 8px;
  overflow: hidden;
}

.btn-close-white {
  background: none;
  border: none;
  color: white;
  font-size: 24px;
  cursor: pointer;
}

/* Grid Tem */
.barcode-grid {
  display: flex;
  flex-wrap: wrap;
  gap: 15px;
  justify-content: center;
  max-height: 400px;
  overflow-y: auto;
  padding: 10px;
}

.barcode-item {
  border: 1px dashed #ccc;
  padding: 10px;
  width: 170px;
  text-align: center;
  background: #f9f9f9;
}

.tem-image {
  width: 100%;
  height: auto;
  margin: 5px 0;
}

.tem-ten-thuoc {
  font-size: 11px;
  font-weight: bold;
  color: #333;
  overflow: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
}

/* Autocomplete */
.pos-autocomplete-list {
  position: absolute;
  z-index: 1000;
  background: white;
  border: 1px solid #ddd;
  width: 100%;
  max-height: 200px;
  overflow-y: auto;
}

.pos-autocomplete-item {
  padding: 8px 12px;
  cursor: pointer;
  border-bottom: 1px solid #eee;
}

.pos-autocomplete-item:hover {
  background: #f1f1f1;
}

/* Toggle switch */
.custom-switch-wrap {
  cursor: pointer;
}

.custom-switch-track {
  width: 40px;
  height: 22px;
  background: rgba(255, 255, 255, 0.3);
  border-radius: 11px;
  position: relative;
  transition: background 0.2s;
}

.custom-switch-track.active {
  background: #ffc107;
}

.custom-switch-thumb {
  position: absolute;
  top: 3px;
  left: 3px;
  width: 16px;
  height: 16px;
  background: white;
  border-radius: 50%;
  transition: left 0.2s;
}

.custom-switch-track.active .custom-switch-thumb {
  left: 21px;
}

/* Hiệu ứng nút in */
.btn-pulse {
  animation: pulse-yellow 2s infinite;
  box-shadow: 0 0 0 0 rgba(255, 193, 7, 0.7);
}

@keyframes pulse-yellow {
  0% {
    transform: scale(0.95);
    box-shadow: 0 0 0 0 rgba(255, 193, 7, 0.7);
  }

  70% {
    transform: scale(1);
    box-shadow: 0 0 0 10px rgba(255, 193, 7, 0);
  }

  100% {
    transform: scale(0.95);
    box-shadow: 0 0 0 0 rgba(255, 193, 7, 0);
  }
}
</style>
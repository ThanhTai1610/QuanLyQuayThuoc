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
            <div class="col-md-3 mb-3">
              <label class="small text-muted font-weight-bold">Nhà sản xuất</label>
              <input class="form-control" v-model="thuocMoi.nhaSanXuat" placeholder="Ví dụ: Medley..." />
            </div>
            <div class="col-md-3 mb-3">
              <label class="small text-muted font-weight-bold">Nước sản xuất</label>
              <input class="form-control" v-model="thuocMoi.nuocSanXuat" placeholder="Ví dụ: Ấn Độ..." />
            </div>
            <div class="col-md-3 mb-3">
              <label class="small text-muted font-weight-bold">Dạng bào chế</label>
              <select class="form-control" v-model="thuocMoi.dangBaoChe">
                <option value="">— Chọn dạng bào chế —</option>
                <option v-for="item in listDangBaoChe" :key="item" :value="item">{{ item }}</option>
              </select>
            </div>
            <div class="col-md-3 mb-3">
              <label class="small text-muted font-weight-bold">Số đăng ký</label>
              <input class="form-control" v-model="thuocMoi.soDangKy" />
            </div>
            <div class="col-md-6 mb-3">
              <label class="small text-muted font-weight-bold">Quy cách đóng gói (Dùng để sinh đơn vị tính)</label>
              <div class="d-flex">
                <select class="form-control mr-2" v-model="quyCachLuaChon" style="flex: 1;">
                  <option value="">— Chọn quy cách —</option>
                  <option v-for="qc in listQuyCach" :key="qc" :value="qc">{{ qc }}</option>
                  <option value="Khác">Quy cách khác...</option>
                </select>
                <input v-if="quyCachLuaChon === 'Khác'" class="form-control" v-model="thuocMoi.quyCach" 
                       placeholder="Nhập quy cách mới..." style="flex: 1;" />
              </div>
            </div>
            <div class="col-md-6 mb-3 d-flex align-items-end pb-2">
              <div class="custom-control custom-checkbox">
                <input type="checkbox" class="custom-control-input" v-model="thuocMoi.laThuocKeDon" id="chkKeDon">
                <label class="custom-control-label font-weight-bold small text-danger" for="chkKeDon">Thuốc kê đơn (Rx)</label>
              </div>
            </div>
            <div class="col-md-12 mb-3">
              <label class="small text-muted font-weight-bold">Thành phần & Hàm lượng</label>
              <input class="form-control" v-model="thuocMoi.thanhPhan" placeholder="Ví dụ: Amoxicillin 500mg..." />
            </div>
            <div class="col-md-12 mb-3">
              <label class="small text-muted font-weight-bold">Mô tả ngắn</label>
              <textarea class="form-control" v-model="thuocMoi.moTaNgan" rows="2" placeholder="Công dụng chính, cách dùng sơ lược..."></textarea>
            </div>

            <!-- Upload hình ảnh -->
            <div class="col-12 mt-2">
              <label class="small text-muted font-weight-bold mb-2 d-block">Hình ảnh thuốc</label>
              <div class="d-flex align-items-start">
                <div class="drug-image-preview border rounded mr-3 d-flex align-items-center justify-content-center bg-white" 
                     style="width: 120px; height: 120px; overflow: hidden; border-style: dashed !important;">
                  <img v-if="thuocMoi.hinhAnh" :src="thuocMoi.hinhAnh" style="width: 100%; height: 100%; object-fit: cover;" />
                  <i v-else class="fas fa-image text-muted fa-2x"></i>
                </div>
                <div>
                  <input type="file" id="fileThuoc" class="d-none" accept="image/*" @change="handleFileUpload" />
                  <label for="fileThuoc" class="btn btn-outline-secondary btn-sm mb-2">
                    <i class="fas fa-upload mr-1"></i> Chọn ảnh
                  </label>
                  <button v-if="thuocMoi.hinhAnh" type="button" class="btn btn-link btn-sm text-danger d-block p-0" @click="thuocMoi.hinhAnh = ''">
                    Xóa ảnh
                  </button>
                  <p class="small text-muted mb-0 mt-1">Dung lượng tối đa 2MB. Hỗ trợ JPG, PNG.</p>
                </div>
              </div>
            </div>
          </div>

          <div class="row bg-white p-3 rounded mb-3 border shadow-sm">
            <div class="col-12 mb-3 border-bottom pb-2">
              <span class="small font-weight-bold text-primary text-uppercase">
                <i class="fas fa-cubes mr-1"></i> 2. Chi tiết nhập kho & Đơn vị tính
              </span>
            </div>

            <!-- Nhập lô tập trung -->
            <div class="col-md-3 mb-3">
              <label class="small text-muted font-weight-bold">Số lô nhập <span class="text-danger">*</span></label>
              <input class="form-control font-weight-bold" v-model="thuocMoi.soLo" placeholder="Ví dụ: LO2024" />
            </div>
            <div class="col-md-3 mb-3">
              <label class="small text-muted font-weight-bold">Hạn sử dụng <span class="text-danger">*</span></label>
              <input type="date" class="form-control" v-model="thuocMoi.hanSuDung" />
            </div>
            <div class="col-md-3 mb-3">
              <label class="small text-muted font-weight-bold">Giá nhập đơn vị <span class="text-danger">*</span></label>
              <input type="number" class="form-control font-weight-bold text-primary" v-model.number="thuocMoi.giaNhap" @input="goiYGiaBan" />
              <small class="text-muted">Cho 1 {{ thuocMoi.tenDonViNhap || 'đơn vị' }}</small>
            </div>
            <div class="col-md-3 mb-3">
              <label class="small text-muted font-weight-bold">Số lượng nhập <span class="text-danger">*</span></label>
              <input type="number" class="form-control font-weight-bold text-primary" v-model.number="thuocMoi.soLuong" />
            </div>

            <div class="col-md-4 mb-3">
              <label class="small text-muted font-weight-bold">Bạn đang nhập theo đơn vị nào?</label>
              <select class="form-control border-primary" v-model="thuocMoi.tenDonViNhap">
                <option v-for="u in danhSachDonViMoi" :key="u.tenDonVi" :value="u.tenDonVi">{{ u.tenDonVi }}</option>
              </select>
            </div>

            <div class="col-12 mt-3">
              <label class="small text-muted font-weight-bold uppercase">Bảng quy đổi đơn vị tính (Xác định giá bán)</label>
              <div class="table-responsive">
                <table class="premium-table">
                  <thead class="thead-dark small">
                    <tr>
                      <th>Tên đơn vị</th>
                      <th>Giá bán (đ)</th>
                      <th width="100">Cơ bản?</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="(u, idx) in danhSachDonViMoi" :key="idx">
                      <td><input class="form-control form-control-sm" v-model="u.tenDonVi" /></td>
                      <td><input type="number" class="form-control form-control-sm" v-model.number="u.giaBan" 
                                 @input="capNhatGiaBanNhanh(idx)" /></td>
                      <td class="text-center">
                        <input type="radio" :value="true" v-model="u.laDonViCoBan" @change="setCoBan(idx)" />
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
              <button type="button" class="btn btn-outline-secondary btn-sm" @click="themDonViThuCong">
                <i class="fas fa-plus mr-1"></i> Thêm đơn vị thủ công
              </button>
            </div>

            <div class="col-12 mt-3" v-if="loiThemMoi">
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
          <div class="table-responsive rounded-lg overflow-hidden">
            <table class="premium-table">
              <thead class="thead-light">
                <tr>
                  <th>#</th>
                  <th>Thời gian</th>
                  <th>Thuốc đầu</th>
                  <th>Nhà cung cấp</th>
                  <th>Số mặt hàng</th>
                  <th class="text-center">Hành động</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(ls, i) in lichSuNhapKho" :key="i">
                  <td>{{ lichSuNhapKho.length - i }}</td>
                  <td>{{ ls.thoiGian }}</td>
                  <td>
                    <span v-if="ls.laThuocMoi" class="badge badge-info mr-1">Mới</span>
                    {{ ls.tenThuoc || (ls.chiTiet[0]?.tenThuoc || '—') }}
                  </td>
                  <td>{{ ls.nhaCungCap || '—' }}</td>
                  <td class="font-weight-bold text-primary">{{ ls.chiTiet.length }} mặt hàng</td>
                  <td class="text-center">
                    <button class="btn btn-warning btn-sm shadow-sm" @click="moChiTietPhieu(ls)">
                      <i class="fas fa-eye mr-1"></i> Chi tiết & Mã vạch
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

      </div>
    </div>



    <!-- MODAL CHI TIẾT PHIẾU NHẬP (GỘP MÃ VẠCH) -->
    <div v-if="hienModalChiTiet" class="custom-modal-overlay">
      <div class="custom-modal-content" style="width: 850px; max-height: 90vh; display: flex; flex-direction: column;">
        <div class="modal-header bg-dark text-white">
          <h5 class="m-0"><i class="fas fa-file-invoice mr-2"></i>Chi tiết phiếu nhập & Mã vạch</h5>
          <button class="btn-close-white" @click="hienModalChiTiet = false">
            <i class="fas fa-times"></i>
          </button>
        </div>
        <div class="modal-body p-0" style="overflow-y: auto;">
          <!-- Thông tin chung -->
          <div class="p-3 bg-light border-bottom d-flex justify-content-between align-items-center">
            <div>
              <span class="small text-muted d-block font-weight-bold">Nhà cung cấp</span>
              <strong class="text-primary">{{ phieuHienTai.nhaCungCap || 'Chưa xác định' }}</strong>
            </div>
            <div class="text-right">
              <span class="small text-muted d-block font-weight-bold">Thời gian nhập kho</span>
              <strong>{{ phieuHienTai.thoiGian }}</strong>
            </div>
          </div>

          <!-- Bảng danh sách thuốc -->
          <div class="p-3 border-bottom">
            <h6 class="small font-weight-bold text-muted mb-2 text-uppercase">1. Danh sách mặt hàng</h6>
            <div class="table-responsive rounded-lg overflow-hidden">
              <table class="premium-table shadow-none" style="border: none;">
                <thead class="bg-light">
                  <tr>
                    <th class="pl-2">Tên thuốc</th>
                    <th>Số lô</th>
                    <th>Hạn dùng</th>
                    <th class="text-right">Số lượng</th>
                    <th class="text-right pr-2">Giá nhập</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(item, idx) in phieuHienTai.chiTiet" :key="idx">
                    <td class="pl-2 font-weight-bold text-dark">{{ item.tenThuoc }}</td>
                    <td><span class="badge badge-secondary">{{ item.soLo }}</span></td>
                    <td class="small">{{ item.hanSuDung ? new Date(item.hanSuDung).toLocaleDateString('vi-VN') : '—' }}</td>
                    <td class="text-right font-weight-bold">{{ item.soLuong }} {{ item.tenDonVi }}</td>
                    <td class="text-right pr-2">{{ formatGia(item.giaNhap) }}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- Phần Mã vạch (Gộp vào đây) -->
          <div class="p-3 bg-white" id="vung-in-tem">
            <h6 class="small font-weight-bold text-muted mb-3 text-uppercase">2. Xem trước mã vạch thương mại</h6>
            <div class="barcode-grid">
              <div v-for="(tem, idx) in phieuHienTai.chiTiet" :key="idx" class="barcode-item">
                <div class="tem-ten-thuoc">{{ tem.tenThuoc }}</div>
                <img :src="tem.hinhAnhMaVach" class="tem-image" v-if="tem.hinhAnhMaVach" />
                <div class="tem-ma-so">{{ tem.maVach }}</div>
              </div>
            </div>
          </div>
        </div>
        <div class="modal-footer bg-light">
          <button class="btn btn-secondary px-4" @click="hienModalChiTiet = false">Đóng</button>
          <button class="btn btn-warning px-4 shadow-sm" @click="inTemAction">
            <i class="fas fa-print mr-2"></i> In tất cả mã vạch
          </button>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { ref, reactive, onMounted, watch } from 'vue';
import axiosClient from '../../api/axiosClient';

// DANH MỤC LỰA CHỌN CỐ ĐỊNH
const listDangBaoChe = [
  'Viên', 'Hộp', 'Vỉ', 'Chai', 'Gói', 'Lọ'
];

const listQuyCach = [
  'Hộp 10 vỉ x 10 viên',
  'Hộp 10 vỉ x 15 viên',
  'Hộp 5 vỉ x 10 viên',
  'Hộp 3 vỉ x 10 viên',
  'Hộp 1 lọ x 30 viên',
  'Hộp 1 lọ x 100 viên',
  'Hộp 1 chai 100ml',
  'Hộp 1 tuýp 20g'
];

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
  moTaNgan: '', laThuocKeDon: false, hinhAnh: '',
  // Thông tin lô nhập một lần
  soLo: '', hanSuDung: '', giaNhap: 0, soLuong: 1, tenDonViNhap: ''
});

const quyCachLuaChon = ref('');

watch(quyCachLuaChon, (newVal) => {
  if (newVal !== 'Khác' && newVal !== '') {
    thuocMoi.quyCach = newVal;
    parseQuyCach(newVal);
  } else if (newVal === 'Khác') {
    if (listQuyCach.includes(thuocMoi.quyCach)) {
      thuocMoi.quyCach = '';
    }
  }
});

watch(() => thuocMoi.quyCach, (newVal) => {
  if (quyCachLuaChon.value === 'Khác') {
    parseQuyCach(newVal);
  }
});

const parseQuyCach = (str) => {
  if (!str) return;
  
  // Format 1: Hộp 10 vỉ x 10 viên
  const pattern1 = /^([^0-9]+)\s+(\d+)\s+([^0-9x]+)\s*[xX]\s*(\d+)\s+([^0-9]+)$/i;
  // Format 2: Hộp 30 viên
  const pattern2 = /^([^0-9]+)\s+(\d+)\s+([^0-9]+)$/i;
  // Format 3: Chai 100ml
  const pattern3 = /^([^0-9]+)\s+(\d+)(ml|g|l|mg)$/i;

  let units = [];
  let match1 = str.match(pattern1);
  if (match1) {
    const [_, u1, n1, u2, n2, u3] = match1;
    const num1 = parseInt(n1);
    const num2 = parseInt(n2);
    units.push({ tenDonVi: u1.trim(), giaTriQuyDoi: num1 * num2, giaBan: 0, laDonViCoBan: false });
    units.push({ tenDonVi: u2.trim(), giaTriQuyDoi: num2, giaBan: 0, laDonViCoBan: false });
    units.push({ tenDonVi: u3.trim(), giaTriQuyDoi: 1, giaBan: 0, laDonViCoBan: true });
  } else {
    let match2 = str.match(pattern2);
    if (match2) {
      const [_, u1, n1, u2] = match2;
      const num1 = parseInt(n1);
      units.push({ tenDonVi: u1.trim(), giaTriQuyDoi: num1, giaBan: 0, laDonViCoBan: false });
      units.push({ tenDonVi: u2.trim(), giaTriQuyDoi: 1, giaBan: 0, laDonViCoBan: true });
    } else {
      let match3 = str.match(pattern3);
      if (match3) {
        const [_, u1, n1, u2] = match3;
        units.push({ tenDonVi: u1.trim(), giaTriQuyDoi: 1, giaBan: 0, laDonViCoBan: true });
      } else {
        // Fallback or leave as is if manually entering
        return;
      }
    }
  }

  if (units.length > 0) {
    danhSachDonViMoi.value = units;
    thuocMoi.tenDonViNhap = units[0].tenDonVi;
    goiYGiaBan(); // Tính giá gợi ý ngay khi sinh đơn vị
  }
};

const capNhatGiaBanNhanh = (idx) => {
  const currentUnit = danhSachDonViMoi.value[idx];
  if (!currentUnit.giaBan || !currentUnit.giaTriQuyDoi) return;
  
  // Tính giá của 1 đơn vị cơ bản (hệ số 1)
  const giaCoBan = currentUnit.giaBan / currentUnit.giaTriQuyDoi;
  
  danhSachDonViMoi.value.forEach((u, i) => {
    if (i !== idx) {
      u.giaBan = Math.round(giaCoBan * u.giaTriQuyDoi);
    }
  });
};

const goiYGiaBan = () => {
  if (!thuocMoi.giaNhap || danhSachDonViMoi.value.length === 0) return;
  
  // Tính giá bán dựa trên lợi nhuận cố định 10%
  const giaBanGoiY = Math.round(thuocMoi.giaNhap * 1.1);
  
  const importIdx = danhSachDonViMoi.value.findIndex(u => u.tenDonVi === thuocMoi.tenDonViNhap);
  if (importIdx !== -1) {
    danhSachDonViMoi.value[importIdx].giaBan = giaBanGoiY;
    capNhatGiaBanNhanh(importIdx);
  } else {
    danhSachDonViMoi.value[0].giaBan = giaBanGoiY;
    capNhatGiaBanNhanh(0);
  }
};

const setCoBan = (idx) => {
  danhSachDonViMoi.value.forEach((u, i) => {
    u.laDonViCoBan = (i === idx);
  });
};

const themDonViThuCong = () => {
  danhSachDonViMoi.value.push({
    tenDonVi: '', giaTriQuyDoi: 1, giaBan: 0, laDonViCoBan: false
  });
};

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

const handleFileUpload = (e) => {
  const file = e.target.files[0];
  if (!file) return;
  if (file.size > 2 * 1024 * 1024) { alert('Ảnh quá lớn (tối đa 2MB)'); return; }

  const reader = new FileReader();
  reader.onload = (event) => {
    thuocMoi.hinhAnh = event.target.result;
  };
  reader.readAsDataURL(file);
};

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

const hienModalChiTiet = ref(false);
const phieuHienTai = ref({});

const moChiTietPhieu = (ls) => {
  phieuHienTai.value = ls;
  hienModalChiTiet.value = true;
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
      const rowData = serverRes.data?.ChiTiet ?? serverRes.data?.chiTiet ?? [];
      const chiTietCamel = rowData.map(x => ({
        maThuoc: x.MaThuoc || x.maThuoc,
        tenThuoc: x.TenThuoc || x.tenThuoc,
        soLo: x.SoLo || x.soLo,
        hanSuDung: x.HanSuDung || x.hanSuDung,
        giaNhap: x.GiaNhap || x.giaNhap,
        soLuong: x.SoLuong || x.soLuong,
        tenDonVi: x.TenDonVi || x.tenDonVi,
        maVach: x.MaVach || x.maVach,
        hinhAnhMaVach: x.HinhAnhMaVach || x.hinhAnhMaVach
      }));

      lichSuNhapKho.value.unshift({
        thoiGian: new Date().toLocaleString('vi-VN'),
        tenThuoc: null,
        laThuocMoi: false,
        nhaCungCap: phieu.nhaCungCap,
        nguoiNhap: phieu.nguoiNhap,
        ghiChu: phieu.ghiChu,
        chiTiet: chiTietCamel
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
      HinhAnh: thuocMoi.hinhAnh,
      ChiTiet: danhSachDonViMoi.value.map(x => ({
        TenDonVi: x.tenDonVi,
        GiaBan: x.giaBan,
        GiaTriQuyDoi: x.giaTriQuyDoi,
        LaDonViCoBan: x.laDonViCoBan
      })),
      // Thông tin lô nhập
      SoLo: thuocMoi.soLo,
      HanSuDung: new Date(thuocMoi.hanSuDung).toISOString(),
      GiaNhap: thuocMoi.giaNhap,
      SoLuong: thuocMoi.soLuong,
      TenDonViNhap: thuocMoi.tenDonViNhap
    };

    const response = await axiosClient.post('/Kho/nhap-kho-thuoc-moi', payload);
    const serverRes = response?.status ? response : response?.data;

    if (serverRes?.status === 'success') {
      const rowData = serverRes.data?.chiTiet ?? serverRes.data?.ChiTiet ?? [];
      
      const chiTietCamel = rowData.map(x => {
        return {
          tenThuoc: thuocMoi.tenThuoc,
          soLo: x.SoLo || x.soLo || thuocMoi.soLo,
          hanSuDung: x.HanSuDung || x.hanSuDung || thuocMoi.hanSuDung,
          giaNhap: x.GiaNhap || x.giaNhap || 0,
          soLuong: x.SoLuong || x.soLuong || 0,
          tenDonVi: x.TenDonVi || x.tenDonVi,
          maVach: x.MaVach || x.maVach,
          hinhAnhMaVach: x.HinhAnhMaVach || x.hinhAnhMaVach
        };
      });

      lichSuNhapKho.value.unshift({
        thoiGian: new Date().toLocaleString('vi-VN'),
        tenThuoc: thuocMoi.tenThuoc,
        laThuocMoi: true,
        nhaCungCap: phieu.nhaCungCap,
        nguoiNhap: phieu.nguoiNhap,
        ghiChu: phieu.ghiChu,
        chiTiet: chiTietCamel
      });
      thanhCong.value = serverRes.message || 'Tạo thuốc và nhập kho thành công!';

      // Reset form thuốc mới
      Object.assign(thuocMoi, {
        tenThuoc: '', maDanhMuc: null, nhaSanXuat: '', nuocSanXuat: '',
        dangBaoChe: '', soDangKy: '', quyCach: '', thanhPhan: '',
        moTaNgan: '', laThuocKeDon: false, hinhAnh: '',
        soLo: '', hanSuDung: '', giaNhap: 0, soLuong: 1, tenDonViNhap: ''
      });
      quyCachLuaChon.value = '';
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
/* ══════════════════════════════════════════════
    PREMIUM UI DESIGN BY ANTIGRAVITY
   ══════════════════════════════════════════════ */

/* Modal Overlay & Content */
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
  width: 900px;
  max-width: 95vw;
  border-radius: 20px;
  overflow: hidden;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
  animation: slideUp 0.4s cubic-bezier(0.16, 1, 0.3, 1);
}

.modal-header {
  padding: 1.25rem 1.5rem;
  border-bottom: none;
  background: linear-gradient(135deg, #1e293b 0%, #334155 100%);
  color: white;
}

.modal-header h5 {
  font-weight: 700;
  letter-spacing: -0.025em;
  font-size: 1.1rem;
}

.btn-close-white {
  background: none;
  border: none;
  color: rgba(255, 255, 255, 0.8);
  font-size: 28px;
  line-height: 1;
  transition: color 0.2s;
}

.btn-close-white:hover {
  color: white;
}

/* Tables */
.premium-table {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
  border: 1px solid #e2e8f0;
}

.premium-table thead th {
  background: #f8fafc !important;
  color: #475569 !important;
  font-weight: 700;
  text-transform: uppercase;
  font-size: 0.75rem;
  letter-spacing: 0.05em;
  padding: 12px 16px;
  border-bottom: 2px solid #e2e8f0;
}

.premium-table tbody td {
  padding: 12px 16px;
  vertical-align: middle;
  border-bottom: 1px solid #f1f5f9;
  color: #1e293b;
  font-size: 0.9rem;
}

.premium-table tbody tr:hover {
  background-color: #f8fafc;
}

/* Barcode Labels (Tem) */
.barcode-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(180px, 1fr));
  gap: 20px;
  padding: 20px;
}

.barcode-item {
  background: #fff;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  padding: 12px;
  text-align: center;
  transition: transform 0.2s, box-shadow 0.2s;
  cursor: default;
}

.barcode-item:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
}

.tem-ten-thuoc {
  font-size: 0.7rem;
  font-weight: 800;
  color: #0f172a;
  margin-bottom: 8px;
  line-height: 1.2;
  text-transform: uppercase;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.tem-image {
  width: 100%;
  height: 45px;
  object-fit: contain;
  margin: 4px 0;
}

.tem-ma-so {
  font-family: 'Courier New', Courier, monospace;
  font-size: 0.75rem;
  color: #64748b;
  margin-top: 4px;
}

/* Badges */
.badge {
  padding: 6px 10px;
  border-radius: 8px;
  font-weight: 700;
  font-size: 0.65rem;
  text-transform: uppercase;
  letter-spacing: 0.025em;
}

.badge-info { background: #e0f2fe; color: #0369a1; }
.badge-success { background: #dcfce7; color: #15803d; }
.badge-warning { background: #fef3c7; color: #92400e; }
.badge-secondary { background: #f1f5f9; color: #475569; }

/* Buttons & Custom UI */
.btn {
  border-radius: 12px;
  font-weight: 700;
  padding: 10px 24px;
  transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
  text-transform: none;
}

.shadow-sm { box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.05); }

.btn-success { background-color: #10b981; border-color: #10b981; }
.btn-success:hover { background-color: #059669; transform: translateY(-1px); box-shadow: 0 10px 15px -3px rgba(16, 185, 129, 0.4); }

.btn-warning { background-color: #f59e0b; border-color: #f59e0b; color: white !important; }
.btn-warning:hover { background-color: #d97706; transform: translateY(-1px); box-shadow: 0 10px 15px -3px rgba(245, 158, 11, 0.4); }

/* Autocomplete */
.pos-autocomplete-list {
  position: absolute;
  z-index: 1000;
  background: white;
  border-radius: 16px;
  width: 100%;
  max-height: 250px;
  overflow-y: auto;
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
  border: 1px solid #e2e8f0;
  margin-top: 4px;
}

.pos-autocomplete-item {
  padding: 12px 16px;
  cursor: pointer;
  transition: background 0.2s;
  color: #334155;
  font-weight: 500;
}

.pos-autocomplete-item:hover {
  background: #f1f5f9;
  color: #0f172a;
}

/* Animations */
@keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }
@keyframes slideUp { from { transform: translateY(24px); opacity: 0; } to { transform: translateY(0); opacity: 1; } }

/* Toggle Switch */
.custom-switch-track {
  width: 48px; height: 26px;
  background: #cbd5e1;
  border-radius: 13px;
  position: relative;
  transition: all 0.3s;
}

.custom-switch-track.active {
  background: #10b981;
}

.custom-switch-thumb {
  position: absolute;
  top: 3px; left: 3px;
  width: 20px; height: 20px;
  background: white;
  border-radius: 50%;
  transition: all 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}

.active .custom-switch-thumb {
  left: 25px;
}
</style>
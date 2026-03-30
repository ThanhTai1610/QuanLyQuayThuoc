<template>
  <div class="site-wrap">

    <!-- Breadcrumb -->
    <div class="bg-light py-3">
      <div class="container">
        <div class="row">
          <div class="col-md-12 mb-0">
            <router-link to="/">Trang chủ</router-link>
            <span class="mx-2 mb-0">/</span>
            <strong class="text-black">Đặt hàng</strong>
          </div>
        </div>
      </div>
    </div>

    <div class="checkout-page-wrapper">
      <div class="container">

        <!-- Bước -->
        <div class="checkout-steps">
          <span class="step">
            <span class="bullet">1</span> Giỏ hàng
          </span>
          <span class="mx-1 text-gray-400">›</span>
          <span class="step active">
            <span class="bullet">2</span> Đặt hàng
          </span>
          <span class="mx-1 text-gray-400">›</span>
          <span class="step">
            <span class="bullet">3</span> Hoàn tất
          </span>
        </div>

        <!-- Loading -->
        <div v-if="dangTai" class="text-center py-5">
          <div class="spinner-border text-primary" role="status">
            <span class="sr-only">Đang tải...</span>
          </div>
        </div>

        <div v-else class="checkout-layout">

          <!-- Phần trái -->
          <div class="checkout-main">

            <!-- Thông tin giao hàng -->
            <div class="checkout-card mb-3">
              <div class="checkout-card-title">Thông tin giao hàng</div>
              <div class="checkout-note">
                Vui lòng điền đầy đủ thông tin bên dưới để Pharmative giao hàng nhanh và chính xác.
              </div>

              <!-- Chọn địa chỉ từ SoDiaChi -->
              <div class="form-group" v-if="danhSachDiaChi.length > 0">
                <label>Dùng địa chỉ đã lưu</label>
                <select class="form-control" v-model="diaChiChon" @change="dieuChinhDiaChi">
                  <option value="">— Nhập địa chỉ mới —</option>
                  <option v-for="dc in danhSachDiaChi" :key="dc.maDiaChi" :value="dc.maDiaChi">
                    {{ dc.hoTenNguoiNhan }} • {{ dc.soDienThoaiNhan }} • {{ dc.diaChiChiTiet }}, {{ dc.phuongXa }}, {{ dc.quanHuyen }}, {{ dc.tinhThanh }}
                  </option>
                </select>
              </div>

              <div class="form-row">
                <div class="form-group col-md-6">
                  <label>Họ và tên <span class="text-danger">*</span></label>
                  <input type="text" class="form-control" v-model="form.hoTenNguoiNhan"
                    placeholder="Nhập họ và tên" />
                </div>
                <div class="form-group col-md-6">
                  <label>Số điện thoại <span class="text-danger">*</span></label>
                  <input type="text" class="form-control" v-model="form.soDienThoaiNhan"
                    placeholder="Nhập số điện thoại" />
                </div>
              </div>

              <div class="form-group">
                <label>Địa chỉ nhận hàng <span class="text-danger">*</span></label>
                <input type="text" class="form-control" v-model="form.diaChiChiTiet"
                  placeholder="Số nhà, tên đường, phường/xã" />
              </div>

              <div class="form-row">
                <div class="form-group col-md-4">
                  <label>Phường/Xã</label>
                  <input type="text" class="form-control" v-model="form.phuongXa" placeholder="VD: Phường 1" />
                </div>
                <div class="form-group col-md-4">
                  <label>Quận/Huyện</label>
                  <input type="text" class="form-control" v-model="form.quanHuyen" placeholder="VD: Quận 1" />
                </div>
                <div class="form-group col-md-4">
                  <label>Tỉnh/Thành phố</label>
                  <input type="text" class="form-control" v-model="form.tinhThanh"
                    placeholder="VD: TP. Hồ Chí Minh" />
                </div>
              </div>

              <div class="form-group">
                <label>Ghi chú cho đơn hàng (không bắt buộc)</label>
                <textarea class="form-control" v-model="form.ghiChu" rows="3"
                  placeholder="Ví dụ: Giao giờ hành chính, gọi trước khi giao..."></textarea>
              </div>

              <!-- Đơn thuốc (AnhDonThuoc) -->
              <div class="form-group">
                <label>Đơn thuốc (nếu có)</label>
                <input type="file" class="form-control-file" accept="image/*" @change="chonAnhDonThuoc" />
                <small class="text-muted">Upload ảnh đơn thuốc nếu đơn hàng có thuốc kê đơn.</small>
                <img v-if="anhDonThuocPreview" :src="anhDonThuocPreview"
                  class="mt-2 img-thumbnail" style="max-height: 120px;" alt="Đơn thuốc" />
              </div>
            </div>

            <!-- Phương thức thanh toán — PhuongThucThanhToan trong DonHang -->
            <div class="checkout-card">
              <div class="checkout-card-title">Phương thức thanh toán</div>

              <div v-for="pt in phuongThucThanhToan" :key="pt.value"
                class="payment-method" :class="{ active: form.phuongThucThanhToan === pt.value }"
                @click="form.phuongThucThanhToan = pt.value" style="cursor:pointer;">
                <div class="payment-title">{{ pt.label }}</div>
                <div class="payment-desc">{{ pt.moTa }}</div>
              </div>

              <p v-if="loi" class="text-danger small mt-2">{{ loi }}</p>

              <div class="place-order-note">
                Bằng cách đặt hàng, bạn đồng ý với điều khoản sử dụng và chính sách bảo mật của Pharmative.
              </div>

              <button class="btn btn-primary btn-block mt-3"
                :disabled="dangDat" @click="xacNhanDatHang">
                <span v-if="dangDat">Đang xử lý...</span>
                <span v-else>Xác nhận đặt hàng</span>
              </button>
            </div>

          </div>

          <!-- Tóm tắt đơn hàng -->
          <div class="checkout-summary">
            <div class="order-card">
              <div class="checkout-card-title">Đơn hàng của bạn</div>

              <div class="order-items">
                <div class="order-item-row" v-for="item in gioHang" :key="item.maGioHang">
                  <div class="order-item-name">{{ item.tenThuoc }}</div>
                  <div class="order-item-qty">x{{ item.soLuong }} {{ item.tenDonVi }}</div>
                  <div class="order-item-price">{{ formatGia(item.giaBan * item.soLuong) }}</div>
                </div>
              </div>

              <div class="order-total-block">
                <div class="order-row">
                  <span>Tạm tính</span>
                  <span>{{ formatGia(tamTinh) }}</span>
                </div>
                <div class="order-row">
                  <span>Phí vận chuyển</span>
                  <span>Miễn phí</span>
                </div>
                <div class="order-row total">
                  <span>Thành tiền</span>
                  <span>{{ formatGia(tamTinh) }}</span>
                </div>
              </div>

              <div class="checkout-note mt-2">
                Quý khách vui lòng kiểm tra kỹ đơn hàng, đặc biệt là số lượng và thông tin liên hệ trước khi xác nhận.
              </div>
            </div>
          </div>

        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import '../../assets/css/checkout-page.css';
import { ref, reactive, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import axiosClient from '../../api/axiosClient';

const router = useRouter();

const gioHang         = ref([]);   // Lấy từ GioHang JOIN Thuoc, DonViTinh
const danhSachDiaChi  = ref([]);   // Lấy từ SoDiaChi của khách
const diaChiChon      = ref('');
const dangTai         = ref(false);
const dangDat         = ref(false);
const loi             = ref('');
const anhDonThuocFile    = ref(null);
const anhDonThuocPreview = ref('');

// Ánh xạ với cột PhuongThucThanhToan trong bảng DonHang
const phuongThucThanhToan = [
  { value: 'COD',          label: 'Thanh toán khi nhận hàng (COD)',   moTa: 'Thanh toán tiền mặt cho nhân viên giao hàng khi nhận được sản phẩm.' },
  { value: 'ChuyenKhoan',  label: 'Chuyển khoản ngân hàng',            moTa: 'Chuyển khoản theo thông tin hiển thị sau khi xác nhận đặt hàng.' },
  { value: 'ViDienTu',     label: 'Ví điện tử / Thẻ',                  moTa: 'Liên kết với các ví Momo, ZaloPay, thẻ ATM.' },
];

// Ánh xạ với các cột trong DonHang + SoDiaChi
const form = reactive({
  hoTenNguoiNhan:    '',
  soDienThoaiNhan:   '',
  diaChiChiTiet:     '',
  phuongXa:          '',
  quanHuyen:         '',
  tinhThanh:         '',
  ghiChu:            '',   // GhiChu — lưu vào DonHang
  phuongThucThanhToan: 'COD',
});

const tamTinh = computed(() =>
  gioHang.value.reduce((sum, item) => sum + item.giaBan * item.soLuong, 0)
);

const loadData = async () => {
  dangTai.value = true;
  try {
    const [resGio, resDiaChi] = await Promise.all([
      axiosClient.get('/GioHang'),
      axiosClient.get('/SoDiaChi'),
    ]);
    gioHang.value        = resGio.data;
    danhSachDiaChi.value = resDiaChi.data;

    // Tự điền địa chỉ mặc định (LaMacDinh = 1)
    const macDinh = resDiaChi.data.find(dc => dc.laMacDinh);
    if (macDinh) {
      diaChiChon.value = macDinh.maDiaChi;
      dieuChinhDiaChi();
    }
  } catch (err) {
    console.error('Lỗi tải dữ liệu:', err);
  } finally {
    dangTai.value = false;
  }
};

// Điền form từ địa chỉ đã chọn trong SoDiaChi
const dieuChinhDiaChi = () => {
  if (!diaChiChon.value) {
    Object.assign(form, { hoTenNguoiNhan: '', soDienThoaiNhan: '', diaChiChiTiet: '', phuongXa: '', quanHuyen: '', tinhThanh: '' });
    return;
  }
  const dc = danhSachDiaChi.value.find(d => d.maDiaChi === diaChiChon.value);
  if (dc) {
    form.hoTenNguoiNhan  = dc.hoTenNguoiNhan;
    form.soDienThoaiNhan = dc.soDienThoaiNhan;
    form.diaChiChiTiet   = dc.diaChiChiTiet;
    form.phuongXa        = dc.phuongXa;
    form.quanHuyen       = dc.quanHuyen;
    form.tinhThanh       = dc.tinhThanh;
  }
};

const chonAnhDonThuoc = (e) => {
  const file = e.target.files[0];
  if (!file) return;
  anhDonThuocFile.value    = file;
  anhDonThuocPreview.value = URL.createObjectURL(file);
};

// Tạo DonHang + ChiTietDonHang
// POST /DonHang — body: form + danh sách sản phẩm từ GioHang
const xacNhanDatHang = async () => {
  loi.value = '';

  if (!form.hoTenNguoiNhan.trim() || !form.soDienThoaiNhan.trim() || !form.diaChiChiTiet.trim()) {
    loi.value = 'Vui lòng điền đầy đủ thông tin giao hàng.';
    return;
  }

  dangDat.value = true;
  try {
    const diaChiGiaoHang = [form.diaChiChiTiet, form.phuongXa, form.quanHuyen, form.tinhThanh]
      .filter(Boolean).join(', ');

    // Upload ảnh đơn thuốc nếu có
    let anhDonThuocUrl = '';
    if (anhDonThuocFile.value) {
      const fd = new FormData();
      fd.append('file', anhDonThuocFile.value);
      const resAnh = await axiosClient.post('/DonHang/upload-don-thuoc', fd);
      anhDonThuocUrl = resAnh.data.url;
    }

    // Body gửi lên — ánh xạ với bảng DonHang + ChiTietDonHang
    const body = {
      diaChiGiaoHang,
      soDienThoaiNhan:     form.soDienThoaiNhan,
      phuongThucThanhToan: form.phuongThucThanhToan,
      ghiChu:              form.ghiChu,
      anhDonThuoc:         anhDonThuocUrl,
      // ChiTietDonHang: server tự lấy từ GioHang của khách
    };

    const res = await axiosClient.post('/DonHang', body);
    router.push({ name: 'HoanTatDatHang', params: { id: res.data.maDonHang } });
  } catch (err) {
    loi.value = err.response?.data?.message || 'Có lỗi xảy ra. Vui lòng thử lại.';
  } finally {
    dangDat.value = false;
  }
};

const formatGia = (value) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value ?? 0);

onMounted(loadData);
</script>
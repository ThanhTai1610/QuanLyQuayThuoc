<template>
  <div class="site-wrap">
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
        <div class="checkout-steps">
          <span class="step"><span class="bullet">1</span> Giỏ hàng</span>
          <span class="mx-1 text-gray-400">›</span>
          <span class="step active"><span class="bullet">2</span> Đặt hàng</span>
        </div>

        <div v-if="dangTai" class="text-center py-5">
          <div class="spinner-border text-primary" role="status"></div>
          <p class="mt-2">Đang tải dữ liệu...</p>
        </div>

        <div v-else class="checkout-layout">
          <div class="checkout-main">

            <!-- ── PHẦN MỚI: Chọn địa chỉ từ sổ địa chỉ ── -->
            <div class="checkout-card mb-3" v-if="danhSachDiaChi.length > 0">
              <div class="checkout-card-title">Địa chỉ giao hàng</div>

              <!-- Từng địa chỉ có sẵn -->
              <div v-for="dc in danhSachDiaChi" :key="dc.maDiaChi" @click="chonDiaChi(dc)" style="
                  border: 1px solid #dee2e6;
                  border-radius: 8px;
                  padding: 12px 16px;
                  margin-bottom: 10px;
                  cursor: pointer;
                  transition: border-color 0.2s, background 0.2s;
                " :style="diaChiDangChon === dc.maDiaChi
                  ? 'border-color: #007bff; background: #f0f7ff;'
                  : 'background: #fff;'">
                <div class="d-flex align-items-start">
                  <!-- Radio tự vẽ -->
                  <div style="margin-right: 12px; margin-top: 3px; flex-shrink: 0;">
                    <div
                      style="width:18px;height:18px;border-radius:50%;border:2px solid;display:flex;align-items:center;justify-content:center;"
                      :style="diaChiDangChon === dc.maDiaChi ? 'border-color:#007bff;' : 'border-color:#adb5bd;'">
                      <div v-if="diaChiDangChon === dc.maDiaChi"
                        style="width:10px;height:10px;border-radius:50%;background:#007bff;">
                      </div>
                    </div>
                  </div>
                  <!-- Nội dung -->
                  <div>
                    <div class="font-weight-bold">
                      {{ dc.hoTenNguoiNhan }}
                      <span class="text-muted font-weight-normal mx-1">|</span>
                      {{ dc.soDienThoaiNhan }}
                      <span v-if="dc.laMacDinh" class="badge badge-success ml-1" style="font-size:0.7rem;">Mặc
                        định</span>
                    </div>
                    <div class="text-muted small mt-1">{{ diaChiDayDu(dc) }}</div>
                    <span v-if="dc.loaiDiaChi" class="badge badge-light border mt-1" style="font-size:0.7rem;">
                      {{ dc.loaiDiaChi }}
                    </span>
                  </div>
                </div>
              </div>

              <!-- Tùy chọn nhập địa chỉ mới -->
              <div @click="chonNhapMoi" style="
                  border: 1px solid #dee2e6;
                  border-radius: 8px;
                  padding: 12px 16px;
                  cursor: pointer;
                  transition: border-color 0.2s, background 0.2s;
                " :style="diaChiDangChon === 'new'
                  ? 'border-color: #007bff; background: #f0f7ff;'
                  : 'background: #fff;'">
                <div class="d-flex align-items-center">
                  <div style="margin-right: 12px; flex-shrink: 0;">
                    <div
                      style="width:18px;height:18px;border-radius:50%;border:2px solid;display:flex;align-items:center;justify-content:center;"
                      :style="diaChiDangChon === 'new' ? 'border-color:#007bff;' : 'border-color:#adb5bd;'">
                      <div v-if="diaChiDangChon === 'new'"
                        style="width:10px;height:10px;border-radius:50%;background:#007bff;">
                      </div>
                    </div>
                  </div>
                  <span class="text-primary font-weight-bold">+ Nhập địa chỉ mới</span>
                </div>
              </div>
            </div>
            <!-- ── HẾT PHẦN CHỌN ĐỊA CHỈ ── -->

            <!-- Form thông tin giao hàng:
                 - Không có sổ địa chỉ: luôn hiện
                 - Có sổ địa chỉ: hiện khi đã chọn một địa chỉ hoặc chọn nhập mới -->
            <div class="checkout-card mb-3" v-if="danhSachDiaChi.length === 0 || diaChiDangChon !== null">
              <div class="checkout-card-title">Thông tin giao hàng</div>
              <div class="form-row">
                <div class="form-group col-md-6">
                  <label>Họ và tên <span class="text-danger">*</span></label>
                  <input type="text" class="form-control" v-model="form.hoTenNguoiNhan" placeholder="Nhập họ tên" />
                </div>
                <div class="form-group col-md-6">
                  <label>Số điện thoại <span class="text-danger">*</span></label>
                  <input type="text" class="form-control" v-model="form.soDienThoaiNhan"
                    placeholder="Nhập số điện thoại" />
                </div>
              </div>
              <div class="form-group">
                <label>Địa chỉ nhận hàng <span class="text-danger">*</span></label>
                <input type="text" class="form-control" v-model="form.diaChiChiTiet" placeholder="Số nhà, tên đường" />
              </div>
              <div class="form-row">
                <div class="form-group col-md-4">
                  <label>Phường/Xã</label>
                  <input type="text" class="form-control" v-model="form.phuongXa" />
                </div>
                <div class="form-group col-md-4">
                  <label>Quận/Huyện</label>
                  <input type="text" class="form-control" v-model="form.quanHuyen" />
                </div>
                <div class="form-group col-md-4">
                  <label>Tỉnh/Thành phố</label>
                  <input type="text" class="form-control" v-model="form.tinhThanh" />
                </div>
              </div>
              <div class="form-group">
                <label>Ghi chú</label>
                <textarea class="form-control" v-model="form.ghiChu" rows="2"></textarea>
              </div>
            </div>

            <div class="checkout-card">
              <div class="checkout-card-title">Phương thức thanh toán</div>
              <div v-for="pt in phuongThucThanhToan" :key="pt.value" class="payment-method"
                :class="{ active: form.phuongThucThanhToan === pt.value }" @click="form.phuongThucThanhToan = pt.value">
                <div class="payment-title">{{ pt.label }}</div>
                <div class="payment-desc text-muted small">{{ pt.moTa }}</div>
              </div>

              <p v-if="loi" class="text-danger small mt-2">{{ loi }}</p>

              <button class="btn btn-primary btn-block mt-3"
                :disabled="dangDat || gioHang.length === 0 || (danhSachDiaChi.length > 0 && diaChiDangChon === null)"
                @click="xacNhanDatHang">
                <span v-if="dangDat" class="spinner-border spinner-border-sm mr-2"></span>
                {{ dangDat ? 'Đang xử lý...' : 'Xác nhận đặt hàng' }}
              </button>

              <!-- Nhắc chọn địa chỉ nếu chưa chọn -->
              <p v-if="danhSachDiaChi.length > 0 && diaChiDangChon === null" class="text-warning small mt-2 mb-0">
                Vui lòng chọn địa chỉ giao hàng ở trên trước khi đặt hàng.
              </p>
            </div>
          </div>

          <div class="checkout-summary">
            <div class="order-card">
              <div class="checkout-card-title">Đơn hàng của bạn</div>
              <div class="order-items">
                <div class="order-item-row" v-for="item in gioHang" :key="item.maGioHang">
                  <div class="order-item-name">{{ item.tenThuoc }}</div>
                  <div class="order-item-qty text-muted">x{{ item.soLuong }} {{ item.tenDonVi }}</div>
                  <div class="order-item-price font-weight-bold">{{ formatGia(item.giaBan * item.soLuong) }}</div>
                </div>
              </div>
              <div class="order-total-block mt-3 pt-3 border-top">
                <div class="order-row d-flex justify-content-between">
                  <span>Tạm tính</span>
                  <span>{{ formatGia(tamTinh) }}</span>
                </div>
                <div class="order-row d-flex justify-content-between total font-weight-bold text-primary h5 mt-2">
                  <span>Thành tiền</span>
                  <span>{{ formatGia(tamTinh) }}</span>
                </div>
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
import { useRouter, useRoute } from 'vue-router';
import { useMomo } from '../../services/useMomo';
import axiosClient from '../../api/axiosClient';
import Swal from 'sweetalert2';

const router = useRouter();
const route = useRoute();
const { createPayment } = useMomo();

const gioHang = ref([]);
const danhSachDiaChi = ref([]);   // Danh sách từ SoDiaChi API
const diaChiDangChon = ref(null); // null = chưa chọn | 'new' = nhập mới | số = maDiaChi
const dangTai = ref(false);
const dangDat = ref(false);
const loi = ref('');

const phuongThucThanhToan = [
  { value: 'COD', label: 'Thanh toán khi nhận hàng (COD)', moTa: 'Thanh toán tiền mặt khi nhận hàng.' },
  { value: 'Momo', label: 'Thanh toán qua ví MoMo', moTa: 'Thanh toán nhanh chóng qua ứng dụng MoMo.' },
];

const form = reactive({
  hoTenNguoiNhan: '',
  soDienThoaiNhan: '',
  diaChiChiTiet: '',
  phuongXa: '',
  quanHuyen: '',
  tinhThanh: '',
  ghiChu: '',
  phuongThucThanhToan: 'COD',
});

const tamTinh = computed(() =>
  Math.round(gioHang.value.reduce((sum, item) => sum + (item.giaBan * item.soLuong), 0))
);

// ── Ghép địa chỉ đầy đủ để hiển thị ──
const diaChiDayDu = (dc) =>
  [dc.diaChiChiTiet, dc.phuongXa, dc.quanHuyen, dc.tinhThanh].filter(Boolean).join(', ');

// ── Chọn địa chỉ từ sổ → tự điền form (vẫn cho chỉnh sửa) ──
const chonDiaChi = (dc) => {
  diaChiDangChon.value = dc.maDiaChi;
  form.hoTenNguoiNhan = dc.hoTenNguoiNhan || '';
  form.soDienThoaiNhan = dc.soDienThoaiNhan || '';
  form.diaChiChiTiet = dc.diaChiChiTiet || '';
  form.phuongXa = dc.phuongXa || '';
  form.quanHuyen = dc.quanHuyen || '';
  form.tinhThanh = dc.tinhThanh || '';
};

// ── Chọn "Nhập địa chỉ mới" → xóa trắng form ──
const chonNhapMoi = () => {
  diaChiDangChon.value = 'new';
  form.hoTenNguoiNhan = '';
  form.soDienThoaiNhan = '';
  form.diaChiChiTiet = '';
  form.phuongXa = '';
  form.quanHuyen = '';
  form.tinhThanh = '';
};

// ── Tải giỏ hàng + sổ địa chỉ song song ──
const loadData = async () => {
  dangTai.value = true;
  try {
    const [resGio, resDC] = await Promise.all([
      axiosClient.get('/GioHang'),
      axiosClient.get('/SoDiaChi').catch(() => []), // Không crash nếu API lỗi
    ]);

    gioHang.value = resGio || [];
    danhSachDiaChi.value = resDC || [];

    if (gioHang.value.length === 0) {
      router.push('/gio-hang');
      return;
    }

    // Tự động chọn địa chỉ mặc định nếu có
    if (danhSachDiaChi.value.length > 0) {
      const macDinh = danhSachDiaChi.value.find(dc => dc.laMacDinh);
      if (macDinh) {
        chonDiaChi(macDinh);
      }
      // Nếu không có mặc định thì diaChiDangChon = null, người dùng tự chọn
    }
  } catch (err) {
    console.error('Lỗi tải dữ liệu:', err);
    router.push('/gio-hang');
  } finally {
    dangTai.value = false;
  }
};

// ── Xử lý khi bấm "Xác nhận đặt hàng" ──
const xacNhanDatHang = async () => {
  loi.value = '';

  // Bắt buộc chọn địa chỉ nếu có sổ
  if (danhSachDiaChi.value.length > 0 && diaChiDangChon.value === null) {
    loi.value = 'Vui lòng chọn địa chỉ giao hàng.';
    return;
  }

  if (!form.hoTenNguoiNhan.trim() || !form.soDienThoaiNhan.trim() || !form.diaChiChiTiet.trim()) {
    loi.value = 'Vui lòng điền đủ Họ tên, SĐT và Địa chỉ.';
    return;
  }

  const sdtRegex = /^(09|03|07|08|05)[0-9]{8}$/;
  if (!sdtRegex.test(form.soDienThoaiNhan)) {
    loi.value = 'Số điện thoại không hợp lệ.';
    return;
  }

  const coHetHang = gioHang.value.some(item => !item.maLo || item.maLo === 0);
  if (coHetHang) {
    loi.value = 'Sản phẩm hết hàng. Vui lòng kiểm tra lại giỏ hàng.';
    return;
  }

  dangDat.value = true;
  try {
    const fullAddress = [form.diaChiChiTiet, form.phuongXa, form.quanHuyen, form.tinhThanh]
      .filter(str => str && str.trim() !== '')
      .join(', ');

    const body = {
      PhuongThucThanhToan: form.phuongThucThanhToan,
      DiaChiGiaoHang: fullAddress,
      SoDienThoaiNhan: form.soDienThoaiNhan,
      HoTenNguoiNhan: form.hoTenNguoiNhan,
      GhiChu: form.ghiChu,
      GiamGia: 0,
      ChiTiet: gioHang.value.map(item => ({
        MaLo: item.maLo,
        MaDVT: item.maDVT,
        SoLuong: item.soLuong,
        GiaBan: item.giaBan
      }))
    };

    const res = await axiosClient.post('/GioHang/dat-hang', body);

    const isSuccess = res?.success === true || res?.Success === true;
    const maDH = res?.maDonHang ?? res?.MaDonHang;

    if (isSuccess && maDH != null) {
      if (form.phuongThucThanhToan === 'Momo') {
        const uniqueOrderId = `${maDH}_${Date.now()}`;

        await createPayment(
          tamTinh.value,
          `Thanh toán Pharmative #${maDH}`,
          uniqueOrderId,
          'KhachHang'
        );
      } else {
        await Swal.fire({
          icon: 'success',
          title: 'Đặt hàng thành công!',
          text: `Mã đơn hàng: #${maDH}`,
          confirmButtonText: 'Xem lịch sử'
        });
        router.push({ name: 'LichSuDonHang' });
      }
    } else {
      const errorMsg = res?.message || res?.Message || 'Đặt hàng thất bại. Vui lòng thử lại.';
      Swal.fire('Lỗi', errorMsg, 'error');
    }
  } catch (err) {
    Swal.fire('Lỗi', err.response?.data?.message || 'Đã có lỗi xảy ra.', 'error');
  } finally {
    dangDat.value = false;
  }
};

const formatGia = (v) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v || 0);

// ── Xử lý kết quả MoMo quay về ──
onMounted(async () => {
  const { orderId, status } = route.query;

  if (orderId && status) {
    if (status === 'success') {

      const displayOrderId = orderId.includes('_') ? orderId.split('_')[0] : orderId;
      await Swal.fire({
        icon: 'success',
        title: 'Thanh toán thành công!',
        text: `Đơn hàng #${displayOrderId} của bạn đã được thanh toán qua ví MoMo.`,
        confirmButtonText: 'Xem đơn hàng',
        allowOutsideClick: false
      });
      router.replace({ query: {} });
      router.push({ name: 'LichSuDonHang' });
    } else {
      await Swal.fire({
        icon: 'error',
        title: 'Thanh toán thất bại',
        text: 'Bạn đã hủy thanh toán hoặc giao dịch không thành công.',
      });
      router.replace({ query: {} });
      loadData();
    }
  } else {
    loadData();
  }
});
</script>
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
          <span class="mx-1 text-gray-400">›</span>
          <span class="step"><span class="bullet">3</span> Hoàn tất</span>
        </div>

        <div v-if="dangTai" class="text-center py-5">
          <div class="spinner-border text-primary" role="status"></div>
        </div>

        <div v-else class="checkout-layout">
          <div class="checkout-main">
            <div class="checkout-card mb-3">
              <div class="checkout-card-title">Thông tin giao hàng</div>

              <div class="form-row">
                <div class="form-group col-md-6">
                  <label>Họ và tên <span class="text-danger">*</span></label>
                  <input type="text" class="form-control" v-model="form.hoTenNguoiNhan" placeholder="Nhập họ tên" />
                </div>
                <div class="form-group col-md-6">
                  <label>Số điện thoại <span class="text-danger">*</span></label>
                  <input type="text" class="form-control" v-model="form.soDienThoaiNhan" placeholder="Nhập số điện thoại" />
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
              <div
                v-for="pt in phuongThucThanhToan"
                :key="pt.value"
                class="payment-method"
                :class="{ active: form.phuongThucThanhToan === pt.value }"
                @click="form.phuongThucThanhToan = pt.value"
              >
                <div class="payment-title">{{ pt.label }}</div>
                <div class="payment-desc text-muted small">{{ pt.moTa }}</div>
              </div>

              <p v-if="loi" class="text-danger small mt-2">{{ loi }}</p>

              <button
                class="btn btn-primary btn-block mt-3"
                :disabled="dangDat || gioHang.length === 0"
                @click="xacNhanDatHang"
              >
                <span v-if="dangDat" class="spinner-border spinner-border-sm mr-2"></span>
                {{ dangDat ? 'Đang xử lý...' : 'Xác nhận đặt hàng' }}
              </button>
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
import { useRouter } from 'vue-router';
import { useMomo } from '../../services/useMomo';
import axiosClient from '../../api/axiosClient';
import Swal from 'sweetalert2';

const router = useRouter();
const { createPayment } = useMomo();

const gioHang = ref([]);
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

// ── Load giỏ hàng ──
// axiosClient đã tự unwrap response.data nên dùng res trực tiếp
const loadData = async () => {
  dangTai.value = true;
  try {
    const res = await axiosClient.get('/GioHang');

    // ✅ Sửa: bỏ res.data vì axiosClient đã unwrap sẵn
    gioHang.value = res || [];

    if (gioHang.value.length === 0) {
      router.push('/gio-hang');
    }
  } catch (err) {
    console.error('Lỗi tải giỏ hàng:', err);
    router.push('/gio-hang');
  } finally {
    dangTai.value = false;
  }
};

// ── Xác nhận đặt hàng ──
const xacNhanDatHang = async () => {
  loi.value = '';

  // 1. Validate thông tin giao hàng
  if (!form.hoTenNguoiNhan.trim() || !form.soDienThoaiNhan.trim() || !form.diaChiChiTiet.trim()) {
    loi.value = 'Vui lòng điền đủ Họ tên, SĐT và Địa chỉ.';
    return;
  }

  const sdtRegex = /^(09|03|07|08|05)[0-9]{8}$/;
  if (!sdtRegex.test(form.soDienThoaiNhan)) {
    loi.value = 'Số điện thoại không hợp lệ.';
    return;
  }

  // 2. Chặn nếu có sản phẩm hết hàng (maLo = 0)
  const coHetHang = gioHang.value.some(item => !item.maLo || item.maLo === 0);
  if (coHetHang) {
    loi.value = 'Một số sản phẩm trong giỏ đã hết hàng. Vui lòng quay lại giỏ hàng để xóa chúng.';
    return;
  }

  dangDat.value = true;

  try {
    // 3. Ghép địa chỉ đầy đủ
    const fullAddress = [form.diaChiChiTiet, form.phuongXa, form.quanHuyen, form.tinhThanh]
      .filter(str => str && str.trim() !== '')
      .join(', ');

    // 4. Chuẩn bị body — gửi lên endpoint mới /GioHang/dat-hang
    const body = {
      PhuongThucThanhToan: form.phuongThucThanhToan,
      DiaChiGiaoHang: fullAddress,
      SoDienThoaiNhan: form.soDienThoaiNhan,
      GhiChu: form.ghiChu,
      GiamGia: 0,
      ChiTiet: gioHang.value.map(item => ({
        MaLo: item.maLo,       // ✅ Không dùng || 0, đã validate ở trên
        MaDVT: item.maDVT,
        SoLuong: item.soLuong,
        GiaBan: item.giaBan
      }))
    };

    // 5. Gửi lên endpoint đúng — /GioHang/dat-hang thay vì /BanHang/thanh-toan
    const res = await axiosClient.post('/GioHang/dat-hang', body);

    // ✅ Sửa: axiosClient đã unwrap nên dùng res trực tiếp, không cần res.data
    const data = res;

    if (data.success) {
      const maDH = data.maDonHang;

      if (form.phuongThucThanhToan === 'Momo') {
        // Thanh toán MoMo
        await createPayment(
          tamTinh.value,
          `Thanh toán đơn hàng Pharmative - #${maDH}`,
          maDH.toString(),
          'KhachHang'
        );
      } else {
        // COD — thông báo thành công và chuyển trang
        await Swal.fire({
          icon: 'success',
          title: 'Đặt hàng thành công!',
          text: `Mã đơn hàng của bạn: #${maDH}`,
          confirmButtonText: 'Xem lịch sử đơn hàng'
        });
        router.push({ name: 'LichSuDonHang' });
      }
    } else {
      throw new Error(data.message || 'Không thể tạo đơn hàng.');
    }
  } catch (err) {
    console.error('Lỗi đặt hàng:', err);
    Swal.fire('Lỗi', err.response?.data?.message || err.message || 'Đã có lỗi xảy ra.', 'error');
  } finally {
    dangDat.value = false;
  }
};

const formatGia = (v) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v || 0);

onMounted(loadData);
</script>
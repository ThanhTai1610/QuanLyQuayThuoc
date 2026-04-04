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
              <div v-for="pt in phuongThucThanhToan" :key="pt.value" 
                   class="payment-method"
                   :class="{ active: form.phuongThucThanhToan === pt.value }" 
                   @click="form.phuongThucThanhToan = pt.value">
                <div class="payment-title">{{ pt.label }}</div>
                <div class="payment-desc text-muted small">{{ pt.moTa }}</div>
              </div>

              <p v-if="loi" class="text-danger small mt-2">{{ loi }}</p>

              <button class="btn btn-primary btn-block mt-3" :disabled="dangDat || gioHang.length === 0" @click="xacNhanDatHang">
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

const loadData = async () => {
  dangTai.value = true;
  try {
    const res = await axiosClient.get('/GioHang');
    gioHang.value = res.data || res || [];
    if (gioHang.value.length === 0) {
      router.push('/gio-hang');
    }
  } catch (err) {
    console.error('Lỗi tải giỏ hàng:', err);
  } finally {
    dangTai.value = false;
  }
};

const xacNhanDatHang = async () => {
  loi.value = '';
  
  // 1. Validate dữ liệu
  if (!form.hoTenNguoiNhan.trim() || !form.soDienThoaiNhan.trim() || !form.diaChiChiTiet.trim()) {
    loi.value = 'Vui lòng điền đủ Họ tên, SĐT và Địa chỉ.';
    return;
  }

  const sdtRegex = /((09|03|07|08|05)+([0-9]{8})\b)/g;
  if (!sdtRegex.test(form.soDienThoaiNhan)) {
    loi.value = 'Số điện thoại không hợp lệ.';
    return;
  }

  dangDat.value = true;

  try {
    // 2. Chuẩn bị Body gửi lên API
    const fullAddress = [form.diaChiChiTiet, form.phuongXa, form.quanHuyen, form.tinhThanh]
                        .filter(str => str && str.trim() !== '')
                        .join(', ');

    const body = {
      MaKhachHang: 1, // Thay bằng ID từ Store sau này
      PhuongThucThanhToan: form.phuongThucThanhToan,
      DiaChiGiaoHang: fullAddress,
      SoDienThoaiNhan: form.soDienThoaiNhan,
      GhiChu: form.ghiChu,
      GiamGia: 0,
      ChiTiet: gioHang.value.map(item => ({
        MaLo: item.maLo || 0,
        MaDVT: item.maDVT,
        SoLuong: item.soLuong,
        GiaBan: item.giaBan
      }))
    };

    // 3. Gửi đơn hàng lên Backend
    const res = await axiosClient.post('/BanHang/thanh-toan', body);
    const data = res.data || res;

    if (data.success) {
      const maDH = data.maDonHang;

      if (form.phuongThucThanhToan === 'Momo') {
  await createPayment(
    tamTinh.value,                             // amount
    `Thanh toán đơn hàng Pharmative - #${maDH}`, // orderInfo
    maDH.toString(),                           // orderId
    "KhachHang"                                // userType (khớp với Model mới)
  );
      } else {
        // Nếu chọn COD, báo thành công
        await Swal.fire({
          icon: 'success',
          title: 'Đặt hàng thành công!',
          text: `Mã đơn hàng: ${maDH}`,
          confirmButtonText: 'Xem lịch sử đơn hàng'
        });
        router.push({ name: 'LichSuDonHang' });
      }
    } else {
      throw new Error(data.message || 'Không thể tạo đơn hàng.');
    }
  } catch (err) {
    console.error('Lỗi đặt hàng:', err);
    Swal.fire("Lỗi", err.response?.data?.message || err.message, "error");
  } finally {
    dangDat.value = false;
  }
};

const formatGia = (v) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v || 0);

onMounted(loadData);
</script>
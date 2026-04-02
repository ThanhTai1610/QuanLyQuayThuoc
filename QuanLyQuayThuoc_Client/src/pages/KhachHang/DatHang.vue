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

        <div v-if="dangTai" class="text-center py-5">
          <div class="spinner-border text-primary" role="status">
            <span class="sr-only">Đang tải...</span>
          </div>
        </div>

        <div v-else class="checkout-layout">

          <div class="checkout-main">

            <div class="checkout-card mb-3">
              <div class="checkout-card-title">Thông tin giao hàng</div>
              <div class="checkout-note">
                Vui lòng điền đầy đủ thông tin bên dưới để Pharmative giao hàng nhanh và chính xác.
              </div>

              <div class="form-group" v-if="danhSachDiaChi.length > 0">
                <label>Dùng địa chỉ đã lưu</label>
                <select class="form-control" v-model="diaChiChon" @change="dieuChinhDiaChi">
                  <option value="">— Nhập địa chỉ mới —</option>
                  <option v-for="dc in danhSachDiaChi" :key="dc.maDiaChi" :value="dc.maDiaChi">
                    {{ dc.hoTenNguoiNhan }} • {{ dc.soDienThoaiNhan }} • {{ dc.diaChiChiTiet }}, {{ dc.phuongXa }}, {{
                    dc.quanHuyen }}, {{ dc.tinhThanh }}
                  </option>
                </select>
              </div>

              <div class="form-row">
                <div class="form-group col-md-6">
                  <label>Họ và tên <span class="text-danger">*</span></label>
                  <input type="text" class="form-control" v-model="form.hoTenNguoiNhan" placeholder="Nhập họ và tên" />
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
                  <input type="text" class="form-control" v-model="form.tinhThanh" placeholder="VD: TP. Hồ Chí Minh" />
                </div>
              </div>

              <div class="form-group">
                <label>Ghi chú cho đơn hàng (không bắt buộc)</label>
                <textarea class="form-control" v-model="form.ghiChu" rows="3"
                  placeholder="Ví dụ: Giao giờ hành chính, gọi trước khi giao..."></textarea>
              </div>

              <div class="form-group">
                <label>Đơn thuốc (nếu có)</label>
                <input type="file" class="form-control-file" accept="image/*" @change="chonAnhDonThuoc" />
                <small class="text-muted">Upload ảnh đơn thuốc nếu đơn hàng có thuốc kê đơn.</small>
                <img v-if="anhDonThuocPreview" :src="anhDonThuocPreview" class="mt-2 img-thumbnail"
                  style="max-height: 120px;" alt="Đơn thuốc" />
              </div>
            </div>

            <div class="checkout-card">
              <div class="checkout-card-title">Phương thức thanh toán</div>

              <div v-for="pt in phuongThucThanhToan" :key="pt.value" class="payment-method"
                :class="{ active: form.phuongThucThanhToan === pt.value }" @click="form.phuongThucThanhToan = pt.value"
                style="cursor:pointer;">
                <div class="payment-title">{{ pt.label }}</div>
                <div class="payment-desc">{{ pt.moTa }}</div>
              </div>

              <p v-if="loi" class="text-danger small mt-2">{{ loi }}</p>

              <div class="place-order-note">
                Bằng cách đặt hàng, bạn đồng ý với điều khoản sử dụng và chính sách bảo mật của Pharmative.
              </div>

              <button class="btn btn-primary btn-block mt-3" :disabled="dangDat" @click="xacNhanDatHang">
                <span v-if="dangDat">Đang xử lý...</span>
                <span v-else>Xác nhận đặt hàng</span>
              </button>
            </div>

          </div>

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
/* import '../../assets/css/checkout-page.css';
import { ref, reactive, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import axiosClient from '../../api/axiosClient';
import Swal from 'sweetalert2';

const router = useRouter();

const gioHang         = ref([]);   
const danhSachDiaChi  = ref([]);   
const diaChiChon      = ref('');
const dangTai         = ref(false);
const dangDat         = ref(false);
const loi             = ref('');
const anhDonThuocFile    = ref(null);
const anhDonThuocPreview = ref('');

const phuongThucThanhToan = [
  { value: 'COD',          label: 'Thanh toán khi nhận hàng (COD)',          moTa: 'Thanh toán tiền mặt cho nhân viên giao hàng khi nhận được sản phẩm.' },
  { value: 'ChuyenKhoan',  label: 'Chuyển khoản ngân hàng',                  moTa: 'Chuyển khoản theo thông tin hiển thị sau khi xác nhận đặt hàng.' },
  { value: 'ViDienTu',     label: 'Ví điện tử / Thẻ',                        moTa: 'Liên kết với các ví Momo, ZaloPay, thẻ ATM.' },
];

const form = reactive({
  hoTenNguoiNhan:    '',
  soDienThoaiNhan:   '',
  diaChiChiTiet:     '',
  phuongXa:          '',
  quanHuyen:         '',
  tinhThanh:         '',
  ghiChu:            '',   
  phuongThucThanhToan: 'COD',
});

const tamTinh = computed(() =>
  gioHang.value.reduce((sum, item) => sum + (item.giaBan || 0) * (item.soLuong || 0), 0)
);

const loadData = async () => {
  dangTai.value = true;
  try {
    const [resGio, resDiaChi] = await Promise.all([
      axiosClient.get('/GioHang'),
      axiosClient.get('/SoDiaChi'),
    ]);
    
    // axiosClient của bạn đã return response.data
    gioHang.value = resGio || [];
    danhSachDiaChi.value = resDiaChi || [];

    if (gioHang.value.length === 0) {
      Swal.fire('Thông báo', 'Giỏ hàng trống, vui lòng chọn sản phẩm.', 'info');
      router.push('/san-pham');
      return;
    }

    const macDinh = danhSachDiaChi.value.find(dc => dc.laMacDinh);
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

const dieuChinhDiaChi = () => {
  if (!diaChiChon.value) {
    form.hoTenNguoiNhan = '';
    form.soDienThoaiNhan = '';
    form.diaChiChiTiet = '';
    form.phuongXa = '';
    form.quanHuyen = '';
    form.tinhThanh = '';
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

const xacNhanDatHang = async () => {
  loi.value = '';

  if (!form.hoTenNguoiNhan?.trim() || !form.soDienThoaiNhan?.trim() || !form.diaChiChiTiet?.trim()) {
    loi.value = 'Vui lòng điền đầy đủ thông tin giao hàng.';
    return;
  }

  dangDat.value = true;
  try {
    const diaChiGiaoHang = [form.diaChiChiTiet, form.phuongXa, form.quanHuyen, form.tinhThanh]
      .filter(Boolean).join(', ');

    let anhDonThuocUrl = '';
    if (anhDonThuocFile.value) {
      const fd = new FormData();
      fd.append('file', anhDonThuocFile.value);
      const resAnh = await axiosClient.post('/DonHang/upload-don-thuoc', fd);
      anhDonThuocUrl = resAnh.url; // Giả sử API trả về { url: '...' }
    }

    // Body khớp với TaoDonHangDto.cs
    const body = {
      MaKhachHang: null, 
      PhuongThucThanhToan: form.phuongThucThanhToan,
      GiamGia: 0, 
      GhiChu: form.ghiChu,
      DiaChiGiaoHang: diaChiGiaoHang,
      SoDienThoaiNhan: form.soDienThoaiNhan,
      AnhDonThuoc: anhDonThuocUrl,
      ChiTiet: gioHang.value.map(item => ({
        MaLo: 0, 
        MaDVT: item.maDVT,
        SoLuong: item.soLuong,
        GiaBan: item.giaBan
      }))
    };

    const res = await axiosClient.post('/DonHang', body);
    
    await Swal.fire({
      icon: 'success',
      title: 'Đặt hàng thành công',
      text: 'Cảm ơn bạn đã tin tưởng Pharmative!',
      timer: 2000,
      showConfirmButton: false
    });

    router.push({ name: 'HoanTatDatHang', params: { id: res.maDonHang } });
  } catch (err) {
    loi.value = err.response?.data?.message || 'Có lỗi xảy ra khi đặt hàng.';
  } finally {
    dangDat.value = false;
  }
};

const formatGia = (value) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value ?? 0);

onMounted(loadData); */

import '../../assets/css/checkout-page.css';
import { ref, reactive, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useMomo } from '../../services/useMomo';
import axiosClient from '../../api/axiosClient';
import Swal from 'sweetalert2';

const router = useRouter();
const { createPayment } = useMomo();

// STATE
const gioHang = ref([]);
const danhSachDiaChi = ref([]); 
const dangTai = ref(false);
const dangDat = ref(false);
const loi = ref('');

// Cấu hình các phương thức
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

// COMPUTED: Tính tổng tiền (Đảm bảo trả về số nguyên)
const tamTinh = computed(() =>
  Math.round(gioHang.value.reduce((sum, item) => sum + (item.giaBan * item.soLuong), 0))
);

// Tải dữ liệu giỏ hàng khi vào trang
const loadData = async () => {
  dangTai.value = true;
  try {
    const resGio = await axiosClient.get('/GioHang');
    // Kiểm tra cấu trúc trả về của axiosClient
    const data = resGio.data || resGio;
    gioHang.value = Array.isArray(data) ? data : [];

    if (gioHang.value.length === 0) {
      Swal.fire('Giỏ hàng trống', 'Vui lòng chọn sản phẩm trước khi thanh toán', 'warning');
      router.push('/');
    }
  } catch (err) {
    console.error('Lỗi tải giỏ hàng:', err);
  } finally {
    dangTai.value = false;
  }
};

const xacNhanDatHang = async () => {
  loi.value = '';

  // 1. Validate đơn giản
  if (!form.hoTenNguoiNhan.trim() || !form.soDienThoaiNhan.trim() || !form.diaChiChiTiet.trim()) {
    loi.value = 'Vui lòng nhập đầy đủ thông tin giao hàng.';
    return;
  }

  dangDat.value = true;

  try {
    // 2. Chuẩn bị địa chỉ và dữ liệu gửi đi
    const fullAddress = `${form.diaChiChiTiet}, ${form.phuongXa}, ${form.quanHuyen}, ${form.tinhThanh}`;

    const body = {
      MaKhachHang: 1, // Tạm thời để 1, nếu có Login hãy lấy từ Store
      PhuongThucThanhToan: form.phuongThucThanhToan,
      GiamGia: 0,
      ChiTiet: gioHang.value.map(item => ({
        MaLo: item.maLo || 0,
        MaDVT: item.maDVT,
        SoLuong: item.soLuong,
        GiaBan: item.giaBan
      })),
      GhiChu: `Người nhận: ${form.hoTenNguoiNhan} - SĐT: ${form.soDienThoaiNhan}. Ghi chú: ${form.ghiChu}. ĐC: ${fullAddress}`
    };

    // 3. Gọi API lưu đơn hàng vào Database
    const res = await axiosClient.post('/BanHang/thanh-toan', body);
    
    // Kiểm tra kết quả trả về từ Backend
    const success = res.data?.success || res.success;
    const maDH = res.data?.maDonHang || res.maDonHang;

    if (success) {
      if (form.phuongThucThanhToan === 'Momo') {
        // TRƯỜNG HỢP: MOMO -> Chuyển hướng sang cổng thanh toán
        await createPayment(
          tamTinh.value,
          `Thanh toán đơn hàng Pharmative - ĐH: ${maDH}`, 
          "KhachHang"
        );
      } else {
        // TRƯỜNG HỢP: COD -> Báo thành công và về trang lịch sử
        await Swal.fire({
          title: 'Đặt hàng thành công!',
          text: `Mã đơn hàng của bạn là: ${maDH}`,
          icon: 'success',
          confirmButtonText: 'Xem đơn hàng',
          confirmButtonColor: '#28a745',
        });
        router.push({ name: 'LichSuDonHang' }); // Đảm bảo route này có tồn tại
      }
    } else {
      throw new Error(res.data?.message || 'Lưu đơn hàng thất bại');
    }

  } catch (err) {
    console.error('Lỗi đặt hàng:', err);
    const message = err.response?.data?.message || err.message || 'Lỗi khi kết nối đến máy chủ.';
    Swal.fire("Thất bại", message, "error");
  } finally {
    dangDat.value = false;
  }
};

const formatGia = (v) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v || 0);

onMounted(loadData);
</script>
<template>
  <div class="site-wrap">

    <!-- Breadcrumb -->
    <div class="bg-light py-3">
      <div class="container">
        <div class="row">
          <div class="col-md-12 mb-0">
            <router-link to="/">Trang chủ</router-link>
            <span class="mx-2 mb-0">/</span>
            <strong class="text-black">Giỏ hàng</strong>
          </div>
        </div>
      </div>
    </div>

    <div class="cart-page-wrapper">
      <div class="container">

        <!-- Bước -->
        <div class="cart-steps">
          <span class="step active">
            <span class="bullet">1</span> Giỏ hàng
          </span>
          <span class="mx-1 text-gray-400">›</span>
          <span class="step">
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

        <div v-else class="cart-layout">

          <!-- Phần chính -->
          <div class="cart-main">
            <div class="cart-card">
              <div class="cart-header-row">
                <div>
                  <div class="cart-title">Giỏ hàng của bạn</div>
                  <div class="cart-meta">{{ gioHang.length }} sản phẩm trong giỏ</div>
                </div>
                <router-link to="/san-pham" class="btn btn-outline-primary btn-sm">
                  Tiếp tục mua sắm
                </router-link>
              </div>

              <!-- Danh sách sản phẩm -->
              <div class="cart-items">
                <div v-if="gioHang.length === 0" class="text-center py-4 text-muted">
                  Giỏ hàng trống. <router-link to="/san-pham">Mua sắm ngay</router-link>
                </div>

                <div class="cart-item-row" v-for="item in gioHang" :key="item.maGioHang">
                  <div class="cart-thumb">
                    <img :src="getImageUrl(item.hinhAnhChinh)" :alt="item.tenThuoc" />
                  </div>
                  <div class="cart-info">
                    <div class="cart-product-name">{{ item.tenThuoc }}</div>
                    <div class="cart-product-meta">
                      {{ item.tenDonVi }}
                      <span v-if="item.moTaNgan"> • {{ item.moTaNgan }}</span>
                    </div>
                    <!-- Chọn đơn vị tính (DonViTinh) -->
                    <div class="mt-1" v-if="item.danhSachDVT && item.danhSachDVT.length > 1">
                      <select class="form-control form-control-sm w-auto d-inline-block"
                        v-model="item.maDVT" @change="doiDonVi(item)">
                        <option v-for="dvt in item.danhSachDVT" :key="dvt.maDVT" :value="dvt.maDVT">
                          {{ dvt.tenDonVi }} — {{ formatGia(dvt.giaBan) }}
                        </option>
                      </select>
                    </div>
                  </div>
                  <div class="cart-controls">
                    <div class="cart-qty-group mb-1">
                      <div class="input-group input-group-sm">
                        <div class="input-group-prepend">
                          <button class="btn btn-outline-primary" type="button"
                            @click="giamSoLuong(item)">&minus;</button>
                        </div>
                        <input type="text" class="form-control text-center"
                          :value="item.soLuong"
                          @change="capNhatSoLuong(item, $event.target.value)" />
                        <div class="input-group-append">
                          <button class="btn btn-outline-primary" type="button"
                            @click="tangSoLuong(item)">&plus;</button>
                        </div>
                      </div>
                    </div>
                    <div class="cart-price">{{ formatGia(item.giaBan) }}</div>
                    <a href="#" class="cart-remove text-danger small d-inline-block mt-1"
                      @click.prevent="xoaSanPham(item)">Xóa</a>
                  </div>
                </div>
              </div>

              <!-- Mã giảm giá -->
              <div class="cart-coupon">
                <label class="mb-1">Mã khuyến mãi</label>
                <p>Nhập mã giảm giá nếu bạn có.</p>
                <div class="form-row">
                  <div class="col-md-8 mb-2 mb-md-0">
                    <input type="text" class="form-control py-2" v-model="maGiamGia"
                      placeholder="Nhập mã giảm giá" />
                  </div>
                  <div class="col-md-4">
                    <button class="btn btn-outline-primary btn-block" @click="apDungMa">Áp dụng</button>
                  </div>
                </div>
                <p v-if="thongBaoMa" class="small mt-2"
                  :class="apDungThanhCong ? 'text-success' : 'text-danger'">
                  {{ thongBaoMa }}
                </p>
              </div>

              <!-- Hành động -->
              <div class="cart-actions">
                <button type="button" class="btn btn-outline-secondary btn-sm" @click="capNhatGioHang">
                  Cập nhật giỏ hàng
                </button>
                <button type="button" class="btn btn-link btn-sm text-danger p-0" @click="xoaTatCa">
                  Xóa tất cả sản phẩm
                </button>
              </div>
            </div>
          </div>

          <!-- Tổng cộng -->
          <div class="cart-summary">
            <div class="summary-card">
              <div class="summary-title">Tổng cộng</div>
              <div class="summary-row">
                <span>Tạm tính</span>
                <span>{{ formatGia(tamTinh) }}</span>
              </div>
              <div class="summary-row" v-if="soTienGiam > 0">
                <span>Giảm giá</span>
                <span class="text-danger">-{{ formatGia(soTienGiam) }}</span>
              </div>
              <div class="summary-row">
                <span>Phí vận chuyển</span>
                <span>Miễn phí</span>
              </div>
              <div class="summary-row total">
                <span>Thành tiền</span>
                <span>{{ formatGia(tongTien) }}</span>
              </div>
              <div class="summary-note">
                Giá đã bao gồm VAT (nếu có). Vui lòng kiểm tra lại sản phẩm trước khi đặt hàng.
              </div>
              <div class="summary-btn">
                <button class="btn btn-primary btn-block"
                  :disabled="gioHang.length === 0"
                  @click="tiepTucDatHang">
                  Tiếp tục đặt hàng
                </button>
              </div>
            </div>
          </div>

        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import '../../assets/css/cart-page.css';
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import axiosClient from '../../api/axiosClient';

const router     = useRouter();
const gioHang    = ref([]);   // Dữ liệu từ bảng GioHang JOIN Thuoc, DonViTinh
const dangTai    = ref(false);
const maGiamGia  = ref('');
const soTienGiam = ref(0);
const thongBaoMa      = ref('');
const apDungThanhCong = ref(false);

// ── Load giỏ hàng từ API ──
// API trả về: MaGioHang, MaKhachHang, MaThuoc, MaDVT, SoLuong
// JOIN thêm: TenThuoc, HinhAnhChinh, MoTaNgan (Thuoc)
//            TenDonVi, GiaBan (DonViTinh)
//            DanhSachDVT[] - tất cả đơn vị tính của thuốc đó
const loadGioHang = async () => {
  dangTai.value = true;
  try {
    const res = await axiosClient.get('/GioHang');
    gioHang.value = res.data;
  } catch (err) {
    console.error('Lỗi tải giỏ hàng:', err);
  } finally {
    dangTai.value = false;
  }
};

// ── Tính tổng ──
const tamTinh = computed(() =>
  gioHang.value.reduce((sum, item) => sum + item.giaBan * item.soLuong, 0)
);
const tongTien = computed(() => tamTinh.value - soTienGiam.value);

// ── Thay đổi số lượng ──
const tangSoLuong = (item) => {
  item.soLuong++;
};

const giamSoLuong = (item) => {
  if (item.soLuong > 1) item.soLuong--;
};

const capNhatSoLuong = (item, val) => {
  const so = parseInt(val);
  if (so > 0) item.soLuong = so;
};

// ── Đổi đơn vị tính (DonViTinh) ──
// Khi đổi MaDVT, cập nhật giá tương ứng
const doiDonVi = (item) => {
  const dvt = item.danhSachDVT.find(d => d.maDVT === item.maDVT);
  if (dvt) {
    item.giaBan  = dvt.giaBan;
    item.tenDonVi = dvt.tenDonVi;
  }
};

// ── Xóa sản phẩm ──
// DELETE /GioHang/{maGioHang}
const xoaSanPham = async (item) => {
  try {
    await axiosClient.delete(`/GioHang/${item.maGioHang}`);
    gioHang.value = gioHang.value.filter(i => i.maGioHang !== item.maGioHang);
  } catch (err) {
    console.error('Lỗi xóa sản phẩm:', err);
  }
};

// ── Xóa tất cả ──
const xoaTatCa = async () => {
  try {
    await axiosClient.delete('/GioHang/xoa-tat-ca');
    gioHang.value = [];
  } catch (err) {
    console.error('Lỗi xóa giỏ hàng:', err);
  }
};

// ── Cập nhật giỏ hàng (đồng bộ SoLuong + MaDVT lên server) ──
// PUT /GioHang/cap-nhat — body: [{ maGioHang, soLuong, maDVT }]
const capNhatGioHang = async () => {
  try {
    await axiosClient.put('/GioHang/cap-nhat', gioHang.value.map(i => ({
      maGioHang: i.maGioHang,
      soLuong:   i.soLuong,
      maDVT:     i.maDVT,
    })));
  } catch (err) {
    console.error('Lỗi cập nhật giỏ hàng:', err);
  }
};

// ── Áp dụng mã giảm giá ──
const apDungMa = async () => {
  thongBaoMa.value = '';
  if (!maGiamGia.value.trim()) return;
  try {
    const res = await axiosClient.post('/GioHang/ap-dung-ma', {
      ma: maGiamGia.value.trim(),
      tongTien: tamTinh.value,
    });
    soTienGiam.value    = res.data.soTienGiam;
    thongBaoMa.value    = `Áp dụng thành công! Giảm ${formatGia(soTienGiam.value)}`;
    apDungThanhCong.value = true;
  } catch {
    thongBaoMa.value      = 'Mã không hợp lệ hoặc đã hết hạn.';
    apDungThanhCong.value = false;
    soTienGiam.value      = 0;
  }
};

// ── Tiếp tục đặt hàng ──
// Đồng bộ giỏ hàng rồi chuyển sang trang checkout
const tiepTucDatHang = async () => {
  await capNhatGioHang();
  router.push({ name: 'DatHang' });
};

const getImageUrl = (path) => {
  if (!path) return '/images/no-image.png';
  if (path.startsWith('http')) return path;
  return `https://localhost:7070${path}`;
};

const formatGia = (value) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value ?? 0);

onMounted(loadGioHang);
</script>
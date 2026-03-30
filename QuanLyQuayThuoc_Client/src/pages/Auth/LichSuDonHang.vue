<template>
  <div class="site-wrap">
    <div class="account-wrapper">
      <div class="container">
        <div class="row">
          
          <AccountSidebar 
            :user="nguoiDungSidebar" 
            activeMenu="orders" 
            activeTitle="Đơn hàng của tôi"
            @logout="dangXuat"
          />

          <div class="col-lg-9">
            <div class="account-content-card">
              <div class="account-content-header d-flex align-items-center justify-content-between">
                <div>
                  <h2 class="mb-1">Đơn hàng của tôi</h2>
                  <p class="mb-0 text-muted small">Theo dõi trạng thái các đơn hàng tại hệ thống</p>
                </div>
                <div class="orders-search d-none d-md-block">
                  <div class="input-group">
                    <input type="text" class="form-control" v-model="tuKhoa"
                      placeholder="Tìm mã đơn hoặc sản phẩm..." />
                  </div>
                </div>
              </div>

              <div class="orders-tabs mt-4">
                <ul class="nav nav-tabs">
                  <li class="nav-item" v-for="tab in tabs" :key="tab.value">
                    <a class="nav-link" :class="{ active: tabHienTai === tab.value }"
                      href="#" @click.prevent="tabHienTai = tab.value">
                      {{ tab.label }}
                    </a>
                  </li>
                </ul>
              </div>

              <div v-if="dangTai" class="text-center py-5">
                <div class="spinner-border text-primary" role="status">
                  <span class="visually-hidden">Đang tải...</span>
                </div>
              </div>

              <div v-else class="orders-list mt-3">
                <div v-if="donHangDaLoc.length === 0" class="text-center py-5 text-muted">
                  <span class="icon-list" style="font-size: 3rem;"></span>
                  <p class="mt-3">Không tìm thấy đơn hàng nào phù hợp.</p>
                </div>

                <div class="order-item mb-4 p-3 border rounded shadow-sm" v-for="don in donHangDaLoc" :key="don.maDonHang">
                  <div class="order-item-header d-flex justify-content-between align-items-center mb-3 pb-2 border-bottom">
                    <div class="order-meta">
                      <span class="fw-bold">Mã đơn: #{{ don.maDonHang }}</span>
                      <span class="mx-2 text-muted">|</span>
                      <span class="text-muted">{{ dinhDangNgay(don.ngayDat) }}</span>
                    </div>
                    <div class="order-tag badge" :class="bgTrangThai(don.trangThai)">
                      {{ don.trangThai }}
                    </div>
                  </div>

                  <div class="order-item-body d-flex">
                    <div class="order-thumb me-3">
                      <img :src="getFullUrl(don.hinhAnh)" alt="Sản phẩm" class="img-fluid rounded" style="width: 80px; height: 80px; object-fit: cover;" />
                    </div>
                    <div class="order-info flex-grow-1">
                      <div class="fw-bold text-truncate" style="max-width: 300px;">{{ don.tenSanPham }}</div>
                      <div v-if="don.soSanPhamKhac > 0" class="small text-muted">
                        +{{ don.soSanPhamKhac }} sản phẩm khác
                      </div>
                      <button class="btn btn-link btn-sm p-0 mt-1" @click="moXemChiTiet(don.maDonHang)">
                        Xem chi tiết
                      </button>
                    </div>
                    <div class="order-summary text-end">
                      <div class="text-muted small">x{{ don.soLuong }} {{ don.donVi }}</div>
                      <div class="mt-2">
                        Thành tiền: <span class="text-primary fw-bold">{{ formatGia(don.tongTien) }}</span>
                      </div>
                      <button type="button" class="btn btn-outline-primary btn-sm mt-2 rounded-pill px-4"
                        @click="muaLai(don)">
                        Mua lại
                      </button>
                    </div>
                  </div>
                </div>
              </div>

            </div>
          </div>

        </div>
      </div>
    </div>

    <div class="modal fade" id="modalChiTiet" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog modal-lg modal-dialog-centered">
        <div class="modal-content" v-if="chiTiet">
          <div class="modal-header bg-light">
            <h5 class="modal-title fw-bold">Chi tiết đơn hàng #{{ chiTiet.maDonHang }}</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
          </div>
          <div class="modal-body">
            <div class="row mb-3 pb-3 border-bottom">
              <div class="col-md-6">
                <p class="mb-1 text-muted small text-uppercase fw-bold">Địa chỉ nhận hàng</p>
                <p class="mb-0 fw-bold text-primary">{{ chiTiet.hoTenNguoiNhan || 'Người nhận' }}</p>
                <p class="mb-0 small"><i class="icon-phone me-1"></i>{{ chiTiet.soDienThoaiNhan }}</p>
                <p class="mb-0 small text-secondary"><i class="icon-room me-1"></i>{{ chiTiet.diaChiGiaoHang }}</p>
              </div>
              <div class="col-md-6 text-md-end mt-3 mt-md-0">
                <p class="mb-1 text-muted small text-uppercase fw-bold">Trạng thái</p>
                <span class="badge mb-2" :class="bgTrangThai(chiTiet.trangThai)">{{ chiTiet.trangThai }}</span>
                <p class="mb-0 small text-muted">Ngày đặt: {{ dinhDangNgay(chiTiet.ngayDat) }}</p>
              </div>
            </div>

            <div class="product-list mt-3">
              <div v-for="sp in chiTiet.sanPhams" :key="sp.tenThuoc" class="d-flex align-items-center mb-3 pb-2 border-bottom-dashed">
                <img :src="getFullUrl(sp.hinhAnh)" class="rounded border" style="width: 60px; height: 60px; object-fit: cover;">
                <div class="ms-3 flex-grow-1">
                  <div class="fw-bold">{{ sp.tenThuoc }}</div>
                  <div class="text-muted small">Số lượng: {{ sp.soLuong }} {{ sp.donVi }}</div>
                </div>
                <div class="text-end">
                  <div class="small text-muted">{{ formatGia(sp.donGia) }}</div>
                  <div class="fw-bold text-dark">{{ formatGia(sp.donGia * sp.soLuong) }}</div>
                </div>
              </div>
            </div>
          </div>
          <div class="modal-footer bg-light d-flex justify-content-between align-items-center">
            <div class="text-start">
              <div class="text-muted small">Tổng cộng</div>
              <div class="fw-bold text-primary fs-4">{{ formatGia(chiTiet.tongTien) }}</div>
            </div>
            <button type="button" class="btn btn-secondary px-4 rounded-pill" data-bs-dismiss="modal">Đóng</button>
          </div>
        </div>
        <div class="modal-content py-5 text-center" v-else>
           <div class="spinner-border text-primary mx-auto"></div>
           <p class="mt-2 text-muted">Đang tải chi tiết...</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import axiosClient from '../../api/axiosClient';
import Swal from 'sweetalert2';
import AccountSidebar from '../../components/AccountSidebar.vue';
import { Modal } from 'bootstrap'; 

const router = useRouter();

// State
const nguoiDungSidebar = ref({ hoTen: '', soDienThoai: '', anhDaiDien: '' });
const donHang = ref([]);
const chiTiet = ref(null); 
const dangTai = ref(false);
const tuKhoa = ref('');
const tabHienTai = ref('');
let modalInstance = null;

const tabs = [
  { label: 'Tất cả',      value: '' },
  { label: 'Chờ xử lý',   value: 'Chờ xử lý' },
  { label: 'Đang giao',   value: 'Đang giao' },
  { label: 'Đã giao',     value: 'Đã giao' },
  { label: 'Đã hủy',      value: 'Đã hủy' },
];

const loadData = async () => {
  dangTai.value = true;
  try {
    const [resUser, resDon] = await Promise.all([
      axiosClient.get('/HoSo/thong-tin'),
      axiosClient.get('/DonHangKhach/cua-toi')
    ]);
    nguoiDungSidebar.value = resUser.data || resUser;
    donHang.value = resDon.data || resDon;
  } catch (err) {
    console.error('Lỗi tải dữ liệu:', err);
    if (err.response?.status === 401) router.push('/auth/dang-nhap');
  } finally {
    dangTai.value = false;
  }
};

const moXemChiTiet = async (id) => {
  chiTiet.value = null; // Hiển thị spinner loading trong modal
  
  if (!modalInstance) {
    modalInstance = new Modal(document.getElementById('modalChiTiet'));
  }
  modalInstance.show();

  try {
    // Chỉ dùng /DonHangKhach/id vì axiosClient đã cấu hình tiền tố /api
    const res = await axiosClient.get(`DonHangKhach/${id}`); 
    chiTiet.value = res.data;
  } catch (err) {
    console.error("Lỗi gọi API chi tiết:", err);
    modalInstance.hide();
    Swal.fire('Lỗi', 'Không thể lấy thông tin đơn hàng', 'error');
  }
};

const donHangDaLoc = computed(() =>
  donHang.value.filter(don => {
    const khopTab = !tabHienTai.value || don.trangThai === tabHienTai.value;
    const searchVal = tuKhoa.value.toLowerCase();
    const khopTuKhoa = !tuKhoa.value
      || String(don.maDonHang).includes(searchVal)
      || don.tenSanPham?.toLowerCase().includes(searchVal);
    return khopTab && khopTuKhoa;
  })
);

const getFullUrl = (path) => {
  if (!path) return '/img/default-product.png';
  if (path.startsWith('http')) return path;
  // Thêm tiền tố /uploads/ nếu database chỉ lưu tên file
  const prefix = path.startsWith('/') ? '' : '/uploads/';
  return `https://localhost:7070${prefix}${path}`;
};

const dinhDangNgay = (val) => {
  if (!val) return '---';
  return new Date(val).toLocaleDateString('vi-VN', {
    day: '2-digit', month: '2-digit', year: 'numeric',
    hour: '2-digit', minute: '2-digit'
  });
};

const formatGia = (value) => 
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value || 0);

const bgTrangThai = (trangThai) => {
  switch (trangThai) {
    case 'Đã giao': return 'bg-success';
    case 'Đang giao': return 'bg-info text-white';
    case 'Chờ xử lý': return 'bg-warning text-dark';
    case 'Đã hủy': return 'bg-danger';
    default: return 'bg-secondary text-white';
  }
};

const muaLai = async (don) => {
  Swal.fire({
    icon: 'success',
    title: 'Đã thêm vào giỏ hàng',
    text: `Đơn hàng #${don.maDonHang} đã được thêm lại.`,
    showConfirmButton: true,
    confirmButtonText: 'Xem giỏ hàng'
  }).then((result) => {
    if (result.isConfirmed) router.push('/gio-hang');
  });
};

const dangXuat = () => {
  Swal.fire({
    title: 'Đăng xuất?',
    icon: 'question',
    showCancelButton: true,
    confirmButtonText: 'Đồng ý',
    cancelButtonText: 'Hủy'
  }).then(r => {
    if (r.isConfirmed) {
      localStorage.clear();
      router.push('/auth/dang-nhap');
    }
  });
};

onMounted(loadData);
</script>

<style scoped>
.order-item {
  background: #fff;
  transition: transform 0.2s;
}
.order-item:hover {
  transform: translateY(-2px);
}
.order-tag {
  font-size: 0.75rem;
  padding: 5px 12px;
  border-radius: 50px;
}
.nav-tabs .nav-link {
  color: #666;
  border: none;
  border-bottom: 2px solid transparent;
}
.nav-tabs .nav-link.active {
  color: #007bff;
  border-bottom: 2px solid #007bff;
  font-weight: bold;
}
.border-bottom-dashed {
  border-bottom: 1px dashed #dee2e6;
}
</style>
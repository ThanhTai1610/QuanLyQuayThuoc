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
                      <router-link :to="`/don-hang/${don.maDonHang}`" class="btn btn-link btn-sm p-0 mt-1">
                        Xem chi tiết
                      </router-link>
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
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import axiosClient from '../../api/axiosClient';
import Swal from 'sweetalert2';
import AccountSidebar from '../../components/AccountSidebar.vue';

const router = useRouter();

// State cho Sidebar (Dùng chung cấu trúc với trang HoSo)
const nguoiDungSidebar = ref({ hoTen: '', soDienThoai: '', anhDaiDien: '' });

// State cho Đơn hàng
const donHang = ref([]);
const dangTai = ref(false);
const tuKhoa = ref('');
const tabHienTai = ref('');

const tabs = [
  { label: 'Tất cả',      value: '' },
  { label: 'Chờ xử lý',   value: 'Đang xử lý' },
  { label: 'Đang giao',   value: 'Đang giao' },
  { label: 'Đã giao',     value: 'Đã giao' },
  { label: 'Đã hủy',      value: 'Đã hủy' },
];

const loadData = async () => {
  dangTai.value = true;
  
  // 1. Lấy thông tin User để hiện ảnh đại diện (Không để đơn hàng làm lỗi cái này)
  try {
    const resUser = await axiosClient.get('/HoSo/thong-tin');
    // Lưu ý: Kiểm tra xem resUser có .data không tùy vào cấu hình axios của bạn
    nguoiDungSidebar.value = resUser.data || resUser;
  } catch (err) {
    console.error('Lỗi lấy profile:', err);
  }

  // 2. Lấy đơn hàng (Nếu lỗi 404 cũng không sao, ảnh vẫn đã hiện ở trên)
  try {
    const resDon = await axiosClient.get('/DonHang/cua-toi');
    donHang.value = resDon.data || resDon;
  } catch (err) {
    console.error('Lỗi lấy đơn hàng:', err);
    donHang.value = []; // Gán mảng rỗng để giao diện không bị crash
  } finally {
    dangTai.value = false;
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

// Helpers
const getFullUrl = (path) => {
  if (!path) return 'https://via.placeholder.com/80';
  return path.startsWith('http') ? path : `https://localhost:7070${path}`;
};

const dinhDangNgay = (val) => val ? new Date(val).toLocaleDateString('vi-VN') : '';

const formatGia = (value) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);

const bgTrangThai = (trangThai) => ({
  'bg-success': trangThai === 'Đã giao',
  'bg-primary': trangThai === 'Đang giao',
  'bg-warning text-dark': trangThai === 'Đang xử lý',
  'bg-danger':  trangThai === 'Đã hủy',
  'bg-secondary': trangThai === 'Trả hàng',
});

const muaLai = (don) => {
  // Logic mua lại (Ví dụ: Chuyển hướng sang trang sản phẩm hoặc thêm thẳng vào giỏ)
  Swal.fire({
    icon: 'success',
    title: 'Đã thêm vào giỏ hàng',
    text: `Đang chuẩn bị đơn hàng cho ${don.tenSanPham}`,
    timer: 1500,
    showConfirmButton: false
  });
  router.push('/gio-hang');
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
</style>
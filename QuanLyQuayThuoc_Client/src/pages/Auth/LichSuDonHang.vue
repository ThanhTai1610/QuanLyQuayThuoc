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

                      <div class="mt-2">
                        <button v-if="don.trangThai === 'Chờ xử lý'" 
                                type="button" 
                                class="btn btn-outline-danger btn-sm rounded-pill px-3"
                                @click="huyDonHang(don.maDonHang)">
                          Hủy đơn hàng
                        </button>

                        <button v-else-if="don.trangThai === 'Đang giao'" 
                                type="button" 
                                class="btn btn-secondary btn-sm rounded-pill px-3" 
                                disabled 
                                title="Đơn hàng đang trên đường giao, không thể hủy">
                          Đang giao...
                        </button>

                        <button v-else-if="don.trangThai === 'Đã giao'" 
                                type="button" 
                                class="btn btn-outline-primary btn-sm rounded-pill px-4"
                                @click="muaLai(don)">
                          Mua lại
                        </button>
                        
                        <button v-else-if="don.trangThai === 'Đã hủy'" 
                                type="button" 
                                class="btn btn-light btn-sm rounded-pill px-4"
                                @click="muaLai(don)">
                          Đặt lại đơn
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
  { label: 'Tất cả',    value: '' },
  { label: 'Chờ xử lý', value: 'Chờ xử lý' },
  { label: 'Đang giao', value: 'Đang giao' },
  { label: 'Đã giao',   value: 'Đã giao' },
  { label: 'Đã hủy',    value: 'Đã hủy' },
];

// Chuyển PascalCase → camelCase đệ quy
const toCamel = (obj) => {
  if (Array.isArray(obj)) return obj.map(toCamel);
  if (obj !== null && typeof obj === 'object') {
    return Object.fromEntries(
      Object.entries(obj).map(([k, v]) => [
        k.charAt(0).toLowerCase() + k.slice(1),
        toCamel(v)
      ])
    );
  }
  return obj;
};

const loadData = async () => {
  dangTai.value = true;
  try {
    const [resUser, resDon] = await Promise.all([
      axiosClient.get('/HoSo/thong-tin'),
      axiosClient.get('/DonHangKhach/cua-toi')
    ]);
    // interceptor đã unwrap response.data, nên res chính là data
    nguoiDungSidebar.value = toCamel(resUser);
    donHang.value = toCamel(resDon);
  } catch (err) {
    console.error('Lỗi tải dữ liệu:', err);
    if (err.response?.status === 401) router.push('/auth/dang-nhap');
  } finally {
    dangTai.value = false;
  }
};

// const loadData = async () => {
//   dangTai.value = true;
//   try {
//     // Tạm thời comment API thật
//     // const [resUser, resDon] = await Promise.all([...]);

//     // Dữ liệu giả lập 4 trạng thái đơn hàng
//     const mockDonHang = [
//       {
//         MaDonHang: 1001,
//         NgayDat: "2026-04-01T10:00:00",
//         TrangThai: "Chờ xử lý", // Sẽ hiện nút [Hủy đơn hàng]
//         TenSanPham: "Thuốc Panadol Extra Đỏ",
//         SoLuong: 2,
//         DonVi: "Hộp",
//         TongTien: 150000,
//         HinhAnh: "/images/panadol.jpg",
//         SoSanPhamKhac: 1
//       },
//       {
//         MaDonHang: 1002,
//         NgayDat: "2026-03-30T14:30:00",
//         TrangThai: "Đang giao", // Sẽ hiện nút [Đang giao...] bị mờ (disabled)
//         TenSanPham: "Vitamin C Berocca",
//         SoLuong: 1,
//         DonVi: "Tuýp",
//         TongTien: 85000,
//         HinhAnh: "/images/berocca.jpg",
//         SoSanPhamKhac: 0
//       },
//       {
//         MaDonHang: 1003,
//         NgayDat: "2026-03-25T08:15:00",
//         TrangThai: "Đã giao", // Sẽ hiện nút [Mua lại] màu xanh dương
//         TenSanPham: "Khẩu trang N95",
//         SoLuong: 5,
//         DonVi: "Cái",
//         TongTien: 125000,
//         HinhAnh: "/images/mask.jpg",
//         SoSanPhamKhac: 2
//       },
//       {
//         MaDonHang: 1004,
//         NgayDat: "2026-03-20T16:00:00",
//         TrangThai: "Đã hủy", // Sẽ hiện nút [Đặt lại đơn] màu nhạt
//         TenSanPham: "Nước rửa tay Lifebuoy",
//         SoLuong: 1,
//         DonVi: "Chai",
//         TongTien: 45000,
//         HinhAnh: "/images/lifebuoy.jpg",
//         SoSanPhamKhac: 0
//       }
//     ];

//     nguoiDungSidebar.value = { hoTen: 'Long IT', soDienThoai: '090xxxxxxx' };
//     donHang.value = toCamel(mockDonHang); // Chuyển sang camelCase để khớp logic template

//   } catch (err) {
//     console.error('Lỗi test:', err);
//   } finally {
//     dangTai.value = false;
//   }
// };

const moXemChiTiet = async (id) => {
  chiTiet.value = null; // hiện spinner trong modal
  if (!modalInstance) {
    modalInstance = new Modal(document.getElementById('modalChiTiet'));
  }
  modalInstance.show();

  try {
    const res = await axiosClient.get(`/DonHangKhach/${id}`);
    // interceptor đã unwrap, res chính là data — không cần res.data
    chiTiet.value = toCamel(res);
  } catch (err) {
    console.error('Lỗi gọi API chi tiết:', err);
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
    case 'Đã giao':   return 'bg-success';
    case 'Đang giao': return 'bg-info text-white';
    case 'Chờ xử lý': return 'bg-warning text-dark';
    case 'Đã hủy':    return 'bg-danger';
    default:          return 'bg-secondary text-white';
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

// Thêm hàm này vào phần script setup
const huyDonHang = async (id) => {
  const result = await Swal.fire({
    title: 'Xác nhận hủy đơn?',
    text: `Bạn có chắc chắn muốn hủy đơn hàng #${id} không?`,
    icon: 'warning',
    showCancelButton: true,
    confirmButtonColor: '#d33',
    cancelButtonColor: '#3085d6',
    confirmButtonText: 'Đồng ý hủy',
    cancelButtonText: 'Quay lại'
  });

  if (result.isConfirmed) {
    try {
      // Gọi API cập nhật trạng thái đơn hàng thành 'Đã hủy'
      // Long hãy kiểm tra lại endpoint chính xác trên Swagger của mình nhé
      await axiosClient.put(`/DonHangKhach/huy/${id}`); 
      
      Swal.fire('Đã hủy!', 'Đơn hàng của bạn đã được hủy thành công.', 'success');
      
      // Tải lại danh sách để cập nhật giao diện
      loadData(); 
    } catch (err) {
      console.error('Lỗi khi hủy đơn:', err);
      Swal.fire('Thất bại', 'Không thể hủy đơn hàng lúc này. Vui lòng thử lại sau.', 'error');
    }
  }
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
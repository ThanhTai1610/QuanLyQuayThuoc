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
                <div v-if="donHangHienThi.length === 0" class="text-center py-5 text-muted">
                  <span class="icon-list" style="font-size: 3rem;"></span>
                  <p class="mt-3">Không tìm thấy đơn hàng nào phù hợp.</p>
                </div>

                <div class="order-item mb-4 p-3 border rounded shadow-sm" v-for="don in donHangHienThi" :key="don.maDonHang">
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

                <nav v-if="tongSoTrang > 1" class="mt-5 d-flex justify-content-center">
                  <ul class="pagination pagination-rounded">
                    <li class="page-item" :class="{ disabled: trangHienTai === 1 }">
                      <a class="page-link" href="#" @click.prevent="chuyenTrang(trangHienTai - 1)">&laquo;</a>
                    </li>
                    <li v-for="p in tongSoTrang" :key="p" class="page-item" :class="{ active: trangHienTai === p }">
                      <a class="page-link" href="#" @click.prevent="chuyenTrang(p)">{{ p }}</a>
                    </li>
                    <li class="page-item" :class="{ disabled: trangHienTai === tongSoTrang }">
                      <a class="page-link" href="#" @click.prevent="chuyenTrang(trangHienTai + 1)">&raquo;</a>
                    </li>
                  </ul>
                </nav>

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
import { ref, computed, onMounted, watch } from 'vue';
import { useRouter } from 'vue-router';
import axiosClient from '../../api/axiosClient';
import Swal from 'sweetalert2';
import AccountSidebar from '../../components/AccountSidebar.vue';
import { Modal } from 'bootstrap';

const router = useRouter();

// State dữ liệu
const nguoiDungSidebar = ref({ hoTen: '', soDienThoai: '', anhDaiDien: '' });
const donHang = ref([]);
const chiTiet = ref(null);
const dangTai = ref(false);
const tuKhoa = ref('');
const tabHienTai = ref('');
let modalInstance = null;

// State phân trang
const trangHienTai = ref(1);
const soDonMoiTrang = 5;

const tabs = [
  { label: 'Tất cả',    value: '' },
  { label: 'Chờ xử lý', value: 'Chờ xử lý' },
  { label: 'Đang giao', value: 'Đang giao' },
  { label: 'Đã giao',   value: 'Đã giao' },
  { label: 'Đã hủy',    value: 'Đã hủy' },
];

// Logic lọc dữ liệu
const tatCaDonDaLoc = computed(() =>
  donHang.value.filter(don => {
    const khopTab = !tabHienTai.value || don.trangThai === tabHienTai.value;
    const searchVal = tuKhoa.value.toLowerCase();
    const khopTuKhoa = !tuKhoa.value
      || String(don.maDonHang).includes(searchVal)
      || don.tenSanPham?.toLowerCase().includes(searchVal);
    return khopTab && khopTuKhoa;
  })
);

// Tính toán phân trang
const tongSoTrang = computed(() => 
  Math.ceil(tatCaDonDaLoc.value.length / soDonMoiTrang)
);

const donHangHienThi = computed(() => {
  const batDau = (trangHienTai.value - 1) * soDonMoiTrang;
  const ketThuc = batDau + soDonMoiTrang;
  return tatCaDonDaLoc.value.slice(batDau, ketThuc);
});

const chuyenTrang = (trang) => {
  if (trang >= 1 && trang <= tongSoTrang.value) {
    trangHienTai.value = trang;
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
};

// Reset trang khi lọc
watch([tabHienTai, tuKhoa], () => {
  trangHienTai.value = 1;
});

// Utils
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
    nguoiDungSidebar.value = toCamel(resUser);
    donHang.value = toCamel(resDon);
  } catch (err) {
    console.error('Lỗi tải dữ liệu:', err);
    if (err.response?.status === 401) router.push('/auth/dang-nhap');
  } finally {
    dangTai.value = false;
  }
};

const moXemChiTiet = async (id) => {
  chiTiet.value = null;
  if (!modalInstance) {
    modalInstance = new Modal(document.getElementById('modalChiTiet'));
  }
  modalInstance.show();
  try {
    const res = await axiosClient.get(`/DonHangKhach/${id}`);
    chiTiet.value = toCamel(res);
  } catch (err) {
    modalInstance.hide();
    Swal.fire('Lỗi', 'Không thể lấy thông tin đơn hàng', 'error');
  }
};

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
  // Log để kiểm tra dữ liệu 'don' có chứa đủ maThuoc và maDVT không
  console.log("Dữ liệu đơn hàng chọn mua lại:", don);

  // Kiểm tra điều kiện trước khi gọi API để tránh lỗi 500 do thiếu dữ liệu
  if (!don.maThuoc || !don.maDVT) {
    Swal.fire({
      icon: 'warning',
      title: 'Thiếu thông tin',
      text: 'Không tìm thấy mã sản phẩm hoặc đơn vị tính để thực hiện mua lại.',
    });
    return;
  }

  try {
    // 1. Gọi API để thêm sản phẩm vào giỏ hàng thực tế
    // Lưu ý: Đảm bảo tên thuộc tính (maThuoc, maDVT) khớp chính xác với DTO ở Backend C#
    await axiosClient.post('/GioHang/them', {
      maThuoc: don.maThuoc, 
      maDVT: don.maDVT,     
      soLuong: don.soLuong || 1
    });

    // 2. Thông báo thành công và điều hướng bằng SweetAlert2
    Swal.fire({
      icon: 'success',
      title: 'Thành công!',
      text: `Sản phẩm từ đơn hàng #${don.maDonHang || don.id} đã được thêm vào giỏ hàng.`,
      showConfirmButton: true,
      confirmButtonText: 'Đến giỏ hàng',
      showCancelButton: true,
      cancelButtonText: 'Ở lại đây',
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#aaa',
    }).then((result) => {
      if (result.isConfirmed) {
        // Điều hướng sang trang giỏ hàng
        router.push('/gio-hang');
      }
    });

  } catch (err) {
    // Log chi tiết lỗi từ server trả về để dễ debug
    console.error('Lỗi khi thực hiện mua lại:', err.response?.data || err.message);
    
    Swal.fire({
      icon: 'error',
      title: 'Thất bại',
      text: err.response?.data?.message || 'Không thể thêm sản phẩm vào giỏ hàng. Vui lòng thử lại sau.',
    });
  }
};

const huyDonHang = async (id) => {
  const { value: formValues } = await Swal.fire({
    title: '<span style="font-size: 20px; font-weight: bold; color: #333;">Xác nhận hủy đơn hàng</span>',
    html: `
      <div style="text-align: left; margin-top: 15px; font-family: sans-serif;">
        <p style="font-size: 14px; color: #666; margin-bottom: 15px;">Vui lòng chọn lý do để Pharmative cải thiện dịch vụ tốt hơn:</p>
        <style>
          .cancel-option { display: flex; align-items: center; margin-bottom: 12px; cursor: pointer; }
          .cancel-option input { margin-right: 10px; width: 18px; height: 18px; cursor: pointer; }
          .cancel-option label { cursor: pointer; font-size: 15px; color: #444; margin: 0; }
          .reason-text { width: 100%; padding: 10px; border: 1px solid #ddd; border-radius: 6px; font-size: 14px; margin-top: 10px; display: none; }
        </style>
        <div class="cancel-option"><input type="radio" name="cancelReason" value="Thay đổi ý định" id="r1" checked><label for="r1">Thay đổi ý định mua hàng</label></div>
        <div class="cancel-option"><input type="radio" name="cancelReason" value="Giá rẻ hơn" id="r2"><label for="r2">Tìm thấy giá rẻ hơn ở nơi khác</label></div>
        <div class="cancel-option"><input type="radio" name="cancelReason" value="Giao lâu" id="r3"><label for="r3">Thời gian giao hàng quá lâu</label></div>
        <div class="cancel-option"><input type="radio" name="cancelReason" value="Đặt trùng" id="r4"><label for="r4">Đặt trùng đơn hàng</label></div>
        <div class="cancel-option"><input type="radio" name="cancelReason" value="khac" id="r5"><label for="r5">Lý do khác...</label></div>
        <textarea id="otherReasonText" class="reason-text" placeholder="Nhập lý do cụ thể..." rows="3"></textarea>
      </div>
    `,
    showCancelButton: true,
    confirmButtonText: 'Xác nhận hủy',
    cancelButtonText: 'Quay lại',
    confirmButtonColor: '#d33',
    didOpen: () => {
      const container = Swal.getHtmlContainer();
      const radios = container.querySelectorAll('input[name="cancelReason"]');
      const textarea = container.querySelector('#otherReasonText');
      radios.forEach(r => r.addEventListener('change', (e) => {
        textarea.style.display = (e.target.value === 'khac') ? 'block' : 'none';
        if (e.target.value === 'khac') textarea.focus();
      }));
    },
    preConfirm: () => {
      const container = Swal.getHtmlContainer();
      const selected = container.querySelector('input[name="cancelReason"]:checked').value;
      const other = container.querySelector('#otherReasonText').value;
      
      if (selected === 'khac' && !other.trim()) {
        Swal.showValidationMessage('Vui lòng nhập lý do cụ thể');
        return false;
      }
      return { reason: selected === 'khac' ? other : selected };
    }
  });

  if (formValues) {
    try {
      // Gọi API hủy đơn hàng với lý do đã chọn
      await axiosClient.put(`/DonHangKhach/huy/${id}`, { lyDo: formValues.reason });
      
      Swal.fire({ 
        icon: 'success', 
        title: 'Đã hủy đơn hàng', 
        text: 'Đơn hàng của bạn đã được hủy thành công.',
        timer: 2000, 
        showConfirmButton: false 
      });
      
      // Tải lại danh sách đơn hàng để cập nhật trạng thái UI
      loadData(); 
    } catch (err) {
      console.error('Lỗi khi hủy đơn:', err);
      Swal.fire('Lỗi', err.response?.data?.message || 'Không thể kết nối đến máy chủ để hủy đơn hàng.', 'error');
    }
  }
};

const dangXuat = () => {
  Swal.fire({ title: 'Đăng xuất?', icon: 'question', showCancelButton: true, confirmButtonText: 'Đồng ý' }).then(r => {
    if (r.isConfirmed) { localStorage.clear(); router.push('/auth/dang-nhap'); }
  });
};

onMounted(loadData);
</script>

<style scoped>
.order-item { background: #fff; transition: transform 0.2s; }
.order-item:hover { transform: translateY(-2px); }
.order-tag { font-size: 0.75rem; padding: 5px 12px; border-radius: 50px; }
.nav-tabs .nav-link { color: #666; border: none; border-bottom: 2px solid transparent; }
.nav-tabs .nav-link.active { color: #007bff; border-bottom: 2px solid #007bff; font-weight: bold; }
.border-bottom-dashed { border-bottom: 1px dashed #dee2e6; }

/* CSS Phân trang */
.pagination-rounded .page-link {
  border-radius: 50% !important;
  margin: 0 3px;
  border: none;
  width: 35px;
  height: 35px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #666;
}
.pagination-rounded .page-item.active .page-link {
  background-color: #007bff;
  color: #fff;
}
</style>
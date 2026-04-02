<template>
  <div class="site-wrap">
    <div class="container py-4">

      <div class="d-flex align-items-center justify-content-between mb-3">
        <router-link to="/" class="btn btn-link p-0">← Về trang chủ</router-link>
        <div class="products-page-title">Danh sách sản phẩm nhà thuốc</div>
      </div>

      <div class="row">

        <div class="col-lg-3 mb-4">
          <aside class="filter-sidebar">
            <h2 class="filter-title">Bộ lọc thông minh</h2>

            <div class="filter-group">
              <div class="filter-label">Danh mục</div>
              <label class="filter-option" v-for="dm in danhSachDanhMuc" :key="dm.maDanhMuc">
                <input type="checkbox" :value="dm.maDanhMuc" v-model="locDanhMuc" />
                {{ dm.tenDanhMuc }}
              </label>
            </div>

            <div class="filter-group">
              <div class="filter-label">Đối tượng</div>
              <label class="filter-option" v-for="dt in danhSachDoiTuong" :key="dt">
                <input type="checkbox" :value="dt" v-model="locDoiTuong" />
                {{ dt }}
              </label>
            </div>

            <div class="filter-group">
              <div class="filter-label">Nhà sản xuất</div>
              <label class="filter-option" v-for="nsx in danhSachNSX" :key="nsx">
                <input type="checkbox" :value="nsx" v-model="locNSX" />
                {{ nsx }}
              </label>
            </div>

            <div class="filter-group">
              <div class="filter-label">Khoảng giá</div>
              <label class="filter-option" v-for="g in danhSachGia" :key="g.value">
                <input type="radio" name="gia" :value="g.value" v-model="locGia" />
                {{ g.label }}
              </label>
            </div>

            <div class="filter-group mb-0">
              <div class="filter-label">Dạng bào chế</div>
              <label class="filter-option" v-for="dbc in danhSachDBC" :key="dbc">
                <input type="checkbox" :value="dbc" v-model="locDBC" />
                {{ dbc }}
              </label>
            </div>

            <button class="btn btn-outline-secondary btn-sm btn-block mt-3" @click="xoaBoLoc">
              Xóa bộ lọc
            </button>
          </aside>
        </div>

        <div class="col-lg-9">

          <div class="products-toolbar">
            <div class="products-found">
              Tìm thấy <strong>{{ tongSanPham }} sản phẩm</strong>
              <span v-if="tuKhoa"> cho "<em>{{ tuKhoa }}</em>"</span>
            </div>
            <div class="products-sort">
              <label for="sapXep" class="mb-0 mr-2">Sắp xếp theo:</label>
              <select id="sapXep" class="form-control" v-model="sapXep" @change="loadData(true)">
                <option value="ban-chay">Bán chạy nhất</option>
                <option value="gia-tang">Giá thấp đến cao</option>
                <option value="gia-giam">Giá cao đến thấp</option>
                <option value="moi-nhat">Mới nhất</option>
              </select>
            </div>
          </div>

          <div v-if="dangTai" class="text-center py-5">
            <div class="spinner-border text-primary" role="status">
              <span class="sr-only">Đang tải...</span>
            </div>
          </div>

          <div v-else>
            <div class="row" v-if="danhSachSanPham.length > 0">
              <div class="col-md-4 col-sm-6 mb-4" v-for="sp in danhSachSanPham" :key="sp.maThuoc">
                <article class="product-card">
                  <div class="product-origin-badge" v-if="sp.tenThuoc"> 
                    <img :src="getFlagUrl(sp.nuocSanXuat || 'việt nam')" class="flag-icon" />
                    <span>{{ sp.nuocSanXuat || 'Việt Nam' }}</span>
                  </div>
                  
                  <div v-if="sp.laThuocKeDon" class="product-badge-prescription">🔴 Thuốc kê đơn</div>

                  <img :src="getImageUrl(sp.hinhAnhChinh)" :alt="sp.tenThuoc" class="product-image" />

                  <h3 class="product-name">{{ sp.tenThuoc }}</h3>

                  <p class="product-meta">{{ sp.quyCach }} • {{ sp.nuocSanXuat || 'Chưa rõ' }}</p>

                  <div class="product-price">
                    {{ formatGia(sp.giaBan) }}
                    <span>/ {{ sp.tenDonVi }}</span>
                  </div>

                  <div class="product-actions">
                    <button class="btn btn-primary btn-sm" @click="themVaoGio(sp)">Chọn mua</button>
                    <router-link
                      :to="{ name: 'ChiTietSanPham', params: { id: sp.maThuoc } }"
                      class="btn btn-outline-primary btn-sm">
                      Xem chi tiết
                    </router-link>
                  </div>
                </article>
                </div>
            </div>

            <div v-else class="text-center py-5 text-muted">
              <p>Không tìm thấy sản phẩm phù hợp.</p>
            </div>

            <nav aria-label="Phân trang sản phẩm" v-if="tongTrang > 1">
              <ul class="pagination justify-content-center products-pagination">
                <li class="page-item" :class="{ disabled: trangHienTai === 1 }">
                  <a class="page-link" href="#" @click.prevent="doiTrang(trangHienTai - 1)">Trước</a>
                </li>
                <li class="page-item"
                  v-for="p in tongTrang" :key="p"
                  :class="{ active: p === trangHienTai }">
                  <a class="page-link" href="#" @click.prevent="doiTrang(p)">{{ p }}</a>
                </li>
                <li class="page-item" :class="{ disabled: trangHienTai === tongTrang }">
                  <a class="page-link" href="#" @click.prevent="doiTrang(trangHienTai + 1)">Sau</a>
                </li>
              </ul>
            </nav>
          </div>

        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import '../../assets/css/products-page.css';
import { ref, watch, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import axiosClient from '../../api/axiosClient';
import Swal from 'sweetalert2';

const route  = useRoute();
const router = useRouter();

// ── State ──
const danhSachSanPham = ref([]);
const dangTai         = ref(false);
const tongSanPham     = ref(0);
const trangHienTai    = ref(1);
const tongTrang       = ref(1);
const soLuongMoiTrang = 12;

const tuKhoa = ref(route.query.q || '');
const sapXep = ref('ban-chay');

const locDanhMuc  = ref([]);
const locDoiTuong = ref([]);
const locNSX      = ref([]);
const locGia      = ref('');
const locDBC      = ref([]);

// ── Dữ liệu cho sidebar ──
const danhSachDanhMuc = ref([]);
const danhSachNSX     = ref([]);
const danhSachDoiTuong = ['Trẻ em', 'Người lớn', 'Phụ nữ mang thai', 'Người già'];
const danhSachDBC      = ['Viên nén', 'Viên nang', 'Siro', 'Bột pha'];
const danhSachGia = [
  { label: 'Dưới 100.000đ',         value: '0-100000'         },
  { label: '100.000đ - 500.000đ',      value: '100000-500000'    },
  { label: '500.000đ - 1.000.000đ',    value: '500000-1000000'   },
  { label: 'Trên 1.000.000đ',          value: '1000000-99999999' },
];

const getFlagUrl = (countryName) => {
  if (!countryName) return 'https://flagcdn.com/w40/vn.png';
  const name = countryName.toLowerCase();

  if (name.includes('việt nam')) return 'https://flagcdn.com/w40/vn.png';
  if (name.includes('hoa kỳ') || name.includes('mỹ') || name.includes('usa')) return 'https://flagcdn.com/w40/us.png';
  
  // Thêm điều kiện cho Vương quốc Anh ở đây
  if (name.includes('anh') || name.includes('vương quốc anh') || name.includes('uk')) return 'https://flagcdn.com/w40/gb.png';

  if (name.includes('pháp')) return 'https://flagcdn.com/w40/fr.png';
  if (name.includes('đức')) return 'https://flagcdn.com/w40/de.png';
  if (name.includes('nhật')) return 'https://flagcdn.com/w40/jp.png';

  return 'https://flagcdn.com/w40/un.png';
};

// ── Load sản phẩm ──
const loadData = async (resetTrang = false) => {
  if (resetTrang) trangHienTai.value = 1;
  dangTai.value = true;
  
  try {
    const params = {
      trang:      trangHienTai.value,
      soLuong:    soLuongMoiTrang,
      sapXep:     sapXep.value,
      q:          tuKhoa.value || undefined,
      danhMuc:    locDanhMuc.value.length > 0 ? locDanhMuc.value.join(',') : undefined,
      doiTuong:   locDoiTuong.value.length > 0 ? locDoiTuong.value.join(',') : undefined,
      nsx:        locNSX.value.length > 0 ? locNSX.value.join(',') : undefined,
      gia:        locGia.value || undefined,
      dangBaoChe: locDBC.value.length > 0 ? locDBC.value.join(',') : undefined,
    };
    
    const res = await axiosClient.get('/Thuoc', { params });
    
    if (res) {
      danhSachSanPham.value = res.items || [];
      tongSanPham.value     = res.total || 0;
      tongTrang.value       = Math.ceil(tongSanPham.value / soLuongMoiTrang);
    } else {
      danhSachSanPham.value = [];
      tongSanPham.value = 0;
    }
  } catch (err) {
    console.error('Lỗi tải sản phẩm:', err);
    danhSachSanPham.value = [];
    tongSanPham.value = 0;
  } finally {
    dangTai.value = false;
  }
};

const loadSidebar = async () => {
  try {
    const [resDM, resNSX] = await Promise.all([
      axiosClient.get('/DanhMuc'),
      axiosClient.get('/Thuoc/nha-san-xuat'),
    ]);
    danhSachDanhMuc.value = resDM.data;
    danhSachNSX.value     = resNSX.data;
  } catch (err) {
    console.error('Lỗi tải sidebar:', err);
  }
};

const doiTrang = (trang) => {
  if (trang < 1 || trang > tongTrang.value) return;
  trangHienTai.value = trang;
  loadData();
  window.scrollTo({ top: 0, behavior: 'smooth' });
};

const themVaoGio = async (sp) => {
  const maDVTSelected = sp.maDVT || 1; 

  try {
    const response = await axiosClient.post('/GioHang/them', {
      MaThuoc: sp.maThuoc,
      MaDvt: maDVTSelected,
      SoLuong: 1,
    });

    // Thông báo bằng SweetAlert2
    Swal.fire({
      icon: 'success',
      title: 'Đã thêm vào giỏ!',
      text: `Sản phẩm ${sp.tenThuoc} đã có trong giỏ hàng của bạn.`,
      showConfirmButton: true,
      confirmButtonText: 'Xem giỏ hàng',
      showCancelButton: true,
      cancelButtonText: 'Tiếp tục mua sắm',
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#aaa',
    }).then((result) => {
      if (result.isConfirmed) {
        router.push('/gio-hang'); // Chuyển trang nếu khách muốn xem giỏ ngay
      }
    });

  } catch (err) {
    console.error('Lỗi thêm giỏ hàng:', err);
    
    if (err.response?.status === 401) {
      Swal.fire({
        icon: 'warning',
        title: 'Yêu cầu đăng nhập',
        text: 'Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng!',
        confirmButtonText: 'Đăng nhập ngay'
      }).then(() => {
        router.push('/dang-nhap'); 
      });
    } else {
      Swal.fire({
        icon: 'error',
        title: 'Lỗi hệ thống',
        text: 'Không thể thêm vào giỏ hàng lúc này.'
      });
    }
  }
};

const xoaBoLoc = () => {
  locDanhMuc.value  = [];
  locDoiTuong.value = [];
  locNSX.value      = [];
  locGia.value      = '';
  locDBC.value      = [];
  loadData(true);
};

watch([locDanhMuc, locDoiTuong, locNSX, locGia, locDBC], () => {
  loadData(true);
}, { deep: true });

watch(() => route.query.q, (newVal) => {
  tuKhoa.value = newVal || '';
  loadData(true);
}, { immediate: true });

const getImageUrl = (path) => {
  // 1. Nếu không có đường dẫn, trả về ảnh mặc định (placeholder)
  if (!path) return 'https://via.placeholder.com/300x300.png?text=No+Image';

  // 2. Nếu đường dẫn bắt đầu bằng http hoặc https (link mạng), dùng luôn link đó
  if (path.startsWith('http')) return path;

  // 3. Nếu là đường dẫn cục bộ (ví dụ: /images/thuoc.jpg), nối với URL của Backend
  return `https://localhost:7070${path.startsWith('/') ? '' : '/'}${path}`;
};

const formatGia = (value) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value ?? 0);

onMounted(() => {
  loadSidebar();
});
</script>

<style>
.product-card {
  position: relative !important;
  background: #fff;
  border-radius: 8px;
  overflow: hidden;
  transition: all 0.3s ease;
  border: 1px solid #eee;
  padding-top: 15px; /* Tăng padding để cờ không bị sát mép */
  display: flex;
  flex-direction: column;
  height: 100%;
}

.product-origin-badge {
  position: absolute;
  top: 10px;
  left: 10px;
  z-index: 10;
  background: rgba(255, 255, 255, 0.9);
  padding: 3px 10px;
  border-radius: 15px;
  display: flex;
  align-items: center;
  font-size: 11px;
  color: #333;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  border: 1px solid #eee;
}

.flag-icon {
  width: 18px !important;
  height: 12px !important;
  object-fit: cover;
  margin-right: 6px;
  border-radius: 2px;
}

.product-badge-prescription {
  position: absolute;
  top: 10px;
  right: 10px;
  z-index: 10;
  font-size: 10px;
  background: white;
  padding: 3px 8px;
  border-radius: 4px;
  border: 1px solid #ff4d4f;
  color: #ff4d4f;
}

.product-image {
  width: 100%;
  height: 160px;
  object-fit: contain;
  margin-top: 15px;
}
</style>
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
                  <div class="product-origin-badge"> 
                    <img :src="getFlagUrl(sp.nuocSanXuat || 'việt nam')" class="flag-icon" />
                    <span>{{ sp.nuocSanXuat || 'Việt Nam' }}</span>
                  </div>
                  
                  <div v-if="sp.laThuocKeDon" class="product-badge-prescription">
                    <i class="fas fa-circle mr-1" style="font-size: 7px;"></i> Thuốc kê đơn
                  </div>

                  <img :src="getImageUrl(sp.hinhAnhChinh)" :alt="sp.tenThuoc" class="product-image" />

                  <h3 class="product-name">{{ sp.tenThuoc }}</h3>

                  <div class="product-category-label" v-if="sp.tenDanhMuc">
                    <i class="fas fa-folder-open mr-1"></i> {{ sp.tenDanhMuc }}
                  </div>

                  <p class="product-meta">{{ sp.quyCach }}</p>

                  <div class="product-price">
                    {{ formatGia(sp.giaBan) }}
                    <span>/ {{ sp.tenDonVi }}</span>
                  </div>

                  <div class="product-actions">
                    <button 
                      class="btn btn-primary btn-sm" 
                      @click="themVaoGio(sp)"
                      :disabled="sp.laThuocKeDon"
                      :title="sp.laThuocKeDon ? 'Sản phẩm này cần có đơn thuốc' : ''"
                    >
                      {{ sp.laThuocKeDon ? 'Cần kê đơn' : 'Chọn mua' }}
                    </button>
                    
                    <router-link
                      :to="{ name: 'ChiTietSanPham', params: { id: sp.maThuoc } }"
                      class="btn btn-outline-primary btn-sm">
                      Chi tiết
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

// State
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

// Dữ liệu sidebar
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
  if (name.includes('anh') || name.includes('vương quốc anh') || name.includes('uk')) return 'https://flagcdn.com/w40/gb.png';
  if (name.includes('pháp')) return 'https://flagcdn.com/w40/fr.png';
  if (name.includes('đức')) return 'https://flagcdn.com/w40/de.png';
  if (name.includes('nhật')) return 'https://flagcdn.com/w40/jp.png';
  return 'https://flagcdn.com/w40/un.png';
};

// Load sản phẩm
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
    const data = res.data || res;
    if (data) {
      danhSachSanPham.value = data.items || [];
      tongSanPham.value     = data.total || 0;
      tongTrang.value       = Math.ceil(tongSanPham.value / soLuongMoiTrang);
    }
  } catch (err) {
    console.error('Lỗi tải sản phẩm:', err);
    danhSachSanPham.value = [];
    tongSanPham.value = 0;
  } finally {
    dangTai.value = false;
  }
};

// Load Sidebar (Fix lỗi nhận diện mảng)
const loadSidebar = async () => {
  try {
    const [resDM, resNSX] = await Promise.all([
      axiosClient.get('/DanhMuc'),
      axiosClient.get('/Thuoc/nha-san-xuat'),
    ]);
    danhSachDanhMuc.value = resDM.data || resDM || [];
    danhSachNSX.value     = resNSX.data || resNSX || [];
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
  try {
    await axiosClient.post('/GioHang/them', {
      MaThuoc: sp.maThuoc,
      MaDvt: sp.maDVT || 1, 
      SoLuong: 1,
    });
    Swal.fire({
      icon: 'success',
      title: 'Đã thêm!',
      text: `${sp.tenThuoc} đã vào giỏ hàng.`,
      confirmButtonText: 'Xem giỏ hàng',
      showCancelButton: true,
      cancelButtonText: 'Tiếp tục',
    }).then((result) => {
      if (result.isConfirmed) router.push('/gio-hang');
    });
  } catch (err) {
    if (err.response?.status === 401) router.push('/dang-nhap'); 
    else Swal.fire({ icon: 'error', title: 'Lỗi', text: 'Không thể thêm vào giỏ.' });
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

watch([locDanhMuc, locDoiTuong, locNSX, locGia, locDBC], () => loadData(true), { deep: true });

watch(() => route.query.q, (newVal) => {
  tuKhoa.value = newVal || '';
  loadData(true);
}, { immediate: true });

const getImageUrl = (path) => {
  if (!path) return 'https://via.placeholder.com/300x300.png?text=No+Image';
  if (path.startsWith('http')) return path;
  return `https://localhost:7070${path.startsWith('/') ? '' : '/'}${path}`;
};

const formatGia = (value) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value ?? 0);

onMounted(() => {
  loadData();
  loadSidebar();
});
</script>

<style scoped>
.product-card {
  position: relative !important;
  background: #fff;
  border-radius: 12px;
  overflow: hidden;
  transition: all 0.3s ease;
  border: 1px solid #f0f0f0;
  padding: 15px;
  display: flex;
  flex-direction: column;
  height: 100%;
}

.product-card:hover {
  box-shadow: 0 10px 20px rgba(0,0,0,0.08);
}

/* Badge Quốc gia & Thuốc kê đơn nằm gọn ở 2 góc */
.product-origin-badge {
  position: absolute;
  top: 10px;
  left: 10px;
  z-index: 10;
  background: rgba(255, 255, 255, 0.95);
  padding: 2px 8px;
  border-radius: 20px;
  display: flex;
  align-items: center;
  font-size: 10px;
  border: 1px solid #eee;
}

.product-badge-prescription {
  /* QUAN TRỌNG: 2 dòng này sẽ làm khung ngắn lại theo chữ */
  display: inline-block; 
  width: fit-content; 

  /* Đưa nó lên góc phải để không đẩy ảnh xuống */
  position: absolute;
  top: 10px;
  right: 10px;
  z-index: 10;

  /* Giữ nguyên style màu sắc của Long */
  font-size: 10px;
  background: #fff1f0;
  padding: 2px 8px;
  border-radius: 20px; /* Bo tròn giống nhãn quốc gia */
  border: 1px solid #ffccc7;
  color: #cf1322;
  font-weight: 500;
  white-space: nowrap; /* Tuyệt đối không cho xuống dòng */
}

.flag-icon {
  width: 16px !important;
  height: 10px !important;
  margin-right: 4px;
}

.product-image {
  width: 100%;
  height: 160px;
  object-fit: contain;
  margin: 15px 0;
}

.product-category-label {
  font-size: 11px;
  color: #1890ff;
  margin-bottom: 4px;
  font-weight: 500;
}

.product-actions {
  display: flex;
  gap: 6px;
  margin-top: auto;
}

.product-actions .btn {
  flex: 1;
  font-size: 11px;
  font-weight: 600;
  text-transform: uppercase;
  padding: 8px 2px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.btn-primary:disabled {
  background-color: #f5f5f5 !important;
  border-color: #d9d9d9 !important;
  color: #bfbfbf !important;
  cursor: not-allowed;
}
</style>
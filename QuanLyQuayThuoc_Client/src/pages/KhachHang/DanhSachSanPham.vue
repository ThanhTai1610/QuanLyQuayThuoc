<template>
  <div class="site-wrap">
    <div class="container py-4">

      <!-- Header -->
      <div class="d-flex align-items-center justify-content-between mb-3">
        <router-link to="/" class="btn btn-link p-0">← Về trang chủ</router-link>
        <div class="products-page-title">Danh sách sản phẩm nhà thuốc</div>
      </div>

      <div class="row">

        <!-- ── Sidebar bộ lọc ── -->
        <div class="col-lg-3 mb-4">
          <aside class="filter-sidebar">
            <h2 class="filter-title">Bộ lọc thông minh</h2>

            <!-- Lọc theo DanhMuc -->
            <div class="filter-group">
              <div class="filter-label">Danh mục</div>
              <label class="filter-option" v-for="dm in danhSachDanhMuc" :key="dm.maDanhMuc">
                <input type="checkbox" :value="dm.maDanhMuc" v-model="locDanhMuc" />
                {{ dm.tenDanhMuc }}
              </label>
            </div>

            <!-- Lọc theo DoiTuongSuDung trong bảng Thuoc -->
            <div class="filter-group">
              <div class="filter-label">Đối tượng</div>
              <label class="filter-option" v-for="dt in danhSachDoiTuong" :key="dt">
                <input type="checkbox" :value="dt" v-model="locDoiTuong" />
                {{ dt }}
              </label>
            </div>

            <!-- Lọc theo NhaSanXuat trong bảng Thuoc -->
            <div class="filter-group">
              <div class="filter-label">Nhà sản xuất</div>
              <label class="filter-option" v-for="nsx in danhSachNSX" :key="nsx">
                <input type="checkbox" :value="nsx" v-model="locNSX" />
                {{ nsx }}
              </label>
            </div>

            <!-- Lọc theo GiaBan trong DonViTinh -->
            <div class="filter-group">
              <div class="filter-label">Khoảng giá</div>
              <label class="filter-option" v-for="g in danhSachGia" :key="g.value">
                <input type="radio" name="gia" :value="g.value" v-model="locGia" />
                {{ g.label }}
              </label>
            </div>

            <!-- Lọc theo DangBaoChe trong bảng Thuoc -->
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

        <!-- ── Danh sách sản phẩm ── -->
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

          <!-- Loading -->
          <div v-if="dangTai" class="text-center py-5">
            <div class="spinner-border text-primary" role="status">
              <span class="sr-only">Đang tải...</span>
            </div>
          </div>

          <div v-else>
            <!-- Lưới sản phẩm -->
            <div class="row" v-if="danhSachSanPham.length > 0">
              <div class="col-md-4 col-sm-6 mb-4" v-for="sp in danhSachSanPham" :key="sp.maThuoc">
                <article class="product-card">
                  <!-- Badge thuốc kê đơn — LaThuocKeDon trong bảng Thuoc -->
                  <div v-if="sp.laThuocKeDon" class="product-badge-prescription">🔴 Thuốc kê đơn</div>

                  <img :src="getImageUrl(sp.hinhAnhChinh)" :alt="sp.tenThuoc" class="product-image" />

                  <h3 class="product-name">{{ sp.tenThuoc }}</h3>

                  <!-- QuyCach + NuocSanXuat từ bảng Thuoc -->
                  <p class="product-meta">{{ sp.quyCach }} • {{ sp.nuocSanXuat }}</p>

                  <!-- GiaBan từ DonViTinh (đơn vị cơ bản) -->
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

            <!-- Phân trang -->
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

const route  = useRoute();
const router = useRouter();

// ── State ──
const danhSachSanPham = ref([]);
const dangTai         = ref(false);
const tongSanPham     = ref(0);
const trangHienTai    = ref(1);
const tongTrang       = ref(1);
const soTrangMoiTrang = 12;

// Lấy từ query param (tìm kiếm từ header)
const tuKhoa = ref(route.query.q || '');
const sapXep = ref('ban-chay');

// Bộ lọc — ánh xạ với các cột trong bảng Thuoc + DonViTinh
const locDanhMuc  = ref([]);   // MaDanhMuc
const locDoiTuong = ref([]);   // DoiTuongSuDung
const locNSX      = ref([]);   // NhaSanXuat
const locGia      = ref('');   // khoảng GiaBan
const locDBC      = ref([]);   // DangBaoChe

// ── Dữ liệu cho sidebar (load từ API) ──
const danhSachDanhMuc = ref([]);
const danhSachNSX     = ref([]);

const danhSachDoiTuong = ['Trẻ em', 'Người lớn', 'Phụ nữ mang thai', 'Người già'];
const danhSachDBC      = ['Viên nén', 'Viên nang', 'Siro', 'Bột pha'];
const danhSachGia = [
  { label: 'Dưới 100.000đ',           value: '0-100000'         },
  { label: '100.000đ - 500.000đ',      value: '100000-500000'    },
  { label: '500.000đ - 1.000.000đ',    value: '500000-1000000'   },
  { label: 'Trên 1.000.000đ',          value: '1000000-99999999' },
];

// ── Load sản phẩm ──
// GET /Thuoc?trang=1&soLuong=12&danhMuc=...&nsx=...&gia=...&dbc=...&q=...&sapXep=...
const loadData = async (resetTrang = false) => {
  if (resetTrang) trangHienTai.value = 1;
  dangTai.value = true;
  try {
    const params = {
      trang:     trangHienTai.value,
      soLuong:   soTrangMoiTrang,
      sapXep:    sapXep.value,
      q:         tuKhoa.value || undefined,
      danhMuc:   locDanhMuc.value.join(',')  || undefined,
      doiTuong:  locDoiTuong.value.join(',') || undefined,
      nsx:       locNSX.value.join(',')      || undefined,
      gia:       locGia.value                || undefined,
      dangBaoChe:locDBC.value.join(',')      || undefined,
    };
    const res = await axiosClient.get('/Thuoc', { params });
    danhSachSanPham.value = res.data.items;
    tongSanPham.value     = res.data.total;
    tongTrang.value       = Math.ceil(res.data.total / soTrangMoiTrang);
  } catch (err) {
    console.error('Lỗi tải sản phẩm:', err);
  } finally {
    dangTai.value = false;
  }
};

// ── Load sidebar ──
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

// ── Đổi trang ──
const doiTrang = (trang) => {
  if (trang < 1 || trang > tongTrang.value) return;
  trangHienTai.value = trang;
  loadData();
  window.scrollTo({ top: 0, behavior: 'smooth' });
};

// ── Thêm vào giỏ — POST /GioHang ──
const themVaoGio = async (sp) => {
  try {
    await axiosClient.post('/GioHang', {
      maThuoc: sp.maThuoc,
      maDVT:   sp.maDVT,
      soLuong: 1,
    });
    router.push('/gio-hang');
  } catch (err) {
    console.error('Lỗi thêm giỏ hàng:', err);
  }
};

// ── Xóa bộ lọc ──
const xoaBoLoc = () => {
  locDanhMuc.value  = [];
  locDoiTuong.value = [];
  locNSX.value      = [];
  locGia.value      = '';
  locDBC.value      = [];
  loadData(true);
};

// ── Watch bộ lọc → tải lại ──
watch([locDanhMuc, locDoiTuong, locNSX, locGia, locDBC], () => loadData(true), { deep: true });

// ── Watch query param từ thanh tìm kiếm ──
watch(() => route.query.q, (val) => {
  tuKhoa.value = val || '';
  loadData(true);
});

const getImageUrl = (path) => {
  if (!path) return '/images/no-image.png';
  if (path.startsWith('http')) return path;
  return `https://localhost:7070${path}`;
};

const formatGia = (value) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value ?? 0);

onMounted(() => {
  loadSidebar();
  loadData();
});
</script>
<template>
  <div class="site-wrap">
    <div class="container py-4">
      <nav class="breadcrumb-wrap" aria-label="breadcrumb">
        <router-link to="/">Trang chủ</router-link> <span>></span>
        <router-link to="/san-pham">Thuốc</router-link> <span>></span>
        <span class="text-muted">{{ thuoc.tenThuoc }}</span>
      </nav>

      <section class="detail-top-card">
        <div class="row">
          <div class="col-lg-5 mb-3 mb-lg-0">
            <div class="image-viewer">
              <div v-if="thuoc.laThuocKeDon" class="prescription-label">
                🔴 Thuốc kê đơn
              </div>
              <img :src="getImageUrl(anhHienTai)" alt="Ảnh thuốc chính" class="main-image" />
            </div>
            
            <div class="thumb-list">
              <button
                v-for="(img, index) in danhSachAnh"
                :key="index"
                class="thumb-item"
                :class="{ active: anhHienTai === img }"
                @click="anhHienTai = img"
              >
                <img :src="getImageUrl(img)" alt="Ảnh nhỏ" />
              </button>
            </div>
          </div>

          <div class="col-lg-7">
            <h1 class="drug-name">{{ thuoc.tenThuoc }}</h1>
            <div class="drug-meta">
              <span><strong>Thương hiệu:</strong> {{ thuoc.nhaSanXuat }}</span>
              <span>|</span>
              <span><strong>Nguồn gốc:</strong> {{ thuoc.nuocSanXuat }}</span>
            </div>
            <div class="drug-code">
              <span><strong>Mã thuốc:</strong> {{ thuoc.maThuoc }}</span>
              <span>|</span>
              <span><strong>Số đăng ký:</strong> {{ thuoc.soDangKy }}</span>
            </div>

            <div class="unit-row mt-3">
              <label class="d-block mb-2"><strong>Chọn đơn vị tính:</strong></label>
              <div class="unit-options d-flex flex-wrap">
                <div 
                  v-for="(dv, index) in thuoc.donViTinhs" 
                  :key="index"
                  class="unit-item"
                  :class="{ active: selectedUnitIndex === index }"
                  @click="selectedUnitIndex = index"
                >
                  {{ dv.tenDonVi }}
                </div>
              </div>
            </div>

            <div class="price-row custom-price-layout">
              <div class="main-unit-price-wrapper d-flex align-items-end">
                <div class="gia-ban text-danger font-weight-bold" style="font-size: 28px; line-height: 1;">
                  {{ formatTien(thuoc.donViTinhs[selectedUnitIndex]?.giaBan || 0) }}
                </div>
                
                <div class="gia-note text-muted small ml-1 pb-1">
                  / {{ thuoc.donViTinhs[selectedUnitIndex]?.tenDonVi }}
                </div>
              </div>
              
              <div class="total-price-wrapper mt-2 d-block">
                <div class="font-weight-bold text-dark" style="font-size: 16px;">
                  Thành tiền: {{ formatTien((thuoc.donViTinhs[selectedUnitIndex]?.giaBan || 0) * soLuong) }}
                </div>
              </div>
            </div>

            <div class="stock-row mt-3">
              Trạng thái kho:
              <span :class="['stock-badge', tongTonKho > 0 ? 'stock-available' : 'stock-empty']">
                {{ tongTonKho > 0 ? 'Còn hàng' : 'Tạm hết hàng' }}
              </span>
              <span class="stock-count-text ml-3" v-if="tongTonKho > 0">
                (Số lượng còn lại trong kho: <strong>{{ tongTonKho }}</strong>)
              </span>
            </div>

            <div v-if="thuoc.laThuocKeDon" class="mt-2">
              <small class="text-danger"><i>* Vui lòng mang theo toa của bác sĩ khi mua loại thuốc này.</i></small>
            </div>

            <div class="quantity-wrapper mt-4 d-flex align-items-center">
              <label class="mr-3 mb-0"><strong>Chọn số lượng:</strong></label>
              <div class="quantity-controls d-flex align-items-center">
                <button @click="giamSoLuong" class="qty-btn" :disabled="soLuong <= 1">-</button>
                
                <input 
                  type="number" 
                  v-model.number="soLuong" 
                  @input="kiemTraNhapTay"
                  class="qty-input" 
                  min="1" 
                />
                
                <button @click="tangSoLuong" class="qty-btn" :disabled="soLuong >= tongTonKho">+</button>
              </div>
            </div>

            <div class="action-row mt-4">
              <template v-if="thuoc.laThuocKeDon">
                <div class="d-flex gap-3">
                  <button @click="diToiTuVan" class="btn btn-danger btn-lg flex-grow-1 py-3 font-weight-bold">
                    Tư vấn ngay
                  </button>
                  
                  <button @click="guiDonThuoc" class="btn btn-light btn-lg flex-grow-1 py-3 font-weight-bold border">
    Gửi đơn thuốc
</button>
                </div>
              </template>

              <template v-else>
                <div class="d-flex gap-3">
                  <button @click="themGioHang" class="btn btn-primary btn-lg flex-grow-1 py-3 font-weight-bold">
                    THÊM VÀO GIỎ HÀNG
                  </button>
                  <button @click="muaNgay" class="btn btn-success btn-lg flex-grow-1 py-3 font-weight-bold">
                    MUA NGAY
                  </button>
                </div>
              </template>
            </div>
          </div>
        </div>
      </section>

      <section class="tabs-card mt-4">
        <div class="tabs-header">
          <button @click="activeTab = 'dacdiem'" :class="['tab-btn', { active: activeTab === 'dacdiem' }]">
            Đặc điểm nổi bật
          </button>
          <button @click="activeTab = 'chitiet'" :class="['tab-btn', { active: activeTab === 'chitiet' }]">
            Thông tin chi tiết
          </button>
          <button @click="activeTab = 'luuy'" :class="['tab-btn', { active: activeTab === 'luuy' }]">
            Lưu ý & Bảo quản
          </button>
        </div>

        <div v-if="activeTab === 'dacdiem'" class="tab-content active">
          <p>{{ thuoc.moTaNgan }}</p>
          <div class="spec-table">
            <div><strong>Quy cách:</strong> {{ thuoc.quyCach }}</div>
            <div><strong>Dạng bào chế:</strong> {{ thuoc.dangBaoChe }}</div>
            <div><strong>Hạn dùng:</strong> {{ thuoc.hanSuDungThang }} tháng</div>
            <div><strong>Thuốc kê đơn:</strong> {{ thuoc.laThuocKeDon ? 'Có' : 'Không' }}</div>
          </div>
        </div>

        <div v-if="activeTab === 'chitiet'" class="tab-content active">
          <h3>Thành phần</h3>
          <p>{{ thuoc.thanhPhan }}</p>
          <h3>Công dụng</h3>
          <p>{{ thuoc.congDung }}</p>
          <h3>Liều dùng - Cách dùng</h3>
          <p>{{ thuoc.cachDung }}</p>
          <h3>Đối tượng sử dụng</h3>
          <p>{{ thuoc.doiTuongSuDung }}</p>
        </div>

        <div v-if="activeTab === 'luuy'" class="tab-content active">
          <h3>Chống chỉ định</h3>
          <p>{{ thuoc.chongChiDinh }}</p>
          <h3>Tác dụng phụ</h3>
          <p>{{ thuoc.tacDungPhu }}</p>
          <h3>Thận trọng / Lưu ý</h3>
          <p>{{ thuoc.luuY }}</p>
          <h3>Bảo quản</h3>
          <p>{{ thuoc.baoQuan }}</p>
        </div>
      </section>

      <section class="related-section mt-5">
        <div class="section-header d-flex justify-content-between align-items-center mb-3">
          <h2 class="section-title">Sản phẩm tương tự</h2>
          <router-link to="/san-pham" class="view-all">Xem tất cả <i class="fas fa-chevron-right ml-1"></i></router-link>
        </div>
        <div class="related-slider pb-3">
          <router-link
            v-for="item in dsSanPhamTuongTu"
            :key="item.maThuoc"
            :to="{ name: 'ChiTietSanPham', params: { id: item.maThuoc } }"
            class="product-mini-card"
          >
            <div class="card-img-wrap">
              <img :src="getImageUrl(item.hinhAnhChinh)" :alt="item.tenThuoc" />
            </div>
            <div class="card-info">
              <h3 class="card-name">{{ item.tenThuoc }}</h3>
              <div class="card-price">{{ formatTien(item.giaBan) }}</div>
            </div>
          </router-link>
        </div>
      </section>

      <section class="related-section mt-4">
        <div class="section-header mb-3">
          <h2 class="section-title">Sản phẩm thường mua cùng</h2>
        </div>
        <div class="related-slider pb-3">
          <div 
            v-for="item in dsThuongMuaCung" 
            :key="item.id" 
            class="product-mini-card has-action"
          >
            <router-link :to="{ name: 'ChiTietSanPham', params: { id: item.id } }" class="card-link">
              <div class="card-img-wrap">
                <img :src="getImageUrl(item.image)" :alt="item.name" />
              </div>
              <div class="card-info">
                <h3 class="card-name">{{ item.name }}</h3>
                <div class="card-price">
                  {{ formatTien(item.price) }} <span class="card-unit">/ {{ item.unit }}</span>
                </div>
              </div>
            </router-link>
            <div class="card-action">
              <button class="btn-quick-add" @click="themNhanhVaoGio(item)">
                <i class="fas fa-cart-plus mr-1"></i> Thêm vào giỏ
              </button>
            </div>
          </div>
          <div v-if="dsThuongMuaCung.length === 0" class="empty-suggestion p-5 text-center w-100">
            <div class="spinner-grow text-primary spinner-grow-sm mr-2"></div>
            Đang tìm kiếm gợi ý phù hợp cho bạn...
          </div>
        </div>
      </section>
    </div>
  </div>
  <GuiDonThuocModal 
  :isOpen="showModalDonThuoc" 
  :product="thuoc" 
  @close="showModalDonThuoc = false" 
/>
</template>

<script setup>
import '../../assets/css/product-detail-page.css';
import { ref, onMounted, watch, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import axiosClient from '../../api/axiosClient'; 
import Swal from 'sweetalert2';
import bus from '../../api/bus';
import GuiDonThuocModal from './GuiDonThuocModal.vue';
const showModalDonThuoc = ref(false);

const guiDonThuoc = () => {
  showModalDonThuoc.value = true;
};
const route = useRoute();
const router = useRouter();

// --- STATE ---
const thuoc = ref({
  tenThuoc: '', nhaSanXuat: '', nuocSanXuat: '', maThuoc: '', soDangKy: '',
  donViTinhs: [], loHangs: [], laThuocKeDon: false, moTaNgan: '', quyCach: '',
  dangBaoChe: '', hanSuDungThang: 0, thanhPhan: '', congDung: '', cachDung: '',
  doiTuongSuDung: '', chongChiDinh: '', tacDungPhu: '', luuY: '', baoQuan: '',
  maDanhMuc: null
});

const danhSachAnh = ref([]);
const anhHienTai = ref('');
const selectedUnitIndex = ref(0);
const activeTab = ref('dacdiem');
const dsSanPhamTuongTu = ref([]);
const dsThuongMuaCung = ref([]); 
const soLuong = ref(1);

const diToiTuVan = () => {
  bus.emit('open-chat', { tenThuoc: thuoc.value.tenThuoc });
};

// --- COMPUTED ---
const tongTonKho = computed(() => {
  const danhSachLo = thuoc.value.loHangs || thuoc.value.loHang; 
  if (!danhSachLo || !Array.isArray(danhSachLo)) return 0;
  return danhSachLo.reduce((sum, lo) => sum + (lo.soLuongTon || 0), 0);
});

// --- HELPERS ---
const formatTien = (so) => {
  if (so === undefined || so === null) return '0đ';
  return so.toLocaleString('vi-VN') + 'đ';
};

const getImageUrl = (path) => {
  if (!path) return 'https://via.placeholder.com/400x400.png?text=Duoc+Pham';
  if (path.startsWith('http')) return path;
  return `${import.meta.env.VITE_API_URL.replace('/api', '')}${path.startsWith('/') ? '' : '/'}${path}`;
};

// --- LOGIC TĂNG GIẢM SỐ LƯỢNG ---
const tangSoLuong = () => {
  if (soLuong.value < tongTonKho.value) {
    soLuong.value++;
  } else {
    Swal.fire('Thông báo', 'Số lượng trong kho đã đạt giới hạn!', 'info');
  }
};

const giamSoLuong = () => {
  if (soLuong.value > 1) {
    soLuong.value--;
  }
};

const kiemTraNhapTay = () => {
  if (!soLuong.value || soLuong.value < 1) soLuong.value = 1;
  if (soLuong.value > tongTonKho.value) {
    Swal.fire({ title: 'Vượt quá tồn kho', text: `Chỉ còn ${tongTonKho.value} sản phẩm`, icon: 'warning' });
    soLuong.value = tongTonKho.value;
  }
};

// --- CALL APIS ---
const loadProduct = async () => {
  const productId = route.params.id;
  try {
    const data = await axiosClient.get(`/ThuocKhachHang/${productId}`);
    thuoc.value = data;

    window.scrollTo({ top: 0, behavior: 'smooth' });

    const images = [];
    if (data.hinhAnhChinh) images.push(data.hinhAnhChinh);
    if (data.hinhAnhThuocs && data.hinhAnhThuocs.length > 0) {
      data.hinhAnhThuocs.forEach((img) => {
        const path = typeof img === 'string' ? img : img.duongDan;
        if (path) images.push(path);
      });
    }
    danhSachAnh.value = images;
    anhHienTai.value = data.hinhAnhChinh || images[0] || '';
    selectedUnitIndex.value = 0;

    // Gọi API Sản phẩm tương tự (Cùng danh mục)
    if (data.maDanhMuc) {
      loadRelatedProducts(data.maDanhMuc, productId);
      // Gọi API Sản phẩm thường mua cùng (Theo mapping danh mục cố định)
      loadFrequentlyBoughtProducts(data.maDanhMuc, productId);
    }
  } catch (error) {
    console.error('Không thể tải dữ liệu thuốc:', error);
  }
};

const loadRelatedProducts = async (maDanhMuc, currentProductId) => {
  try {
    const data = await axiosClient.get(`/ThuocKhachHang/Related`, {
      params: { maDanhMuc: Number(maDanhMuc), currentProductId: Number(currentProductId) }
    });
    dsSanPhamTuongTu.value = data;
  } catch (error) {
    console.error('Lỗi khi tải thuốc tương tự:', error);
  }
};

const loadFrequentlyBoughtProducts = async (maDanhMuc, currentProductId) => {
  try {
    // Gọi API mới làm ở Backend với 2 tham số: maDanhMuc hiện tại và productId hiện tại
    const data = await axiosClient.get(`/ThuocKhachHang/FrequentlyBoughtWith/${maDanhMuc}/${currentProductId}`);
    dsThuongMuaCung.value = data;
  } catch (error) {
    console.error('Lỗi khi tải thuốc thường mua cùng:', error);
  }
};

const themGioHang = async () => {
  if (!localStorage.getItem('token')) {
    Swal.fire({ title: 'Thông báo', text: 'Vui lòng đăng nhập!', icon: 'warning', confirmButtonText: 'Đăng nhập' })
      .then((r) => { if (r.isConfirmed) router.push('/auth/dang-nhap'); });
    return;
  }

  const activeUnit = thuoc.value.donViTinhs[selectedUnitIndex.value];
  if (!activeUnit) return;
  const payload = { MaThuoc: thuoc.value.maThuoc, MaDvt: activeUnit.maDvt, SoLuong: soLuong.value };

  try {
    await axiosClient.post('/GioHang/them', payload);
    Swal.fire({
      icon: 'success',
      title: 'Đã thêm!',
      text: `${thuoc.value.tenThuoc} đã vào giỏ hàng.`,
      confirmButtonText: 'Xem giỏ hàng',
      showCancelButton: true,
      cancelButtonText: 'Tiếp tục',
    }).then((result) => {
      if (result.isConfirmed) router.push('/gio-hang');
    });
  } catch (e) {
    Swal.fire('Thất bại', 'Không thể thêm sản phẩm.', 'error');
  }
};

const muaNgay = async () => {
  if (!localStorage.getItem('token')) {
    Swal.fire({ title: 'Thông báo', text: 'Vui lòng đăng nhập!', icon: 'warning', confirmButtonText: 'Đăng nhập' })
      .then((r) => { if (r.isConfirmed) router.push('/auth/dang-nhap'); });
    return;
  }

  const activeUnit = thuoc.value.donViTinhs[selectedUnitIndex.value];
  if (!activeUnit) return;

  const payload = { MaThuoc: thuoc.value.maThuoc, MaDvt: activeUnit.maDvt, SoLuong: soLuong.value };

  try {
    await axiosClient.post('/GioHang/them', payload);
    router.push('/gio-hang');
  } catch (e) {
    Swal.fire('Thất bại', 'Không thể mua ngay lúc này.', 'error');
  }
};

const themNhanhVaoGio = async (item) => {
    if (!localStorage.getItem('token')) {
        Swal.fire({ title: 'Thông báo', text: 'Vui lòng đăng nhập!', icon: 'warning', confirmButtonText: 'Đăng nhập' })
            .then((r) => { if (r.isConfirmed) router.push('/auth/dang-nhap'); });
        return;
    }
    try {
        const payload = { 
            MaThuoc: item.id || item.maThuoc, 
            MaDvt: item.maDvt || 1, // Fallback về 1 nếu backend vẫn trả về 0
            SoLuong: 1 
        };
        console.log("Payload thêm nhanh:", payload);
        await axiosClient.post('/GioHang/them', payload);
        
        Swal.fire({
            icon: 'success',
            title: 'Đã thêm!',
            text: `${item.name} đã vào giỏ hàng.`,
            confirmButtonText: 'Xem giỏ hàng',
            showCancelButton: true,
            cancelButtonText: 'Tiếp tục',
        }).then((result) => {
            if (result.isConfirmed) router.push('/gio-hang');
        });
    } catch (e) {
        console.error(e);
        Swal.fire('Lỗi', 'Không thể thêm sản phẩm nhanh vào giỏ.', 'error');
    }
}

onMounted(loadProduct);

watch(() => route.params.id, (newId) => {
  if (newId) loadProduct();
});
</script>

<style scoped>
/* CSS RE-DESIGNED SECTIONS */
.related-section {
  background: transparent;
}
.section-title {
  font-size: 20px;
  font-weight: 700;
  color: #333;
  position: relative;
  padding-left: 15px;
}
.section-title::before {
  content: '';
  position: absolute;
  left: 0;
  top: 50%;
  transform: translateY(-50%);
  width: 4px;
  height: 24px;
  background: #007bff;
  border-radius: 4px;
}
.view-all {
  font-size: 14px;
  color: #007bff;
  font-weight: 500;
  text-decoration: none;
  transition: all 0.2s;
}
.view-all:hover {
  color: #0056b3;
  text-decoration: underline;
}

.related-slider {
  display: flex;
  overflow-x: auto;
  gap: 20px;
  padding: 10px 5px;
  scroll-behavior: smooth;
}
.related-slider::-webkit-scrollbar {
  height: 6px;
}
.related-slider::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 10px;
}
.related-slider::-webkit-scrollbar-thumb {
  background: #cbd5e0;
  border-radius: 10px;
}

.product-mini-card {
  min-width: 200px;
  max-width: 200px;
  background: #fff;
  border-radius: 16px;
  border: 1px solid #f0f0f0;
  overflow: hidden;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  text-decoration: none;
  color: inherit;
  display: flex;
  flex-direction: column;
}
.product-mini-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 12px 24px rgba(0, 0, 0, 0.1);
  border-color: #007bff33;
}
.product-mini-card.has-action {
  height: 100%;
}

.card-img-wrap {
  width: 100%;
  height: 160px;
  padding: 15px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #fff;
}
.card-img-wrap img {
  max-width: 100%;
  max-height: 100%;
  object-fit: contain;
  transition: transform 0.5s ease;
}
.product-mini-card:hover .card-img-wrap img {
  transform: scale(1.1);
}

.card-info {
  padding: 12px 15px;
  flex-grow: 1;
  display: flex;
  flex-direction: column;
}
.card-name {
  font-size: 14px;
  font-weight: 600;
  color: #333;
  margin-bottom: 8px;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  line-height: 1.4;
  height: 2.8em;
}
.card-price {
  font-size: 16px;
  font-weight: 700;
  color: #d9534f;
  margin-top: auto;
}
.card-unit {
  font-size: 12px;
  font-weight: 400;
  color: #718096;
}

.card-action {
  padding: 0 15px 15px;
}
.btn-quick-add {
  width: 100%;
  padding: 8px;
  border-radius: 8px;
  border: 1px solid #007bff;
  background: #fff;
  color: #007bff;
  font-size: 13px;
  font-weight: 600;
  transition: all 0.2s;
  cursor: pointer;
}
.btn-quick-add:hover {
  background: #007bff;
  color: #fff;
}

.empty-suggestion {
  background: #f8fafc;
  border-radius: 12px;
  color: #64748b;
  font-size: 14px;
}

.card-link {
  text-decoration: none;
  color: inherit;
  display: flex;
  flex-direction: column;
  flex-grow: 1;
}

/* GIỮ NGUYÊN CSS CŨ CỦA BẠN NẾU CẦN */
.image-viewer { position: relative; background-color: #f8f9fa; border-radius: 8px; overflow: hidden; border: 1px solid #eee; }
.prescription-label { position: absolute; top: 12px; left: 12px; z-index: 10; background-color: rgba(255, 255, 255, 0.9); color: #d9534f; padding: 5px 12px; border-radius: 4px; font-weight: bold; font-size: 13px; border: 1px solid #d9534f; }
.main-image { width: 100%; height: 400px; object-fit: contain; display: block; }
.unit-item { padding: 8px 25px; border: 1px solid #dee2e6; border-radius: 20px; cursor: pointer; transition: all 0.2s ease; }
.unit-item.active { border-color: #007bff; color: #007bff; background-color: #f0f7ff; }
.stock-available { background-color: #e6f9f0; color: #1a7f4e; }
.stock-empty { background-color: #ffebee; color: #c62828; }
</style>

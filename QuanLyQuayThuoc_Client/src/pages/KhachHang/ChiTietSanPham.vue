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

            <div class="unit-row">
              <label><strong>Chọn đơn vị tính:</strong></label>
              <select v-model="selectedUnitIndex" class="form-control form-control-sm w-50">
                <option v-for="(dv, index) in thuoc.donViTinhs" :key="index" :value="index">
                  {{ dv.tenDonVi }}
                </option>
              </select>
            </div>

            <div class="price-row">
              <div class="gia-ban">{{ formatTien(thuoc.donViTinhs[selectedUnitIndex]?.giaBan || 0) }}</div>
              <div class="gia-note">/ {{ thuoc.donViTinhs[selectedUnitIndex]?.tenDonVi }}</div>
            </div>

            <div class="stock-row">
              Trạng thái kho:
              <span :class="['stock-badge', tongTonKho > 0 ? 'stock-available' : 'stock-empty']">
                {{ tongTonKho > 0 ? 'Còn hàng' : 'Tạm hết hàng' }}
              </span>
            </div>

            <div v-if="thuoc.laThuocKeDon" class="mt-2">
              <span class="prescription-pill">🔴 Thuốc kê đơn - Vui lòng mang toa của bác sĩ</span>
            </div>

            <div class="quantity-wrapper mt-4 d-flex align-items-center">
            <label class="mr-3 mb-0"><strong>Chọn số lượng:</strong></label>
            <div class="quantity-controls d-flex align-items-center">
                <button @click="giamSoLuong" class="qty-btn" :disabled="soLuong <= 1">-</button>
                <input type="number" v-model.number="soLuong" class="qty-input" min="1" readonly />
                <button @click="tangSoLuong" class="qty-btn">+</button>
              </div>
            </div>

            <div class="action-row mt-4">
              <button @click="themGioHang" class="btn btn-primary btn-action mr-2">Thêm vào giỏ hàng</button>
              <button class="btn btn-success btn-action">Mua ngay</button>
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

      <section class="related-card mt-4">
        <h2>Sản phẩm tương tự</h2>
        <div class="related-slider d-flex overflow-auto">
          <router-link
            v-for="item in dsSanPhamTuongTu"
            :key="item.maThuoc"
            :to="{ name: 'ChiTietSanPham', params: { id: item.maThuoc } }"
            class="related-item p-3 text-center"
          >
            <img :src="getImageUrl(item.hinhAnhChinh)" style="width: 100px; height: 100px; object-fit: cover;" />
            <div class="mt-2">{{ item.tenThuoc }}</div>
          </router-link>
        </div>
      </section>

      <section class="related-card mt-4">
        <h2>Sản phẩm thường mua cùng</h2>
        <div class="related-slider d-flex overflow-auto">
          <router-link
            v-for="item in dsThuongMuaCung"
            :key="item.maThuoc"
            :to="{ name: 'ChiTietSanPham', params: { id: item.maThuoc } }"
            class="related-item p-3 text-center"
          >
            <img :src="getImageUrl(item.hinhAnhChinh)" style="width: 100px; height: 100px; object-fit: cover;" />
            <div class="mt-2">{{ item.tenThuoc }}</div>
          </router-link>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup>
import '../../assets/css/product-detail-page.css';
import { ref, onMounted, watch, computed } from 'vue';
import { useRoute } from 'vue-router';
// Import axiosClient đã cấu hình interceptor để dùng token và baseURL
import axiosClient from '../../api/axiosClient'; 

const route = useRoute();

// --- STATE ---
const thuoc = ref({
  tenThuoc: '',
  nhaSanXuat: '',
  nuocSanXuat: '',
  maThuoc: '',
  soDangKy: '',
  donViTinhs: [], 
  loHangs: [],
  laThuocKeDon: false,
  moTaNgan: '',
  quyCach: '',
  dangBaoChe: '',
  hanSuDungThang: 0,
  thanhPhan: '',
  congDung: '',
  cachDung: '',
  doiTuongSuDung: '',
  chongChiDinh: '',
  tacDungPhu: '',
  luuY: '',
  baoQuan: ''
});

const danhSachAnh = ref([]);
const anhHienTai = ref('');
const selectedUnitIndex = ref(0);
const activeTab = ref('dacdiem');
const dsSanPhamTuongTu = ref([]);
const dsThuongMuaCung = ref([]); 
const soLuong = ref(1); // Số lượng chọn mua

// --- COMPUTED ---
const tongTonKho = computed(() => {
  if (!thuoc.value.loHangs) return 0;
  return thuoc.value.loHangs.reduce((sum, lo) => sum + lo.soLuongTon, 0);
});

// --- HELPERS ---
const formatTien = (so) => {
  if (so === undefined || so === null) return '0đ';
  return so.toLocaleString('vi-VN') + 'đ';
};

const getImageUrl = (path) => {
  if (!path) return '/images/no-image.png';
  if (path.startsWith('http')) return path;
  return `https://localhost:7070${path}`;
};

// --- LOGIC TĂNG GIẢM SỐ LƯỢNG ---
const tangSoLuong = () => {
  if (soLuong.value < tongTonKho.value) {
    soLuong.value++;
  } else {
    alert("Số lượng đạt giới hạn tồn kho!");
  }
};

const giamSoLuong = () => {
  if (soLuong.value > 1) {
    soLuong.value--;
  }
};

// --- CALL APIS ---
const loadProduct = async () => {
  const productId = route.params.id;
  try {
    // Dùng axiosClient: không cần ghi lại baseURL https://localhost:7070/api
    const data = await axiosClient.get(`/ThuocKhachHang/${productId}`);
    
    thuoc.value = data;

    // Xử lý ảnh
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

    // Reset trạng thái
    selectedUnitIndex.value = 0;
    activeTab.value = 'dacdiem';
    soLuong.value = 1;

    if (data.maDanhMuc) {
      loadRelatedProducts(data.maDanhMuc, productId);
    }
    loadFrequentlyBoughtProducts(productId);

  } catch (error) {
    console.error('Không thể tải dữ liệu thuốc:', error);
  }
};

const loadRelatedProducts = async (maDanhMuc, currentProductId) => {
  try {
    const data = await axiosClient.get(`/ThuocKhachHang/Related`, {
      params: {
        maDanhMuc: Number(maDanhMuc),
        currentProductId: Number(currentProductId)
      }
    });
    dsSanPhamTuongTu.value = data;
  } catch (error) {
    console.error('Lỗi khi tải thuốc tương tự:', error);
  }
};

const loadFrequentlyBoughtProducts = async (currentProductId) => {
  try {
    const data = await axiosClient.get(`/ThuocKhachHang/FrequentlyBoughtWith`, {
      params: { currentProductId: Number(currentProductId) }
    });
    dsThuongMuaCung.value = data;
  } catch (error) {
    console.error('Lỗi khi tải thuốc thường mua cùng:', error);
  }
};

// HÀM DUY NHẤT ĐỂ THÊM VÀO GIỎ HÀNG
const themGioHang = async () => {
  const activeUnit = thuoc.value.donViTinhs[selectedUnitIndex.value];
  
  if (!activeUnit) {
    alert('Vui lòng chọn đơn vị tính hợp lệ!');
    return;
  }

  const payload = {
    maKhachHang: 1, // Bạn có thể lấy từ User Store/localStorage nếu có
    maThuoc: thuoc.value.maThuoc,
    maDvt: activeUnit.maDvt,
    soLuong: soLuong.value 
  };

  try {
    await axiosClient.post('/ThuocKhachHang/AddToCart', payload);
    alert('Thêm vào giỏ hàng thành công!');
  } catch (error) {
    console.error('Lỗi khi thêm giỏ hàng:', error);
    // Nếu lỗi 401, axiosClient sẽ tự chuyển hướng người dùng đi đăng nhập
  }
};

// --- LIFECYCLE ---
onMounted(loadProduct);

watch(() => route.params.id, (newId) => {
  if (newId) loadProduct();
});
</script>

<style scoped>
.site-wrap, 
.site-wrap h1, .site-wrap h2, .site-wrap h3, 
.site-wrap p, .site-wrap span, .site-wrap select, .site-wrap button {
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif !important;
  -webkit-font-smoothing: antialiased; 
  -moz-osx-font-smoothing: grayscale;
}


/* Tiêu đề mục (Thành phần, Công dụng...) */
.tab-content h3 {
  font-size: 1.15rem;
  font-weight: 600; 
  color: #111827; 
  margin-top: 1.5rem;
  margin-bottom: 0.5rem;
}

/* Nội dung chữ mô tả */
.tab-content p {
  font-size: 0.95rem;
  font-weight: 400;
  color: #4b5563; 
  line-height: 1.6; 
}


.related-card h2 {
  font-size: 1.25rem;
  font-weight: 600;
  color: #111827;
  margin-bottom: 1.25rem;
}

/* Tên sản phẩm nhỏ dưới hình */
.related-item .mt-2 {
  font-size: 0.9rem;
  font-weight: 500;
  color: #374151;
  line-height: 1.4;
}


.thumb-item.active { border: 2px solid #28a745; }
.tab-content { display: block; }
.prescription-pill { color: red; font-weight: bold; }
.related-item { min-width: 150px; text-decoration: none; color: black; }

.thumb-list { display: flex; gap: 8px; margin-top: 10px; }
.thumb-item { border: 1px solid #eee; background: #fff; padding: 4px; cursor: pointer; }
.thumb-item img { width: 60px; height: 60px; object-fit: cover; }
.main-image { width: 100%; border-radius: 8px; height: 350px; object-fit: contain; background-color: #f8f9fa; }


.tab-btn.active { 
  background-color: #0062cc; 
  color: #fff; 
}

.quantity-controls {
  border: 1px solid #ced4da;
  border-radius: 20px; /* Bo tròn giống hình mẫu */
  overflow: hidden;
  display: flex;
  width: fit-content;
}

.qty-btn {
  background: #fff;
  border: none;
  padding: 5px 15px;
  font-size: 1.2rem;
  cursor: pointer;
  transition: background 0.2s;
}

.qty-btn:hover:not(:disabled) {
  background: #f8f9fa;
}

.qty-input {
  width: 50px;
  text-align: center;
  border: none;
  border-left: 1px solid #eee;
  border-right: 1px solid #eee;
  font-weight: 600;
  outline: none;
}

/* Ẩn mũi tên mặc định của input number */
.qty-input::-webkit-inner-spin-button,
.qty-input::-webkit-outer-spin-button {
  -webkit-appearance: none;
  margin: 0;
}
</style>
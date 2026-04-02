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
                <span class="check-icon" v-if="selectedUnitIndex === index">
                  <i class="fa fa-check"></i> </span>
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
import { useRoute, useRouter } from 'vue-router';

// Import axiosClient đã cấu hình interceptor để dùng token và baseURL
import axiosClient from '../../api/axiosClient'; 
import { authState } from '../../api/auth'; // Kiểm tra đúng đường dẫn đến file auth.js của Tài
import Swal from 'sweetalert2';
const route = useRoute();
const router = useRouter();
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
  if (!path) return 'https://via.placeholder.com/400x400.png?text=Duoc+Pham';
  if (path.startsWith('http')) return path;
  return `https://localhost:7070${path.startsWith('/') ? '' : '/'}${path}`;
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

const isLoggedIn = computed(() => !!authState.user);
// HÀM DUY NHẤT ĐỂ THÊM VÀO GIỎ HÀNG
// 1. Đảm bảo chỉ có DUY NHẤT một khai báo này
const themGioHang = async () => {
  // 1. Kiểm tra đăng nhập (Nới lỏng điều kiện để test)
  if (!authState.user) {
    Swal.fire({
      title: 'Thông báo',
      text: 'Vui lòng đăng nhập để thực hiện chức năng này!',
      icon: 'warning',
      confirmButtonText: 'Đăng nhập ngay',
      showCancelButton: true,
      cancelButtonText: 'Để sau'
    }).then((result) => {
      if (result.isConfirmed) {
        router.push('/dang-nhap'); 
      }
    });
    return;
  }

  // 2. Lấy đơn vị tính đã chọn
  const activeUnit = thuoc.value.donViTinhs[selectedUnitIndex.value];
  if (!activeUnit) {
    Swal.fire('Lỗi', 'Vui lòng chọn đơn vị tính!', 'error');
    return;
  }

  // 3. Payload: PHẢI VIẾT HOA chữ cái đầu để khớp với ThemVaoGioDto ở Backend
  const payload = {
    MaThuoc: thuoc.value.maThuoc,
    MaDvt: activeUnit.maDvt,
    SoLuong: soLuong.value 
  };

  console.log("Dữ liệu gửi đi:", payload);

  try {
    // 4. Gọi API: Đảm bảo đường dẫn là 'GioHang/them' 
    // (Bỏ dấu / ở đầu nếu axiosClient đã có /api/ ở cuối baseURL)
    await axiosClient.post('GioHang/them', payload);
    
    Swal.fire({
      title: 'Thành công!',
      text: 'Sản phẩm đã được thêm vào giỏ hàng',
      icon: 'success',
      confirmButtonText: 'Xem giỏ hàng',
      showCancelButton: true,
      cancelButtonText: 'Ở lại'
    }).then((result) => {
      if (result.isConfirmed) {
        router.push('/gio-hang');
      }
    });
  } catch (error) {
    console.error('Lỗi khi thêm giỏ hàng:', error);
    // Nếu vẫn lỗi 404, hãy kiểm tra lại baseURL trong axiosClient có kết thúc bằng /api/ chưa
    Swal.fire('Thất bại', 'Không tìm thấy đường dẫn API (404) hoặc lỗi server.', 'error');
  }
};

// --- LIFECYCLE ---
onMounted(loadProduct);

watch(() => route.params.id, (newId) => {
  if (newId) loadProduct();
});
</script>
<style>
  .unit-options {
  gap: 10px;
}

.unit-item {
  position: relative;
  padding: 8px 25px;
  border: 1px solid #dee2e6;
  border-radius: 20px; /* Bo tròn giống hình mẫu */
  cursor: pointer;
  background-color: #fff;
  transition: all 0.2s ease;
  user-select: none;
  font-size: 14px;
  min-width: 80px;
  text-align: center;
}

.unit-item:hover {
  border-color: #007bff;
  color: #007bff;
}

/* Khi đơn vị được chọn */
.unit-item.active {
  border-color: #007bff;
  color: #007bff;
  background-color: #f0f7ff;
  font-weight: 500;
  overflow: hidden; /* Để cắt phần vát góc của dấu check */
}

/* Tạo hình tam giác xanh và dấu tích ở góc trên bên phải */
.unit-item.active::after {
  content: "";
  position: absolute;
  top: 0;
  right: 0;
  width: 0;
  height: 0;
  border-style: solid;
  border-width: 0 18px 18px 0;
  border-color: transparent #007bff transparent transparent;
}

/* Dấu check trắng đè lên tam giác xanh */
.unit-item.active::before {
  content: "✓";
  position: absolute;
  top: -1px;
  right: 2px;
  color: white;
  font-size: 10px;
  z-index: 1;
  font-weight: bold;
}
</style>
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

      <section class="related-card mt-4">
        <h2>Sản phẩm tương tự</h2>
        <div class="related-slider d-flex overflow-auto p-2" style="gap: 15px;">
          <router-link
            v-for="item in dsSanPhamTuongTu"
            :key="item.maThuoc"
            :to="{ name: 'ChiTietSanPham', params: { id: item.maThuoc } }"
            class="related-item p-3 text-center shadow-sm bg-white rounded"
            style="min-width: 180px; text-decoration: none; color: inherit;"
          >
            <img :src="getImageUrl(item.hinhAnhChinh)" style="width: 100px; height: 100px; object-fit: cover;" />
            <div class="mt-2 text-truncate font-weight-bold">{{ item.tenThuoc }}</div>
            <div class="text-danger small">{{ formatTien(item.giaBan) }}</div>
          </router-link>
        </div>
      </section>

      <section class="related-card mt-4">
        <h2>Sản phẩm thường mua cùng</h2>
        <div class="related-slider d-flex overflow-auto p-2" style="gap: 15px;">
          <div 
            v-for="item in dsThuongMuaCung" 
            :key="item.id" 
            class="related-item p-3 text-center shadow-sm bg-white rounded"
            style="min-width: 180px;"
          >
            <router-link :to="{ name: 'ChiTietSanPham', params: { id: item.id } }" style="text-decoration: none; color: inherit;">
                <img :src="getImageUrl(item.image)" style="width: 100px; height: 100px; object-fit: cover;" />
                <div class="mt-2 text-truncate font-weight-bold">{{ item.name }}</div>
                <div class="text-danger small">{{ formatTien(item.price) }} / {{ item.unit }}</div>
            </router-link>
            <button class="btn btn-sm btn-outline-primary mt-2 w-100" @click="themNhanhVaoGio(item)">Thêm nhanh</button>
          </div>
          <div v-if="dsThuongMuaCung.length === 0" class="p-4 text-muted w-100 text-center">
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
import { authState } from '../../api/auth'; 
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
  if (!authState.user) {
    Swal.fire({ title: 'Thông báo', text: 'Vui lòng đăng nhập!', icon: 'warning', confirmButtonText: 'Đăng nhập' })
      .then((r) => { if (r.isConfirmed) router.push('/dang-nhap'); });
    return;
  }

  const activeUnit = thuoc.value.donViTinhs[selectedUnitIndex.value];
  const payload = { MaThuoc: thuoc.value.maThuoc, MaDvt: activeUnit.maDvt, SoLuong: soLuong.value };

  try {
    await axiosClient.post('GioHang/them', payload);
    Swal.fire({ title: 'Thành công!', icon: 'success', showCancelButton: true, confirmButtonText: 'Xem giỏ' })
      .then((r) => { if (r.isConfirmed) router.push('/gio-hang'); });
  } catch (e) {
    Swal.fire('Thất bại', 'Không thể thêm sản phẩm.', 'error');
  }
};

const themNhanhVaoGio = async (item) => {
    // Logic thêm nhanh dành cho gợi ý "Mua cùng"
    if (!authState.user) {
        Swal.fire('Thông báo', 'Bạn cần đăng nhập để mua hàng', 'warning');
        return;
    }
    try {
        // Tự động tìm đơn vị cơ bản của sản phẩm gợi ý để thêm vào giỏ
        // Chú ý: Backend cần trả về thêm MaDvt của đơn vị cơ bản trong API FrequentlyBoughtWith
        // Nếu chưa có, bạn có thể hướng người dùng vào xem Chi Tiết.
        router.push(`/thuoc/${item.id}`);
    } catch (e) {
        console.error(e);
    }
}

onMounted(loadProduct);

watch(() => route.params.id, (newId) => {
  if (newId) loadProduct();
});
</script>

<style scoped>
/* GIỮ NGUYÊN CSS CỦA BẠN */
.image-viewer { position: relative; background-color: #f8f9fa; border-radius: 8px; overflow: hidden; border: 1px solid #eee; }
.prescription-label { position: absolute; top: 12px; left: 12px; z-index: 10; background-color: rgba(255, 255, 255, 0.9); color: #d9534f; padding: 5px 12px; border-radius: 4px; font-weight: bold; font-size: 13px; border: 1px solid #d9534f; }
.main-image { width: 100%; height: 400px; object-fit: contain; display: block; }
.unit-item { padding: 8px 25px; border: 1px solid #dee2e6; border-radius: 20px; cursor: pointer; transition: all 0.2s ease; }
.unit-item.active { border-color: #007bff; color: #007bff; background-color: #f0f7ff; }
.stock-available { background-color: #e6f9f0; color: #1a7f4e; }
.stock-empty { background-color: #ffebee; color: #c62828; }
.related-slider::-webkit-scrollbar { height: 6px; }
.related-slider::-webkit-scrollbar-thumb { background: #ccc; border-radius: 10px; }
</style>
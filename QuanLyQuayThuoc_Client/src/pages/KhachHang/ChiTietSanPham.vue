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
              <img :src="anhHienTai" alt="Ảnh thuốc chính" class="main-image" />
            </div>
            <div class="thumb-list">
              <button
                v-for="(img, index) in thuoc.hinhAnh"
                :key="index"
                class="thumb-item"
                :class="{ active: anhHienTai === img }"
                @click="anhHienTai = img"
              >
                <img :src="img" alt="Ảnh nhỏ" />
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
                <option v-for="(dv, index) in thuoc.donViTinh" :key="index" :value="index">
                  {{ dv.ten }}
                </option>
              </select>
            </div>

            <div class="price-row">
              <div class="gia-ban">{{ formatTien(thuoc.donViTinh[selectedUnitIndex]?.giaBan || 0) }}</div>
              <div class="gia-note">/ {{ thuoc.donViTinh[selectedUnitIndex]?.ten }}</div>
            </div>

            <div class="stock-row">
              Trạng thái kho:
              <span :class="['stock-badge', thuoc.soLuongTon > 0 ? 'stock-available' : 'stock-empty']">
                {{ thuoc.soLuongTon > 0 ? 'Còn hàng' : 'Tạm hết hàng' }}
              </span>
            </div>

            <div v-if="thuoc.laThuocKeDon" class="mt-2">
              <span class="prescription-pill">🔴 Thuốc kê đơn - Vui lòng mang toa của bác sĩ</span>
            </div>

            <div class="action-row mt-4">
              <button class="btn btn-primary btn-action mr-2">Thêm vào giỏ hàng</button>
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
            :key="item.id"
            :to="{ name: 'ChiTietSanPham', params: { id: item.id } }"
            class="related-item p-3 text-center"
          >
            <img :src="item.image" style="width: 100px" />
            <div class="mt-2">{{ item.name }}</div>
          </router-link>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup>
import '../../assets/css/product-detail-page.css';
import { ref, onMounted, watch } from 'vue';
import { useRoute } from 'vue-router';

const duLieuThuoc = {
  omega3: {
    tenThuoc: 'Omega 3 Orihiro hỗ trợ tim mạch và não bộ',
    nhaSanXuat: 'Orihiro',
    nuocSanXuat: 'Nhật Bản',
    maThuoc: 'ORI-001',
    soDangKy: 'VN-10234-21',
    hinhAnh: ['/src/assets/images/product_01.png', '/src/assets/images/product_04.png'],
    donViTinh: [
      { ten: 'Hộp', giaBan: 736000 },
      { ten: 'Vỉ', giaBan: 98000 }
    ],
    soLuongTon: 55,
    laThuocKeDon: false,
    moTaNgan: 'Sản phẩm bổ sung Omega 3 tinh khiết giúp hỗ trợ não bộ...',
    quyCach: 'Hộp 180 viên',
    dangBaoChe: 'Viên nang mềm',
    hanSuDungThang: 24,
    thanhPhan: 'Omega-3 (EPA, DHA), gelatin...',
    congDung: 'Hỗ trợ tăng cường trí nhớ...',
    cachDung: 'Uống 1-2 viên/ngày...',
    doiTuongSuDung: 'Người trưởng thành...',
    chongChiDinh: 'Không dùng cho người mẫn cảm...',
    tacDungPhu: 'Hiếm gặp...',
    luuY: 'Không dùng quá liều...',
    baoQuan: 'Bảo quản nơi khô ráo...'
  },
  jointcare: {
    tenThuoc: 'Viên uống Joint Care hỗ trợ giảm đau nhức xương khớp',
    nhaSanXuat: 'Joint Pharma',
    nuocSanXuat: 'Nhật Bản',
    maThuoc: 'JNT-002',
    soDangKy: 'VN-19453-23',
    hinhAnh: ['/src/assets/images/product_02.png', '/src/assets/images/product_04.png'],
    donViTinh: [
      { ten: 'Hộp', giaBan: 561000 },
      { ten: 'Vỉ', giaBan: 79000 }
    ],
    soLuongTon: 22,
    laThuocKeDon: true,
    moTaNgan: 'Hỗ trợ xương khớp linh hoạt...',
    quyCach: 'Hộp 120 viên',
    dangBaoChe: 'Viên nén bao phim',
    hanSuDungThang: 36,
    thanhPhan: 'Glucosamine sulfate, Chondroitin...',
    congDung: 'Hỗ trợ giảm đau nhức khớp...',
    cachDung: 'Uống 1 viên/lần, ngày 2 lần...',
    doiTuongSuDung: 'Người trưởng thành...',
    chongChiDinh: 'Người dị ứng hải sản...',
    tacDungPhu: 'Có thể gặp buồn nôn nhẹ...',
    luuY: 'Nên tham khảo bác sĩ...',
    baoQuan: 'Để nơi khô, dưới 30 độ C...'
  }
};

const route = useRoute();
const thuoc = ref(duLieuThuoc.omega3);
const anhHienTai = ref('');
const selectedUnitIndex = ref(0);
const activeTab = ref('dacdiem');

const dsSanPhamTuongTu = [
  { id: 'omega3', name: 'Omega 3 Orihiro', image: '/src/assets/images/product_01.png' },
  { id: 'jointcare', name: 'Joint Care', image: '/src/assets/images/product_02.png' }
];

const formatTien = (so) => so.toLocaleString('vi-VN') + 'đ';

const loadProduct = () => {
  const id = route.params.id;
  thuoc.value = duLieuThuoc[id] || duLieuThuoc.omega3;
  anhHienTai.value = thuoc.value.hinhAnh[0];
  selectedUnitIndex.value = 0;
  activeTab.value = 'dacdiem';
};

onMounted(loadProduct);
watch(() => route.params.id, loadProduct);
</script>

<style scoped>
.thumb-item.active { border: 2px solid #28a745; }
.tab-content { display: block; }
.prescription-pill { color: red; font-weight: bold; }
.related-item { min-width: 150px; text-decoration: none; color: black; }

/* optional basic spacing nếu chưa có css ngoài */
.thumb-list { display: flex; gap: 8px; margin-top: 10px; }
.thumb-item { border: 1px solid #eee; background: #fff; padding: 4px; cursor: pointer; }
.thumb-item img { width: 60px; height: 60px; object-fit: cover; }
.main-image { width: 100%; border-radius: 8px; }
.tab-btn.active { background: #28a745; color: #fff; }
</style>
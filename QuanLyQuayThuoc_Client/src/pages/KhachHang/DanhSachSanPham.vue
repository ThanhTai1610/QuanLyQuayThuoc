<template>
  <div class="site-wrap">
    <div class="container py-4">

      <div class="d-flex align-items-center justify-content-between mb-3">
        <router-link to="/" class="btn btn-link p-0">â† Vá» trang chá»§</router-link>
        <div class="products-page-title">Danh sÃ¡ch sáº£n pháº©m nhÃ  thuá»‘c</div>
      </div>

      <div class="row">
        <div class="col-lg-3 mb-4">
          <aside class="filter-sidebar">
            <h2 class="filter-title"><i class="fas fa-filter mr-2"></i>Bá»™ lá»c thÃ´ng minh</h2>

            <div class="filter-group">
              <div class="filter-label">Danh má»¥c</div>
              <div class="filter-scroll-container">
                <label class="filter-option" v-for="dm in danhSachDanhMuc" :key="dm.maDanhMuc">
                  <input type="checkbox" :value="dm.maDanhMuc" v-model="locDanhMuc" />
                  <span class="custom-check"></span>
                  <span class="option-text">{{ dm.tenDanhMuc }}</span>
                </label>
              </div>
            </div>

            <div class="filter-group">
              <div class="filter-label">Äá»‘i tÆ°á»£ng</div>
              <label class="filter-option" v-for="dt in danhSachDoiTuong" :key="dt">
                <input type="checkbox" :value="dt" v-model="locDoiTuong" />
                <span class="custom-check"></span>
                <span class="option-text">{{ dt }}</span>
              </label>
            </div>

            <div class="filter-group">
              <div class="filter-label">NhÃ  sáº£n xuáº¥t</div>
              <div class="filter-scroll-container">
                <label class="filter-option" v-for="nsx in danhSachNSX" :key="nsx">
                  <input type="checkbox" :value="nsx" v-model="locNSX" />
                  <span class="custom-check"></span>
                  <span class="option-text">{{ nsx }}</span>
                </label>
              </div>
            </div>

            <div class="filter-group">
              <div class="filter-label">Khoáº£ng giÃ¡</div>
              <label class="filter-option" v-for="g in danhSachGia" :key="g.value">
                <input type="radio" name="gia" :value="g.value" v-model="locGia" />
                <span class="custom-radio"></span>
                <span class="option-text">{{ g.label }}</span>
              </label>
            </div>

            <div class="filter-group mb-0">
              <div class="filter-label">Dáº¡ng bÃ o cháº¿</div>
              <label class="filter-option" v-for="dbc in danhSachDBC" :key="dbc">
                <input type="checkbox" :value="dbc" v-model="locDBC" />
                <span class="custom-check"></span>
                <span class="option-text">{{ dbc }}</span>
              </label>
            </div>

            <button class="btn btn-reset-filter btn-sm btn-block mt-4" @click="xoaBoLoc">
              <i class="fas fa-sync-alt mr-2"></i> LÃ m má»›i bá»™ lá»c
            </button>
          </aside>
        </div>

        <div class="col-lg-9">
          <div class="products-toolbar">
            <div class="products-found">
              TÃ¬m tháº¥y <strong>{{ tongSanPham }} sáº£n pháº©m</strong>
              <span v-if="tuKhoa"> cho "<em>{{ tuKhoa }}</em>"</span>
            </div>
            <div class="products-found">
              <label for="sapXep" class="mb-0 mr-2">Sáº¯p xáº¿p theo:</label>
              <select id="sapXep" class="form-control d-inline-block w-auto" v-model="sapXep" @change="loadData(true)">
                <option value="ban-chay">BÃ¡n cháº¡y nháº¥t</option>
                <option value="gia-tang">GiÃ¡ tháº¥p Ä‘áº¿n cao</option>
                <option value="gia-giam">GiÃ¡ cao Ä‘áº¿n tháº¥p</option>
                <option value="moi-nhat">Má»›i nháº¥t</option>
              </select>
            </div>
          </div>

          <div v-if="dangTai" class="text-center py-5">
            <div class="spinner-border text-primary" role="status">
              <span class="sr-only">Äang táº£i...</span>
            </div>
          </div>

          <div v-else>
            <div class="row" v-if="danhSachSanPham.length > 0">
              <div class="col-md-4 col-sm-6 mb-4" v-for="sp in danhSachSanPham" :key="sp.maThuoc">
                <article class="product-card">
                  <div class="product-origin-badge"> 
                    <img :src="getFlagUrl(sp.nuocSanXuat || 'viá»‡t nam')" class="flag-icon" />
                    <span>{{ sp.nuocSanXuat || 'Viá»‡t Nam' }}</span>
                  </div>
                  
                  <div v-if="sp.laThuocKeDon" class="product-badge-prescription" title="Thuá»‘c kÃª Ä‘Æ¡n">
                     ðŸ”´ Thuá»‘c kÃª Ä‘Æ¡n
                  </div>

                  <img :src="getImageUrl(sp.hinhAnhChinh)" :alt="sp.tenThuoc" class="product-image" />

                  <h3 class="product-name">{{ sp.tenThuoc }}</h3>

                  <div class="product-category-label" v-if="sp.tenDanhMuc">
                    <img v-if="isImageUrl(sp.iconDanhMuc)" 
                         :src="getImageUrl(sp.iconDanhMuc)" 
                         class="category-img-mini mr-1" />
                    
                    <i v-else-if="sp.iconDanhMuc && (sp.iconDanhMuc.startsWith('fa-') || sp.iconDanhMuc.startsWith('fas'))" 
                       :class="['fas', sp.iconDanhMuc, getIconColorClass(sp.tenDanhMuc), 'mr-1']"></i>
                    
                    <i v-else :class="[getFallbackIcon(sp.tenDanhMuc), 'text-primary mr-1']"></i>

                    {{ sp.tenDanhMuc }}
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
                      :title="sp.laThuocKeDon ? 'Sáº£n pháº©m nÃ y cáº§n cÃ³ Ä‘Æ¡n thuá»‘c' : ''"
                    >
                      {{ sp.laThuocKeDon ? 'Cáº§n kÃª Ä‘Æ¡n' : 'Chá»n mua' }}
                    </button>
                    
                    <router-link
                      :to="{ name: 'ChiTietSanPham', params: { id: sp.maThuoc } }"
                      class="btn btn-outline-primary btn-sm">
                      Chi tiáº¿t
                    </router-link>
                  </div>
                </article>
              </div>
            </div>

            <div v-else class="text-center py-5 text-muted">
              <p>KhÃ´ng tÃ¬m tháº¥y sáº£n pháº©m phÃ¹ há»£p.</p>
            </div>

            <nav aria-label="PhÃ¢n trang sáº£n pháº©m" v-if="tongTrang > 1">
              <ul class="pagination justify-content-center products-pagination">
                <li class="page-item" :class="{ disabled: trangHienTai === 1 }">
                  <a class="page-link" href="#" @click.prevent="doiTrang(trangHienTai - 1)">TrÆ°á»›c</a>
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

const danhSachDanhMuc = ref([]);
const danhSachNSX     = ref([]);
const danhSachDoiTuong = ['Tráº» em', 'NgÆ°á»i lá»›n', 'Phá»¥ ná»¯ mang thai', 'NgÆ°á»i giÃ '];
const danhSachDBC      = ['ViÃªn nÃ©n', 'ViÃªn nang', 'Siro', 'Bá»™t pha'];
const danhSachGia = [
  { label: 'DÆ°á»›i 100.000Ä‘',         value: '0-100000'         },
  { label: '100.000Ä‘ - 500.000Ä‘',      value: '100000-500000'    },
  { label: '500.000Ä‘ - 1.000.000Ä‘',    value: '500000-1000000'   },
  { label: 'TrÃªn 1.000.000Ä‘',          value: '1000000-99999999' },
];

// --- LOGIC Xá»¬ LÃ ICON ---

const isImageUrl = (icon) => {
  if (!icon) return false;
  return icon.match(/\.(jpeg|jpg|gif|png|svg)$/) != null || icon.startsWith('http') || icon.includes('/');
};

const getIconColorClass = (categoryName) => {
  return 'text-primary'; 
};

const getFallbackIcon = (categoryName) => {
  if (!categoryName) return 'fas fa-pills';
  const name = categoryName.toLowerCase();

  if (name.includes('tháº§n kinh') || name.includes('nÃ£o')) return 'fas fa-brain';
  if (name.includes('vitamin') || name.includes('khoÃ¡ng cháº¥t')) return 'fas fa-capsules';
  if (name.includes('sinh lÃ½') || name.includes('ná»™i tiáº¿t')) return 'fas fa-venetian-mask';
  if (name.includes('tim máº¡ch') || name.includes('huyáº¿t Ã¡p')) return 'fas fa-heartbeat';
  if (name.includes('miá»…n dá»‹ch') || name.includes('Ä‘á» khÃ¡ng')) return 'fas fa-shield-virus';
  if (name.includes('tiÃªu hÃ³a')) return 'fas fa-lungs'; 
  if (name.includes('lÃ n da')) return 'fas fa-hand-sparkles';
  if (name.includes('da máº·t')) return 'fas fa-pump-medical';
  
  return 'fas fa-pills';
};

// --- LOGIC Dá»® LIá»†U ---

const loadData = async (resetTrang = false) => {
  if (resetTrang) trangHienTai.value = 1;
  dangTai.value = true;
  try {
    const params = {
      trang: trangHienTai.value,
      soLuong: soLuongMoiTrang,
      sapXep: sapXep.value,
      q: tuKhoa.value || undefined,
      danhMuc: locDanhMuc.value.length > 0 ? locDanhMuc.value.join(',') : undefined,
      doiTuong: locDoiTuong.value.length > 0 ? locDoiTuong.value.join(',') : undefined,
      nsx: locNSX.value.length > 0 ? locNSX.value.join(',') : undefined,
      gia: locGia.value || undefined,
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
    console.error('Lá»—i táº£i sáº£n pháº©m:', err);
  } finally {
    dangTai.value = false;
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
};

const loadSidebar = async () => {
  try {
    const [resDM, resNSX] = await Promise.all([
      axiosClient.get('/DanhMuc'),
      axiosClient.get('/Thuoc/nha-san-xuat'),
    ]);
    danhSachDanhMuc.value = resDM.data || resDM || [];
    danhSachNSX.value     = resNSX.data || resNSX || [];
  } catch (err) {
    console.error('Lá»—i táº£i sidebar:', err);
  }
};

const doiTrang = (trang) => {
  if (trang < 1 || trang > tongTrang.value) return;
  trangHienTai.value = trang;
  loadData();
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
      title: 'ÄÃ£ thÃªm!',
      text: `${sp.tenThuoc} Ä‘Ã£ vÃ o giá» hÃ ng.`,
      confirmButtonText: 'Xem giá» hÃ ng',
      showCancelButton: true,
      cancelButtonText: 'Tiáº¿p tá»¥c',
    }).then((result) => {
      if (result.isConfirmed) router.push('/gio-hang');
    });
  } catch (err) {
    if (err.response?.status === 401) router.push('/dang-nhap'); 
    else Swal.fire({ icon: 'error', title: 'Lá»—i', text: 'KhÃ´ng thá»ƒ thÃªm vÃ o giá».' });
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

const getImageUrl = (path) => {
  if (!path) return 'https://via.placeholder.com/300x300.png?text=No+Image';
  if (path.startsWith('http')) return path;
  return `${import.meta.env.VITE_API_URL.replace('/api', '')}${path.startsWith('/') ? '' : '/'}${path}`;
};

const getFlagUrl = (countryName) => {
  if (!countryName) return 'https://flagcdn.com/w40/vn.png';

  const name = countryName
    .toLowerCase()
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '');

  if (name.includes('viet nam') || name === 'vn') return 'https://flagcdn.com/w40/vn.png';
  if (name.includes('hoa ky') || name.includes('my') || name.includes('usa') || name === 'us') return 'https://flagcdn.com/w40/us.png';
  if (name.includes('anh') || name === 'uk' || name === 'gb') return 'https://flagcdn.com/w40/gb.png';
  if (name.includes('phap') || name === 'fr') return 'https://flagcdn.com/w40/fr.png';
  if (name.includes('duc') || name === 'de') return 'https://flagcdn.com/w40/de.png';
  if (name.includes('nhat') || name === 'jp') return 'https://flagcdn.com/w40/jp.png';
  if (name.includes('han quoc') || name === 'kr') return 'https://flagcdn.com/w40/kr.png';
  if (name.includes('canada') || name === 'ca') return 'https://flagcdn.com/w40/ca.png';
  if (name.includes('italy') || name.includes('italia') || name === 'it' || name === 'y') return 'https://flagcdn.com/w40/it.png';
  if (name.includes('nga') || name.includes('russia') || name === 'ru') return 'https://flagcdn.com/w40/ru.png';
  if (name.includes('philippines') || name.includes('philippin') || name.includes('phi lip pin') || name === 'ph') return 'https://flagcdn.com/w40/ph.png';

  return 'https://flagcdn.com/w40/un.png';
};


const formatGia = (value) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value ?? 0);

watch([locDanhMuc, locDoiTuong, locNSX, locGia, locDBC], () => loadData(true), { deep: true });

onMounted(() => {
  if (route.query.maDanhMuc) {
    // Náº¿u cÃ³ mÃ£ danh má»¥c tá»« URL (vÃ­ dá»¥ tá»« Trang chá»§ báº¥m qua), thÃ¬ tick chá»n nÃ³ luÃ´n
    const dmId = parseInt(route.query.maDanhMuc);
    if (!isNaN(dmId)) {
      locDanhMuc.value = [dmId];
    }
  }
  loadData();
  loadSidebar();
});
</script>

<style scoped>
/* Giá»¯ nguyÃªn CSS cÅ© cá»§a báº¡n */
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
  height: 22px;
}

.product-badge-prescription {
  position: absolute;
  top: 10px;
  right: 10px; /* Äá»‘i diá»‡n quá»‘c ká»³ */
  z-index: 10;
  height: 22px;
  /* Cá»°C Ká»² QUAN TRá»ŒNG: GiÃºp khung chá»‰ dÃ i báº±ng chá»¯ */
  width: fit-content; 
  display: flex;
  align-items: center;

  /* Style nhÃ£n nhá» gá»n */
  background: rgba(255, 241, 240, 0.95); /* MÃ u ná»n Ä‘á» nháº¡t */
  padding: 2px 8px;
  border-radius: 20px; 
  border: 1px solid #ffccc7;
  
  /* Font chá»¯ */
  color: #cf1322;
  font-size: 10px;
  font-weight: 600;
  line-height: 1;
  backdrop-filter: blur(2px);
}

.product-badge-prescription i {
  font-size: 14px;
  margin-right: 3px;
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

.category-img-mini {
  width: 16px;
  height: 16px;
  object-fit: contain;
  vertical-align: middle;
}

.product-category-label {
  font-size: 13px;
  margin-bottom: 8px;
  font-weight: 500;
  color: #007bff;
  display: flex;
  align-items: center;
  background: #f0f7ff;
  padding: 2px 8px;
  border-radius: 4px;
  width: fit-content;
}

.product-category-label i {
  font-size: 14px;
  margin-right: 6px;
  width: 18px;
  text-align: center;
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
}

.btn-primary:disabled {
  background-color: #f5f5f5 !important;
  border-color: #d9d9d9 !important;
  color: #bfbfbf !important;
}

/* --- UI Bá»˜ Lá»ŒC Má»šI --- */
.filter-sidebar {
  background: #fff;
  border-radius: 12px;
  border: 1px solid #eef2f6;
  padding: 20px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.03);
}

.filter-title {
  font-size: 16px;
  font-weight: 700;
  color: #333;
  margin-bottom: 20px;
  padding-bottom: 12px;
  border-bottom: 2px solid #f0f4f8;
}

.filter-group {
  margin-bottom: 20px;
}

.filter-label {
  font-size: 14px;
  font-weight: 600;
  color: #555;
  margin-bottom: 12px;
}

/* Khung cuá»™n cho danh má»¥c dÃ i */
.filter-scroll-container {
  max-height: 180px;
  overflow-y: auto;
  padding-right: 5px;
}

.filter-scroll-container::-webkit-scrollbar { width: 4px; }
.filter-scroll-container::-webkit-scrollbar-thumb { background: #e0e0e0; border-radius: 10px; }

/* Custom Checkbox & Radio */
.filter-option {
  display: flex;
  align-items: center;
  position: relative;
  padding: 6px 0;
  cursor: pointer;
  margin-bottom: 0;
}

.filter-option input {
  position: absolute;
  opacity: 0;
}

.custom-check, .custom-radio {
  width: 18px;
  height: 18px;
  border: 2px solid #d1d9e6;
  border-radius: 4px;
  margin-right: 12px;
  display: inline-block;
  position: relative;
  background: #fff;
  transition: all 0.2s;
}

.custom-radio { border-radius: 50%; }

/* Khi Ä‘Æ°á»£c chá»n */
.filter-option input:checked ~ .custom-check {
  background: #007bff;
  border-color: #007bff;
}

.filter-option input:checked ~ .custom-check::after {
  content: '\f00c';
  font-family: 'Font Awesome 5 Free';
  font-weight: 900;
  color: #fff;
  font-size: 10px;
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
}

.filter-option input:checked ~ .custom-radio {
  border-color: #007bff;
}

.filter-option input:checked ~ .custom-radio::after {
  content: '';
  width: 8px;
  height: 8px;
  background: #007bff;
  border-radius: 50%;
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
}

.option-text {
  font-size: 13.5px;
  color: #666;
  transition: color 0.2s;
}

.filter-option:hover .option-text {
  color: #007bff;
}

.btn-reset-filter {
  background: #f8f9fa;
  border: 1px solid #dee2e6;
  color: #666;
  font-weight: 600;
  border-radius: 8px;
}

.btn-reset-filter:hover {
  background: #e9ecef;
  color: #333;
}
</style>

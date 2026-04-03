<template>
  <div class="site-wrap">

    <!-- Breadcrumb -->
    <nav class="ql-breadcrumb" aria-label="Đường dẫn">
      <div class="container">
        <ol class="breadcrumb mb-0">
          <li class="breadcrumb-item"><router-link to="/">Trang chủ</router-link></li>
          <li class="breadcrumb-item"><router-link to="/ca-nhan">Cá nhân</router-link></li>
          <li class="breadcrumb-item active" aria-current="page">Quản lý sổ địa chỉ</li>
        </ol>
      </div>
    </nav>

    <div class="ql-wrapper">
      <div class="container">
        <div class="row">

          <!-- Sidebar -->
          <AccountSidebar
            :user="nguoiDung"
            activeMenu="addresses"
            @logout="dangXuat"
          />

          <!-- Nội dung chính -->
          <main class="col-lg-9 ql-main-column">

            <!-- Toolbar -->
            <div class="ql-toolbar-card">
              <header class="ql-main-header ql-main-header--toolbar">
                <h1>Quản lý sổ địa chỉ</h1>
                <button type="button" class="btn btn-primary ql-btn-add" @click="moFormThem">
                  + Thêm địa chỉ mới
                </button>
              </header>
            </div>

            <!-- Form thêm / sửa -->
            <transition name="slide">
              <div v-if="hienForm" class="ql-add-address-panel">
                <div class="ql-add-card">
                  <div class="ql-add-card-head">
                    <h2 class="ql-add-card-title">{{ dangSuaId ? 'Sửa địa chỉ' : 'Thêm địa chỉ mới' }}</h2>
                    <button type="button" class="ql-add-close" @click="dongForm" aria-label="Đóng form">&times;</button>
                  </div>

                  <div class="ql-add-form ql-add-form-grid">
                    <p class="ql-form-section-label ql-form-section-label--recipient">Thông tin người nhận</p>
                    <div class="form-row">
                      <div class="form-group col-md-6">
                        <label class="ql-field-label">Họ tên</label>
                        <input type="text" class="form-control ql-input ql-input-lg"
                          v-model="form.hoTenNguoiNhan" placeholder="Nhập họ tên" />
                      </div>
                      <div class="form-group col-md-6">
                        <label class="ql-field-label">Số điện thoại</label>
                        <input type="tel" class="form-control ql-input ql-input-lg"
                          v-model="form.soDienThoaiNhan" placeholder="Ví dụ: 0912345678" />
                      </div>
                    </div>

                    <p class="ql-form-section-label ql-form-section-label--addr">Địa chỉ nhận hàng</p>
                    <div class="form-row">
                      <div class="form-group col-md-4">
                        <label class="ql-field-label ql-field-label--sm">Tỉnh / Thành phố</label>
                        <input type="text" class="form-control ql-input ql-input-lg"
                          v-model="form.tinhThanh" placeholder="VD: TP. Hồ Chí Minh" />
                      </div>
                      <div class="form-group col-md-4">
                        <label class="ql-field-label ql-field-label--sm">Quận / Huyện</label>
                        <input type="text" class="form-control ql-input ql-input-lg"
                          v-model="form.quanHuyen" placeholder="VD: Quận 7" />
                      </div>
                      <div class="form-group col-md-4">
                        <label class="ql-field-label ql-field-label--sm">Phường / Xã</label>
                        <input type="text" class="form-control ql-input ql-input-lg"
                          v-model="form.phuongXa" placeholder="VD: Phường Tân Phong" />
                      </div>
                    </div>

                    <div class="form-group ql-form-group-full">
                      <label class="ql-field-label">Địa chỉ cụ thể</label>
                      <input type="text" class="form-control ql-input ql-input-lg"
                        v-model="form.diaChiChiTiet" placeholder="Số nhà, tên đường, tòa nhà..." />
                    </div>

                    <div class="form-row align-items-center ql-form-row-bottom">
                      <div class="col-lg-6 mb-3 mb-lg-0">
                        <span class="ql-form-section-label d-block mb-2">Loại địa chỉ</span>
                        <div class="ql-chip-group" role="group" aria-label="Loại địa chỉ">
                          <label class="ql-chip">
                            <input type="radio" value="Nhà riêng" v-model="form.loaiDiaChi" />
                            <span>Nhà</span>
                          </label>
                          <label class="ql-chip">
                            <input type="radio" value="Văn phòng" v-model="form.loaiDiaChi" />
                            <span>Văn phòng</span>
                          </label>
                        </div>
                      </div>
                      <div class="col-lg-6">
                        <div class="ql-default-row ql-default-row--inline">
                          <span class="ql-default-label">Đặt làm địa chỉ mặc định</span>
                          <label class="ql-switch">
                            <input type="checkbox" v-model="form.laMacDinh" aria-label="Đặt làm mặc định" />
                            <span class="ql-switch-slider"></span>
                          </label>
                        </div>
                      </div>
                    </div>

                    <p v-if="loiForm" class="text-danger small mt-2">{{ loiForm }}</p>

                    <div class="ql-add-actions">
                      <button type="button" class="btn btn-primary ql-btn-hoan-tat"
                        :disabled="dangLuu" @click="luuDiaChi">
                        {{ dangLuu ? 'Đang lưu...' : 'Hoàn tất' }}
                      </button>
                      <button type="button" class="btn btn-outline-secondary ql-btn-cancel"
                        @click="dongForm">Hủy</button>
                    </div>
                  </div>
                </div>
              </div>
            </transition>

            <!-- Danh sách địa chỉ -->
            <div class="ql-main-card ql-main-card--list">
              <div v-if="dangTai" class="text-center py-4">
                <div class="spinner-border text-primary" role="status">
                  <span class="sr-only">Đang tải...</span>
                </div>
              </div>

              <ul v-else class="ql-address-list">
                <li v-if="danhSachDiaChi.length === 0" class="text-center py-4 text-muted">
                  Chưa có địa chỉ nào. Hãy thêm địa chỉ mới.
                </li>

                <li class="ql-address-item" v-for="dc in danhSachDiaChi" :key="dc.maDiaChi">
                  <div class="ql-address-body">
                    <div class="ql-address-line1">
                      {{ dc.hoTenNguoiNhan }}
                      <span class="ql-sep">|</span>
                      {{ dc.soDienThoaiNhan }}
                      <span v-if="dc.laMacDinh" class="badge badge-success ml-2 small">Mặc định</span>
                    </div>
                    <div class="ql-address-line">
                      {{ diaChiDayDu(dc) }}
                    </div>
                    <span v-if="dc.loaiDiaChi" class="ql-tag">
                      <span :class="dc.loaiDiaChi === 'Nhà riêng' ? 'icon-home' : 'icon-briefcase'"></span>
                      {{ dc.loaiDiaChi }}
                    </span>
                  </div>
                  <div class="ql-address-actions">
                    <a href="#" @click.prevent="moFormSua(dc)">Sửa</a>
                    <span class="ql-action-sep">|</span>
                    <a href="#" class="ql-delete" @click.prevent="xoaDiaChi(dc)">Xóa</a>
                    <template v-if="!dc.laMacDinh">
                      <span class="ql-action-sep">|</span>
                      <a href="#" @click.prevent="datMacDinh(dc)">Đặt mặc định</a>
                    </template>
                  </div>
                </li>
              </ul>
            </div>

          </main>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import '../../assets/css/quan-ly-so-dia-chi.css';
import { ref, reactive, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import axiosClient from '../../api/axiosClient';
import AccountSidebar from '../../components/AccountSidebar.vue';

const router = useRouter();

const nguoiDung       = ref({ hoTen: '', soDienThoai: '' });
const danhSachDiaChi  = ref([]);
const dangTai         = ref(false);
const dangLuu         = ref(false);
const hienForm        = ref(false);
const dangSuaId       = ref(null);
const loiForm         = ref('');

const formRong = () => ({
  hoTenNguoiNhan:  '',
  soDienThoaiNhan: '',
  tinhThanh:       '',
  quanHuyen:       '',
  phuongXa:        '',
  diaChiChiTiet:   '',
  loaiDiaChi:      'Nhà riêng',
  laMacDinh:       false,
});
const form = reactive(formRong());

// ── Load dữ liệu ──
const loadData = async () => {
  dangTai.value = true;
  try {
    const [resUser, resDC] = await Promise.all([
      axiosClient.get('/HoSo/thong-tin'),
      axiosClient.get('/SoDiaChi'),
    ]);
    nguoiDung.value      = resUser;
    danhSachDiaChi.value = resDC;
  } catch (err) {
    console.error('Lỗi tải dữ liệu:', err);
    if (err.response?.status === 401) router.push('/dang-nhap');
  } finally {
    dangTai.value = false;
  }
};

// ── Form ──
const moFormThem = () => {
  dangSuaId.value = null;
  loiForm.value   = '';
  Object.assign(form, formRong());
  hienForm.value  = true;
};

const moFormSua = (dc) => {
  dangSuaId.value = dc.maDiaChi;
  loiForm.value   = '';
  Object.assign(form, {
    hoTenNguoiNhan:  dc.hoTenNguoiNhan,
    soDienThoaiNhan: dc.soDienThoaiNhan,
    tinhThanh:       dc.tinhThanh,
    quanHuyen:       dc.quanHuyen,
    phuongXa:        dc.phuongXa,
    diaChiChiTiet:   dc.diaChiChiTiet,
    loaiDiaChi:      dc.loaiDiaChi || 'Nhà riêng',
    laMacDinh:       !!dc.laMacDinh,
  });
  hienForm.value = true;
};

const dongForm = () => { hienForm.value = false; };

// ── Lưu (thêm / sửa) ──
// POST /SoDiaChi     → thêm mới
// PUT  /SoDiaChi/:id → cập nhật
const luuDiaChi = async () => {
  loiForm.value = '';
  if (!form.hoTenNguoiNhan.trim() || !form.soDienThoaiNhan.trim() || !form.diaChiChiTiet.trim()) {
    loiForm.value = 'Vui lòng điền đầy đủ họ tên, số điện thoại và địa chỉ.';
    return;
  }

  dangLuu.value = true;
  try {
    if (dangSuaId.value) {
      await axiosClient.put(`/SoDiaChi/${dangSuaId.value}`, form);
    } else {
      await axiosClient.post('/SoDiaChi', form);
    }
    dongForm();
    await loadData();
  } catch (err) {
    loiForm.value = err.response?.data?.message || 'Có lỗi xảy ra. Vui lòng thử lại.';
  } finally {
    dangLuu.value = false;
  }
};

// ── Xóa ──
// DELETE /SoDiaChi/:id
const xoaDiaChi = async (dc) => {
  if (!confirm(`Xóa địa chỉ "${diaChiDayDu(dc)}"?`)) return;
  try {
    await axiosClient.delete(`/SoDiaChi/${dc.maDiaChi}`);
    await loadData();
  } catch (err) {
    alert('Không thể xóa địa chỉ này.');
  }
};

// ── Đặt mặc định ──
// PUT /SoDiaChi/:id/mac-dinh
const datMacDinh = async (dc) => {
  try {
    await axiosClient.put(`/SoDiaChi/${dc.maDiaChi}/mac-dinh`);
    await loadData();
  } catch (err) {
    alert('Có lỗi xảy ra.');
  }
};

const diaChiDayDu = (dc) =>
  [dc.diaChiChiTiet, dc.phuongXa, dc.quanHuyen, dc.tinhThanh].filter(Boolean).join(', ');

const dangXuat = () => {
  localStorage.clear();
  router.push('/dang-nhap');
};

onMounted(loadData);
</script>

<style scoped>
.slide-enter-active,
.slide-leave-active {
  transition: all 0.3s ease;
  overflow: hidden;
}
.slide-enter-from,
.slide-leave-to {
  max-height: 0;
  opacity: 0;
}
.slide-enter-to,
.slide-leave-from {
  max-height: 800px;
  opacity: 1;
}
</style>
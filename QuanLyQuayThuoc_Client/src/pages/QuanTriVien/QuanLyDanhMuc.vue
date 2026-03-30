<template>
  <div class="container-fluid">

    <!-- Tiêu đề -->
    <div class="d-sm-flex align-items-center justify-content-between mb-4">
      <h1 class="h3 mb-0 text-gray-800">Quản lý danh mục</h1>
      <button type="button" class="btn btn-sm btn-primary shadow-sm" @click="moModalThem">
        <i class="fas fa-plus fa-sm"></i> Thêm danh mục
      </button>
    </div>

    <!-- Cây danh mục -->
    <div class="card shadow mb-4">
      <div class="card-header py-3 d-flex align-items-center justify-content-between">
        <div>
          <h6 class="m-0 font-weight-bold text-primary">Danh mục dạng cây (Tree View)</h6>
          <small class="text-muted">Phân cấp rõ ràng — bấm mũi tên để thu gọn nhánh.</small>
        </div>
      </div>
      <div class="card-body p-0">
        <div class="px-3 pt-3">
          <div class="dm-tree-header">
            <span>Tên danh mục</span>
            <span class="text-center">Biểu tượng</span>
            <span>Số sản phẩm</span>
            <span>Trạng thái</span>
            <span class="dm-tree-header__col--actions">Hành động</span>
          </div>
        </div>

        <div v-if="dangTai" class="text-center py-4">
          <div class="spinner-border text-primary" role="status"><span class="sr-only">Đang tải...</span></div>
        </div>

        <div v-else class="dm-tree-wrap mx-3 mb-3">
          <ul class="dm-tree">
            <DanhMucNode
              v-for="node in cayDanhMuc"
              :key="node.maDanhMuc"
              :node="node"
              @sua="moModalSua"
              @xoa="xuLyXoa"
              @len="doiThuTu($event, 'len')"
              @xuong="doiThuTu($event, 'xuong')"
            />
          </ul>
        </div>
      </div>
    </div>

    <!-- Toast thông báo -->
    <div class="dm-toast-wrap" aria-live="polite">
      <div v-for="(t, i) in toasts" :key="i" :class="['alert', 'shadow', 'mb-2', 'alert-' + t.type]">
        {{ t.message }}
      </div>
    </div>

    <!-- ── MODAL: Thêm / Sửa ── -->
    <div class="modal fade" :class="{ show: hienModal }" :style="hienModal ? 'display:block' : ''"
      tabindex="-1" role="dialog" @click.self="dongModal">
      <div class="modal-dialog modal-lg modal-dialog-scrollable" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ dangSuaId ? 'Chỉnh sửa danh mục' : 'Thêm danh mục' }}</h5>
            <button type="button" class="close" @click="dongModal"><span>&times;</span></button>
          </div>
          <div class="modal-body">

            <div class="form-group">
              <label>Tên danh mục <span class="text-danger">*</span></label>
              <input type="text" class="form-control" v-model="form.tenDanhMuc"
                placeholder="Ví dụ: Dược mỹ phẩm" @input="autoSlug" />
              <small class="form-text text-muted">Hiển thị cho Admin và menu khách.</small>
            </div>

            <div class="form-group">
              <label>Danh mục cha</label>
              <select class="form-control" v-model="form.maDanhMucCha">
                <option :value="null">— Không chọn (danh mục gốc) —</option>
                <option
                  v-for="dm in danhSachPhang"
                  :key="dm.maDanhMuc"
                  :value="dm.maDanhMuc"
                  :disabled="dm.maDanhMuc === dangSuaId"
                >
                  {{ dm.tenDanhMuc }}
                </option>
              </select>
              <small class="form-text text-muted">Khi sửa, không thể chọn chính danh mục đó làm cha.</small>
            </div>

            <div class="form-group">
              <label>Tải lên biểu tượng (icon)</label>
              <div class="dm-modal-icon-upload">
                <input type="file" accept="image/*,.svg" @change="chonIcon" />
                <small class="d-block text-muted mt-2">PNG, SVG nhỏ — kiểu icon menu.</small>
                <div class="dm-modal-icon-preview">
                  <img v-if="iconPreview" :src="iconPreview" alt="Icon" />
                  <i v-else class="fas fa-image text-muted"></i>
                </div>
              </div>
            </div>

            <div class="form-group">
              <label>Mô tả</label>
              <textarea class="form-control" v-model="form.moTa" rows="3"
                placeholder="Giới thiệu ngắn về nhóm sản phẩm…"></textarea>
            </div>

            <div class="form-group">
              <label>Đường dẫn (Slug) — SEO</label>
              <input type="text" class="form-control dm-slug-input" v-model="form.slug"
                placeholder="tu-dong-tu-ten" @input="slugTay = true" />
              <small class="form-text dm-slug-hint text-muted">
                Tự sinh từ tên. Có thể chỉnh tay trước khi lưu.
              </small>
            </div>

            <div class="form-group mb-0">
              <label>Trạng thái hiển thị</label>
              <select class="form-control" v-model="form.trangThai">
                <option value="hien">Hiện</option>
                <option value="an">Ẩn (kèm ẩn cả nhánh con trên trang khách)</option>
              </select>
            </div>

            <p v-if="loiModal" class="text-danger small mt-2">{{ loiModal }}</p>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="dongModal">Đóng</button>
            <button type="button" class="btn btn-primary" :disabled="dangLuu" @click="luuDanhMuc">
              <i class="fas fa-save mr-1"></i>
              {{ dangLuu ? 'Đang lưu...' : 'Lưu' }}
            </button>
          </div>
        </div>
      </div>
    </div>
    <div v-if="hienModal" class="modal-backdrop fade show"></div>

    <!-- ── MODAL: Lỗi xóa ── -->
    <div class="modal fade" :class="{ show: hienLoiXoa }" :style="hienLoiXoa ? 'display:block' : ''"
      tabindex="-1" role="dialog" @click.self="hienLoiXoa = false">
      <div class="modal-dialog" role="document">
        <div class="modal-content border-left-danger">
          <div class="modal-header bg-light">
            <h5 class="modal-title text-danger"><i class="fas fa-exclamation-triangle mr-2"></i>Không thể xóa</h5>
            <button type="button" class="close" @click="hienLoiXoa = false"><span>&times;</span></button>
          </div>
          <div class="modal-body">
            <p class="mb-0">{{ loiXoaNoiDung }}</p>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-primary" @click="hienLoiXoa = false">Đã hiểu</button>
          </div>
        </div>
      </div>
    </div>
    <div v-if="hienLoiXoa" class="modal-backdrop fade show"></div>

    <!-- ── MODAL: Xác nhận xóa ── -->
    <div class="modal fade" :class="{ show: hienXacNhanXoa }" :style="hienXacNhanXoa ? 'display:block' : ''"
      tabindex="-1" role="dialog" @click.self="hienXacNhanXoa = false">
      <div class="modal-dialog" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Xác nhận xóa</h5>
            <button type="button" class="close" @click="hienXacNhanXoa = false"><span>&times;</span></button>
          </div>
          <div class="modal-body">
            <p class="mb-0">{{ xacNhanXoaText }}</p>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="hienXacNhanXoa = false">Hủy</button>
            <button type="button" class="btn btn-danger" :disabled="dangXoa" @click="dongYXoa">
              {{ dangXoa ? 'Đang xóa...' : 'Xóa' }}
            </button>
          </div>
        </div>
      </div>
    </div>
    <div v-if="hienXacNhanXoa" class="modal-backdrop fade show"></div>

  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue';
import axiosClient from '../../api/axiosClient';
import DanhMucNode from './DanhMucNode.vue';

// ── State ──
const cayDanhMuc  = ref([]);   // Dữ liệu dạng cây từ API
const dangTai     = ref(false);
const dangLuu     = ref(false);
const dangXoa     = ref(false);
const toasts      = ref([]);

// Modal thêm/sửa
const hienModal   = ref(false);
const dangSuaId   = ref(null);
const loiModal    = ref('');
const slugTay     = ref(false);
const iconFile    = ref(null);
const iconPreview = ref('');

const form = reactive({
  tenDanhMuc:   '',
  maDanhMucCha: null,
  moTa:         '',
  slug:         '',
  trangThai:    'hien',
});

// Modal lỗi xóa
const hienLoiXoa    = ref(false);
const loiXoaNoiDung = ref('');

// Modal xác nhận xóa
const hienXacNhanXoa  = ref(false);
const xacNhanXoaText  = ref('');
const pendingXoaId    = ref(null);

// ── Danh sách phẳng để chọn cha trong modal ──
const danhSachPhang = computed(() => {
  const result = [];
  const flatten = (nodes) => {
    nodes.forEach(n => {
      result.push({ maDanhMuc: n.maDanhMuc, tenDanhMuc: n.tenDanhMuc });
      if (n.children?.length) flatten(n.children);
    });
  };
  flatten(cayDanhMuc.value);
  return result;
});

// ── Load dữ liệu ──
// GET /DanhMuc/cay — trả về mảng dạng cây với children[]
const loadData = async () => {
  dangTai.value = true;
  try {
    const res = await axiosClient.get('/DanhMuc/cay');
    cayDanhMuc.value = res.data;
  } catch (err) {
    showToast('Lỗi tải danh mục.', 'danger');
  } finally {
    dangTai.value = false;
  }
};

// ── Slug ──
const slugifyVi = (str) => {
  return str.normalize('NFD').replace(/[\u0300-\u036f]/g, '')
    .replace(/đ/g, 'd').replace(/Đ/g, 'D')
    .toLowerCase().trim()
    .replace(/[^a-z0-9]+/g, '-').replace(/^-+|-+$/g, '');
};

const autoSlug = () => {
  if (!slugTay.value) form.slug = slugifyVi(form.tenDanhMuc);
};

// ── Modal thêm ──
const moModalThem = () => {
  dangSuaId.value   = null;
  loiModal.value    = '';
  slugTay.value     = false;
  iconFile.value    = null;
  iconPreview.value = '';
  Object.assign(form, { tenDanhMuc: '', maDanhMucCha: null, moTa: '', slug: '', trangThai: 'hien' });
  hienModal.value = true;
};

// ── Modal sửa ──
const moModalSua = (node) => {
  dangSuaId.value   = node.maDanhMuc;
  loiModal.value    = '';
  slugTay.value     = true;
  iconFile.value    = null;
  iconPreview.value = node.icon || '';
  Object.assign(form, {
    tenDanhMuc:   node.tenDanhMuc,
    maDanhMucCha: node.maDanhMucCha ?? null,
    moTa:         node.moTa || '',
    slug:         node.slug || '',
    trangThai:    node.trangThai || 'hien',
  });
  hienModal.value = true;
};

const dongModal = () => { hienModal.value = false; };

// ── Chọn icon ──
const chonIcon = (e) => {
  const file = e.target.files[0];
  if (!file) return;
  iconFile.value    = file;
  iconPreview.value = URL.createObjectURL(file);
};

// ── Lưu danh mục ──
// POST /DanhMuc     (thêm mới)
// PUT  /DanhMuc/:id (sửa)
const luuDanhMuc = async () => {
  loiModal.value = '';
  if (!form.tenDanhMuc.trim()) { loiModal.value = 'Vui lòng nhập tên danh mục.'; return; }
  if (dangSuaId.value && form.maDanhMucCha === dangSuaId.value) {
    loiModal.value = 'Không thể chọn chính danh mục này làm danh mục cha.'; return;
  }

  dangLuu.value = true;
  try {
    const fd = new FormData();
    fd.append('tenDanhMuc',   form.tenDanhMuc);
    fd.append('maDanhMucCha', form.maDanhMucCha ?? '');
    fd.append('moTa',         form.moTa);
    fd.append('slug',         form.slug);
    fd.append('trangThai',    form.trangThai);
    if (iconFile.value) fd.append('icon', iconFile.value);

    if (dangSuaId.value) {
      await axiosClient.put(`/DanhMuc/${dangSuaId.value}`, fd);
      showToast('Đã cập nhật danh mục.', 'success');
    } else {
      await axiosClient.post('/DanhMuc', fd);
      showToast('Đã thêm danh mục.', 'success');
    }
    dongModal();
    loadData();
  } catch (err) {
    loiModal.value = err.response?.data?.message || 'Có lỗi xảy ra.';
  } finally {
    dangLuu.value = false;
  }
};

// ── Xóa ──
// DELETE /DanhMuc/:id
const xuLyXoa = (node) => {
  if (node.soSanPham > 0) {
    loiXoaNoiDung.value = `Danh mục "${node.tenDanhMuc}" đang có ${node.soSanPham} sản phẩm. Hãy chuyển các sản phẩm sang danh mục khác trước khi xóa.`;
    hienLoiXoa.value = true;
    return;
  }
  pendingXoaId.value   = node.maDanhMuc;
  xacNhanXoaText.value = `Bạn có chắc muốn xóa danh mục "${node.tenDanhMuc}"?`;
  hienXacNhanXoa.value = true;
};

const dongYXoa = async () => {
  dangXoa.value = true;
  try {
    await axiosClient.delete(`/DanhMuc/${pendingXoaId.value}`);
    showToast('Đã xóa danh mục.', 'success');
    hienXacNhanXoa.value = false;
    loadData();
  } catch (err) {
    showToast(err.response?.data?.message || 'Không thể xóa.', 'danger');
  } finally {
    dangXoa.value = false;
  }
};

// ── Đổi thứ tự ──
// PUT /DanhMuc/:id/thu-tu  body: { huong: 'len' | 'xuong' }
const doiThuTu = async (node, huong) => {
  try {
    await axiosClient.put(`/DanhMuc/${node.maDanhMuc}/thu-tu`, { huong });
    loadData();
  } catch {
    showToast('Không thể đổi thứ tự.', 'danger');
  }
};

// ── Toast ──
const showToast = (message, type = 'info') => {
  toasts.value.push({ message, type });
  setTimeout(() => toasts.value.shift(), 3200);
};

onMounted(loadData);
</script>
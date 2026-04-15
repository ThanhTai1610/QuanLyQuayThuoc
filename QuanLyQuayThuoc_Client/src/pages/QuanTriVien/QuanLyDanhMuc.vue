<template>
  <div class="container-fluid">
    <div class="d-sm-flex align-items-center justify-content-between mb-4">
      <h1 class="h3 mb-0 text-gray-800">Quản lý danh mục</h1>
      <button type="button" class="btn btn-sm btn-primary shadow-sm" @click="moModalThem">
        <i class="fas fa-plus fa-sm"></i> Thêm danh mục
      </button>
    </div>

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

    <div class="dm-toast-wrap" aria-live="polite">
      <div v-for="(t, i) in toasts" :key="i" :class="['alert', 'shadow', 'mb-2', 'alert-' + t.type]">
        {{ t.message }}
      </div>
    </div>

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
            </div>

            <div class="form-group">
              <label>Biểu tượng hiển thị <span class="text-danger">*</span></label>
              <div class="d-flex align-items-center mb-2 p-2 border rounded bg-light">
                <div class="mr-3 text-primary" style="font-size: 1.5rem; width: 40px; text-align: center;">
                  <i :class="['fas', form.icon || 'fa-capsules']"></i>
                </div>
                <span class="text-muted small">Đang chọn: <b>{{ form.icon || 'fa-capsules' }}</b></span>
              </div>
              
              <div class="icon-grid p-2 border rounded">
                <button v-for="ico in danhSachIcon" :key="ico" 
                  type="button" 
                  class="btn btn-outline-secondary m-1 btn-icon-select"
                  :class="{ 'active': form.icon === ico }"
                  @click="form.icon = ico">
                  <i :class="['fas', ico]"></i>
                </button>
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
            </div>

            <div class="form-group mb-0">
              <label>Trạng thái hiển thị</label>
              <select class="form-control" v-model="form.trangThai">
                <option value="hien">Hiện</option>
                <option value="an">Ẩn</option>
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

    <div v-if="hienLoiXoa" class="modal-backdrop fade show"></div>
    <div v-if="hienXacNhanXoa" class="modal-backdrop fade show"></div>

  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue';
import axiosClient from '../../api/axiosClient';
import DanhMucNode from './DanhMucnode.vue';
import '../../assets/css_admin/quan-ly-danh-muc.css';

// ── Danh sách Icon gợi ý ──
const danhSachIcon = [
  'fa-capsules', 'fa-pills', 'fa-prescription-bottle-alt', 'fa-heartbeat', 
  'fa-brain', 'fa-shield-virus', 'fa-baby', 'fa-hand-holding-medical', 
  'fa-thermometer', 'fa-vials', 'fa-first-aid', 'fa-lungs', 
  'fa-eye', 'fa-tooth', 'fa-spa'
];

const cayDanhMuc  = ref([]);
const dangTai     = ref(false);
const dangLuu     = ref(false);
const dangXoa     = ref(false);
const toasts      = ref([]);
const hienModal   = ref(false);
const dangSuaId   = ref(null);
const loiModal    = ref('');
const slugTay     = ref(false);

const form = reactive({
  tenDanhMuc:   '',
  maDanhMucCha: null,
  moTa:         '',
  slug:         '',
  trangThai:    'hien',
  icon:         'fa-capsules'
});

const hienLoiXoa = ref(false);
const loiXoaNoiDung = ref('');
const hienXacNhanXoa = ref(false);
const xacNhanXoaText = ref('');
const pendingXoaId = ref(null);

const danhSachPhang = computed(() => {
  const result = [];
  if (!cayDanhMuc.value || !Array.isArray(cayDanhMuc.value)) return result;
  const flatten = (nodes) => {
    nodes.forEach(n => {
      result.push({ maDanhMuc: n.maDanhMuc, tenDanhMuc: n.tenDanhMuc });
      if (n.children && n.children.length > 0) flatten(n.children);
    });
  };
  flatten(cayDanhMuc.value);
  return result;
});

const loadData = async () => {
  dangTai.value = true;
  try {
    const res = await axiosClient.get('/DanhMuc/cay'); 
    cayDanhMuc.value = Array.isArray(res) ? res : (res.data || []); 
  } catch (err) {
    showToast('Lỗi tải danh mục.', 'danger');
  } finally {
    dangTai.value = false;
  }
};

const slugifyVi = (str) => {
  return str.normalize('NFD').replace(/[\u0300-\u036f]/g, '')
    .replace(/đ/g, 'd').replace(/Đ/g, 'D')
    .toLowerCase().trim()
    .replace(/[^a-z0-9]+/g, '-').replace(/^-+|-+$/g, '');
};

const autoSlug = () => {
  if (!slugTay.value) form.slug = slugifyVi(form.tenDanhMuc);
};

const moModalThem = () => {
  dangSuaId.value = null;
  loiModal.value = '';
  slugTay.value = false;
  Object.assign(form, { tenDanhMuc: '', maDanhMucCha: null, moTa: '', slug: '', trangThai: 'hien', icon: 'fa-capsules' });
  hienModal.value = true;
};

const moModalSua = (node) => {
  dangSuaId.value = node.maDanhMuc;
  loiModal.value = '';
  slugTay.value = true;
  Object.assign(form, {
    tenDanhMuc:   node.tenDanhMuc,
    maDanhMucCha: node.maDanhMucCha ?? null,
    moTa:         node.moTa || '',
    slug:         node.slug || '',
    trangThai:    node.trangThai || 'hien',
    icon:         node.icon || 'fa-capsules'
  });
  hienModal.value = true;
};

const dongModal = () => { hienModal.value = false; };

const luuDanhMuc = async () => {
  loiModal.value = '';
  if (!form.tenDanhMuc.trim()) { loiModal.value = 'Vui lòng nhập tên danh mục.'; return; }

  dangLuu.value = true;
  try {
    const formData = new FormData();
    // Ghi đúng tên thuộc tính giống trong class DanhMucDTO của C#
    formData.append('TenDanhMuc', form.tenDanhMuc);
    if (form.maDanhMucCha && form.maDanhMucCha !== 0) {
    formData.append('MaDanhMucCha', form.maDanhMucCha);
}
    formData.append('MoTa', form.moTa || '');
    formData.append('Slug', form.slug);
    formData.append('TrangThai', form.trangThai);
    formData.append('Icon', form.icon); 

    // QUAN TRỌNG: Thêm cấu hình Header ở tham số thứ 3
    const config = {
      headers: { 'Content-Type': 'multipart/form-data' }
    };

    if (dangSuaId.value) {
      // axiosClient.put(url, data, config)
      await axiosClient.put(`/DanhMuc/${dangSuaId.value}`, formData, config);
      showToast('Đã cập nhật thành công.', 'success');
    } else {
      await axiosClient.post('/DanhMuc', formData, config);
      showToast('Đã thêm thành công.', 'success');
    }
    
    dongModal();
    loadData();
  } catch (err) {
    console.error("Lỗi 415 chi tiết:", err.response);
    loiModal.value = err.response?.data?.message || 'Lỗi định dạng dữ liệu (415).';
  } finally {
    dangLuu.value = false;
  }
};

const showToast = (message, type = 'info') => {
  toasts.value.push({ message, type });
  setTimeout(() => toasts.value.shift(), 3200);
};
// Thêm vào trong <script setup> của file QuanLyDanhMuc.vue

// 1. Hàm xử lý đổi thứ tự
const doiThuTu = async (node, huong) => {
  try {
    // Gọi API Put kèm body là hướng di chuyển
    await axiosClient.put(`/DanhMuc/${node.maDanhMuc}/thu-tu`, { huong: huong });
    showToast(`Đã chuyển ${node.tenDanhMuc} lên/xuống.`, 'success');
    loadData(); // Load lại cây danh mục để thấy sự thay đổi
  } catch (err) {
    const msg = err.response?.data?.message || 'Không thể đổi thứ tự.';
    showToast(msg, 'warning');
  }
};

// 2. Hàm xử lý Xóa
const xuLyXoa = async (node) => {
  // Kiểm tra nhanh ở Frontend nếu node có sản phẩm
  if (node.soSanPham > 0) {
    alert(`Danh mục "${node.tenDanhMuc}" đang có ${node.soSanPham} sản phẩm. Bạn không thể xóa!`);
    return;
  }

  if (confirm(`Bạn có chắc muốn xóa danh mục "${node.tenDanhMuc}"?`)) {
    try {
      await axiosClient.delete(`/DanhMuc/${node.maDanhMuc}`);
      showToast('Đã xóa danh mục thành công.', 'success');
      loadData();
    } catch (err) {
      // Backend sẽ trả về lỗi nếu có sản phẩm mà FE chưa check hết
      const errorMsg = err.response?.data?.message || 'Lỗi khi xóa.';
      alert(errorMsg); 
    }
  }
};
onMounted(loadData);
</script>

<style scoped>
.icon-grid {
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  gap: 8px;
  background: #fdfdfd;
}
.btn-icon-select {
  font-size: 1.2rem;
  padding: 10px;
  transition: 0.2s;
}
.btn-icon-select:hover {
  transform: scale(1.1);
}
.btn-icon-select.active {
  background-color: #4e73df;
  color: white;
  border-color: #4e73df;
}
</style>
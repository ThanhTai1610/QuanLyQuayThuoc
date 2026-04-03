<template>
  <li class="dm-tree-item" :class="{ 'dm-tree-item--collapsed': collapsed }">
    <div class="dm-tree-row" :class="coChildren ? 'dm-tree-row--parent' : 'dm-tree-row--leaf'">

      <div class="dm-tree-name-cell">
        <button 
          v-if="coChildren" 
          type="button" 
          class="dm-tree-toggle" 
          @click="collapsed = !collapsed"
        >
          <i :class="['fas', collapsed ? 'fa-chevron-right' : 'fa-chevron-down']"></i>
        </button>
        <span v-else class="dm-tree-toggle-spacer"></span>

        <i :class="['fas', coChildren ? 'fa-folder' : 'fa-file-alt', 'dm-tree-type-icon']"></i>
        <span class="dm-tree-name-text">{{ node.tenDanhMuc }}</span>
      </div>

      <div class="dm-tree-col-icon text-center">
        <span class="dm-tree-icon-preview">
          <img 
            v-if="iconInfo.type === 'image'" 
            :src="iconInfo.value" 
            :alt="node.tenDanhMuc" 
          />
          
          <i 
            v-else 
            :class="['fas', iconInfo.value]" 
            style="font-size: 18px; color: #28a745;"
          ></i>
        </span>
      </div>

      <div class="dm-tree-col-count text-center">
        <span class="dm-tree-count" :class="{ 'dm-tree-count--zero': !node.soSanPham }">
          {{ node.soSanPham ?? 0 }}
        </span>
      </div>

      <div class="dm-tree-col-status text-center">
        <span class="badge" :class="node.trangThai === 'hien' ? 'badge-success' : 'badge-secondary'">
          {{ node.trangThai === 'hien' ? 'Hiện' : 'Ẩn' }}
        </span>
      </div>

      <div class="dm-tree-actions">
        <div class="btn-group">
          <button type="button" class="btn btn-warning btn-sm" @click="$emit('sua', node)" title="Sửa">
            <i class="fas fa-edit"></i>
          </button>
          <button type="button" class="btn btn-danger btn-sm" @click="$emit('xoa', node)" title="Xóa">
            <i class="fas fa-trash"></i>
          </button>
        </div>
        <div class="btn-group ml-2">
          <button type="button" class="btn btn-outline-secondary btn-sm" @click="$emit('len', node)" title="Lên">
            <i class="fas fa-arrow-up"></i>
          </button>
          <button type="button" class="btn btn-outline-secondary btn-sm" @click="$emit('xuong', node)" title="Xuống">
            <i class="fas fa-arrow-down"></i>
          </button>
        </div>
      </div>
    </div>

    <ul v-if="coChildren && !collapsed" class="dm-tree-children">
      <DanhMucNode
        v-for="child in node.children"
        :key="child.maDanhMuc"
        :node="child"
        @sua="$emit('sua', $event)"
        @xoa="$emit('xoa', $event)"
        @len="$emit('len', $event)"
        @xuong="$emit('xuong', $event)"
      />
    </ul>
  </li>
</template>

<script>
export default { name: 'DanhMucNode' }
</script>

<script setup>
import { ref, computed } from 'vue';

const props = defineProps({
  node: { type: Object, required: true },
});

defineEmits(['sua', 'xoa', 'len', 'xuong']);

const collapsed = ref(true);
const coChildren = computed(() => props.node.children && props.node.children.length > 0);

// Logic xử lý Icon thông minh
const iconInfo = computed(() => {
  const iconData = props.node.icon;
  const name = props.node.tenDanhMuc ? props.node.tenDanhMuc.toLowerCase() : '';

  // 1. Kiểm tra nếu là file ảnh thực sự (Tránh lỗi load icon-flask như ảnh)
  if (iconData && (iconData.includes('.') || iconData.includes('/'))) {
    return {
      type: 'image',
      value: iconData.startsWith('http') ? iconData : `https://localhost:7070${iconData.startsWith('/') ? '' : '/'}${iconData}`
    };
  }

  // 2. Nếu là Class Icon (Từ API của Tài)
  if (iconData) {
    // Chuyển 'icon-stomach' thành 'fa-stomach' để khớp FontAwesome
    const faClass = iconData.replace('icon-', 'fa-');
    return { type: 'class', value: faClass };
  }

  // 3. Nếu icon bị NULL -> Tự động "đổ" icon theo từ khóa tên danh mục
  let autoClass = 'fa-box-medical'; // Mặc định

  if (name.includes('thuốc') || name.includes('kháng sinh')) autoClass = 'fa-pills';
  else if (name.includes('thực phẩm') || name.includes('chức năng')) autoClass = 'fa-flask';
  else if (name.includes('xương') || name.includes('khớp')) autoClass = 'fa-bone';
  else if (name.includes('mỹ phẩm') || name.includes('chăm sóc')) autoClass = 'fa-pump-medical';
  else if (name.includes('tiêu hóa') || name.includes('dạ dày')) autoClass = 'fa-stomach';
  else if (name.includes('mắt') || name.includes('não')) autoClass = 'fa-eye';
  else if (name.includes('vitamin')) autoClass = 'fa-capsules';
  else if (name.includes('mẹ') || name.includes('bé')) autoClass = 'fa-baby';
  else if (coChildren.value) autoClass = 'fa-folder-open';

  return { type: 'class', value: autoClass };
});
</script>

<style scoped>
.dm-tree-row {
  display: flex;
  align-items: center;
  padding: 8px 12px;
  border-bottom: 1px solid #f0f0f0;
}

.dm-tree-name-cell { flex: 1; min-width: 250px; display: flex; align-items: center; }
.dm-tree-col-icon   { width: 80px; }
.dm-tree-col-count  { width: 100px; }
.dm-tree-col-status { width: 100px; }
.dm-tree-actions    { width: 180px; text-align: right; }

.dm-tree-toggle-spacer {
  display: inline-block;
  width: 26px;
}

.dm-tree-icon-preview img {
  height: 24px;
  width: 24px;
  object-fit: contain;
  border-radius: 4px;
}

.dm-tree-row:hover {
  background-color: #f8f9fa;
}

.dm-tree-type-icon {
  margin-right: 8px;
  color: #6c757d;
  width: 16px;
  text-align: center;
}
</style>
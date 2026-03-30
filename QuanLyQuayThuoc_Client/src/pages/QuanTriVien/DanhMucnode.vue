<template>
  <li class="dm-tree-item" :class="{ 'dm-tree-item--collapsed': collapsed }">
    <div class="dm-tree-row" :class="coChildren ? 'dm-tree-row--parent' : 'dm-tree-row--leaf'">

      <!-- Tên -->
      <div class="dm-tree-name-cell">
        <button type="button" class="dm-tree-toggle"
          :aria-expanded="!collapsed"
          :tabindex="coChildren ? 0 : -1"
          @click="collapsed = !collapsed">
          <i class="fas fa-chevron-down"></i>
        </button>
        <i :class="['fas', coChildren ? 'fa-folder' : 'fa-file-alt',
          'dm-tree-type-icon',
          coChildren ? 'dm-tree-type-icon--folder' : 'dm-tree-type-icon--leaf']">
        </i>
        <span class="dm-tree-name-text">{{ node.tenDanhMuc }}</span>
      </div>

      <!-- Icon -->
      <div class="text-center">
        <span class="dm-tree-icon-preview">
          <img v-if="node.icon" :src="node.icon" alt="" style="height:20px;" />
          <i v-else class="fas fa-image text-muted"></i>
        </span>
      </div>

      <!-- Số sản phẩm -->
      <span class="dm-tree-count" :class="{ 'dm-tree-count--zero': node.soSanPham === 0 }">
        {{ node.soSanPham ?? 0 }}
      </span>

      <!-- Trạng thái -->
      <span>
        <span class="badge" :class="node.trangThai === 'hien' ? 'badge-success' : 'badge-secondary'">
          {{ node.trangThai === 'hien' ? 'Hiện' : 'Ẩn' }}
        </span>
      </span>

      <!-- Hành động -->
      <div class="dm-tree-actions">
        <button type="button" class="btn btn-warning btn-sm" @click="$emit('sua', node)">
          <i class="fas fa-edit"></i> Sửa
        </button>
        <button type="button" class="btn btn-danger btn-sm" @click="$emit('xoa', node)">
          <i class="fas fa-trash"></i> Xóa
        </button>
        <div class="btn-group" role="group">
          <button type="button" class="btn btn-outline-secondary btn-sm" title="Lên" @click="$emit('len', node)">
            <i class="fas fa-arrow-up"></i>
          </button>
          <button type="button" class="btn btn-outline-secondary btn-sm" title="Xuống" @click="$emit('xuong', node)">
            <i class="fas fa-arrow-down"></i>
          </button>
        </div>
      </div>
    </div>

    <!-- Children — đệ quy -->
    <ul v-if="coChildren" class="dm-tree-children" :class="{ 'dm-tree-children--collapsed': collapsed }">
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

<script setup>
import { ref, computed } from 'vue';

const props = defineProps({
  node: { type: Object, required: true },
});

defineEmits(['sua', 'xoa', 'len', 'xuong']);

const collapsed  = ref(false);
const coChildren = computed(() => props.node.children?.length > 0);
</script>
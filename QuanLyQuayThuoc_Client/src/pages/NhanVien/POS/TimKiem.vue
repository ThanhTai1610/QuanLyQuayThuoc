<template>
  <div class="pos-header mb-3">
    <div class="pos-autocomplete-wrap">
      <label class="small text-muted mb-1" for="posSearch">
        Tìm thuốc theo tên / hoạt chất
      </label>
      <input 
        id="posSearch" 
        type="text" 
        class="form-control pos-search-input"
        placeholder="Ví dụ: Smecta, 8937..., hoặc LOT-..." 
        autocomplete="off"
        v-model="searchQuery"
        @input="handleSearch"
      >

      <div v-if="showSuggestions" class="pos-autocomplete-list border rounded shadow-sm bg-white">
        <div 
          v-for="thuoc in filteredResults" 
          :key="thuoc.id" 
          class="suggestion-item p-2 border-bottom"
          @click="selectItem(thuoc)"
        >
          <div class="d-flex justify-content-between">
            <strong class="text-primary">{{ thuoc.tenThuoc }}</strong>
            <span class="badge badge-info">{{ formatMoney(thuoc.giaBan) }}</span>
          </div>
          <div class="small text-muted">
            Hoạt chất: {{ thuoc.hoatChat }} | Tồn: <span class="text-danger">{{ thuoc.tonKho }} {{ thuoc.donVi }}</span>
          </div>
        </div>
        <div v-if="filteredResults.length === 0" class="p-3 text-center text-muted">
          Không tìm thấy thuốc nào phù hợp.
        </div>
      </div>
    </div>

    <div class="d-flex justify-content-end mt-2">
      <button type="button" class="btn btn-primary" id="posBtnThemNhanh" data-toggle="modal" data-target="#modalThemNhanh">
        <i class="fas fa-plus mr-1"></i> Thêm nhanh
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';

const emit = defineEmits(['add-to-cart']);

const searchQuery = ref('');
const showSuggestions = ref(false);

// Giả lập danh sách thuốc từ Database (Sau này Tài gọi API lấy cái này)
const danhSachThuoc = ref([
  { id: 1, tenThuoc: 'Smecta', hoatChat: 'Diosmectite', giaBan: 5000, donVi: 'Gói', tonKho: 100 },
  { id: 2, tenThuoc: 'Paracetamol 500mg', hoatChat: 'Paracetamol', giaBan: 2000, donVi: 'Viên', tonKho: 500 },
  { id: 3, tenThuoc: 'Panadol Extra', hoatChat: 'Paracetamol + Caffeine', giaBan: 3500, donVi: 'Viên', tonKho: 250 },
]);

// Lọc kết quả tìm kiếm
const filteredResults = computed(() => {
  if (!searchQuery.value) return [];
  const query = searchQuery.value.toLowerCase();
  return danhSachThuoc.value.filter(t => 
    t.tenThuoc.toLowerCase().includes(query) || 
    t.hoatChat.toLowerCase().includes(query)
  );
});

const handleSearch = () => {
  showSuggestions.value = searchQuery.value.length > 0;
};

const selectItem = (thuoc) => {
  // Gửi thông tin thuốc đã chọn lên trang cha
  emit('add-to-cart', { ...thuoc, soLuong: 1 });
  
  // Reset ô tìm kiếm
  searchQuery.value = '';
  showSuggestions.value = false;
};

const formatMoney = (val) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val);
};
</script>

<style scoped>
.pos-autocomplete-wrap {
  position: relative;
}
.pos-autocomplete-list {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  z-index: 1050;
  max-height: 300px;
  overflow-y: auto;
}
.suggestion-item:hover {
  background-color: #f8f9fc;
  cursor: pointer;
}
</style>
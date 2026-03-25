<template>
  <div class="pos-header mb-3">
    <div class="pos-autocomplete-wrap">
      <label class="small text-muted mb-1" for="posSearch">
        Tìm thuốc theo tên / hoạt chất
      </label>
      <input id="posSearch" type="text" class="form-control pos-search-input"
        placeholder="Ví dụ: Smecta, Paracetamol..." autocomplete="off" v-model="searchQuery" @input="handleSearch">

      <div v-if="showSuggestions" class="pos-autocomplete-list border rounded shadow-sm bg-white">
        <div v-for="thuoc in danhSachThuoc" :key="thuoc.maThuoc" class="suggestion-item p-2 border-bottom"
          @click="selectItem(thuoc)">
          <div class="d-flex justify-content-between">
            <strong class="text-primary">{{ thuoc.tenThuoc }}</strong>
            <span class="badge badge-info">{{ formatMoney(thuoc.giaBanHienTai) }}</span>
          </div>
          <div class="small text-muted">
            Hoạt chất: {{ thuoc.hoatChat }}
          </div>
        </div>

        <div v-if="danhSachThuoc.length === 0 && searchQuery.length > 0" class="p-3 text-center text-muted">
          Không tìm thấy thuốc nào phù hợp.
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import axios from 'axios';

const emit = defineEmits(['add-to-cart']);

const searchQuery = ref('');
const showSuggestions = ref(false);
const danhSachThuoc = ref([]);
const isLoading = ref(false);

const getField = (obj, camel, pascal) => obj[camel] !== undefined ? obj[camel] : obj[pascal];

let timeout = null;

const handleSearch = () => {
  clearTimeout(timeout);

  timeout = setTimeout(async () => {
    if (!searchQuery.value.trim() || searchQuery.value.trim().length < 2) {
      showSuggestions.value = false;
      danhSachThuoc.value = [];
      return;
    }

    try {
      isLoading.value = true;
      const res = await axios.get('https://localhost:7070/api/BanHang/tim-kiem', {
        params: { tenThuoc: searchQuery.value }
      });

      danhSachThuoc.value = (res.data || []).map(t => ({
        maThuoc: t.maThuoc,
        tenThuoc: t.tenThuoc,
        hoatChat: t.hamLuong || 'Chưa có thông tin',
        soLuongTon: t.soLuongTon || 0,
        giaBanHienTai: t.giaBanHienTai || 0,
        tenDonVi: t.tenDonVi || 'Đơn vị',
        danhSachDonVi: t.danhSachDonVi || []
      }));

      showSuggestions.value = true;
    } catch (err) {
      console.error("Lỗi tìm kiếm:", err);
      danhSachThuoc.value = [];
    } finally {
      isLoading.value = false;
    }
  }, 300);
};

const selectItem = async (thuoc) => {
  try {
    const resLo = await axios.get(`https://localhost:7070/api/BanHang/lo-hang/${thuoc.maThuoc}`);
    const danhSachLo = (resLo.data || []).map(lo => ({
      maLo: lo.maLo,
      hanSuDung: lo.hanSuDung,
      soLuongTon: lo.soLuongTon
    }));

    if (danhSachLo.length === 0) {
      alert("Thuốc này đã hết các lô hàng khả dụng!");
      return;
    }

    // Gửi sự kiện add-to-cart với đầy đủ thông tin đơn vị
    emit('add-to-cart', {
      ...thuoc,
      soLuong: 1,
      maDvtSelected: thuoc.danhSachDonVi.length > 0 ? thuoc.danhSachDonVi[0].maDvt : null,
      giaBan: thuoc.danhSachDonVi.length > 0 ? thuoc.danhSachDonVi[0].giaBan : thuoc.giaBanHienTai,
      danhSachLo: danhSachLo,
      loHangSelected: danhSachLo[0].maLo
    });

    searchQuery.value = '';
    showSuggestions.value = false;
  } catch (err) {
    console.error("Lỗi khi chọn thuốc:", err);
  }
};

const formatMoney = (val) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val || 0);
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
  background: white;
}

.suggestion-item:hover {
  background-color: #f8f9fc;
  cursor: pointer;
}
</style>
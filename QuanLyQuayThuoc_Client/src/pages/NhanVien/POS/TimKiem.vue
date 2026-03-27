<template>
  <div class="pos-header mb-3">
    <div class="pos-autocomplete-wrap">
      <label class="small text-muted mb-1" for="posSearch">
        Tìm thuốc theo tên / hoạt chất
      </label>
      <input id="posSearch" type="text" class="form-control pos-search-input"
        placeholder="Ví dụ: Smecta, Paracetamol..." autocomplete="off" v-model="tuKhoaTimKiem" @input="xuLyTimKiem">

      <div v-if="hienThiGoiY" class="pos-autocomplete-list border rounded shadow-sm bg-white">
        <div v-for="thuoc in danhSachThuoc" :key="thuoc.maThuoc" class="suggestion-item p-2 border-bottom"
          @click="chonSanPham(thuoc)">
          <div class="d-flex justify-content-between">
            <strong class="text-primary">{{ thuoc.tenThuoc }}</strong>
            <span class="badge badge-info">{{ dinhDangTien(thuoc.giaBanHienTai) }}</span>
          </div>
          <div class="small text-muted">
            Hoạt chất: {{ thuoc.hoatChat }}
          </div>
        </div>

        <div v-if="danhSachThuoc.length === 0 && tuKhoaTimKiem.length > 0" class="p-3 text-center text-muted">
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

const tuKhoaTimKiem = ref('');
const hienThiGoiY = ref(false);
const danhSachThuoc = ref([]);
const dangTai = ref(false);

const layTruong = (obj, tenCamelCase, tenPascalCase) => obj[tenCamelCase] !== undefined ? obj[tenCamelCase] : obj[tenPascalCase];

let boHenGio = null;

const xuLyTimKiem = () => {
  clearTimeout(boHenGio);

  boHenGio = setTimeout(async () => {
    if (!tuKhoaTimKiem.value.trim() || tuKhoaTimKiem.value.trim().length < 2) {
      hienThiGoiY.value = false;
      danhSachThuoc.value = [];
      return;
    }

    try {
      dangTai.value = true;
      const ketQua = await axios.get('https://localhost:7070/api/BanHang/tim-kiem', {
        params: { tenThuoc: tuKhoaTimKiem.value }
      });

      danhSachThuoc.value = (ketQua.data || []).map(t => ({
        maThuoc: t.maThuoc,
        tenThuoc: t.tenThuoc,
        hoatChat: t.hamLuong || 'Chưa có thông tin',
        soLuongTon: t.soLuongTon || 0,
        giaBanHienTai: t.giaBanHienTai || 0,
        tenDonVi: t.tenDonVi || 'Đơn vị',
        danhSachDonVi: t.danhSachDonVi || []
      }));

      hienThiGoiY.value = true;
    } catch (loi) {
      console.error("Lỗi tìm kiếm:", loi);
      danhSachThuoc.value = [];
    } finally {
      dangTai.value = false;
    }
  }, 300);
};

const chonSanPham = async (thuoc) => {
  try {
    const ketQuaLo = await axios.get(`https://localhost:7070/api/BanHang/lo-hang/${thuoc.maThuoc}`);
    const danhSachLo = (ketQuaLo.data || []).map(lo => ({
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

    tuKhoaTimKiem.value = '';
    hienThiGoiY.value = false;
  } catch (loi) {
    console.error("Lỗi khi chọn thuốc:", loi);
  }
};

const dinhDangTien = (giaTri) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(giaTri || 0);
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
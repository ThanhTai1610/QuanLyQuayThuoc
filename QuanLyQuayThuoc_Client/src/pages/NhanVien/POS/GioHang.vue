<template>
  <div class="pos-left">
    <div class="pos-card mb-3">
      <div class="pos-card__header">
        <h5 class="pos-card__title mb-0">Giỏ hàng đang bán</h5>
        <div class="small text-muted">
          <span class="pos-cart-count" id="posTongMon">{{ cartItems.length }}</span> món
        </div>
      </div>
      <div class="p-3">
        <div class="table-responsive">
          <table class="table table-bordered mb-0 pos-cart-table" id="posCartTable">
            <thead class="thead-light">
              <tr>
                <th style="min-width: 260px;">Tên thuốc &amp; Đơn vị</th>
                <th style="min-width: 170px;">Số lượng</th>
                <th style="min-width: 240px;">Lô hàng (FEFO)</th>
                <th style="min-width: 140px;">Thành tiền</th>
                <th style="min-width: 70px;">Xóa</th>
              </tr>
            </thead>
            <tbody id="posCartBody">
              <tr v-for="(item, index) in cartItems" :key="item.id || index">
                <td>
                  <div class="font-weight-bold text-primary">{{ item.tenThuoc }}</div>
                  <small class="text-muted">Đơn vị: {{ item.donVi }}</small>
                </td>
                <td>
                  <div class="input-group input-group-sm">
                    <div class="input-group-prepend">
                      <button class="btn btn-outline-secondary" @click="updateQty(index, -1)">-</button>
                    </div>
                    <input type="number" class="form-control text-center" v-model.number="item.soLuong" min="1">
                    <div class="input-group-append">
                      <button class="btn btn-outline-secondary" @click="updateQty(index, 1)">+</button>
                    </div>
                  </div>
                </td>
                <td>
                  <select class="form-control form-control-sm" v-model="item.loHangSelected">
                    <option v-for="lo in item.danhSachLo" :key="lo.id" :value="lo.id">
                      {{ lo.maLo }} - HSD: {{ lo.hanSuDung }} (Tồn: {{ lo.tonKho }})
                    </option>
                  </select>
                </td>
                <td class="text-right font-weight-bold">
                  {{ formatMoney(item.giaBan * item.soLuong) }}
                </td>
                <td class="text-center">
                  <button class="btn btn-sm btn-outline-danger" @click="removeItem(index)">
                    <i class="fas fa-trash"></i>
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <div v-if="cartItems.length === 0" id="posCartEmpty" class="text-center text-muted py-4">
          Chưa có sản phẩm. Hãy tìm và thêm bằng ô tìm kiếm.
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
// Nhận dữ liệu giỏ hàng từ trang cha (BanHangTaiQuay.vue)
const props = defineProps({
  cartItems: {
    type: Array,
    default: () => []
  }
});

const emit = defineEmits(['remove-item', 'update-quantity']);

// Hàm format tiền tệ
const formatMoney = (value) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
};

// Gửi sự kiện xóa sản phẩm lên cha
const removeItem = (index) => {
  emit('remove-item', index);
};

// Hàm cập nhật nhanh số lượng bằng nút +/-
const updateQty = (index, change) => {
  emit('update-quantity', { index, change });
};
</script>

<style scoped>
/* CSS bổ sung nếu cần, các class SB Admin 2 sẽ tự nhận từ file css tổng */
.pos-cart-table th {
  vertical-align: middle;
  text-align: center;
}
.pos-cart-table td {
  vertical-align: middle;
}
</style>
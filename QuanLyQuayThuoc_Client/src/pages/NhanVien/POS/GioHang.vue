<template>
  <div class="pos-left">
    <div class="pos-card mb-3">
      <div class="pos-card__header">
        <h5 class="pos-card__title mb-0">Giỏ hàng đang bán</h5>
        <div class="small text-muted">
          <span class="pos-cart-count">{{ cartItems.length }}</span> món
        </div>
      </div>
      <div class="p-3">
        <div class="table-responsive">
          <table class="table table-bordered mb-0 pos-cart-table">
            <thead class="thead-light">
              <tr>
                <th style="min-width: 200px;">Tên thuốc</th>
                <th style="min-width: 130px;">Đơn vị</th>
                <th style="min-width: 150px;">Số lượng</th>
                <th style="min-width: 220px;">Lô hàng (FEFO)</th>
                <th style="min-width: 120px;">Thành tiền</th>
                <th style="min-width: 50px;">Xóa</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, index) in cartItems" :key="index">
                <td>
                  <div class="font-weight-bold text-primary">{{ item.tenThuoc }}</div>
                  <small class="text-muted">Giá: {{ formatMoney(item.giaBan) }}</small>
                </td>

                <td>
                  <select class="form-control form-control-sm" v-model="item.maDvtSelected"
                    @change="updatePriceByUnit(item)">
                    <option v-for="dv in item.danhSachDonVi" :key="dv.maDvt" :value="dv.maDvt">
                      {{ dv.tenDonVi }} - {{ formatMoney(dv.giaBan) }}
                    </option>
                  </select>
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
                    <option v-for="lo in item.danhSachLo" :key="lo.maLo" :value="lo.maLo">
                      Lô: {{ lo.maLo }} - HSD: {{ lo.hanSuDung }} (Tồn: {{ lo.soLuongTon }})
                    </option>
                  </select>
                  <div v-if="!item.danhSachLo || item.danhSachLo.length === 0" class="small text-danger">
                    Hết hàng!
                  </div>
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

        <div v-if="cartItems.length === 0" class="text-center text-muted py-4">
          Chưa có sản phẩm. Hãy tìm và thêm bằng ô tìm kiếm.
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
const props = defineProps({
  cartItems: { type: Array, default: () => [] }
});

const emit = defineEmits(['remove-item', 'update-quantity', 'update-unit']);

const formatMoney = (value) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value || 0);
};

const formatDate = (dateStr) => {
  if (!dateStr) return 'N/A';
  const date = new Date(dateStr);
  return date.toLocaleDateString('vi-VN');
};

const removeItem = (index) => {
  emit('remove-item', index);
};

const updateQty = (index, change) => {
  emit('update-quantity', { index, change });
};

const updatePriceByUnit = (item) => {
  const unit = item.danhSachDonVi.find(d => d.maDvt === item.maDvtSelected);
  if (unit) {
    item.giaBan = unit.giaBan;
  }
};
</script>
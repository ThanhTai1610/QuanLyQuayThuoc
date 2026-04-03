import { reactive, computed } from 'vue';

export const cartState = reactive({
  items: JSON.parse(localStorage.getItem('cart')) || [],

  // Hàm tính tổng số lượng (Cộng dồn tất cả quantity)
  totalQuantity: computed(() => {
    return cartState.items.reduce((sum, item) => sum + (item.soLuong || 0), 0);
  }),

  // Hàm cập nhật giỏ hàng (Gọi hàm này khi thêm/xóa/sửa ở trang chi tiết hoặc trang giỏ hàng)
  refreshCart() {
    this.items = JSON.parse(localStorage.getItem('cart')) || [];
  }
});
import axiosClient from '../api/axiosClient';
import Swal from 'sweetalert2';

export const useMomo = () => {
  const createPayment = async (amount, orderInfo, orderId, userType = 'KhachHang') => {
    try {
      const res = await axiosClient.post('/ThanhToan/tao-thanh-toan', {
        OrderId: String(orderId),
        OrderInfo: String(orderInfo),
        Amount: Math.round(Number(amount)),
        UserType: String(userType)
      });

      console.log('Dữ liệu nhận được từ axiosClient:', res);

      // axiosClient đã unwrap response.data nên res chính là object trả về
      const payUrl = res?.payUrl || res?.PayUrl;

      if (payUrl) {
        window.location.href = payUrl;
      } else {
        // Không có payUrl — MoMo trả về lỗi
        console.error('Mất link thanh toán. Nội dung Server trả về:', res);
        const errorMsg = res?.message || res?.Message || 'Không lấy được link thanh toán từ MoMo.';
        Swal.fire('Thanh toán thất bại', errorMsg, 'error');
      }
    } catch (error) {
      console.error('Lỗi gọi API MoMo:', error);
      if (error.response?.data) {
        console.log('Chi tiết lỗi từ Backend:', error.response.data);
      }
      const errorMsg = error.response?.data?.detail
        || error.response?.data?.message
        || error.message
        || 'Không thể khởi tạo giao dịch MoMo. Vui lòng thử lại!';
      Swal.fire('Thanh toán thất bại', errorMsg, 'error');
    }
  };

  return { createPayment };
};
import axiosClient from '../api/axiosClient';
import Swal from 'sweetalert2';

export const useMomo = () => {
  // Thêm tham số userType vào hàm
  const createPayment = async (amount, orderInfo, orderId, userType = "KhachHang") => {
    try {
      // Gửi đúng các trường dữ liệu mà OrderInfoModel ở C# đang chờ
      // File: useMomo.js
const res = await axiosClient.post('/ThanhToan/tao-thanh-toan', {
  OrderId: String(orderId),     // Khớp với public string OrderId
  OrderInfo: String(orderInfo), // Khớp với public string OrderInfo
  Amount: Math.round(Number(amount)),       // Khớp với public long Amount
  UserType: "KhachHang"         // Khớp với public string UserType
});
console.log("Dữ liệu nhận được từ axiosClient:", res);  
      // Lấy dữ liệu trả về (Lưu ý: C# thường trả về Property viết hoa như PayUrl)
const payUrl = res.payUrl || res.PayUrl || (res.data && res.data.payUrl);

      if (payUrl) {
        window.location.href = payUrl;
      } {
        // Nếu không có link, có thể MoMo trả về lỗi bên trong res
        console.error("Mất link thanh toán. Nội dung Server trả về:", res);
        const errorMsg = res.message || "Không lấy được link thanh toán từ MoMo.";
        throw new Error(errorMsg);
      }
    } catch (error) {
      console.error("Lỗi gọi API MoMo:", error);
      // In chi tiết lỗi từ Server ra console để debug nếu vẫn lỗi 400
      if (error.response && error.response.data) {
        console.log("Chi tiết lỗi từ Backend:", error.response.data);
      }
      Swal.fire("Thanh toán thất bại", "Không thể khởi tạo giao dịch MoMo. Vui lòng thử lại!", "error");
    }
  };

  return { createPayment };
};
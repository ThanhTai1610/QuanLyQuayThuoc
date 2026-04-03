import axios from "axios";
import Swal from "sweetalert2";

export const useMomo = () => {
  const createPayment = async (amount, orderInfo, userType) => {
    try {
      Swal.fire({
        title: "Đang kết nối MoMo...",
        didOpen: () => {
          Swal.showLoading();
        },
      });

      const response = await axios.post(
        "https://localhost:7070/api/ThanhToan/tao-thanh-toan",
        {
          amount: amount,
          orderInfo: orderInfo,
          userType: userType,
        },
      );

      if (response.data && response.data.payUrl) {
        window.location.href = response.data.payUrl;
      }
    } catch (error) {
      Swal.fire("Lỗi", "Không thể gọi API MoMo", "error");
    }
  };

  return { createPayment };
};

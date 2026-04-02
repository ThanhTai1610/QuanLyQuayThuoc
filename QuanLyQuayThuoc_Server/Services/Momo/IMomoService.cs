using QuanLyQuayThuoc.Models.Momo;
using Microsoft.AspNetCore.Http;

namespace QuanLyQuayThuoc.Services.Momo
{
    public interface IMomoService
    {
        // Hàm này để gửi yêu cầu tạo thanh toán sang MoMo
        Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfoModel model);

        // Hàm này để xử lý dữ liệu MoMo trả về sau khi khách thanh toán xong
        MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
    }
}


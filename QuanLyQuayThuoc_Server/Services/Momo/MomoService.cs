using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using QuanLyQuayThuoc.Models.Momo;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyQuayThuoc.Services.Momo
{
    public class MomoService : IMomoService
    {
        private readonly IOptions<MomoOptionModel> _options;

        public MomoService(IOptions<MomoOptionModel> options)
        {
            _options = options;
        }

        public async Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfoModel model)
        {
            // 1. Khởi tạo các tham số
            string requestId = DateTime.UtcNow.Ticks.ToString();
            string orderId = model.OrderId;

            // Vì model.Amount đã là long, lấy trực tiếp luôn:
            long amountLong = model.Amount;

            // Sử dụng UserType để làm extraData (để phân biệt loại user khi redirect)
            // Nếu UserType null thì mặc định là "KhachHang"
            string extraData = model.UserType ?? "KhachHang";
            string orderInfo = model.OrderInfo ?? "Thanh toan don hang";

            // 2. Tạo chuỗi ký (Raw Hash) - Phải đúng thứ tự bảng chữ cái của Key
            string rawData =
                $"accessKey={_options.Value.AccessKey}" +
                $"&amount={amountLong}" +
                $"&extraData={extraData}" +
                $"&ipnUrl={_options.Value.NotifyUrl}" +
                $"&orderId={orderId}" +
                $"&orderInfo={orderInfo}" +
                $"&partnerCode={_options.Value.PartnerCode}" +
                $"&redirectUrl={_options.Value.ReturnUrl}" +
                $"&requestId={requestId}" +
                $"&requestType=captureWallet";

            string signature = GenerateSignature(rawData, _options.Value.SecretKey);

            // 3. Tạo Object gửi sang API MoMo
            var requestData = new
            {
                partnerCode = _options.Value.PartnerCode,
                requestId = requestId,
                amount = amountLong,
                orderId = orderId,
                orderInfo = orderInfo,
                redirectUrl = _options.Value.ReturnUrl,
                ipnUrl = _options.Value.NotifyUrl,
                requestType = "captureWallet",
                extraData = extraData,
                signature = signature,
                lang = "vi"
            };

            // 4. Thực thi gửi request
            using (var client = new HttpClient())
            {
                var requestContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(_options.Value.MomoApiUrl, requestContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(responseContent)
                       ?? new MomoCreatePaymentResponseModel();
            }
        }

        public MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection)
        {
            // Đọc các tham số MoMo trả về trên URL
            return new MomoExecuteResponseModel
            {
                OrderId = collection["orderId"],
                Amount = collection["amount"],
                OrderInfo = collection["orderInfo"],
                ResultCode = collection["resultCode"],
                Message = collection["message"],
                LocalMessage = collection["localMessage"]
            };
        }

        // Hàm băm chữ ký SHA256
        private string GenerateSignature(string rawData, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(rawData);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(messageBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
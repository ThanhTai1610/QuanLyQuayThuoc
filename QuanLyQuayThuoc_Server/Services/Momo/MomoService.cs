using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using QuanLyQuayThuoc.Models.Momo;
using QuanLyQuayThuoc.Models.Momo;
using RestSharp;
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
            // Đảm bảo OrderId không quá dài vàRequestId phải giống OrderId
            model.OrderId = DateTime.UtcNow.Ticks.ToString();
            model.OrderInfo = "Thanh toan don thuoc ma " + model.OrderId;

            // Ép kiểu số tiền thành chuỗi số nguyên để tránh sai lệch chữ ký
            var amountString = ((long)model.Amount).ToString();
            var extraData = "";

            // Chuỗi rawData PHẢI sắp xếp theo thứ tự bảng chữ cái A-Z của Key
            var rawData =
                $"accessKey={_options.Value.AccessKey}&" +
                $"amount={amountString}&" +
                $"extraData={extraData}&" +
                $"ipnUrl={_options.Value.NotifyUrl}&" +
                $"orderId={model.OrderId}&" +
                $"orderInfo={model.OrderInfo}&" +
                $"partnerCode={_options.Value.PartnerCode}&" +
                $"redirectUrl={_options.Value.ReturnUrl}&" +
                $"requestId={model.OrderId}&" +
                $"requestType={_options.Value.RequestType}";

            var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);

            var client = new RestClient(_options.Value.MomoApiUrl);
            var request = new RestRequest("", Method.Post);
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");

            // Các trường trong Object này phải khớp hoàn toàn với chuỗi rawData ở trên
            var requestData = new
            {
                partnerCode = _options.Value.PartnerCode,
                partnerName = "Test Store", // Thêm tên cửa hàng
                requestId = model.OrderId,
                amount = amountString, // Dùng chuỗi số nguyên
                orderId = model.OrderId,
                orderInfo = model.OrderInfo,
                redirectUrl = _options.Value.ReturnUrl,
                ipnUrl = _options.Value.NotifyUrl,
                requestType = _options.Value.RequestType,
                extraData = extraData,
                signature = signature,
                lang = "vi"
            };

            request.AddJsonBody(requestData);
            var response = await client.ExecuteAsync(request);

            // Lưu ý: Nếu response.Content bị lỗi, hãy kiểm tra log tại đây
            return JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(response.Content);
        }

        public MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection)
        {
            // MoMo trả về các tham số này trên URL sau khi thanh toán 
            var amount = collection["amount"];
            var orderId = collection["orderId"];
            var orderInfo = collection["orderInfo"];
            var resultCode = collection["resultCode"]; // 0 là thành công [cite: 14]

            return new MomoExecuteResponseModel()
            {
                Amount = amount,
                OrderId = orderId,
                OrderInfo = orderInfo,
                ResultCode = resultCode // Bạn cần thêm property này vào Model tương ứng [cite: 14, 17]
            };
        }

        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] hashBytes;

            using (var hmac = new HMACSHA256(keyBytes))
            {
                hashBytes = hmac.ComputeHash(messageBytes);
            }

            var hashString = new StringBuilder();
            foreach (var x in hashBytes)
            {
                hashString.Append(x.ToString("x2"));
            }

            return hashString.ToString();
        }
    }
}
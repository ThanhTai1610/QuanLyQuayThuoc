using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using QuanLyQuayThuoc.Models.Momo;
using RestSharp;
using System.Security.Cryptography;
using System.Text;
using System.Net.Security; // Thêm dòng này

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
            var amountString = ((long)model.Amount).ToString();
            var requestId = Guid.NewGuid().ToString();
            var extraData = "";

            // 1. Tạo chữ ký (rawData phải đúng thứ tự A-Z)
            // Đảm bảo thứ tự ĐÚNG như thế này (A-Z):
            var rawData =
                $"accessKey={_options.Value.AccessKey}&" +
                $"amount={amountString}&" +
                $"extraData={extraData}&" +
                $"ipnUrl={_options.Value.NotifyUrl}&" +
                $"orderId={model.OrderId}&" +
                $"orderInfo={model.OrderInfo}&" +
                $"partnerCode={_options.Value.PartnerCode}&" +
                $"redirectUrl={_options.Value.ReturnUrl}&" +
                $"requestId={requestId}&" +
                $"requestType={_options.Value.RequestType}";

            var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);

            // 2. Định nghĩa dữ liệu gửi đi
            var requestData = new
            {
                partnerCode = _options.Value.PartnerCode,
                partnerName = "Pharmative Store",
                storeId = "Pharmative_Store",
                requestId = requestId,
                amount = amountString,
                orderId = model.OrderId,
                orderInfo = model.OrderInfo,
                redirectUrl = _options.Value.ReturnUrl,
                ipnUrl = _options.Value.NotifyUrl,
                requestType = _options.Value.RequestType,
                extraData = extraData,
                signature = signature,
                lang = "vi"
            };

            // 3. Cấu hình RestClient BỎ QUA LỖI SSL (Giải quyết Unknown Error)
            var options = new RestClientOptions(_options.Value.MomoApiUrl)
            {
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };
            var client = new RestClient(options);

            var request = new RestRequest("");
            request.Method = Method.Post;
            request.AddJsonBody(requestData);

            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                // Log ra console để Tài dễ debug
                Console.WriteLine($"--- MOMO DEBUG ---");
                Console.WriteLine($"Status: {response.StatusCode}");
                Console.WriteLine($"Error: {response.ErrorMessage}");
                Console.WriteLine($"Content: {response.Content}");

                return new MomoCreatePaymentResponseModel
                {
                    Message = "MoMo từ chối yêu cầu: " + (response.ErrorMessage ?? "Vui lòng kiểm tra lại Key/Số tiền"),
                    ResultCode = (int)response.StatusCode
                };
            }

            return JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(response.Content);
        }

        public MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection)
        {
            return new MomoExecuteResponseModel()
            {
                Amount = collection["amount"],
                OrderId = collection["orderId"],
                OrderInfo = collection["orderInfo"],
                ResultCode = collection["resultCode"],
                Message = collection["message"]
            };
        }

        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(messageBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
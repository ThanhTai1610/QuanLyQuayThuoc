using Microsoft.OpenApi.Services;
using QuanLyQuayThuoc.Services.Interfaces;

public class ChatbotService : IChatBotService
{
    private readonly HttpClient _http;
    private readonly List<string> _apiKeys;
    private static int _currentKeyIndex = 0;
    private static readonly object _lock = new object();

    // Thêm danh sách model dự phòng
    // 1. Chỉ nên tập trung vào các model mạnh nhất và phổ biến nhất
    private static readonly string[] _models =
{
    "models/gemini-2.5-flash",
    "models/gemini-2.0-flash",
    "models/gemini-pro-latest"
};


    public ChatbotService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _http.BaseAddress = new Uri("https://generativelanguage.googleapis.com/");

        // Sửa "Gemini:ApiKeys" thành "GeminiAI:ApiKeys" cho đúng với file JSON của bạn
        var rawKeys = config.GetSection("GeminiAI:ApiKeys").Get<List<string>>();

        _apiKeys = rawKeys ?? new List<string>();

        // Kiểm tra log ở đây, nếu số key vẫn là 0 thì do file JSON chưa lưu hoặc sai tên
        Console.WriteLine($"[DEBUG] Số key đọc được: {_apiKeys.Count}");

        if (_apiKeys.Count == 0)
            throw new Exception("Chưa cấu hình Gemini ApiKeys trong appsettings.json!");
    }

    private string LayKeyHienTai(bool chuyenKeyTiepTheo = false)
    {
        lock (_lock)
        {
            if (chuyenKeyTiepTheo)
            {
                _currentKeyIndex = (_currentKeyIndex + 1) % _apiKeys.Count;
                Console.WriteLine($"[CẢNH BÁO] Đã đổi sang API Key số {_currentKeyIndex + 1}");
            }
            return _apiKeys[_currentKeyIndex];
        }
    }

    public async Task<string> GenerateAsync(string prompt)
    {
        var requestBody = new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } }
        };

        // Thử từng model, mỗi model thử tất cả các key
        foreach (var model in _models)
        {
            int soLanThuLai = 0;
            int toiDaSoLanThu = _apiKeys.Count;

            while (soLanThuLai < toiDaSoLanThu)
            {
                string currentKey = LayKeyHienTai();
                // Sử dụng v1beta cho các dòng model 1.5 và 2.0
                string requestUrl = $"v1beta/{model}:generateContent?key={currentKey}";
                Console.WriteLine($"Đang thử model: {model}, key: ...{currentKey[^4..]}");

                var response = await _http.PostAsJsonAsync(requestUrl, requestBody);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Thành công với model: {model}");
                    return await response.Content.ReadAsStringAsync();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    Console.WriteLine($"Key ...{currentKey[^4..]} + model {model} bị quá tải. Xoay key...");
                    LayKeyHienTai(chuyenKeyTiepTheo: true);
                    soLanThuLai++;
                    await Task.Delay(1000);
                    continue;
                }

                // Lỗi khác (400, 404...) -> thử model tiếp theo luôn
                var errorBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Model {model} lỗi {response.StatusCode}: {errorBody}");
                break;
            }
        }

        throw new Exception("Tất cả API Key và Model đều đã vượt quá giới hạn. Vui lòng thử lại sau.");
    }
}
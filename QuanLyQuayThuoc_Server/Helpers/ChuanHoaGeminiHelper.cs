using System.Text.Json;

namespace QuanLyQuayThuoc.Helpers
{
    public static class ChuanHoaGeminiHelper
    {
        public static string LayText(string rawJson)
        {
            using var doc = JsonDocument.Parse(rawJson);
            return doc.RootElement
                      .GetProperty("candidates")[0]
                      .GetProperty("content")
                      .GetProperty("parts")[0]
                      .GetProperty("text")
                      .GetString() ?? "";
        }
    }
}

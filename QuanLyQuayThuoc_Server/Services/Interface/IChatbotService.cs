namespace QuanLyQuayThuoc.Services.Interfaces
{
    public interface IChatBotService
    {
        Task<string> GenerateAsync(string prompt);
    }
}
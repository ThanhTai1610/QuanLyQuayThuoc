using QuanLyQuayThuoc.Services.Models;

namespace QuanLyQuayThuoc.Services.Interfaces
{
    public interface IEmailQueueService
    {
        ValueTask QueueEmailAsync(EmailQueueItem item, CancellationToken cancellationToken = default);
    }
}

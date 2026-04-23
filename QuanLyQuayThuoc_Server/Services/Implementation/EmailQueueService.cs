using System.Threading.Channels;
using QuanLyQuayThuoc.Services.Interfaces;
using QuanLyQuayThuoc.Services.Models;

namespace QuanLyQuayThuoc.Services.Implementation
{
    public class EmailQueueService : IEmailQueueService
    {
        private readonly Channel<EmailQueueItem> _queue;

        public EmailQueueService()
        {
            _queue = Channel.CreateUnbounded<EmailQueueItem>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            });
        }

        public ValueTask QueueEmailAsync(EmailQueueItem item, CancellationToken cancellationToken = default)
        {
            return _queue.Writer.WriteAsync(item, cancellationToken);
        }

        public IAsyncEnumerable<EmailQueueItem> ReadAllAsync(CancellationToken cancellationToken)
        {
            return _queue.Reader.ReadAllAsync(cancellationToken);
        }
    }
}

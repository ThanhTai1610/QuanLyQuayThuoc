using QuanLyQuayThuoc.Helpers;

namespace QuanLyQuayThuoc.Services.Implementation
{
    public class EmailBackgroundService : BackgroundService
    {
        private readonly EmailQueueService _emailQueueService;
        private readonly ILogger<EmailBackgroundService> _logger;

        public EmailBackgroundService(
            EmailQueueService emailQueueService,
            ILogger<EmailBackgroundService> logger)
        {
            _emailQueueService = emailQueueService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var email in _emailQueueService.ReadAllAsync(stoppingToken))
            {
                try
                {
                    var isSent = await EmailHelper.SendEmailWithAttachmentsAsync(
                        email.ToEmail,
                        email.Subject,
                        email.Body,
                        email.Attachments);

                    if (!isSent)
                    {
                        _logger.LogWarning(
                            "Gửi email nền thất bại tới {ToEmail} với tiêu đề {Subject}",
                            email.ToEmail,
                            email.Subject);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi xử lý email nền tới {ToEmail}", email.ToEmail);
                }
            }
        }
    }
}

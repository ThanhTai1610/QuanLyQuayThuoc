namespace QuanLyQuayThuoc.Services.Models
{
    public class EmailQueueItem
    {
        public string ToEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public List<EmailAttachmentData> Attachments { get; set; } = new();
    }

    public class EmailAttachmentData
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = "application/octet-stream";
        public byte[] Content { get; set; } = Array.Empty<byte>();
    }
}

using System.Net;
using System.Net.Mail;
using QuanLyQuayThuoc.Services.Models;

namespace QuanLyQuayThuoc.Helpers
{
    public static class EmailHelper
    {
        // HÀM 1: Gửi văn bản bình thường (Dùng cho OTP, Quên mật khẩu)
        public static async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            return await SendEmailWithAttachmentsAsync(toEmail, subject, body, (List<EmailAttachmentData>?)null);
        }

        // HÀM 2: Gửi có đính kèm trực tiếp từ request (giữ để tương thích code cũ)
        public static async Task<bool> SendEmailWithAttachmentsAsync(string toEmail, string subject, string body, List<IFormFile>? attachments = null)
        {
            List<EmailAttachmentData>? attachmentData = null;

            if (attachments != null)
            {
                attachmentData = new List<EmailAttachmentData>();

                foreach (var file in attachments)
                {
                    await using var stream = file.OpenReadStream();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);

                    attachmentData.Add(new EmailAttachmentData
                    {
                        FileName = file.FileName,
                        ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
                        Content = memoryStream.ToArray()
                    });
                }
            }

            return await SendEmailWithAttachmentsAsync(toEmail, subject, body, attachmentData);
        }

        // HÀM 3: Gửi với file đã copy sẵn vào bộ nhớ, phù hợp cho background worker
        public static async Task<bool> SendEmailWithAttachmentsAsync(string toEmail, string subject, string body, List<EmailAttachmentData>? attachments)
        {
            try
            {
                using var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("taiptpk04158@gmail.com", "esvu qupo qryq peop"),
                    EnableSsl = true
                };

                using var message = new MailMessage();
                message.From = new MailAddress("taiptpk04158@gmail.com", "Nhà Thuốc Pharmative");
                message.To.Add(toEmail);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                if (attachments != null)
                {
                    foreach (var file in attachments)
                    {
                        var stream = new MemoryStream(file.Content, writable: false);
                        message.Attachments.Add(new Attachment(stream, file.FileName, file.ContentType));
                    }
                }

                await smtp.SendMailAsync(message);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

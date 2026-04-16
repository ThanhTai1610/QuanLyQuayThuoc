using System.Net;
using System.Net.Mail;

namespace QuanLyQuayThuoc.Helpers
{
    public static class EmailHelper
    {
        // HÀM 1: Gửi văn bản bình thường (Dùng cho OTP, Quên mật khẩu)
        public static async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            return await SendEmailWithAttachmentsAsync(toEmail, subject, body, null);
        }

        // HÀM 2: Gửi có đính kèm (Dùng cho Gửi đơn thuốc tư vấn)
        public static async Task<bool> SendEmailWithAttachmentsAsync(string toEmail, string subject, string body, List<IFormFile>? attachments = null)
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
                        var stream = file.OpenReadStream();
                        message.Attachments.Add(new Attachment(stream, file.FileName, file.ContentType));
                    }
                }

                await smtp.SendMailAsync(message);
                return true;
            }
            catch { return false; }
        }
    }
}
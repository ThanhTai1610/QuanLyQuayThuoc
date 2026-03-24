using BCrypt.Net;

namespace QuanLyQuayThuoc.Helpers
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            // Gọi trực tiếp BCrypt.HashPassword
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(hashedPassword)) return false;

            try
            {
                // Gọi trực tiếp BCrypt.Verify
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch
            {
                return false;
            }
        }
    }
}
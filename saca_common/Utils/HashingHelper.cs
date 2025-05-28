using System.Security.Cryptography;

namespace SACA_Common.Utils
{
    public static class HashingHelper
    {
        private const int SaltSize = 16; // Kích thước của salt (16 bytes)
        private const int HashSize = 32; // Kích thước của hash (SHA-256 sử dụng 256 bits = 32 bytes)
        private const int Iterations = 10000; // Số lần lặp
        public static string GenerateSalt()
        {
            var saltBytes = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            var salt = Convert.ToBase64String(saltBytes);
            return salt;
        }
        public static string HashPassword(string password, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);
            var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            // Kết hợp salt và hash lại với nhau
            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(saltBytes, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            // Chuyển đổi sang dạng Base64 để lưu trữ trong cơ sở dữ liệu
            string savedPasswordHash = Convert.ToBase64String(hashBytes);

            return savedPasswordHash;
        }
        public static bool VerifyPassword(string password, string savedPasswordHash, string salt)
        {
            // Lấy salt từ savedPasswordHash
            byte[] saltBytes = Convert.FromBase64String(salt);

            // Tạo hash từ mật khẩu nhập vào kèm theo salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            // Lấy hash từ savedPasswordHash
            byte[] savedHashBytes = Convert.FromBase64String(savedPasswordHash);

            // So sánh hash được tạo ra với hash trong savedPasswordHash
            for (int i = 0; i < HashSize; i++)
            {
                if (savedHashBytes[i + SaltSize] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }

    }
}

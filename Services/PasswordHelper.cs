using System;
using System.Security.Cryptography;
using System.Text;

namespace TipaDiplom.Services
{
    public static class PasswordHelper
    {
        // Генерация случайной соли
        public static string GenerateSalt()
        {
            byte[] saltBytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        // Хеширование пароля с солью
        public static string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                string saltedPassword = password + salt;
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(bytes);
            }
        }

        // Проверка пароля
        public static bool VerifyPassword(string password, string hash, string salt)
        {
            string computedHash = HashPassword(password, salt);
            return computedHash == hash;
        }

        // Валидация пароля
        public static (bool isValid, string message) ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return (false, "Пароль не может быть пустым");

            if (password.Length < 6)
                return (false, "Пароль должен содержать минимум 6 символов");

            bool hasUpper = false;
            bool hasLower = false;
            bool hasDigit = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                if (char.IsLower(c)) hasLower = true;
                if (char.IsDigit(c)) hasDigit = true;
            }

            if (!hasUpper)
                return (false, "Пароль должен содержать хотя бы одну заглавную букву");
            if (!hasLower)
                return (false, "Пароль должен содержать хотя бы одну строчную букву");
            if (!hasDigit)
                return (false, "Пароль должен содержать хотя бы одну цифру");

            return (true, "OK");
        }

        // Валидация email
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Простая проверка формата
                int atIndex = email.IndexOf('@');
                if (atIndex <= 0) return false;

                int dotIndex = email.LastIndexOf('.');
                if (dotIndex <= atIndex + 1) return false;

                if (dotIndex >= email.Length - 1) return false;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
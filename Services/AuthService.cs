using TipaDiplom.Models;
using System;
using System.Data.SqlClient;
using TipaDiplom.Services;

namespace TipaDiplom.Services
{
    public class AuthService
    {
        private readonly DatabaseService _db;

        public static User CurrentUser { get; private set; }

        public AuthService()
        {
            _db = new DatabaseService();
        }

        public (bool success, string message) Register(
            string username, string email, string password,
            string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
                return (false, "Имя пользователя должно содержать минимум 3 символа");

            if (!PasswordHelper.IsValidEmail(email))
                return (false, "Некорректный email адрес");

            var (isValidPass, passMessage) = PasswordHelper.ValidatePassword(password);
            if (!isValidPass)
                return (false, passMessage);

            try
            {
                using (var connection = _db.GetConnection())
                {
                    connection.Open();

                    string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                    using (var checkCmd = new SqlCommand(checkQuery, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@Username", username);
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                            return (false, "Пользователь с таким именем уже существует");
                    }

                    string checkEmailQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                    using (var checkCmd = new SqlCommand(checkEmailQuery, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", email);
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                            return (false, "Пользователь с таким email уже существует");
                    }

                    string salt = PasswordHelper.GenerateSalt();
                    string hash = PasswordHelper.HashPassword(password, salt);

                    string insertQuery = @"
                        INSERT INTO Users (Username, Email, PasswordHash, PasswordSalt, FirstName, LastName)
                        VALUES (@Username, @Email, @PasswordHash, @PasswordSalt, @FirstName, @LastName)";

                    using (var insertCmd = new SqlCommand(insertQuery, connection))
                    {
                        insertCmd.Parameters.AddWithValue("@Username", username);
                        insertCmd.Parameters.AddWithValue("@Email", email);
                        insertCmd.Parameters.AddWithValue("@PasswordHash", hash);
                        insertCmd.Parameters.AddWithValue("@PasswordSalt", salt);
                        insertCmd.Parameters.AddWithValue("@FirstName",
                            string.IsNullOrEmpty(firstName) ? (object)DBNull.Value : firstName);
                        insertCmd.Parameters.AddWithValue("@LastName",
                            string.IsNullOrEmpty(lastName) ? (object)DBNull.Value : lastName);

                        insertCmd.ExecuteNonQuery();
                    }

                    return (true, "Регистрация успешна! Теперь вы можете войти.");
                }
            }
            catch (Exception ex)
            {
                return (false, "Ошибка при регистрации: " + ex.Message);
            }
        }

        public (bool success, string message) Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return (false, "Введите имя пользователя и пароль");

            try
            {
                using (var connection = _db.GetConnection())
                {
                    connection.Open();

                    string query = @"
                        SELECT Id, Username, Email, PasswordHash, PasswordSalt, 
                               FirstName, LastName, CreatedAt, LastLoginAt, IsActive
                        FROM Users 
                        WHERE Username = @Username OR Email = @Username";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);

                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                LogLogin(connection, 0, false);
                                return (false, "Пользователь не найден");
                            }

                            string storedHash = reader["PasswordHash"].ToString();
                            string storedSalt = reader["PasswordSalt"].ToString();
                            bool isActive = (bool)reader["IsActive"];

                            if (!isActive)
                                return (false, "Аккаунт деактивирован");

                            if (!PasswordHelper.VerifyPassword(password, storedHash, storedSalt))
                            {
                                int userId = (int)reader["Id"];
                                reader.Close();
                                LogLogin(connection, userId, false);
                                return (false, "Неверный пароль");
                            }

                            CurrentUser = new User
                            {
                                Id = (int)reader["Id"],
                                Username = reader["Username"].ToString(),
                                Email = reader["Email"].ToString(),
                                FirstName = reader["FirstName"] != DBNull.Value
                                    ? reader["FirstName"].ToString() : "",
                                LastName = reader["LastName"] != DBNull.Value
                                    ? reader["LastName"].ToString() : "",
                                CreatedAt = (DateTime)reader["CreatedAt"],
                                LastLoginAt = reader["LastLoginAt"] != DBNull.Value
                                    ? (DateTime?)reader["LastLoginAt"] : null,
                                IsActive = isActive
                            };

                            reader.Close();
                            UpdateLastLogin(connection, CurrentUser.Id);
                            LogLogin(connection, CurrentUser.Id, true);

                            return (true, $"Добро пожаловать, {CurrentUser.DisplayName}!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, "Ошибка подключения к серверу: " + ex.Message);
            }
        }

        public static void Logout()
        {
            CurrentUser = null;
        }

        private void UpdateLastLogin(SqlConnection connection, int userId)
        {
            try
            {
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();

                string query = "UPDATE Users SET LastLoginAt = GETDATE() WHERE Id = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", userId);
                    command.ExecuteNonQuery();
                }
            }
            catch { /* игнорируем ошибку логирования */ }
        }

        private void LogLogin(SqlConnection connection, int userId, bool isSuccessful)
        {
            try
            {
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();

                if (userId == 0) return;

                string query = @"
                    INSERT INTO LoginHistory (UserId, IsSuccessful)
                    VALUES (@UserId, @IsSuccessful)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@IsSuccessful", isSuccessful);
                    command.ExecuteNonQuery();
                }
            }
            catch { /* игнорируем ошибку логирования */ }
        }

        public void SaveResult(string moduleName, string taskDescription, string result)
        {
            if (CurrentUser == null) return;

            try
            {
                using (var connection = _db.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        INSERT INTO UserResults (UserId, ModuleName, TaskDescription, Result)
                        VALUES (@UserId, @ModuleName, @TaskDescription, @Result)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", CurrentUser.Id);
                        command.Parameters.AddWithValue("@ModuleName", moduleName);
                        command.Parameters.AddWithValue("@TaskDescription", taskDescription);
                        command.Parameters.AddWithValue("@Result", result);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch { /* игнорируем */ }
        }

        public (bool success, string message) ChangePassword(
            string oldPassword, string newPassword)
        {
            if (CurrentUser == null)
                return (false, "Пользователь не авторизован");

            var (isValid, validMessage) = PasswordHelper.ValidatePassword(newPassword);
            if (!isValid) return (false, validMessage);

            try
            {
                using (var connection = _db.GetConnection())
                {
                    connection.Open();

                    // Получаем текущий хеш и соль
                    string getQuery = "SELECT PasswordHash, PasswordSalt FROM Users WHERE Id = @Id";
                    string currentHash, currentSalt;

                    using (var cmd = new SqlCommand(getQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", CurrentUser.Id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            currentHash = reader["PasswordHash"].ToString();
                            currentSalt = reader["PasswordSalt"].ToString();
                        }
                    }

                    if (!PasswordHelper.VerifyPassword(oldPassword, currentHash, currentSalt))
                        return (false, "Неверный текущий пароль");

                    string newSalt = PasswordHelper.GenerateSalt();
                    string newHash = PasswordHelper.HashPassword(newPassword, newSalt);

                    string updateQuery = @"
                        UPDATE Users SET PasswordHash = @Hash, PasswordSalt = @Salt 
                        WHERE Id = @Id";

                    using (var cmd = new SqlCommand(updateQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@Hash", newHash);
                        cmd.Parameters.AddWithValue("@Salt", newSalt);
                        cmd.Parameters.AddWithValue("@Id", CurrentUser.Id);
                        cmd.ExecuteNonQuery();
                    }

                    return (true, "Пароль успешно изменён");
                }
            }
            catch (Exception ex)
            {
                return (false, "Ошибка: " + ex.Message);
            }
        }
    }
}
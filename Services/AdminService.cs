
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TipaDiplom.Models;
using TipaDiplom.Services;

namespace TipaDiplom.Services
{
    public class AdminService
    {
        private readonly DatabaseService _db;

        // Логин и пароль преподавателя
        public const string AdminLogin = "admin";
        public const string AdminPassword = "admin";

        public AdminService()
        {
            _db = new DatabaseService();
        }

        // Проверка входа преподавателя
        public bool IsAdmin(string login, string password)
        {
            return login == AdminLogin && password == AdminPassword;
        }



        // Получить все результаты всех студентов
        public List<StudentResult> GetAllResults(
            string filterCategory = "", string filterUser = "")
        {
            var results = new List<StudentResult>();
            try
            {
                using (var connection = _db.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        SELECT r.Id, u.Username, u.FirstName, 
                               u.LastName, u.Email,
                               r.ModuleName, r.TaskDescription, 
                               r.Result, r.CreatedAt
                        FROM UserResults r
                        JOIN Users u ON r.UserId = u.Id
                        WHERE 1=1";

                    if (!string.IsNullOrEmpty(filterCategory))
                        query += " AND r.ModuleName LIKE @Category";
                    if (!string.IsNullOrEmpty(filterUser))
                        query += " AND (u.Username LIKE @User " +
                                 "OR u.FirstName LIKE @User " +
                                 "OR u.LastName LIKE @User)";

                    query += " ORDER BY r.CreatedAt DESC";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(filterCategory))
                            cmd.Parameters.AddWithValue(
                                "@Category", "%" + filterCategory + "%");
                        if (!string.IsNullOrEmpty(filterUser))
                            cmd.Parameters.AddWithValue(
                                "@User", "%" + filterUser + "%");

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(new StudentResult
                                {
                                    Id = (int)reader["Id"],
                                    Username = reader["Username"].ToString(),
                                    FirstName = reader["FirstName"] != DBNull.Value
                                        ? reader["FirstName"].ToString() : "",
                                    LastName = reader["LastName"] != DBNull.Value
                                        ? reader["LastName"].ToString() : "",
                                    Email = reader["Email"].ToString(),
                                    ModuleName = reader["ModuleName"].ToString(),
                                    TaskDescription = reader["TaskDescription"].ToString(),
                                    Result = reader["Result"].ToString(),
                                    CreatedAt = (DateTime)reader["CreatedAt"]
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения результатов: " + ex.Message);
            }
            return results;
        }

        // Статистика по студентам
        public List<StudentStats> GetStudentStats()
        {
            var stats = new List<StudentStats>();
            try
            {
                using (var connection = _db.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        SELECT u.Username, u.FirstName, u.LastName, 
                               u.Email, u.CreatedAt,
                               COUNT(r.Id) as TotalTests,
                               MAX(r.CreatedAt) as LastActivity
                        FROM Users u
                        LEFT JOIN UserResults r ON u.Id = r.UserId
                        GROUP BY u.Id, u.Username, u.FirstName, 
                                 u.LastName, u.Email, u.CreatedAt
                        ORDER BY LastActivity DESC";

                    using (var cmd = new SqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string fn = reader["FirstName"] != DBNull.Value
                                ? reader["FirstName"].ToString() : "";
                            string ln = reader["LastName"] != DBNull.Value
                                ? reader["LastName"].ToString() : "";

                            stats.Add(new StudentStats
                            {
                                Username = reader["Username"].ToString(),
                                DisplayName = string.IsNullOrEmpty(fn + ln)
                                    ? reader["Username"].ToString()
                                    : $"{fn} {ln}".Trim(),
                                Email = reader["Email"].ToString(),
                                TotalTests = (int)reader["TotalTests"],
                                LastActivity = reader["LastActivity"] != DBNull.Value
                                    ? (DateTime?)reader["LastActivity"] : null,
                                RegisteredAt = (DateTime)reader["CreatedAt"]
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения статистики: " + ex.Message);
            }
            return stats;
        }

        // Удалить результат
        public void DeleteResult(int resultId)
        {
            using (var connection = _db.GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM UserResults WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", resultId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

     

        // Получить все лекции
        public List<LectureItem> GetAllLectures()
        {
            var items = new List<LectureItem>();
            try
            {
                using (var connection = _db.GetConnection())
                {
                    connection.Open();
                    string query = @"
                SELECT Id, Number, Title, Content, 
                       CreatedAt, UpdatedAt, IsActive
                FROM Lectures
                ORDER BY Number ASC";

                    using (var cmd = new SqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new LectureItem
                            {
                                Id = (int)reader["Id"],
                                Number = (int)reader["Number"],
                                Title = reader["Title"].ToString(),
                                Content = reader["Content"].ToString(),
                                CreatedAt = (DateTime)reader["CreatedAt"],
                                UpdatedAt = reader["UpdatedAt"] != DBNull.Value
                                    ? (DateTime?)reader["UpdatedAt"] : null,
                                IsActive = (bool)reader["IsActive"]
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения лекций: " + ex.Message);
            }
            return items;
        }

        // Добавить лекцию
        public void AddLecture(LectureItem item)
        {
            using (var connection = _db.GetConnection())
            {
                connection.Open();
                string query = @"
            INSERT INTO Lectures 
            (Number, Title, Content)
            VALUES (@Number, @Title, @Content)";

                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Number", item.Number);
                    cmd.Parameters.AddWithValue("@Title", item.Title);
                    cmd.Parameters.AddWithValue("@Content", item.Content);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Обновить лекцию
        public void UpdateLecture(LectureItem item)
        {
            using (var connection = _db.GetConnection())
            {
                connection.Open();
                string query = @"
            UPDATE Lectures 
            SET Number = @Number,
                Title = @Title, 
                Content = @Content,
                UpdatedAt = GETDATE(),
                IsActive = @IsActive
            WHERE Id = @Id";

                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Number", item.Number);
                    cmd.Parameters.AddWithValue("@Title", item.Title);
                    cmd.Parameters.AddWithValue("@Content", item.Content);
                    cmd.Parameters.AddWithValue("@IsActive", item.IsActive);
                    cmd.Parameters.AddWithValue("@Id", item.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Удалить лекцию
        public void DeleteLecture(int id)
        {
            using (var connection = _db.GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM Lectures WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Получить следующий номер лекции
        public int GetNextLectureNumber()
        {
            using (var connection = _db.GetConnection())
            {
                connection.Open();
                string query = "SELECT ISNULL(MAX(Number), 0) + 1 FROM Lectures";
                using (var cmd = new SqlCommand(query, connection))
                {
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        // Получить все вопросы из БД
        public List<DbTestQuestion> GetAllDbQuestions()
        {
            var questions = new List<DbTestQuestion>();
            try
            {
                using (var connection = _db.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        SELECT Id, Category, QuestionText, 
                               OptionA, OptionB, OptionC, OptionD,
                               CorrectOption, Explanation, 
                               Difficulty, CreatedAt, IsActive
                        FROM TestQuestions
                        ORDER BY Category, CreatedAt DESC";

                    using (var cmd = new SqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            questions.Add(new DbTestQuestion
                            {
                                Id = (int)reader["Id"],
                                Category = reader["Category"].ToString(),
                                QuestionText = reader["QuestionText"].ToString(),
                                OptionA = reader["OptionA"].ToString(),
                                OptionB = reader["OptionB"].ToString(),
                                OptionC = reader["OptionC"].ToString(),
                                OptionD = reader["OptionD"].ToString(),
                                CorrectOption = reader["CorrectOption"].ToString(),
                                Explanation = reader["Explanation"] != DBNull.Value
                                    ? reader["Explanation"].ToString() : "",
                                Difficulty = (int)reader["Difficulty"],
                                CreatedAt = (DateTime)reader["CreatedAt"],
                                IsActive = (bool)reader["IsActive"]
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения вопросов: " + ex.Message);
            }
            return questions;
        }

        // Добавить вопрос
        public void AddQuestion(DbTestQuestion question)
        {
            using (var connection = _db.GetConnection())
            {
                connection.Open();
                string query = @"
                    INSERT INTO TestQuestions 
                    (Category, QuestionText, OptionA, OptionB, 
                     OptionC, OptionD, CorrectOption, 
                     Explanation, Difficulty)
                    VALUES 
                    (@Category, @QuestionText, @OptionA, @OptionB,
                     @OptionC, @OptionD, @CorrectOption,
                     @Explanation, @Difficulty)";

                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Category", question.Category);
                    cmd.Parameters.AddWithValue("@QuestionText", question.QuestionText);
                    cmd.Parameters.AddWithValue("@OptionA", question.OptionA);
                    cmd.Parameters.AddWithValue("@OptionB", question.OptionB);
                    cmd.Parameters.AddWithValue("@OptionC", question.OptionC);
                    cmd.Parameters.AddWithValue("@OptionD", question.OptionD);
                    cmd.Parameters.AddWithValue("@CorrectOption", question.CorrectOption);
                    cmd.Parameters.AddWithValue("@Explanation",
                        string.IsNullOrEmpty(question.Explanation)
                            ? (object)DBNull.Value : question.Explanation);
                    cmd.Parameters.AddWithValue("@Difficulty", question.Difficulty);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Обновить вопрос
        public void UpdateQuestion(DbTestQuestion question)
        {
            using (var connection = _db.GetConnection())
            {
                connection.Open();
                string query = @"
                    UPDATE TestQuestions SET
                        Category = @Category,
                        QuestionText = @QuestionText,
                        OptionA = @OptionA,
                        OptionB = @OptionB,
                        OptionC = @OptionC,
                        OptionD = @OptionD,
                        CorrectOption = @CorrectOption,
                        Explanation = @Explanation,
                        Difficulty = @Difficulty,
                        IsActive = @IsActive
                    WHERE Id = @Id";

                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Category", question.Category);
                    cmd.Parameters.AddWithValue("@QuestionText", question.QuestionText);
                    cmd.Parameters.AddWithValue("@OptionA", question.OptionA);
                    cmd.Parameters.AddWithValue("@OptionB", question.OptionB);
                    cmd.Parameters.AddWithValue("@OptionC", question.OptionC);
                    cmd.Parameters.AddWithValue("@OptionD", question.OptionD);
                    cmd.Parameters.AddWithValue("@CorrectOption", question.CorrectOption);
                    cmd.Parameters.AddWithValue("@Explanation",
                        string.IsNullOrEmpty(question.Explanation)
                            ? (object)DBNull.Value : question.Explanation);
                    cmd.Parameters.AddWithValue("@Difficulty", question.Difficulty);
                    cmd.Parameters.AddWithValue("@IsActive", question.IsActive);
                    cmd.Parameters.AddWithValue("@Id", question.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Удалить вопрос
        public void DeleteQuestion(int id)
        {
            using (var connection = _db.GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM TestQuestions WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Инициализация таблиц для преподавателя
        public void InitializeAdminTables()
        {
            string sql = @"
        -- Создаём таблицу лекций
        IF NOT EXISTS (SELECT * FROM sysobjects 
            WHERE name='Lectures' AND xtype='U')
        BEGIN
            CREATE TABLE Lectures (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                Number INT NOT NULL UNIQUE,
                Title NVARCHAR(200) NOT NULL,
                Content NVARCHAR(MAX) NOT NULL,
                CreatedAt DATETIME2 DEFAULT GETDATE(),
                UpdatedAt DATETIME2,
                IsActive BIT DEFAULT 1
            );

            -- Добавляем начальные лекции
            INSERT INTO Lectures (Number, Title, Content) VALUES
            (1, 'Теория множеств', 
'📐 ТЕОРИЯ МНОЖЕСТВ

Множество — это совокупность различных объектов, рассматриваемых как единое целое.

Основные операции:
• Объединение: A ∪ B
• Пересечение: A ∩ B  
• Разность: A \ B
• Дополнение: A''

Законы де Моргана:
(A ∪ B)'' = A'' ∩ B''
(A ∩ B)'' = A'' ∪ B'''),

            (2, 'Булева алгебра',
'⚡ БУЛЕВА АЛГЕБРА

Основные операции:
• Конъюнкция (И): A ∧ B
• Дизъюнкция (ИЛИ): A ∨ B
• Отрицание (НЕ): ¬A
• Импликация: A → B = ¬A ∨ B

Законы:
• Коммутативность: A ∧ B = B ∧ A
• Ассоциативность: (A ∧ B) ∧ C = A ∧ (B ∧ C)
• Дистрибутивность: A ∧ (B ∨ C) = (A ∧ B) ∨ (A ∧ C)'),

            (3, 'Теория графов',
'🕸️ ТЕОРИЯ ГРАФОВ

Граф G = (V, E), где:
• V — множество вершин
• E — множество рёбер

Виды графов:
• Полный граф Kₙ
• Двудольный граф
• Дерево
• Планарный граф

Алгоритмы:
• BFS (поиск в ширину)
• DFS (поиск в глубину)
• Дейкстра (кратчайший путь)
• Краскал (минимальное остовное дерево)');
        END

        -- Таблица тестовых вопросов
        IF NOT EXISTS (SELECT * FROM sysobjects 
            WHERE name='TestQuestions' AND xtype='U')
        BEGIN
            CREATE TABLE TestQuestions (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                Category NVARCHAR(50) NOT NULL,
                QuestionText NVARCHAR(MAX) NOT NULL,
                OptionA NVARCHAR(500) NOT NULL,
                OptionB NVARCHAR(500) NOT NULL,
                OptionC NVARCHAR(500) NOT NULL,
                OptionD NVARCHAR(500) NOT NULL,
                CorrectOption CHAR(1) NOT NULL,
                Explanation NVARCHAR(MAX),
                Difficulty INT DEFAULT 1,
                CreatedAt DATETIME2 DEFAULT GETDATE(),
                IsActive BIT DEFAULT 1
            );
        END";

            try
            {
                using (var connection = _db.GetConnection())
                {
                    connection.Open();
                    using (var cmd = new SqlCommand(sql, connection))
                        cmd.ExecuteNonQuery();
                }
            }
            catch { }
        }
    }
}
using System;
using System.Data;
using System.Data.SqlClient;

namespace TipaDiplom.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
          

            // Локальный SQL Server 
            _connectionString = @"Server=localhost;Database=DiscreteMathDB;Trusted_Connection=True;";

           
        }

        // Получение подключения
        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // Проверка подключения к БД
        public bool TestConnection(out string errorMessage)
        {
            errorMessage = "";
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        // Инициализация БД (создание таблиц если их нет)
        public void InitializeDatabase()
        {
            string createTables = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
                BEGIN
                    CREATE TABLE Users (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        Username NVARCHAR(50) NOT NULL UNIQUE,
                        Email NVARCHAR(100) NOT NULL UNIQUE,
                        PasswordHash NVARCHAR(256) NOT NULL,
                        PasswordSalt NVARCHAR(256) NOT NULL,
                        FirstName NVARCHAR(50),
                        LastName NVARCHAR(50),
                        CreatedAt DATETIME2 DEFAULT GETDATE(),
                        LastLoginAt DATETIME2,
                        IsActive BIT DEFAULT 1
                    );
                END

                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='LoginHistory' AND xtype='U')
                BEGIN
                    CREATE TABLE LoginHistory (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        UserId INT FOREIGN KEY REFERENCES Users(Id),
                        LoginTime DATETIME2 DEFAULT GETDATE(),
                        IsSuccessful BIT
                    );
                END

                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserResults' AND xtype='U')
                BEGIN
                    CREATE TABLE UserResults (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        UserId INT FOREIGN KEY REFERENCES Users(Id),
                        ModuleName NVARCHAR(50),
                        TaskDescription NVARCHAR(MAX),
                        Result NVARCHAR(MAX),
                        CreatedAt DATETIME2 DEFAULT GETDATE()
                    );
                END";

            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(createTables, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка инициализации БД: " + ex.Message);
            }
        }
    }
}
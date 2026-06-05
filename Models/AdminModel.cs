using System;
using System.Collections.Generic;

namespace TipaDiplom.Models
{
 
    public class StudentResult
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ModuleName { get; set; }
        public string TaskDescription { get; set; }
        public string Result { get; set; }
        public DateTime CreatedAt { get; set; }

        public string DisplayName =>
            string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName)
                ? Username
                : $"{FirstName} {LastName}".Trim();
    }


    public class StudentStats
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public int TotalTests { get; set; }
        public DateTime? LastActivity { get; set; }
        public DateTime RegisteredAt { get; set; }
    }

    public class DbTestQuestion
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string QuestionText { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string CorrectOption { get; set; }
        public string Explanation { get; set; }
        public int Difficulty { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
    public class LectureItem
    {
        public int Id { get; set; }
        public int Number { get; set; }  // Номер лекции
        public string Title { get; set; }  // Название темы
        public string Content { get; set; }  // Содержимое лекции
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        public string DisplayTitle => $"Лекция {Number}. {Title}";
    }


    public class TheoryItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
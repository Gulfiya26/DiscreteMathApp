
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TipaDiplom.Models;
using TipaDiplom.Services;

namespace TipaDiplom.Views
{
    public partial class AddTestView : UserControl
    {
        private readonly AdminService _adminService;
        private DbTestQuestion _selectedQuestion = null;
        private bool _isEditing = false;

        public AddTestView()
        {
            InitializeComponent();
            _adminService = new AdminService();
            LoadQuestions();
        }

        private void LoadQuestions()
        {
            try
            {
                var questions = _adminService.GetAllDbQuestions();
                QuestionList.ItemsSource = questions;
            }
            catch (Exception ex)
            {
                ShowStatus("Ошибка загрузки: " + ex.Message, false);
            }
        }

        private void QuestionList_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            if (QuestionList.SelectedItem is DbTestQuestion q)
            {
                _selectedQuestion = q;
                _isEditing = true;

                QuestionTextBox.Text = q.QuestionText;
                OptionABox.Text = q.OptionA;
                OptionBBox.Text = q.OptionB;
                OptionCBox.Text = q.OptionC;
                OptionDBox.Text = q.OptionD;
                ExplanationBox.Text = q.Explanation;

                // Правильный ответ
                CorrectA.IsChecked = q.CorrectOption == "A";
                CorrectB.IsChecked = q.CorrectOption == "B";
                CorrectC.IsChecked = q.CorrectOption == "C";
                CorrectD.IsChecked = q.CorrectOption == "D";

                // Категория
                foreach (ComboBoxItem item in QCategoryBox.Items)
                {
                    if (item.Content.ToString() == q.Category)
                    {
                        QCategoryBox.SelectedItem = item;
                        break;
                    }
                }

                // Сложность
                DifficultyBox.SelectedIndex = q.Difficulty - 1;

                ShowStatus($"Редактирование вопроса #{q.Id}", true);
            }
        }

        private void NewQuestion_Click(object sender, RoutedEventArgs e)
        {
            ClearQuestion_Click(sender, e);
            QuestionTextBox.Focus();
        }

        private void SaveQuestion_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(QuestionTextBox.Text))
            {
                ShowStatus("Введите текст вопроса!", false);
                return;
            }

            if (string.IsNullOrWhiteSpace(OptionABox.Text) ||
                string.IsNullOrWhiteSpace(OptionBBox.Text) ||
                string.IsNullOrWhiteSpace(OptionCBox.Text) ||
                string.IsNullOrWhiteSpace(OptionDBox.Text))
            {
                ShowStatus("Заполните все варианты ответов!", false);
                return;
            }

            if (CorrectA.IsChecked != true && CorrectB.IsChecked != true &&
                CorrectC.IsChecked != true && CorrectD.IsChecked != true)
            {
                ShowStatus("Выберите правильный ответ!", false);
                return;
            }

            string correctOption = CorrectA.IsChecked == true ? "A" :
                                   CorrectB.IsChecked == true ? "B" :
                                   CorrectC.IsChecked == true ? "C" : "D";

            string category = ((ComboBoxItem)QCategoryBox.SelectedItem)
                ?.Content?.ToString() ?? "Теория множеств";

            int difficulty = DifficultyBox.SelectedIndex + 1;

            try
            {
                if (_isEditing && _selectedQuestion != null)
                {
                    _selectedQuestion.Category = category;
                    _selectedQuestion.QuestionText = QuestionTextBox.Text.Trim();
                    _selectedQuestion.OptionA = OptionABox.Text.Trim();
                    _selectedQuestion.OptionB = OptionBBox.Text.Trim();
                    _selectedQuestion.OptionC = OptionCBox.Text.Trim();
                    _selectedQuestion.OptionD = OptionDBox.Text.Trim();
                    _selectedQuestion.CorrectOption = correctOption;
                    _selectedQuestion.Explanation = ExplanationBox.Text.Trim();
                    _selectedQuestion.Difficulty = difficulty;
                    _selectedQuestion.IsActive = true;

                    _adminService.UpdateQuestion(_selectedQuestion);
                    ShowStatus("✅ Вопрос обновлён!", true);
                }
                else
                {
                    var newQuestion = new DbTestQuestion
                    {
                        Category = category,
                        QuestionText = QuestionTextBox.Text.Trim(),
                        OptionA = OptionABox.Text.Trim(),
                        OptionB = OptionBBox.Text.Trim(),
                        OptionC = OptionCBox.Text.Trim(),
                        OptionD = OptionDBox.Text.Trim(),
                        CorrectOption = correctOption,
                        Explanation = ExplanationBox.Text.Trim(),
                        Difficulty = difficulty,
                        IsActive = true
                    };

                    _adminService.AddQuestion(newQuestion);
                    ShowStatus("✅ Вопрос добавлен в банк тестов!", true);
                }

                LoadQuestions();
                ClearQuestion_Click(sender, e);
            }
            catch (Exception ex)
            {
                ShowStatus("❌ Ошибка: " + ex.Message, false);
            }
        }

        private void DeleteQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (QuestionList.SelectedItem is DbTestQuestion q)
            {
                var confirm = MessageBox.Show(
                    "Удалить этот вопрос из банка тестов?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirm == MessageBoxResult.Yes)
                {
                    try
                    {
                        _adminService.DeleteQuestion(q.Id);
                        ClearQuestion_Click(sender, e);
                        LoadQuestions();
                        ShowStatus("✅ Вопрос удалён", true);
                    }
                    catch (Exception ex)
                    {
                        ShowStatus("❌ Ошибка: " + ex.Message, false);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите вопрос для удаления",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ClearQuestion_Click(object sender, RoutedEventArgs e)
        {
            _selectedQuestion = null;
            _isEditing = false;
            QuestionTextBox.Text = "";
            OptionABox.Text = "";
            OptionBBox.Text = "";
            OptionCBox.Text = "";
            OptionDBox.Text = "";
            ExplanationBox.Text = "";
            CorrectA.IsChecked = false;
            CorrectB.IsChecked = false;
            CorrectC.IsChecked = false;
            CorrectD.IsChecked = false;
            QCategoryBox.SelectedIndex = 0;
            DifficultyBox.SelectedIndex = 0;
            QuestionList.SelectedItem = null;
            QStatusText.Text = "";
        }

        private void ShowStatus(string message, bool isSuccess)
        {
            QStatusText.Text = message;
            QStatusText.Foreground = new SolidColorBrush(
                isSuccess ? Color.FromRgb(166, 227, 161)
                          : Color.FromRgb(243, 139, 168));
        }
    }
}
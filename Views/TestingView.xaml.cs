using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using TipaDiplom.Models;

namespace TipaDiplom.Views
{
    public partial class TestingView : UserControl
    {
        private List<TestQuestion> _questions;
        private int _currentIndex;
        private int _correctCount;
        private int _wrongCount;
        private bool _answered;
        private string _selectedCategory;
        private DateTime _startTime;
        private DispatcherTimer _timer;
        private Random _random = new Random();

        public TestingView()
        {
            InitializeComponent();
            UpdateQuestionStats();
        }

        private void UpdateQuestionStats()
        {
            var stats = "";
            var categories = new List<string>
            {
                "Теория множеств",
                "Булева алгебра",
                "Теория графов",
                "Комбинаторика",
                "Системы счисления",
                "Матрицы и отношения"
            };

            int total = 0;
            foreach (var cat in categories)
            {
                int count = QuestionBank.GetQuestionsByCategory(cat).Count;
                stats += $"  {cat}: {count} вопросов\n";
                total += count;
            }
            stats += $"\n  Всего: {total} вопросов";
            QuestionStatsText.Text = stats;
        }

        private void StartTest_Click(object sender, RoutedEventArgs e)
        {
            // Получаем категорию
            var selectedItem = (ComboBoxItem)CategoryCombo.SelectedItem;
            _selectedCategory = selectedItem.Content.ToString();

            // Получаем вопросы
            if (_selectedCategory == "Все темы")
                _questions = QuestionBank.GetAllQuestions();
            else
                _questions = QuestionBank.GetQuestionsByCategory(_selectedCategory);

            if (_questions.Count == 0)
            {
                MessageBox.Show("Нет вопросов для выбранной категории.", "Ошибка");
                return;
            }

            // Перемешиваем
            _questions = _questions.OrderBy(q => _random.Next()).ToList();

            // Ограничиваем количество
            var countItem = (ComboBoxItem)QuestionCountCombo.SelectedItem;
            string countText = countItem.Content.ToString();
            if (countText != "Все вопросы")
            {
                int maxCount = int.Parse(countText);
                if (_questions.Count > maxCount)
                    _questions = _questions.Take(maxCount).ToList();
            }

            // Перемешиваем варианты ответов внутри каждого вопроса
            foreach (var q in _questions)
            {
                ShuffleOptions(q);
            }

            // Инициализация
            _currentIndex = 0;
            _correctCount = 0;
            _wrongCount = 0;
            _startTime = DateTime.Now;

            // Запускаем таймер
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();

            // Показываем первый вопрос
            ShowQuestion();

            SelectionPanel.Visibility = Visibility.Collapsed;
            QuestionPanel.Visibility = Visibility.Visible;
            ResultPanel.Visibility = Visibility.Collapsed;
        }

        private void ShuffleOptions(TestQuestion question)
        {
            string correctAnswer = question.Options[question.CorrectAnswerIndex];
            var shuffled = question.Options.OrderBy(o => _random.Next()).ToList();
            question.Options = shuffled;
            question.CorrectAnswerIndex = shuffled.IndexOf(correctAnswer);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var elapsed = DateTime.Now - _startTime;
            TimerText.Text = $"⏱ {elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
        }

        private void ShowQuestion()
        {
            _answered = false;
            var question = _questions[_currentIndex];

            // Обновляем прогресс
            ProgressText.Text = $"Вопрос {_currentIndex + 1} из {_questions.Count}";
            ProgressBar.Value = (_currentIndex + 1.0) / _questions.Count * 100;
            ScoreText.Text = $"✅ {_correctCount}  ❌ {_wrongCount}";

            // Категория и сложность
            CategoryText.Text = question.Category;
            string stars = new string('⭐', question.Difficulty);
            DifficultyText.Text = stars;

            // Текст вопроса
            QuestionText.Text = question.QuestionText;

            // Варианты ответов
            OptionsPanel.Children.Clear();
            string[] letters = { "A", "B", "C", "D", "E", "F" };

            for (int i = 0; i < question.Options.Count; i++)
            {
                int index = i;
                var optionButton = new Button
                {
                    Content = $"  {letters[i]}.  {question.Options[i]}",
                    FontSize = 15,
                    Padding = new Thickness(15, 12, 15, 12),
                    Margin = new Thickness(0, 4, 0, 4),
                    Background = new SolidColorBrush(Color.FromRgb(49, 50, 68)),
                    Foreground = new SolidColorBrush(Color.FromRgb(205, 214, 244)),
                    BorderThickness = new Thickness(2),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(69, 71, 90)),
                    Cursor = System.Windows.Input.Cursors.Hand,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Tag = index
                };

                // Скруглённые углы
                optionButton.Resources.Add(typeof(Border), new Style(typeof(Border))
                {
                    Setters = { new Setter(Border.CornerRadiusProperty, new CornerRadius(8)) }
                });

                optionButton.Click += OptionButton_Click;
                OptionsPanel.Children.Add(optionButton);
            }

            // Скрываем объяснение и кнопку "Далее"
            ExplanationPanel.Visibility = Visibility.Collapsed;
            NextButton.Visibility = Visibility.Collapsed;
        }

        private void OptionButton_Click(object sender, RoutedEventArgs e)
        {
            if (_answered) return;
            _answered = true;

            var button = (Button)sender;
            int selectedIndex = (int)button.Tag;
            var question = _questions[_currentIndex];
            bool isCorrect = selectedIndex == question.CorrectAnswerIndex;

            if (isCorrect)
                _correctCount++;
            else
                _wrongCount++;

            // Подсвечиваем ответы
            for (int i = 0; i < OptionsPanel.Children.Count; i++)
            {
                var btn = (Button)OptionsPanel.Children[i];
                int btnIndex = (int)btn.Tag;

                if (btnIndex == question.CorrectAnswerIndex)
                {
                    // Правильный ответ — зелёный
                    btn.Background = new SolidColorBrush(Color.FromRgb(166, 227, 161));
                    btn.Foreground = new SolidColorBrush(Color.FromRgb(30, 30, 46));
                    btn.BorderBrush = new SolidColorBrush(Color.FromRgb(166, 227, 161));
                }
                else if (btnIndex == selectedIndex && !isCorrect)
                {
                    // Неправильный выбранный — красный
                    btn.Background = new SolidColorBrush(Color.FromRgb(243, 139, 168));
                    btn.Foreground = new SolidColorBrush(Color.FromRgb(30, 30, 46));
                    btn.BorderBrush = new SolidColorBrush(Color.FromRgb(243, 139, 168));
                }
                else
                {
                    // Остальные — приглушённые
                    btn.Opacity = 0.5;
                }

                btn.IsEnabled = false;
            }

            // Показываем объяснение
            ExplanationPanel.Visibility = Visibility.Visible;
            if (isCorrect)
            {
                ExplanationHeader.Text = "✅ Правильно!";
                ExplanationHeader.Foreground = new SolidColorBrush(Color.FromRgb(166, 227, 161));
            }
            else
            {
                ExplanationHeader.Text = "❌ Неправильно";
                ExplanationHeader.Foreground = new SolidColorBrush(Color.FromRgb(243, 139, 168));
            }
            ExplanationText.Text = question.Explanation;

            // Обновляем счёт
            ScoreText.Text = $"✅ {_correctCount}  ❌ {_wrongCount}";

            // Показываем кнопку "Далее"
            if (_currentIndex < _questions.Count - 1)
                NextButton.Content = "Следующий вопрос →";
            else
                NextButton.Content = "📊 Показать результаты";

            NextButton.Visibility = Visibility.Visible;
        }

        private void NextQuestion_Click(object sender, RoutedEventArgs e)
        {
            _currentIndex++;

            if (_currentIndex < _questions.Count)
            {
                ShowQuestion();
            }
            else
            {
                ShowResults();
            }
        }

        private void ShowResults()
        {
            _timer.Stop();
            var elapsed = DateTime.Now - _startTime;

            QuestionPanel.Visibility = Visibility.Collapsed;
            ResultPanel.Visibility = Visibility.Visible;

            double percentage = _questions.Count > 0
                ? Math.Round((double)_correctCount / _questions.Count * 100, 1) : 0;

            // Оценка
            string grade;
            Color gradeColor;
            if (percentage >= 90)
            {
                grade = "🏆 Отлично (5)";
                gradeColor = Color.FromRgb(166, 227, 161);
            }
            else if (percentage >= 75)
            {
                grade = "👍 Хорошо (4)";
                gradeColor = Color.FromRgb(137, 180, 250);
            }
            else if (percentage >= 60)
            {
                grade = "📝 Удовлетворительно (3)";
                gradeColor = Color.FromRgb(249, 226, 175);
            }
            else
            {
                grade = "📚 Неудовлетворительно (2)";
                gradeColor = Color.FromRgb(243, 139, 168);
            }

            GradeText.Text = grade;
            GradeText.Foreground = new SolidColorBrush(gradeColor);
            PercentageText.Text = $"{percentage}%";

            DetailCorrectText.Text = $"✅ Правильных: {_correctCount}";
            DetailWrongText.Text = $"❌ Неправильных: {_wrongCount}";
            DetailTotalText.Text = $"📋 Всего вопросов: {_questions.Count}";
            DetailTimeText.Text = $"⏱ Время: {elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
            DetailCategoryText.Text = $"📁 Тема: {_selectedCategory}";

            // Сохраняем результат в БД
            try
            {
                var authService = new Services.AuthService();
                authService.SaveResult("Тестирование",
                    $"Тема: {_selectedCategory}, Вопросов: {_questions.Count}",
                    $"Результат: {_correctCount}/{_questions.Count} ({percentage}%) — {grade}");
            }
            catch { /* игнорируем */ }
        }

        private void RetryTest_Click(object sender, RoutedEventArgs e)
        {
            ResultPanel.Visibility = Visibility.Collapsed;
            StartTest_Click(sender, e);
        }

        private void NewTest_Click(object sender, RoutedEventArgs e)
        {
            ResultPanel.Visibility = Visibility.Collapsed;
            SelectionPanel.Visibility = Visibility.Visible;
        }
    }
}
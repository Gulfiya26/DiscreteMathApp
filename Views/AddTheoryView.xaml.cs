using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TipaDiplom.Models;
using TipaDiplom.Services;

namespace TipaDiplom.Views
{
    public partial class AddTheoryView : UserControl
    {
        private readonly AdminService _adminService;
        private LectureItem _selectedItem = null;
        private bool _isEditing = false;

        public AddTheoryView()
        {
            InitializeComponent();
            _adminService = new AdminService();
            LoadLectureList();
        }

        private void LoadLectureList()
        {
            try
            {
                var items = _adminService.GetAllLectures();
                LectureList.ItemsSource = items;
            }
            catch (Exception ex)
            {
                ShowStatus("Ошибка загрузки: " + ex.Message, false);
            }
        }

        private void LectureList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LectureList.SelectedItem is LectureItem item)
            {
                _selectedItem = item;
                _isEditing = true;

                NumberBox.Text = item.Number.ToString();
                TitleBox.Text = item.Title;
                ContentBox.Text = item.Content;

                ShowStatus($"Редактирование: Лекция {item.Number}", true);
            }
        }

        private void NewLecture_Click(object sender, RoutedEventArgs e)
        {
            ClearForm_Click(sender, e);

            // Автоматически подставляем следующий номер
            try
            {
                int nextNumber = _adminService.GetNextLectureNumber();
                NumberBox.Text = nextNumber.ToString();
            }
            catch { }

            TitleBox.Focus();
        }

        private void SaveLecture_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NumberBox.Text))
            {
                ShowStatus("Введите номер лекции!", false);
                return;
            }

            if (!int.TryParse(NumberBox.Text, out int number) || number < 1)
            {
                ShowStatus("Номер лекции должен быть положительным числом!", false);
                return;
            }

            if (string.IsNullOrWhiteSpace(TitleBox.Text))
            {
                ShowStatus("Введите название темы!", false);
                return;
            }

            if (string.IsNullOrWhiteSpace(ContentBox.Text))
            {
                ShowStatus("Введите содержимое лекции!", false);
                return;
            }

            try
            {
                if (_isEditing && _selectedItem != null)
                {
                    // Обновляем существующую
                    _selectedItem.Number = number;
                    _selectedItem.Title = TitleBox.Text.Trim();
                    _selectedItem.Content = ContentBox.Text.Trim();
                    _selectedItem.IsActive = true;

                    _adminService.UpdateLecture(_selectedItem);
                    ShowStatus("✅ Лекция обновлена успешно!", true);
                }
                else
                {
                    // Добавляем новую
                    var newItem = new LectureItem
                    {
                        Number = number,
                        Title = TitleBox.Text.Trim(),
                        Content = ContentBox.Text.Trim(),
                        IsActive = true
                    };

                    _adminService.AddLecture(newItem);
                    ShowStatus("✅ Новая лекция добавлена!", true);
                }

                LoadLectureList();
            }
            catch (Exception ex)
            {
                ShowStatus("❌ Ошибка: " + ex.Message, false);
            }
        }

        private void DeleteLecture_Click(object sender, RoutedEventArgs e)
        {
            if (LectureList.SelectedItem is LectureItem item)
            {
                var confirm = MessageBox.Show(
                    $"Удалить лекцию {item.Number} \"{item.Title}\"?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirm == MessageBoxResult.Yes)
                {
                    try
                    {
                        _adminService.DeleteLecture(item.Id);
                        ClearForm_Click(sender, e);
                        LoadLectureList();
                        ShowStatus("✅ Лекция удалена", true);
                    }
                    catch (Exception ex)
                    {
                        ShowStatus("❌ Ошибка: " + ex.Message, false);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите лекцию для удаления",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ClearForm_Click(object sender, RoutedEventArgs e)
        {
            _selectedItem = null;
            _isEditing = false;
            NumberBox.Text = "";
            TitleBox.Text = "";
            ContentBox.Text = "";
            LectureList.SelectedItem = null;
            StatusText.Text = "";
        }

        private void ShowStatus(string message, bool isSuccess)
        {
            StatusText.Text = message;
            StatusText.Foreground = new SolidColorBrush(
                isSuccess ? Color.FromRgb(166, 227, 161)
                          : Color.FromRgb(243, 139, 168));
        }
    }
}
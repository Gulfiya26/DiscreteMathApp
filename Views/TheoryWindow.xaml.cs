using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TipaDiplom.Models;
using TipaDiplom.Services;

namespace TipaDiplom.Views
{
    public partial class TheoryWindow : Window
    {
        private readonly AdminService _adminService;

        public TheoryWindow()
        {
            InitializeComponent();
            _adminService = new AdminService();
            LoadLectures();
        }

        private void LoadLectures()
        {
            try
            {
                var lectures = _adminService.GetAllLectures()
                    .Where(l => l.IsActive)
                    .OrderBy(l => l.Number)
                    .ToList();

                LectureListBox.ItemsSource = lectures;

                // Автоматически выбираем первую лекцию
                if (lectures.Count > 0)
                    LectureListBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки лекций: " + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LectureListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LectureListBox.SelectedItem is LectureItem lecture)
            {
                LectureTitle.Text = lecture.DisplayTitle;
                LectureContent.Text = lecture.Content;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
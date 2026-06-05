
using System;
using System.Runtime.Remoting.Contexts;
using System.Windows;
using System.Windows.Controls;
using TipaDiplom.Models;
using TipaDiplom.Services;

namespace TipaDiplom.Views
{
    public partial class AdminResultsView : UserControl
    {
        private readonly AdminService _adminService;

        public AdminResultsView()
        {
            InitializeComponent();
            _adminService = new AdminService();
            LoadResults();
        }

        private void LoadResults(string user = "", string category = "")
        {
            try
            {
                var catFilter = category == "Все модули" ? "" : category;
                var results = _adminService.GetAllResults(catFilter, user);
                ResultsGrid.ItemsSource = results;
                CountText.Text = $"Всего записей: {results.Count}";
            }
            catch (Exception ex)
            {
                CountText.Text = "Ошибка загрузки: " + ex.Message;
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string user = FilterUserBox.Text == "Поиск по студенту..."
                ? "" : FilterUserBox.Text;
            string cat = ((ComboBoxItem)FilterCategoryBox.SelectedItem)
                ?.Content?.ToString() ?? "";
            LoadResults(user, cat);
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            FilterUserBox.Text = "Поиск по студенту...";
            FilterCategoryBox.SelectedIndex = 0;
            LoadResults();
        }

        private void DeleteResult_Click(object sender, RoutedEventArgs e)
        {
            if (ResultsGrid.SelectedItem is StudentResult selected)
            {
                var confirm = MessageBox.Show(
                    $"Удалить результат студента {selected.DisplayName}?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirm == MessageBoxResult.Yes)
                {
                    try
                    {
                        _adminService.DeleteResult(selected.Id);
                        LoadResults();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления",
                    "Внимание", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void FilterBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (FilterUserBox.Text == "Поиск по студенту...")
                FilterUserBox.Text = "";
        }
    }
}

using System.Windows;
using TipaDiplom.Services;
using TipaDiplom.Views;

namespace TipaDiplom.Views
{
    public partial class AdminWindow : Window
    {
        private readonly AdminService _adminService;

        public AdminWindow()
        {
            InitializeComponent();
            _adminService = new AdminService();

            // Инициализируем таблицы
            _adminService.InitializeAdminTables();

            // По умолчанию открываем результаты
            AdminFrame.Content = new AdminResultsView();
        }

        private void ShowResults_Click(object sender, RoutedEventArgs e)
            => AdminFrame.Content = new AdminResultsView();

        private void ShowStudents_Click(object sender, RoutedEventArgs e)
            => AdminFrame.Content = new AdminStudentsView();

        private void ShowTheory_Click(object sender, RoutedEventArgs e)
            => AdminFrame.Content = new AddTheoryView();

        private void ShowTests_Click(object sender, RoutedEventArgs e)
            => AdminFrame.Content = new AddTestView();

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
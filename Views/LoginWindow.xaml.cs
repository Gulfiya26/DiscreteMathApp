using TipaDiplom.Services;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TipaDiplom;
using TipaDiplom.Views;

namespace TipaDiplom.Views
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService;
        private readonly DatabaseService _dbService;

        public LoginWindow()
        {
            InitializeComponent();

            _authService = new AuthService();
            _dbService = new DatabaseService();

            // Проверяем подключение к БД при запуске
            CheckConnection();

            // Enter для входа
            PasswordBox.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Enter) Login_Click(s, e);
            };

            UsernameBox.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Enter) PasswordBox.Focus();
            };

            UsernameBox.Focus();
        }

        private void CheckConnection()
        {
            string error;
            if (_dbService.TestConnection(out error))
            {
                ConnectionStatus.Text = "🟢 Подключение к серверу установлено";
                ConnectionStatus.Foreground =
                    new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(166, 227, 161));

                // Инициализируем таблицы
                try
                {
                    _dbService.InitializeDatabase();
                }
                catch (Exception ex)
                {
                    ShowError("Ошибка инициализации БД: " + ex.Message);
                }
            }
            else
            {
                ConnectionStatus.Text = "🔴 Нет подключения к серверу";
                ConnectionStatus.Foreground =
                    new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(243, 139, 168));
                ShowError("Не удалось подключиться к серверу БД.\n" + error);
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password;

            var adminService = new AdminService();
            if (adminService.IsAdmin(username, password))
            {
                var adminWindow = new AdminWindow();
                adminWindow.Show();
                this.Close();
                return;
            }

            
            var (success, message) = _authService.Login(username, password);

            if (success)
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                ShowError(message);
            }
        }

        private void GoToRegister_Click(object sender, MouseButtonEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }

        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
            ErrorMessage.Visibility = Visibility.Visible;
        }
    }
}
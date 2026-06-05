
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TipaDiplom.Services;
using TipaDiplom.Views;

namespace TipaDiplom.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly AuthService _authService;

        public RegisterWindow()
        {
            InitializeComponent();
            _authService = new AuthService();
            UsernameBox.Focus();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string email = EmailBox.Text.Trim();
            string firstName = FirstNameBox.Text.Trim();
            string lastName = LastNameBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            // Проверка совпадения паролей
            if (password != confirmPassword)
            {
                ShowMessage("Пароли не совпадают", false);
                return;
            }

            var (success, message) = _authService.Register(
                username, email, password, firstName, lastName);

            ShowMessage(message, success);

            if (success)
            {
                // Задержка и переход на логин
                MessageBox.Show(message, "Успешно",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        private void GoToLogin_Click(object sender, MouseButtonEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            MessageText.Text = message;
            MessageText.Visibility = Visibility.Visible;
            MessageText.Foreground = new SolidColorBrush(
                isSuccess ? Color.FromRgb(166, 227, 161)   // зелёный
                          : Color.FromRgb(243, 139, 168)); // красный
        }
    }
}
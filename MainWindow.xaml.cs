
using System.Windows;
using TipaDiplom.Services;
using TipaDiplom.Views;

namespace TipaDiplom
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Content = new SetOperationsView();

       
            if (AuthService.CurrentUser != null)
            {
                UserNameText.Text = AuthService.CurrentUser.DisplayName;
                UserEmailText.Text = AuthService.CurrentUser.Email;
            }
        }

        private void ShowSetOperations(object sender, RoutedEventArgs e)
            => MainFrame.Content = new SetOperationsView();

        private void ShowBooleanAlgebra(object sender, RoutedEventArgs e)
            => MainFrame.Content = new BooleanAlgebraView();

        private void ShowGraphTheory(object sender, RoutedEventArgs e)
            => MainFrame.Content = new GraphView();

        private void ShowCombinatorics(object sender, RoutedEventArgs e)
            => MainFrame.Content = new CombinatoricsView();

        private void ShowMatrix(object sender, RoutedEventArgs e)
            => MainFrame.Content = new MatrixView();

        private void ShowNumberSystems(object sender, RoutedEventArgs e)
            => MainFrame.Content = new NumberSystemsView();

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти?",
                "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                AuthService.Logout();
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }
        

        private void ShowTesting(object sender, RoutedEventArgs e)
            => MainFrame.Content = new TestingView();

        private void ShowTheory(object sender, RoutedEventArgs e)
        {
            var theoryWindow = new TheoryWindow();
            theoryWindow.Owner = this;
            theoryWindow.Show();
        }
    }
}
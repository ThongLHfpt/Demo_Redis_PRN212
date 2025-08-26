using BussinessLayer;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly UserService _userService;

        public MainWindow()
        {
            InitializeComponent();
            _userService = new UserService();
        }

        private void FetchButton_Click(object sender, RoutedEventArgs e)
        {
            FetchButton.IsEnabled = false;
            ResultTextBox.Text = string.Empty;

            var userIdText = UserIdTextBox.Text?.Trim();
            if (string.IsNullOrWhiteSpace(userIdText))
            {
                ResultTextBox.Text = "Vui lòng nhập User Id";
                FetchButton.IsEnabled = true;
                return;
            }

            if (!int.TryParse(userIdText, out int userId))
            {
                ResultTextBox.Text = "User Id phải là số nguyên";
                FetchButton.IsEnabled = true;
                return;
            }

            try
            {
                var result = _userService.GetUserProfile(userId);
                ResultTextBox.Text = result;
            }
            catch (Exception ex)
            {
                ResultTextBox.Text = $"Lỗi: {ex.Message}";
            }
            finally
            {
                FetchButton.IsEnabled = true;
            }
        }
    }
}
using System.Windows;
using System.Windows.Controls;
using TimekeepingApp.Services;
using TimekeepingApp.ViewModels;

namespace TimekeepingApp.Views
{
    public partial class LoginView : UserControl
    {
        private readonly DatabaseService _databaseService;
        private readonly MainViewModel _mainViewModel;

        // Default constructor required for XAML
        public LoginView()
        {
            InitializeComponent();
        }

        // Constructor with parameters
        public LoginView(DatabaseService databaseService, MainViewModel mainViewModel)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _mainViewModel = mainViewModel;
            DataContext = new LoginViewModel(_databaseService, _mainViewModel);
        }

        // Event handler for the PasswordBox PasswordChanged event
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}

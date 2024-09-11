using System;
using System.ComponentModel;
using System.Windows.Input;
using TimekeepingApp.Models;
using TimekeepingApp.Services;

namespace TimekeepingApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _password;
        private string _statusMessage;
        private bool _isLoggedIn;
        private readonly DatabaseService _databaseService;
        private readonly MainViewModel _mainViewModel; // Reference to MainViewModel

        public ICommand LoginCommand { get; }
        public ICommand LogoutCommand { get; }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
                ((RelayCommand)LoginCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
                ((RelayCommand)LoginCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                _isLoggedIn = value;
                OnPropertyChanged(nameof(IsLoggedIn));
            }
        }

        // Constructor accepting DatabaseService and MainViewModel
        public LoginViewModel(DatabaseService databaseService, MainViewModel mainViewModel)
        {
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            _mainViewModel = mainViewModel ?? throw new ArgumentNullException(nameof(mainViewModel)); // Ensure MainViewModel is passed

            LoginCommand = new RelayCommand(Login, CanLogin);
            LogoutCommand = new RelayCommand(Logout, CanLogout);
        }

        private bool CanLogin(object parameter)
        {
            // Only allow login if username and password are not empty
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }

        private async void Login(object parameter)
        {
            try
            {
                // Simulate fetching a user from the database (replace this with actual DB call if needed)
                var user = await _databaseService.GetAllUsersAsync();

                // Dummy authentication logic: replace with real authentication
                var authenticatedUser = user.Find(u => u.Username == Username && u.Password == Password);

                if (authenticatedUser != null)
                {
                    IsLoggedIn = true;
                    StatusMessage = $"Welcome, {authenticatedUser.Username}! You are logged in as {authenticatedUser.Role}.";

                    // Update MainViewModel with the authenticated user
                    _mainViewModel.CurrentUser = authenticatedUser;
                    _mainViewModel.IsLoggedIn = true;
                    _mainViewModel.Username = authenticatedUser.Username;

                    // Navigate to the appropriate view based on user role
                    if (authenticatedUser.Role == "IC") // Individual Contributor
                    {
                        _mainViewModel.CurrentViewModel = new ICViewModel(_databaseService, authenticatedUser);
                    }
                    else if (authenticatedUser.Role == "Manager") // Manager
                    {
                        _mainViewModel.CurrentViewModel = new ManagerViewModel(_databaseService);
                    }
                }
                else
                {
                    // Handle invalid credentials
                    IsLoggedIn = false;
                    StatusMessage = "Invalid username or password.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error during login: {ex.Message}";
            }
        }

        private bool CanLogout(object parameter)
        {
            // Allow logout only if the user is logged in
            return IsLoggedIn;
        }

        private void Logout(object parameter)
        {
            Username = string.Empty;
            Password = string.Empty;
            IsLoggedIn = false;
            StatusMessage = "You have been logged out.";
            _mainViewModel.IsLoggedIn = false;
            _mainViewModel.Username = string.Empty;
            _mainViewModel.CurrentViewModel = this; // Return to the login view
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

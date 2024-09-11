using System.ComponentModel;
using System.Windows.Input;
using TimekeepingApp.Data;
using TimekeepingApp.Models;
using TimekeepingApp.Services;
using TimekeepingApp.Views;

namespace TimekeepingApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object _currentViewModel;
        private string _username;
        private string _statusMessage;
        private bool _isLoggedIn;
        private UserModel _currentUser; // Property to hold the current logged-in user
        public DatabaseService DatabaseService { get; } // Make DatabaseService accessible

        private readonly DatabaseService _databaseService;

        // Commands for navigation
        public ICommand ShowICViewCommand { get; set; }
        public ICommand ShowManagerViewCommand { get; set; }
        public ICommand ShowAddTimesheetViewCommand { get; set; }
        public ICommand LogoutCommand { get; set; }

        // Current ViewModel to determine which view is displayed
        public object CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage)); // Notify the UI of the change
            }
        }

        // Username of the logged-in user
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        // State indicating if a user is logged in
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                _isLoggedIn = value;
                OnPropertyChanged(nameof(IsLoggedIn));
                // Update command availability when the login state changes
                ((RelayCommand)ShowICViewCommand)?.RaiseCanExecuteChanged();
                ((RelayCommand)ShowManagerViewCommand)?.RaiseCanExecuteChanged();
                ((RelayCommand)LogoutCommand)?.RaiseCanExecuteChanged();
            }
        }

        // Constructor initializing commands and setting the initial view
        public MainViewModel(DatabaseService databaseService)
        {
            DatabaseService = new DatabaseService(new AppDbContext());

            ShowICViewCommand = new RelayCommand(ShowICView, CanShowICView);
            ShowManagerViewCommand = new RelayCommand(ShowManagerView, CanShowManagerView);
            ShowAddTimesheetViewCommand = new RelayCommand(ShowAddTimesheetView, CanShowICView);
            LogoutCommand = new RelayCommand(Logout, CanLogout);

            // Set the initial view to the login screen
            CurrentViewModel = new LoginViewModel(DatabaseService, this); // Pass MainViewModel reference
            StatusMessage = string.Empty; // Start with an empty message or some initial value
        }

        // Property to manage the current logged-in user
        public UserModel CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged(nameof(CurrentUser));
            }
        }

        // Show IC View
        private void ShowICView(object parameter)
        {
            // Assume the parameter is of type UserModel
            if (parameter is UserModel currentUser)
            {
                // Create the DatabaseService if not already created
                var databaseService = new DatabaseService(new AppDbContext());

                // Create the ICView instance
                var icView = new ICView();

                // Initialize the ICView with the necessary parameters
                icView.Initialize(databaseService, currentUser);

                // Set the CurrentViewModel or ContentPresenter to the newly created ICView
                CurrentViewModel = icView;
            }
            else
            {
                // Handle the case where the parameter is not the expected type
                StatusMessage = "Invalid user information provided.";
            }
        }

        private bool CanShowICView(object parameter)
        {
            // Allow showing the IC View only if a valid user is provided and logged in
            return parameter is UserModel && IsLoggedIn;
        }

        // Show Manager View
        private void ShowManagerView(object parameter)
        {
            if (parameter is DatabaseService databaseService)
            {
                var managerView = new ManagerView();
                managerView.Initialize(databaseService); // Set up the view with its dependencies
                CurrentViewModel = managerView; // Display the manager view
            }
            else
            {
                StatusMessage = "Failed to load Manager View.";
            }
        }


        // Show Add Timesheet View
        private void ShowAddTimesheetView(object parameter)
        {
            // Ensure CurrentUser is passed to the AddTimesheetViewModel
            if (_currentUser != null)
            {
                // Create the DatabaseService if not already created
                var databaseService = new DatabaseService(new AppDbContext());

                // Create the AddTimesheetView instance
                var addTimesheetView = new AddTimesheetView();

                // Initialize the AddTimesheetView with the necessary parameters
                addTimesheetView.Initialize(databaseService, _currentUser, this); // Ensure Initialize method is implemented in AddTimesheetView if not already

                // Set the CurrentViewModel or ContentPresenter to the newly created AddTimesheetView
                CurrentViewModel = addTimesheetView;
            }
            else
            {
                // Handle the case where the _currentUser is null
                StatusMessage = "Current user is not set.";
            }
        }



        // Logout logic
        private void Logout(object parameter)
        {
            IsLoggedIn = false;
            Username = string.Empty;
            CurrentUser = null;
            CurrentViewModel = new LoginViewModel(DatabaseService, this); // Pass MainViewModel reference
        }

        // Command CanExecute logic
        private bool CanShowManagerView(object parameter) => IsLoggedIn;
        private bool CanLogout(object parameter) => IsLoggedIn;

        // PropertyChanged event implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

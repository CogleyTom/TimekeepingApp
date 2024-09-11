using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TimekeepingApp.Models;
using TimekeepingApp.Services;

namespace TimekeepingApp.ViewModels
{
    public class ICViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private readonly UserModel _currentUser; // Hold the logged-in user
        private TimesheetModel _selectedTimesheet;
        private string _statusMessage;

        public ObservableCollection<TimesheetModel> Timesheets { get; set; }
        public ICommand SubmitCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        public ICommand ShowAddTimesheetCommand { get; set; } // command to show AddTimesheetView
        public ICommand DeleteCommand { get; set; }

        // Property for the selected timesheet
        public TimesheetModel SelectedTimesheet
        {
            get => _selectedTimesheet;
            set
            {
                _selectedTimesheet = value;
                OnPropertyChanged(nameof(SelectedTimesheet));
                ((RelayCommand)SubmitCommand).RaiseCanExecuteChanged();
            }
        }

        // Property for the status message
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        // Constructor
        public ICViewModel(DatabaseService databaseService, UserModel currentUser)
        {
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser)); // Ensure user is passed correctly
            Timesheets = new ObservableCollection<TimesheetModel>();
            SubmitCommand = new RelayCommand(SubmitTimesheet, CanSubmitTimesheet);
            DeleteCommand = new RelayCommand(DeleteTimesheet, CanDeleteTimesheet);
            LogoutCommand = new RelayCommand(Logout);
            ShowAddTimesheetCommand = new RelayCommand(ShowAddTimesheetView); // Initialize the new command
            _ = LoadTimesheets(); // Initial load of timesheets
        }

        // Asynchronous method to load timesheets for the current user
        private async Task LoadTimesheets()
        {
            try
            {
                var timesheets = await _databaseService.GetTimesheetsByUserIdAsync(_currentUser.UserId);
                Timesheets.Clear();
                foreach (var timesheet in timesheets)
                {
                    Timesheets.Add(timesheet);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading timesheets: {ex.Message}";
            }
        }

        // Check if the submit command can be executed
        private bool CanSubmitTimesheet(object parameter)
        {
            return SelectedTimesheet != null && SelectedTimesheet.Status == "Pending";
        }

        // Submit the selected timesheet
        private async void SubmitTimesheet(object parameter)
        {
            try
            {
                if (SelectedTimesheet != null)
                {
                    SelectedTimesheet.Status = "Submitted";
                    SelectedTimesheet.UserId = _currentUser.UserId; // Ensure the timesheet is linked to the current user
                    await _databaseService.AddTimesheetAsync(SelectedTimesheet);

                    StatusMessage = "Timesheet submitted successfully.";
                    await LoadTimesheets(); // Refresh timesheets to reflect the submission
                }
                else
                {
                    StatusMessage = "No timesheet selected.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error submitting timesheet: {ex.Message}";
            }
        }

        // Method to show AddTimesheetView
        private void ShowAddTimesheetView(object parameter)
        {
            if (parameter is MainViewModel mainViewModel)
            {
                mainViewModel.CurrentViewModel = new AddTimesheetViewModel(_databaseService, _currentUser, mainViewModel);
            }
            else
            {
                StatusMessage = "Unable to navigate to Add Timesheet View.";
            }
        }

        // Determine if the delete command can execute
        private bool CanDeleteTimesheet(object parameter)
        {
            return SelectedTimesheet != null;
        }

        // Method to delete the selected timesheet
        private async void DeleteTimesheet(object parameter)
        {
            try
            {
                if (SelectedTimesheet != null)
                {
                    await _databaseService.DeleteTimesheetAsync(SelectedTimesheet.TimesheetId);
                    Timesheets.Remove(SelectedTimesheet);
                    StatusMessage = "Timesheet deleted successfully.";
                }
                else
                {
                    StatusMessage = "No timesheet selected to delete.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error deleting timesheet: {ex.Message}";
            }
        }


        // Logout method
        private void Logout(object parameter)
        {
            if (parameter is MainViewModel mainViewModel)
            {
                mainViewModel.CurrentViewModel = new LoginViewModel(mainViewModel.DatabaseService, mainViewModel);
                StatusMessage = "Logged out successfully.";
            }
            else
            {
                StatusMessage = "Logout process failed due to incorrect setup.";
            }
        }

        // PropertyChanged event implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

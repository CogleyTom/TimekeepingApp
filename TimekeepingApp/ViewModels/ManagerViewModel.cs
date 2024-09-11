using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TimekeepingApp.Models;
using TimekeepingApp.Services;

namespace TimekeepingApp.ViewModels
{
    public class ManagerViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<TimesheetModel> _timesheets;
        private TimesheetModel _selectedTimesheet;
        private string _statusMessage;
        private readonly DatabaseService _databaseService;

        // Public properties for Timesheets and SelectedTimesheet
        public ObservableCollection<TimesheetModel> Timesheets
        {
            get => _timesheets;
            set
            {
                _timesheets = value;
                OnPropertyChanged(nameof(Timesheets));
            }
        }

        public TimesheetModel SelectedTimesheet
        {
            get => _selectedTimesheet;
            set
            {
                _selectedTimesheet = value;
                OnPropertyChanged(nameof(SelectedTimesheet));
                ((RelayCommand)ApproveCommand).RaiseCanExecuteChanged();
                ((RelayCommand)RejectCommand).RaiseCanExecuteChanged();
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

        // Commands for Approve, Reject, Logout, and Refresh
        public ICommand ApproveCommand { get; private set; }
        public ICommand RejectCommand { get; private set; }
        public ICommand LogoutCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        // Constructor to initialize the ManagerViewModel with DatabaseService
        public ManagerViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));

            // Initialize commands
            ApproveCommand = new RelayCommand(Approve, CanApprove);
            RejectCommand = new RelayCommand(Reject, CanReject);
            RefreshCommand = new RelayCommand(async _ => await LoadTimesheets());
            LogoutCommand = new RelayCommand(Logout);

            // Initialize the Timesheets collection and load data
            Timesheets = new ObservableCollection<TimesheetModel>();
            _ = LoadTimesheets();
        }

        // Method to load timesheets from the database
        private async Task LoadTimesheets()
        {
            try
            {
                var timesheets = await _databaseService.GetAllTimesheetsAsync();
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



        // Method to handle the logout action
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

        // Command execution conditions
        private bool CanApprove(object parameter) => SelectedTimesheet != null && SelectedTimesheet.Status == "Pending";
        private bool CanReject(object parameter) => SelectedTimesheet != null && SelectedTimesheet.Status == "Pending";

        // Approve action
        private async void Approve(object parameter)
        {
            if (SelectedTimesheet == null) return;

            try
            {
                SelectedTimesheet.Status = "Approved";
                await _databaseService.UpdateTimesheetStatusAsync(SelectedTimesheet.TimesheetId, "Approved");
                StatusMessage = "Timesheet approved.";
                await LoadTimesheets(); // Refresh timesheets after approval
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error approving timesheet: {ex.Message}";
            }
        }

        // Reject action
        private async void Reject(object parameter)
        {
            if (SelectedTimesheet == null) return;

            try
            {
                SelectedTimesheet.Status = "Rejected";
                await _databaseService.UpdateTimesheetStatusAsync(SelectedTimesheet.TimesheetId, "Rejected");
                StatusMessage = "Timesheet rejected.";
                await LoadTimesheets(); // Refresh timesheets after rejection
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error rejecting timesheet: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

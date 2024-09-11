using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TimekeepingApp.Models;
using TimekeepingApp.Services;

namespace TimekeepingApp.ViewModels
{
    public class AddTimesheetViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private readonly UserModel _currentUser; // Holds the logged-in user
        private readonly MainViewModel _mainViewModel; // Reference to MainViewModel for navigation

        private DateTime? _date;
        private TimeSpan _timeIn;
        private TimeSpan _timeOut;
        private string _statusMessage;

        // Properties for data binding in the view
        public DateTime? Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
                ((RelayCommand)SubmitCommand)?.RaiseCanExecuteChanged();
            }
        }

        public TimeSpan TimeIn
        {
            get => _timeIn;
            set
            {
                _timeIn = value;
                OnPropertyChanged(nameof(TimeIn));
                ((RelayCommand)SubmitCommand)?.RaiseCanExecuteChanged();
            }
        }

        public TimeSpan TimeOut
        {
            get => _timeOut;
            set
            {
                _timeOut = value;
                OnPropertyChanged(nameof(TimeOut));
                ((RelayCommand)SubmitCommand)?.RaiseCanExecuteChanged();
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

        // Commands
        public ICommand SubmitCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ExitCommand { get; }

        // Constructor
        public AddTimesheetViewModel(DatabaseService databaseService, UserModel currentUser, MainViewModel mainViewModel)
        {
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _mainViewModel = mainViewModel ?? throw new ArgumentNullException(nameof(mainViewModel));

            Date = DateTime.Now; // Set default date to today
            TimeIn = TimeSpan.FromHours(9); // Default TimeIn (09:00)
            TimeOut = TimeSpan.FromHours(17); // Default TimeOut (17:00)

            SubmitCommand = new RelayCommand(async _ => await SubmitTimesheet(), CanSubmit);
            CancelCommand = new RelayCommand(Cancel);
            ExitCommand = new RelayCommand(Exit);
        }

        // Method to validate if the timesheet can be submitted
        private bool CanSubmit(object parameter)
        {
            // Ensure Date is set and TimeIn is earlier than TimeOut
            return Date.HasValue && TimeIn < TimeOut;
        }

        // Submit the timesheet
        private async Task SubmitTimesheet()
        {
            try
            {
                if (!Date.HasValue)
                {
                    StatusMessage = "Date must be selected.";
                    return;
                }

                var newTimesheet = new TimesheetModel
                {
                    Date = Date.Value,
                    TimeIn = TimeIn,
                    TimeOut = TimeOut,
                    Status = "Pending", // New timesheets are set to Pending status
                    UserId = _currentUser.UserId // Associate the timesheet with the logged-in user
                };

                await _databaseService.AddTimesheetAsync(newTimesheet);
                StatusMessage = "Timesheet submitted successfully.";
                // Optionally, navigate back to ICView after successful submission
                Exit(null);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error submitting timesheet: {ex.Message}";
            }
        }

        // Method to handle cancel operation
        private void Cancel(object parameter)
        {
            // Reset fields
            Date = DateTime.Now;
            TimeIn = TimeSpan.FromHours(9);
            TimeOut = TimeSpan.FromHours(17);
            StatusMessage = "Submission canceled.";
        }

        // Method to navigate back to ICView
        private void Exit(object parameter)
        {
            _mainViewModel.CurrentViewModel = new ICViewModel(_databaseService, _currentUser);
        }

        // PropertyChanged event implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

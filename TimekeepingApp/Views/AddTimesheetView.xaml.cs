using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TimekeepingApp.ViewModels;
using TimekeepingApp.Services;
using TimekeepingApp.Models;

namespace TimekeepingApp.Views
{
    /// <summary>
    /// Interaction logic for AddTimesheetView.xaml
    /// </summary>
    public partial class AddTimesheetView : UserControl
    {
        public AddTimesheetView()
        {
            InitializeComponent();
        }

        // Add this method to initialize with necessary parameters
        public void Initialize(DatabaseService databaseService, UserModel currentUser, MainViewModel mainViewModel)
        {
            // Set the DataContext with the AddTimesheetViewModel using the provided parameters
            DataContext = new AddTimesheetViewModel(databaseService, currentUser, mainViewModel);
        }
    }
}

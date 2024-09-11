// ManagerView.xaml.cs
using System.Windows.Controls;
using TimekeepingApp.Models;
using TimekeepingApp.Services;
using TimekeepingApp.ViewModels;

namespace TimekeepingApp.Views
{
    public partial class ManagerView : UserControl
    {
        public ManagerView()
        {
            InitializeComponent();
        }

        // Method to initialize the view with necessary dependencies
        public void Initialize(DatabaseService databaseService)
        {
            // Assuming you don't need a specific user object here
            DataContext = new ManagerViewModel(databaseService);
        }
    }
}

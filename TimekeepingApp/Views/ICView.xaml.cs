using System.Windows.Controls;
using TimekeepingApp.Models;
using TimekeepingApp.Services;
using TimekeepingApp.ViewModels;

namespace TimekeepingApp.Views
{
    /// <summary>
    /// Interaction logic for ICView.xaml
    /// </summary>
    public partial class ICView : UserControl
    {
        // Parameterless constructor required for XAML
        public ICView()
        {
            InitializeComponent();
        }

        // Method to set DataContext with required parameters
        public void Initialize(DatabaseService databaseService, UserModel user)
        {
            DataContext = new ICViewModel(databaseService, user);
        }
    }
}

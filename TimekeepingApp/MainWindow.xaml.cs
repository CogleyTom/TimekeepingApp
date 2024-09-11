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
using TimekeepingApp.Data;
using TimekeepingApp.Services;
using TimekeepingApp.ViewModels;

namespace TimekeepingApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Initialize the database context and service
            var dbContext = new AppDbContext();
            var databaseService = new DatabaseService(dbContext);

            // Set the DataContext to MainViewModel
            DataContext = new MainViewModel(databaseService);
        }
    }
}
using System.Windows;
using Bookmaker.ViewModel;

namespace Bookmaker.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public MainWindow()
        {
            Logger.Info("Start App");
            DataContext = new MainWindowViewModel();
            InitializeComponent();
        }
    }
}

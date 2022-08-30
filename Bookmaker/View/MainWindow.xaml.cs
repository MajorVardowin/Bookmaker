using System;
using System.Windows.Controls;
using System.Windows.Input;
using Bookmaker.ViewModel;

namespace Bookmaker.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly MainWindowViewModel? _viewModel;
        public MainWindow()
        {
            Logger.Info("Start App");
            //DataContext = new MainWindowViewModel();
            InitializeComponent();
            try
            {
                _viewModel = (MainWindowViewModel) DataContext;
            }
            catch (Exception e)
            {
                Logger.Fatal(e, "Not possible to cast view model to type MainWindowViewModel");
            }
        }

        private void Bookmarks_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_viewModel == null)
            {
                Logger.Fatal("No binded view model!");
                return;
            }

            try
            {
                ListBox listBoxObj = sender as ListBox ?? throw new InvalidOperationException();
                _viewModel.DoubleClick(listBoxObj, e);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Failed to convert sender objekt to ListBox type");
            }
            
            
        }
    }
}

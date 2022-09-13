using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        private bool desx = false;
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
            if (NoViewModelAttached() || Bookmarks.SelectedItem == null) return;
            Debug.Assert(_viewModel != null, "View model shouldn't be null there!");

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
        
        private void Description_OnLostFocus(object sender, RoutedEventArgs e)
        {
            desx = false;
        }

        private void Description_OnGotFocus(object sender, RoutedEventArgs e)
        {
            desx = true;
        }

        private void Bookmarks_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private bool NoViewModelAttached()
        {
            if (_viewModel != null) return false;
            Logger.Fatal("No binded view model!");
            return true;
        }

        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            return;
            if (NoViewModelAttached()) return;
            Debug.Assert(_viewModel != null, "View model shouldn't be null there!");

            // TODO HERE CODE TO MAKE THE ADDING LOGIC VISIBLE
            _viewModel.AddNewVisibility = Visibility.Visible;
            Bookmarks.SelectedItem = null;
            //https://stackoverflow.com/questions/23133527/wpf-listbox-remove-selection-by-clicking-on-blank-space
        }

        private void DeleteBookmark_OnClick(object sender, RoutedEventArgs e)
        {
            
        }
    }
}

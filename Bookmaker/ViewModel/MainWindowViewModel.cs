using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Bookmaker.Annotations;
using Bookmaker.Logic;
using CSharpFunctionalExtensions;
using static System.Net.WebRequestMethods;

namespace Bookmaker.ViewModel;

public class MainWindowViewModel : INotifyPropertyChanged
{
    public MainWindowViewModel()
    {
        AddBookMark("Google", "https://www.google.com/", "Google main page");
        //AddBookMark("test", "tee", "test1");
        //AddBookMark("dire", "bier", "test2");
        Logger.Error(new Exception("test abbruch", new IndexOutOfRangeException("d")), "Test");
    }
    #region Fields
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    private string _descriptionTest = "";
    private string _addOrDelete = "Add";

    private ICommand _addClicked;
    private ICommand _deleteClicked;
    private ICommand _selectItem;
    private string _seletedBookmark = "Not set";
    private Visibility _addNewVisibility = Visibility.Visible;
    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion

    #region Properties
    public ObservableCollection<string> SavedBookmarks { get; } = new ObservableCollection<string>();
    public SortedList<string, string> BookMarks { get; } = new SortedList<string, string>();
    public SortedList<string, string> Descriptions { get; } = new SortedList<string, string>();

    public string SelectedBookmark
    {
        get => _seletedBookmark;
        set
        {
            _seletedBookmark = value;
            if (value == null)
            {
                DescriptionText = "";
                return;
            }
            DescriptionText = BookMarks[value];
            OnPropertyChanged();
        }
    }

    public string Name { get; set; }
    public string Url { get; set; }

    public string DescriptionText
    {
        get
        {
            if (string.IsNullOrEmpty(SelectedBookmark))
                return "No description set";
            if (!Descriptions.ContainsKey(SelectedBookmark))
                return "No description for " + SelectedBookmark + " set";
            
            return Descriptions[SelectedBookmark];
        }

        set
        {
            _descriptionTest = value;
            OnPropertyChanged();
        }
    }

    public Visibility AddNewVisibility
    {
        get => _addNewVisibility;
        set
        {
            _addNewVisibility = value;
            OnPropertyChanged();
        }
    }

    public string AddOrDelete
    {
        get => _addOrDelete;
        private set
        {
            _addOrDelete = value;
            OnPropertyChanged();
        }
    }

    public ICommand AddClicked
    {
        get
        { 
            return _addClicked ??= new CommandHandler(() => AddBookMark(Name, Url), () => true);
        }
    }

    public ICommand DeleteClicked
    {
        get
        {
            return _deleteClicked ??= new CommandHandler(() => DeleteBookMark(Name), () => true);
        }
    }

    public ICommand SelectItem
    {
        get
        {
            return _selectItem ??= new CommandHandler(() => UpdateDescription(Name), () => true);
        }
    }

    private void UpdateDescription(string name)
    {
        DescriptionText = Descriptions[name];
    }

    #endregion
    
    
    public Result AddBookMark(string name, string url, string description = "")
    {
        try
        {
            //todo check if bookmark already exists
            if (CheckIfExists(name)) return Result.Failure("Already Exists");
            SavedBookmarks.Add(name);
            BookMarks.Add(name, url);

            // todo GetWebsiteIco.GetIcon(name, url);
            
            Descriptions.Add(name, description);
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to add a new bookmark");
            return Result.Failure("Failed to add new bookmark.");
        }
        return Result.Success();
    }

    private bool CheckIfExists(string name)
    {
        return SavedBookmarks.Contains(name) || BookMarks.ContainsKey(name);
    }

    public Result AddDescription(string name, string description)
    {
        try
        {
            //todo check if bookmark already exists
            Descriptions.Add(name, description);
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to add a new bookmark description");
            return Result.Failure("Failed to add new bookmark.");
        }
        return Result.Success();
    }

    public void SwitchAddOrDeleteLabel()
    {
        if (AddOrDelete == "Add") AddOrDelete = "Delete";
        if (AddOrDelete == "Delete") AddOrDelete = "Add";
    }

    public Result DeleteBookMark(string name)
    {
        try
        {
            //todo check if bookmark exist
            SavedBookmarks.Remove(name);
            BookMarks.Remove(name);
            Descriptions.Remove(name);
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to delete a bookmark");
            return Result.Failure("No bookmark deleted");
        }
        return Result.Success();
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void DoubleClick(ListBox listBoxObj, MouseButtonEventArgs mouseButtonEventArgs1)
    {
        string url = "";
        try
        {
            string item = listBoxObj.SelectedItem.ToString() ?? throw new InvalidOperationException();
            url = BookMarks[item];
            if (string.IsNullOrEmpty(url)) throw new Exception("URL is empty");
        }
        catch (InvalidOperationException invalid)
        {
            Logger.Error(invalid, "double clicked item is null");
            return;
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to open linked URL");
            return;
        }

        bool result = Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                      && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        if (!result)
        {
            Logger.Error("No valid URL");
            Uri.TryCreate("https://www.google.de/search?q=" + url, UriKind.Absolute, out uriResult);
        }
        Debug.Assert(uriResult != null, nameof(uriResult) + " != null");

        Process myProcess = new();
        try
        {
            myProcess.StartInfo.UseShellExecute = true;
            myProcess.StartInfo.FileName = uriResult.AbsoluteUri;
            myProcess.Start();
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to open linked URL");
        }
    }
}
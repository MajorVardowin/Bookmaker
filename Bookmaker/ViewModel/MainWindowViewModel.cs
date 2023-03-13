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

namespace Bookmaker.ViewModel;

public class MainWindowViewModel : INotifyPropertyChanged
{
    public MainWindowViewModel()
    {
        AddBookMark("Google", "https://www.google.com/", "Google main page");
        Logger.Error(new Exception("test abbruch", new IndexOutOfRangeException("d")), "Test");
    }

    #region Fields
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    private string _descriptionText = "";
    private string _descriptionTextVisulized = "";
    private string _addOrDelete = "Add";

    private ICommand? _addClicked;
    private ICommand? _deleteClicked;
    private ICommand? _saveDescriptionText;
    private string _name = "";
    private string _url = "";
    private string _seletedBookmark = "";
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
            UpdateDescription();
            OnPropertyChanged();
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }

    public string Url 
    {
        get => _url;
        set
        {
            _url = value;
            OnPropertyChanged();
        }
    }

    public string DescriptionText
    {
        get
        {
            return string.IsNullOrEmpty(SelectedBookmark) ? "No description set" : _descriptionText;
        }

        set
        {
            _descriptionText = value;
            //AddDescription();
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
            return _deleteClicked ??= new CommandHandler(() => DeleteBookMark(SelectedBookmark), () => true);
        }
    }

    public ICommand SaveDescriptionText
    {
        get
        {
            return _saveDescriptionText ??= new CommandHandler(() => AddDescription(), () => true);
        }
    }

    public string DescriptionTextVisulized
    {
        get => _descriptionTextVisulized;
        set
        {
            _descriptionTextVisulized = value;
            OnPropertyChanged();
        }
    }

    private void UpdateDescription()
    {
        Result result = GetDescription(SelectedBookmark);
        if (result.IsFailure)
        {
            Logger.Error("Failed to receive description");
        }
        DescriptionTextVisulized = DescriptionText;
    }


    #endregion


    public Result AddBookMark(string name, string url, string description = "Not set")
    {
        if (string.IsNullOrEmpty(name)) return Result.Failure("Name null or empty");
        if (string.IsNullOrEmpty(url)) return Result.Failure("URL null or empty");
        try
        {
            //todo check if bookmark already exists
            if (CheckIfExists(name)) return Result.Failure("Already Exists");
            SavedBookmarks.Add(name);
            BookMarks.Add(name, url);

            GetWebsiteIcon.GetIcon(name, url);

            Descriptions.Add(name, description);
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to add a new bookmark");
            return Result.Failure("Failed to add new bookmark.");
        }
        finally
        {
            Name = "";
            Url = "";
        }
        return Result.Success();
    }

    private bool CheckIfExists(string name)
    {
        return SavedBookmarks.Contains(name) || BookMarks.ContainsKey(name);
    }

    public Result AddDescription()
    {
        DescriptionText = DescriptionTextVisulized;
        try
        {
            if (BookMarks.ContainsKey(SelectedBookmark))
            {
                Descriptions[SelectedBookmark] = DescriptionText;
            }
            else
            {
                Descriptions.Add(SelectedBookmark, DescriptionText);
            }
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
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (SelectedBookmark == null) return Result.Failure("No bookmark selected");
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

    private Result GetDescription(string nameOfBookmark)
    {
        try
        {
            var description = Descriptions[nameOfBookmark];
            DescriptionText = description;
        }
        catch (Exception e)
        {
            var errorMsg = $"Failed to get description of {nameOfBookmark} from saved bookmarks.";
            Logger.Error(e, errorMsg);
            return Result.Failure(errorMsg);
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
        string url;
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
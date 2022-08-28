using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Bookmaker.Annotations;
using Bookmaker.Logic;
using CSharpFunctionalExtensions;

namespace Bookmaker.ViewModel;

public class MainWindowViewModel : INotifyPropertyChanged
{
    public SortedList<string, string> BookMarks { get; } = new SortedList<string, string>();
    public SortedList<string, string> Descriptions { get; } = new SortedList<string, string>();
    public string SelectedBookmark { get; set; }

    private string _descriptionTest = "";

    public string DescriptionText
    {
        get
        {
            if (Descriptions.ContainsKey("test"))
            {
                int elem = Descriptions.IndexOfKey("test");
                return Descriptions["test"];
            }

            return "No description set";
        }

        set
        {
            _descriptionTest = value;
            OnPropertyChanged();
        }
    }

    public Result AddBookMark(string name, string url, string description = "")
    {
        try
        {
            //todo check if bookmark already exists
            BookMarks.Add(name, url);
            Descriptions.Add(name, description);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Result.Failure("Failed to add new bookmark.");
        }
        return Result.Success();
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
            Console.WriteLine(e);
            return Result.Failure("Failed to add new bookmark.");
        }
        return Result.Success();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
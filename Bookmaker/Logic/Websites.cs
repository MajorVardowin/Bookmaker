using System.ComponentModel;
using System.Runtime.CompilerServices;
using Bookmaker.Annotations;

namespace Bookmaker.Logic;

public class Websites : INotifyPropertyChanged
{
    #region Events

    public event PropertyChangedEventHandler? PropertyChanged;
    #endregion

    #region Fields

    private string _url = "";
    #endregion

    #region Properties
    public string HyperLink
    {
        get => _url;
        set
        {
            _url = value;
            OnPropertyChanged();
        }
    }

    #endregion

    


    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
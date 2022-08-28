using System.ComponentModel;
using System.Runtime.CompilerServices;
using Bookmaker.Annotations;

namespace Bookmaker.Logic;

public class Websites : INotifyPropertyChanged
{
    #region Events

    #endregion

    #region Fields

    #endregion

    #region Properties

    #endregion
    public event PropertyChangedEventHandler? PropertyChanged;

    private string _url = "";
    public string HyperLink
    {
        get => _url;
        set
        {
            _url = value;
            OnPropertyChanged();
        }
    }

    
    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
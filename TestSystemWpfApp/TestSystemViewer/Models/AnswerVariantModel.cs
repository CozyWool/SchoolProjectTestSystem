using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestSystemViewer.Models;

public sealed class AnswerVariantModel : INotifyPropertyChanged
{
    private string _text;
    private bool _isSelected;

    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            OnPropertyChanged();
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
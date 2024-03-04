using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestSystemClassLibrary.Models;

public sealed class QuestionVariant : INotifyPropertyChanged
{
    private string _text;
    private bool _isCorrect;
    public bool IsChanged { get; set; }

    public QuestionVariant() : this("", false)
    {
        
    }
    public QuestionVariant(string text, bool isCorrect)
    {
        _text = text;
        _isCorrect = isCorrect;
        IsChanged = false;
    }
    
    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            OnPropertyChanged();
        }
    }

    public bool IsCorrect
    {
        get => _isCorrect;
        set
        {
            _isCorrect = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        IsChanged = true;
    }
}
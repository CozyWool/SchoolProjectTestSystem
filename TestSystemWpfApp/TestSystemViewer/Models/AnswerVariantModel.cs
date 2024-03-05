namespace TestSystemViewer.Models;

public sealed class AnswerVariantModel : NotifyModelBase
{
    private string _text;
    private bool _isSelected;

    public int Index { get; set; }

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
}
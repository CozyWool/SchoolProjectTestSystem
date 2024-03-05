namespace TestEditorWpfApp.Models;

public class AnswerModel : NotifyModelBase
{
    private string _text;

    public bool IsChanged { get; private set; }

    public bool IsSelected { get; set; }

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

    public void Reset()
    {
        IsChanged = false;
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        IsChanged = true;
    }
}
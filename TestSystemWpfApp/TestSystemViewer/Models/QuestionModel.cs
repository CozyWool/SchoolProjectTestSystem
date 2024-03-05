using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestSystemViewer.Models;

public class QuestionModel : INotifyPropertyChanged
{
    private string _conditionText;

    public int CorrectVariantNumber
    {
        get
        {
            if (First.IsSelected) return 1;
            if (Second.IsSelected) return 2;
            if (Third.IsSelected) return 3;
            if (Fourth.IsSelected) return 4;

            return -1;
        }
        set
        {
            switch (value)
            {
                case 1:
                    First.IsSelected = true;
                    break;
                case 2:
                    Second.IsSelected = true;
                    break;
                case 3:
                    Third.IsSelected = true;
                    break;
                case 4:
                    Fourth.IsSelected = true;
                    break;
            }
        }
    }

    public string ConditionText
    {
        get => _conditionText;
        set
        {
            _conditionText = value;
            OnPropertyChanged();
        }
    }

    public AnswerVariantModel First { get; set; }

    public AnswerVariantModel Second { get; set; }

    public AnswerVariantModel Third { get; set; }

    public AnswerVariantModel Fourth { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
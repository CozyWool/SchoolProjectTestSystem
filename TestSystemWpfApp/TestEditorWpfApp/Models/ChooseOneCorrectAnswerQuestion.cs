using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestEditorWpfApp.Models;

public class ChooseOneCorrectAnswerQuestion : INotifyPropertyChanged
{
    private string _conditionText;
    private bool _isConditionTextChanged;
    public bool IsChanged
    {
        get => FirstVariant.IsChanged || SecondVariant.IsChanged || ThirdVariant.IsChanged || FourthVariant.IsChanged || _isConditionTextChanged;
        set
        {
            FirstVariant.IsChanged = value;
            SecondVariant.IsChanged = value;
            ThirdVariant.IsChanged = value;
            FourthVariant.IsChanged = value;
            _isConditionTextChanged = value;
        }
    }

    public ChooseOneCorrectAnswerQuestion() : this("Новый вопрос", new QuestionVariant(), new QuestionVariant(),
        new QuestionVariant(), new QuestionVariant())
    {
    }

    public ChooseOneCorrectAnswerQuestion(string conditionText, QuestionVariant firstVariant,
        QuestionVariant secondVariant, QuestionVariant thirdVariant, QuestionVariant fourthVariant)
    {
        _conditionText = conditionText;
        FirstVariant = firstVariant;
        SecondVariant = secondVariant;
        ThirdVariant = thirdVariant;
        FourthVariant = fourthVariant;
        IsChanged = false;
        _isConditionTextChanged = false;
    }

    public bool IsFilled => ConditionText.Length > 0 &&
                            FirstVariant.Text.Length > 0 &&
                            SecondVariant.Text.Length > 0 &&
                            ThirdVariant.Text.Length > 0 &&
                            FourthVariant.Text.Length > 0 &&
                            (FirstVariant.IsCorrect || SecondVariant.IsCorrect || ThirdVariant.IsCorrect ||
                             FourthVariant.IsCorrect);


    public string ConditionText
    {
        get => _conditionText;
        set
        {
            _conditionText = value;
            _isConditionTextChanged = true;
            OnPropertyChanged();
        }
    }

    public QuestionVariant FirstVariant { get; }

    public QuestionVariant SecondVariant { get; }

    public QuestionVariant ThirdVariant { get; }

    public QuestionVariant FourthVariant { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        IsChanged = true;
    }
}
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestSystemWpfApp.Models;

public class ChooseOneCorrectAnswerQuestion : INotifyPropertyChanged
{
    private string _conditionText;

    public int CorrectVariantNumber
    {
        get
        {
            if (FirstVariant.IsCorrect) return 1;
            if (SecondVariant.IsCorrect) return 2;
            if (ThirdVariant.IsCorrect) return 3;
            if (FourthVariant.IsCorrect) return 4;
            
            return -1;
        }
        set
        {
            switch (value)
            {
               case 1: 
                   FirstVariant.IsCorrect = true;
                   break;
               case 2:
                   SecondVariant.IsCorrect = true;
                   break;
               case 3:
                   ThirdVariant.IsCorrect = true;
                   break;
               case 4:
                   FourthVariant.IsCorrect = true;
                   break;
            }  
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

    public QuestionVariant FirstVariant { get; }

    public QuestionVariant SecondVariant { get; }

    public QuestionVariant ThirdVariant { get; }

    public QuestionVariant FourthVariant { get; }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
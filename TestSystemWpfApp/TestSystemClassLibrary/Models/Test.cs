using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestSystemClassLibrary.Models;

public class Test : INotifyPropertyChanged
{
    private ObservableCollection<ChooseOneCorrectAnswerQuestion> _questionList;

    public Test()
    {
        _questionList = new ObservableCollection<ChooseOneCorrectAnswerQuestion>();
    }

    public ObservableCollection<ChooseOneCorrectAnswerQuestion> QuestionList
    {
        get => _questionList;
        set
        {
            _questionList = value; 
            OnPropertyChanged();
        }
    }

    public bool IsAllQuestionFilled => _questionList.All(question => question.IsFilled);
    public bool IsTestChanged
    {
        get => _questionList.All(question => question.IsChanged);
        set => _questionList.AsParallel().ForAll(question => question.IsChanged = value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
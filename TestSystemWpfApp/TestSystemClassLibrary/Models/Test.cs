using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestSystemClassLibrary.Models;

public class Test : INotifyPropertyChanged
{
    private ObservableCollection<ChooseOneCorrectAnswerQuestion> _questionList;
    private string _name;

    public Test() : this(new(), "Новый тест", false)
    {
        QuestionList = new ObservableCollection<ChooseOneCorrectAnswerQuestion>();
        _name = "Новый тест1";
    }

    public Test(ObservableCollection<ChooseOneCorrectAnswerQuestion> questionList, string name, bool isNameChanged)
    {
        _questionList = questionList;
        _name = name;
    }

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
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

    public ObservableCollection<int> CorrectAnswers =>
        new(_questionList.Select(question => question.CorrectVariantNumber).ToList());

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
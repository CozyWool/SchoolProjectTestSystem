using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace TestSystemClassLibrary.Models;

public class Test : INotifyPropertyChanged
{
    private ObservableCollection<ChooseOneCorrectAnswerQuestion> _questionList;
    private string _name;
    private bool _isNameChanged;


    public Test() : this(new(), "Новый тест", false)
    {
        QuestionList = new ObservableCollection<ChooseOneCorrectAnswerQuestion>();
        _name = "Новый тест1";
        _isNameChanged = false;
    }

    [JsonConstructor]
    public Test(ObservableCollection<ChooseOneCorrectAnswerQuestion> questionList, string name, bool isNameChanged)
    {
        _questionList = questionList;
        _name = name;
        _isNameChanged = isNameChanged;
    }


    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            _isNameChanged = true;
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

    [JsonIgnore]
    public ObservableCollection<int> CorrectAnswers => new(_questionList.Select(question => question.CorrectVariantNumber).ToList());

    [JsonIgnore]
    public bool IsAllQuestionFilled => _questionList.All(question => question.IsFilled);
    [JsonIgnore]

    public bool IsTestChanged
    {
        get => _questionList.All(question => question.IsChanged) && _isNameChanged;
        set => _questionList.AsParallel().ForAll(question => question.IsChanged = value);
    }


    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TestSystemEditor.Models;

public class QuestionModel : NotifyModelBase
{
    public QuestionModel()
    {
        _answers = [];
        _answers.CollectionChanged += (_, _) => { IsChanged = true; };
    }

    private int _correctAnswerNumber;
    private string _conditionText;
    private ObservableCollection<AnswerModel> _answers;

    public bool IsChanged { get; private set; }

    public int CorrectAnswerNumber
    {
        get => _correctAnswerNumber;
        set
        {
            _correctAnswerNumber = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<AnswerModel> Answers
    {
        get => _answers;
        set
        {
            UnSubscribe(_answers);
            _answers = value;
            Subscribe(_answers);
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

    public void Reset()
    {
        foreach (var answer in Answers)
        {
            answer.Reset();
        }
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        IsChanged = true;
    }

    private void Subscribe(ObservableCollection<AnswerModel> answers)
    {
        foreach (var answer in answers)
        {
            answer.PropertyChanged += answerOnPropertyChanged();
        }
    }

    private void UnSubscribe(ObservableCollection<AnswerModel> answers)
    {
        foreach (var answer in answers)
        {
            answer.PropertyChanged -= answerOnPropertyChanged();
        }
    }

    private PropertyChangedEventHandler answerOnPropertyChanged()
    {
        return (_, _) => IsChanged = true;
    }
}
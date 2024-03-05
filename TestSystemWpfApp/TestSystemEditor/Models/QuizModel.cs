using System.Collections.ObjectModel;

namespace TestSystemEditor.Models;

public class QuizModel : NotifyModelBase
{
    private bool _isChanged;
    private string _name;

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<QuestionModel> Questions { get; set; }

    public bool IsAllQuestionFilled
    {
        get
        {
            return Questions.All(q =>
                q.ConditionText.Length > 0 && q.Answers.All(a => a.Text.Length > 0) && q.CorrectAnswerNumber >= 0);
        }
    }

    public bool IsTestChanged => Questions.Any(q => q.IsChanged) || _isChanged;

    public void Reset()
    {
        foreach (var question in Questions)
        {
            question.Reset();
        }
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        _isChanged = true;
    }
}
using System.Collections.ObjectModel;

namespace TestSystemViewer.Models;

public class TestModel : NotifyModelBase
{
    private ObservableCollection<QuestionModel> _questions;
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

    public ObservableCollection<QuestionModel> Questions
    {
        get => _questions;
        set
        {
            _questions = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<int> CorrectAnswers =>
        new(_questions.Select(question => question.CorrectVariantNumber).ToList());
}
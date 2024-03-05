using System.Collections.ObjectModel;

namespace TestSystemViewer.Models;

public class QuestionModel : NotifyModelBase
{
    private string _conditionText;

    public int CorrectVariantNumber
    {
        get
        {
            for (var index = 0; index < Answers.Count; index++)
            {
                var answer = Answers[index];
                if (answer.IsSelected)
                {
                    return index;
                }
            }

            return -1;
        }
        /*set
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
        }*/
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

    public ObservableCollection<AnswerVariantModel> Answers { get; set; }
}
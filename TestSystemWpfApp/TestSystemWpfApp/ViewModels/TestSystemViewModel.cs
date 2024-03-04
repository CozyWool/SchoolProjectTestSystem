using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using AutoMapper;
using TestSystemClassLibrary;
using TestSystemClassLibrary.Commands;
using TestSystemClassLibrary.Models;
using TestSystemWpfApp.Views;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using MessageBox = System.Windows.MessageBox;

namespace TestSystemWpfApp.ViewModels;

public class TestSystemViewModel : INotifyPropertyChanged
{
    private ChooseOneCorrectAnswerQuestion _currentQuestion;
    private ObservableCollection<int> _userAnswers;
    private readonly TestSystemView _owner;
    private readonly IMapper _mapper;
    private int _currentQuestionIndex;
    private Test _currentTest;
    private string _beforeTestText;
    private string _resultTestText;
    private bool _isTestGoing;
    private bool IsTestSelected => CurrentTest != null;
    private bool IsQuestionSelected => CurrentQuestionIndex != -1;

    public bool IsTestGoing
    {
        get => _isTestGoing;
        set
        {
            _isTestGoing = value;
            OnPropertyChanged();
        }
    }

    public Test CurrentTest
    {
        get => _currentTest;
        set
        {
            _currentTest = value;
            _userAnswers = new ObservableCollection<int>();
            for (var i = 0; i < _currentTest?.QuestionList.Count; i++)
            {
                _userAnswers.Add(-1);
            }

            OnPropertyChanged();
        }
    }

    public ChooseOneCorrectAnswerQuestion CurrentQuestion
    {
        get => _currentQuestion;
        set
        {
            _currentQuestion = value;
            OnPropertyChanged();
        }
    }

    public int CurrentQuestionIndex
    {
        get => _currentQuestionIndex;
        set
        {
            if (CurrentTest == null || value < 0 || value > CurrentTest.QuestionList.Count) return;

            _currentQuestionIndex = value;
            CurrentQuestion = CurrentTest.QuestionList[CurrentQuestionIndex];
            OnPropertyChanged();
        }
    }

    public Command OpenTestCommand { get; set; }
    public Command CloseTestCommand { get; set; }
    public Command StartTestCommand { get; set; }
    public Command EndTestCommand { get; set; }
    public Command NextQuestionCommand { get; set; }
    public Command PreviousQuestionCommand { get; set; }
    public Command QuitCommand { get; set; }
    public Command AnswerCommand { get; set; }
    public string QuestionText { get; set; }

    public string BeforeTestText
    {
        get => _beforeTestText;
        set
        {
            _beforeTestText = value;
            OnPropertyChanged();
        }
    }

    public string ResultTestText
    {
        get => _resultTestText;
        set
        {
            _resultTestText = value;
            OnPropertyChanged();
        }
    }

    public TestSystemViewModel(TestSystemView owner, IMapper mapper)
    {
        _owner = owner;
        _mapper = mapper;
        _owner.DataContext = this;

        QuestionText = "Информация:";
        _currentQuestionIndex = -1;
        IsTestGoing = false;

        NextQuestionCommand = new DelegateCommand(_ => NextQuestion(),
            _ => IsQuestionSelected && IsTestSelected && CurrentQuestionIndex + 1 < CurrentTest.QuestionList.Count);
        PreviousQuestionCommand =
            new DelegateCommand(_ => PreviousQuestion(),
                _ => IsQuestionSelected && IsTestSelected && CurrentQuestionIndex - 1 >= 0);
        OpenTestCommand = new DelegateCommand(_ => OpenTest());
        CloseTestCommand = new DelegateCommand(_ => CloseTest());
        QuitCommand = new DelegateCommand(_ => Quit());
        AnswerCommand = new DelegateCommand(answerNumber => Answer(Convert.ToInt32(answerNumber)),
            _ => IsTestSelected && IsQuestionSelected);
        StartTestCommand = new DelegateCommand(_ => StartTest(), _ => !_isTestGoing && IsTestSelected);
        EndTestCommand = new DelegateCommand(_ => EndTest(), _ => IsTestGoing);
    }

    private void StartTest()
    {
        ShowConditionTextBox();
        IsTestGoing = true;
    }

    private void EndTest()
    {
        var messageBoxText = "Точно завершить тест?";
        if (_userAnswers.Contains(-1))
            messageBoxText = "Вы ответили не на все вопросы, все равно завершить тест?";

        var dialogResult = MessageBox.Show(messageBoxText,
            "Завершить тест?", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (dialogResult == MessageBoxResult.No)
            return;
        ResultTestText = GetTestResult();

        IsTestGoing = false;
        CurrentTest = null;
        ShowTestResultRichTextBox();
    }

    private string GetTestResult()
    {
        var rightAnswersCount = CurrentTest.CorrectAnswers.Where((t, i) => t == _userAnswers[i]).Count();
        var donePercentage = Math.Round((double) rightAnswersCount / CurrentTest.CorrectAnswers.Count * 100, 1);
        int mark = -1;
        switch (donePercentage)
        {
            case >= 80:
                mark = 5;
                break;
            case >= 70:
                mark = 4;
                break;
            case >= 50:
                mark = 3;
                break;
            default:
                mark = 2;
                break;
        }

        var result = $"Тест {CurrentTest.Name} закончен.\n" +
                     $"Всего заданий в тесте: {CurrentTest.QuestionList.Count}.\n" +
                     $"Правильно: {rightAnswersCount}\n" +
                     $"Ваш результат: {donePercentage}%.\n" +
                     $"Оценка: {mark}.";

        return result;
    }

    private void CloseTest()
    {
        CurrentTest = null;
        ShowChooseTestRichTextBox();
    }

    private void Answer(int answerNumber)
    {
        if (answerNumber < 0 || answerNumber >= _userAnswers.Count) return;

        _userAnswers[_currentQuestionIndex] = answerNumber;
    }


    private void Quit()
    {
        _owner.Close();
    }

    private void NextQuestion()
    {
        if (!IsQuestionSelected || CurrentTest == null ||
            CurrentQuestionIndex + 1 >= CurrentTest.QuestionList.Count) return;

        CurrentQuestionIndex++;
        ResetRadioButtons();
    }

    private void PreviousQuestion()
    {
        if (!IsQuestionSelected || CurrentTest == null || CurrentQuestionIndex - 1 < 0) return;

        CurrentQuestionIndex--;
        ResetRadioButtons();
    }

    private void ResetRadioButtons()
    {
        _owner.FirstRadioButton.IsChecked = _userAnswers[_currentQuestionIndex] == 1;
        _owner.SecondRadioButton.IsChecked = _userAnswers[_currentQuestionIndex] == 2;
        _owner.ThirdRadioButton.IsChecked = _userAnswers[_currentQuestionIndex] == 3;
        _owner.FourthRadioButton.IsChecked = _userAnswers[_currentQuestionIndex] == 4;
    }

    private void ShowChooseTestRichTextBox()
    {
        _owner.ChooseTestRichTextBox.Visibility = Visibility.Visible;
        _owner.StartTestRichTextBox.Visibility = Visibility.Collapsed;
        _owner.TestResultRichTextBox.Visibility = Visibility.Collapsed;
        _owner.ConditionTextBox.Visibility = Visibility.Collapsed;
    }
    private void ShowStartTestRichTextBox()
    {
        _owner.ChooseTestRichTextBox.Visibility = Visibility.Collapsed;
        _owner.StartTestRichTextBox.Visibility = Visibility.Visible;
        _owner.TestResultRichTextBox.Visibility = Visibility.Collapsed;
        _owner.ConditionTextBox.Visibility = Visibility.Collapsed;
    }
    private void ShowTestResultRichTextBox()
    {
        _owner.ChooseTestRichTextBox.Visibility = Visibility.Collapsed;
        _owner.StartTestRichTextBox.Visibility = Visibility.Collapsed;
        _owner.TestResultRichTextBox.Visibility = Visibility.Visible;
        _owner.ConditionTextBox.Visibility = Visibility.Collapsed;
    }
    private void ShowConditionTextBox()
    {
        _owner.ChooseTestRichTextBox.Visibility = Visibility.Collapsed;
        _owner.StartTestRichTextBox.Visibility = Visibility.Collapsed;
        _owner.TestResultRichTextBox.Visibility = Visibility.Collapsed;
        _owner.ConditionTextBox.Visibility = Visibility.Visible;
    }

    private void OpenTest()
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "JSON файлы(*.json)|*.json",
            RestoreDirectory = true
        };
        var result = dialog.ShowDialog();
        if (result != DialogResult.OK) return;

        CurrentTest = _mapper.Map<Test>(TestFileManager.Load(dialog.FileName)) ?? throw new InvalidOperationException();
        CurrentQuestionIndex = 0;
        BeforeTestText = $"Тест \"{CurrentTest.Name}\" открыт.\n" +
                         $"Всего заданий в тесте {CurrentTest.QuestionList.Count}\n" +
                         "Удачи!";
        ShowStartTestRichTextBox();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
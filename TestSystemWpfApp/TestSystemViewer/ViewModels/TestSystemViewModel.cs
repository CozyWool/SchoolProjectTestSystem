using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using AutoMapper;
using TestSystem.Infrastructure;
using TestSystem.Infrastructure.Commands;
using TestSystemViewer.Models;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using MessageBox = System.Windows.MessageBox;

namespace TestSystemViewer.ViewModels;

public class TestSystemViewModel : INotifyPropertyChanged
{
    private QuestionModel _currentQuestion;
    private ObservableCollection<int> _userAnswers;
    private readonly IMapper _mapper;
    private int _currentQuestionIndex;
    private TestModel _currentTestModel;
    private string _beforeTestText;
    private string _resultTestText;
    private bool _isTestGoing;
    private bool _isChooseTestVisible;
    private bool _isStartTestVisible;
    private bool _isTestResultsVisible;
    private bool _isConditionTextVisible;
    private bool IsTestSelected => CurrentTest != null;
    private bool IsQuestionSelected => CurrentQuestionIndex != -1;

    public Action CloseAction { get; set; }

    public bool IsTestGoing
    {
        get => _isTestGoing;
        set
        {
            _isTestGoing = value;
            OnPropertyChanged();
        }
    }

    public TestModel CurrentTest
    {
        get => _currentTestModel;
        set
        {
            _currentTestModel = value;
            _userAnswers = new ObservableCollection<int>();
            for (var i = 0; i < _currentTestModel?.Questions.Count; i++)
            {
                _userAnswers.Add(-1);
            }

            OnPropertyChanged();
        }
    }

    public QuestionModel CurrentQuestion
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
            if (CurrentTest == null || value < 0 || value > CurrentTest.Questions.Count) return;

            _currentQuestionIndex = value;
            CurrentQuestion = CurrentTest.Questions[CurrentQuestionIndex];
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

    public TestSystemViewModel(IMapper mapper)
    {
        _mapper = mapper;

        QuestionText = "Информация:";
        _currentQuestionIndex = -1;
        IsTestGoing = false;

        NextQuestionCommand = new DelegateCommand(_ => NextQuestion(),
            _ => IsQuestionSelected && IsTestSelected && CurrentQuestionIndex + 1 < CurrentTest.Questions.Count);
        PreviousQuestionCommand =
            new DelegateCommand(_ => PreviousQuestion(),
                _ => IsQuestionSelected && IsTestSelected && CurrentQuestionIndex - 1 >= 0);
        OpenTestCommand = new DelegateCommand(_ => OpenTest());
        CloseTestCommand = new DelegateCommand(_ => CloseTest());
        QuitCommand = new DelegateCommand(_ => CloseAction?.Invoke());
        AnswerCommand = new DelegateCommand(answerNumber => Answer(Convert.ToInt32(answerNumber)),
            _ => IsTestSelected && IsQuestionSelected);
        StartTestCommand = new DelegateCommand(_ => StartTest(), _ => !_isTestGoing && IsTestSelected);
        EndTestCommand = new DelegateCommand(_ => EndTest(), _ => IsTestGoing);

        ShowChooseTestRichTextBox();
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
        var donePercentage = Math.Round((double)rightAnswersCount / CurrentTest.CorrectAnswers.Count * 100, 1);
        var mark = donePercentage switch
        {
            >= 80 => 5,
            >= 70 => 4,
            >= 50 => 3,
            _ => 2
        };

        var result = $"Тест {CurrentTest.Name} закончен.\n" +
                     $"Всего заданий в тесте: {CurrentTest.Questions.Count}.\n" +
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

    private void NextQuestion()
    {
        if (!IsQuestionSelected || CurrentTest == null ||
            CurrentQuestionIndex + 1 >= CurrentTest.Questions.Count) return;

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
        /*_owner.FirstRadioButton.IsChecked = _userAnswers[_currentQuestionIndex] == 1;
        _owner.SecondRadioButton.IsChecked = _userAnswers[_currentQuestionIndex] == 2;
        _owner.ThirdRadioButton.IsChecked = _userAnswers[_currentQuestionIndex] == 3;
        _owner.FourthRadioButton.IsChecked = _userAnswers[_currentQuestionIndex] == 4;*/
    }

    public bool IsChooseTestVisible
    {
        get => _isChooseTestVisible;
        set
        {
            _isChooseTestVisible = value;
            OnPropertyChanged();
        }
    }

    public bool IsStartTestVisible
    {
        get => _isStartTestVisible;
        set
        {
            _isStartTestVisible = value;
            OnPropertyChanged();
        }
    }

    public bool IsTestResultsVisible
    {
        get => _isTestResultsVisible;
        set
        {
            _isTestResultsVisible = value;
            OnPropertyChanged();
        }
    }

    public bool IsConditionTextVisible
    {
        get => _isConditionTextVisible;
        set
        {
            _isConditionTextVisible = value;
            OnPropertyChanged();
        }
    }

    private void ShowChooseTestRichTextBox()
    {
        IsChooseTestVisible = true;
        IsStartTestVisible = false;
        IsTestResultsVisible = false;
        IsConditionTextVisible = false;
    }

    private void ShowStartTestRichTextBox()
    {
        IsChooseTestVisible = false;
        IsStartTestVisible = true;
        IsTestResultsVisible = false;
        IsConditionTextVisible = false;
    }

    private void ShowTestResultRichTextBox()
    {
        IsChooseTestVisible = false;
        IsStartTestVisible = false;
        IsTestResultsVisible = true;
        IsConditionTextVisible = false;
    }

    private void ShowConditionTextBox()
    {
        IsChooseTestVisible = false;
        IsStartTestVisible = false;
        IsTestResultsVisible = false;
        IsConditionTextVisible = true;
    }

    private void OpenTest()
    {
        using var dialog = new OpenFileDialog();
        dialog.Filter = "JSON файлы(*.json)|*.json";
        dialog.RestoreDirectory = true;
        var result = dialog.ShowDialog();
        if (result != DialogResult.OK) return;

        CurrentTest = _mapper.Map<TestModel>(TestFileManager.Load(dialog.FileName)) ??
                      throw new InvalidOperationException();
        CurrentQuestionIndex = 0;
        BeforeTestText = $"Тест \"{CurrentTest.Name}\" открыт.\n" +
                         $"Всего заданий в тесте {CurrentTest.Questions.Count}\n" +
                         "Удачи!";
        ShowStartTestRichTextBox();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
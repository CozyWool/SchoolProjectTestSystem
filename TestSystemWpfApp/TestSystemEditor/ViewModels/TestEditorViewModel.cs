using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using AutoMapper;
using TestSystem.Dto;
using TestSystem.Infrastructure;
using TestSystem.Infrastructure.Commands;
using TestSystemEditor.Models;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;
using MessageBox = System.Windows.MessageBox;

namespace TestSystemEditor.ViewModels;

public class TestEditorViewModel : INotifyPropertyChanged
{
    private QuestionModel _selectedQuestion;
    private readonly Window _owner;
    private readonly IMapper _mapper;

    private int _selectedQuestionIndex;

    private QuizModel _currentTest;

    private bool IsTestCreated => CurrentTest != null;

    private bool IsQuestionSelected => SelectedQuestionIndex != -1;

    public QuizModel CurrentTest
    {
        get => _currentTest;
        set
        {
            _currentTest = value;
            OnPropertyChanged();
        }
    }

    public QuestionModel SelectedQuestion
    {
        get => _selectedQuestion;
        set
        {
            _selectedQuestion = value;
            OnPropertyChanged();
        }
    }

    public int SelectedQuestionIndex
    {
        get => _selectedQuestionIndex;
        set
        {
            if (_selectedQuestionIndex != -1 && _selectedQuestion != null)
            {
                CurrentTest.Questions[_selectedQuestionIndex].CorrectAnswerNumber =
                    _selectedQuestion.CorrectAnswerNumber;
            }

            _selectedQuestionIndex = value;
            OnPropertyChanged();
        }
    }

    public Command CreateTestCommand { get; set; }

    public Command OpenTestCommand { get; set; }

    public Command CreateQuestionCommand { get; set; }

    public Command DeleteQuestionCommand { get; set; }

    public Command NextQuestionCommand { get; set; }

    public Command PreviousQuestionCommand { get; set; }

    public Command SaveTestCommand { get; set; }

    public Command QuitCommand { get; set; }

    public TestEditorViewModel(Window owner, IMapper mapper)
    {
        _owner = owner;
        _owner.DataContext = this;
        _mapper = mapper;

        CreateTestCommand = new DelegateCommand(_ => CreateTest());
        CreateQuestionCommand = new DelegateCommand(_ => CreateQuestion(), _ => IsTestCreated);
        DeleteQuestionCommand = new DelegateCommand(_ => DeleteQuestion(), _ => IsQuestionSelected && IsTestCreated);
        NextQuestionCommand = new DelegateCommand(_ => NextQuestion(), _ => IsQuestionSelected && IsTestCreated);
        PreviousQuestionCommand =
            new DelegateCommand(_ => PreviousQuestion(), _ => IsQuestionSelected && IsTestCreated);
        SaveTestCommand = new DelegateCommand(_ =>
        {
            try
            {
                SaveTest();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message,
                    "Внимание!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }, _ => IsTestCreated);
        OpenTestCommand = new DelegateCommand(_ => OpenTest());
        QuitCommand = new DelegateCommand(_ => Quit());
    }

    public bool SaveChanges()
    {
        if (CurrentTest is not { IsTestChanged: true }) return true;

        var dialogResult = MessageBox.Show("Сохранить текущий тест?",
            "Сохранение...",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (dialogResult != MessageBoxResult.Yes) return true;

        try
        {
            SaveTest();
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(ex.Message,
                "Внимание!",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    private void SaveTest()
    {
        if (CurrentTest == null) return;

        if (!CurrentTest.IsAllQuestionFilled)
        {
            throw new InvalidOperationException("Не все вопросы полностью заполнены!\n" +
                                                "Пожалуйста, заполните их перед сохранением.");
        }

        using var dialog = new SaveFileDialog();
        dialog.Filter = "JSON файлы(*.json)|*.json";
        dialog.RestoreDirectory = true;
        var result = dialog.ShowDialog();
        if (result != DialogResult.OK) return;

        var savePath = dialog.FileName;
        var saveResult = TestFileManager.Save(_mapper.Map<Quiz>(CurrentTest), savePath);

        if (saveResult)
        {
            CurrentTest.Reset();
            MessageBox.Show($"Файл сохранен в папку программы под названием: {savePath}.", "Сохранение",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show("Произошла ошибка при сохранении файла.", "Ошибка!", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void Quit()
    {
        _owner.Close();
    }

    private void NextQuestion()
    {
        if (!IsQuestionSelected || CurrentTest == null) return;

        if (SelectedQuestionIndex + 1 < CurrentTest.Questions.Count)
        {
            SelectedQuestionIndex++;
        }
        else
        {
            SelectedQuestionIndex = 0;
        }
    }

    private void PreviousQuestion()
    {
        if (!IsQuestionSelected || CurrentTest == null) return;

        if (SelectedQuestionIndex - 1 >= 0)
        {
            SelectedQuestionIndex--;
        }
        else
        {
            SelectedQuestionIndex = CurrentTest.Questions.Count - 1;
        }
    }

    private void OpenTest()
    {
        if (!SaveChanges()) return;

        using var dialog = new OpenFileDialog();
        dialog.Filter = "JSON файлы(*.json)|*.json";
        dialog.RestoreDirectory = true;
        var result = dialog.ShowDialog();
        if (result != DialogResult.OK) return;

        CurrentTest = _mapper.Map<QuizModel>(TestFileManager.Load(dialog.FileName)) ??
                      throw new InvalidOperationException();
    }

    private void CreateTest()
    {
        if (!SaveChanges()) return;

        CurrentTest = new QuizModel();
        CreateQuestion();
    }

    private void DeleteQuestion()
    {
        CurrentTest?.Questions.RemoveAt(SelectedQuestionIndex);
    }

    private void CreateQuestion()
    {
        CurrentTest?.Questions.Add(new QuestionModel());
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
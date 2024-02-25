using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using TestSystemClassLibrary;
using TestSystemClassLibrary.Commands;
using TestSystemClassLibrary.Models;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;
using MessageBox = System.Windows.MessageBox;

namespace TestEditorWpfApp.ViewModels;

public class TestEditorViewModel : INotifyPropertyChanged
{
    private ChooseOneCorrectAnswerQuestion _selectedQuestion;
    private readonly Window _owner;
    private int _selectedQuestionIndex;
    private Test? _currentTest;
    private bool IsTestCreated => CurrentTest != null;
    private bool IsQuestionSelected => SelectedQuestionIndex != -1;

    public Test? CurrentTest
    {
        get => _currentTest;
        set
        {
            _currentTest = value;
            OnPropertyChanged();
        }
    }

    public ChooseOneCorrectAnswerQuestion SelectedQuestion
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
            _selectedQuestionIndex = value;
            OnPropertyChanged();
        }
    }

    public Command CreateNewTestCommand { get; set; }
    public Command OpenTestCommand { get; set; }
    public Command CreateNewQuestionCommand { get; set; }
    public Command DeleteQuestionCommand { get; set; }
    public Command NextQuestionCommand { get; set; }
    public Command PreviousQuestionCommand { get; set; }

    public Command SaveTestCommand { get; set; }
    public Command QuitCommand { get; set; }

    public TestEditorViewModel(Window owner)
    {
        _owner = owner;
        _owner.DataContext = this;

        CreateNewTestCommand = new DelegateCommand(_ => CreateNewTest());

        CreateNewQuestionCommand = new DelegateCommand(_ => CreateNewQuestion(), _ => IsTestCreated);
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
        OpenTestCommand = new DelegateCommand(_ => LoadTest());
        QuitCommand = new DelegateCommand(_ => Quit());
    }

    public bool SaveChanges()
    {
        if (CurrentTest is not {IsTestChanged: true}) return true;

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

        using var dialog = new SaveFileDialog
        {
            Filter = "JSON файлы(*.json)|*.json",
            RestoreDirectory = true
        };
        var result = dialog.ShowDialog();
        if (result != DialogResult.OK) return;

        var savePath = dialog.FileName;
        var saveResult = TestFileManager.Save(CurrentTest, savePath);
        
        if (saveResult)
        {
            CurrentTest.IsTestChanged = false;
            MessageBox.Show($"Файл сохранен в папку программы под названием: {savePath}.", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show("Произошла ошибка при сохранении файла.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Quit()
    {
        _owner.Close();
    }

    private void NextQuestion()
    {
        if (!IsQuestionSelected || CurrentTest == null) return;

        if (SelectedQuestionIndex + 1 < CurrentTest.QuestionList.Count)
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
            SelectedQuestionIndex = CurrentTest.QuestionList.Count - 1;
        }
    }


    private void LoadTest()
    {
        if (!SaveChanges()) return;

        using var dialog = new OpenFileDialog
        {
            Filter = "JSON файлы(*.json)|*.json",
            RestoreDirectory = true
        };
        var result = dialog.ShowDialog();
        if (result != DialogResult.OK) return;

        CurrentTest = TestFileManager.Load(dialog.FileName) ?? throw new InvalidOperationException();
    }

    private void CreateNewTest()
    {
        if (!SaveChanges()) return;

        CurrentTest = new Test();
        CreateNewQuestion();
    }

    private void DeleteQuestion()
    {
        CurrentTest?.QuestionList.RemoveAt(SelectedQuestionIndex);
    }

    private void CreateNewQuestion()
    {
        CurrentTest?.QuestionList.Add(new ChooseOneCorrectAnswerQuestion());
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
namespace TestSystemWpf.Dto;

public class Question
{
    public int CorrectAnswerNumber { get; set; }

    public Answer[] Answers { get; set; }

    public string ConditionText { get; set; }
}
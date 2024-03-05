using System.Collections.ObjectModel;
using AutoMapper;
using TestSystem.Dto;
using TestSystemViewer.Models;

namespace TestSystemViewer.Configuration;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<AnswerVariantModel, Answer>()
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
            .ReverseMap()
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
            .ForMember(dest => dest.IsSelected, opt => opt.MapFrom(src => false));

        CreateMap<QuestionModel, Question>()
            .ForMember(dest => dest.CorrectAnswerNumber, opt => opt.MapFrom(src => src.CorrectVariantNumber))
            .ForMember(dest => dest.Answers,
                opt => opt.MapFrom((chooseOneCorrectAnswerQuestion, _, _, context) =>
                    CreateAnswers(chooseOneCorrectAnswerQuestion, context)))
            .ForMember(dest => dest.ConditionText, opt => opt.MapFrom(src => src.ConditionText))
            .ReverseMap()
            .ForMember(dest => dest.ConditionText, opt => opt.MapFrom(src => src.ConditionText))
            .ForMember(dest => dest.First, opt => opt.MapFrom((question, _) => CreateQuestionVariant(question, 0)))
            .ForMember(dest => dest.Second, opt => opt.MapFrom((question, _) => CreateQuestionVariant(question, 1)))
            .ForMember(dest => dest.Third, opt => opt.MapFrom((question, _) => CreateQuestionVariant(question, 2)))
            .ForMember(dest => dest.Fourth, opt => opt.MapFrom((question, _) => CreateQuestionVariant(question, 3)))
            .ForMember(dest => dest.CorrectVariantNumber, opt => opt.MapFrom(src => src.CorrectAnswerNumber));

        CreateMap<TestModel, Quiz>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Questions,
                opt => opt.MapFrom((test, _, _, context) => CreateQuestions(test, context)))
            .ReverseMap()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Questions,
                opt => opt.MapFrom((quiz, _, _, context) => CreateQuestions(quiz, context)))
            .ForMember(dest => dest.CorrectAnswers,
                opt => opt.MapFrom(src => CreateCorrectAnswers(src)));
    }

    private static Answer[] CreateAnswers(QuestionModel question, ResolutionContext context)
    {
        var mapper = context.Mapper;
        return
        [
            mapper.Map<Answer>(question.First),
            mapper.Map<Answer>(question.Second),
            mapper.Map<Answer>(question.Third),
            mapper.Map<Answer>(question.Fourth)
        ];
    }

    private static AnswerVariantModel CreateQuestionVariant(Question question, int index)
    {
        var answers = question.Answers;
        return new AnswerVariantModel
        {
            Text = answers[index].Text,
            IsSelected = question.CorrectAnswerNumber == index
        };
    }

    private static Question[] CreateQuestions(TestModel testModel, ResolutionContext context)
    {
        var mapper = context.Mapper;
        return testModel.Questions.Select(x => mapper.Map<Question>(x)).ToArray();
    }

    private static ObservableCollection<QuestionModel> CreateQuestions(Quiz quiz,
        ResolutionContext context)
    {
        var mapper = context.Mapper;
        return new ObservableCollection<QuestionModel>(
            quiz.Questions.Select(question => new QuestionModel
            {
                ConditionText = question.ConditionText,
                First = mapper.Map<AnswerVariantModel>(question.Answers[0]),
                Second = mapper.Map<AnswerVariantModel>(question.Answers[1]),
                Third = mapper.Map<AnswerVariantModel>(question.Answers[2]),
                Fourth = mapper.Map<AnswerVariantModel>(question.Answers[3]),
                CorrectVariantNumber = question.CorrectAnswerNumber
            }));
    }

    private static ObservableCollection<int> CreateCorrectAnswers(Quiz quiz)
    {
        return new ObservableCollection<int>(quiz.Questions.Select(q => q.CorrectAnswerNumber));
    }
}
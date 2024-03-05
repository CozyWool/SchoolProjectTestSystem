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
            .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => CreateAnswerModels(src)))
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
        return question.Answers.Select(x => mapper.Map<Answer>(x)).ToArray();
    }

    private static ObservableCollection<AnswerVariantModel> CreateAnswerModels(Question question)
    {
        return new ObservableCollection<AnswerVariantModel>(question.Answers.Select((x, index) => new AnswerVariantModel
        {
            Text = x.Text,
            Index = index + 1,
            IsSelected = question.CorrectAnswerNumber == index
        }));
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
                Answers = new ObservableCollection<AnswerVariantModel>(
                    question.Answers.Select(x => mapper.Map<AnswerVariantModel>(x)))
            }));
    }

    private static ObservableCollection<int> CreateCorrectAnswers(Quiz quiz)
    {
        return new ObservableCollection<int>(quiz.Questions.Select(q => q.CorrectAnswerNumber));
    }
}
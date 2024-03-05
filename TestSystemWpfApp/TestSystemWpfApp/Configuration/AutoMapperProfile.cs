using System.Collections.ObjectModel;
using AutoMapper;
using TestSystemClassLibrary.Models;
using TestSystemWpf.Dto;

namespace TestSystemWpfApp.Configuration;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<QuestionVariant, Answer>()
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
            .ReverseMap()
            .ForCtorParam(nameof(QuestionVariant.Text), opt => opt.MapFrom(src => src.Text))
            .ForCtorParam(nameof(QuestionVariant.IsCorrect), opt => opt.MapFrom(src => false));

        CreateMap<ChooseOneCorrectAnswerQuestion, Question>()
            .ForMember(dest => dest.CorrectAnswerNumber, opt => opt.MapFrom(src => src.CorrectVariantNumber))
            .ForMember(dest => dest.Answers,
                opt => opt.MapFrom((chooseOneCorrectAnswerQuestion, _, _, context) =>
                    CreateAnswers(chooseOneCorrectAnswerQuestion, context)))
            .ForMember(dest => dest.ConditionText, opt => opt.MapFrom(src => src.ConditionText))
            .ReverseMap()
            .ForCtorParam(nameof(ChooseOneCorrectAnswerQuestion.ConditionText),
                opt => opt.MapFrom(src => src.ConditionText))
            .ForCtorParam(nameof(ChooseOneCorrectAnswerQuestion.FirstVariant),
                opt => opt.MapFrom((question, _) => CreateQuestionVariant(question, 0)))
            .ForCtorParam(nameof(ChooseOneCorrectAnswerQuestion.SecondVariant),
                opt => opt.MapFrom((question, _) => CreateQuestionVariant(question, 1)))
            .ForCtorParam(nameof(ChooseOneCorrectAnswerQuestion.ThirdVariant),
                opt => opt.MapFrom((question, _) => CreateQuestionVariant(question, 2)))
            .ForCtorParam(nameof(ChooseOneCorrectAnswerQuestion.FourthVariant),
                opt => opt.MapFrom((question, _) => CreateQuestionVariant(question, 3)));

        CreateMap<Test, Quiz>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Questions,
                opt => opt.MapFrom((test, _, _, context) => CreateQuestions(test, context)))
            .ReverseMap()
            .ForCtorParam(nameof(Test.QuestionList), opt => opt.MapFrom(CreateQuestions))
            .ForCtorParam(nameof(Test.Name), opt => opt.MapFrom(src => src.Name))
            .ForCtorParam("isNameChanged", opt => opt.MapFrom(src => false));
    }

    private static Answer[] CreateAnswers(ChooseOneCorrectAnswerQuestion question, ResolutionContext context)
    {
        var mapper = context.Mapper;
        return
        [
            mapper.Map<Answer>(question.FirstVariant),
            mapper.Map<Answer>(question.SecondVariant),
            mapper.Map<Answer>(question.ThirdVariant),
            mapper.Map<Answer>(question.FourthVariant)
        ];
    }

    private static QuestionVariant CreateQuestionVariant(Question question, int index)
    {
        var answers = question.Answers;
        return new QuestionVariant
        {
            Text = answers[index].Text,
            IsCorrect = question.CorrectAnswerNumber == index
        };
    }

    private static Question[] CreateQuestions(Test test, ResolutionContext context)
    {
        var mapper = context.Mapper;
        return test.QuestionList.Select(x => mapper.Map<Question>(x)).ToArray();
    }

    private static ObservableCollection<ChooseOneCorrectAnswerQuestion> CreateQuestions(Quiz quiz,
        ResolutionContext context)
    {
        var mapper = context.Mapper;
        return new ObservableCollection<ChooseOneCorrectAnswerQuestion>(
            quiz.Questions.Select(question => mapper.Map<ChooseOneCorrectAnswerQuestion>(question)));
    }
}
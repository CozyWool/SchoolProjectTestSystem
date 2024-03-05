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
            .ForCtorParam(nameof(AnswerVariantModel.Text), opt => opt.MapFrom(src => src.Text))
            .ForCtorParam(nameof(AnswerVariantModel.IsSelected), opt => opt.MapFrom(src => false));

        CreateMap<QuestionModel, Question>()
            .ForMember(dest => dest.CorrectAnswerNumber, opt => opt.MapFrom(src => src.CorrectVariantNumber))
            .ForMember(dest => dest.Answers,
                opt => opt.MapFrom((chooseOneCorrectAnswerQuestion, _, _, context) =>
                    CreateAnswers(chooseOneCorrectAnswerQuestion, context)))
            .ForMember(dest => dest.ConditionText, opt => opt.MapFrom(src => src.ConditionText))
            .ReverseMap()
            .ForCtorParam(nameof(QuestionModel.ConditionText),
                opt => opt.MapFrom(src => src.ConditionText))
            .ForCtorParam(nameof(QuestionModel.First),
                opt => opt.MapFrom((question, _) => CreateQuestionVariant(question, 0)))
            .ForCtorParam(nameof(QuestionModel.Second),
                opt => opt.MapFrom((question, _) => CreateQuestionVariant(question, 1)))
            .ForCtorParam(nameof(QuestionModel.Third),
                opt => opt.MapFrom((question, _) => CreateQuestionVariant(question, 2)))
            .ForCtorParam(nameof(QuestionModel.Fourth),
                opt => opt.MapFrom((question, _) => CreateQuestionVariant(question, 3)));

        CreateMap<TestModel, Quiz>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Questions,
                opt => opt.MapFrom((test, _, _, context) => CreateQuestions(test, context)))
            .ReverseMap()
            .ForCtorParam(nameof(TestModel.Questions), opt => opt.MapFrom(CreateQuestions))
            .ForCtorParam(nameof(TestModel.Name), opt => opt.MapFrom(src => src.Name))
            .ForCtorParam("isNameChanged", opt => opt.MapFrom(src => false));
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
            quiz.Questions.Select(question => mapper.Map<QuestionModel>(question)));
    }
}
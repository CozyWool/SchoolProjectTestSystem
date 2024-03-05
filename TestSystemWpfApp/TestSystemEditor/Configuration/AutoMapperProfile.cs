using System.Collections.ObjectModel;
using AutoMapper;
using TestSystem.Dto;
using TestSystemEditor.Models;

namespace TestSystemEditor.Configuration;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<AnswerModel, Answer>()
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
            .ReverseMap();

        CreateMap<QuestionModel, Question>()
            .ForMember(dest => dest.CorrectAnswerNumber, opt => opt.MapFrom(src => src.CorrectAnswerNumber))
            .ForMember(dest => dest.Answers,
                opt => opt.MapFrom((questionModel, _, _, context) =>
                    CreateAnswers(questionModel, context)))
            .ForMember(dest => dest.ConditionText, opt => opt.MapFrom(src => src.ConditionText))
            .ReverseMap()
            .ForMember(dest => dest.CorrectAnswerNumber, opt => opt.MapFrom(src => src.CorrectAnswerNumber))
            .ForMember(dest => dest.ConditionText, opt => opt.MapFrom(src => src.ConditionText))
            .ForMember(dest => dest.Answers, opt => opt.MapFrom((q, _, _, context) => CreateAnswerModels(q, context)));

        CreateMap<QuizModel, Quiz>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Questions,
                opt => opt.MapFrom((quizModel, _, _, context) => CreateQuestions(quizModel, context)))
            .ReverseMap()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Questions,
                opt => opt.MapFrom((quiz, _, _, context) => CreateQuestionModels(quiz, context)));
    }

    private static Answer[] CreateAnswers(QuestionModel questionModel, ResolutionContext context)
    {
        var mapper = context.Mapper;
        return questionModel.Answers.Select(x => mapper.Map<Answer>(x)).ToArray();
    }

    private static ObservableCollection<AnswerModel> CreateAnswerModels(Question question, ResolutionContext context)
    {
        var mapper = context.Mapper;
        return new ObservableCollection<AnswerModel>(question.Answers.Select((x, index) =>
        {
            var answerModel = mapper.Map<AnswerModel>(x);
            answerModel.Index = index + 1;
            answerModel.IsSelected = question.CorrectAnswerNumber == answerModel.Index;
            return answerModel;
        }));
    }

    private static Question[] CreateQuestions(QuizModel quizModel, ResolutionContext context)
    {
        var mapper = context.Mapper;
        return quizModel.Questions.Select(x => mapper.Map<Question>(x)).ToArray();
    }

    private static ObservableCollection<QuestionModel> CreateQuestionModels(Quiz quiz, ResolutionContext context)
    {
        var mapper = context.Mapper;
        return new ObservableCollection<QuestionModel>(quiz.Questions.Select(x => mapper.Map<QuestionModel>(x)));
    }
}
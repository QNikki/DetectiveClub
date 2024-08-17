using AutoMapper;
using DetectiveClub.Data;
using DetectiveClub.Business.Contracts;

namespace DetectiveClub.Business;

internal class QuestionService(
    IRepository<Question> questions,
    IRepository<EnvironmentQuestion> environmentQuestions,
    IRepository<QuestionType> questionTypes) : IQuestionService
{
    public StatusResult<ServiceStatus, int> AddQuestion(QuestionDto newQuestion)
    {
        var isValid = ValidateQuestion(newQuestion);
        if (!isValid.Status)
        {
            return new StatusResult<ServiceStatus, int>(isValid.Result, -1);
        }

        var alreadyExist = questions.GetAll()
            .Any(question => question.TypeId == newQuestion.TypeId && question.Сontent == newQuestion.Content);

        if (alreadyExist)
        {
            return new StatusResult<ServiceStatus, int>(ServiceStatus.AlreadyExist, -1);
        }

        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<QuestionDto, Question>())
            .CreateMapper();

        var question = mapper.Map<Question>(newQuestion);
        var questionId = questions.Add(question);
        return new StatusResult<ServiceStatus, int>(ServiceStatus.Success, questionId);
    }

    public ServiceStatus RemoveQuestion(int questionId)
    {
        var question = questions.GetById(questionId);
        if (question == null)
        {
            return ServiceStatus.NotFound;
        }

        var questionsEnv = environmentQuestions.GetList(x => x.QuestionId == questionId)
            .ToList();

        foreach (var questionEnv in questionsEnv)
        {
            environmentQuestions.Delete(questionEnv);
        }

        questions.Delete(question);
        return ServiceStatus.Success;
    }

    public ServiceStatus EditQuestion(QuestionDto toEditQuestion)
    {
        var question = questions.GetById(toEditQuestion.Id);
        if (question == null)
        {
            return ServiceStatus.NotFound;
        }

        if (!IsTypeValid(toEditQuestion.TypeId))
        {
            return ServiceStatus.WrongType;
        }


        questions.Edit(question);
        return ServiceStatus.Success;
    }

    public QuestionDto? GetQuestion(int questionId)
    {
        var question = questions.GetById(questionId);
        if (question is null)
        {
            return null;
        }

        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Question, QuestionDto>())
            .CreateMapper();

        return mapper.Map<QuestionDto>(question);
    }

    public StatusResult<ServiceStatus, IEnumerable<QuestionDto>> GetQuestionsByType(int typeId)
    {
        if (!IsTypeValid(typeId))
        {
            return new StatusResult<ServiceStatus, IEnumerable<QuestionDto>>(ServiceStatus.WrongType,
                new List<QuestionDto>());
        }

        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<List<Question>, List<QuestionDto>>())
            .CreateMapper();

        var questionsByType = questions.GetList(question => question.TypeId == typeId);
        var mappedQuestionsByType = mapper.Map<List<QuestionDto>>(questionsByType);
        return new StatusResult<ServiceStatus, IEnumerable<QuestionDto>>(ServiceStatus.Success, mappedQuestionsByType);
    }

    public StatusResult<ServiceStatus, int> AddQuestionType(QuestionTypeDto newQuestionType)
    {
        var alreadyExist = questionTypes.GetAll()
            .Any(questionType => questionType.Name == newQuestionType.Name);

        if (alreadyExist)
        {
            return new StatusResult<ServiceStatus, int>(ServiceStatus.AlreadyExist, -1);
        }

        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<QuestionTypeDto, QuestionType>())
            .CreateMapper();

        var questionType = mapper.Map<QuestionType>(newQuestionType);
        var secretId = questionTypes.Add(questionType);
        return new StatusResult<ServiceStatus, int>(ServiceStatus.Success, secretId);
    }

    public ServiceStatus RemoveQuestionType(int questionTypeId)
    {
        var questionType = questionTypes.GetById(questionTypeId);
        if (questionType == null)
        {
            return ServiceStatus.NotFound;
        }

        var questionsByType = GetQuestionsByType(questionTypeId).Result.ToList();
        var envQuestions = environmentQuestions.GetAll().ToList();

        //Delete question environment data 
        foreach (var envQuestion in envQuestions.Where(envQuestion => questionsByType.Any(x => x.Id == envQuestion.Id)))
        {
            environmentQuestions.Delete(envQuestion);
        }

        //Delete question  data 
        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<List<QuestionDto>, List<Question>>())
            .CreateMapper();

        var mappedQuestionsByType = mapper.Map<List<Question>>(questionsByType);
        foreach (var question in mappedQuestionsByType)
        {
            questions.Delete(question);
        }

        questionTypes.Delete(questionType);
        return ServiceStatus.Success;
    }

    public ServiceStatus EditQuestionType(QuestionTypeDto toEditQuestionType)
    {
        var questionType = questionTypes.GetById(toEditQuestionType.Id);
        if (questionType == null)
        {
            return ServiceStatus.NotFound;
        }

        questionTypes.Edit(questionType);
        return ServiceStatus.Success;
    }

    public StatusResult<ServiceStatus, QuestionTypeDto> GetQuestionType(int questionTypeId)
    {
        var questionType = questionTypes.GetById(questionTypeId);
        if (questionType == null)
        {
            return new(ServiceStatus.NotFound, new QuestionTypeDto(-1, ""));
        }

        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<QuestionType, QuestionTypeDto>()).CreateMapper();
        return new(ServiceStatus.Success, mapper.Map<QuestionTypeDto>(questionType));
    }

    public IEnumerable<QuestionTypeDto> GetQuestionTypes()
    {
        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<List<QuestionType>, List<QuestionTypeDto>>())
            .CreateMapper();

        return mapper.Map<List<QuestionTypeDto>>(questionTypes.GetAll());
    }


    private StatusResult<bool, ServiceStatus> ValidateQuestion(QuestionDto target)
    {
        return IsTypeValid(target.TypeId)
            ? new StatusResult<bool, ServiceStatus>(true, ServiceStatus.Success)
            : new StatusResult<bool, ServiceStatus>(false, ServiceStatus.WrongType);
    }

    private bool IsTypeValid(int typeId) => questionTypes.GetById(typeId) is not null;
}
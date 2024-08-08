using DetectiveClub.Data;
using Environment = DetectiveClub.Data.Environment;

namespace DetectiveClub.Business;

internal class QuestionService(
    IRepository<Question> questions,
    IRepository<Environment> environments,
    IRepository<EnvironmentQuestion> environmentQuestions,
    IRepository<QuestionType> questionTypes) : IQuestionService
{
    public StatusResult<ServiceStatus, int> AddQuestion(Question newQuestion)
    {
        var isValid = ValidateQuestion(newQuestion);
        if (!isValid.Status)
        {
            return new StatusResult<ServiceStatus, int>(isValid.Result, -1);
        }

        var alreadyExist = questions.GetAll()
            .Any(secret => secret.TypeId == newQuestion.TypeId && secret.Сontent == newQuestion.Сontent);

        if (alreadyExist)
        {
            return new StatusResult<ServiceStatus, int>(ServiceStatus.AlreadyExist, -1);
        }

        var secretId = questions.Add(newQuestion);
        return new StatusResult<ServiceStatus, int>(ServiceStatus.Success, secretId);
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

    public ServiceStatus EditQuestion(Question toEditQuestion)
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

    public Question? GetQuestion(int questionId) => questions.GetById(questionId);

    public StatusResult<ServiceStatus, IEnumerable<Question>> GetQuestionsByType(int typeId)
    {
        if (!IsTypeValid(typeId))
        {
            return new StatusResult<ServiceStatus, IEnumerable<Question>>(ServiceStatus.WrongType, default);
        }

        return new StatusResult<ServiceStatus, IEnumerable<Question>>(ServiceStatus.Success,
            questions.GetList(question => question.TypeId == typeId));
    }

    public StatusResult<ServiceStatus, IEnumerable<Question>> GetQuestionByEnvironment(int environmentId)
    {
        if (!IsEnvironmentValid(environmentId))
        {
            return new StatusResult<ServiceStatus, IEnumerable<Question>>(ServiceStatus.WrongEnvironment, default);
        }

        var environmentQuestion =
            environmentQuestions.GetList(envQuestion => envQuestion.EnvironmentId == environmentId)
                .Select(x => x.QuestionId)
                .ToList();

        return new StatusResult<ServiceStatus, IEnumerable<Question>>(ServiceStatus.WrongEnvironment,
            questions.GetList(question => environmentQuestion.Contains(question.Id)));
    }

    public StatusResult<ServiceStatus, int> AddQuestionType(QuestionType newQuestionType)
    {
        var alreadyExist = questionTypes.GetAll()
            .Any(questionType => questionType.Name == newQuestionType.Name);

        if (alreadyExist)
        {
            return new StatusResult<ServiceStatus, int>(ServiceStatus.AlreadyExist, -1);
        }

        var secretId = questionTypes.Add(newQuestionType);
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
        foreach (var envQuestion in envQuestions)
        {
            if (questionsByType.Any(x => x.Id == envQuestion.Id))
            {
                environmentQuestions.Delete(envQuestion);
            }
        }
        
        //Delete question  data 
        foreach (var question in questionsByType)
        {
            questions.Delete(question);
        }

        questionTypes.Delete(questionType);
        return ServiceStatus.Success;
    }

    public ServiceStatus EditQuestionType(QuestionType toEditQuestionType)
    {
        var questionType = questionTypes.GetById(toEditQuestionType.Id);
        if (questionType == null)
        {
            return ServiceStatus.NotFound;
        }

        questionTypes.Edit(questionType);
        return ServiceStatus.Success;
    }

    public StatusResult<ServiceStatus, QuestionType> GetQuestionType(int questionTypeId)
    {
        var questionType = questionTypes.GetById(questionTypeId);
        var status = questionType is null ? ServiceStatus.NotFound : ServiceStatus.Success;
        return new(status, questionType);
    }

    public IEnumerable<QuestionType> GetQuestionTypes() => questionTypes.GetAll();
    public StatusResult<ServiceStatus, int> AddEnvironmentToQuestion(int questionId, int environmentId)
    {
        throw new NotImplementedException();
    }

    public ServiceStatus RemoveEnvironmentFromQuestion(int questionId, int environmentId)
    {
        throw new NotImplementedException();
    }

    private StatusResult<bool, ServiceStatus> ValidateQuestion(Question target)
    {
        return IsTypeValid(target.TypeId)
            ? new StatusResult<bool, ServiceStatus>(true, ServiceStatus.Success)
            : new StatusResult<bool, ServiceStatus>(false, ServiceStatus.WrongType);
    }


    private bool IsEnvironmentValid(int environmentId) => environments.GetById(environmentId) is not null;

    private bool IsTypeValid(int typeId) => questionTypes.GetById(typeId) is not null;
}
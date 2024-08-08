using DetectiveClub.Data;
using Environment = DetectiveClub.Data.Environment;

namespace DetectiveClub.Business;

public interface IGameService
{
}

public record StatusResult<TStatus, TResult>(TStatus Status, TResult Result);

internal class QuestionService(
    IRepository<Question> questions,
    IRepository<Environment> environments,
    IRepository<EnvironmentQuestion> environmentQuestions,
    IRepository<QuestionType> questionTypes) : IGameService
{
    public IEnumerable<QuestionType> GetQuestionTypes() => questionTypes.GetAll();

    public IEnumerable<Environment> GetEnvironments() => environments.GetAll();

    public IEnumerable<Question> GetQuestions(int questionTypeId, bool useEnvironmentQuestions = true)
    {
        var result = questions.GetList(q => q.TypeId == questionTypeId);
        if (useEnvironmentQuestions)
        {
            return result;
        }

        var environmentQuestionsId = environmentQuestions.GetAll().Select(x => x.QuestionId);
        return result.Where(x => !environmentQuestionsId.Contains(x.Id));
    }

    public StatusResult<ServiceStatus, int> AddEnvironment(Environment newEnvironment)
    {
        var alreadyExist = environments.GetAll()
            .Any(environment => environment.Name == newEnvironment.Name);

        if (alreadyExist)
        {
            return new StatusResult<ServiceStatus, int>(ServiceStatus.AlreadyExist, -1);
        }

        var environmentId = environments.Add(newEnvironment);
        return new StatusResult<ServiceStatus, int>(ServiceStatus.Success, environmentId);
    }

    public StatusResult<ServiceStatus, int> AddQuestion(Question newQuestion)
    {
        var alreadyExist = questions.GetAll()
            .Any(question => question.TypeId == newQuestion.TypeId &&
                             question.Сontent == newQuestion.Сontent);

        if (alreadyExist)
        {
            return new StatusResult<ServiceStatus, int>(ServiceStatus.AlreadyExist, -1);
        }

        var questionId = questions.Add(newQuestion);
        return new StatusResult<ServiceStatus, int>(ServiceStatus.Success, questionId);
    }

    public StatusResult<ServiceStatus, int> AddQuestionType(QuestionType newQuestionType)
    {
        var alreadyExist = questionTypes.GetAll()
            .Any(questionType => questionType.Name == newQuestionType.Name);

        if (alreadyExist)
        {
            return new StatusResult<ServiceStatus, int>(ServiceStatus.AlreadyExist, -1);
        }

        var questionId = questionTypes.Add(newQuestionType);
        return new StatusResult<ServiceStatus, int>(ServiceStatus.Success, questionId);
    }

    public StatusResult<ServiceStatus, int> AddEnvironmentToQuestion(EnvironmentQuestion newEnvironmentQuestion)
    {
        var alreadyExist = environmentQuestions.GetAll().Any(envQ =>
            envQ.QuestionId == newEnvironmentQuestion.QuestionId &&
            envQ.EnvironmentId == newEnvironmentQuestion.EnvironmentId);

        if (alreadyExist)
        {
            return new StatusResult<ServiceStatus, int>(ServiceStatus.AlreadyExist, -1);
        }

        var environmentQuestionId = environmentQuestions.Add(newEnvironmentQuestion);
        return new StatusResult<ServiceStatus, int>(ServiceStatus.Success, environmentQuestionId);
    }
}
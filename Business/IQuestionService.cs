using DetectiveClub.Data;

namespace DetectiveClub.Business;

public interface IQuestionService
{
    public StatusResult<ServiceStatus, int> AddQuestion(Question newQuestion);
    public ServiceStatus RemoveQuestion(int questionId);

    public ServiceStatus EditQuestion(Question toEditQuestion);

    public Question? GetQuestion(int questionId);

    public StatusResult<ServiceStatus, IEnumerable<Question>> GetQuestionsByType(int typeId);

    public StatusResult<ServiceStatus, IEnumerable<Question>> GetQuestionByEnvironment(int environmentId);

    public StatusResult<ServiceStatus, int> AddQuestionType(QuestionType newQuestionType);

    public ServiceStatus RemoveQuestionType(int questionTypeId);

    public ServiceStatus EditQuestionType(QuestionType toEditQuestionType);

    public StatusResult<ServiceStatus, QuestionType> GetQuestionType(int questionTypeId);

    public IEnumerable<QuestionType> GetQuestionTypes();

    public StatusResult<ServiceStatus, int> AddEnvironmentToQuestion(int questionId, int environmentId);
    public ServiceStatus RemoveEnvironmentFromQuestion(int questionId, int environmentId);
}
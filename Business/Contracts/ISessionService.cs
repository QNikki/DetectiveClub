
namespace DetectiveClub.Business.Contracts;

public interface ISessionService
{
    public StatusResult<ServiceStatus, int> AddSession(SessionDto session);

    public ServiceStatus RemoveSession(int sessionId);

    ServiceStatus EditSession(SessionDto session);

    public SessionDto? GetSession(int sessionId);

    public ServiceStatus AddSessionQuestions(int sessionId, IEnumerable<QuestionDto> questions);

    public ServiceStatus EditSessionQuestion(int sessionId, int previousQuestionId, int newQuestionId);

    public IEnumerable<QuestionDto>? GetSessionQuestions(int sessionId);
}
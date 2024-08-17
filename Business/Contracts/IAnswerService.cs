namespace DetectiveClub.Business.Contracts;

public interface IAnswerService
{
    public StatusResult<ServiceStatus, int> AddAnswer(AnswerDto answerDto);

    public ServiceStatus RemoveAnswer(int answerId);

    ServiceStatus EditAnswer(int answerId, string content);

    public AnswerDto? GetAnswer(int answerId);

    public IEnumerable<AnswerDto> GetAnswersByQuestion(int sessionId, int questionId);

    public IEnumerable<AnswerDto> GetAnswersByCharacter(int charId);
}

namespace DetectiveClub.Business.Contracts;

public interface IQuestionService
{
    public StatusResult<ServiceStatus, int> AddQuestion(QuestionDto newQuestion);
    public ServiceStatus RemoveQuestion(int questionId);

    public ServiceStatus EditQuestion(QuestionDto toEditQuestion);

    public QuestionDto? GetQuestion(int questionId);

    public StatusResult<ServiceStatus, IEnumerable<QuestionDto>> GetQuestionsByType(int typeId);

    public StatusResult<ServiceStatus, int> AddQuestionType(QuestionTypeDto newQuestionType);

    public ServiceStatus RemoveQuestionType(int questionTypeId);

    public ServiceStatus EditQuestionType(QuestionTypeDto toEditQuestionType);

    public StatusResult<ServiceStatus, QuestionTypeDto> GetQuestionType(int questionTypeId);

    public IEnumerable<QuestionTypeDto> GetQuestionTypes();
    
}
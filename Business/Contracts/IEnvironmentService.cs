
namespace DetectiveClub.Business.Contracts;

public interface IEnvironmentService
{
    StatusResult<ServiceStatus, int> CreateEnvironment(EnvironmentDto environmentDto);
    ServiceStatus DeleteEnvironment(int environment);
    ServiceStatus EditEnvironment(EnvironmentDto environmentDto);
    EnvironmentDto? GetEnvironmentById(int environmentId);
    IEnumerable<EnvironmentDto> GetAllEnvironments();

    public StatusResult<ServiceStatus, int> AddEnvironmentToQuestion(QuestionDto question, EnvironmentDto environment);

    public ServiceStatus RemoveEnvironmentFromQuestion(QuestionDto question, EnvironmentDto environment);

    public StatusResult<ServiceStatus, IEnumerable<QuestionDto>> GetQuestionByEnvironment(int environmentId);
}
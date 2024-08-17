using AutoMapper;
using DetectiveClub.Data;
using DetectiveClub.Business.Contracts;
using Environment = DetectiveClub.Data.Environment;

namespace DetectiveClub.Business;

internal class EnvironmentService(
    IRepository<Environment> environments,
    IRepository<EnvironmentQuestion> environmentQuestions,
    IRepository<Question> questions) : IEnvironmentService
{
    public StatusResult<ServiceStatus, int> CreateEnvironment(EnvironmentDto environmentDto)
    {
        if (environments.GetAll().Any(x => x.Name == environmentDto.Name))
        {
            return new(ServiceStatus.AlreadyExist, -1);
        }

        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<EnvironmentDto, Environment>()).CreateMapper();
        var environment = mapper.Map<Environment>(environmentDto);
        return new(ServiceStatus.Success, environments.Add(environment));
    }

    public ServiceStatus DeleteEnvironment(int environmentId)
    {
        var environmentEntity = environments.GetById(environmentId);
        if (environmentEntity is null)
        {
            return ServiceStatus.NotFound;
        }

        var environmentQuestionsToDelete = environmentQuestions
            .GetList(x => x.EnvironmentId == environmentId)
            .ToList();

        foreach (var environmentQuestion in environmentQuestionsToDelete)
        {
            environmentQuestions.Delete(environmentQuestion);
        }

        environments.Delete(environmentEntity);
        return ServiceStatus.Success;
    }

    public ServiceStatus EditEnvironment(EnvironmentDto environmentDto)
    {
        // Validate env name 
        var environmentEntity = environments.GetById(environmentDto.Id);
        if (environmentEntity is null)
        {
            return ServiceStatus.NotFound;
        }

        environmentEntity.Name = environmentDto.Name;
        environments.Edit(environmentEntity);
        return ServiceStatus.Success;
    }

    public EnvironmentDto? GetEnvironmentById(int environmentId)
    {
        var environmentEntity = environments.GetById(environmentId);
        if (environmentEntity is null)
        {
            return null;
        }

        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Environment, EnvironmentDto>()).CreateMapper();
        return mapper.Map<EnvironmentDto>(environmentEntity);
    }

    public IEnumerable<EnvironmentDto> GetAllEnvironments()
    {
        var environmentEntities = environments.GetAll();
        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<List<Environment>, List<EnvironmentDto>>())
            .CreateMapper();
        return mapper.Map<List<EnvironmentDto>>(environmentEntities);
    }

    public StatusResult<ServiceStatus, int> AddEnvironmentToQuestion(QuestionDto question, EnvironmentDto environment)
    {
        if (!IsEnvironmentValid(environment.Id))
        {
            return new StatusResult<ServiceStatus, int>(ServiceStatus.WrongEnvironment, -1);
        }
        else if (!IsQuestionValid(question.Id))
        {
            return new StatusResult<ServiceStatus, int>(ServiceStatus.NotFound, -1);
        }

        var index = environmentQuestions.Add(new EnvironmentQuestion
            { EnvironmentId = environment.Id, QuestionId = question.Id });

        return new StatusResult<ServiceStatus, int>(ServiceStatus.Success, index);
    }

    public ServiceStatus RemoveEnvironmentFromQuestion(QuestionDto question, EnvironmentDto environment)
    {
        var maybeEnvironmentQuestion = environmentQuestions.GetAll()
            .FirstOrDefault(envQ =>
                envQ.QuestionId == question.Id && envQ.EnvironmentId == environment.Id);

        if (maybeEnvironmentQuestion == default)
        {
            return ServiceStatus.NotFound;
        }

        environmentQuestions.Delete(maybeEnvironmentQuestion);
        return ServiceStatus.Success;
    }

    public StatusResult<ServiceStatus, IEnumerable<QuestionDto>> GetQuestionByEnvironment(int environmentId)
    {
        if (!IsEnvironmentValid(environmentId))
        {
            return new StatusResult<ServiceStatus, IEnumerable<QuestionDto>>(ServiceStatus.WrongEnvironment,
                new List<QuestionDto>());
        }

        var environmentQuestion =
            environmentQuestions.GetList(envQuestion => envQuestion.EnvironmentId == environmentId)
                .Select(x => x.QuestionId)
                .ToList();

        var questionsByEnvironment = questions.GetList(question => environmentQuestion.Contains(question.Id)).ToList();
        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<List<Question>, List<QuestionDto>>()).CreateMapper();
        var mappedResult = mapper.Map<List<QuestionDto>>(questionsByEnvironment);
        return new StatusResult<ServiceStatus, IEnumerable<QuestionDto>>(ServiceStatus.WrongEnvironment, mappedResult);
    }

    private bool IsEnvironmentValid(int environmentId) => environments.GetById(environmentId) is not null;

    private bool IsQuestionValid(int questionId) => questions.GetById(questionId) is not null;
}
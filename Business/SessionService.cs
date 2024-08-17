using AutoMapper;
using DetectiveClub.Business.Contracts;
using DetectiveClub.Data;
using Environment = DetectiveClub.Data.Environment;

namespace DetectiveClub.Business;

internal class SessionService(
    IRepository<Session> sessions,
    IRepository<Question> questions,
    IRepository<SessionQuestion> sessionQuestions,
    IRepository<GameMode> gameModes,
    IRepository<Environment> environments,
    IRepository<Character> characters) : ISessionService
{
    public StatusResult<ServiceStatus, int> AddSession(SessionDto session)
    {
        var validateStatus = ValidateSession(session);
        if (validateStatus is not ServiceStatus.Success)
        {
            return new(validateStatus, -1);
        }

        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<SessionDto, Session>()).CreateMapper();
        var sessionToAdd = mapper.Map<Session>(session);
        var sessionId = sessions.Add(sessionToAdd);
        return new(ServiceStatus.Success, sessionId);
    }

    public ServiceStatus RemoveSession(int sessionId)
    {
        var session = sessions.GetById(sessionId);
        if (session is null)
        {
            return ServiceStatus.NotFound;
        }

        foreach (var sessionQuestion in sessionQuestions
                     .GetList(sessionQuestion => sessionQuestion.SessionId == sessionId).ToList())
        {
            sessionQuestions.Delete(sessionQuestion);
        }

        foreach (var character in characters.GetList(character => character.SessionId == sessionId).ToList())
        {
            characters.Delete(character);
        }

        sessions.Delete(session);
        return ServiceStatus.Success;
    }

    public ServiceStatus EditSession(SessionDto session)
    {
        var status = ValidateSession(session);
        if (status is not ServiceStatus.Success)
        {
            return ServiceStatus.IncorrectData;
        }

        var sessionToEdit = sessions.GetById(session.Id);
        if (sessionToEdit is null)
        {
            return ServiceStatus.NotFound;
        }

        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<SessionDto, Session>()).CreateMapper();
        sessions.Edit(mapper.Map<Session>(session));
        return ServiceStatus.Success;
    }

    public SessionDto? GetSession(int sessionId)
    {
        var session = sessions.GetById(sessionId);
        if (session is null)
        {
            return null;
        }

        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Session, SessionDto>()).CreateMapper();
        return mapper.Map<SessionDto>(session);
    }

    public ServiceStatus AddSessionQuestions(int sessionId, IEnumerable<QuestionDto> questionsDto)
    {
        var session = sessions.GetById(sessionId);
        if (session is null)
        {
            return ServiceStatus.NotFound;
        }

        var questionIds = questionsDto.Select(questionDto => questionDto.Id).ToList();
        var questionsExistCount = questions.GetList(question => questionIds.Contains(question.Id)).Count();
        if (questionIds.Count != questionsExistCount)
        {
            return ServiceStatus.IncorrectData;
        }

        var targetSessionQuestions = sessionQuestions
            .GetList(sessionQuestion => sessionQuestion.SessionId == sessionId)
            .ToList();

        if (targetSessionQuestions.Any(sessionQuestion => questionIds.Contains(sessionQuestion.Id)))
        {
            return ServiceStatus.AlreadyExist;
        }

        foreach (var questionId in questionIds)
        {
            sessionQuestions.Add(new SessionQuestion { QuestionId = questionId, SessionId = sessionId });
        }

        return ServiceStatus.Success;
    }

    public ServiceStatus EditSessionQuestion(int sessionId, int previousQuestionId, int newQuestionId)
    {
        var session = sessions.GetById(sessionId);
        if (session is null)
        {
            return ServiceStatus.NotFound;
        }

        var targetSessionQuestions = sessionQuestions
            .GetList(sessionQuestion => sessionQuestion.SessionId == sessionId)
            .ToList();

        var targetSessionQuestion = targetSessionQuestions
            .FirstOrDefault(sessionQuestion => sessionQuestion.QuestionId == previousQuestionId);

        if (targetSessionQuestion is null)
        {
            return ServiceStatus.NotFound;
        }

        if (questions.GetById(newQuestionId) is null)
        {
            return ServiceStatus.NotFound;
        }

        targetSessionQuestion.QuestionId = newQuestionId;
        sessionQuestions.Edit(targetSessionQuestion);
        return ServiceStatus.Success;
    }

    public IEnumerable<QuestionDto>? GetSessionQuestions(int sessionId)
    {
        var session = sessions.GetById(sessionId);
        if (session is null)
        {
            return null;
        }

        var targetQuestionIds = sessionQuestions
            .GetList(sessionQuestion => sessionQuestion.SessionId == sessionId)
            .Select(sessionQuestion => sessionQuestion.QuestionId)
            .ToList();


        var targetQuestions = questions.GetList(question => targetQuestionIds.Contains(question.Id)).ToList();
        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<List<Question>, List<QuestionDto>>()).CreateMapper();
        return mapper.Map<List<QuestionDto>>(targetQuestions);
    }

    private ServiceStatus ValidateSession(SessionDto session)
    {
        if (environments.GetById(session.EnvironmentId) is null)
        {
            return ServiceStatus.NotFound;
        }

        if (gameModes.GetById(session.GameModeId) is null)
        {
            return ServiceStatus.NotFound;
        }

        return ServiceStatus.Success;
    }
}
using AutoMapper;
using DetectiveClub.Business.Contracts;
using DetectiveClub.Data;

namespace DetectiveClub.Business;

internal class AnswerService(
    IRepository<Answer> answers,
    IRepository<Question> questions,
    IRepository<Character> characters) : IAnswerService
{
    public StatusResult<ServiceStatus, int> AddAnswer(AnswerDto answerDto)
    {
        var validateStatus = ValidateAnswer(answerDto);
        if (validateStatus is not ServiceStatus.Success)
        {
            return new(validateStatus, -1);
        }

        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<AnswerDto, Answer>())
            .CreateMapper();

        var answer = mapper.Map<Answer>(answerDto);
        var answerId = answers.Add(answer);
        return new(ServiceStatus.Success, answerId);
    }

    public ServiceStatus RemoveAnswer(int answerId)
    {
        var answer = answers.GetById(answerId);
        if (answer is null)
        {
            return ServiceStatus.NotFound;
        }

        answers.Delete(answer);
        return ServiceStatus.Success;
    }

    public ServiceStatus EditAnswer(int answerId, string content)
    {
        var answer = answers.GetById(answerId);
        if (answer is null)
        {
            return ServiceStatus.NotFound;
        }

        // Validate string 
        if (content == "")
        {
            return ServiceStatus.IncorrectData;
        }

        answer.Content = content;
        answers.Edit(answer);
        return ServiceStatus.Success;
    }

    public AnswerDto? GetAnswer(int answerId)
    {
        var answer = answers.GetById(answerId);
        if (answer is null)
        {
            return null;
        }

        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Answer, AnswerDto>())
            .CreateMapper();

        var answerResult = mapper.Map<AnswerDto>(answer);
        return answerResult;
    }

    public IEnumerable<AnswerDto> GetAnswersByQuestion(int sessionId, int questionId)
    {
        var characterIdsInGame = characters.GetList(character => character.SessionId == sessionId)
            .Select(character => character.Id);

        var answersByQuestion = answers
            .GetList(answer => answer.QuestionId == questionId && characterIdsInGame.Contains(answer.CharacterId))
            .ToList();

        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<List<Answer>, List<AnswerDto>>())
            .CreateMapper();

        var result = mapper.Map<List<AnswerDto>>(answersByQuestion);
        return result;
    }

    public IEnumerable<AnswerDto> GetAnswersByCharacter(int charId)
    {
       var answersByChar = answers.GetList(answer => answer.CharacterId == charId).ToList();
        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<List<Answer>, List<AnswerDto>>())
            .CreateMapper();

        var result = mapper.Map<List<AnswerDto>>(answersByChar);
        return result;
    }

    private ServiceStatus ValidateAnswer(AnswerDto answerDto)
    {
        if (questions.GetById(answerDto.QuestionId) is null)
        {
            return ServiceStatus.NotFound;
        }

        if (characters.GetById(answerDto.CharacterId) is null)
        {
            return ServiceStatus.NotFound;
        }

        if (answerDto.Content == "")
        {
            return ServiceStatus.IncorrectData;
        }

        return ServiceStatus.Success;
    }
}
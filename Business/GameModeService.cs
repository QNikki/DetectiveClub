using AutoMapper;
using DetectiveClub.Data;
using DetectiveClub.Business.Contracts;

namespace DetectiveClub.Business;

internal class GameModeService(
    IRepository<GameMode> gameModes,
    IRepository<GameModeQuestionType> gameModeQuestions,
    IRepository<QuestionType> questionTypes,
    IRepository<GameModeSecretType> gameModeSecrets,
    IRepository<SecretType> secretTypes) : IGameModeService
{
    public StatusResult<ServiceStatus, int> CreateGameMode(GameModeDto gameModeDto)
    {
        if (!ValidateGameMode(gameModeDto))
        {
            return new(ServiceStatus.IncorrectData, -1);
        }

        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<GameModeDto, GameMode>()).CreateMapper();
        var gameMode = mapper.Map<GameMode>(gameModeDto);
        return new(ServiceStatus.Success, gameModes.Add(gameMode));
    }

    public ServiceStatus DeleteGameMode(int gameModeId)
    {
        var gameMode= gameModes.GetById(gameModeId);
        if (gameMode == default)
        {
            return ServiceStatus.NotFound;
        }

        var questionTypeDtos = GetQuestionTypes(gameModeId);
        foreach (var questionType in questionTypeDtos)
        {
            RemoveQuestionFromGameMode(gameModeId, questionType);
        }

        var secretTypeDtos = GetSecretTypes(gameModeId);
        foreach (var secretType in secretTypeDtos)
        {
            RemoveSecretFromGameMode(gameModeId, secretType);
        }
        
        gameModes.Delete(gameMode);
        return ServiceStatus.Success;
    }
    
    public ServiceStatus EditGameMode(GameModeDto gameMode)
    {
        var targetMode = gameModes.GetById(gameMode.Id);
        if (targetMode == default)
        {
            return ServiceStatus.NotFound;
        }

        if (!ValidateGameMode(gameMode))
        {
            return ServiceStatus.IncorrectData;
        }

        targetMode.MaxCharacterCount = gameMode.MaxCharacterCount;
        targetMode.Name = gameMode.Name;
        gameModes.Edit(targetMode);

        return ServiceStatus.Success;
    }


    public ServiceStatus AddQuestionToGameMode(int gameModeId, QuestionTypeDto questionType,
        int count)
    {
        var status = ValidateGameModeStuff(gameModeId, questionTypes, questionType.Id, count);
        if (status is ServiceStatus.Success)
        {
            gameModeQuestions.Add(new GameModeQuestionType
                { GameModeId = gameModeId, TypeId = questionType.Id, Count = count });
        }

        return ServiceStatus.Success;
    }

    public ServiceStatus AddSecretToGameMode(int gameModeId, SecretTypeDto secretType,
        int count)
    {
        var status = ValidateGameModeStuff(gameModeId, secretTypes, secretType.Id, count);
        if (status is ServiceStatus.Success)
        {
            gameModeSecrets.Add(new GameModeSecretType
                { GameModeId = gameModeId, TypeId = secretType.Id, Count = count });
        }

        return status;
    }

    public ServiceStatus RemoveQuestionFromGameMode(int gameModeId, QuestionTypeDto questionType) =>
        Remove(gameModeId, questionType.Id, gameModeQuestions);

    public ServiceStatus RemoveSecretFromGameMode(int gameModeId, SecretTypeDto secretType) =>
        Remove(gameModeId, secretType.Id, gameModeSecrets);

    public ServiceStatus EditCountGameModeSecret(int gameModeId, SecretTypeDto secretType, int newCount) =>
        EditCount(gameModeId, secretType.Id, gameModeSecrets, newCount);

    public ServiceStatus EditCountGameModeQuestion(int gameModeId, QuestionTypeDto questionType, int newCount) =>
        EditCount(gameModeId, questionType.Id, gameModeQuestions, newCount);

    public int GetQuestionTypeCount(int gameModeId, QuestionTypeDto questionType) =>
        GetCount(gameModeId, questionType.Id, gameModeQuestions);

    public int GetSecretTypeCount(int gameModeId, SecretTypeDto secretType) =>
        GetCount(gameModeId, secretType.Id, gameModeSecrets);

    public IEnumerable<SecretTypeDto> GetSecretTypes(int gameModeId)
    {
        var secretTypeIds = gameModeSecrets.GetList(x => x.GameModeId == gameModeId)
            .Select(x => x.TypeId);

        var modeSecretTypes = secretTypes.GetList(x => secretTypeIds.Contains(x.Id)).ToList();
        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<List<SecretType>, List<SecretTypeDto>>()).CreateMapper();
        return mapper.Map<List<SecretTypeDto>>(modeSecretTypes);
    }

    public IEnumerable<QuestionTypeDto> GetQuestionTypes(int gameModeId)
    {
        var questionsTypeIds = gameModeQuestions.GetList(x => x.GameModeId == gameModeId)
            .Select(x => x.TypeId);

        var modeQuestionsTypes = questionTypes.GetList(x => questionsTypeIds.Contains(x.Id)).ToList();
        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<List<QuestionType>, List<QuestionTypeDto>>()).CreateMapper();
        return mapper.Map<List<QuestionTypeDto>>(modeQuestionsTypes);
    }

    private bool ValidateGameMode(GameModeDto gameMode)
    {
        //TODO: add name validation
        if (gameMode.MaxCharacterCount <= 0)
        {
            return false;
        }

        return !gameModes.GetAll().Any(gm => gm.Name == gameMode.Name);
    }

    private ServiceStatus EditCount<T>(int gameModeId, int gameModeTypeId, IRepository<T> gameModeTypes, int newCount)
        where T : GameModeType
    {
        var stuffModeType = gameModeTypes.GetAll()
            .FirstOrDefault(gmqt => gmqt.GameModeId == gameModeId && gmqt.TypeId == gameModeTypeId);

        if (stuffModeType == default)
        {
            return ServiceStatus.NotFound;
        }

        if (newCount <= 0)
        {
            return ServiceStatus.IncorrectData;
        }

        stuffModeType.Count = newCount;
        gameModeTypes.Edit(stuffModeType);
        return ServiceStatus.Success;
    }

    private int GetCount<T>(int gameModeId, int typeId, IRepository<T> gameModeTypes) where T : GameModeType
    {
        var modeType = gameModeTypes.GetAll()
            .FirstOrDefault(gmqt => gmqt.GameModeId == gameModeId && gmqt.TypeId == typeId);

        if (modeType == default)
        {
            return -1;
        }

        return modeType.Count;
    }

    private ServiceStatus Remove<T>(int gameModeId, int typeId, IRepository<T> gameModeTypes) where T : GameModeType
    {
        var modeType = gameModeTypes.GetAll()
            .FirstOrDefault(gmqt => gmqt.GameModeId == gameModeId && gmqt.TypeId == typeId);

        if (modeType == default)
        {
            return ServiceStatus.NotFound;
        }

        gameModeTypes.Delete(modeType);
        return ServiceStatus.Success;
    }

    private ServiceStatus ValidateGameModeStuff<TStuff>(int gameModeId, IRepository<TStuff> stuffRepository,
        int stuffId,
        int count) where TStuff : IEntity<int>
    {
        if (stuffRepository.GetById(stuffId) is null)
        {
            return ServiceStatus.WrongType;
        }

        if (gameModes.GetById(gameModeId) == default)
        {
            return ServiceStatus.NotFound;
        }

        return count <= 0 ? ServiceStatus.IncorrectData : ServiceStatus.Success;
    }
}
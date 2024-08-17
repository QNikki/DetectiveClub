
namespace DetectiveClub.Business.Contracts;

public interface IGameModeService
{
    public StatusResult<ServiceStatus, int> CreateGameMode(GameModeDto gameModeDto);

    public ServiceStatus DeleteGameMode(int gameModeId);
    
    public ServiceStatus EditGameMode(GameModeDto gameMode);

    public ServiceStatus AddQuestionToGameMode(int gameModeId, QuestionTypeDto questionType, int count);

    public ServiceStatus AddSecretToGameMode(int gameModeId, SecretTypeDto secretType, int count);

    public ServiceStatus RemoveQuestionFromGameMode(int gameModeId, QuestionTypeDto questionType);

    public ServiceStatus RemoveSecretFromGameMode(int gameModeId, SecretTypeDto secretType);

    public ServiceStatus EditCountGameModeSecret(int gameModeId, SecretTypeDto secretType, int newCount);

    public ServiceStatus EditCountGameModeQuestion(int gameModeId, QuestionTypeDto questionType, int newCount);

    public int GetQuestionTypeCount(int gameModeId, QuestionTypeDto questionType);
    
    public int GetSecretTypeCount(int gameModeId, SecretTypeDto secretType);
    
    public IEnumerable<SecretTypeDto> GetSecretTypes(int gameModeId);
    
    public IEnumerable<QuestionTypeDto> GetQuestionTypes(int gameModeId);
}
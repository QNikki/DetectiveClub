namespace DetectiveClub.Business;

public record AnswerDto(int Id, int QuestionId, int CharacterId, string Content);

public record EnvironmentDto(int Id, string Name);
public record GameModeDto(int Id, string Name, int MaxCharacterCount);

public record GuessDto(int Id, int AnswerId , int CharacterId,  int SecretId);

public record QuestionDto(int Id, int TypeId , string Content);
public record QuestionTypeDto(int Id, string Name);

public record SecretDto(int Id,int TypeId, int EnvironmentId, string Сontent);

public record SecretTypeDto(int Id, string Name);

public record SessionDto(int Id, int GameModeId, int EnvironmentId, bool IsEnabledGuessScore, int HostId);

public record CharacterDto(int Id, int UserId, string Name);



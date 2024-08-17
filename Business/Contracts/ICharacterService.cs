
namespace DetectiveClub.Business.Contracts;

public interface ICharacterService
{
    public StatusResult<ServiceStatus, int> AddCharacter(CharacterDto character);

    public ServiceStatus AddCharacterSecret(int characterId, int secretId);

    public ServiceStatus EditCharacterSecret(int characterId, int previousSecretId, int newSecretId);

    public ServiceStatus RemoveCharacter(int characterId);

    ServiceStatus EditCharacter(int characterId, string name);

    public CharacterDto? GetCharacter(int characterId);

    public IEnumerable<SecretDto>? GetCharacterSecrets(int characterId);

    public IEnumerable<CharacterDto> GetCharactersBySession(int sessionId);
}
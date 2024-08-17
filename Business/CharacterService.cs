using AutoMapper;
using DetectiveClub.Business.Contracts;
using DetectiveClub.Data;

namespace DetectiveClub.Business;

internal class CharacterService(
    IRepository<Character> characters,
    IRepository<CharacterSecret> characterSecrets,
    IRepository<Secret> secrets,
    IRepository<Answer> answers,
    IRepository<Guess> guesses) : ICharacterService
{
    public StatusResult<ServiceStatus, int> AddCharacter(CharacterDto character)
    {
        var validateStatus = ValidateCharacter(character);
        if (validateStatus is not ServiceStatus.Success)
        {
            return new(validateStatus, -1);
        }

        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<CharacterDto, Character>())
            .CreateMapper();

        var newCharacter = mapper.Map<Character>(character);
        var charId = characters.Add(newCharacter);
        return new(ServiceStatus.Success, charId);
    }
    
    public ServiceStatus AddCharacterSecret(int characterId, int secretId)
    {
        var character = characters.GetById(characterId);
        var secret = secrets.GetById(secretId);
        if (secret is null || character is null)
        {
            return ServiceStatus.NotFound;
        }
        
        var alreadyExist = characterSecrets.GetAll()
            .Any(cs => cs.CharacterId == characterId && cs.Id == secretId);

        if (alreadyExist)
        {
            return ServiceStatus.AlreadyExist;
        }
        
        characterSecrets.Add(new CharacterSecret { CharacterId = characterId, SecretId = secretId });
        return ServiceStatus.Success;
    }

    public ServiceStatus EditCharacterSecret(int characterId, int previousSecretId, int newSecretId)
    {
        var character = characters.GetById(characterId);
        var previousSecret = secrets.GetById(previousSecretId);
        var newSecret = secrets.GetById(newSecretId);
        if (character is null || previousSecret is null || newSecret is null)
        {
            return ServiceStatus.NotFound;
        }

        var targetCharacterSecret = characterSecrets.GetAll()
            .FirstOrDefault(cs => cs.CharacterId == characterId && cs.SecretId == previousSecretId);

        if (targetCharacterSecret is null)
        {
            return ServiceStatus.NotFound;
        }

        targetCharacterSecret.SecretId = newSecretId;
        characterSecrets.Edit(targetCharacterSecret);
        return ServiceStatus.Success;
    }

    public ServiceStatus RemoveCharacter(int characterId)
    {
        var character = characters.GetById(characterId);
        if (character is null)
        {
            return ServiceStatus.NotFound;
        }
        
        foreach (var charSecret in characterSecrets.GetList(cs => cs.CharacterId == characterId).ToList())
        {
            characterSecrets.Delete(charSecret);
        }
        
        foreach (var charAnswer in answers.GetList(answer => answer.CharacterId == characterId).ToList())
        {
            answers.Delete(charAnswer);
        }
        
        foreach (var charGuess in guesses.GetList(guess => guess.CharacterOwnerId == characterId).ToList())
        {
            guesses.Delete(charGuess);
        }

        characters.Delete(character);
        return ServiceStatus.Success;
    }

    public ServiceStatus EditCharacter(int characterId, string name)
    {
        var character = characters.GetById(characterId);
        if (character is null)
        {
            return ServiceStatus.NotFound;
        }

        // TODO: better validation for strings;
        if (name is "")
        {
            return ServiceStatus.IncorrectData;
        }

        character.Name = name;
        characters.Edit(character);
        return ServiceStatus.Success;
    }

    public CharacterDto? GetCharacter(int characterId)
    {
        var character = characters.GetById(characterId);
        if (character is null)
        {
            return null;
        }

        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Character, CharacterDto>()).CreateMapper();
        var result = mapper.Map<CharacterDto>(character);
        return result;
    }

    public IEnumerable<SecretDto>? GetCharacterSecrets(int characterId)
    {
        var character = characters.GetById(characterId);
        if (character is null)
        {
            return null;
        }

        var charSecretIds = characterSecrets
            .GetList(characterSecret => characterSecret.CharacterId == characterId)
            .Select(characterSecret => characterSecret.SecretId)
            .ToList();

        var charSecrets = secrets.GetList(secret => charSecretIds.Contains(secret.Id)).ToList();
        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<List<Secret>, List<SecretDto>>())
            .CreateMapper();
        
        var result = mapper.Map<List<SecretDto>>(charSecrets);
        return result;
    }

    public IEnumerable<CharacterDto> GetCharactersBySession(int sessionId)
    {
        var targets=characters.GetList(character => character.SessionId == sessionId).ToList();
        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<List<Character>, List<CharacterDto>>())
            .CreateMapper();
        
        var result = mapper.Map<List<CharacterDto>>(targets);
        return result;
        
    }

    private ServiceStatus ValidateCharacter(CharacterDto character)
    {
        // TODO: better validation for strings 
        return character.Name != "" ? ServiceStatus.Success : ServiceStatus.IncorrectData;
    }
}
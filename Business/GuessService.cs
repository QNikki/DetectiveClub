using AutoMapper;
using DetectiveClub.Business.Contracts;
using DetectiveClub.Data;

namespace DetectiveClub.Business;

internal class GuessService(
    IRepository<Character> characters,
    IRepository<Guess> guesses,
    IRepository<Secret> secrets,
    IRepository<Answer> answers)
    : IGuessService
{
    public StatusResult<ServiceStatus, int> AddGuess(GuessDto newGuess)
    {
        var validateStatus = ValidateGuess(newGuess);
        if (validateStatus is not ServiceStatus.Success)
        {
            return new(validateStatus, -1);
        }

        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<GuessDto, Guess>())
            .CreateMapper();

        var guess = mapper.Map<Guess>(newGuess);
        var guessId = guesses.Add(guess);
        return new(ServiceStatus.Success, guessId);
    }

    public ServiceStatus RemoveGuess(int guessId)
    {
        var guess = guesses.GetById(guessId);
        if (guess is null)
        {
            return ServiceStatus.NotFound;
        }

        guesses.Delete(guess);
        return ServiceStatus.Success;
    }

    public ServiceStatus EditGuess(int guessId, int secretId)
    {
        var guess = guesses.GetById(guessId);
        if (guess is null)
        {
            return ServiceStatus.NotFound;
        }

        var secret = secrets.GetById(guessId);
        if (secret is null)
        {
            return ServiceStatus.IncorrectData;
        }

        guess.SecretId = secretId;
        guesses.Edit(guess);
        return ServiceStatus.Success;
    }

    public GuessDto? GetGuess(int guessId)
    {
        var guess = guesses.GetById(guessId);
        if (guess is null)
        {
            return null;
        }

        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Guess, GuessDto>())
            .CreateMapper();

        var guessResult = mapper.Map<GuessDto>(guess);
        return guessResult;
    }

    public IEnumerable<GuessDto> GetGuessesByCharacter(int charId)
    {
        var guessesByCharacter = guesses.GetList(guess => guess.CharacterOwnerId == charId).ToList();
        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<List<Guess>, List<GuessDto>>())
            .CreateMapper();

        var result = mapper.Map<List<GuessDto>>(guessesByCharacter);
        return result;
    }

    public IEnumerable<GuessDto> GetGuessesByBasis(GuessType guessType, int basisId)
    {
        var typeId = (int)guessType;
        var guessesBy = guesses.GetList(guess => guess.TypeId == typeId && guess.BasisId == basisId).ToList();
        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<List<Guess>, List<GuessDto>>())
            .CreateMapper();

        var result = mapper.Map<List<GuessDto>>(guessesBy);
        return result;
    }

    private ServiceStatus ValidateGuess(GuessDto guess)
    {
        IEntity<int>? targetBasis = guess.Type switch
        {
            GuessType.Answer => answers.GetById(guess.BasisId),
            GuessType.Character => characters.GetById(guess.BasisId),
            _ => throw new NotImplementedException("")
        };

        if (targetBasis is null)
        {
            return ServiceStatus.NotFound;
        }

        var isCharNotFound = characters.GetById(guess.CharacterId) is null;
        if (isCharNotFound)
        {
            return ServiceStatus.NotFound;
        }

        var isSecretNotFound = secrets.GetById(guess.SecretId) is null;
        return isSecretNotFound ? ServiceStatus.NotFound : ServiceStatus.Success;
    }
}

namespace DetectiveClub.Business.Contracts;

public interface IGuessService
{
    public StatusResult<ServiceStatus, int> AddGuess(GuessDto newGuess);
        
    public ServiceStatus RemoveGuess(int guessId);

    ServiceStatus EditGuess(int guessId, int secretId);

    public GuessDto? GetGuess(int guessId);

    public IEnumerable<GuessDto> GetGuessesByBasis(GuessType guessType, int basisId);

    public IEnumerable<GuessDto> GetGuessesByCharacter(int charId);
}
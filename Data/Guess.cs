namespace DetectiveClub.Data;

public record Guess : IEntity<int>
{
    public int Id { get; init; }

    public int GuessTypeId { get; set; }

    public int CharacterOwnerId {get; set; }

    public int BasisId { get; set; }

    public int SecretId { get; set; }
}
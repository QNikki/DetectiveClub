namespace DetectiveClub.Data;

public record CharacterSecret : IEntity<int>
{
    public int Id { get; init; }
    
    public int CharacterId { get; set; }

    public int SecretId { get; set; }

}
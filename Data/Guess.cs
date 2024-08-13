namespace DetectiveClub.Data;

public record Guess : IEntity<int>
{
    public int Id { get; init; }
    
    public int AnswerId { get; set; }
    
    public int CharacterId {get; set; }
  
    public int SecretId { get; set; }
}
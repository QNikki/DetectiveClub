namespace DetectiveClub.Data;

public record Answer : IEntity<int>
{
    public int Id { get; init; }

    public int QuestionId { get; set; }
    
    public int CharacterId {get; set; }
    
    public required string Content { get; set; }
}
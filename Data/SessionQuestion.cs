namespace DetectiveClub.Data;

public record SessionQuestion: IEntity<int>
{
    public int Id { get; init; }
    
    public int SessionId { get; set; }
    
    public int QuestionId { get; set; }
}
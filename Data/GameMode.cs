namespace DetectiveClub.Data;

public record GameMode: IEntity<int>
{
    public int Id { get; init; }
    
    public required string Name { get; set; }
    
    public int MaxCharacterCount { get; set; }
}
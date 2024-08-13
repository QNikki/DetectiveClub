namespace DetectiveClub.Data;

public record Character: IEntity<int>
{
    public int Id { get; init; }
    
    public int UserId { get; set; }

    public required string Name { get; set; }
}
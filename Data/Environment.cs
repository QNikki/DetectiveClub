namespace DetectiveClub.Data;

public record Environment : IEntity<int>
{
    public int Id { get; init; }

    public required string Name { get; set; }
}
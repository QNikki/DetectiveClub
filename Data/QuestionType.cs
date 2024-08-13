namespace DetectiveClub.Data;

public record QuestionType : IEntity<int>
{
    public int Id { get; init; }

    public required string Name { get; set; }
}
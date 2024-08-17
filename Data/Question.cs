namespace DetectiveClub.Data;

public record Question: IEntityWithType<int, int>
{
    public int Id { get; init; }

    public int TypeId { get; set; }

    public required string Сontent { get; set; }
}
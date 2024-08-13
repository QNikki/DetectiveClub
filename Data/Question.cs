namespace DetectiveClub.Data;

public record Question: ITypeEntity<int, int>
{
    public int Id { get; init; }

    public int TypeId { get; set; }

    public required string Сontent { get; set; }
}
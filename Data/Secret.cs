namespace DetectiveClub.Data;

public record Secret : ITypeEntity<int, int>
{
    public int Id { get; init; }
    
    public int TypeId { get; set; }

    public int EnvironmentId { get; set; }
    
    public required string Сontent { get; set; }

}
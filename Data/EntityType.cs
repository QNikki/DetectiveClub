namespace DetectiveClub.Data;

public abstract record EntityType<T> : IEntity<T>
{
    public T Id { get; init; }
    
    public required string Name { get; set; }
}